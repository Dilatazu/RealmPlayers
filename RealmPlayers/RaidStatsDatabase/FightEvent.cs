using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RaidDamageDatabase
{
    public enum FightEventEnum
    {
        Unknown,
        UnitDeath,
        PhaseChange,
        BossAt20Percentage,
        BossHealth,
    }
    public class FightEvent
    {
        public FightEventEnum m_FightEvent = FightEventEnum.Unknown;
        public string m_StringData = "Unknown";
        public int m_TimeIntoTheFight = -1;

        public static List<FightEvent> GenerateFightEvents(RaidBossFight _Fight)
        {
            //var bossUnitData = _Fight.GetUnitData(_Fight.m_Fight.FightName);
            //if (bossUnitData == null)
            //{
            //    bossUnitData = null;
            //}
            //double bossDmgTaken = bossUnitData.DmgTaken;
            List<FightEvent> fightEvents = new List<FightEvent>();

            //bool added20PercentageEvent = false;

            var fightDetails = _Fight.GetFightDetails();
            int firstTime = _Fight.GetFightData().TimeSlices.First().Time;
            //TimeSlice lastTimeSlice = null;
            for (int i = 0; i < fightDetails.Count; ++i)
            {
                var fightDetail = fightDetails[i];

                foreach (var fightEvent in fightDetail.Events)
                {
                    if (fightEvent.StartsWith("Phase"))
                    {
                        string phaseType = fightEvent.Split(new char[] { '=', ' ' })[0];
                        if (phaseType.EndsWith("_Y"))
                            phaseType = phaseType.Substring(0, phaseType.Length - 2);

                        fightEvents.Add(new FightEvent
                        {
                            m_FightEvent = FightEventEnum.PhaseChange,
                            m_StringData = phaseType,
                            m_TimeIntoTheFight = fightDetail.Time
                        });
                    }
                    else if (fightEvent.StartsWith("BossHealth"))
                    {
                        fightEvents.Add(new FightEvent
                        {
                            m_FightEvent = FightEventEnum.BossHealth,
                            m_StringData = fightEvent,
                            m_TimeIntoTheFight = fightDetail.Time
                        });
                    }
                }
                foreach (var unitData in fightDetail.UnitDatas)
                {
                    if (unitData.Value.I.Death > 0)
                    {
                        fightEvents.Add(new FightEvent
                        {
                            m_FightEvent = FightEventEnum.UnitDeath,
                            m_StringData = unitData.Key,
                            m_TimeIntoTheFight = fightDetail.Time
                        });
                    }
                }
            }
            return fightEvents;
        }
    }
}
