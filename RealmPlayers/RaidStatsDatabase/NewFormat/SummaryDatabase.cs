using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ProtoBuf;
using VF_RaidDamageDatabase.Models;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;

using Old_RaidCollection = VF_RaidDamageDatabase.RaidCollection;
using Old_RaidCollection_Raid = VF_RaidDamageDatabase.RaidCollection_Raid;
using Old_FightDataCollection = VF_RaidDamageDatabase.FightDataCollection;

using Utility = VF_RaidDamageDatabase.Utility;

namespace VF_RDDatabase
{
    [ProtoContract]
    public class SummaryDatabase
    {
        [ProtoMember(1)]
        private Dictionary<string, GroupRaidCollection> m_GroupRCs = new Dictionary<string, GroupRaidCollection>();

        //Generated data, do not save to protoBuf!!!
        private Dictionary<string, PlayerSummary> m_PlayerSummaries = new Dictionary<string, PlayerSummary>();
        //Generated data, do not save to protoBuf!!!

        public Dictionary<string, GroupRaidCollection> GroupRCs
        {
            get { return m_GroupRCs; }
        }
        public Dictionary<string, PlayerSummary> PlayerSummaries
        {
            get 
            {
                if (m_PlayerSummaries.Count == 0)
                {
                    GeneratePlayerSummaries();
                }
                return m_PlayerSummaries; 
            }
        }

        public void GeneratePlayerSummaries()
        {
            if (m_PlayerSummaries.Count == 0)
            {
                lock (m_PlayerSummaries)
                {
                    if (m_PlayerSummaries.Count != 0)
                        return;

                    foreach (var groupRC in GroupRCs)
                    {
                        foreach (var raid in groupRC.Value.Raids)
                        {
                            List<string> bossFightsAdded = new List<string>();
                            foreach (var bossFight in raid.Value.BossFights)
                            {
                                if (bossFight.AttemptType != AttemptType.KillAttempt)
                                    continue;

                                if (bossFightsAdded.Contains(bossFight.BossName))
                                    continue;//Do not add duplicates!

                                bossFightsAdded.Add(bossFight.BossName);

                                foreach (var playerData in bossFight.PlayerFightData)
                                {
                                    if (playerData.Item2.Deaths > 0 || playerData.Item2.Damage > 0 || playerData.Item2.RawHeal > 0)
                                    {//If check can be removed if SummaryDatabase is fresh generated after 2014-04-12. This check exists in BossFight.cs generation aswell
                                        string playerKeyName = Utility.GetRealmPreString(groupRC.Value.m_Realm) + playerData.Item1;
                                        if (m_PlayerSummaries.ContainsKey(playerKeyName) == false)
                                        {
                                            m_PlayerSummaries.Add(playerKeyName, new PlayerSummary(playerData.Item1, groupRC.Value.Realm));
                                        }
                                        var currPlayerSummary = m_PlayerSummaries[playerKeyName];
                                        currPlayerSummary.AddBossFightData(bossFight, playerData.Item2);
                                    }
                                }
                            }
                        }
                    }
                    foreach (var playerSummary in m_PlayerSummaries)
                    {
                        playerSummary.Value.SortBossFights();
                    }
                }
            }
        }
        public static readonly DateTime EARLIEST_HSELLIGIBLE_DATE = new DateTime(2013, 10, 23, 0, 0, 0);
        public static readonly DateTime EARLIEST_HSELLIGIBLE_NOS_MAJORDOMO_DATE = new DateTime(2015, 6, 1, 0, 0, 0);
        public List<BossFight> GetHSElligibleBossFights(string _BossName, WowRealm _Realm = WowRealm.All, string _GuildFilter = null, string _PlayerFilter = null, IEnumerable<PurgedPlayer> _PurgePlayers = null)
        {
            List<BossFight> fightInstances = new List<BossFight>();

            DateTime earliestCompatibleDate = EARLIEST_HSELLIGIBLE_DATE;
            if(_BossName == "Majordomo Executus" && _Realm == WowRealm.Nostalrius)
            {
                earliestCompatibleDate = EARLIEST_HSELLIGIBLE_NOS_MAJORDOMO_DATE;
            }

            double highestPrecision = 0;
            double totalPrecision = 0;
            foreach (var groupRC in GroupRCs)
            {
                if (_Realm != WowRealm.All && _Realm != groupRC.Value.Realm)
                    continue;

                if (_GuildFilter != null && _GuildFilter != groupRC.Value.GroupName)
                    continue;

                foreach (var raid in groupRC.Value.Raids)
                {
                    List<string> bossFightsAdded = new List<string>();

                    if (_PurgePlayers != null && _PurgePlayers.Any(pp => raid.Value.RaidStartDate > pp.BeginDate
                                            && raid.Value.RaidEndDate < pp.EndDate
                                            && groupRC.Value.Realm == pp.Realm
                                            && raid.Value.m_RaidMembers.Contains(pp.Name)))
                    {
                        continue; //Disqualify any raid having a purgeplayer as raidmember since there is basically no guarantee anything is correct anymore.
                    }

                    foreach (var bossFight in raid.Value.BossFights)
                    {
                        if (bossFightsAdded.Contains(bossFight.BossName))
                            continue;//Do not add duplicates!

                        if (bossFight.BossName == _BossName && bossFight.IsQualityHigh()
                        && bossFight.StartDateTime > earliestCompatibleDate)
                        {
                            if (_PlayerFilter != null)
                            {
                                int playerIndex = bossFight.PlayerFightData.FindIndex((_Value) => { return _Value.Item1 == _PlayerFilter; });
                                if (playerIndex <= 0)
                                {
                                    continue;
                                }
                            }

                            bossFightsAdded.Add(bossFight.BossName);

                            double precision = bossFight.DataDetails.FightPrecision;// fight.CalculatePrecision(realmDB.RD_IsPlayer);
                            fightInstances.Add(bossFight);

                            if (precision > highestPrecision)
                                highestPrecision = precision;
                            totalPrecision += precision;
                        }
                    }
                }
            }
            double averagePrecision = totalPrecision / fightInstances.Count;
            double acceptablePrecisionMin = averagePrecision - 0.05;

            fightInstances.RemoveAll((_Value) => { return _Value.DataDetails.FightPrecision < acceptablePrecisionMin; });
            return fightInstances;
        }
        public PlayerSummary GetPlayerSummary(string _Player, WowRealm _Realm)
        {
            if (m_PlayerSummaries.Count == 0)
            {
                DateTime timer = DateTime.Now;
                VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Started generating PlayerSummary");
                GC.Collect();
                GeneratePlayerSummaries();
                GC.Collect();
                VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Done with generating PlayerSummary after " + (DateTime.Now - timer));
            }
            PlayerSummary retValue = null;
            if (m_PlayerSummaries.TryGetValue(Utility.GetRealmPreString(_Realm) + _Player, out retValue) == false)
                return null;

            return retValue;
        }

