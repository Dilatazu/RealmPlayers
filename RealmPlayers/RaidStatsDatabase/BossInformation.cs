using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * This file contains lists of all raids and dungeon bosses and zones.
 * There also are various information about bosses such as if they have boss yells
 * and if there are any adds.
 */

namespace VF_RaidDamageDatabase
{
    public class BossInformation
    {
        public static bool IsDungeonZone(string _Zone)
        {
            return _Zone == "Ragefire Chasm" || _Zone == "The Deadmines" || _Zone == "Stratholme"
                || _Zone == "Scholomance" || _Zone == "Dire Maul" || _Zone == "Hall of Blackhand" 
                || _Zone == "The Temple of Atal'Hakkar" || _Zone == "Zul'Farrak" || _Zone == "Wailing Caverns"
                || _Zone == "Shadowfang Keep" || _Zone == "Razorfen Kraul";
        }
        public static Dictionary<string, Dictionary<string, string[]>> InstanceRuns = new Dictionary<string, Dictionary<string, string[]>>
        {
            {"Onyxia's Lair", new Dictionary<string, string[]>{{"Onyxia", new string[]{"Onyxia"}}}},
            {"Molten Core", new Dictionary<string, string[]>{{"Molten Core", new string[]{
                "Lucifron","Magmadar", "Gehennas", 
                "Garr", "Baron Geddon", "Shazzrah", 
                "Sulfuron Harbinger", "Golemagg the Incinerator", 
                "Majordomo Executus", "Ragnaros",
            }}}},
            {"Blackwing Lair", new Dictionary<string, string[]>{{"Blackwing Lair", new string[]{
                "Razorgore the Untamed","Vaelastrasz the Corrupt", "Broodlord Lashlayer", 
                "Firemaw", "Ebonroc", "Flamegor", 
                "Chromaggus", "Nefarian",
            }}}},
            {"Zul'Gurub", new Dictionary<string, string[]>{{"Zul'Gurub", new string[]{
                "High Priestess Jeklik","High Priest Venoxis", "High Priestess Mar'li", 
                "High Priest Thekal", "High Priestess Arlokk", "Hakkar",
                
                "Bloodlord Mandokir", "Jin'do the Hexxer", //"Optional"
            }}}},
            {"Ruins of Ahn'Qiraj", new Dictionary<string, string[]>{{"Ruins of Ahn'Qiraj", new string[]{
                "Kurinnaxx", "General Rajaxx",
	            "Moam", "Buru the Gorger",
	            "Ayamiss the Hunter", "Ossirian the Unscarred",
            }}}},
            {"Ahn'Qiraj Temple", new Dictionary<string, string[]>{{"Temple of Ahn'Qiraj", new string[]{
                "The Prophet Skeram", "Battleguard Sartura", 
                "Fankriss the Unyielding", "Princess Huhuran", 
                "Twin Emperors", "C'Thun"
            }}}},
            {"Naxxramas", new Dictionary<string, string[]>{
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
            {"Naxxramas - All Quarters", new string[]{
                "Anub'Rekhan", "Grand Widow Faerlina", "Maexxna",
                "Patchwerk", "Grobbulus", "Gluth", "Thaddius",
                "Noth the Plaguebringer",
                "Heigan the Unclean", "Loatheb",
                "Instructor Razuvious", "Gothik the Harvester",
                "The Four Horsemen",
            }}}},
        };

