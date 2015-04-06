using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VF
{
    public class BuffParser
    {
        static Dictionary<char, string> BuffIconParser = new Dictionary<char, string>{
            {'a', "WTF_"},
            {'b', "Ability_"},
            {'c', "Racial_"},
            {'d', "Creature_"},
            {'e', "Warrior_"},
            {'f', "Paladin_"},
            {'g', "Hunter_"},
            {'h', "Druid_"},
            {'i', "Rogue_"},
            {'j', "Shaman_"},
            {'k', "Mage_"},
            {'l', "Mount_"},
            {'m', "Pet_"},
            {'n', "INV_"},
            {'o', "Potion_"},
            {'p', "Spell_"},
            {'q', "Fire_"},
            {'r', "Frost_"},
            {'s', "Holy_"},
            {'t', "Nature_"},
            {'u', "Shadow_"},
            {'v', "Sword_"},
            {'w', "Misc_"},
            {'x', "Food_"},
            {'y', "Jewelry_"},
            {'z', "Drink_"},
        };

        public static string GetBuffIconImage(string _Buff)
        {
            string realBuffName = "";
            bool fresh = true;
            for (int i = 0; i < _Buff.Length; ++i)
            {
                if (fresh == true)
                {
                    string value;
                    if (BuffIconParser.TryGetValue(_Buff[i], out value) == true)
                    {
                        realBuffName += value;
                    }
                    else
                    {
                        realBuffName += _Buff[i];
                        fresh = false;
                    }
                }
                else
                {
                    realBuffName += _Buff[i];
                    if (_Buff[i] == '_')
                        fresh = true;
                    else
                        fresh = false;
                }
            }
            return "<img src='http://realmplayers.com/Assets/wowicons/43x43/" + realBuffName + ".png'></img>";
        }

    }
}