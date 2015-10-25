using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlayerData = VF_RealmPlayersDatabase.PlayerData;
using UploadID = VF_RealmPlayersDatabase.UploadID;

using CharacterData = VF_RealmPlayersDatabase.PlayerData.CharacterData;
using GuildData = VF_RealmPlayersDatabase.PlayerData.GuildData;
using HonorData = VF_RealmPlayersDatabase.PlayerData.HonorData;
using GearData = VF_RealmPlayersDatabase.PlayerData.GearData;

using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;
using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerSex = VF_RealmPlayersDatabase.PlayerSex;

namespace ManualOldHistoryDataGenerator
{


    class Program
    {
        static Dictionary<string, PlayerData.PlayerHistory> sm_OldHistory = new Dictionary<string, PlayerData.PlayerHistory>();

        static void Main(string[] args)
        {
            //Thunderfury
            AddGearData("Sidesprang", GenerateGearDataStr(19019, ItemSlot.Main_Hand), new DateTime(2013, 5, 18, 10, 10, 10));
            AddGearData("Sixt", GenerateGearDataStr(19019, ItemSlot.Main_Hand), new DateTime(2013, 5, 19, 10, 10, 10));
            AddGearData("Arawn", GenerateGearDataStr(19019, ItemSlot.Main_Hand), new DateTime(2013, 6, 19, 10, 10, 10));
            AddGearData("Jamcastle", GenerateGearDataStr(19019, ItemSlot.Main_Hand), new DateTime(2013, 7, 18, 10, 10, 10));

            //Hand of Ragnaros
            AddGearData("Vitamina", GenerateGearDataStr(17182, ItemSlot.Main_Hand), new DateTime(2013, 1, 3, 10, 10, 10));
            AddGearData("Buiaka", GenerateGearDataStr(17182, ItemSlot.Main_Hand), new DateTime(2013, 1, 28, 10, 10, 10));
            AddGearData("Dobermickey", GenerateGearDataStr(17182, ItemSlot.Main_Hand), new DateTime(2013, 1, 28, 10, 10, 11));
            AddGearData("Vast", GenerateGearDataStr(17182, ItemSlot.Main_Hand), new DateTime(2013, 3, 4, 10, 10, 10));
            AddGearData("Bearnbeer", GenerateGearDataStr(17182, ItemSlot.Main_Hand), new DateTime(2013, 4, 23, 10, 10, 10));
            AddGearData("Thrashing", GenerateGearDataStr(17182, ItemSlot.Main_Hand), new DateTime(2013, 5, 11, 10, 10, 10));
            AddGearData("Xoxa", GenerateGearDataStr(17182, ItemSlot.Main_Hand), new DateTime(2013, 5, 13, 10, 10, 10));

            //Mood Ring(lol)
            AddGearData("Thrashing", GenerateGearDataStr(7338, ItemSlot.Finger_1), new DateTime(2012, 8, 25, 10, 10, 10));
            //Treants Bane
            AddGearData("Dilatazu", GenerateGearDataStr(18538, ItemSlot.Main_Hand), new DateTime(2013, 2, 15, 10, 10, 10));
            //Neltharion's Tear
            AddGearData("Kaii", GenerateGearDataStr(19379, ItemSlot.Trinket_1), new DateTime(2013, 5, 17, 10, 10, 10));
            //Perdition's Blade
            AddGearData("Gurrwald", GenerateGearDataStr(18816, ItemSlot.Main_Hand), new DateTime(2012, 10, 30, 10, 10, 10));
            //Rune of Metamorphosis
            AddGearData("Vanillacoke", GenerateGearDataStr(19340, ItemSlot.Trinket_1), new DateTime(2012, 10, 30, 10, 10, 10));
            //Bonereaver's Edge(17076): 2012-11-29 - Thrashing
            AddGearData("Thrashing", GenerateGearDataStr(17076, ItemSlot.Main_Hand), new DateTime(2012, 11, 29, 10, 10, 10));
            //Warsong Battle Tabard(19505): 2013-01-15 - Thrashing
            AddGearData("Thrashing", GenerateGearDataStr(19505, ItemSlot.Tabard), new DateTime(2013, 1, 15, 10, 10, 10));
            //Outrider's Plate Legguards(22651) 2013-01-15 - Thrashing
            AddGearData("Thrashing", GenerateGearDataStr(22651, ItemSlot.Legs), new DateTime(2013, 1, 15, 10, 10, 10));
            //Rhok'Delar(18713): 2013-03-17 - Leegoolas < (????-??-?? - Imfenion)
            AddGearData("Leegoolas", GenerateGearDataStr(18713, ItemSlot.Ranged), new DateTime(2013, 3, 17, 10, 10, 10));
            //Benediction(18608): 2012-10-31 - Townraider
            AddGearData("Townraider", GenerateGearDataStr(18608, ItemSlot.Main_Hand), new DateTime(2012, 10, 31, 10, 10, 10));//Benediction
            AddGearData("Townraider", GenerateGearDataStr(18609, ItemSlot.Off_Hand), new DateTime(2012, 10, 31, 10, 10, 10));//Anathema
            //Lok'Amir(19360): 2013-05-17 - Madmena
            AddGearData("Madmena", GenerateGearDataStr(19360, ItemSlot.Main_Hand), new DateTime(2013, 5, 17, 10, 10, 10));


            //R14
            AddHonorData("Xeogta", GenerateHonorDataStr_MaxRank(14), new DateTime(2013, 3, 24, 10, 10, 10));
            AddHonorData("Zairah", GenerateHonorDataStr_MaxRank(14), new DateTime(2013, 4, 7, 10, 10, 10));
            AddHonorData("Nuc", GenerateHonorDataStr_MaxRank(14), new DateTime(2013, 5, 5, 10, 10, 10));
            AddHonorData("Showtime", GenerateHonorDataStr_MaxRank(14), new DateTime(2013, 5, 12, 10, 10, 10));
            AddHonorData("Mal", GenerateHonorDataStr_MaxRank(14), new DateTime(2013, 6, 2, 10, 10, 10));
            AddHonorData("Hummer", GenerateHonorDataStr_MaxRank(14), new DateTime(2013, 6, 9, 10, 10, 10));
            //R14

            //Lvl60
            AddCharacterData("Zarcharias", GenerateCharacterData_LVL60(PlayerRace.Undead, PlayerClass.Mage, PlayerSex.Male), new DateTime(2012, 9, 2, 10, 10, 10));
            AddCharacterData("Dunlope", GenerateCharacterData_LVL60(PlayerRace.Human, PlayerClass.Mage, PlayerSex.Female), new DateTime(2012, 9, 2, 15, 10, 10));
            AddCharacterData("Kaii", GenerateCharacterData_LVL60(PlayerRace.Gnome, PlayerClass.Mage, PlayerSex.Female), new DateTime(2012, 9, 4, 10, 10, 10));
            AddCharacterData("Maitoz", GenerateCharacterData_LVL60(PlayerRace.Human, PlayerClass.Rogue, PlayerSex.Female), new DateTime(2012, 9, 4, 15, 10, 10));
            AddCharacterData("Xeogta", GenerateCharacterData_LVL60(PlayerRace.Undead, PlayerClass.Warrior, PlayerSex.Female), new DateTime(2012, 9, 14, 10, 10, 10));
            AddCharacterData("Showtime", GenerateCharacterData_LVL60(PlayerRace.Orc, PlayerClass.Shaman, PlayerSex.Female), new DateTime(2012, 9, 18, 10, 10, 10));
            AddCharacterData("Mindmelt", GenerateCharacterData_LVL60(PlayerRace.Undead, PlayerClass.Warlock, PlayerSex.Female), new DateTime(2012, 9, 18, 10, 10, 11));
            AddCharacterData("Xaizer", GenerateCharacterData_LVL60(PlayerRace.Orc, PlayerClass.Warrior, PlayerSex.Male), new DateTime(2012, 9, 18, 10, 10, 12));
            AddCharacterData("Thrashing", GenerateCharacterData_LVL60(PlayerRace.Tauren, PlayerClass.Warrior, PlayerSex.Male), new DateTime(2012, 9, 19, 10, 10, 10));
            AddCharacterData("Twig", GenerateCharacterData_LVL60(PlayerRace.Night_Elf, PlayerClass.Hunter, PlayerSex.Female), new DateTime(2012, 9, 19, 10, 10, 11));
            AddCharacterData("Kaeter", GenerateCharacterData_LVL60(PlayerRace.Night_Elf, PlayerClass.Druid, PlayerSex.Male), new DateTime(2012, 9, 20, 10, 10, 10));
            AddCharacterData("Dende", GenerateCharacterData_LVL60(PlayerRace.Gnome, PlayerClass.Warlock, PlayerSex.Female), new DateTime(2012, 9, 22, 10, 10, 10));
            //Lvl60

            //Rivendare mount 2012-12-04 - Caitlin, 2012-??-?? - Buiaka 

            VF_RealmPlayersDatabase.Utility.SaveSerialize("PlayersHistoryData_ManuallyAdded.dat", sm_OldHistory);
            //"PlayersHistoryData_ManuallyAdded.dat"
        }

