using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;
using GearData = VF_RealmPlayersDatabase.PlayerData.GearData;
using DatabaseItemInfo = VF_RealmPlayersDatabase.PlayerData.ItemInfo;
using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

namespace RealmPlayersServer.Code
{
    public enum GearType
    {
        Latest,
        PVP,
        Tank,
        Heal,
        DPS,
        PVP_Heal,
        PVP_DPS,
        Random,
    }
    public class GearAnalyze
    {
        public struct ItemStat
        {
            public ItemStatType StatType;
            public int NormalValue;
            public int EnchantValue;
            public int GemValue;
            public ItemStat(ItemStatType _Type, int _NormalValue, int _EnchantValue, int _GemValue)
            {
                StatType = _Type;
                NormalValue = _NormalValue;
                EnchantValue = _EnchantValue;
                GemValue = _GemValue;
            }
        }
        //public static Dictionary<GearType, Tuple<int, GearData>> GetGearSets(PlayerHistory _PlayerHistory)
        //{
        //    Dictionary<GearType, Tuple<int, GearData>> gearSets = new Dictionary<GearType, Tuple<int, GearData>>();
        //    var commonGearSets = _PlayerHistory.GetMostCommonGearSets();
        //    foreach(var commonGearSet in commonGearSets)
        //    {
        //        var gearStats = GenerateGearStats(commonGearSet.Gear);
        //        var gearType = GenerateGearType(gearStats);
        //        var gearItemLevel = GetStat(gearStats, ItemStatType.Item_Level);
        //        if (gearSets.ContainsKey(gearType) == true)
        //        {
        //            if (gearItemLevel > gearSets[gearType].Item1)
        //                gearSets[gearType] = new Tuple<int, GearData>(gearItemLevel, commonGearSet.Gear);
        //        }
        //        else
        //            gearSets.Add(gearType, new Tuple<int, GearData>(gearItemLevel, commonGearSet.Gear));
        //    }
        //    return gearSets;
        //}
        public static int GetStat(Dictionary<ItemStatType, ItemStat> _Stats, ItemStatType _Stat)
        {
            return _Stats.ContainsKey(_Stat) ? (_Stats[_Stat].NormalValue + _Stats[_Stat].EnchantValue) : 0;
        }
        public static int GetStat(List<ItemStat> _Stats, ItemStatType _StatType)
        {
            int index = _Stats.FindIndex((ItemStat _Stat) => { return _Stat.StatType == _StatType; });
            return (index != -1) ? (_Stats[index].NormalValue + _Stats[index].EnchantValue) : 0;
        }
        public static List<ItemStat> GenerateGearStats(GearData _Gear, WowVersionEnum _WowVersion)
        {
            return GenerateGearStats(_Gear.Items.Values, _WowVersion);
        }
        public static List<ItemStat> GenerateGearStats(IEnumerable<DatabaseItemInfo> _Items, WowVersionEnum _WowVersion)
        {
            List<ItemStat> totalStats = new List<ItemStat>();
            foreach (var item in _Items)
            {
                try
                {
                    var itemInfo = DatabaseAccess.GetItemInfo(item.ItemID, _WowVersion);
                    var stats = ItemStats.ParseAjaxTooltip(itemInfo.GetAjaxTooltip(_WowVersion, item.ItemID, item.SuffixID, item.EnchantID, item.UniqueID, item.GemIDs));

                    foreach (var stat in stats)
                    {
                        ItemStat[] addStats = null;
                        int normalValue = 0;
                        int enchantValue = 0;
                        int gemValue = 0;
                        if (stat.Enchant == true)
                            enchantValue = stat.Value;
                        else if (stat.Gem == true)
                            gemValue = stat.Value;
                        else
                            normalValue = stat.Value;
                        if (stat.Type == ItemStatType.Spell_Damage_and_Healing)
                        {
                            addStats = new ItemStat[] { new ItemStat(ItemStatType.Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Spell_Healing, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Frost_Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Nature_Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Arcane_Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Shadow_Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Fire_Spell_Damage, normalValue, enchantValue, gemValue) };
                        }
                        else if (stat.Type == ItemStatType.Spell_Damage)
                        {
                            addStats = new ItemStat[] { new ItemStat(ItemStatType.Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Frost_Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Nature_Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Arcane_Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Shadow_Spell_Damage, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Fire_Spell_Damage, normalValue, enchantValue, gemValue) };
                        }
                        else if (stat.Type == ItemStatType.All_Resistances)
                        {
                            addStats = new ItemStat[] { 
                                new ItemStat(ItemStatType.Shadow_Resistance, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Fire_Resistance, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Nature_Resistance, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Frost_Resistance, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Arcane_Resistance, normalValue, enchantValue, gemValue)
                            };
                        }
                        else if (stat.Type == ItemStatType.Attack_Power)
                        {
                            addStats = new ItemStat[] { 
                                new ItemStat(ItemStatType.Attack_Power, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Ranged_Attack_Power, normalValue, enchantValue, gemValue)
                            };
                        }
                        else if(stat.Type == ItemStatType.HMp5)
                        {
                            addStats = new ItemStat[] { 
                                new ItemStat(ItemStatType.Hp5, normalValue, enchantValue, gemValue)
                                , new ItemStat(ItemStatType.Mp5, normalValue, enchantValue, gemValue)
                            };
                        }
                        else
                        {
                            addStats = new ItemStat[] { new ItemStat(stat.Type, normalValue, enchantValue, gemValue) };
                        }
                        foreach (var addStat in addStats)
                        {
                            if (addStat.StatType == ItemStatType.Unknown)
                                continue;
                            int statIndex = totalStats.FindIndex((ItemStat _Stat) => { return addStat.StatType == _Stat.StatType; });
                            if (statIndex != -1)
                            {
                                var currStat = totalStats[statIndex];
                                currStat.NormalValue += addStat.NormalValue;
                                currStat.EnchantValue += addStat.EnchantValue;
                                currStat.GemValue += addStat.GemValue;
                                totalStats[statIndex] = currStat;
                            }
                            else
                                totalStats.Add(addStat);
                        }
                    }
                }
                catch (Exception)
                {}
            }
            List<ItemStatType> removeStats = new List<ItemStatType>();
            int spellDmg = GetStat(totalStats, ItemStatType.Spell_Damage);
            if (spellDmg == GetStat(totalStats, ItemStatType.Frost_Spell_Damage)) removeStats.Add(ItemStatType.Frost_Spell_Damage);
            if (spellDmg == GetStat(totalStats, ItemStatType.Fire_Spell_Damage)) removeStats.Add(ItemStatType.Fire_Spell_Damage);
            if (spellDmg == GetStat(totalStats, ItemStatType.Arcane_Spell_Damage)) removeStats.Add(ItemStatType.Arcane_Spell_Damage);
            if (spellDmg == GetStat(totalStats, ItemStatType.Nature_Spell_Damage)) removeStats.Add(ItemStatType.Nature_Spell_Damage);
            if (spellDmg == GetStat(totalStats, ItemStatType.Shadow_Spell_Damage)) removeStats.Add(ItemStatType.Shadow_Spell_Damage);

            int spellResistance = 0;
            if (spellResistance == GetStat(totalStats, ItemStatType.Frost_Resistance)) removeStats.Add(ItemStatType.Frost_Resistance);
            if (spellResistance == GetStat(totalStats, ItemStatType.Fire_Resistance)) removeStats.Add(ItemStatType.Fire_Resistance);
            if (spellResistance == GetStat(totalStats, ItemStatType.Arcane_Resistance)) removeStats.Add(ItemStatType.Arcane_Resistance);
            if (spellResistance == GetStat(totalStats, ItemStatType.Nature_Resistance)) removeStats.Add(ItemStatType.Nature_Resistance);
            if (spellResistance == GetStat(totalStats, ItemStatType.Shadow_Resistance)) removeStats.Add(ItemStatType.Shadow_Resistance);

            if (GetStat(totalStats, ItemStatType.Attack_Power) == GetStat(totalStats, ItemStatType.Ranged_Attack_Power))
                removeStats.Add(ItemStatType.Ranged_Attack_Power);

            foreach (var removeStat in removeStats)
            {
                int removeIndex = totalStats.FindIndex((_Value) => _Value.StatType == removeStat);
                if (removeIndex != -1)
                {
                    totalStats.RemoveAt(removeIndex);
                }
            }

            return totalStats;
        }
        //Use only if lazy
        //public static GearType GenerateGearType(GearData _Gear)
        //{
        //    return GenerateGearType(GenerateGearStats(_Gear));
        //}
        //public static GearType GenerateGearType(List<ItemStat> _TotalStats)
        //{
        //    Dictionary<ItemStatType, ItemStat> gearStats = new Dictionary<ItemStatType, ItemStat>();
        //    Dictionary<ItemStatType, ItemStat> gearEnchantStats = new Dictionary<ItemStatType, ItemStat>();
        //    foreach (var stat in _TotalStats)
        //    {
        //        if (stat.Enchant == false)
        //            gearStats.Add(stat.Type, stat);
        //        else
        //            gearEnchantStats.Add(stat.Type, stat);
        //    }

