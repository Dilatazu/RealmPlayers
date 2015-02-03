using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

using RPPDatabase = VF_RealmPlayersDatabase.Database;
using ItemDropDatabase = VF_RealmPlayersDatabase.ItemDropDatabase;
using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

namespace RealmPlayersServer
{
    namespace Hidden
    {
        public class UserActivityStats
        {
            public enum IntervalStat
            {
                Last30Sec,
                Last5Min,
                Last10Min,
                Last30Min,
                LastHour,
                Last2Hours,
                Last3Hours,
                Last6Hours,
                Last12Hours,
                Last24Hours,
                Last2Days,
                Last4Days,
                Last7Days,
                LastMonth,
                Total,
            }
            private static Dictionary<IntervalStat, TimeSpan> g_DefinedIntervals = new Dictionary<IntervalStat, TimeSpan>
            {
                {IntervalStat.Last30Sec, TimeSpan.FromSeconds(30)},
                {IntervalStat.Last5Min, TimeSpan.FromMinutes(5)},
                {IntervalStat.Last10Min, TimeSpan.FromMinutes(10)},
                {IntervalStat.Last30Min, TimeSpan.FromMinutes(30)},
                {IntervalStat.LastHour, TimeSpan.FromMinutes(60)},
                {IntervalStat.Last2Hours, TimeSpan.FromHours(2)},
                {IntervalStat.Last3Hours, TimeSpan.FromHours(3)},
                {IntervalStat.Last6Hours, TimeSpan.FromHours(6)},
                {IntervalStat.Last12Hours, TimeSpan.FromHours(12)},
                {IntervalStat.Last24Hours, TimeSpan.FromHours(24)},
                {IntervalStat.Last2Days, TimeSpan.FromDays(2)},
                {IntervalStat.Last4Days, TimeSpan.FromDays(4)},
                {IntervalStat.Last7Days, TimeSpan.FromDays(7)},
                {IntervalStat.LastMonth, TimeSpan.FromDays(30)},
            };

            Dictionary<IntervalStat, int> m_Stats = new Dictionary<IntervalStat, int>();

            //public static UserActivityStats Generate(Dictionary<string, List<Tuple<DateTime, string>>> _UsersOnSite)
            //{
            //    var startTime = DateTime.Now;
            //    UserActivityStats userActivity = new UserActivityStats();
            //    foreach (var interval in g_DefinedIntervals)
            //    {
            //        DateTime comparerValue = DateTime.Now.Subtract(interval.Value);
            //        userActivity.m_Stats.Add(interval.Key, _UsersOnSite.Where((_Value) => { return _Value.Value.Last().Item1 > comparerValue; }).Count());
            //    }

            //    userActivity.m_Stats.Add(IntervalStat.Total, _UsersOnSite.Count);

            //    Logger.ConsoleWriteLine("Took " + (DateTime.Now - startTime).TotalSeconds.ToStringDot("0.0") + " seconds to generate UserActivityStats");
            //    return userActivity;
            //}
            public static UserActivityStats Generate()
            {
                var startTime = DateTime.Now;
                UserActivityStats userActivity = new UserActivityStats();
                return userActivity;
                //TODO: This code below took 20 seconds etc freezing the entire webservice. FIX THE PROBLEM IN THE FUTURE
                try
                {
                    foreach (var interval in g_DefinedIntervals)
                    {
                        DateTime comparerValue = DateTime.Now.Subtract(interval.Value);
                        userActivity.m_Stats.Add(interval.Key, (int)UserActivityDB.GetUniqueVisitCountsSince(comparerValue));
                    }

                    userActivity.m_Stats.Add(IntervalStat.Total, (int)UserActivityDB.GetTotalUniqueVisitors());

                    Logger.ConsoleWriteLine("Took " + (DateTime.Now - startTime).TotalSeconds.ToStringDot("0.0") + " seconds to generate UserActivityStats");
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                return userActivity;
            }

            public int GetStat(IntervalStat _Stat)
            {
                if (m_Stats.ContainsKey(_Stat))
                    return m_Stats[_Stat];
                else
                    return -1;
            }
        }
        public sealed class ApplicationInstance
        {
            static readonly ApplicationInstance m_Instance = new ApplicationInstance();

