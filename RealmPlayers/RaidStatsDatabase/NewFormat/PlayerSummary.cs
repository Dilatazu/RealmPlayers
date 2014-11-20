using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RDDatabase
{
    public class TotalPlayerBossStats
    {
        //List is sorted, newest fight is first in the list!
        public List<PlayerFightData> m_PlayerFightDatas = new List<PlayerFightData>();

        public int GetSamplesCount()
        {
            return m_PlayerFightDatas.Count;
        }
        public float GetAverageDPS(int _SampleMinSize, int _SampleMaxSize, int _SampleAcceptCount = -1)
        {
            return _GetAverage_Generic(_SampleMinSize, _SampleMaxSize, (_Value) => _Value.DPS, true, _SampleAcceptCount, null);
        }
        public float GetAverageDPS(int _SampleMinSize, int _SampleMaxSize, int _SampleAcceptCount, out List<VF_RDDatabase.PlayerFightData> _RetSamplesUsed)
        {
            _RetSamplesUsed = new List<PlayerFightData>();
            return _GetAverage_Generic(_SampleMinSize, _SampleMaxSize, (_Value) => _Value.DPS, true, _SampleAcceptCount, _RetSamplesUsed);
        }
        public float GetAverageEffectiveHPS(int _SampleMinSize, int _SampleMaxSize, int _SampleAcceptCount = -1)
        {
            return _GetAverage_Generic(_SampleMinSize, _SampleMaxSize, (_Value) => _Value.EffectiveHPS, true, _SampleAcceptCount, null);
        }
        public float GetAverageEffectiveHPS(int _SampleMinSize, int _SampleMaxSize, int _SampleAcceptCount, out List<VF_RDDatabase.PlayerFightData> _RetSamplesUsed)
        {
            _RetSamplesUsed = new List<PlayerFightData>();
            return _GetAverage_Generic(_SampleMinSize, _SampleMaxSize, (_Value) => _Value.EffectiveHPS, true, _SampleAcceptCount, _RetSamplesUsed);
        }
        public float GetAverageRawHPS(int _SampleMinSize, int _SampleMaxSize, int _SampleAcceptCount = -1)
        {
            return _GetAverage_Generic(_SampleMinSize, _SampleMaxSize, (_Value) => _Value.RawHPS, true, _SampleAcceptCount, null);
        }
        public float GetAverageDeaths(int _SampleMinSize, int _SampleMaxSize, int _SampleAcceptCount = -1)
        {
            return _GetAverage_Generic(_SampleMinSize, _SampleMaxSize, (_Value) => _Value.Deaths, false, _SampleAcceptCount, null);
        }
        public float GetAverageDeaths(int _SampleMinSize, int _SampleMaxSize, int _SampleAcceptCount, out List<VF_RDDatabase.PlayerFightData> _RetSamplesUsed)
        {
            _RetSamplesUsed = new List<PlayerFightData>();
            return _GetAverage_Generic(_SampleMinSize, _SampleMaxSize, (_Value) => _Value.Deaths, false, _SampleAcceptCount, _RetSamplesUsed);
        }
        //Example: _SampleMinSize=6, _SampleMaxSize=8
        //Latest 8 raids will be looked at, the 6 top values found will be used to calculate average
        public float _GetAverage_Generic(int _SampleMinSize, int _SampleMaxSize, Func<PlayerFightData, float> _GetValue, bool _HighValuePrio, int _SampleAcceptCount, List<VF_RDDatabase.PlayerFightData> RetSamplesUsed)
        {
            if (_SampleMinSize < 1 || _SampleMinSize > _SampleMaxSize)
                return -1;
            if (m_PlayerFightDatas.Count < _SampleMinSize)
            {
                if (m_PlayerFightDatas.Count < _SampleAcceptCount)
                    return -1;
                else
                    _SampleMinSize = _SampleAcceptCount;
            }
            if (m_PlayerFightDatas.Count < _SampleMaxSize)
                _SampleMaxSize = m_PlayerFightDatas.Count;

            List<VF_RDDatabase.PlayerFightData> sampleData = m_PlayerFightDatas.GetRange(0, _SampleMaxSize);

            sampleData = sampleData.OrderByDescending(_GetValue).ToList();
            if (sampleData.Count < _SampleMinSize)
                return -1;

            double averageValue = 0;
            //First _SampleMinSize DPS are the ones we calculate average for
            for (int i = 0; i < _SampleMinSize; ++i)
            {
                averageValue += _GetValue(sampleData[i]);
                if(RetSamplesUsed != null)
                    RetSamplesUsed.Add(sampleData[i]);
            }
            return (float)(averageValue / _SampleMinSize);
        }

        public void SortBossFights()
        {
            m_PlayerFightDatas = m_PlayerFightDatas.OrderByDescending((_Value) => _Value.CacheBossFight.StartDateTime).ToList();
        }
    }
    public class PlayerSummary
    {
        private string m_Name = "Unknown";
        private VF_RealmPlayersDatabase.WowRealm m_Realm = VF_RealmPlayersDatabase.WowRealm.Unknown;
        private Dictionary<string, TotalPlayerBossStats> m_PlayerBossStats = new Dictionary<string, TotalPlayerBossStats>();

        //List is sorted, oldest fight is first in the list!
        private List<BossFight> m_AttendedFights = new List<BossFight>();

        public string Name
        {
            get { return m_Name; }
        }
        public VF_RealmPlayersDatabase.WowRealm Realm
        {
            get { return m_Realm; }
        }
        //List is sorted, newest fight is last in the list!
        public List<BossFight> AttendedFights
        {
            get { return m_AttendedFights; }
        }
        public Dictionary<string, TotalPlayerBossStats> PlayerBossStats
        {
            get { return m_PlayerBossStats; }
        }

        public PlayerSummary(string _Name, VF_RealmPlayersDatabase.WowRealm _Realm)
        {
            m_Name = _Name;
            m_Realm = _Realm;
        }
        public BossFight GetLatestAttendedBossFight(string _Boss)
        {
            //m_AttendedFights är sorterad senaste raidet är sisst, så vi går baklänges!
            for (int i = m_AttendedFights.Count - 1; i >= 0; --i)
            {
                if (m_AttendedFights[i].BossName == _Boss)
                {
                    return m_AttendedFights[i];
                }
            }
            return null;
        }
        public DateTime GetLatestAttendedBossFightDateTime(string _Boss)
        {
            var latestAttendedBossFight = GetLatestAttendedBossFight(_Boss);
            if (latestAttendedBossFight == null)
                return DateTime.MinValue;
            return latestAttendedBossFight.StartDateTime;
        }
        public TotalPlayerBossStats GetBossStats(string _Boss)
        {
            TotalPlayerBossStats retValue;
            if (m_PlayerBossStats.TryGetValue(_Boss, out retValue) == false)
                return null;
            return retValue;
        }

        public float GenerateTotalAverageData(int _SampleCount, Func<TotalPlayerBossStats, float> _ValueFunc, Func<string, bool> _IncludeBossFunc = null)//, DateTime? _BossDateTimeRequirementParam = null, DateTime? _InstanceDateTimeRequirementParam = null)
        {
            DateTime _BossDateTimeRequirement = DateTime.Now.AddMonths(-2);
            DateTime _InstanceDateTimeRequirement = DateTime.Now.AddMonths(-1);

            //if (_BossDateTimeRequirementParam.HasValue == true)
            //    _BossDateTimeRequirement = _BossDateTimeRequirementParam.Value;

            //if (_InstanceDateTimeRequirementParam.HasValue == true)
            //    _InstanceDateTimeRequirement = _InstanceDateTimeRequirementParam.Value;

            bool attendedAtleastOnce = false;
            List<float> bossSamplesData = new List<float>();
            foreach (var bossStats in m_PlayerBossStats)
            {
                if (_IncludeBossFunc == null || _IncludeBossFunc(bossStats.Key) == true)
                {
                    if (bossStats.Value.m_PlayerFightDatas.Count > 0)
                    {
                        if (bossStats.Value.m_PlayerFightDatas.First().CacheBossFight.StartDateTime > _BossDateTimeRequirement)
                        {
                            float currValue = _ValueFunc(bossStats.Value);
                            if (currValue > 0)
                            {
                                bossSamplesData.Add(currValue);
                            }
                        }
                        if (bossStats.Value.m_PlayerFightDatas.First().CacheBossFight.StartDateTime > _InstanceDateTimeRequirement)
                        {
                            attendedAtleastOnce = true;
                        }
                    }
                }
            }
            var orderedBossData = bossSamplesData.OrderByDescending(_Value => _Value);

            if (attendedAtleastOnce == false || bossSamplesData.Count != _SampleCount)
                return 0.0f;

            int sampleNr = 0;
            double totalValue = 0;
            foreach (var bossDataValue in orderedBossData)
            {
                if (++sampleNr > _SampleCount)
                    break;

                totalValue += bossDataValue;
            }
            return (float)(totalValue / _SampleCount);
        }

        public void AddBossFightData(BossFight _BossFight, PlayerFightData _PlayerFightData)
        {
            if (_BossFight.IsQualityHigh(true))
            {
                if (m_PlayerBossStats.ContainsKey(_BossFight.BossName) == false)
                    m_PlayerBossStats.Add(_BossFight.BossName, new TotalPlayerBossStats());
                var bossFightDataList = m_PlayerBossStats[_BossFight.BossName];

                bossFightDataList.m_PlayerFightDatas.Add(_PlayerFightData);
            }
            m_AttendedFights.Add(_BossFight);
        }

        public void SortBossFights()
        {
            foreach (var bossStats in m_PlayerBossStats)
            {
                bossStats.Value.SortBossFights();
            }
            m_AttendedFights = m_AttendedFights.OrderBy((_Value) => _Value.StartDateTime).ToList();
        }

        /*public static PlayerSummary Generate(SummaryDatabase _SummaryDB, VF_RealmPlayersDatabase.WowRealm _Realm, string _Player)
        {
            PlayerSummary playerSummary = new PlayerSummary(_Player, _Realm);

            foreach (var groupRC in _SummaryDB.GroupRCs)
            {
                if (groupRC.Value.m_Realm != _Realm)
                    continue;

                foreach (var raid in groupRC.Value.Raids)
                {
                    foreach (var bossFight in raid.Value.BossFights)
                    {
                        if (bossFight.PlayerFightData.FindIndex((_Value) => _Value.Item1 == _Player) != -1)
                        {
                            playerSummary.m_AttendedFights.Add(bossFight);
                        }
                    }
                }
            }

            if (playerSummary.m_AttendedFights.Count == 0)
                return null;

            playerSummary.m_AttendedFights = playerSummary.m_AttendedFights.OrderByDescending((_Value) => _Value.StartDateTime).ToList();

            foreach (var attendedBossFight in playerSummary.m_AttendedFights)
            {
                if (attendedBossFight.AttemptType != AttemptType.KillAttempt)
                    continue;

                var playerDataIndex = attendedBossFight.PlayerFightData.FindIndex((_Value) => _Value.Item1 == _Player);
                if (playerDataIndex != -1)
                {
                    var playerFightData = attendedBossFight.PlayerFightData[playerDataIndex];

                    if (playerSummary.m_PlayerBossStats.ContainsKey(attendedBossFight.BossName) == false)
                        playerSummary.m_PlayerBossStats.Add(attendedBossFight.BossName, new TotalPlayerBossStats());
                    var bossFightDataList = playerSummary.m_PlayerBossStats[attendedBossFight.BossName];

                    bossFightDataList.m_PlayerFightDatas.Add(playerFightData.Item2);
                }
            }

            return playerSummary;
        }
        public static Dictionary<string, PlayerSummary> Generate(SummaryDatabase _SummaryDB)
        {
            Dictionary<string, PlayerSummary> generatedPlayerSummaries = new Dictionary<string, PlayerSummary>();
            foreach (var groupRC in _SummaryDB.GroupRCs)
            {
                foreach (var raid in groupRC.Value.Raids)
                {
                    foreach (var bossFight in raid.Value.BossFights)
                    {
                        if (bossFight.AttemptType != AttemptType.KillAttempt)
                            continue;

                        foreach (var playerData in bossFight.PlayerFightData)
                        {
                            string playerKeyName = "" + (int)groupRC.Value.m_Realm + playerData.Item1;
                            if (generatedPlayerSummaries.ContainsKey(playerKeyName) == false)
                            {
                                generatedPlayerSummaries.Add(playerKeyName, new PlayerSummary(playerData.Item1, groupRC.Value.Realm));
                            }
                            var currPlayerSummary = generatedPlayerSummaries[playerKeyName];

                            if(currPlayerSummary.m_PlayerBossStats.ContainsKey(bossFight.BossName) == false)
                                currPlayerSummary.m_PlayerBossStats.Add(bossFight.BossName, new TotalPlayerBossStats());
                            var bossFightDataList = currPlayerSummary.m_PlayerBossStats[bossFight.BossName];

                            bossFightDataList.m_PlayerFightDatas.Add(playerData.Item2);
                        }
                    }
                }
            }
            return generatedPlayerSummaries;
        }*/
    }
}
