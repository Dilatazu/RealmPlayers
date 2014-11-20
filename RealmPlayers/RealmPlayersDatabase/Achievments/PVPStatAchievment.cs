using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase.Achievments
{
    public enum PVPStatType
    {
        Rank,
        Honor,
        HK,
        DK,
        RPChange,
    }
    public class PVPStatAchievment : AchievmentBase
    {
        public readonly PVPStatType m_StatType;
        public readonly int m_Stat;

        public PVPStatAchievment(PVPStatType _StatType, int _Stat, AchievmentType _AchievmentType, AchievmentBreadth _Breadth) 
            : base(_AchievmentType, _Breadth)
        {
            m_StatType = _StatType;
            m_Stat = _Stat;
        }
    }
}