            public VF.ThreadSafeCache m_ThreadSafeCache = new VF.ThreadSafeCache();
            public DateTime m_StartTime = DateTime.Now;
            public object m_RealmPlayersMutex = new object();
            volatile RPPDatabase m_RPPDatabase;
            public DateTime m_LastLoadedDateTime;
            public DateTime m_LastDatabaseUpdateTime;
            public Code.ContributorStatistics m_ContributorStatistics = null;
            System.Threading.Thread m_LoadRealmPlayersThread = null;

            //public Dictionary<string, List<Tuple<DateTime, string>>> m_UsersOnSite = new Dictionary<string, List<Tuple<DateTime, string>>>();
            //public void AddUserToSite(string _Name)
            //{
            //    Monitor.Enter(m_RealmPlayersMutex);
            //    if (m_UsersOnSite.ContainsKey(_Name) == false)
            //        m_UsersOnSite.Add(_Name, DateTime.Now);
            //    else
            //        m_UsersOnSite[_Name] = DateTime.Now;
            //    Monitor.Exit(m_RealmPlayersMutex);
            //}
            public DateTime m_LastUserActivitySave = DateTime.Now;
            public void AddUserActivity(string _IP, string _Url, string _UrlReferrer)
            {
                //TODO: This code below is freezing the entire webservice. FIX THE PROBLEM IN THE FUTURE
                return;
                UserActivityDB.AddUserActivity(_IP, _Url, _UrlReferrer);
                return;
                //Monitor.Enter(m_RealmPlayersMutex);
                //if (m_UsersOnSite.ContainsKey(_IP) == false)
                //    m_UsersOnSite.Add(_IP, new List<Tuple<DateTime, string>>());

                //var theList = m_UsersOnSite[_IP];
                //if (theList.Count > 100)
                //    theList.RemoveRange(0, 50);

                //if (_UrlReferrer.EndsWith(_Url) == true)
                //    _UrlReferrer = "";

                //string newUserActivity = _Url + (_UrlReferrer == "" ? "" : (" @<@ " + _UrlReferrer));

                //if (theList.Count < 1 || theList.Last().Item2 != newUserActivity)
                //{
                //    theList.Add(new Tuple<DateTime, string>(DateTime.Now, newUserActivity));
                //}

                //if ((DateTime.Now - m_LastUserActivitySave).Minutes > 30)
                //{
                //    SaveUserActivityData();
                //    Logger.SaveToDisk();
                //}
                //Monitor.Exit(m_RealmPlayersMutex);
            }
            public UserActivityStats GetUserActivityStats()
            {
                UserActivityStats stats = DynamicReloader.GetData<UserActivityStats>(() =>
                {
                    lock (m_RealmPlayersMutex)
                    {
                        return UserActivityStats.Generate();
                    }
                }, (_UserActivityStats, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 5; }, TimeSpan.FromMinutes(5), true);
                return stats;
            }
            //public void SaveUserActivityData()
            //{
            //    Monitor.Enter(m_RealmPlayersMutex);
            //    try
            //    {
            //        VF_RealmPlayersDatabase.Utility.BackupFile(Constants.RPPDbWriteDir + "VF_UsersOnSite.dat");
            //        m_LastUserActivitySave = DateTime.Now;
            //        Logger.ConsoleWriteLine("SaveUserActivityData(): Saved VF_UsersOnSite.dat", ConsoleColor.Yellow);
            //    }
            //    catch (Exception)
            //    { }
            //    Monitor.Exit(m_RealmPlayersMutex);
            //}
            public void ReloadRealmPlayers()
            {
                try
                {
                    GC.Collect();
                    bool firstTimeLoading = (m_RPPDatabase == null);
                    DateTime startLoadTime = DateTime.UtcNow;
                    RPPDatabase rppDatabase = DatabaseLoader.LoadRPPDatabase(firstTimeLoading);
                    lock(m_RealmPlayersMutex)
                    {
                        m_RPPDatabase = rppDatabase;
                        m_ContributorStatistics = null;
                        m_LastLoadedDateTime = DateTime.UtcNow;
                    }
                    Logger.ConsoleWriteLine("LoadRPPDatabase(): Reloaded database, it took: " + (int)(DateTime.UtcNow - startLoadTime).TotalSeconds + " seconds", ConsoleColor.Green);
                    m_LoadRealmPlayersThread = null;
                    m_ThreadSafeCache.ClearCache("FindPlayersMatching");
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
            static ApplicationInstance()
            {
            }

            ApplicationInstance()
            {
                //Monitor.Enter(m_RealmPlayersMutex);
                //try
                //{
                //    if (System.IO.File.Exists(Constants.RPPDbDir + "VF_UsersOnSite.dat") == true)
                //    {
                //        VF_RealmPlayersDatabase.Utility.LoadSerialize(Constants.RPPDbDir + "VF_UsersOnSite.dat", out m_UsersOnSite);
                //    }
                //}
                //catch (Exception)
                //{}
                //if(m_UsersOnSite == null)
                //    m_UsersOnSite = new Dictionary<string, List<Tuple<DateTime, string>>>();
                //Monitor.Exit(m_RealmPlayersMutex);
                (new System.Threading.Tasks.Task(() =>
                {
                    try
                    {
                        GetItemInfoCache(WowVersionEnum.Vanilla, true);
                        GetItemInfoCache(WowVersionEnum.TBC, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                })).Start();
            }
            public void BackupItemInfos()
            {
                if (m_ItemInfoUpdated == true && (m_ItemInfoCacheVanilla != null || m_ItemInfoCacheTBC != null))
                {
                    m_ItemInfoUpdated = false;
                    Logger.ConsoleWriteLine("BackupItemInfos(): \"Saving\" ItemInfos due to being updated (SAVING IS DISABLED ATM)", ConsoleColor.Yellow);
                    return;
                    lock(m_ItemInfoLock)
                    {
                        if (m_ItemInfoCacheVanilla != null)
                        {
                            try
                            {
                                VF_RealmPlayersDatabase.Utility.SaveSerialize(Constants.RPPDbWriteDir + "VF_ItemInfoCache.dat", m_ItemInfoCacheVanilla);
                                m_ItemInfoUpdated = false;
                            }
                            catch (Exception)
                            { }
                        }
                        if (m_ItemInfoCacheTBC != null)
                        {
                            try
                            {
                                VF_RealmPlayersDatabase.Utility.SaveSerialize(Constants.RPPDbWriteDir + "VF_ItemInfoCacheTBC.dat", m_ItemInfoCacheTBC);
                                m_ItemInfoUpdated = false;
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
            }
            public string GetCurrentItemDatabaseAddress()
            {
                return m_CurrentItemDatabaseOrder.First();
            }
            public List<string> m_CurrentItemDatabaseOrder = new List<string>(StaticValues.ItemDatabaseAddresses);
            //System.Threading.ReaderWriterLock m_ItemInfoLock = new System.Threading.ReaderWriterLock();
            object m_ItemInfoLock = new object();
            volatile Dictionary<int, ItemInfo> m_ItemInfoCacheVanilla = null;
            volatile Dictionary<int, ItemInfo> m_ItemInfoCacheTBC = null;
            ItemDropDatabase m_ItemDropDatabase = null;
            public volatile bool m_ItemInfoUpdated = false;
            System.Net.CookieContainer m_DatabaseWowOneCookieContainer = null;
            DateTime m_DatabaseWowOneCookieContainerCookieCreationTime = DateTime.UtcNow;

            //need the cookie "dbVersion=0" for database.wow-one.com which is generated by accessing database.wow-one.com/?version=0
            private System.Net.CookieContainer DatabaseWowOneCookies_Get()
            {
                if (m_DatabaseWowOneCookieContainer == null
                || (DateTime.UtcNow - m_DatabaseWowOneCookieContainerCookieCreationTime).TotalHours > 1)
                {
                    m_DatabaseWowOneCookieContainer = new System.Net.CookieContainer();
                    var webRequest1 = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://database.wow-one.com/?version=0");
                    webRequest1.CookieContainer = m_DatabaseWowOneCookieContainer;
                    var test = webRequest1.GetResponse();
                    var test2 = test.GetResponseStream();
                    m_DatabaseWowOneCookieContainerCookieCreationTime = DateTime.UtcNow;
                }
                return m_DatabaseWowOneCookieContainer;
            }
            private void DatabaseWowOneCookies_Clear()
            {
                m_DatabaseWowOneCookieContainerCookieCreationTime = DateTime.UtcNow.AddDays(-1);
            }
            public Dictionary<int, ItemInfo> GetItemInfoCache(WowVersionEnum _WowVersion, bool _WaitForLoad = true)
            {
                if (_WowVersion == WowVersionEnum.Vanilla)
                {
                    if (m_ItemInfoCacheVanilla == null && _WaitForLoad == true)
                    {
                        if (_WaitForLoad == true)
                        {
                            Monitor.Enter(m_ItemInfoLock);
                        }
                        else
                        {
                            if (Monitor.TryEnter(m_ItemInfoLock) == false)
                                return m_ItemInfoCacheVanilla;
                        }
                        try
                        {
                            if (m_ItemInfoCacheVanilla == null)
                            {
                                if (System.IO.File.Exists(Constants.RPPDbDir + "VF_ItemInfoCache.dat"))
                                {
                                    Dictionary<int, ItemInfo> itemInfoCache = new Dictionary<int, ItemInfo>();
                                    if (VF_RealmPlayersDatabase.Utility.LoadSerialize(Constants.RPPDbDir + "VF_ItemInfoCache.dat", out itemInfoCache))
                                    {
                                        m_ItemInfoCacheVanilla = itemInfoCache;
                                    }
                                }
                                if (m_ItemDropDatabase == null)
                                    m_ItemDropDatabase = new ItemDropDatabase(Constants.RPPDbDir + "Database\\");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                        if (m_ItemInfoCacheVanilla == null)
                            m_ItemInfoCacheVanilla = new Dictionary<int, ItemInfo>();
                        Monitor.Exit(m_ItemInfoLock);
                        return m_ItemInfoCacheVanilla;
                    }
                    else
                        return m_ItemInfoCacheVanilla;
                }
                else if (_WowVersion == WowVersionEnum.TBC)
                {
                    if (m_ItemInfoCacheTBC == null && _WaitForLoad == true)
                    {
                        if (_WaitForLoad == true)
                        {
                            Monitor.Enter(m_ItemInfoLock);
                        }
                        else
                        {
                            if (Monitor.TryEnter(m_ItemInfoLock) == false)
                                return m_ItemInfoCacheTBC;
                        }
                        try
                        {
                            if (m_ItemInfoCacheTBC == null)
                            {
                                if (System.IO.File.Exists(Constants.RPPDbDir + "VF_ItemInfoCacheTBC.dat"))
                                {
                                    Dictionary<int, ItemInfo> itemInfoCache = new Dictionary<int, ItemInfo>();
                                    if (VF_RealmPlayersDatabase.Utility.LoadSerialize(Constants.RPPDbDir + "VF_ItemInfoCacheTBC.dat", out itemInfoCache))
                                    {
                                        m_ItemInfoCacheTBC = itemInfoCache;
                                    }
                                }
                                if(m_ItemDropDatabase == null)
                                    m_ItemDropDatabase = new ItemDropDatabase(Constants.RPPDbDir + "Database\\");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                        if (m_ItemInfoCacheTBC == null)
                            m_ItemInfoCacheTBC = new Dictionary<int, ItemInfo>();
                        Monitor.Exit(m_ItemInfoLock);
                        return m_ItemInfoCacheTBC;
                    }
                    else
                        return m_ItemInfoCacheTBC;
                }
                return null;
            }
            public bool IsItemInfoCacheLoaded(WowVersionEnum _WowVersion)
            {
                return (_WowVersion == WowVersionEnum.Vanilla && m_ItemInfoCacheVanilla != null) || (_WowVersion == WowVersionEnum.TBC && m_ItemInfoCacheTBC != null);
            }
            public ItemInfo GetItemInfo(int _ItemID, WowVersionEnum _WowVersion)
            {
                try
                { 
                    var itemInfoCache = GetItemInfoCache(_WowVersion, true);
                    if (itemInfoCache == null)
                        return null;
                    ItemInfo itemInfo = null;
                    //m_ItemInfoLock.AcquireReaderLock(1000);
                    Monitor.Enter(m_ItemInfoLock);
                    if (itemInfoCache.ContainsKey(_ItemID) == true)
                    {
                        itemInfo = itemInfoCache[_ItemID];
                        Monitor.Exit(m_ItemInfoLock);
                    }
                    else
                    {
                        Monitor.Exit(m_ItemInfoLock);
                        foreach (string itemDatabaseAddress in m_CurrentItemDatabaseOrder)
                        {
                            try
                            {
                                //System.Net.WebClient webClient = new System.Net.WebClient();
                                //var cook = new System.Collections.Specialized.NameValueCollection();
                                //cook.Add("dbVersion", "0");
                                System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();
                                //cookieContainer.Add(new Uri(itemDatabaseAddress), new System.Net.Cookie("PHPSESSID", "d617cebcf593d37bde6c9c8caa01ef18"));
                                var webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(itemDatabaseAddress + "ajax.php?item=" + _ItemID);
                                webRequest.Timeout = 5000;
                                webRequest.ReadWriteTimeout = 5000;
                                if (itemDatabaseAddress.Contains("database.wow-one.com"))
                                {
                                    if (_WowVersion == WowVersionEnum.Vanilla)
                                    {
                                        cookieContainer = DatabaseWowOneCookies_Get();//Hämta rätt cookies när det är vanilla
                                        //cookieContainer.SetCookies(new Uri("http://database.wow-one.com"), "dbVersion=0");
                                    }
                                }
                                else
                                {
                                    if (_WowVersion == WowVersionEnum.TBC)
                                        continue;//Bara stöd för database.wow-one.com när det är TBC
                                }
                                webRequest.CookieContainer = cookieContainer;
                                using (var webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse())
                                {
                                    using (System.IO.StreamReader reader = new System.IO.StreamReader(webResponse.GetResponseStream()))
                                    {
                                        string ajaxItemData = reader.ReadToEnd();
                                        if (ajaxItemData.StartsWith("$WowheadPower.registerItem"))//Success?
                                        {
                                            string[] itemData = ajaxItemData.Split('{', '}');
                                            if (itemData.Length == 3)//Success!(?)
                                            {
                                                itemInfo = new ItemInfo(_ItemID, ajaxItemData, itemDatabaseAddress);
                                                if (
                                                    (
                                                        itemDatabaseAddress.Contains("database.wow-one.com") && (
                                                            (_WowVersion == WowVersionEnum.Vanilla && webResponse.Headers.Get("Set-Cookie").Contains("dbVersion=0"))
                                                            || (_WowVersion == WowVersionEnum.TBC && webResponse.Headers.Get("Set-Cookie").Contains("dbVersion=1"))
                                                        )
                                                    )
                                                || itemDatabaseAddress.Contains("db.vanillagaming.org")
                                                || itemDatabaseAddress.Contains("db.valkyrie-wow.com"))
                                                {
                                                    lock(m_ItemInfoLock)
                                                    {
                                                        if (itemInfoCache.ContainsKey(_ItemID) == false)
                                                            itemInfoCache.Add(_ItemID, itemInfo);
                                                        else
                                                            itemInfoCache[_ItemID] = itemInfo;
                                                        m_ItemInfoUpdated = true;
                                                    }
                                                    if (itemDatabaseAddress != m_CurrentItemDatabaseOrder.First())
                                                    {
                                                        var newItemDatabaseOrder = new List<string>(StaticValues.ItemDatabaseAddresses);
                                                        newItemDatabaseOrder.Remove(itemDatabaseAddress);
                                                        newItemDatabaseOrder.Insert(0, itemDatabaseAddress);
                                                        m_CurrentItemDatabaseOrder = newItemDatabaseOrder;
                                                    }
                                                }
                                                else
                                                    DatabaseWowOneCookies_Clear();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            { }
                        }
                        //http://db.vanillagaming.org/ajax.php?item=19146

                        //http://db.vanillagaming.org/ajax.php?item=19146
                        //http://db.vanillagaming.org/images/icons/large/inv_bracer_04.jpg
                    }
                    return itemInfo;
                }
                catch(Exception ex)
                {
                    Logger.LogException(ex);
                    return null;
                }
            }
            public RPPDatabase GetRPPDatabase(bool _WaitUntilLoaded = true)
            {
                if (m_RPPDatabase == null && _WaitUntilLoaded == false)
                {
                    if (Monitor.TryEnter(m_RealmPlayersMutex, 100) == false)
                        return null;
                }
                else
                {
                    Monitor.Enter(m_RealmPlayersMutex);
                }
                RPPDatabase rppDatabase = m_RPPDatabase;
                if (rppDatabase == null)
                {
                    if (m_LoadRealmPlayersThread == null)
                    {
                        m_LoadRealmPlayersThread = new System.Threading.Thread(ReloadRealmPlayers);
                        m_LoadRealmPlayersThread.Start();
                        m_LastDatabaseUpdateTime = DatabaseLoader.GetLastDatabaseUpdateTimeUTC();
                    }
                    if (_WaitUntilLoaded == false)
                    {
                        Monitor.Exit(m_RealmPlayersMutex);
                        return null;
                    }
                    while (rppDatabase == null)
                    {
                        Monitor.Exit(m_RealmPlayersMutex);
                        System.Threading.Thread.Sleep(100);
                        Monitor.Enter(m_RealmPlayersMutex);
                        rppDatabase = m_RPPDatabase;
                    }
                }
                else if ((DateTime.UtcNow - m_LastLoadedDateTime).TotalMinutes > 30)
                {
                    if (m_LoadRealmPlayersThread == null)
                    {
                        DateTime lastDatabaseUpdateTime = DatabaseLoader.GetLastDatabaseUpdateTimeUTC();
                        if (lastDatabaseUpdateTime != m_LastDatabaseUpdateTime 
                            && (DateTime.UtcNow - lastDatabaseUpdateTime).TotalMinutes > 5) //Wait atleast 5 minutes after last database save
                        {
                            m_LastDatabaseUpdateTime = lastDatabaseUpdateTime;
                            m_LoadRealmPlayersThread = new System.Threading.Thread(ReloadRealmPlayers);
                            m_LoadRealmPlayersThread.Start();
                        }
                        else
                        {
                            m_LastLoadedDateTime = DateTime.UtcNow.AddMinutes(-24);
                        }
                    }
                }
                Monitor.Exit(m_RealmPlayersMutex);

                ApplicationInstance.Instance.BackupItemInfos();
                return rppDatabase;
            }
            public Code.ContributorStatistics GetContributorStatistics()
            {
                var contributorStatistics = m_ContributorStatistics;
                if (contributorStatistics == null)
                {
                    contributorStatistics = new Code.ContributorStatistics(GetRPPDatabase());
                    m_ContributorStatistics = contributorStatistics;
                }
                return contributorStatistics;
            }
            public static ApplicationInstance Instance
            {
                get
                {
                    return m_Instance;
                }
            }
            public VF_RealmPlayersDatabase.ItemDropDatabase GetItemDropDatabase()
            {
                return m_ItemDropDatabase;
            }

            public VF_RPDatabase.GuildSummary GetGuildSummary(VF_RealmPlayersDatabase.WowRealm _Realm, string _Guild)
            {
                var guildSummaryDatabase = GetGuildSummaryDatabase();
                return guildSummaryDatabase.GetGuildSummary(_Realm, _Guild);
            }

            public VF_RPDatabase.GuildSummaryDatabase GetGuildSummaryDatabase()
            {
                return DynamicReloader.GetData<VF_RPDatabase.GuildSummaryDatabase>(() =>
                {
                    VF_RPDatabase.GuildSummaryDatabase summaryDB = null;
                    summaryDB = VF_RPDatabase.GuildSummaryDatabase.LoadSummaryDatabase(Constants.RPPDbDir);
                    return summaryDB;
                }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 40; });
            }
            public VF_RPDatabase.ItemSummaryDatabase GetItemSummaryDatabase()
            {
                return DynamicReloader.GetData<VF_RPDatabase.ItemSummaryDatabase>(() =>
                {
                    VF_RPDatabase.ItemSummaryDatabase summaryDB = null;
                    summaryDB = VF_RPDatabase.ItemSummaryDatabase.LoadSummaryDatabase(Constants.RPPDbDir);
                    return summaryDB;
                }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 120; });
            }
            public VF_RPDatabase.PlayerSummaryDatabase GetPlayerSummaryDatabase()
            {
                return DynamicReloader.GetData<VF_RPDatabase.PlayerSummaryDatabase>(() =>
                {
                    VF_RPDatabase.PlayerSummaryDatabase summaryDB = null;
                    summaryDB = VF_RPDatabase.PlayerSummaryDatabase.LoadSummaryDatabase(Constants.RPPDbDir);
                    return summaryDB;
                }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 60; });
            }
            public VF_RDDatabase.GroupSummaryDatabase GetGroupSummaryDatabase()
            {
                return DynamicReloader.GetData<VF_RDDatabase.GroupSummaryDatabase>(() =>
                {
                    VF_RDDatabase.GroupSummaryDatabase summaryDB = null;
                    summaryDB = VF_RDDatabase.GroupSummaryDatabase.LoadSummaryDatabase(Constants.RDDbDir);
                    return summaryDB;
                }, (_RaidCollection, _LastLoadTime) => { return (DateTime.UtcNow - _LastLoadTime).TotalMinutes > 60; });
            }
        }
    }
}