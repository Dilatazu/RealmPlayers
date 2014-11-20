using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase.Achievments
{
    public class ItemsetAchievment : AchievmentBase
    {
        public readonly List<PlayerData.ItemInfo> m_Items;
        public ItemsetAchievment(List<PlayerData.ItemInfo> _Items, AchievmentType _Type, AchievmentBreadth _Breadth)
            : base(_Type, _Breadth)
        {
            m_Items = _Items;
        }
    }
}
