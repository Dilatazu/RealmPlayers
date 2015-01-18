using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;

using Old_RaidCollection = VF_RaidDamageDatabase.RaidCollection;
using Old_FightDataCollection = VF_RaidDamageDatabase.FightDataCollection;

using Utility = VF_RaidDamageDatabase.Utility;

using BossInformation = VF_RaidDamageDatabase.BossInformation;

namespace VF_RDDatabase
{
    [ProtoContract]
    public struct BossKillInfo
    {
        [ProtoMember(1)]
        public DateTime KillStartDateTime;
        [ProtoMember(2)]
        public DateTime KillEndDateTime;
        public bool IsOverlapping(BossKillInfo _KillInfo)
        {
            if ((_KillInfo.KillStartDateTime > KillStartDateTime
                    && _KillInfo.KillStartDateTime < KillEndDateTime)
                || (KillStartDateTime > _KillInfo.KillStartDateTime
                    && KillStartDateTime < _KillInfo.KillEndDateTime))
                return false;
            return true;
        }
        public int GetKillTimeSeconds()
        {
            double totalSeconds = (KillEndDateTime - KillStartDateTime).TotalSeconds;
            if (totalSeconds > 1209600/*3600 * 24 * 14*/) //No need to care about time spans longer than 14 days
                return 1209600;
            if (totalSeconds <= 0) //If for some reason the value is 0 or less(impossible) we change it to result in 28 days
                return 2419200;
            return (int)totalSeconds;
        }
    }
    [ProtoContract]
    public struct InstanceClearInfo
    {
        [ProtoMember(1)]
        public DateTime ClearStartDateTime;
        [ProtoMember(2)]
        public DateTime ClearEndDateTime;
        public bool IsOverlapping(InstanceClearInfo _ClearInfo)
        {
            if ((_ClearInfo.ClearStartDateTime > ClearStartDateTime
                    && _ClearInfo.ClearStartDateTime < ClearEndDateTime)
                || (ClearStartDateTime > _ClearInfo.ClearStartDateTime
                    && ClearStartDateTime < _ClearInfo.ClearEndDateTime))
                return false;
            return true;
        }
        public int GetClearTimeSeconds()
        {
            double totalSeconds = (ClearEndDateTime - ClearStartDateTime).TotalSeconds;
            if (totalSeconds > 1209600/*3600 * 24 * 14*/) //No need to care about time spans longer than 14 days
                return 1209600;
            if (totalSeconds <= 0) //If for some reason the value is 0 or less(impossible) we change it to result in 28 days
                return 2419200;
            return (int)totalSeconds;
        }
    }
    [ProtoContract]
    public class GroupInfo
    {
        [ProtoMember(1)]
        public VF_RealmPlayersDatabase.WowRealm Realm = WowRealm.Unknown;
        [ProtoMember(2)]
        public string GroupName = "Unknown";
        [ProtoMember(3)]
        public Dictionary<string, List<BossKillInfo>> FastestBossKills = new Dictionary<string, List<BossKillInfo>>();
        [ProtoMember(4)]
        public Dictionary<string, List<InstanceClearInfo>> FastestInstanceClears = new Dictionary<string, List<InstanceClearInfo>>();

        public void Update(GroupRaidCollection _GroupRC)
        {
            foreach (var raid in _GroupRC.Raids)
            {
                Dictionary<string, string[]> instanceRuns;
                if(BossInformation.InstanceRuns.TryGetValue(raid.Value.RaidInstance, out instanceRuns) == false)
                    continue;
                foreach (var instanceRun in instanceRuns)
                {
                    var instanceClearData = VF_RaidDamageDatabase.RaidInstanceClearData.Generate(raid, instanceRun.Value);
                    if (instanceClearData != null)
                    {
                        InstanceClearInfo instanceClearInfo = new InstanceClearInfo
                        {
                            ClearStartDateTime = instanceClearData.m_RaidStartClearTime,
                            ClearEndDateTime = instanceClearData.m_RaidEndClearTime
                        };
                        bool foundDuplicate = false;
                        if (FastestInstanceClears.ContainsKey(instanceRun.Key) == true)
                        {
                            var fastestClears = FastestInstanceClears[instanceRun.Key];
                            for (int i = 0; i < fastestClears.Count; ++i)
                            {
                                if (fastestClears[i].IsOverlapping(instanceClearInfo) == true)
                                {
                                    fastestClears[i] = instanceClearInfo;
                                    foundDuplicate = true;
                                }
                            }
                        }
                        if (foundDuplicate == false)
                        {
                            FastestInstanceClears.AddToList(instanceRun.Key, instanceClearInfo);
                        }
                    }
                }
                foreach (var bossFight in raid.Value.BossFights)
                {
                    if (bossFight.IsQualityHigh(true) == true && bossFight.AttemptType == AttemptType.KillAttempt)
                    {
                        var bossKillInfo = new BossKillInfo
                        {
                            KillStartDateTime = bossFight.StartDateTime,
                            KillEndDateTime = bossFight.EndDateTime,
                        };
                        bool foundDuplicate = false;
                        if (FastestBossKills.ContainsKey(bossFight.BossName) == true)
                        {
                            var fastestKills = FastestBossKills[bossFight.BossName];
                            for (int i = 0; i < fastestKills.Count; ++i)
                            {
                                if (fastestKills[i].IsOverlapping(bossKillInfo) == true)
                                {
                                    fastestKills[i] = bossKillInfo;
                                    foundDuplicate = true;
                                }
                            }
                        }
                        if (foundDuplicate == false)
                        {
                            FastestBossKills.AddToList(bossFight.BossName, bossKillInfo);
                        }
                    }
                }
            }
            foreach (var fastestClears in FastestInstanceClears)
            {
                fastestClears.Value.Sort((x, y) => x.GetClearTimeSeconds() - y.GetClearTimeSeconds());
            }
            foreach (var fastestKills in FastestBossKills)
            {
                fastestKills.Value.Sort((x, y) => x.GetKillTimeSeconds() - y.GetKillTimeSeconds());
            }
        }
    }
}
