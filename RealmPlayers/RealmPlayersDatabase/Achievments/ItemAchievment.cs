using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase.Achievments
{
    public class ItemAchievment : AchievmentBase
    {
        public readonly PlayerData.ItemInfo m_Item;

        public ItemAchievment(PlayerData.ItemInfo _Item, AchievmentType _Type, AchievmentBreadth _Breadth) 
            : base(_Type, _Breadth)
        {
            m_Item = _Item;
        }
    }
}