        //    int stamina = GetStat(gearStats, ItemStatType.Stamina);
        //    int agility = GetStat(gearStats, ItemStatType.Agility);
        //    int strength = GetStat(gearStats, ItemStatType.Strength);
        //    int intellect = GetStat(gearStats, ItemStatType.Intellect);
        //    int spirit = GetStat(gearStats, ItemStatType.Spirit);
        //    int spellDamage = GetStat(gearStats, ItemStatType.Spell_Damage) + GetStat(gearStats, ItemStatType.Spell_Damage_and_Healing);
        //    int spellHeal = GetStat(gearStats, ItemStatType.Spell_Healing) + GetStat(gearStats, ItemStatType.Spell_Damage_and_Healing);
        //    int mp5 = GetStat(gearStats, ItemStatType.Mp5);
        //    if (stamina > agility + intellect && stamina > strength + spirit)
        //    {
        //        if(gearStats.ContainsKey(ItemStatType.Defense) || gearStats.ContainsKey(ItemStatType.Block_Chance) || gearStats.ContainsKey(ItemStatType.Block_Value))
        //        {
        //            //Tank
        //            return GearType.Tank;
        //        }
        //        else
        //        {
        //            //PVP
        //            return GearType.PVP;
        //        }
        //    }
        //    else if (spellHeal >= spellDamage && (mp5 > 0 || intellect + spirit > stamina))
        //    {
        //        //Healer
        //        return GearType.Heal;
        //    }
        //    else
        //    {
        //        //DPS
        //        return GearType.DPS;
        //    }
        //}
    }
}