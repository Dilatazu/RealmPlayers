using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;

namespace RealmPlayersServer
{
    public class PerformanceStatistics
    {
        [ProtoContract]
        public class PageAccessItem
        {
            [ProtoMember(1)]
            public string m_ClientIP;
            [ProtoMember(2)]
            public DateTime m_StartRequestDateTime;
            [ProtoMember(3)]
            public int m_ExecutionTimeMs;
            [ProtoMember(4)]
            public int m_PreRenderTimeMs;
            [ProtoMember(5)]
            public int m_RenderTimeMs;
            [ProtoMember(6)]
            public int m_PageSize;
            [ProtoMember(7)]
            public string m_FullUrl;
            [ProtoMember(8)]
            public string m_ReferrerUrl;
            [ProtoMember(9)]
            public string m_RedirectedTo;
        }
        [ProtoContract]
        public class PerfPageItem
        {
            [ProtoMember(1)]
            public string m_PageName;
            [ProtoMember(2)]
            public List<PageAccessItem> m_PageAccessItems = new List<PageAccessItem>();

            public float GetAverageExecutionTime()
            {
                if(m_PageAccessItems.Count < 1)
                    return 0.0f;
                int startI = m_PageAccessItems.Count - 100;
                if (startI < 0)
                    startI = 0;
                double totalLoadTimeMs = 0;
                for (int i = startI; i < m_PageAccessItems.Count; ++i)
                {
                    totalLoadTimeMs += m_PageAccessItems[i].m_ExecutionTimeMs;
                }
                double averageLoadTimeMs = totalLoadTimeMs / (m_PageAccessItems.Count - startI);

                return (float)(averageLoadTimeMs / 1000.0);//Convert to seconds
            }
        }
        [ProtoContract]
        public class PerfStatisticData
        {
            [ProtoMember(1)]
            public Dictionary<string, PerfPageItem> m_PerfPageItems = new Dictionary<string, PerfPageItem>();
            [ProtoMember(2)]
            public Dictionary<string, List<PageAccessItem>> m_UserAccesses = new Dictionary<string, List<PageAccessItem>>();

