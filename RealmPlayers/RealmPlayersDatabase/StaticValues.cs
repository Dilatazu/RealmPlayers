using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase
{
    public enum WowRealm
    {
        Unknown = 0,
        Emerald_Dream = 1,
        Al_Akir = 2,
        Warsong = 3,
        All = 4,
        Archangel = 5,
        VanillaGaming = 6,
        Valkyrie = 7,
        Rebirth = 8,
        Test_Server = 9,
        Nostalrius = 10,
        Kronos = 11,
    }
    public enum WowVersionEnum
    {
        Unknown,
        Vanilla,
        TBC,
        WOTLK,
    }
    public enum PlayerFaction
    {
        Unknown,
        Horde,
        Alliance
    }
    public enum PlayerRace
    {
        Unknown,
        Orc,
        Undead,
        Tauren,
        Troll,
        Human,
        Dwarf,
        Gnome,
        Night_Elf,
        Blood_Elf,
        Draenei,
    }
    public enum PlayerClass
    {
        Unknown,
        Druid,
        Warrior,
        Shaman,
        Priest,
        Mage,
        Rogue,
        Warlock,
        Hunter,
        Paladin
    }
    public enum PlayerSex
    {
        Unknown,
        Male,
        Female,
    }
    public enum ItemSlot
    {
        Unknown = 0,
        Head = 1,
        Neck = 2,
        Shoulder = 3,
        Shirt = 4,
        Chest = 5,
        Belt = 6,
        Legs = 7,
        Feet = 8,
        Wrist = 9,
        Gloves = 10,
        Finger_1 = 11,
        Finger_2 = 12,
        Trinket_1 = 13,
        Trinket_2 = 14,
        Back = 15,
        Main_Hand = 16,
        Off_Hand = 17,
        Ranged = 18,
        Tabard = 19
    }
    public enum WowInstance
    {
        Molten_Core,
        Onyxia,
        Blackwing_Lair,
        Zul_Gurub,
        Ruins_Of_Ahn_Qiraj,
        Temple_Of_Ahn_Qiraj,
        Naxxramas,
        World_Bosses,
    }
    public enum WowBoss
    {
        //Molten Core
        Lucifron,
        Magmadar,
        Gehennas,
        Garr,
        Baron_Geddon,
        Shazzrah,
        Sulfuron_Harbinger,
        Golemagg_The_Incinerator,
        Majordomo_Executus,
        Ragnaros,
        MCTrashMobs,
        MCRandomBoss,
        
        //Onyxia
        Onyxia,

        //BWL
        Razorgore_The_Untamed,
        Vaelstrasz_The_Corrupt,
        Broodlord_Lashlayer,
        Firemaw,
        Ebonroc,
        Flamegor,
        Chromaggus,
        Nefarian,
        BWLTrashMobs,

        //ZG
        High_Priestess_Jeklik,
        High_Priest_Venoxis,
        High_Priestess_Arlokk,
        High_Priest_Thekal,
        High_Priestess_Mar_Li,
        Hakkar_The_Soulflayer,
        Broodlord_Mandokir, //TODO: change to Bloodlord_Mandokir make sure there is no bug being introduced if changed!
        Jin_Do_The_Hexxer,
        Gahz_Ranka,
        Edge_Of_Madness,
        Renataki_Of_The_Thousand_Blades,
        Wushoolay_the_Storm_Witch,
        Gri_Lek_Of_The_Iron_Blood,
        Hazzarah_The_Dreamweaver,
        ZGTrashMobs,
        ZGShared,

        //AQ20
        Kurinnaxx,
        General_Rajaxx,
        Ossirian_The_Unscarred,
        Buru_The_Gorger,
        Moam,
        Ayamiss_The_Hunter,
        Event_Captain,
        Lieutenant_General_Andorov,

        //AQ40
        The_Prophet_Skeram,
        Battleguard_Sartura,
        Fankriss_The_Unyielding,
        Huhuran,
        The_Twin_Emperors,
        C_Thun,
        Three_Bugs,
        Viscidus,
        Ouro,
        Three_Bugs_Vem_Last,
        Three_Bugs_Princess_Yauj_Last,
        Three_Bugs_Lord_Kri_Last,
        AQ40Trash,
        AQBroodRep,
        AQOpening,

        //Naxxramas
        //Arachnid Quarter
        Anub_Rekhan,
        Grand_Widow_Faerlina,
        Maexxna,
        //Construct Quarter
        Patchwerk,
        Grobbulus,
        Gluth,
        Thaddius,
        //Plague Quarter
        Noth_The_Plaguebringer,
        Heigan_The_Unclean,
        Loatheb,
        //Military Quarter
        Instructor_Razuvious,
        Gothik_The_Harvester,
        The_Four_Horsemen,
        //Frostwyrm Lair
        Sapphiron,
        Kel_Thuzad,
        NAXTrashMobs,

        //World Bosses
        Azuregos,
        Kazzak,
        Emeriss,
        Taerar,
        Lethon,
        Ysondre,

        //Item Info
        AVFriendly,
        AVHonored,
        AVRevered,
        AVExalted,

        ABFriendly,
        ABHonored,
        ABRevered,
        ABExalted,
        
        WSGFriendly,
        WSGHonored,
        WSGRevered,
        WSGExalted,

        T3Mage,
        T3Warlock,
        T3Priest,
        T3Rogue,
        T3Druid,
        T3Hunter,
        T3Paladin,
        T3Shaman,
        T3Warrior,

        T2Mage,
        T2Warlock,
        T2Priest,
        T2Rogue,
        T2Druid,
        T2Hunter,
        T2Paladin,
        T2Shaman,
        T2Warrior,

        T1Mage,
        T1Warlock,
        T1Priest,
        T1Rogue,
        T1Druid,
        T1Hunter,
        T1Paladin,
        T1Shaman,
        T1Warrior,

        T0Mage,
        T0Warlock,
        T0Priest,
        T0Rogue,
        T0Druid,
        T0Hunter,
        T0Paladin,
        T0Shaman,
        T0Warrior,

        T05Mage,
        T05Warlock,
        T05Priest,
        T05Rogue,
        T05Druid,
        T05Hunter,
        T05Paladin,
        T05Shaman,
        T05Warrior,

        ZGMage,
        ZGWarlock,
        ZGPriest,
        ZGRogue,
        ZGDruid,
        ZGHunter,
        ZGPaladin,
        ZGShaman,
        ZGWarrior,

        AQ20Mage,
        AQ20Warlock,
        AQ20Priest,
        AQ20Rogue,
        AQ20Druid,
        AQ20Hunter,
        AQ20Paladin,
        AQ20Shaman,
        AQ20Warrior,

        AQ40Mage,
        AQ40Warlock,
        AQ40Priest,
        AQ40Rogue,
        AQ40Druid,
        AQ40Hunter,
        AQ40Paladin,
        AQ40Shaman,
        AQ40Warrior,

        PVPRareMage,
        PVPRareWarlock,
        PVPRarePriest,
        PVPRareRogue,
        PVPRareDruid,
        PVPRareHunter,
        PVPRarePaladin,
        PVPRareShaman,
        PVPRareWarrior,

        PVPEpicMage,
        PVPEpicWarlock,
        PVPEpicPriest,
        PVPEpicRogue,
        PVPEpicDruid,
        PVPEpicHunter,
        PVPEpicPaladin,
        PVPEpicShaman,
        PVPEpicWarrior,

        PVPWeapons,

        Legendary,

        MCFirst = WowBoss.Lucifron,
        MCLast = WowBoss.Ragnaros,

        OnyFirst = WowBoss.Onyxia,
        OnyLast = WowBoss.Onyxia,

        BWLFirst = WowBoss.Razorgore_The_Untamed,
        BWLLast = WowBoss.Nefarian,

        ZGFirst = WowBoss.High_Priestess_Jeklik,
        ZGLast = WowBoss.Edge_Of_Madness,

        ZGEOMFirst = WowBoss.Renataki_Of_The_Thousand_Blades,
        ZGEOMLast = WowBoss.Hazzarah_The_Dreamweaver,

        AQ20First = WowBoss.Kurinnaxx,
        AQ20Last = WowBoss.Ayamiss_The_Hunter,

        AQ40First = WowBoss.The_Prophet_Skeram,
        AQ40Last = WowBoss.Ouro,

        NaxxFirst = WowBoss.Anub_Rekhan,
        NaxxLast = WowBoss.Kel_Thuzad,

        WBFirst = WowBoss.Azuregos,
        WBLast = WowBoss.Ysondre,

        PVPOffsetFirst = WowBoss.AVFriendly,
        PVPOffsetLast = WowBoss.WSGExalted,

        PVPSetFirst = WowBoss.PVPRareMage,
        PVPSetLast = WowBoss.PVPWeapons,
    }
    public class StaticValues
    {
        public static Dictionary<string, WowBoss> _WowBossConvert = new Dictionary<string, WowBoss>
        {
            //Molten Core
            {"Golemagg the Incinerator", WowBoss.Golemagg_The_Incinerator},
        
            //BWL
            {"Razorgore the Untamed", WowBoss.Razorgore_The_Untamed},
            {"Vaelstrasz the Corrupt", WowBoss.Vaelstrasz_The_Corrupt},

            //ZG
            {"High Priestess Mar'li", WowBoss.High_Priestess_Mar_Li},
            {"Hakkar", WowBoss.Hakkar_The_Soulflayer},
            {"Bloodlord Mandokir", WowBoss.Broodlord_Mandokir},
            {"Jin'do the Hexxer", WowBoss.Jin_Do_The_Hexxer},
            {"Gahz'ranka", WowBoss.Gahz_Ranka},
            {"Renataki", WowBoss.Renataki_Of_The_Thousand_Blades},
            {"Wushoolay", WowBoss.Wushoolay_the_Storm_Witch},
            {"Gri'lek", WowBoss.Gri_Lek_Of_The_Iron_Blood},
            {"Hazza'rah", WowBoss.Hazzarah_The_Dreamweaver},

            //AQ20
            {"Ossirian the Unscarred", WowBoss.Ossirian_The_Unscarred},
            {"Buru the Gorger", WowBoss.Buru_The_Gorger},
            {"Ayamiss the Hunter", WowBoss.Ayamiss_The_Hunter},

            //AQ40
            {"Fankriss the Unyielding", WowBoss.Fankriss_The_Unyielding},
            {"Princess Huhuran", WowBoss.Huhuran},
            {"Twin Emperors", WowBoss.The_Twin_Emperors},
            {"C'Thun", WowBoss.C_Thun},

            //Naxxramas
            {"Anub'Rekhan", WowBoss.Anub_Rekhan},
            {"Noth the Plaguebringer", WowBoss.Noth_The_Plaguebringer},
            {"Heigan the Unclean", WowBoss.Heigan_The_Unclean},
            {"Gothik the Harvester", WowBoss.Gothik_The_Harvester},
            {"Kel'Thuzad", WowBoss.Kel_Thuzad},

            //World Bosses
        };
        public static WowBoss ConvertWowBoss(string _BossName)
        {
            if (_WowBossConvert.ContainsKey(_BossName) == true)
                return _WowBossConvert[_BossName];
            else
            {
                WowBoss parseResult;
                if (Enum.TryParse<WowBoss>(_BossName.Replace(' ', '_'), out parseResult) == true)
                    return parseResult;
            }
            return WowBoss.Legendary;
        }
        public static Dictionary<string, PlayerClass> _ClassConvert = new Dictionary<string, PlayerClass>
        {
            {"DRUID", PlayerClass.Druid},
            {"WARRIOR", PlayerClass.Warrior},
            {"SHAMAN", PlayerClass.Shaman},
            {"PRIEST", PlayerClass.Priest},
            {"WARLOCK", PlayerClass.Warlock},
            {"MAGE", PlayerClass.Mage},
            {"ROGUE", PlayerClass.Rogue},
            {"HUNTER", PlayerClass.Hunter},
            {"PALADIN", PlayerClass.Paladin},

            {"Druid", PlayerClass.Druid},
            {"Warrior", PlayerClass.Warrior},
            {"Shaman", PlayerClass.Shaman},
            {"Priest", PlayerClass.Priest},
            {"Warlock", PlayerClass.Warlock},
            {"Mage", PlayerClass.Mage},
            {"Rogue", PlayerClass.Rogue},
            {"Hunter", PlayerClass.Hunter},
            {"Paladin", PlayerClass.Paladin},
        };
        public static Dictionary<string, PlayerRace> _RaceConvert = new Dictionary<string, PlayerRace>
        {
            {"Scourge", PlayerRace.Undead},
            {"Undead", PlayerRace.Undead},
            {"Orc", PlayerRace.Orc},
            {"Tauren", PlayerRace.Tauren},
            {"Troll", PlayerRace.Troll},
            {"Human", PlayerRace.Human},
            {"Dwarf", PlayerRace.Dwarf},
            {"Gnome", PlayerRace.Gnome},
            {"NightElf", PlayerRace.Night_Elf},
            {"Night_Elf", PlayerRace.Night_Elf},
            {"BloodElf", PlayerRace.Blood_Elf},
            {"Blood_Elf", PlayerRace.Blood_Elf},
            {"Draenei", PlayerRace.Draenei},
        };
        public static PlayerClass ConvertClass(string _Class)
        {
            if (_ClassConvert.ContainsKey(_Class) == true)
                return _ClassConvert[_Class];
            return PlayerClass.Unknown;
        }
        public static PlayerRace ConvertRace(string _Race)
        {
            if (_RaceConvert.ContainsKey(_Race) == true)
                return _RaceConvert[_Race];
            return PlayerRace.Unknown;
        }
        public static PlayerSex ConvertSex(string _Sex)
        {
            if (_Sex == "2")
                return PlayerSex.Male;
            else if (_Sex == "3")
                return PlayerSex.Female;
            else
                return PlayerSex.Unknown;
        }
        public static Dictionary<string, WowRealm> _RealmConvert = new Dictionary<string, WowRealm>
        {
            {"Emerald Dream", WowRealm.Emerald_Dream},
            {"Emerald Dream [1x] Blizzlike", WowRealm.Emerald_Dream},
            {"ED", WowRealm.Emerald_Dream},
            {"Warsong [12x] Blizzlike", WowRealm.Warsong},
            {"Warsong", WowRealm.Warsong},
            {"WSG", WowRealm.Warsong},
            {"Al'Akir [instant 60] Blizzlike", WowRealm.Al_Akir},
            {"Al&apos;Akir [instant 60] Blizzlike", WowRealm.Al_Akir},
            {"Al'Akir", WowRealm.Al_Akir},
            {"AlA", WowRealm.Al_Akir},
            {"All", WowRealm.All},
            {"VanillaGaming", WowRealm.VanillaGaming},
            {"Valkyrie", WowRealm.Valkyrie},
            {"VG", WowRealm.VanillaGaming},
            {"VAL", WowRealm.Valkyrie},
            {"Archangel", WowRealm.Archangel},
            {"Archangel [14x] Blizzlike", WowRealm.Archangel},
            {"ArA", WowRealm.Archangel},
            {"REB", WowRealm.Rebirth},
            {"Rebirth", WowRealm.Rebirth},
            {"Test_Server", WowRealm.Test_Server},
            {"TestServer", WowRealm.Test_Server},
            {"Nostalrius Begins PTR", WowRealm.Test_Server},
            {"Nostalrius Begins", WowRealm.Nostalrius},
            {"Nostalrius", WowRealm.Nostalrius},
            {"NRB", WowRealm.Nostalrius},
            {"NB", WowRealm.Nostalrius},
            {"Kronos", WowRealm.Kronos},
        };
        public static WowRealm ConvertRealm(string _Realm)
        {
            if (_RealmConvert.ContainsKey(_Realm) == true)
                return _RealmConvert[_Realm];
            return WowRealm.Unknown;
        }
        public static WowVersionEnum GetWowVersion(WowRealm _Realm)
        {
            if (_Realm == WowRealm.Emerald_Dream || _Realm == WowRealm.Warsong || _Realm == WowRealm.Al_Akir
                || _Realm == WowRealm.Rebirth || _Realm == WowRealm.Valkyrie || _Realm == WowRealm.VanillaGaming || _Realm == WowRealm.Test_Server
                || _Realm == WowRealm.Nostalrius || _Realm == WowRealm.Kronos)
            {
                return WowVersionEnum.Vanilla;
            }
            else if (_Realm == WowRealm.Archangel)
            {
                return WowVersionEnum.TBC;
            }

            Logger.ConsoleWriteLine("Error, WoW version was not specified for Realm: \"" + _Realm + "\"", ConsoleColor.Red);
            throw new Exception("Error, WoW version was not specified for Realm: \"" + _Realm + "\"");
        }
        public static int GetMaxLevel(WowVersionEnum _WowVersion)
        {
            if (_WowVersion == WowVersionEnum.Vanilla)
                return 60;
            else if (_WowVersion == WowVersionEnum.TBC)
                return 70;
            else if (_WowVersion == WowVersionEnum.WOTLK)
                return 80;
            else
                return 1;
        }
        public static PlayerFaction GetFaction(PlayerRace _Race)
        {
            if (_Race == PlayerRace.Human || _Race == PlayerRace.Gnome || _Race == PlayerRace.Dwarf || _Race == PlayerRace.Night_Elf || _Race == PlayerRace.Draenei)
                return PlayerFaction.Alliance;
            else if (_Race == PlayerRace.Orc || _Race == PlayerRace.Undead || _Race == PlayerRace.Tauren || _Race == PlayerRace.Troll || _Race == PlayerRace.Blood_Elf)
                return PlayerFaction.Horde;

            return PlayerFaction.Unknown;
        }

        public static DateTime CalculateLastRankUpdadeDateUTC(DateTime? _NowUTC = null)
        {
            DateTime rankDate = DateTime.Now;
            if (_NowUTC.HasValue)
                rankDate = _NowUTC.Value.ToLocalTime();
            rankDate = rankDate.AddHours(-rankDate.Hour);
            rankDate = rankDate.AddMinutes(-rankDate.Minute);
            rankDate = rankDate.AddDays(DayOfWeek.Sunday - rankDate.DayOfWeek);
            rankDate = rankDate.ToUniversalTime();
            return rankDate;
        }
    }
}
