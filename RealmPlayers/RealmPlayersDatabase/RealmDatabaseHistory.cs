using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase
{
    public class RealmDatabaseHistory
    {
        public Dictionary<string, PlayerData.PlayerHistory> m_PlayersHistory = new Dictionary<string, PlayerData.PlayerHistory>();

        public DateTime GetEarliestHistoryData()
        {
            DateTime earliestHistoryData = DateTime.MaxValue;
            foreach (var playerHistory in m_PlayersHistory)
            {
                DateTime dateTime = playerHistory.Value.GetEarliestDateTime();
                if (dateTime < earliestHistoryData)
                    earliestHistoryData = dateTime;
            }
            return earliestHistoryData;
        }
        public DateTime GetLatestHistoryData()
        {
            DateTime latestHistoryData = DateTime.MaxValue;
            foreach (var playerHistory in m_PlayersHistory)
            {
                DateTime dateTime = playerHistory.Value.GetLatestDateTime();
                if (dateTime > latestHistoryData)
                    latestHistoryData = dateTime;
            }
            return latestHistoryData;
        }
        public bool ExistsEarlierThan(DateTime _EarlyDateTime)
        {
            foreach (var playerHistory in m_PlayersHistory)
            {
                if (playerHistory.Value.GetEarliestDateTime() < _EarlyDateTime)
                    return true;
            }
            return false;
        }
        public bool ExistsLaterThan(DateTime _LateDateTime)
        {
            foreach (var playerHistory in m_PlayersHistory)
            {
                if (playerHistory.Value.GetLatestDateTime() > _LateDateTime)
                    return true;
            }
            return false;
        }
    }
}
