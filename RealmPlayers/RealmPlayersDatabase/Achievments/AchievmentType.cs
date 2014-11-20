using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase.Achievments
{
    public enum AchievmentBreadth
    {
        Server,
        Faction,
        Guild,
        Class,
        Character,
    }
    public enum AchievmentType
    {
        //Item
        Achievment_Item_First,
        Achievment_Item_Few,
        Achievment_Item_Legendary,
        //Item

        //Itemset
        Achievment_Itemset_First,
        Achievment_Itemset_Enchanted,
        Achievment_Itemset_Rare,
        Achievment_Itemset_Epic,
        Achievment_Itemset_Highest_Stat,
        Achievment_Itemset_Collector, //Complete Sets
        //Itemset

        //PVP Rank or Honor or HKs
        Achievment_PVPStat_First_Rank,
        Achievment_PVPStat_Lifetime_Rank,
        Achievment_PVPStat_Fastest_Rank,

        Achievment_PVPStat_Above_1000,
        Achievment_PVPStat_Above_10000,
        Achievment_PVPStat_Above_100000,
        Achievment_PVPStat_Above_1000000,

        Achievment_PVPStat_Most_InDay,
        Achievment_PVPStat_Most_InWeek,
        Achievment_PVPStat_Most_InMonth,
        Achievment_PVPStat_Most_Total,
        //PVP Honor or HKs
    }
}
