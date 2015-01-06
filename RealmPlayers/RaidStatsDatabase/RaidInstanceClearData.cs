using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RaidDamageDatabase
{
    public class RaidInstanceClearData
    {
        public VF_RDDatabase.Raid m_Raid;
        public DateTime m_RaidStartClearTime;
        public DateTime m_RaidEndClearTime;

        public TimeSpan GetTimeSpan()
        {
            return (m_RaidEndClearTime - m_RaidStartClearTime);
        }

        public static RaidInstanceClearData Generate(KeyValuePair<int, VF_RDDatabase.Raid> _Raid, string[] _InstanceBosses)
        {
            DateTime earliestTime = DateTime.MaxValue;
            DateTime latestTime = DateTime.MinValue;

            int bossKills = 0;
            List<string> prevAddedBosses = new List<string>();
            foreach (var existingFight in _Raid.Value.BossFights)
            {
                if (_InstanceBosses.Contains(existingFight.BossName) == true)
                {
                    if (existingFight.StartDateTime < earliestTime)
                        earliestTime = existingFight.StartDateTime;

                    if (existingFight.EndDateTime > latestTime)
                        latestTime = existingFight.EndDateTime;

                    if (prevAddedBosses.Contains(existingFight.BossName))
                        continue;

                    if (existingFight.AttemptType == VF_RDDatabase.AttemptType.KillAttempt)
                    {
                        //if (BossInformation.LastInstanceBoss.ContainsKey(raid.Value.RaidInstance) == true)
                        //{
                        //    if (BossInformation.LastInstanceBoss[raid.Value.RaidInstance] == existingFight.FightName)
                        //    {
                        //        bossKills = BossInformation.BossCountInInstance[raid.Value.RaidInstance];
                        //        break;
                        //    }
                        //}
                        ++bossKills;
                        prevAddedBosses.Add(existingFight.BossName);
                    }
                    else if (existingFight.AttemptType == VF_RDDatabase.AttemptType.UnknownAttempt)
                    {//If unknown attempt we have to fetch bossfights and check manually
                        var bossFights = _Raid.Value.BossFights;

                        bool wasBossKill = false;
                        foreach (var bossFight in bossFights)
                        {
                            if (bossFight.StartDateTime == existingFight.StartDateTime)
                            {
                                if (bossFight.AttemptType == VF_RDDatabase.AttemptType.KillAttempt)
                                {
                                    wasBossKill = true;
                                }
                                break;
                            }
                        }
                        if (wasBossKill == true)
                        {
                            //if (BossInformation.LastInstanceBoss.ContainsKey(raid.Value.RaidInstance) == true)
                            //{
                            //    if (BossInformation.LastInstanceBoss[raid.Value.RaidInstance] == existingFight.FightName)
                            //    {
                            //        bossKills = BossInformation.BossCountInInstance[raid.Value.RaidInstance];
                            //        break;
                            //    }
                            //}
                            ++bossKills;
                            prevAddedBosses.Add(existingFight.BossName);
                        }
                    }
                }
            }
            if (bossKills == _InstanceBosses.Length)
            {
                //There was an instance clear!
                if (bossKills > 1)//Onyxia is not interesting
                {
                    return new RaidInstanceClearData
                    {
                        m_Raid = _Raid.Value,
                        m_RaidStartClearTime = earliestTime,
                        m_RaidEndClearTime = latestTime,
                    };
                }
            }
            return null;
        }
    }
}
