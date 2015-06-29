using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using RPPDatabase = VF_RealmPlayersDatabase.Database;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using StaticValues = VF_RealmPlayersDatabase.StaticValues;

namespace VF_RPDatabase
{
    [ProtoContract]
    public class PVPSummary
    {
        [ProtoMember(1)]
        public KeyValuePair<float, DateTime> m_HighestRank = new KeyValuePair<float,DateTime>();
        [ProtoMember(2)]
        public int m_ActivePVPWeeks = 0;//Weeks where a standing was received

        public PVPSummary()
        { }
    }

    [ProtoContract]
    public class PlayerSummaryDatabase
    {
        [ProtoMember(1)]
        Dictionary<string, PVPSummary> m_PVPSummaries = new Dictionary<string, PVPSummary>();

        public PVPSummary GetPVPSummary(WowRealm _Realm, string _Player)
        {
            PVPSummary retValue = null;
            string realm = VF_RealmPlayersDatabase.Utility.GetRealmPreString(_Realm);
            if (m_PVPSummaries.TryGetValue(realm + _Player, out retValue) == false)
                return null;

            return retValue;
        }
        private void AddPVPSummary(WowRealm _Realm, string _Player, PVPSummary _PVPSummary)
        {
            m_PVPSummaries.Add(VF_RealmPlayersDatabase.Utility.GetRealmPreString(_Realm) + _Player, _PVPSummary);
        }
        public IEnumerable<KeyValuePair<string, PVPSummary>> GetPVPSummaries(WowRealm _Realm)
        {
            string realm = VF_RealmPlayersDatabase.Utility.GetRealmPreString(_Realm);
            return m_PVPSummaries.Where((_Value) => _Value.Key.StartsWith(realm));
        }
        public string GetPlayer(KeyValuePair<string, PVPSummary> _KeyValue)
        {
            if (_KeyValue.Key.StartsWith("R"))
                return _KeyValue.Key.Substring(3);
            else
                return _KeyValue.Key.Substring(1);
        }

        public PlayerSummaryDatabase()
        { }

        public PlayerSummaryDatabase(RPPDatabase _Database)
        {
            var realmDBs = _Database.GetRealms();
            foreach (var realmDB in realmDBs)
            {
                UpdateRealm(realmDB.Value);
            }
        }

        public void UpdateRealm(VF_RealmPlayersDatabase.RealmDatabase _RealmDB)
        {
            DateTime nostalrius_HighestRank_FixDate = new DateTime(2017, 5, 30);
            foreach (var playerHistory in _RealmDB.PlayersHistory)
            {
                try
                {
                    if (playerHistory.Value.HonorHistory.Count < 1)
                        continue;

                    PVPSummary playerSummary = new PVPSummary();

                    DateTime currPVPWeek = DateTime.MinValue;
                    foreach (var honorHistory in playerHistory.Value.HonorHistory)
                    {
                        if (honorHistory.Data.LifetimeHighestRank > playerSummary.m_HighestRank.Key && honorHistory.Data.LifetimeHighestRank > honorHistory.Data.CurrentRank && (_RealmDB.Realm != WowRealm.Nostalrius || (_RealmDB.Realm == WowRealm.Nostalrius && honorHistory.Uploader.GetTime() > nostalrius_HighestRank_FixDate)))
                        {
                            playerSummary.m_HighestRank = new KeyValuePair<float, DateTime>(honorHistory.Data.LifetimeHighestRank, honorHistory.Uploader.GetTime());
                        }
                        else if (honorHistory.Data.GetRankTotal() > playerSummary.m_HighestRank.Key)
                        {
                            if (honorHistory.Data.CurrentRank > playerSummary.m_HighestRank.Key)
                            {
                                playerSummary.m_HighestRank = new KeyValuePair<float, DateTime>(honorHistory.Data.GetRankTotal(), honorHistory.Uploader.GetTime());
                            }
                            else
                            {
                                playerSummary.m_HighestRank = new KeyValuePair<float, DateTime>(honorHistory.Data.GetRankTotal(), playerSummary.m_HighestRank.Value);
                            }
                        }
                        if (currPVPWeek < honorHistory.Uploader.GetTime())
                        {
                            if (honorHistory.Data.LastWeekStanding > 0)
                            {
                                currPVPWeek = StaticValues.CalculateLastRankUpdadeDateUTC(_RealmDB.Realm, honorHistory.Uploader.GetTime()).AddDays(7);
                                playerSummary.m_ActivePVPWeeks += 1;
                            }
                        }
                    }
                    AddPVPSummary(_RealmDB.Realm, playerHistory.Key, playerSummary);
                }
                catch (Exception ex)
                {
                    VF_RealmPlayersDatabase.Logger.LogException(ex);
                }
            }
        }

        public static PlayerSummaryDatabase LoadSummaryDatabase(string _RootDirectory)
        {
            PlayerSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\PlayerSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (VF.Utility.LoadSerialize(databaseFile, out database) == false)
                    database = null;
            }
            return database;
        }
        public static void GenerateSummaryDatabase(string _RootDirectory, RPPDatabase _Database)
        {
            PlayerSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\PlayerSummaryDatabase.dat";

            database = new PlayerSummaryDatabase(_Database);
            VF.Utility.SaveSerialize(databaseFile, database);
        }
        public static void UpdateSummaryDatabase(string _RootDirectory, RPPDatabase _Database)
        {
            PlayerSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\PlayerSummaryDatabase.dat";

            database = new PlayerSummaryDatabase(_Database);
            VF.Utility.SaveSerialize(databaseFile, database);
        }
        public void SaveSummaryDatabase(string _RootDirectory)
        {
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\PlayerSummaryDatabase.dat";
            VF.Utility.SaveSerialize(databaseFile, this);
        }
    }
}
