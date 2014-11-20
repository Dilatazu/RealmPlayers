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
        Dictionary<WowRealm, Dictionary<int, ContributorStatisticItem>> m_ContributorStatisticData = new Dictionary<WowRealm, Dictionary<int, ContributorStatisticItem>>();
        System.Threading.Tasks.Task m_LoadTask;
        public ContributorStatistics(RPPDatabase _Database)
        {
            m_LoadTask = new System.Threading.Tasks.Task(new Action(() => { Thread_GenerateData(_Database); }));
            m_LoadTask.Start();
        }
        public void Thread_GenerateData(RPPDatabase _Database)
        {
            for (int i = 0; i < 5; ++i)
            {
                m_ContributorStatisticData = new Dictionary<WowRealm, Dictionary<int, ContributorStatisticItem>>();
                try
                {
                    ContributorStatisticItem lastUsedCSI = new ContributorStatisticItem();
                    foreach (var realm in _Database.GetRealms())
                    {
                        if (m_ContributorStatisticData.ContainsKey(realm.Key) == false)
                            m_ContributorStatisticData.Add(realm.Key, new Dictionary<int, ContributorStatisticItem>());
                        var realmCSD = m_ContributorStatisticData[realm.Key];

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
        public bool IsGenerationComplete() 
        {
            return m_LoadTask.IsCompleted;
        }
        public Dictionary<WowRealm, Dictionary<int, ContributorStatisticItem>> GetContributorStatisticsData()
        {
            if(IsGenerationComplete() == false)
            {
                m_LoadTask.Wait();
                if(IsGenerationComplete() == false)
                {
                    throw new Exception("ContributorStatisticGeneration was not completed when it should have been");
                }
            }
            return m_ContributorStatisticData;
        }
    }
}