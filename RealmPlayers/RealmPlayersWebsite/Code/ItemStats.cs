using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealmPlayersServer.Code
{
    public enum ItemStatType
    {
        Unknown,
        Health,
        Mana,
        Stamina,
        Agility,
        Strength,
        Spirit,
        Intellect,
        Defense,
        Armor,
        Attack_Power,
        Item_Level,
        Crit_Chance,
        Hit_Chance,
        Block_Chance,
        Dodge_Chance,
        Parry_Chance,
        Block_Value,
        Shadow_Resistance,
        Fire_Resistance,
        Frost_Resistance,
        Nature_Resistance,
        Arcane_Resistance,
        All_Resistances,
        Spell_Crit_Chance,
        Spell_Hit_Chance,
        Spell_Damage,
        Spell_Damage_and_Healing,
        Spell_Healing,
        Mp5,
        Frost_Spell_Damage,
        Fire_Spell_Damage,
        Shadow_Spell_Damage,
        Nature_Spell_Damage,
        Arcane_Spell_Damage,
        Attack_Speed,
        Ranged_Attack_Power,
        All_Stats,
        Spell_Penetration,

        //TBC
        Resilience_Rating,
        Critical_Strike_Rating,
        Hit_Rating,
        Haste_Rating,
        Armor_Penetration,
        Spell_Critical_Strike_Rating,
        Spell_Hit_Rating,
        Spell_Haste_Rating,
        Defense_Rating,
        Dodge_Rating,
        Parry_Rating,
        Expertise_Rating,
        Shield_Block_Rating,
        Increased_Critical_Damage,
        HMp5,
        Hp5,
        //TBC
    }
    public struct ItemStat
    {
        public ItemStatType Type;
        public int Value;
        public bool Enchant;
        public bool Gem;
        public ItemStat(ItemStatType _Type, int _Value, bool _Enchant = false, bool _Gem = false)
        {
            Type = _Type;
            Value = _Value;
            Enchant = _Enchant;
            Gem = _Gem;
        }
    }
    public class ItemStats
    {
        //public int GenerateStat(int _ItemID, int _SuffixID, int _EnchantID)
        //{
        //    if (_ItemID == int.Parse(ItemID))
        //    {
        //        var specifiedAjaxTooltip = GetAjaxTooltip(_ItemID, _SuffixID, _EnchantID, "");
        //    }
        //    return 0;
        //}

        private static ItemStat[] _Parse_Plus_Attribute(string _Data)
        {
            string[] datas = null;
            ItemStat[] result = null;
            if(_Data.Contains(" and "))
            {
                int afterAndIndex = _Data.IndexOf(" and ") + " and ".Length;
                if((_Data[afterAndIndex] >= 'A' && _Data[afterAndIndex] <= 'Z')
                || (_Data[afterAndIndex] >= 'a' && _Data[afterAndIndex] <= 'z'))
                {
                    //Special case for "+5 Health and Mana every 5 sec"
                    datas = new string[] { _Data };
                }
                else
                { 
                    datas = _Data.SplitVF(" and ");
                    for(int i = 0; i < datas.Length; ++i)
                    {
                        if(datas[i].Count((c) => c == '+') >= 2)
                        { 
                            //Special case for "+11 Healing +4 Spell Damage and 2 mana per 5 sec"
                            string[] extraData = datas[i].SplitVF(" +");
                            string[] newDatas = new string[datas.Length + extraData.Length - 1];
                            for(int u = 0; u < i; ++u)
                            {
                                newDatas[u] = datas[u];
                            }
                            for(int u = 0; u < extraData.Length; ++u)
                            {
                                newDatas[i + u] = extraData[u];
                            }
                            for(int u = i + 1; u < datas.Length; ++u)
                            {
                                newDatas[extraData.Length + u - 1] = datas[u];
                            }
                            datas = newDatas;
                        }
                    }
                }
            }
            else if (_Data.Contains(" & "))
            {
                datas = _Data.SplitVF(" & ");
            }
            else if (_Data.Contains(", "))
            {
                datas = _Data.SplitVF(", ");
            }
            else if(_Data.Contains(" +") == true)
            {
                datas = _Data.SplitVF(" +", StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                datas = new string[]{_Data};
            }
            result = new ItemStat[datas.Length];
            for(int i = 0; i < datas.Length; ++i)
            {
                string[] value_And_StatType = datas[i].Split(new char[] { ' ' }, 2);
                if (value_And_StatType[0].StartsWith("+"))
                    value_And_StatType[0] = value_And_StatType[0].Substring(1);
                string valueStr = value_And_StatType[0].Replace("%", "");
                string statTypeStr = value_And_StatType[1].Replace(' ', '_').Replace(".","");
                ItemStatType valueType = ItemStatType.Unknown;
                statTypeStr = statTypeStr.ToLower();
                if (statTypeStr == "hit")
                    valueType = ItemStatType.Hit_Chance;
                else if (statTypeStr == "mana_every_5_seconds" || statTypeStr.ToLower() == "mana_per_5_sec" || statTypeStr == "mana_regen" || statTypeStr == "mana_every_5_sec")
                    valueType = ItemStatType.Mp5;
                else if (statTypeStr == "healing" || statTypeStr == "healing_spells")
                    valueType = ItemStatType.Spell_Healing;
                else if (statTypeStr == "health_and_mana_every_5_sec")
                    valueType = ItemStatType.HMp5;
                else
                {
                    try
                    {
                        valueType = (ItemStatType)Enum.Parse(typeof(ItemStatType), statTypeStr, true);
                    }
                    catch (Exception)
                    {
                        //to be added something in the future?
                    }
                }
                if(valueType != ItemStatType.Unknown)
                    result[i] = new ItemStat(valueType, int.Parse(valueStr));
            }
            return result;
        }
        private static void _AddStats(string _StatType, string _Value, bool _IsEnchant, List<ItemStat> _ResultStats)
        {
            _StatType = _StatType.Replace(' ', '_');
            _Value = _Value.Replace("%", "");
            ItemStatType valueType = ItemStatType.Unknown;
            try
            {
                valueType = (ItemStatType)Enum.Parse(typeof(ItemStatType), _StatType, true);
            }
            catch (Exception)
            {
                if (_StatType == "Healing_Spells")
                    valueType = ItemStatType.Spell_Healing;
                else if (_StatType == "HP")
                    valueType = ItemStatType.Health;
                else if (_StatType == "Healing_and_Spell_Damage" || _StatType == "Spell_Power")
                    valueType = ItemStatType.Spell_Damage_and_Healing;
                else if (_StatType == "Spell_Hit")
                    valueType = ItemStatType.Spell_Hit_Chance;
                else if (_StatType == "Hit")
                    valueType = ItemStatType.Hit_Chance;
                else if (_StatType == "Critical_Strike")
                    valueType = ItemStatType.Crit_Chance;
                else if (_StatType == "Spell_Critical_Strike")
                    valueType = ItemStatType.Spell_Crit_Chance;
                else if (_StatType.EndsWith("Damage"))
                {
                    try
                    {
                        _StatType = _StatType.Replace("Damage", "Spell_Damage");
                        valueType = (ItemStatType)Enum.Parse(typeof(ItemStatType), _StatType, true);
                    }
                    catch (Exception)
                    {
                        valueType = ItemStatType.Unknown;
                    }
                }
                else if (_StatType == "All_Stats")
                {
                    int allStatVal = int.Parse(_Value);
                    _ResultStats.Add(new ItemStat(ItemStatType.Stamina, allStatVal, _IsEnchant));
                    _ResultStats.Add(new ItemStat(ItemStatType.Strength, allStatVal, _IsEnchant));
                    _ResultStats.Add(new ItemStat(ItemStatType.Agility, allStatVal, _IsEnchant));
                    _ResultStats.Add(new ItemStat(ItemStatType.Spirit, allStatVal, _IsEnchant));
                    _ResultStats.Add(new ItemStat(ItemStatType.Intellect, allStatVal, _IsEnchant));
                    valueType = ItemStatType.Unknown;
                }
            }
            if (valueType == ItemStatType.All_Stats)
            {
                int allStatVal = int.Parse(_Value);
                _ResultStats.Add(new ItemStat(ItemStatType.Stamina, allStatVal, _IsEnchant));
                _ResultStats.Add(new ItemStat(ItemStatType.Strength, allStatVal, _IsEnchant));
                _ResultStats.Add(new ItemStat(ItemStatType.Agility, allStatVal, _IsEnchant));
                _ResultStats.Add(new ItemStat(ItemStatType.Spirit, allStatVal, _IsEnchant));
                _ResultStats.Add(new ItemStat(ItemStatType.Intellect, allStatVal, _IsEnchant));
            }
            else if (valueType != ItemStatType.Unknown)
                _ResultStats.Add(new ItemStat(valueType, int.Parse(_Value), _IsEnchant));
        }
        public static List<ItemStat> ParseAjaxTooltip(string _AjaxToolTip)
        {
            List<ItemStat> stats = new List<ItemStat>();

            var splitData = _AjaxToolTip.Split(new string[] { 
                "<font color=\\\"gold\\\">", 
                "<font color=\"gold\">", 
                "<\\/font>", 
                "</font>", 
                "<table>", 
                "<tr>", 
                "<td>", 
                "<br \\/>", 
                "<br />", 
                "<\\/table>", }, StringSplitOptions.RemoveEmptyEntries);
            //int itemLevelStartIndex = _AjaxToolTip.IndexOf("Item Level ");
            //itemLevelStartIndex += "Item Level ".Length;
            //int itemLevelStopIndex = _AjaxToolTip.IndexOf('<', itemLevelStartIndex);
            //string itemLevelStr = _AjaxToolTip.Substring(itemLevelStartIndex, itemLevelStopIndex - itemLevelStartIndex);
            //stats.Add(new ItemStat(ItemStatType.Item_Level, int.Parse(itemLevelStr)));

            //"<table><tr><td><b class="q4">Onslaught Legguards</b><br />
            //<font color="gold">Item Level 146</font><br />
            //Binds when picked up<table width="100%"><tr>
            //<td>Legs</td><th>Plate</th></tr></table>
            //1597 Armor<br />+24 Agility<br />
            //+78 Stamina<br /><span class="socket-meta" style="background: url(http://database.feenixserver.com/images/icons/small/inv_jewelcrafting_nightseye_03.jpg) no-repeat left center">
            //Dodge Rating +5 and Stamina +6</span><br />
            //Socket Bonus:<span class="q2"> +3 Stamina</span><br />
            //<span class="q2">+35 Stamina and +12 Agility</Span><br />
            //Durability 120 / 120<br />
            //Classes: <font color="#CD853F">Warrior</font><br />
            //Requires Level 70<br /></td></tr></table>
            //<table><tr><td><span class="q2">Equip: Defense rating + 40&nbsp;<small>(<a href="javascript:;" onmousedown="return false" onclick="g_setRatingLevel(this,70,12,40)">16.91&nbsp;@&nbsp;L70</a>)</small>.</span><br />
            //<span class="q2">Equip: Parry rating + 41&nbsp;<small>(<a href="javascript:;" onmousedown="return false" onclick="g_setRatingLevel(this,70,14,41)">1.73%&nbsp;@&nbsp;L70</a>)</small>.</span><br />
            //<span class="q2">Equip: <a href="?spell=39987" class="q2">Increases the block value of your shield by 42.</a></span><br />
            //<br /><span class="q"><a href="?itemset=673" class="q">Onslaught Armor</a> (2/8)</span>
            //<div class="q0 indent"><span><a href="?item=30976">Onslaught Chestguard</a></span><br />
            //<span><a href="?item=30974">Onslaught Greathelm</a></span><br /><span><a href="?item=30970">Onslaught Handguards</a></span><br />
            //<span><span class="q8"><a href="?item=30978">Onslaught Legguards</a></Span></span><br />
            //<span><span class="q8"><a href="?item=30980">Onslaught Shoulderguards</a></Span></span><br />
            //<span><a href="?item=34568">Onslaught Boots</a></span><br /><span><a href="?item=34442">Onslaught Wristguards</a></span><br />
            //<span><a href="?item=34547">Onslaught Waistguard</a></span><br /></div><br />
            //<span class="q0"><span class="q0"><span class="q2">(2) Set: <a href="?spell=38408">Increases the health bonus from your Commanding Shout ability by 170.</a></span><br />
            //<span>(4) Set: <a href="?spell=38407">Increases the damage of your Shield Slam ability by 10%.</a></span><br />
            //</span></span></td></tr></table>"

            foreach (string data in splitData)
            {
                try
                {
                    if (data.StartsWith("Item Level "))
                    {
                        string itemLevelStr = data.Substring("Item Level ".Length);
                        stats.Add(new ItemStat(ItemStatType.Item_Level, int.Parse(itemLevelStr)));
                    }
                    else if (data.StartsWith("+"))
                    {
                        var parseResults = _Parse_Plus_Attribute(data);
                        foreach (var parseResult in parseResults)
                        {
                            stats.Add(parseResult);
                        }
                    }
                    else if (data.StartsWith("<span class=\\\"q2\\\">")) //Equip stat or enchant
                    {
                        string dataNext = data.Substring("<span class=\\\"q2\\\">".Length);
                        if (dataNext.StartsWith("Equip: ")) //Special Item ItemStat
                        {
                            string[] specialItemStatSplit = dataNext.Split(new char[] { '>', '<', '.', '%' });
                            foreach (string specialItemStatData in specialItemStatSplit)
                            {
                                if (specialItemStatData.StartsWith("Increased Defense +"))
                                {
                                    string defenseStr = specialItemStatData.Substring("Increased Defense +".Length);
                                    stats.Add(new ItemStat(ItemStatType.Defense, int.Parse(defenseStr)));
                                }
                                else if (specialItemStatData.StartsWith("Increases your chance to block attacks with a shield by "))
                                {
                                    string blockChanceStr = specialItemStatData.Substring("Increases your chance to block attacks with a shield by ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Block_Chance, int.Parse(blockChanceStr)));
                                }
                                else if (specialItemStatData.StartsWith("Increases the block value of your shield by "))
                                {
                                    string blockValueStr = specialItemStatData.Substring("Increases the block value of your shield by ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Block_Value, int.Parse(blockValueStr)));
                                }
                                else if (specialItemStatData.StartsWith("Increases your chance to dodge an attack by "))
                                {
                                    string dodgeChanceStr = specialItemStatData.Substring("Increases your chance to dodge an attack by ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Dodge_Chance, int.Parse(dodgeChanceStr)));
                                }
                                else if (specialItemStatData.StartsWith("Increases your chance to parry an attack by "))
                                {
                                    string parryChanceStr = specialItemStatData.Substring("Increases your chance to parry an attack by ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Parry_Chance, int.Parse(parryChanceStr)));
                                }
                                else if (specialItemStatData.StartsWith("Improves your chance to hit by "))
                                {
                                    string hitChanceStr = specialItemStatData.Substring("Improves your chance to hit by ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Hit_Chance, int.Parse(hitChanceStr)));
                                }
                                else if (specialItemStatData.StartsWith("Improves your chance to get a critical strike by "))
                                {
                                    string critChanceStr = specialItemStatData.Substring("Improves your chance to get a critical strike by ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Crit_Chance, int.Parse(critChanceStr)));
                                }
                                else if (specialItemStatData.StartsWith("Improves your chance to get a critical strike with spells by "))
                                {
                                    string spellCritChanceStr = specialItemStatData.Substring("Improves your chance to get a critical strike with spells by ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Spell_Crit_Chance, int.Parse(spellCritChanceStr)));
                                }
                                else if (specialItemStatData.StartsWith("Improves your chance to hit with spells by "))
                                {
                                    string spellHitChanceStr = specialItemStatData.Substring("Improves your chance to hit with spells by ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Spell_Hit_Chance, int.Parse(spellHitChanceStr)));
                                }
                                else if (specialItemStatData.StartsWith("Increases damage and healing done by magical spells and effects by up to "))
                                {
                                    string spellDamageStr = specialItemStatData.Substring("Increases damage and healing done by magical spells and effects by up to ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Spell_Damage_and_Healing, int.Parse(spellDamageStr)));
                                }
                                else if (specialItemStatData.StartsWith("Increases healing done by spells and effects by up to "))
                                {
                                    string spellHealStr = specialItemStatData.Substring("Increases healing done by spells and effects by up to ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Spell_Healing, int.Parse(spellHealStr)));
                                }
                                else if(specialItemStatData.StartsWith("Increases healing done by up to "))
                                {
                                    string spellHealStr = specialItemStatData.Substring("Increases healing done by up to ".Length);
                                    var spellData = spellHealStr.SplitVF(" and ");
                                    stats.Add(new ItemStat(ItemStatType.Spell_Healing, int.Parse(spellData.First())));
                                    if (spellData.Length > 1) //damage done by up to * for all magical spells and effects
                                    {
                                        string spellDamageStr = spellData[1].Substring("damage done by up to ".Length);
                                        spellDamageStr = spellDamageStr.Split(' ').First();
                                        stats.Add(new ItemStat(ItemStatType.Spell_Damage, int.Parse(spellDamageStr)));
                                    }
                                }
                                else if (specialItemStatData.StartsWith("Increases damage done by "))
                                {
                                    string[] splitsData = specialItemStatData.Substring("Increases damage done by ".Length).Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                                    ItemStatType valueType = ItemStatType.Unknown;
                                    try
                                    {
                                        valueType = (ItemStatType)Enum.Parse(typeof(ItemStatType), splitsData.First() + "_spell_damage", true);
                                        int value = int.Parse(splitsData.Last());
                                        stats.Add(new ItemStat(valueType, value));
                                    }
                                    catch (Exception)
                                    {
                                        //to be added something in the future?
                                    }
                                }
                                else if (specialItemStatData.StartsWith("Restores ") && specialItemStatData.EndsWith(" mana per 5 sec"))
                                {
                                    string mp5Str = specialItemStatData.Split(' ')[1];
                                    stats.Add(new ItemStat(ItemStatType.Mp5, int.Parse(mp5Str)));
                                }
                                else if (specialItemStatData.StartsWith("Increases attack power by "))
                                {
                                    string attackPowerStr = specialItemStatData.Substring("Increases attack power by ".Length);
                                    stats.Add(new ItemStat(ItemStatType.Attack_Power, int.Parse(attackPowerStr)));
                                }
                                else if (specialItemStatData.StartsWith("+"))
                                {
                                    var parseResults = _Parse_Plus_Attribute(specialItemStatData);
                                    foreach (var parseResult in parseResults)
                                    {
                                        stats.Add(parseResult);
                                    }
                                }
                                //ADDED FOR TBC
                                else if(specialItemStatData.StartsWith("Equip: ") && specialItemStatData.Contains("+"))
                                {
                                    string statStr = specialItemStatData.Substring("Equip: ".Length);
                                    var statParts = statStr.SplitVF(" + ");
                                    if (statParts.Length >= 2)
                                    {
                                        _AddStats(statParts[0], statParts[1].Split('&').First(), false, stats);
                                    }
                                }
                                else if(specialItemStatData.StartsWith("Your attacks ignore "))
                                {
                                    var statParts = specialItemStatData.Split(' ');
                                    if(statParts.Length == 8 && statParts[4] == "of" && statParts[5] == "your" && statParts[6] == "opponent\\'s" && statParts[7] == "armor")
                                    {
                                        stats.Add(new ItemStat(ItemStatType.Armor_Penetration, int.Parse(statParts[3])));
                                    }
                                }
                            }
                        }
                        else if (dataNext.StartsWith("Mana Regen ") && dataNext.Substring(0, dataNext.IndexOf('<')).EndsWith(" per 5 sec."))
                        {
                            string mp5Str = dataNext.Split(' ')[2];
                            stats.Add(new ItemStat(ItemStatType.Mp5, int.Parse(mp5Str), true));
                        }
                        else if (dataNext.StartsWith("("))//Set Bonus
                        { }
                        else if (dataNext.Contains('+'))//Most likely Enchant Type
                        {
                            if (dataNext.StartsWith("<!---->+")) //Suffix
                            {
                                int endOfSuffixIndex = dataNext.IndexOf('<', 5);
                                if(endOfSuffixIndex != -1)
                                    dataNext = dataNext.Substring(0, dataNext.IndexOf('<', 5));
                                var parseResults = _Parse_Plus_Attribute(dataNext.Substring("<!---->".Length));
                                foreach (var parseResult in parseResults)
                                {
                                    var newStat = parseResult;
                                    newStat.Enchant = false;
                                    stats.Add(newStat);
                                }
                            }
                            else
                            {
                                dataNext = dataNext.Substring(0, dataNext.IndexOf('<'));
                                if (dataNext.StartsWith("+")) //Enchant
                                {
                                    var parseResults = _Parse_Plus_Attribute(dataNext);
                                    foreach (var parseResult in parseResults)
                                    {
                                        var newStat = parseResult;
                                        newStat.Enchant = true;
                                        if (newStat.Type == ItemStatType.All_Stats)
                                        {
                                            stats.Add(new ItemStat(ItemStatType.Stamina, newStat.Value, newStat.Enchant));
                                            stats.Add(new ItemStat(ItemStatType.Strength, newStat.Value, newStat.Enchant));
                                            stats.Add(new ItemStat(ItemStatType.Agility, newStat.Value, newStat.Enchant));
                                            stats.Add(new ItemStat(ItemStatType.Spirit, newStat.Value, newStat.Enchant));
                                            stats.Add(new ItemStat(ItemStatType.Intellect, newStat.Value, newStat.Enchant));
                                        }
                                        else
                                        {
                                            stats.Add(newStat);
                                        }
                                    }
                                }
                                else //Enchant
                                {
                                    string[] enchantDatas = dataNext.Split(new string[] { "\\/", "/" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (enchantDatas.Length == 1)
                                    {
                                        //Special case for:
                                        //Spell Damage +15 and +1% Spell Critical Strike -> Spell Damage +15/Spell Crit +1%
                                        //Healing +31 and 5 mana per 5 sec. -> Healing +31/Mp5 +5
                                        //Attack Power +26 and +1% Critical Strike -> Attack Power +26/Critical Strike +1%
                                        //Stamina +16 and Armor +100 -> Stamina +16/Armor +100
                                        if (dataNext.Contains("Spell Damage +15 and +1% Spell Critical Strike"))
                                            enchantDatas = new string[] { "Spell Damage +15", "Spell Critical Strike +1%" };
                                        else if (dataNext.Contains("Healing +31 and 5 mana per 5 sec"))
                                            enchantDatas = new string[] { "Spell Healing +31", "Mp5 +5" };
                                        else if (dataNext.Contains("Attack Power +26 and +1% Critical Strike"))
                                            enchantDatas = new string[] { "Attack Power +26", "Critical Strike +1%" };
                                        else if (dataNext.Contains("Stamina +16 and Armor +100"))
                                            enchantDatas = new string[] { "Stamina +16", "Armor +100" };
                                    }
                                    foreach (var enchData in enchantDatas)
                                    {
                                        string[] statType_And_Value = enchData.Split(new string[] { " +" }, StringSplitOptions.RemoveEmptyEntries);
                                        _AddStats(statType_And_Value[0], statType_And_Value[1], true, stats);
                                    }
                                }
                            }
                        }
                        else //Specific Enchant (Crusader etc)
                        { }
                    }
                    else if(data.StartsWith("<!---->+")) //Special suffix path
                    {
                        string dataNext = data.Substring(0, data.IndexOf('<', 5));
                        var parseResults = _Parse_Plus_Attribute(dataNext.Substring("<!---->".Length));
                        foreach (var parseResult in parseResults)
                        {
                            var newStat = parseResult;
                            newStat.Enchant = false;
                            stats.Add(newStat);
                        }
                    }
                    else if(data.StartsWith("<span class=\\\"socket-")) //Gem!
                    {
                        string dataNext = data.Substring(data.IndexOf('>') + 1);
                        dataNext = dataNext.Substring(0, dataNext.IndexOf('<'));
                        var parseResults = _Parse_Plus_Attribute(dataNext);
                        foreach (var parseResult in parseResults)
                        {
                            var newStat = parseResult;
                            newStat.Gem = true;
                            stats.Add(newStat);
                        }
                    }
                    else if (data.StartsWith("Socket Bonus:<span class=\\\"q2\\\"> ")) //Socket bonus!
                    {
                        string dataNext = data.Substring("Socket Bonus:<span class=\\\"q2\\\"> ".Length);
                        dataNext = dataNext.Substring(0, dataNext.IndexOf('<'));
                        var parseResults = _Parse_Plus_Attribute(dataNext);
                        foreach (var parseResult in parseResults)
                        {
                            var newStat = parseResult;
                            newStat.Gem = true;
                            stats.Add(newStat);
                        }
                    }
                    else if (data.EndsWith("Armor"))
                    {
                        string armorStr = data.Split(' ')[0];
                        stats.Add(new ItemStat(ItemStatType.Armor, int.Parse(armorStr)));
                    }
                    else if (data.EndsWith("Block"))
                    {
                        string blockStr = data.Split(' ')[0];
                        stats.Add(new ItemStat(ItemStatType.Block_Value, int.Parse(blockStr)));
                    }
                }
                catch (Exception)
                {}
            }
            return stats;
        }
    }
}