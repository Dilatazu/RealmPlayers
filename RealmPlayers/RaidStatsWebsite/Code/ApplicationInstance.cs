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

namespace VF.RaidDamageWebsite
{
    public class ApplicationInstance
    {
        public static string g_RPPDBDir = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RPPDatabase\\";
        public static string g_RDDBDir = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\";

        static ApplicationInstance m_Instance;
        static readonly object m_InstanceLock = new object();
        static bool m_InstanceInitialized = false;
        RPPDatabase m_RPPDatabase = null;
        Dictionary<string, Tuple<DateTime, FightDataCollection>> m_Fights = new Dictionary<string, Tuple<DateTime, FightDataCollection>>();
        public readonly object m_Mutex = new object();
        static ApplicationInstance()
        {}
        ApplicationInstance()
        {
            Logger.ConsoleWriteLine("Initializing ApplicationInstance!");
            RealmPlayersServer.Constants.AssertInitialize();

            Authentication.Initialize();
            if (System.IO.Directory.Exists(g_RDDBDir) == false)
            {
                g_RPPDBDir = g_RPPDBDir.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                g_RDDBDir = g_RDDBDir.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
            }
            string rppDBDir = g_RPPDBDir + "Database\\";
            var timeSinceWrite = DateTime.UtcNow - System.IO.File.GetLastWriteTime(rppDBDir + "Emerald_Dream\\PlayersData.dat");
            //if (timeSinceWrite.TotalDays > 20)
            //{
            //    rppDBDir = "\\\\" + HiddenStrings.ServerComputerName + "\\VF_RealmPlayersData\\RPPDatabase\\Database\\";
            //}
            m_RPPDatabase = new RPPDatabase(rppDBDir);
        }
        public static ApplicationInstance Instance
        {
            get
            {
                if (m_InstanceInitialized == false)
                {
                    lock (m_InstanceLock)
                    {
                        if (m_InstanceInitialized == false)
                        {
                            m_Instance = new ApplicationInstance();
                            System.Threading.Thread.MemoryBarrier();
                            m_InstanceInitialized = true;
                        }
                    }
                }
                return m_Instance;
            }
        }
        object m_ItemInfoLock = new object();
        volatile Dictionary<int, ItemInfo> m_ItemInfoCache = null;
        object m_ItemInfoLockTBC = new object();
        volatile Dictionary<int, ItemInfo> m_ItemInfoCacheTBC = null;
        public RealmPlayersServer.ItemInfo GetItemInfo(int _ItemID, VF_RealmPlayersDatabase.WowVersionEnum _WowVersion)
        {
            if (_WowVersion != VF_RealmPlayersDatabase.WowVersionEnum.Vanilla && _WowVersion != VF_RealmPlayersDatabase.WowVersionEnum.TBC)
            {
                Logger.ConsoleWriteLine("ERROR, WowVersion was not Vanilla or TBC!!!");
                return null;
            }
            if (_WowVersion == VF_RealmPlayersDatabase.WowVersionEnum.TBC)
            {
                if(m_ItemInfoCacheTBC == null)
                {
                    lock (m_ItemInfoLockTBC)
                    {
                        try
                        {
                            if (m_ItemInfoCacheTBC == null)
                            {
                                if (System.IO.File.Exists(g_RPPDBDir + "VF_ItemInfoCache.dat"))
                                {
                                    Dictionary<int, ItemInfo> itemInfoCache = new Dictionary<int, ItemInfo>();
                                    if (VF_RealmPlayersDatabase.Utility.LoadSerialize(g_RPPDBDir + "VF_ItemInfoCacheTBC.dat", out itemInfoCache))
                                    {
                                        m_ItemInfoCacheTBC = itemInfoCache;
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
                if (m_ItemInfoCacheTBC == null)
                    return null;
                ItemInfo itemInfo = null;
                if (m_ItemInfoCacheTBC.TryGetValue(_ItemID, out itemInfo) == true)
                    return itemInfo;
                try
                {
                    var webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://realmplayers.com/ItemTooltip.aspx?item=" + System.Web.HttpUtility.UrlEncode("?item=" + _ItemID + "-1"));
                    webRequest.Timeout = 2000;
                    webRequest.ReadWriteTimeout = 2000;
                    using (var webResponse = webRequest.GetResponse())
                    {
                        using (var streamReader = new System.IO.StreamReader(webResponse.GetResponseStream()))
                        {
                            itemInfo = new ItemInfo(_ItemID, streamReader.ReadToEnd(), "");
                            lock (m_ItemInfoLockTBC)
                            {
                                if (m_ItemInfoCacheTBC.ContainsKey(_ItemID) == false)
                                {
                                    m_ItemInfoCacheTBC.Add(_ItemID, itemInfo);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    itemInfo = null;
                }
                return itemInfo;
            }
            else if (_WowVersion == VF_RealmPlayersDatabase.WowVersionEnum.Vanilla)
            {
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
                    var webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://realmplayers.com/ItemTooltip.aspx?item=" + System.Web.HttpUtility.UrlEncode("?item=" + _ItemID + "-0"));
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
                catch (Exception)
                {
                    itemInfo = null;
                }
                return itemInfo;
            }
            return null;
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
        VF_RDDatabase.SummaryDatabase m_FullSummaryDatabase = null;
        public VF_RDDatabase.SummaryDatabase GetSummaryDatabase()
        {
            return DynamicReloader.GetData<VF_RDDatabase.SummaryDatabase>(() =>
            {
                Logger.ConsoleWriteLine("Inside GetSummaryDatabase->GetData()");
                try
                {
                    if (m_FullSummaryDatabase == null)
                    {
                        m_FullSummaryDatabase = VF_RDDatabase.SummaryDatabase.LoadSummaryDatabase_New(g_RDDBDir + "\\SummaryDatabase\\BaseSummaryDatabase.dat");
                        if (m_FullSummaryDatabase != null)
                        {
                            m_FullSummaryDatabase.AddSummaryDatabase(g_RDDBDir + "\\SummaryDatabase\\VeryOldSummaryDatabase.dat");
                            m_FullSummaryDatabase.AddSummaryDatabase(g_RDDBDir + "\\SummaryDatabase\\OldSummaryDatabase.dat");
                            m_FullSummaryDatabase.GeneratePlayerSummaries(true);
                            //TODO: Add code to load all summarydatabases from a specific history directory
                        }
                    }
                    if (m_FullSummaryDatabase != null)
                    {
                        if (m_FullSummaryDatabase.AddSummaryDatabase(g_RDDBDir + "\\SummaryDatabase\\NewSummaryDatabase.dat") == true)
                        {
                            m_FullSummaryDatabase.GeneratePlayerSummaries(true);
                        }
                    }
                    else
                    {
                        m_FullSummaryDatabase = VF_RDDatabase.SummaryDatabase.LoadSummaryDatabase_New(g_RDDBDir + "\\SummaryDatabase\\NewSummaryDatabase.dat");
                        m_FullSummaryDatabase.GeneratePlayerSummaries();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                Logger.ConsoleWriteLine("Done GetSummaryDatabase->GetData()");
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                return m_FullSummaryDatabase;
            }, (_SummaryDatabase, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 30; });
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
                    
                    int secondThreshold = 15 * 60;
                    long currMemory = GC.GetTotalMemory(false);
                    if (currMemory > 13L * 1024L * 1024L * 1024L)
                        secondThreshold = 60;
                    if (currMemory > 10L * 1024L * 1024L * 1024L)
                        secondThreshold = 5 * 60;
                    else if (currMemory > 7L * 1024L * 1024L * 1024L)
                        secondThreshold = 10 * 60;
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
                        if (currMemory > 15L * 1024L * 1024L * 1024L || unloadFiles.Count > 50)
                        {
                            Logger.ConsoleWriteLine(HttpContext.Current.Request.UserHostAddress + " Forced a big garbage collect!", ConsoleColor.Red);
                            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                        }
                        if (currMemory > 13L * 1024L * 1024L * 1024L || unloadFiles.Count > 50)
                        {
                            Logger.ConsoleWriteLine(HttpContext.Current.Request.UserHostAddress + " Forced a garbage collect!", ConsoleColor.White);
                            GC.Collect();
                        }
                    }
                }
            }
            //GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
            return fightDataCollection;
        }
        public VF_RaidDamageDatabase.RaidCollection GetRaidCollection()
        {
            return DynamicReloader.GetData<VF_RaidDamageDatabase.RaidCollection>(() =>
            {
                VF_RaidDamageDatabase.RaidCollection raidCollection = null;
                try
                {
                    VF.Utility.LoadSerialize<VF_RaidDamageDatabase.RaidCollection>(g_RDDBDir + "RaidCollection.dat", out raidCollection);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
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
        public DateTime GetRaidDate(int _UniqueRaidID)
        {
            var raidCollection = GetRaidCollection();
            if (raidCollection.m_Raids.ContainsKey(_UniqueRaidID) == false)
                return DateTime.Now;

            var currRaid = raidCollection.m_Raids[_UniqueRaidID];
            return currRaid.RaidEndDate;
        }
        public List<string> GetRaidFiles(int _UniqueRaidID)
        {
            var raidCollection = GetRaidCollection();
            if (raidCollection.m_Raids.ContainsKey(_UniqueRaidID) == false)
                return null;

            var currRaid = raidCollection.m_Raids[_UniqueRaidID];
            return currRaid.m_DataFiles;
        }
        public List<RaidBossFight> GetRaidBossFights(int _UniqueRaidID)
        {
            var raidCollection = GetRaidCollection();
            if (raidCollection.m_Raids.ContainsKey(_UniqueRaidID) == false)
                return null;

            var summaryDB = GetSummaryDatabase();

            var currRaid = raidCollection.m_Raids[_UniqueRaidID];
            var groupRC = summaryDB.GetGroupRC(currRaid.Realm, currRaid.RaidOwnerName);
            if(groupRC != null)
            {
                var raidSummary = groupRC.GetRaid(_UniqueRaidID);
                return currRaid.GetBossFights(GetRaidFightCollection, raidSummary);
            }
            else
            {
                return currRaid.GetBossFights(GetRaidFightCollection, null);
            }
        }
        public List<RaidBossFight> GetDungeonBossFights(int _UniqueDungeonID)
        {
            var raidCollection = GetRaidCollection();
            if (raidCollection.m_Dungeons.ContainsKey(_UniqueDungeonID) == false)
                return null;

            var currDungeon = raidCollection.m_Dungeons[_UniqueDungeonID];
            return currDungeon.GetBossFights(GetRaidFightCollection);
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
                try
                {
                    summaryDB = VF_RPDatabase.ItemSummaryDatabase.LoadSummaryDatabase(g_RPPDBDir);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                return summaryDB;
            }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 60; });
        }
        public VF_RPDatabase.GuildSummaryDatabase GetGuildSummaryDatabase()
        {
            return DynamicReloader.GetData<VF_RPDatabase.GuildSummaryDatabase>(() =>
            {
                VF_RPDatabase.GuildSummaryDatabase summaryDB = null;
                try
                {
                    summaryDB = VF_RPDatabase.GuildSummaryDatabase.LoadSummaryDatabase(g_RPPDBDir);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                return summaryDB;
            }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 30; });
        }
        public IEnumerable<VF_RaidDamageDatabase.Models.PurgedPlayer> GetPurgedPlayers(WowRealm _Realm)
        {
            if (_Realm == WowRealm.Unknown)
                return null;

            var allPurgedPlayers = DynamicReloader.GetData<List<VF_RaidDamageDatabase.Models.PurgedPlayer>>(() =>
            {
                List<VF_RaidDamageDatabase.Models.PurgedPlayer> resultPurgedPlayers = new List<VF_RaidDamageDatabase.Models.PurgedPlayer>();
                string purgedPlayersStr = RealmPlayersServer.DynamicFileData.GetTextFile(g_RDDBDir + "Variables\\PurgedPlayers.txt");
                var purgedPlayerStrArray = purgedPlayersStr.SplitVF("\r\n");

                foreach (string purgedPlayerStr in purgedPlayerStrArray)
                {
                    var purgedPlayerData = purgedPlayerStr.Split(',');

                    if (purgedPlayerData.Length < 2)
                    {
                        if(purgedPlayerStr != "")
                        {
                            Logger.ConsoleWriteLine("Error!!! Could not parse PurgedPlayer String \"" + purgedPlayerStr + "\"", System.Drawing.Color.Red);
                        }
                        continue;
                    }

                    string realmName = purgedPlayerData[0];
                    string playerName = purgedPlayerData[1];
                    string beginDate = "";
                    string endDate = "";
                    if (purgedPlayerData.Length >= 3) beginDate = purgedPlayerData[2];
                    if (purgedPlayerData.Length >= 4) endDate = purgedPlayerData[3];

                    resultPurgedPlayers.Add(new VF_RaidDamageDatabase.Models.PurgedPlayer(playerName, realmName, beginDate, endDate));
                }
                return resultPurgedPlayers;
            }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 60; });

            if (_Realm == WowRealm.All)
                return allPurgedPlayers;

            var realmPurgedPlayers = allPurgedPlayers.Where((pp) => pp.Realm == _Realm);
            if (realmPurgedPlayers.Count() > 0)
                return realmPurgedPlayers;
            return null;
        }
    }
}