        static CharacterData GenerateCharacterData_LVL60(PlayerRace _Race, PlayerClass _Class, PlayerSex _Sex)
        {
            CharacterData characterData = new CharacterData("");
            characterData.Race = _Race;
            characterData.Class = _Class;
            characterData.Sex = _Sex;
            characterData.Level = 60;
            return characterData;
        }
        static string GenerateGearDataStr(int _ItemID, ItemSlot _Slot)
        {
            return ((int)_Slot).ToString() + ":" + _ItemID.ToString() + ":0:0:0";
        }
        static string GenerateHonorDataStr_MaxRank(int _MaxRank)
        {
            return _MaxRank.ToString() + ":0.0:0:0:0:0:0:0:0:" + _MaxRank.ToString() + ":0:0:0:0";
        }


        static void AddGearData(string _Player, string _GearDataStr, DateTime _DateTime)
        {
            if (sm_OldHistory.ContainsKey(_Player) == false)
                sm_OldHistory.Add(_Player, new PlayerData.PlayerHistory());
            sm_OldHistory[_Player].AddToHistory(new GearData(_GearDataStr, VF_RealmPlayersDatabase.WowVersionEnum.Vanilla), new UploadID(0, _DateTime));
        }
        static void AddHonorData(string _Player, string _HonorDataStr, DateTime _DateTime)
        {
            if (sm_OldHistory.ContainsKey(_Player) == false)
                sm_OldHistory.Add(_Player, new PlayerData.PlayerHistory());
            sm_OldHistory[_Player].AddToHistory(new HonorData(_HonorDataStr, VF_RealmPlayersDatabase.WowVersionEnum.Vanilla), new UploadID(0, _DateTime));
        }
        static void AddCharacterData(string _Player, CharacterData _CharacterData, DateTime _DateTime)
        {
            if (sm_OldHistory.ContainsKey(_Player) == false)
                sm_OldHistory.Add(_Player, new PlayerData.PlayerHistory());
            sm_OldHistory[_Player].AddToHistory(_CharacterData, new UploadID(0, _DateTime));
        }
    }
}
