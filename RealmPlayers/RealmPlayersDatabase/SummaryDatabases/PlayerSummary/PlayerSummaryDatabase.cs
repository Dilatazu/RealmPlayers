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
            string realm = "" + (int)_Realm;
            if (realm.Length > 1)
            {
                if ((int)_Realm > 99)
                    throw new Exception("ERROR, REALM WAS INTENDED TO NEVER BE BIGGER THAN VALUE 99");
                realm = "R" + (int)_Realm;
            }

            if (m_PVPSummaries.TryGetValue(realm + _Player, out retValue) == false)
                return null;

            return retValue;
        }
        private void AddPVPSummary(WowRealm _Realm, string _Player, PVPSummary _PVPSummary)
        {
            string realm = "" + (int)_Realm;
            if (realm.Length > 1)
            {
                if ((int)_Realm > 99)
                    throw new Exception("ERROR, REALM WAS INTENDED TO NEVER BE BIGGER THAN VALUE 99");
                realm = "R" + (int)_Realm;
            }
            m_PVPSummaries.Add(realm + _Player, _PVPSummary);
        }
        public IEnumerable<KeyValuePair<string, PVPSummary>> GetPVPSummaries(WowRealm _Realm)
        {
            string realm = "" + (int)_Realm;
            if (realm.Length > 1)
            {
                if ((int)_Realm > 99)
                    throw new Exception("ERROR, REALM WAS INTENDED TO NEVER BE BIGGER THAN VALUE 99");
                realm = "R" + (int)_Realm;
            }
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
                foreach (var playerHistory in realmDB.Value.PlayersHistory)
                {
                    try
                    {
                        if (playerHistory.Value.HonorHistory.Count < 1)
                            continue;

                        PVPSummary playerSummary = new PVPSummary();

                        DateTime currPVPWeek = DateTime.MinValue;
                        foreach (var honorHistory in playerHistory.Value.HonorHistory)
                        {
                            if (honorHistory.Data.LifetimeHighestRank > playerSummary.m_HighestRank.Key && honorHistory.Data.LifetimeHighestRank > honorHistory.Data.CurrentRank)
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
                                    currPVPWeek = StaticValues.CalculateLastRankUpdadeDateUTC(honorHistory.Uploader.GetTime()).AddDays(7);
                                    playerSummary.m_ActivePVPWeeks += 1;
                                }
                            }
                        }
                        AddPVPSummary(realmDB.Key, playerHistory.Key, playerSummary);
                    }
                    catch (Exception ex)
                    {
                        VF_RealmPlayersDatabase.Logger.LogException(ex);
                    }
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
    }
}
