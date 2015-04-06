using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;
using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;

using RPPDatabase = VF_RaidDamageDatabase.RPPDatabase;
using RealmDB = VF_RaidDamageDatabase.RealmDB;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;

using FightDataCollection = VF_RaidDamageDatabase.FightDataCollection;

using RaidBossFight = VF_RaidDamageDatabase.RaidBossFight;

using ItemInfo = RealmPlayersServer.ItemInfo;

namespace VF_RaidDamageWebsite
{
    public class ApplicationInstance
    {
        public static string g_RPPDBDir = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RPPDatabase\\";
        public static string g_RDDBDir = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\";

        static readonly ApplicationInstance m_Instance = new ApplicationInstance();

        RPPDatabase m_RPPDatabase = null;
        Dictionary<string, Tuple<DateTime, FightDataCollection>> m_Fights = new Dictionary<string, Tuple<DateTime, FightDataCollection>>();
        public object m_Mutex = new object();
        static ApplicationInstance()
        {}
        ApplicationInstance()
        {
            if (System.IO.Directory.Exists(g_RDDBDir) == false)
            {
                g_RPPDBDir = g_RPPDBDir.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                g_RDDBDir = g_RDDBDir.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
            }
            string rppDBDir = g_RPPDBDir + "Database\\";
            var timeSinceWrite = DateTime.UtcNow - System.IO.File.GetLastWriteTime(rppDBDir + "Emerald_Dream\\PlayersData.dat");
            //if (timeSinceWrite.TotalDays > 20)
            //{
            //    rppDBDir = "\\\\***REMOVED***\\VF_RealmPlayersData\\RPPDatabase\\Database\\";
            //}
            m_RPPDatabase = new RPPDatabase(rppDBDir);

        }
        public static ApplicationInstance Instance
        {
            get
            {
                return m_Instance;
            }
        }
        object m_ItemInfoLock = new object();
        volatile Dictionary<int, ItemInfo> m_ItemInfoCache = null;
        public RealmPlayersServer.ItemInfo GetItemInfo(int _ItemID, VF_RealmPlayersDatabase.WowVersionEnum _WowVersion)
        {
            if (_WowVersion != VF_RealmPlayersDatabase.WowVersionEnum.Vanilla)
            {
                Logger.ConsoleWriteLine("ERROR, WowVersion was not Vanilla!!!");
                return null;
            }
            if (m_ItemInfoCache == null)
            {
                lock (m_ItemInfoLock)
                {
                    try
                    {
                        if (m_ItemInfoCache == null)
                        {
                            if (System.IO.File.Exists(g_RPPDBDir + "VF_ItemInfoCache.dat"))
                            {
                                Dictionary<int, ItemInfo> itemInfoCache = new Dictionary<int, ItemInfo>();
                                if (VF_RealmPlayersDatabase.Utility.LoadSerialize(g_RPPDBDir + "VF_ItemInfoCache.dat", out itemInfoCache))
                                {
                                    m_ItemInfoCache = itemInfoCache;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
            if (m_ItemInfoCache == null)
                return null;
            ItemInfo itemInfo = null;
            if (m_ItemInfoCache.TryGetValue(_ItemID, out itemInfo) == true)
                return itemInfo;
            try
            {
                var webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://realmplayers.com/ItemTooltip.aspx?item=" + System.Web.HttpUtility.UrlEncode("?item=" + _ItemID + ""));
                webRequest.Timeout = 2000;
                webRequest.ReadWriteTimeout = 2000;
                using (var webResponse = webRequest.GetResponse())
                {
                    using (var streamReader = new System.IO.StreamReader(webResponse.GetResponseStream()))
                    {
                        itemInfo = new ItemInfo(_ItemID, streamReader.ReadToEnd(), "");
                        lock (m_ItemInfoLock)
                        {
                            if (m_ItemInfoCache.ContainsKey(_ItemID) == false)
                            {
                                m_ItemInfoCache.Add(_ItemID, itemInfo);
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
                itemInfo = null;
            }
            return itemInfo;
        }
        public List<string> GetFightsFileList()
        {
            List<string> returnList = new List<string>();
            string[] fightsList = System.IO.Directory.GetFiles(g_RDDBDir + "Fights\\");
            foreach (var fight in fightsList)
            {
                returnList.Add(fight.Replace(g_RDDBDir + "Fights\\", "").Replace(".dat", ""));
            }
            return returnList;
        }
        public RealmDB GetRealmDB(WowRealm _Realm)
        {
            return m_RPPDatabase.GetRealmDB(_Realm);
        }
        public RPPDatabase GetRPPDatabase()
        {
            return m_RPPDatabase;
        }
        public Player GetRealmPlayer(string _Player, WowRealm _Realm)
        {
            return m_RPPDatabase.GetRealmDB(_Realm).FindPlayer(_Player);
        }
        public VF_RDDatabase.SummaryDatabase GetSummaryDatabase()
        {
            return DynamicReloader.GetData<VF_RDDatabase.SummaryDatabase>(() =>
            {
                VF_RDDatabase.SummaryDatabase summaryDB = null;
                summaryDB = VF_RDDatabase.SummaryDatabase.LoadSummaryDatabase(g_RDDBDir);
                summaryDB.GeneratePlayerSummaries();
                return summaryDB;
            }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 10; });
        }
        public FightDataCollection GetRaidFightCollection(string _FightFile)
        {
            if (_FightFile.StartsWith(g_RDDBDir))
            {
                _FightFile = _FightFile.Substring(g_RDDBDir.Length);
            }
            VF_RaidDamageDatabase.FightDataCollection fightDataCollection = null;
            lock(m_Mutex)
            {
                if (m_Fights.ContainsKey(_FightFile) == true)
                {
                    fightDataCollection = m_Fights[_FightFile].Item2;
                    m_Fights[_FightFile] = Tuple.Create(DateTime.UtcNow, fightDataCollection);
                }
                else
                {
                    VF.Utility.LoadSerialize(g_RDDBDir + _FightFile, out fightDataCollection);
                    m_Fights.Add(_FightFile, Tuple.Create(DateTime.UtcNow, fightDataCollection));
                    
                    int secondThreshold = 30 * 60;
                    long currMemory = GC.GetTotalMemory(false);
                    if (currMemory > 3L * 1024L * 1024L * 1024L)
                        secondThreshold = 0;
                    else if (currMemory > 2L * 1024L * 1024L * 1024L)
                        secondThreshold = 5 * 60;
                    else if (currMemory > 1L * 1024L * 1024L * 1024L)
                        secondThreshold = 20 * 60;
                    try
                    {
                        Logger.ConsoleWriteLine(HttpContext.Current.Request.UserHostAddress + " - Loaded data file: \"" + _FightFile + "\", Current memory usage: " + ((double)currMemory / 1024.0 / 1024.0).ToString("0.000") + "MB", ConsoleColor.White);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                    List<string> unloadFiles = new List<string>();
                    foreach (var fight in m_Fights)
                    {
                        if ((DateTime.UtcNow - fight.Value.Item1).TotalSeconds > secondThreshold)
                        {
                            unloadFiles.Add(fight.Key);
                        }
                    }
                    m_Fights.RemoveKeys((_Key) => unloadFiles.Contains(_Key));
                    if (unloadFiles.Count > 0)
                    {
                        Logger.ConsoleWriteLine("Unloaded " + unloadFiles.Count + " files", ConsoleColor.Yellow);
                        if (currMemory > 4L * 1024L * 1024L * 1024L || unloadFiles.Count > 50)
                        {
                            Logger.ConsoleWriteLine(HttpContext.Current.Request.UserHostAddress + " Forced a garbage collect!", ConsoleColor.White);
                            GC.Collect();
                        }
                    }
                }
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
            return fightDataCollection;
        }
        public VF_RaidDamageDatabase.RaidCollection GetRaidCollection()
        {
            return DynamicReloader.GetData<VF_RaidDamageDatabase.RaidCollection>(() =>
            {
                VF_RaidDamageDatabase.RaidCollection raidCollection = null;
                VF.Utility.LoadSerialize<VF_RaidDamageDatabase.RaidCollection>(g_RDDBDir + "RaidCollection.dat", out raidCollection);
                return raidCollection;
            }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 5; });
        }


        public RaidBossFight GetRaidBossFight(int _UniqueRaidID, int _RaidBossFightIndex)
        {
            var raidCollection = GetRaidCollection();
            if (raidCollection.m_Raids.ContainsKey(_UniqueRaidID) == false)
                return null;

            var summaryDB = GetSummaryDatabase();
            var currRaid = raidCollection.m_Raids[_UniqueRaidID];
            var groupRC = summaryDB.GetGroupRC(currRaid.Realm, currRaid.RaidOwnerName);
            var raidSummary = groupRC.GetRaid(_UniqueRaidID);
            return currRaid.GetBossFight(_RaidBossFightIndex, GetRaidFightCollection, raidSummary);
        }
        public List<RaidBossFight> GetRaidBossFights(int _UniqueRaidID)
        {
            var raidCollection = GetRaidCollection();
            if (raidCollection.m_Raids.ContainsKey(_UniqueRaidID) == false)
                return null;

            var summaryDB = GetSummaryDatabase();

            var currRaid = raidCollection.m_Raids[_UniqueRaidID];
            var groupRC = summaryDB.GetGroupRC(currRaid.Realm, currRaid.RaidOwnerName);
            var raidSummary = groupRC.GetRaid(_UniqueRaidID);
            return currRaid.GetBossFights(GetRaidFightCollection, raidSummary);
        }

        public RaidBossFight GetRaidTrashFight(int _UniqueRaidID, int _RaidTrashFightIndex)
        {
            var raidCollection = GetRaidCollection();
            if (raidCollection.m_Raids.ContainsKey(_UniqueRaidID) == false)
                return null;

            var currRaid = raidCollection.m_Raids[_UniqueRaidID];
            return currRaid.GetTrashFight(_RaidTrashFightIndex, GetRaidFightCollection);
        }
        public List<RaidBossFight> GetRaidTrashFights(int _UniqueRaidID)
        {
            var raidCollection = GetRaidCollection();
            if (raidCollection.m_Raids.ContainsKey(_UniqueRaidID) == false)
                return null;

            var currRaid = raidCollection.m_Raids[_UniqueRaidID];
            return currRaid.GetTrashFights(GetRaidFightCollection);
        }
        public VF_RPDatabase.ItemSummaryDatabase GetItemSummaryDatabase()
        {
            return DynamicReloader.GetData<VF_RPDatabase.ItemSummaryDatabase>(() =>
            {
                VF_RPDatabase.ItemSummaryDatabase summaryDB = null;
                summaryDB = VF_RPDatabase.ItemSummaryDatabase.LoadSummaryDatabase(g_RPPDBDir);
                return summaryDB;
            }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 60; });
        }
        public VF_RPDatabase.GuildSummaryDatabase GetGuildSummaryDatabase()
        {
            return DynamicReloader.GetData<VF_RPDatabase.GuildSummaryDatabase>(() =>
            {
                VF_RPDatabase.GuildSummaryDatabase summaryDB = null;
                summaryDB = VF_RPDatabase.GuildSummaryDatabase.LoadSummaryDatabase(g_RPPDBDir);
                return summaryDB;
            }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 30; });
        }
    }
}