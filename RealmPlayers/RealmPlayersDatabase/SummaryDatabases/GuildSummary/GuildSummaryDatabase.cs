using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using Utility = VF_RealmPlayersDatabase.Utility;
using RPPDatabase = VF_RealmPlayersDatabase.Database;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;

namespace VF_RPDatabase
{
    [ProtoContract]
    public class GuildSummaryDatabase
    {
        [ProtoMember(1)]
        private Dictionary<string, GuildSummary> m_Guilds = new Dictionary<string, GuildSummary>();

        public GuildSummary GetGuildSummary(WowRealm _Realm, string _GuildName)
        {
            GuildSummary retValue = null;
            string realm = "" + (int)_Realm;
            if (realm.Length > 1)
            {
                if ((int)_Realm > 99)
                    throw new Exception("ERROR, REALM WAS INTENDED TO NEVER BE BIGGER THAN VALUE 99");
                realm = "R" + (int)_Realm;
            }
            if (m_Guilds.TryGetValue(realm + _GuildName, out retValue) == false)
                return null;

            return retValue;
        }
        private void AddGuildSummary(GuildSummary _GuildSummary)
        {
            string realm = "" + (int)_GuildSummary.Realm;
            if (realm.Length > 1)
            {
                if ((int)_GuildSummary.Realm > 99)
                    throw new Exception("ERROR, REALM WAS INTENDED TO NEVER BE BIGGER THAN VALUE 99");
                realm = "R" + (int)_GuildSummary.Realm;
            }
            m_Guilds.Add(realm + _GuildSummary.GuildName, _GuildSummary);
        }

        //public GuildSummary GetGuildSummary(string _GuildName)
        //{
        //    GuildSummary guildSummary = null;
        //    if(m_Guilds.TryGetValue(_GuildName, out guildSummary) == false)
        //        return null;

        //    return guildSummary;
        //}
        public IEnumerable<KeyValuePair<string, GuildSummary>> GetGuilds(WowRealm _Realm)
        {
            if (_Realm == WowRealm.All)
                return m_Guilds;
            return m_Guilds.Where((_Value) => _Value.Value.Realm == _Realm);
        }
        public void UpdateDatabase(RPPDatabase _Database)
        {
            UpdateDatabase(_Database, DateTime.MinValue);
        }
        public void UpdateDatabase(RPPDatabase _Database, DateTime _EarliestDateTime)
        {
            var realmDBs = _Database.GetRealms();
            foreach (var realmDB in realmDBs)
            {
                foreach (var playerHistory in realmDB.Value.PlayersHistory)
                {
                    try
                    {
                        if (playerHistory.Value.GuildHistory.Count < 1)
                            continue;

                        if (playerHistory.Value.GuildHistory.Last().Uploader.GetTime() >= _EarliestDateTime)
                        {
                            List<string> guildsAffected = new List<string>();
                            for (int i = playerHistory.Value.GuildHistory.Count - 1; i >= 0; --i)
                            {
                                if (playerHistory.Value.GuildHistory[i].Uploader.GetTime() < _EarliestDateTime)
                                    break;

                                string guildName = playerHistory.Value.GuildHistory[i].Data.GuildName;
                                if (guildsAffected.Contains(guildName) == false)
                                    guildsAffected.Add(guildName);
                            }
                            foreach (string guildName in guildsAffected)
                            {
                                if (guildName == "nil" || guildName == "None" || guildName == "Unknown")
                                    continue;
                                if (GetGuildSummary(realmDB.Value.Realm, guildName) == null)
                                {
                                    var newGuildSummary = new GuildSummary(guildName, realmDB.Value.Realm);
                                    newGuildSummary.InitCache();
                                    AddGuildSummary(newGuildSummary);
                                }
                            }
                            foreach (var guild in m_Guilds)
                            {
                                if (guild.Value.Realm == realmDB.Value.Realm)
                                {
                                    guild.Value.Update(playerHistory.Key, playerHistory.Value, _EarliestDateTime);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VF_RealmPlayersDatabase.Logger.LogException(ex);
                    }
                }
            }
        }

        public static GuildSummaryDatabase LoadSummaryDatabase(string _RootDirectory)
        {
            GuildSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\GuildSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (Utility.LoadSerialize(databaseFile, out database) == false)
                    database = null;
            }
            if (database != null)
            {
                foreach (var guild in database.m_Guilds)
                {
                    guild.Value.InitCache();
                }
            }
            return database;
        }
        public static void UpdateSummaryDatabase(string _RootDirectory, RPPDatabase _Database)
        {
            GuildSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\GuildSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (Utility.LoadSerialize(databaseFile, out database) == false)
                    database = null;
            }
            if (database != null)
            {
                foreach (var guild in database.m_Guilds)
                {
                    guild.Value.InitCache();
                }
            }
            if (database == null)
            {
                database = new GuildSummaryDatabase();
                database.UpdateDatabase(_Database);
            }
            else
            {
                database.UpdateDatabase(_Database, DateTime.UtcNow.AddDays(-8));
            }
            Utility.SaveSerialize(databaseFile, database);
        }
    }
}
