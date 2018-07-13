using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;
using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerItem = VF_RealmPlayersDatabase.ItemSlot;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;

namespace RealmPlayersServer.Code.Resources
{
    public class VisualResources
    {
        public static Dictionary<PlayerRace, string> _RaceCrestImage = new Dictionary<PlayerRace, string>
        {
            {PlayerRace.Undead, "http://images4.wikia.nocookie.net/__cb20111016213839/wowwiki/images/3/35/ForsakenCrest.jpg"},
            {PlayerRace.Orc, "http://images1.wikia.nocookie.net/__cb20111016214820/wowwiki/images/0/07/OrcCrest.jpg"},
            {PlayerRace.Tauren, "http://images4.wikia.nocookie.net/__cb20111016215116/wowwiki/images/d/d4/TaurenCrest.jpg"},
            {PlayerRace.Troll, "http://images2.wikia.nocookie.net/__cb20111016214956/wowwiki/images/0/0f/TrollCrest.jpg"},
            {PlayerRace.Human, "http://images2.wikia.nocookie.net/__cb20050804060403/wowwiki/images/2/29/Human_Crest.jpg"},
            {PlayerRace.Dwarf, "http://images2.wikia.nocookie.net/__cb20121116163927/wowwiki/images/9/9f/DwarfCrest.jpg"},
            {PlayerRace.Gnome, "http://images2.wikia.nocookie.net/__cb20050804060106/wowwiki/images/d/db/Gnome_Crest.jpg"},
            {PlayerRace.Night_Elf, "http://images3.wikia.nocookie.net/__cb20111016214348/wowwiki/images/3/38/NightElfCrest.jpg"},
            {PlayerRace.Blood_Elf, "http://img3.wikia.nocookie.net/__cb20070308172149/wowwiki/images/thumb/1/1f/Icon_of_Blood.jpg/406px-Icon_of_Blood.jpg"},
            {PlayerRace.Draenei, "http://img1.wikia.nocookie.net/__cb20070218054243/wowwiki/images/6/63/Draenei_Icon.png"},
        };
        public static Dictionary<PlayerClass, string> _ClassColors = new Dictionary<PlayerClass, string>
        {
	        {PlayerClass.Druid  , "#ff7d0a"},
	        {PlayerClass.Warrior, "#c79c6e"},
	        {PlayerClass.Shaman , "#0070DE"},
	        {PlayerClass.Priest , "#ffffff"},
	        {PlayerClass.Mage   , "#69ccf0"},
	        {PlayerClass.Rogue  , "#fff569"},
	        {PlayerClass.Warlock, "#9482ca"},
	        {PlayerClass.Hunter , "#abd473"},
	        {PlayerClass.Paladin , "#f58cba"},
	        {PlayerClass.Unknown , "#777777"},
        };
        public static Dictionary<PlayerFaction, string> _FactionColors = new Dictionary<PlayerFaction, string>
        {
            {PlayerFaction.Horde, "#f70002"},
            {PlayerFaction.Alliance, "#007df7"},
            {PlayerFaction.Unknown, "#cccccc"},
        };
        public static Dictionary<PlayerFaction, string> _FactionImgUrl = new Dictionary<PlayerFaction, string>
        {
	        {PlayerFaction.Horde  , "/assets/img/icons/Horde_32.png"},
	        {PlayerFaction.Alliance  , "/assets/img/icons/Alliance_32.png"},
	        {PlayerFaction.Unknown  , ""},
        };
        public static Dictionary<PlayerFaction, string> _FactionCSSName = new Dictionary<PlayerFaction, string>
        {
	        {PlayerFaction.Horde  , "horde"},
	        {PlayerFaction.Alliance  , "alliance"},
	        {PlayerFaction.Unknown  , ""},
        };
        public static Dictionary<WowRealm, string> _RealmParamString = new Dictionary<WowRealm, string>
        {
	        {WowRealm.Emerald_Dream , "ED"},
	        {WowRealm.Al_Akir , "AlA"},
            {WowRealm.Warsong , "WSG"},
            {WowRealm.WarsongTBC , "WBC"},
            {WowRealm.Unknown , "Unknown"},
	        {WowRealm.All , "All"},
	        {WowRealm.Archangel , "ArA"},
	        {WowRealm.VanillaGaming , "VG"},
	        {WowRealm.Valkyrie , "VAL"},
	        {WowRealm.Rebirth , "REB"},
	        {WowRealm.Nostalrius , "Ana"},
	        {WowRealm.NostalriusPVE , "Dar"},
	        {WowRealm.Kronos , "KRO"},
	        {WowRealm.KronosII , "KR2"},
	        {WowRealm.NostalGeek , "NG"},
	        {WowRealm.Nefarian , "NEF"},
	        {WowRealm.Test_Server , "TSV"},
	        {WowRealm.Vengeance_Wildhammer , "VWH"},
	        {WowRealm.ExcaliburTBC , "EXC"},
            {WowRealm.L4G_Hellfire , "HLF"},
            {WowRealm.Warsong2 , "WS2"},
            {WowRealm.Vengeance_Stonetalon , "VST"},
            {WowRealm.Elysium , "ELY"},
            {WowRealm.Elysium2 , "LB"},
            {WowRealm.Zeth_Kur , "ZeK"},
            {WowRealm.Nemesis , "NES"},
            {WowRealm.HellGround , "HG"},
            {WowRealm.Nostralia , "NST"},
            {WowRealm.Hellfire2, "HF2" },
            {WowRealm.Outland, "OUT" },
            {WowRealm.Medivh, "MDV" },
            {WowRealm.Firemaw, "FMW" },
            {WowRealm.Felmyst, "FLM" },
            {WowRealm.Ares, "AR" },
            {WowRealm.Nighthaven, "NH" },
            {WowRealm.Northdale, "ND" },
        };
        public static Dictionary<WowRealm, string> _RealmVisualString = new Dictionary<WowRealm, string>
        {
	        {WowRealm.Emerald_Dream , "Emerald Dream"},
	        {WowRealm.Al_Akir , "Al'Akir"},
            {WowRealm.Warsong , "Warsong"},
            {WowRealm.WarsongTBC , "WarsongTBC"},
            {WowRealm.Unknown , "Unknown"},
	        {WowRealm.All , "All"},
	        {WowRealm.Archangel , "Archangel"},
	        {WowRealm.VanillaGaming , "VanillaGaming"},
	        {WowRealm.Valkyrie , "Valkyrie"},
	        {WowRealm.Rebirth , "Rebirth"},
            {WowRealm.Nostalrius , "Anathema"},
            {WowRealm.NostalriusPVE , "Darrowshire"},
	        {WowRealm.Kronos , "Kronos"},
	        {WowRealm.KronosII , "Kronos II"},
	        {WowRealm.NostalGeek , "NostalGeek"},
	        {WowRealm.Nefarian , "Nefarian"},
	        {WowRealm.Test_Server , "Test Server"},
	        {WowRealm.Vengeance_Wildhammer , "Wildhammer"},
            {WowRealm.ExcaliburTBC , "ExcaliburTBC"},
            {WowRealm.L4G_Hellfire , "Hellfire I"},
            {WowRealm.Warsong2 , "Warsong"},
            {WowRealm.Vengeance_Stonetalon , "Stonetalon"},
            {WowRealm.Elysium , "Elysium(Old)"},
            {WowRealm.Elysium2 , "Lightbringer"},
            {WowRealm.Zeth_Kur , "Zeth'Kur"},
            {WowRealm.Nemesis , "Nemesis"},
            {WowRealm.HellGround , "WarGate"},
            {WowRealm.Nostralia , "Nostralia"},
            {WowRealm.Hellfire2, "Hellfire II" },
            {WowRealm.Outland, "Outland" },
            {WowRealm.Medivh, "Medivh" },
            {WowRealm.Firemaw, "Firemaw" },
            {WowRealm.Felmyst, "Felmyst" },
            {WowRealm.Ares, "Ares" },
            {WowRealm.Nighthaven, "Nighthaven" },
            {WowRealm.Northdale, "Northdale" },
        };
        public static Dictionary<int, string> _HordeRankVisualName = new Dictionary<int, string>
        {
            {0, "No Rank"},
            {1, "Scout"},
            {2, "Grunt"},
            {3, "Sergeant"},
            {4, "Senior Sergeant"},
            {5, "First Sergeant"},
            {6, "Stone Guard"},
            {7, "Blood Guard"},
            {8, "Legionnaire"},
            {9, "Centurion"},
            {10, "Champion"},
            {11, "Lieutenant General"},
            {12, "General"},
            {13, "Warlord"},
            {14, "High Warlord"},
        };
        public static Dictionary<int, string> _AllianceRankVisualName = new Dictionary<int, string>
        {
            {0, "No Rank"},
            {1, "Private"},
            {2, "Corporal"},
            {3, "Sergeant"},
            {4, "Master Sergeant"},
            {5, "Sergeant Major"},
            {6, "Knight"},
            {7, "Knight-Lieutenant"},
            {8, "Knight-Captain"},
            {9, "Knight-Champion"},
            {10, "Lieutenant Commander"},
            {11, "Commander"},
            {12, "Marshal"},
            {13, "Field Marshal"},
            {14, "Grand Marshal"},
        };
    }
}