        public GroupRaidCollection GetGroupRC(WowRealm _Realm, string _GroupName)
        {
            GroupRaidCollection retValue = null;
            if (m_GroupRCs.TryGetValue(Utility.GetRealmPreString(_Realm) + _GroupName, out retValue) == false)
                return null;

            return retValue;
        }
        private void AddGroupRC(GroupRaidCollection _GroupRaidCollection)
        {
            m_GroupRCs.Add(Utility.GetRealmPreString(_GroupRaidCollection.Realm) + _GroupRaidCollection.GroupName, _GroupRaidCollection);
        }

        public void UpdateDatabase(Old_RaidCollection _RaidCollection, Func<string, Old_FightDataCollection> _CachedGetFightDataCollectionFunc, Func<WowRealm, VF_RaidDamageDatabase.RealmDB> _GetRealmDB)
        {
            UpdateDatabase(_RaidCollection.m_Raids.Values.ToList(), _CachedGetFightDataCollectionFunc, _GetRealmDB);
        }
        public void UpdateDatabase(List<Old_RaidCollection_Raid> _Raids, Func<string, Old_FightDataCollection> _CachedGetFightDataCollectionFunc, Func<WowRealm, VF_RaidDamageDatabase.RealmDB> _GetRealmDB)
        {
            Hidden._GlobalInitializationData.Init(_GetRealmDB, _CachedGetFightDataCollectionFunc);
            Console.Write("SummaryDatabase.UpdateDatabase: " + _Raids.Count + " raids");
            int i = 0;
            foreach (var raid in _Raids)
            {
                var groupRC = GetGroupRC(raid.Realm, raid.RaidOwnerName);
                if (groupRC == null)
                {
                    groupRC = new GroupRaidCollection();
                    groupRC.m_Realm = raid.Realm;
                    groupRC.m_GroupName = raid.RaidOwnerName;
                    AddGroupRC(groupRC);
                }

                groupRC.GenerateSummary_AddRaid(raid);
                Console.Write(".");
                ++i;
                if (i % 50 == 49)
                {
                    Console.Write("Added " + i + " raids");
                    GC.Collect();
                }
            }
            Hidden._GlobalInitializationData.Clear();
        }
        public void UpdateDatabaseReplace(List<Old_RaidCollection_Raid> _Raids, Func<string, Old_FightDataCollection> _CachedGetFightDataCollectionFunc, Func<WowRealm, VF_RaidDamageDatabase.RealmDB> _GetRealmDB)
        {
            Hidden._GlobalInitializationData.Init(_GetRealmDB, _CachedGetFightDataCollectionFunc);
            Console.Write("SummaryDatabase.UpdateDatabaseReplace: " + _Raids.Count + " raids");
            int i = 0;
            foreach (var raid in _Raids)
            {
                var groupRC = GetGroupRC(raid.Realm, raid.RaidOwnerName);
                if (groupRC == null)
                {
                    groupRC = new GroupRaidCollection();
                    groupRC.m_Realm = raid.Realm;
                    groupRC.m_GroupName = raid.RaidOwnerName;
                    AddGroupRC(groupRC);
                }

                groupRC.GenerateSummary_ReplaceRaid(raid);
                Console.Write(".");
                ++i;
                if (i % 50 == 49)
                {
                    Console.Write("Replaced " + i + " raids");
                    GC.Collect();
                }
            }
            Hidden._GlobalInitializationData.Clear();
        }
        public static SummaryDatabase GenerateSummaryDatabase(Old_RaidCollection _RaidCollection, Func<string, Old_FightDataCollection> _CachedGetFightDataCollectionFunc, Func<WowRealm, VF_RaidDamageDatabase.RealmDB> _GetRealmDB)
        {
            SummaryDatabase newDatabase = new SummaryDatabase();
            newDatabase.UpdateDatabase(_RaidCollection, _CachedGetFightDataCollectionFunc, _GetRealmDB);
            return newDatabase;
        }
        public static SummaryDatabase LoadSummaryDatabase(string _RootDirectory)
        {
            SummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\FullSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (VF.Utility.LoadSerialize(databaseFile, out database) == false)
                    database = null;
            }
            if (database != null)
            {
                foreach (var groupRC in database.m_GroupRCs)
                {
                    groupRC.Value.InitCache();
                }
            }
            return database;
        }
        public static SummaryDatabase UpdateSummaryDatabase(string _RootDirectory, Old_RaidCollection _FullRaidCollection, List<Old_RaidCollection_Raid> _RecentChangedRaids, Func<string, Old_FightDataCollection> _CachedGetFightDataCollectionFunc, Func<WowRealm, VF_RaidDamageDatabase.RealmDB> _GetRealmDB)
        {
            SummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\FullSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (VF.Utility.LoadSerialize(databaseFile, out database) == false)
                    database = null;
            }
            if (database == null)
            {
                database = GenerateSummaryDatabase(_FullRaidCollection, _CachedGetFightDataCollectionFunc, _GetRealmDB);
            }
            else
            {
                database.UpdateDatabase(_RecentChangedRaids, _CachedGetFightDataCollectionFunc, _GetRealmDB);
            }
            VF.Utility.SaveSerialize(databaseFile, database);
            return database;
        }
        public static void FixBuggedSummaryDatabase(string _RootDirectory, Old_RaidCollection _FullRaidCollection, List<Old_RaidCollection_Raid> _BuggedRaids, Func<string, Old_FightDataCollection> _CachedGetFightDataCollectionFunc, Func<WowRealm, VF_RaidDamageDatabase.RealmDB> _GetRealmDB)
        {
            SummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\FullSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (VF.Utility.LoadSerialize(databaseFile, out database) == false)
                    database = null;
            }
            if (database == null)
            {
                database = GenerateSummaryDatabase(_FullRaidCollection, _CachedGetFightDataCollectionFunc, _GetRealmDB);
            }
            else
            {
                database.UpdateDatabaseReplace(_BuggedRaids, _CachedGetFightDataCollectionFunc, _GetRealmDB);
            }
            VF.Utility.SaveSerialize(databaseFile, database);
        }

        public Raid GetRaid(int _UniqueRaidID)
        {
            foreach (var groupRC in m_GroupRCs)
            {
                Raid raid = null;
                if (groupRC.Value.Raids.TryGetValue(_UniqueRaidID, out raid) == true)
                    return raid;
            }
            return null;
        }
    }
}
