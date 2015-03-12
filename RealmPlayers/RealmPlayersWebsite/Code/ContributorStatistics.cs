using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;
using UploadID = VF_RealmPlayersDatabase.UploadID;
using RPPDatabase = VF_RealmPlayersDatabase.Database;
using RealmDatabase = VF_RealmPlayersDatabase.RealmDatabase;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;

namespace RealmPlayersServer.Code
{
    public class ContributorStatisticItem
    {
        public int m_ContributorID = -1;
        public DateTime m_EarliestActiveUTC = DateTime.MaxValue;
        public DateTime m_LatestActiveUTC = DateTime.MinValue;
        //public int m_TotalInspectCount = 0;
        //public Dictionary<UploadID, int> m_Uploads = new Dictionary<UploadID, int>();
        //public int m_CharCount = 0;
        public Dictionary<string, int> m_PlayerInspects = new Dictionary<string, int>();
        public ContributorStatisticItem(int _ContributorID = -1)
        {
            m_ContributorID = _ContributorID;
        }
        public void AddInspectData(string _PlayerName, DateTime _Time)
        {
            if(m_PlayerInspects.ContainsKey(_PlayerName) == false)
                m_PlayerInspects.Add(_PlayerName, 0);
            m_PlayerInspects[_PlayerName] += 1;

            if(_Time < m_EarliestActiveUTC)
                m_EarliestActiveUTC = _Time;
            if(_Time > m_LatestActiveUTC)
                m_LatestActiveUTC = _Time;
        }
    }
    public class ContributorStatistics
    {
        static Dictionary<WowRealm, Dictionary<int, ContributorStatisticItem>> sm_ContributorStatisticData = new Dictionary<WowRealm, Dictionary<int, ContributorStatisticItem>>();
        static DateTime sm_LastGenerationTime = DateTime.MinValue;
        static System.Threading.Tasks.Task sm_LoadTask = null;
        static object sm_LoadTaskMutex = new object();
        public static void AssertInitialize(RPPDatabase _Database, bool _WaitUntilLoaded = false)
        {
            lock(sm_LoadTaskMutex)
            {
                if (sm_LoadTask == null || (DateTime.UtcNow - sm_LastGenerationTime).TotalMinutes > 600) //10 hours
                {
                    sm_LastGenerationTime = DateTime.UtcNow;
                    sm_LoadTask = new System.Threading.Tasks.Task(new Action(() => { Thread_GenerateData(_Database); }));
                    sm_LoadTask.Start();
                }
            }
            if (_WaitUntilLoaded == true && sm_LoadTask.IsCompleted == false)
                sm_LoadTask.Wait();
        }
        public static bool IsInitialized()
        {
            return sm_LoadTask != null && sm_LoadTask.IsCompleted;
        }
        public static Dictionary<WowRealm, Dictionary<int, ContributorStatisticItem>> GetData()
        {
            if(Code.ContributorStatistics.IsInitialized() == false)
                return null;
            return sm_ContributorStatisticData;
        }
        public static void Thread_GenerateData(RPPDatabase _Database)
        {
            for (int i = 0; i < 5; ++i)
            {
                var generatedContributorStatisticData = new Dictionary<WowRealm, Dictionary<int, ContributorStatisticItem>>();
                try
                {
                    ContributorStatisticItem lastUsedCSI = new ContributorStatisticItem();
                    foreach (var realm in _Database.GetRealms())
                    {
                        if (generatedContributorStatisticData.ContainsKey(realm.Key) == false)
                            generatedContributorStatisticData.Add(realm.Key, new Dictionary<int, ContributorStatisticItem>());
                        var realmCSD = generatedContributorStatisticData[realm.Key];

                        foreach (var playerHistory in realm.Value.PlayersHistory)
                        {
                            var playerUpdates = playerHistory.Value.GetUpdates();
                            foreach (var playerUpdate in playerUpdates)
                            {
                                var currContributorID = playerUpdate.GetContributorID();
                                if (lastUsedCSI.m_ContributorID != currContributorID)
                                {
                                    if (realmCSD.ContainsKey(currContributorID) == false)
                                        realmCSD.Add(currContributorID, new ContributorStatisticItem(currContributorID));
                                    lastUsedCSI = realmCSD[currContributorID];
                                }
                                lastUsedCSI.AddInspectData(playerHistory.Key, playerUpdate.GetTime());
                            }
                        }
                    }
                    sm_ContributorStatisticData = generatedContributorStatisticData;
                    break;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    System.Threading.Thread.Sleep(500);
                    Logger.ConsoleWriteLine("Thread_GenerateData() Trying again!");
                }
            }
        }
        //public bool IsGenerating()
        //{
        //    return sm_LoadTask != null;
        //}
        //public bool IsGenerationComplete() 
        //{
        //    return sm_LoadTask.IsCompleted;
        //}
        //public Dictionary<WowRealm, Dictionary<int, ContributorStatisticItem>> GetContributorStatisticsData()
        //{
        //    if(IsGenerationComplete() == false)
        //    {
        //        sm_LoadTask.Wait();
        //        if(IsGenerationComplete() == false)
        //        {
        //            throw new Exception("ContributorStatisticGeneration was not completed when it should have been");
        //        }
        //    }
        //    return m_ContributorStatisticData;
        //}
    }
}