        public static Dictionary<string, int> BossCountInInstance = new Dictionary<string, int>
        {
            {"Onyxia's Lair", 1},
            {"Molten Core", 10},
            {"Blackwing Lair", 8},
            {"Zul'Gurub", 10},
            {"Ruins of Ahn'Qiraj", 6},
            {"Ahn'Qiraj Temple", 9},
            {"Naxxramas", 15},
            
            ///////////////////////TBC///////////////////////
            {"Karazhan", 8},//Not counting chess event and some other minor ones
            {"Zul'Aman", 6},
            {"Magtheridon's Lair", 1},
            {"Gruul's Lair", 2},
            {"Serpentshrine Cavern", 6},
            {"The Eye", 4},
            {"Black Temple", 9},
            {"Hyjal Summit", 5},
            {"Sunwell Plateau", 7},
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
            
            ///////////////////////DUNGEONS///////////////////////
            {"Ragefire Chasm", "RFC"},
            {"The Deadmines", "DM"},
	        {"Wailing Caverns", "WC"},
	        {"Stockades", "STO"},
	        {"Shadowfang Keep", "SFK"},
	        {"Blackfathom Deeps", "BFD"},
	        {"Razorfen Kraul", "RFK"},
	        {"Gnomeregan", "GNO"},
	        {"Scarlet Monastery", "SM"},
	        {"Razorfen Downs", "RFD"},
	        {"Uldaman", "ULDA"},
	        {"Zul'Farrak", "ZF"},
	        {"Maraudon", "MARA"},
	        {"The Temple of Atal'Hakkar", "ST"},
	        {"Blackrock Depths", "BRD"},
	        {"Hall of Blackhand", "BRS"}, //Blackrock Spire
	        {"Stratholme", "STRAT"},
	        {"Scholomance", "SCHOLO"},
	        {"Dire Maul", "DMA"},

            ///////////////////////TBC///////////////////////
            {"Karazhan", "KZ"},
            {"Zul'Aman", "ZA"},
            {"Magtheridon's Lair", "ML"},
            {"Gruul's Lair", "GL"},
            {"Serpentshrine Cavern", "SSC"},
            {"Tempest Keep", "TK"},
            {"Black Temple", "BT"},
            {"Hyjal Summit", "MH"},
            {"Sunwell Plateau", "SWP"},
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
            {"Naxxramas - All Quarters", new string[]{"Anub'Rekhan", "Grand Widow Faerlina",
                "Maexxna", "Patchwerk",
                "Grobbulus", "Gluth",
                "Thaddius", "Noth the Plaguebringer",
                "Heigan the Unclean", "Loatheb",
                "Instructor Razuvious", "Gothik the Harvester",
                "The Four Horsemen",
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
        public static List<string> FightsWithDisappearingBoss = new List<string>
        {
            "Gothik the Harvester",
            "Noth the Plaguebringer",
            "Nefarian",
            "Razorgore the Untamed",
            "C'Thun",
            "Kel'Thuzad",
            "Ragnaros",
            "Kael'thas Sunstrider",
            "High Astromancer Solarian",
            "Illidan Stormrage",
            "The Lurker Below",
        };
        #region BossFights
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
            
            ///////////////////////DUNGEONS///////////////////////
            //Ragefire Chasm
	        {"Oggleflint", "Ragefire Chasm"},
	        {"Taragaman the Hungerer", "Ragefire Chasm"},
	        {"Bazzalan", "Ragefire Chasm"},
	        {"Jergosh the Invoker", "Ragefire Chasm"},

	        //The Deadmines
	        {"Rhahk'Zor", "The Deadmines"},
	        {"Sneed", "The Deadmines"},
	        {"Gilnid", "The Deadmines"},
	        {"Mr. Smite", "The Deadmines"},
	        {"Captain Greenskin", "The Deadmines"},
	        {"Edwin VanCleef", "The Deadmines"},
	        //Optional/Rare
	        {"Miner Johnson", "The Deadmines"},
	        {"Cookie", "The Deadmines"},
            
            //Wailing Caverns
	        {"Lady Anacondra", "Wailing Caverns"},
	        {"Lord Cobrahn", "Wailing Caverns"},
	        {"Lord Pythas", "Wailing Caverns"},
	        {"Lord Serpentis", "Wailing Caverns"},
	        {"Verdan the Everliving", "Wailing Caverns"},
	        //Optional/Rare
	        {"Skum", "Wailing Caverns"},
	        {"Kresh", "Wailing Caverns"},
	        {"Mutanus the Devourer", "Wailing Caverns"},

            //Shadowfang Keep
	        {"Baron Silverlaine", "Shadowfang Keep"},
	        {"Commander Springvale", "Shadowfang Keep"},
	        {"Odo the Blindwatcher", "Shadowfang Keep"},
	        {"Wolf Master Nandos", "Shadowfang Keep"},
	        {"Archmage Arugal", "Shadowfang Keep"},
	        //Optional/Rare
	        {"Razorclaw the Butcher", "Shadowfang Keep"},

            //Razorfen Kraul
	        {"Death Speaker Jargba", "Razorfen Kraul"},
	        {"Aggem Thorncurse", "Razorfen Kraul"},
	        {"Overlord Ramtusk", "Razorfen Kraul"},
	        {"Earthcaller Halmgar", "Razorfen Kraul"},
	        {"Agathelos the Raging", "Razorfen Kraul"},
	        {"Charlga Razorflank", "Razorfen Kraul"},
	        //Optional/Rare
	        {"Blind Hunter", "Razorfen Kraul"},


	        //Uldaman
            {"Ironaya", "Uldaman"},
            {"Galgann Firehammer", "Uldaman"},
            {"Ancient Stone Keeper", "Uldaman"},
            {"Grimlok", "Uldaman"},
            {"Archaedas", "Uldaman"},
            //--Optional/Rare
            {"Revelosh", "Uldaman"},
            {"The Three Dwarfs", "Uldaman"},
            {"Obsidian Sentinel", "Uldaman"},

            //Zul'Farrak
	        {"Theka the Martyr", "Zul'Farrak"},
	        {"Antu'sul", "Zul'Farrak"},
	        {"Chief Ukorz Sandscalp", "Zul'Farrak"},
	        //Optional/Rare
	        {"Zerillis", "Zul'Farrak"},
	        {"Shadowpriest Sezz'ziz", "Zul'Farrak"},
	        {"Gahz'rilla", "Zul'Farrak"},

            //Sunken Temple
            {"Zul'Lor", "The Temple of Atal'Hakkar"},
	        {"Mijan", "The Temple of Atal'Hakkar"},
	        {"Hukku", "The Temple of Atal'Hakkar"},
	        {"Loro", "The Temple of Atal'Hakkar"},
	        {"Gasher", "The Temple of Atal'Hakkar"},
	        {"Weaver", "The Temple of Atal'Hakkar"},
	        {"Dreamscythe", "The Temple of Atal'Hakkar"},
	        {"Jammal'an", "The Temple of Atal'Hakkar"}, //Jammal'an the Prophet && Ogom the Wretched
	        {"Morphaz", "The Temple of Atal'Hakkar"},
	        {"Hazzas", "The Temple of Atal'Hakkar"},
	        {"Shade of Eranikus", "The Temple of Atal'Hakkar"},
	        //Optional/Rare
	        {"Avatar of Hakkar", "The Temple of Atal'Hakkar"},
            


            //Stratholme
            //Living
            {"Malor the Zealous", "Stratholme"},
	        {"Cannon Master Willey", "Stratholme"},
	        {"Archivist Galford", "Stratholme"},
	        {"Grand Crusader Dathrohan", "Stratholme"},
	        {"Balnazzar", "Stratholme"},
	        //Undead
	        {"Magistrate Barthilas", "Stratholme"},
            {"Nerub'enkan", "Stratholme"},
            {"Baroness Anastari", "Stratholme"},
            {"Maleki the Pallid", "Stratholme"},
            {"Ramstein the Gorger", "Stratholme"},
            {"Baron Rivendare", "Stratholme"},
	        //Optional/Rare
	        {"The Unforgiven", "Stratholme"},
            {"Skul", "Stratholme"},

            //Scholomance
	        {"Rattlegore", "Scholomance"},
            {"Vectus And Marduk", "Scholomance"},
	        //["Marduk Blackpool", "Scholomance"},
	        //["Vectus", "Scholomance"},
	        {"Ras Frostwhisper", "Scholomance"},
	        {"Lorekeeper Polkelt", "Scholomance"},
	        {"Doctor Theolen Krastinov", "Scholomance"},
	        {"Instructor Malicia", "Scholomance"},
	        {"Lady Illucia Barov", "Scholomance"},
	        {"The Ravenian", "Scholomance"},
	        {"Lord Alexei Barov", "Scholomance"},
	        {"Darkmaster Gandling", "Scholomance"},
	        //Optional/Rare
	        {"Jandice Barov", "Scholomance"},
	        {"Kirtonos the Herald", "Scholomance"},
            
	        //Dire Maul
	        //East
	        {"Hydrospawn", "Dire Maul"},
	        {"Zevrim Thornhoof", "Dire Maul"},
	        {"Lethtendris And Pimgib", "Dire Maul"}, //Lethtendris && Pimgib
	        {"Alzzin the Wildshaper", "Dire Maul"},
	        //West
	        {"Tendris Warpwood", "Dire Maul"},
	        {"Magister Kalendris", "Dire Maul"},
	        {"Illyanna And Ferra", "Dire Maul"}, //Illyanna Ravenoak && Ferra
	        {"Immol'thar", "Dire Maul"},
	        {"Prince Tortheldrin", "Dire Maul"},
	        //North
	        {"Guard Mol'dar", "Dire Maul"},
	        {"Guard Fengus", "Dire Maul"},
	        {"Guard Slip'kik", "Dire Maul"},
	        {"Captain Kromcrush", "Dire Maul"},
	        {"King Gordok", "Dire Maul"},
	        //Optional/Rare
	        {"Stomper Kreeg", "Dire Maul"},


	        //UBRS
	        {"Pyroguard Emberseer", "Hall of Blackhand"},
	        {"Warchief Rend Blackhand", "Hall of Blackhand"},
	        {"The Beast", "Hall of Blackhand"},
	        {"General Drakkisath", "Hall of Blackhand"},
	        //Optional/Rare
	        {"Jed Runewatcher", "Hall of Blackhand"},

	        //LBRS
	        {"Highlord Omokk", "Hall of Blackhand"},
	        {"Shadow Hunter Vosh'gajin", "Hall of Blackhand"},
	        {"War Master Voone", "Hall of Blackhand"},
	        {"Mother Smolderweb", "Hall of Blackhand"},
	        {"Quartermaster Zigris", "Hall of Blackhand"},
	        {"Halycon", "Hall of Blackhand"},
	        {"Overlord Wyrmthalak", "Hall of Blackhand"},
	        //Optional/Rare
	        {"Gizrul the Slavener", "Hall of Blackhand"},

            ///////////////////////TBC///////////////////////

            //Karazhan
	        //Attumen the Huntsman
	        {"Attumen the Huntsman", "Karazhan"},
	        //{"Midnight", "Karazhan"}, //ATTUMEN ADD
	        //Attumen the Huntsman
	        //Moroes
	        {"Moroes", "Karazhan"},
	        //{"Baroness Dorothea Millstipe", "Karazhan"}, //MOROES ADD
	        //{"Lady Catriona Von'Indi", "Karazhan"}, //MOROES ADD
	        //{"Lady Keira Berrybuck", "Karazhan"}, //MOROES ADD
	        //{"Baron Rafe Dreuger", "Karazhan"}, //MOROES ADD
	        //{"Lord Robin Daris", "Karazhan"}, //MOROES ADD
	        //{"Lord Crispin Ference", "Karazhan"}, //MOROES ADD
	        //Moroes
	        {"Maiden of Virtue", "Karazhan"},
	        //The Opera Event
	        {"The Big Bad Wolf", "Karazhan"},
	        //Romulo+Julianne
	        {"Romulo and Julianne", "Karazhan"},
	        //{"Romulo", "Karazhan"},
	        //{"Julianne", "Karazhan"},
	        //Romulo+Julianne
	        //The Crone
	        {"Wizard of Oz", "Karazhan"},
            //{"The Crone", "Karazhan"},
            //{"Dorothee", "Karazhan"},
            //{"Tito", "Karazhan"},
            //{"Roar", "Karazhan"},
            //{"Strawman", "Karazhan"},
            //{"Tinhead", "Karazhan"},
	        //The Crone
	        //The Opera Event
	        //The Curator
	        {"The Curator", "Karazhan"},
	        //{"Astral Flare", "VF_RS_MobType_Boss"}, //CURATOR ADD
	        //The Curator
	        {"Shade of Aran", "Karazhan"},
	        //Terestian Illhoof
	        {"Terestian Illhoof", "Karazhan"},
	        //{"Kil'rek", "Karazhan"}, //Illhoof ADD
	        //{"Demon Chains", "VF_RS_MobType_Boss"}, //Illhoof ADD
	        //Terestian Illhoof
	        {"Netherspite", "Karazhan"},
	        //Nightbane
	        {"Nightbane", "Karazhan"},
	        //{"Restless Skeleton", "VF_RS_MobType_Boss"}, //NIGHTBANE ADD
	        //Nightbane
	        {"Prince Malchezaar", "Karazhan"},
	
	        //Gruul's Lair
	        //High King Maulgar
	        {"High King Maulgar", "Gruul's Lair"},
	        //{"Krosh Firehand", "VF_RS_MobType_Boss"}, //High King Maulgar ADD
	        //{"Olm the Summoner", "VF_RS_MobType_Boss"}, //High King Maulgar ADD
	        //{"Kiggler the Crazed", "VF_RS_MobType_Boss"}, //High King Maulgar ADD
	        //{"Blindeye the Seer", "VF_RS_MobType_Boss"}, //High King Maulgar ADD
	        //High King Maulgar
	        {"Gruul the Dragonkiller", "Gruul's Lair"},
	
	        //Magtheridon's Lair
	        {"Magtheridon", "Magtheridon's Lair"},
	        //{"Hellfire Channeler", "VF_RS_MobType_Boss"}, //MAGTHERIDON ADD
	
	        //Serpentshrine Cavern
	        {"Hydross the Unstable", "Serpentshrine Cavern"},
	        {"The Lurker Below", "Serpentshrine Cavern"},
	        //Leotheras the Blind
	        {"Leotheras the Blind", "Serpentshrine Cavern"},
	        //{"Shadow of Leotheras", "VF_RS_MobType_Boss"},
	        //Leotheras the Blind
	        //Fathom-Lord Karathress
	        {"Fathom-Lord Karathress", "Serpentshrine Cavern"},
            //{"Fathom-Guard Sharkkis", "Serpentshrine Cavern"}, //ADD
            //{"Fathom-Guard Tidalvess", "Serpentshrine Cavern"}, //ADD
            //{"Fathom-Guard Caribdis", "Serpentshrine Cavern"}, //ADD
	        //Fathom-Lord Karathress
	        {"Morogrim Tidewalker", "Serpentshrine Cavern"},
	        //Lady Vashj
	        {"Lady Vashj", "Serpentshrine Cavern"},
	        //{"Enchanted Elemental", "VF_RS_MobType_Boss"}, //ADD
	        //{"Tainted Elemental", "VF_RS_MobType_Boss"}, //ADD
	        //{"Coilfang Elite", "VF_RS_MobType_Boss"}, //ADD
	        //{"Coilfang Strider", "VF_RS_MobType_Boss"}, //ADD
	        //{"Toxic Spore Bat", "VF_RS_MobType_Boss"}, //ADD
	        //Lady Vashj
	
	        //Tempest Keep
	        //Al'ar
	        {"Al'ar", "Tempest Keep"},
	        //{"Ember of Al'ar", "Tempest Keep"}, //ADD
	        //Al'ar
	        {"Void Reaver", "Tempest Keep"},
	        //High Astromancer Solarian
	        {"High Astromancer Solarian", "Tempest Keep"}, 
	        //{"Solarium Agent", "VF_RS_MobType_Boss"}, //ADD
	        //{"Solarium Priest", "VF_RS_MobType_Boss"}, //ADD
	        //High Astromancer Solarian
	        //Kael'thas Sunstrider
	        {"Kael'thas Sunstrider", "Tempest Keep"},
            //{"Thaladred the Darkener", "Tempest Keep"}, //ADD
            //{"Lord Sanguinar", "Tempest Keep"}, //ADD
            //{"Grand Astromancer Capernian", "Tempest Keep"}, //ADD
            //{"Master Engineer Telonicus", "Tempest Keep"}, //ADD
            //{"Netherstrand Longbow", "Tempest Keep"}, //WEAPON ADD
            //{"Staff of Disintegration", "Tempest Keep"}, //WEAPON ADD
            //{"Cosmic Infuser", "Tempest Keep"}, //WEAPON ADD
            //{"Infinity Blade", "Tempest Keep"}, //WEAPON ADD
            //{"Warp Slicer", "Tempest Keep"}, //WEAPON ADD
            //{"Devastation", "Tempest Keep"}, //WEAPON ADD
            //{"Phaseshift Bulwark", "Tempest Keep"}, //WEAPON ADD
            //{"Phoenix", "Tempest Keep"}, //ADD
            //{"Phoenix Egg", "Tempest Keep"}, //ADD
	        //Kael'thas Sunstrider
	
	        //Black Temple
	        {"High Warlord Naj'entus", "Black Temple"},
	        {"Supremus", "Black Temple"},
	        //Shade of Akama
	        {"Shade of Akama", "Black Temple"},
	        //{"Ashtongue Defender", "VF_RS_MobType_Boss"}, //ADD
	        //{"Ashtongue Elementalist", "VF_RS_MobType_Boss"}, //ADD
	        //{"Ashtongue Rogue", "VF_RS_MobType_Boss"}, //ADD
	        //{"Ashtongue Spiritbinder", "VF_RS_MobType_Boss"}, //ADD
	        //{"Ashtongue Channeler", "VF_RS_MobType_Boss"}, //ADD
	        //{"Ashtongue Sorcerer", "VF_RS_MobType_Boss"}, //ADD
	        //Shade of Akama
	        //Teron Gorefiend
	        {"Teron Gorefiend", "Black Temple"},
	        //{"Shadowy Construct", "VF_RS_MobType_Boss"}, //ADD
	        //Teron Gorefiend
	        {"Gurtogg Bloodboil", "Black Temple"},
	        //Reliquary of Souls
	        {"Reliquary of Souls", "Black Temple"},
            //{"Essence of Anger", "Black Temple"}, //BOSSPART
            //{"Essence of Desire", "Black Temple"}, //BOSSPART
            //{"Essence of Suffering", "Black Temple"}, //BOSSPART
	        //{"Enslaved Soul", "VF_RS_MobType_Boss"}, //ADD
	        //Reliquary of Souls
	        {"Mother Shahraz", "Black Temple"},
	        //Illidari Council
	        {"Illidari Council", "Black Temple"},
            //{"Gathios the Shatterer", "Black Temple"}, //BOSSPART
            //{"High Nethermancer Zerevor", "Black Temple"}, //BOSSPART
            //{"Lady Malande", "Black Temple"}, //BOSSPART
            //{"Veras Darkshadow", "Black Temple"}, //BOSSPART
	        //Illidari Council
	        {"Illidan Stormrage", "Black Temple"},

	        //Mount Hyjal
	        {"Rage Winterchill", "Hyjal Summit"},
	        {"Anetheron", "Hyjal Summit"},
	        {"Kaz'rogal", "Hyjal Summit"},
	        {"Azgalor", "Hyjal Summit"},
	        {"Archimonde", "Hyjal Summit"},
        };
        #endregion BossFights
        #region BossParts
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
        

            ///////////////////////Dungeons///////////////////////
            {"The Three Dwarfs", new string[]{"Olaf", "Eric \"The Swift\"", "Baelog"}},
            {"Jammal'an", new string[]{"Jammal'an the Prophet", "Ogom the Wretched"}},
            {"The Seven Event", new string[]{"Anger'rel", "Seeth'rel", "Dope'rel", "Gloom'rel", "Vile'rel", "Hate'rel", "Doom'rel"}},
            {"Illyanna And Ferra", new string[]{"Illyanna Ravenoak", "Ferra"}},
            {"Lethtendris And Pimgib", new string[]{"Lethtendris", "Pimgib"}},
            {"Vectus And Marduk", new string[]{"Marduk Blackpool", "Vectus"}},

            ///////////////////////TBC///////////////////////
            {"Romulo and Julianne", new string[]{"Romulo", "Julianne"}},
	        {"Wizard of Oz", new string[]{"Dorothee", "Tito", "Roar", "Strawman", "Tinhead", "The Crone"}},
	
	        {"Reliquary of Souls", new string[]{"Essence of Anger", "Essence of Desire", "Essence of Suffering"}},
	        {"Illidari Council", new string[]{"Gathios the Shatterer", "High Nethermancer Zerevor", "Lady Malande", "Veras Darkshadow"}},
        };
        #endregion BossParts
        #region BossAdds
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
        
            /////////////////////Dungeons////////////////////
            {"King Gordok", new string[]{"Cho'Rush the Observer"}},
            {"Balnazzar", new string[]{"Grand Crusader Dathrohan"}},
            {"Warchief Rend Blackhand", new string[]{"Gyth"}},

            ///////////////////////TBC///////////////////////
            
            //Karazhan
            {"Attumen the Huntsman", new string[]{"Midnight"}},
            {"Moroes", new string[]{"Baroness Dorothea Millstipe", "Lady Catriona Von'Indi", "Lady Keira Berrybuck", "Baron Rafe Dreuger", "Lord Robin Daris", "Lord Crispin Ference"}},
            {"Romulo and Julianne", new string[]{"Romulo", "Julianne"}},
	        {"Wizard of Oz", new string[]{"Dorothee", "Tito", "Roar", "Strawman", "Tinhead", "The Crone"}},
            {"The Curator", new string[]{"Astral Flare"}},
            {"Terestian Illhoof", new string[]{"Kil'rek", "Demon Chains"}},
            {"Nightbane", new string[]{"Restless Skeleton"}},
            
            //Gruul's Lair
	        {"High King Maulgar", new string[]{"Krosh Firehand", "Olm the Summoner", "Kiggler the Crazed", "Blindeye the Seer"}},
            
            //Magtheridon's Lair
            {"Magtheridon", new string[]{"Hellfire Channeler"}},

            //Serpentshrine Cavern
            {"The Lurker Below", new string[]{"Coilfang Guardian", "Coilfang Ambusher"}},
            {"Leotheras the Blind", new string[]{"Shadow of Leotheras"}},
            {"Fathom-Lord Karathress", new string[]{"Fathom-Guard Sharkkis", "Fathom-Guard Tidalvess", "Fathom-Guard Caribdis"}},
            {"Lady Vashj", new string[]{"Enchanted Elemental", "Tainted Elemental", "Coilfang Elite", "Coilfang Strider", "Toxic Spore Bat"}},

            //Tempest Keep
            {"Al'ar", new string[]{"Ember of Al'ar"}},
            {"High Astromancer Solarian", new string[]{"Solarium Agent", "Solarium Priest"}},
            {"Kael'thas Sunstrider", new string[]{"Thaladred the Darkener", "Lord Sanguinar"
                , "Grand Astromancer Capernian", "Master Engineer Telonicus", "Netherstrand Longbow"
                , "Staff of Disintegration", "Cosmic Infuser", "Infinity Blade"
                , "Warp Slicer", "Devastation", "Phaseshift Bulwark", "Phoenix", "Phoenix Egg"}},

            //Black Temple
            {"Shade of Akama", new string[]{"Ashtongue Defender" , "Ashtongue Elementalist", "Ashtongue Rogue"
                , "Ashtongue Spiritbinder", "Ashtongue Channeler", "Ashtongue Sorcerer"}},
            {"Teron Gorefiend", new string[]{"Shadowy Construct"}},

            {"Reliquary of Souls", new string[]{"Essence of Anger", "Essence of Desire", "Essence of Suffering", "Enslaved Soul"}},
	        /*
                was it wipe on Reliquary of Souls?
                [20:47:25] Kevin Heidemann: nioe
                [20:47:27] Kevin Heidemann: nope*
                [20:47:31] Kevin Heidemann: It's three phases of 1 boss
                [20:47:39] Kevin Heidemann: On WoL it counted as wipe as well^^
                [20:48:36] Viktor Friberg: hmm kk. The Death of Essence Of Desire is never registered it seems. Says it has 465k hp left. Maybe some way the server is coded?
                [20:48:52] Viktor Friberg: maybe you have some idea?
                [21:06:39] Kevin Heidemann: Probably, would have to check into it
                [21:08:27] Viktor Friberg: how does the essences die during the fight? Is there anything like "at 20% the boss merges with another one" or something similar special mechanic?
                [21:08:43] Viktor Friberg: cant remember the fight
                [21:53:25] Kevin Heidemann: Oh yeah that's it
                [21:53:44] Kevin Heidemann: at 10% it moves back, spawns add and then comes as next phase after adds down
                [21:54:18] Viktor Friberg: ahh that explains it
             */ 
             

            {"Illidari Council", new string[]{"Gathios the Shatterer", "High Nethermancer Zerevor", "Lady Malande", "Veras Darkshadow"}},
            {"Illidan Stormrage", new string[]{"Flame of Azzinoth", "Parasitic Shadowfiend", "Shadow Demon"}},
               
            //Mount Hyjal
            {"Anetheron", new string[]{"Towering Infernal"}},
            {"Azgalor", new string[]{"Lesser Doomguard"}},
        };
        #endregion BossAdds
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