            public DateTime GetEarliestDataDateTime()
            {
                DateTime earliestDateFound = DateTime.MaxValue;
                foreach (var currUserAccess in m_UserAccesses)
                {
                    if(currUserAccess.Value.Count > 0)
                    {
                        if (currUserAccess.Value.First().m_StartRequestDateTime < earliestDateFound)
                            earliestDateFound = currUserAccess.Value.First().m_StartRequestDateTime;
                    }
                }
                return earliestDateFound;
            }
            public bool ExistsEarlierThan(DateTime _DateTime)
            {
                foreach (var currUserAccess in m_UserAccesses)
                {
                    if (currUserAccess.Value.Count > 0)
                    {
                        if (currUserAccess.Value.First().m_StartRequestDateTime < _DateTime)
                            return true;
                    }
                }
                return false;
            }
            public void AddOldData(PerfStatisticData _OldData)
            {
                foreach (var currOldPerfPageItem in _OldData.m_PerfPageItems)
                {
                    if (m_PerfPageItems.ContainsKey(currOldPerfPageItem.Key) == true)
                    {
                        m_PerfPageItems[currOldPerfPageItem.Key].m_PageAccessItems.InsertRange(0, currOldPerfPageItem.Value.m_PageAccessItems);
                    }
                    else
                    {
                        m_PerfPageItems.Add(currOldPerfPageItem.Key, currOldPerfPageItem.Value);
                    }
                }
                foreach (var currOldUserAccess in _OldData.m_UserAccesses)
                {
                    if (m_UserAccesses.ContainsKey(currOldUserAccess.Key) == true)
                    {
                        m_UserAccesses[currOldUserAccess.Key].InsertRange(0, currOldUserAccess.Value);
                    }
                    else
                    {
                        m_UserAccesses.Add(currOldUserAccess.Key, currOldUserAccess.Value);
                    }
                }
            }
            public PerfStatisticData ExtractOldData(DateTime _NewestData)
            {
                PerfStatisticData oldData = new PerfStatisticData();
                foreach (var currPerfPageItem in m_PerfPageItems)
                {
                    int lastIndex = currPerfPageItem.Value.m_PageAccessItems.FindLastIndex((_Value) => { return _Value.m_StartRequestDateTime < _NewestData; });
                    if (lastIndex != -1)
                    {
                        PerfPageItem newPerfPageItem = new PerfPageItem();
                        newPerfPageItem.m_PageName = currPerfPageItem.Value.m_PageName;
                        newPerfPageItem.m_PageAccessItems = currPerfPageItem.Value.m_PageAccessItems.GetRange(0, lastIndex + 1);
                        currPerfPageItem.Value.m_PageAccessItems.RemoveRange(0, lastIndex + 1);
                        oldData.m_PerfPageItems.Add(currPerfPageItem.Key, newPerfPageItem);
                    }
                }
                foreach (var currUserAccess in m_UserAccesses)
                {
                    int lastIndex = currUserAccess.Value.FindLastIndex((_Value) => { return _Value.m_StartRequestDateTime < _NewestData; });
                    if (lastIndex != -1)
                    {
                        List<PageAccessItem> newPerfAccessItems = currUserAccess.Value.GetRange(0, lastIndex + 1);
                        currUserAccess.Value.RemoveRange(0, lastIndex + 1);
                        oldData.m_UserAccesses.Add(currUserAccess.Key, newPerfAccessItems);
                    }
                }


                return oldData;
            }
        }
        private static PerfStatisticData sm_PerfStatsData = null;
        private static object sm_PerfStatsDataLock = new object();
        public static void AssertInitialize()
        {
            if (sm_PerfStatsData == null)
            {
                lock (sm_PerfStatsDataLock)
                {
                    if (sm_PerfStatsData == null)
                    {
                        sm_PerfStatsData = LoadStatistics(DateTime.UtcNow.AddDays(1));
                    }
                }
            }
        }
        public static PerfStatisticData LoadStatistics(DateTime _EarliestDateTime)
        {
            PerfStatisticData retData = null;
            string statsNowFilename = Constants.RPPDbDir + "PerformanceStatistics/PerfStats_Now.dat";
            if (System.IO.File.Exists(statsNowFilename) == true)
            {
                VF_RealmPlayersDatabase.Utility.LoadSerialize(statsNowFilename, out retData);
                if (_EarliestDateTime < DateTime.UtcNow) //Add History
                {
                    string[] pathDirs = System.IO.Directory.GetDirectories(Constants.RPPDbDir + "PerformanceStatistics");
                    DateTime earliestStatisticsDate = DateTime.MaxValue;
                    foreach (var pathDir in pathDirs)
                    {
                        try
                        {
                            string[] yearMonth = pathDir.Split('\\', '/').Last().Split('_');
                            if (yearMonth.Count() == 2)
                            {
                                if (yearMonth[0].Length == 4 && yearMonth[1].Length == 2)
                                {
                                    int year = int.Parse(yearMonth[0]);
                                    int month = int.Parse(yearMonth[1]);
                                    if (earliestStatisticsDate.Year < year || earliestStatisticsDate.Month < month)
                                        earliestStatisticsDate = new DateTime(year, month, 1);
                                }
                            }
                        }
                        catch (Exception)
                        { }
                    }
                    if (earliestStatisticsDate < DateTime.UtcNow)
                    {
                        for (DateTime currDate = DateTime.UtcNow.Date.AddDays(1); currDate >= earliestStatisticsDate; currDate = currDate.AddDays(-1))
                        {
                            string statsLoadDateFilename = Constants.RPPDbDir + "PerformanceStatistics/" + currDate.ToString("yyyy_MM") + "/PerfStats_" + currDate.ToString("dd") + ".dat";
                            if (System.IO.File.Exists(statsLoadDateFilename) == true)
                            {
                                PerfStatisticData oldData = null;
                                VF_RealmPlayersDatabase.Utility.LoadSerialize(statsNowFilename, out oldData);
                                retData.AddOldData(oldData);
                            }
                        }
                    }
                }
            }
            else
            {
                retData = new PerfStatisticData();
            }
            return retData;
        }
        public static void SaveStatistics()
        {
            return;//DISABLED FOR NOW
            AssertInitialize();
            string statsNowFilename = Constants.RPPDbWriteDir + "PerformanceStatistics/PerfStats_Now.dat";
            VF_RealmPlayersDatabase.Utility.AssertFilePath(statsNowFilename);
            lock (sm_PerfStatsDataLock)
            {
                DateTime saveDateTime = sm_PerfStatsData.GetEarliestDataDateTime();
                try
                {
                    while (saveDateTime.AddDays(1) < DateTime.UtcNow)
                    {
                        if (sm_PerfStatsData.ExistsEarlierThan(saveDateTime.AddDays(1)) == true)
                        {
                            if (sm_PerfStatsData.ExistsEarlierThan(saveDateTime) == false)
                            {
                                bool removeOldData = (DateTime.UtcNow - saveDateTime.AddDays(1)).TotalDays > 7;
                                if (removeOldData == true)
                                {
                                    string statsSaveDateFilename = Constants.RPPDbDir + "PerformanceStatistics/" + saveDateTime.ToString("yyyy_MM") + "/PerfStats_" + saveDateTime.ToString("dd") + ".dat";
                                    VF_RealmPlayersDatabase.Utility.AssertFilePath(statsSaveDateFilename);
                                    PerfStatisticData oldPerfStatsData = sm_PerfStatsData.ExtractOldData(saveDateTime.AddDays(1));
                                    VF_RealmPlayersDatabase.Utility.SaveSerialize(statsSaveDateFilename, oldPerfStatsData);
                                }
                            }
                            else
                                Logger.ConsoleWriteLine("Error occurred in history data saving", ConsoleColor.Red);
                        }
                        saveDateTime = saveDateTime.AddMonths(1);
                    }
                    VF_RealmPlayersDatabase.Utility.SaveSerialize(statsNowFilename, sm_PerfStatsData);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }
        public static void AddStatistics(HttpContext _ContextObject, int _PreRenderTimeMs, int _RenderTimeMs, int _PageSize)
        {
            return;//DISABLED FOR NOW
            AssertInitialize();
            string clientIP = _ContextObject.Request.UserHostAddress;
            if (clientIP == null)
                clientIP = "NULL";
            PageAccessItem newPageAccess = new PageAccessItem();
            newPageAccess.m_FullUrl = _ContextObject.Request.RawUrl;
            newPageAccess.m_ReferrerUrl = (_ContextObject.Request.UrlReferrer != null ? _ContextObject.Request.UrlReferrer.ToString() : "");
            newPageAccess.m_StartRequestDateTime = _ContextObject.Timestamp.ToUniversalTime();
            newPageAccess.m_ExecutionTimeMs = (int)(DateTime.UtcNow - newPageAccess.m_StartRequestDateTime).TotalMilliseconds;
            newPageAccess.m_PreRenderTimeMs = _PreRenderTimeMs;
            newPageAccess.m_RenderTimeMs = _RenderTimeMs;
            newPageAccess.m_PageSize = _PageSize;
            newPageAccess.m_ClientIP = clientIP;
            if (_ContextObject.Response.IsRequestBeingRedirected == true)
                newPageAccess.m_RedirectedTo = _ContextObject.Response.RedirectLocation;
            else
                newPageAccess.m_RedirectedTo = "";

            lock (sm_PerfStatsDataLock)
            {
                {//Add to m_PerfPageItems
                    string pageName = _ContextObject.Request.Url.AbsolutePath;
                    PerfPageItem perfPageItem = null;
                    if (sm_PerfStatsData.m_PerfPageItems.TryGetValue(pageName, out perfPageItem) == false)
                    {
                        perfPageItem = new PerfPageItem();
                        perfPageItem.m_PageName = pageName;
                        sm_PerfStatsData.m_PerfPageItems.Add(pageName, perfPageItem);
                    }
                    perfPageItem.m_PageAccessItems.Add(newPageAccess);
                }
                {//Add to m_UserAccesses
                    List<PageAccessItem> pageAccessItems = null;
                    if (sm_PerfStatsData.m_UserAccesses.TryGetValue(clientIP, out pageAccessItems) == false)
                    {
                        pageAccessItems = new List<PageAccessItem>();
                        sm_PerfStatsData.m_UserAccesses.Add(clientIP, pageAccessItems);
                    }
                    if (pageAccessItems != null)
                    {
                        pageAccessItems.Add(newPageAccess);
                    }
                    else
                    {
                        Logger.ConsoleWriteLine("pageAccessItems was null!!!", ConsoleColor.Red);
                    }
                }
            }
            LogExecutionTimes();
        }
        public static float GetAverageExecutionTime(string _PageName)
        {
            AssertInitialize();
            PerfPageItem perfPageItem = null;
            lock (sm_PerfStatsDataLock)
            {
                if (sm_PerfStatsData.m_PerfPageItems.TryGetValue(_PageName, out perfPageItem) == true)
                {
                    return perfPageItem.GetAverageExecutionTime();
                }
            }
            return -1.0f;
        }
        public static Dictionary<string, float> GetAverageExecutionTimes()
        {
            AssertInitialize();
            Dictionary<string, float> averageExecutionTimes = new Dictionary<string, float>();
            lock (sm_PerfStatsDataLock)
            {
                foreach (var perfPageItems in sm_PerfStatsData.m_PerfPageItems)
                {
                    averageExecutionTimes.Add(perfPageItems.Key, perfPageItems.Value.GetAverageExecutionTime());
                }
            }
            return averageExecutionTimes;
        }
        private static DateTime sm_LastExecutionTimeLog = DateTime.UtcNow;
        public static void LogExecutionTimes()
        {
            if((DateTime.UtcNow - sm_LastExecutionTimeLog).Minutes >= 30)
            {
                sm_LastExecutionTimeLog = DateTime.UtcNow;
                string LogString = "";
                var averageExecutionTimes = GetAverageExecutionTimes();
                foreach (var averageExecutionTime in averageExecutionTimes)
                {
                    LogString += averageExecutionTime.Key + "(" + averageExecutionTime.Value.ToStringDot("0.000") + "),";
                }
                Logger.ConsoleWriteLine(LogString, ConsoleColor.Magenta);
            }
        }
    }
}