using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RaidDamageDatabase
{
    public class BossInformation
    {
        public static Dictionary<string, int> BossCountInInstance = new Dictionary<string, int>
        {
            {"Onyxia's Lair", 1},
            {"Molten Core", 10},
            {"Blackwing Lair", 8},
            {"Zul'Gurub", 10},
            {"Ruins of Ahn'Qiraj", 6},
            {"Ahn'Qiraj Temple", 9},
            {"Naxxramas", 15},
        };
        public static Dictionary<string, string> InstanceAcronym = new Dictionary<string, string>
        {
            {"Onyxia's Lair", "ONY"},
            {"Molten Core", "MC"},
            {"Blackwing Lair", "BWL"},
            {"Zul'Gurub", "ZG"},
            {"Ruins of Ahn'Qiraj", "AQ20"},
            {"Ahn'Qiraj Temple", "AQ40"},
            {"Naxxramas", "Naxx"},
        };
        public static Dictionary<string, string[]> BossesInInstanceNoOptional = new Dictionary<string, string[]>
        {
            {"Onyxia's Lair", new string[]{"Onyxia"}},
            {"Molten Core", new string[]{
                "Lucifron","Magmadar", "Gehennas", 
                "Garr", "Baron Geddon", "Shazzrah", 
                "Sulfuron Harbinger", "Golemagg the Incinerator", 
                "Majordomo Executus", "Ragnaros",
            }},
            {"Blackwing Lair", new string[]{
                "Razorgore the Untamed","Vaelastrasz the Corrupt", "Broodlord Lashlayer", 
                "Firemaw", "Ebonroc", "Flamegor", 
                "Chromaggus", "Nefarian",
            }},
            {"Zul'Gurub", new string[]{
                "High Priestess Jeklik","High Priest Venoxis", "High Priestess Mar'li", 
                "High Priest Thekal", "High Priestess Arlokk", "Hakkar",
            }},
            {"Ruins of Ahn'Qiraj", new string[]{
                "Kurinnaxx", "General Rajaxx", "Ossirian the Unscarred", 
            }},
            {"Ahn'Qiraj Temple", new string[]{
                "The Prophet Skeram", "Battleguard Sartura", 
                "Fankriss the Unyielding", "Princess Huhuran", 
                "Twin Emperors", "C'Thun"
            }},
            {"Naxxramas", new string[]{"Anub'Rekhan", "Grand Widow Faerlina",
                "Maexxna", "Patchwerk",
                "Grobbulus", "Gluth",
                "Thaddius", "Noth the Plaguebringer",
                "Heigan the Unclean", "Loatheb",
                "Instructor Razuvious", "Gothik the Harvester",
                "The Four Horsemen", "Sapphiron",
                "Kel'Thuzad",
            }},
            {"Naxxramas - Arachnid Quarter", new string[]{
                "Anub'Rekhan", "Grand Widow Faerlina", "Maexxna",
            }},
            {"Naxxramas - Construct Quarter", new string[]{
                "Patchwerk", "Grobbulus", "Gluth", "Thaddius",
            }},
            {"Naxxramas - Plague Quarter", new string[]{
                "Noth the Plaguebringer",
                "Heigan the Unclean", "Loatheb",
            }},
            {"Naxxramas - Military Quarter", new string[]{
                "Instructor Razuvious", "Gothik the Harvester",
                "The Four Horsemen",
            }},
        };
        public static Dictionary<string, string> LastInstanceBoss = new Dictionary<string, string>
        {
            {"Onyxia's Lair", "Onyxia"},
            {"Molten Core", "Ragnaros"},
            {"Blackwing Lair", "Nefarian"},
        };
        public static List<string> BossWithStartYell = new List<string>
        {
            "Majordomo Executus",
            "Onyxia",
            //"High Priestess Jeklik",
            "High Priestess Mar'li",
            "Bloodlord Mandokir",
            "High Priest Thekal",
            "High Priestess Arlokk",
            "Jin'do the Hexxer",
            "Hakkar",

            "Razorgore the Untamed",
            "Vaelastrasz the Corrupt",
            "Broodlord Lashlayer",
            "Nefarian",

            "The Prophet Skeram",
            "Battleguard Sartura",

            "Gothik the Harvester",
            "Noth the Plaguebringer",
            "Heigan the Unclean",
            "Anub'Rekhan",
            "Grand Widow Faerlina",
            "Instructor Razuvious",
            //"The Four Horsemen", 
            "Patchwerk",
        };
        public static List<string> BossWithEndYell = new List<string>
        {
            "Majordomo Executus",
            "High Priestess Jeklik",
            "High Priest Venoxis",
            "High Priestess Mar'li",
            "High Priest Thekal",
            "High Priestess Arlokk",

            "Nefarian",

            "The Prophet Skeram",
            "Battleguard Sartura",

            "Gothik the Harvester",
            "Noth the Plaguebringer",
            "Heigan the Unclean",
            "Grand Widow Faerlina",
            "Instructor Razuvious",
            //"The Four Horsemen",
            "Patchwerk",
            "Thaddius",
        };
        public static Dictionary<string, string> BossFights = new Dictionary<string, string>
        {
            //Trash
            {"Trash", "Trash"},

            //MC
            {"Lucifron", "Molten Core"},
	        {"Magmadar", "Molten Core"},
	        {"Gehennas", "Molten Core"},
	        {"Garr", "Molten Core"},
	        {"Baron Geddon", "Molten Core"},
	        {"Shazzrah", "Molten Core"},
	        {"Sulfuron Harbinger", "Molten Core"},
	        {"Golemagg the Incinerator", "Molten Core"},
	        {"Majordomo Executus", "Molten Core"},
	        {"Ragnaros","Molten Core"},
	
	        //Onyxia
	        {"Onyxia", "Onyxia's Lair"},
	
	        //BWL
	        {"Razorgore the Untamed", "Blackwing Lair"},
	        {"Vaelastrasz the Corrupt", "Blackwing Lair"},
	        {"Broodlord Lashlayer", "Blackwing Lair"},
	        {"Firemaw", "Blackwing Lair"},
	        {"Ebonroc", "Blackwing Lair"},
	        {"Flamegor", "Blackwing Lair"},
	        {"Chromaggus", "Blackwing Lair"},
	        {"Nefarian", "Blackwing Lair"},
	
	        //ZG
	        {"High Priestess Jeklik", "Zul'Gurub"},
	        {"High Priest Venoxis", "Zul'Gurub"},
	        {"High Priestess Mar'li", "Zul'Gurub"},
	        {"High Priest Thekal", "Zul'Gurub"},
	        {"High Priestess Arlokk", "Zul'Gurub"},
	        {"Hakkar the Soulflayer", "Zul'Gurub"},//inte rätt
	        {"Hakkar", "Zul'Gurub"},
	        {"Bloodlord Mandokir", "Zul'Gurub"},
	        {"Jin'do the Hexxer", "Zul'Gurub"},
	        {"Gahz'ranka", "Zul'Gurub"},
	        {"Gri'lek", "Zul'Gurub"},
	        {"Renataki", "Zul'Gurub"},
	        {"Hazza'rah", "Zul'Gurub"},
	        {"Wushoolay", "Zul'Gurub"},
	
	        //AQ20
	        {"Kurinnaxx", "Ruins of Ahn'Qiraj"},
	        {"General Rajaxx", "Ruins of Ahn'Qiraj"},
	        {"Moam", "Ruins of Ahn'Qiraj"},
	        {"Buru the Gorger", "Ruins of Ahn'Qiraj"},
	        {"Ayamiss the Hunter", "Ruins of Ahn'Qiraj"},
	        {"Ossirian the Unscarred", "Ruins of Ahn'Qiraj"},
	
	        //AQ40
	        {"C'Thun", "Ahn'Qiraj Temple"},
	        {"Twin Emperors", "Ahn'Qiraj Temple"},
	        {"Ouro", "Ahn'Qiraj Temple"},
	        {"Viscidus", "Ahn'Qiraj Temple"},
	        {"Princess Huhuran", "Ahn'Qiraj Temple"},
	        {"Anubisath Defenders", "Ahn'Qiraj Temple"}, //whaaat? varför har jag denna här?
	        {"Fankriss the Unyielding", "Ahn'Qiraj Temple"},
	        {"Battleguard Sartura", "Ahn'Qiraj Temple"},
	        {"The Prophet Skeram", "Ahn'Qiraj Temple"},
	        {"Three Bugs", "Ahn'Qiraj Temple"},
            //{"Vem", "Ahn'Qiraj Temple"},
            //{"Lord Kri", "Ahn'Qiraj Temple"},
            //{"Princess Yauj", "Ahn'Qiraj Temple"},
	
            //Naxx
            {"Anub'Rekhan", "Naxxramas"},
            {"Grand Widow Faerlina", "Naxxramas"},
            {"Maexxna", "Naxxramas"},
            {"Patchwerk", "Naxxramas"},
            {"Grobbulus", "Naxxramas"},
            {"Gluth", "Naxxramas"},
            {"Thaddius", "Naxxramas"},
            {"Noth the Plaguebringer", "Naxxramas"},
            {"Heigan the Unclean", "Naxxramas"},
            {"Loatheb", "Naxxramas"},
            {"Instructor Razuvious", "Naxxramas"},
            {"Gothik the Harvester", "Naxxramas"},
            {"The Four Horsemen", "Naxxramas"},
            //{"Highlord Mograine", "Naxxramas"},
            //{"Thane Korth'azz", "Naxxramas"},
            //{"Lady Blaumeux", "Naxxramas"},
            //{"Sir Zeliek", "Naxxramas"},
            {"Sapphiron", "Naxxramas"},
            {"Kel'Thuzad", "Naxxramas"},

	        //UBRS
	        {"Pyroguard Emberseer", "UBRS"},
	        {"Warchief Rend Blackhand", "UBRS"},
	        {"The Beast", "UBRS"},
	        {"General Drakkisath", "UBRS"},
        };
        public static Dictionary<string, string[]> BossParts = new Dictionary<string, string[]>
        { 
            //Sista kommer användas som "MAIN BOSS"
            //DVS Razorgore, Nefarian och C'Thun måste ligga sisst i arrayen!!! 
            //Exempel där detta används: FightEvent.cs rad 82: if (currUnit.Key == _Fight.m_Fight.FightUnitIDs.Last())

            {"Three Bugs", new string[]{"Vem", "Lord Kri", "Princess Yauj"}},
            {"Twin Emperors", new string[]{"Emperor Vek'lor", "Emperor Vek'nilash"}},
            //{"C'Thun", new string[]{"Eye Tentacle", "Giant Claw Tentacle", "Giant Eye Tentacle", "Eye of C'Thun", "Flesh Tentacle", "C'Thun"}},
            //{"C'Thun", new string[]{"Eye of C'Thun", "C'Thun"}},
            {"The Four Horsemen", new string[]{"Highlord Mograine", "Thane Korth'azz", "Lady Blaumeux", "Sir Zeliek"}},
            
            //{"Razorgore the Untamed", new string[]{"Grethok the Controller", "Blackwing Guardsman", "Death Talon Dragonspawn", "Blackwing Legionnaire", "Blackwing Mage", "Razorgore the Untamed"}},
            //{"Nefarian", new string[]{"Lord Victor Nefarius", "Red Drakonid", "Blue Drakonid", "Green Drakonid", "Black Drakonid", "Bronze Drakonid", "Chromatic Drakonid", "Nefarian"}},
        };
        public static Dictionary<string, string[]> BossAdds = new Dictionary<string, string[]>
        {
            //MC
            {"Lucifron", new string[]{"Flamewaker Protector"}},
	        {"Gehennas", new string[]{"Flamewaker"}},
	        {"Garr", new string[]{"Firesworn"}},
	        {"Sulfuron Harbinger", new string[]{"Flamewaker Priest"}},
	        {"Golemagg the Incinerator", new string[]{"Core Rager"}},
	        {"Majordomo Executus", new string[]{"Flamewaker Healer", "Flamewaker Elite"}},
	        {"Ragnaros", new string[]{"Son of Flame"}},
	
	        //BWL
            {"Razorgore the Untamed", new string[]{"Grethok the Controller", "Blackwing Guardsman", "Death Talon Dragonspawn", "Blackwing Legionnaire", "Blackwing Mage"}},
	        {"Nefarian", new string[]{"Bone Construct", "Corrupted Infernal", "Red Drakonid", "Blue Drakonid", "Green Drakonid", "Black Drakonid", "Bronze Drakonid", "Chromatic Drakonid"}},

            //ZG
            {"High Priestess Jeklik", new string[]{"Bloodseeker Bat"}},
            {"High Priest Venoxis", new string[]{"Razzashi Cobra"}},
            {"High Priestess Mar'li", new string[]{"Spawn of Mar'li", "Witherbark Speaker"}},
	        {"High Priest Thekal", new string[]{"Zealot Zath", "Zealot Lor'Khan", "Zulian Tiger"}},
	        {"High Priestess Arlokk", new string[]{"Zulian Prowler"}},
	        {"Hakkar", new string[]{"Son of Hakkar"}},
	        {"Bloodlord Mandokir", new string[]{"Ohgan"}},
	        {"Jin'do the Hexxer", new string[]{"Powerful Healing Ward", "Brain Wash Totem", "Sacrificed Troll", "Shade of Jin'do"}},
        
            //AQ40
            {"Three Bugs", new string[]{"Vem", "Lord Kri", "Princess Yauj"}},
            {"Battleguard Sartura", new string[]{"Sartura's Royal Guard"}},
            {"Fankriss the Unyielding", new string[]{"Spawn of Fankriss", "Vekniss Hatchling"}},
            {"Viscidus", new string[]{"Glob of Viscidus"}},
            {"Twin Emperors", new string[]{"Emperor Vek'lor", "Emperor Vek'nilash", "Qiraji Scarab", "Qiraji Scorpion"}},
            {"C'Thun", new string[]{"Eye Tentacle", "Giant Claw Tentacle", "Giant Eye Tentacle", "Eye of C'Thun", "Flesh Tentacle"}},

            //Naxx
            {"Instructor Razuvious", new string[]{"Death Knight Understudy"}},
            {"Noth the Plaguebringer", new string[]{"Plagued Champion", "Plagued Guardian", "Plagued Warrior"}},
            {"Anub'Rekhan", new string[]{"Crypt Guard", "Corpse Scarab"}},
            {"Maexxna", new string[]{"Web Wrap", "Maexxna Spiderling"}},
            
            {"Grobbulus", new string[]{"Fallout Slime"}},
            {"Gluth", new string[]{"Zombie Chow"}},

            {"The Four Horsemen", new string[]{"Highlord Mograine", "Thane Korth'azz", "Lady Blaumeux", "Sir Zeliek"}},

            {"Kel'Thuzad", new string[]{"Unstoppable Abomination", "Soul Weaver", "Soldier of the Frozen Wastes", "Guardian of Icecrown"}},

            {"Grand Widow Faerlina", new string[]{"Naxxramas Follower", "Naxxramas Worshipper"}},
            {"Gothik the Harvester", new string[]{"Unrelenting Deathknight", "Unrelenting Death Knight", "Unrelenting Rider", "Spectral Trainee", "Spectral Deathknight", "Spectral Death Knight", "Spectral Rider", "Spectral Horse"}},
        };

        public static List<string> GetAllEntitiesForFight(string _FightName)
        {
            List<string> retList = new List<string>();
            retList.Add(_FightName);
            string[] bossAdds = null;
            if (BossInformation.BossAdds.TryGetValue(_FightName, out bossAdds) == true)
            {
                foreach (var bossAdd in bossAdds)
                {
                    retList.Add(bossAdd);
                }
            }
            return retList;
        }
    }
}
