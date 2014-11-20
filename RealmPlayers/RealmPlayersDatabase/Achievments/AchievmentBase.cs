using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase.Achievments
{
    public class AchievmentBase
    {
        public readonly AchievmentType m_Type;
        public readonly AchievmentBreadth m_Breadth;
        public AchievmentBase(AchievmentType _Type, AchievmentBreadth _Breadth)
        {
            m_Type = _Type;
            m_Breadth = _Breadth;
        }
    }
}
