using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF
{
    public class ItemTranslations
    {
        public static int FindItemID(string _Name)
        {
            int itemID = 0;
            if(_NameToIDTranslation.TryGetValue(_Name, out itemID) == true)
            {
                return itemID;
            }
            return -1;
        }

        const int SPELL_MODIFIER = 0x1000000;
        const int NPC_MODIFIER = 0x2000000;
        public static Dictionary<string, int> _NameToIDTranslation = new Dictionary<string, int>
        {
            //Orc mounts
            {"Arctic Wolf", 12351},
            {"Red Wolf", 12330},

            {"Brown Wolf", 5668},
            {"Dire Wolf", 5665},
            {"Timber Wolf", 1132},
            {"Large Timber Wolf", 1132},

            {"Swift Timber Wolf", 18797},
            {"Swift Gray Wolf", 18798},
            {"Swift Brown Wolf", 18796},
            
            //Tauren mounts
            {"Brown Kodo", 15290},
            {"Gray Kodo", 15277},

            {"Great Gray Kodo", 18795},
            {"Great Brown Kodo", 18794},
            {"Great White Kodo", 18793},

            {"Green Kodo", 15292},
            {"Teal Kodo", 15293},
            
            //Troll mounts
            {"Emerald Raptor", 8588},
            {"Turquoise Raptor", 8591},
            {"Violet Raptor", 8592},

            {"Swift Blue Raptor", 18788},
            {"Swift Olive Raptor", 18789},
            {"Swift Orange Raptor", 18790},

            {"Ivory Raptor", 13317},
            {"Mottled Red Raptor", 8586},
            
            //Undead mounts
            {"Blue Skeletal Horse", 13332},
            {"Brown Skeletal Horse", 13333},
            {"Red Skeletal Horse", 13331},

            {"Purple Skeletal Warhorse", 18791},
            {"Green Skeletal Warhorse", 13334},
            
            //Nightelf mounts
            {"Spotted Frostsaber", 8632},
            //{"Spotted Nightsaber", 8628},
            {"Spotted Panther", 8628},
            {"Striped Frostsaber", 8631},
            {"Striped Nightsaber", 8629},
            {"Tawny Sabercat", SPELL_MODIFIER + 16059},
            
            {"Frostsaber", 12302},
            {"Nightsaber", 12303},

            {"Swift Frostsaber", 8632},
            {"Swift Stormsaber", 18902},
            {"Swift Mistsaber", 18767},
            {"Swift Dawnsaber", SPELL_MODIFIER + 23220},
            
            //Dwarf mounts
            {"Brown Ram", 5872},
            {"Gray Ram", 5864},
            {"White Ram", 5873},

            {"Black Ram", 13328},
            {"Frost Ram", 13329},
            
            {"Swift Brown Ram", 18786},
            {"Swift Gray Ram", 18787},
            {"Swift White Ram", 18785},
            
            //Gnome mounts
            {"Red Mechanostrider", 8563},
            {"Green Mechanostrider", 13321},
            {"Blue Mechanostrider", 8595},
            {"Unpainted Mechanostrider", 13322},
            
            {"Icy Blue Mechanostrider", 13327},
            {"White Mechanostrider", 13326},

            {"Swift Green Mechanostrider", 18772},
            {"Swift White Mechanostrider", 18773},
            {"Swift Yellow Mechanostrider", 18774},
            
            //Human mounts
            {"Black Stallion", 2411},
            {"Brown Horse", 5656},
            {"Chestnut Mare", 5655},
            {"Pinto Horse", 2414},
            
            //{"Swift White Stallion", 12353},
            {"White Stallion", 12353},

            {"Swift White Steed", 18778},
            {"Swift Brown Steed", 18777},
            {"Swift Palomino", 18776},
            {"Palomino Stallion", 12354},
            
            //Rare drop mounts
            {"Winterspring Frostsaber", 13086},
            {"Deathcharger", 13335},
            {"Swift Spectral Tiger", 33225},

            //AV mounts
            {"Stormpike Battle Charger", 19030},
            {"Frostwolf Howler", 19029},

            //Rank11 mounts
            {"Black War Kodo", 18247},
            {"Black War Raptor", 18246},
            {"Black War Wolf", 18245},
            {"Red Skeletal Warhorse", 18248},

            {"Black War Steed", 18241},
            {"Black War Tiger", 18242},
            {"Black War Ram", 18244},
            {"Black Battlestrider", 18243},
            
            //AQ40 mounts
            {"Summon Red Qiraji Battle Tank", 21321},
            {"Summon Yellow Qiraji Battle Tank", 21324},
            {"Summon Green Qiraji Battle Tank", 21323},
            {"Summon Blue Qiraji Battle Tank", 21218},
            {"Summon Black Qiraji Battle Tank", 21176},
            
            //ZG mounts
            {"Swift Razzashi Raptor", 19872},
            {"Swift Zulian Tiger", 19902},
            
            //Occassional mounts
            {"Reindeer", 21211},

            
            //Warlock mounts
            {"Summon Felsteed", SPELL_MODIFIER + 5784},
            {"Summon Dreadsteed", SPELL_MODIFIER + 23161},
            
            //Paladin mounts
            {"Summon Warhorse", SPELL_MODIFIER + 13819},
            {"Summon Charger", SPELL_MODIFIER + 23214},
            

            {"Mr. Wiggles", 23720},

            
            /*---------------------------------TBC MOUNTS---------------------------------*/
            
            {"Riding Turtle", 23720},
            {"Fiery Warhorse", 30480},
            {"Big Battle Bear", 38576},
            {"Swift Warstrider", 34129},
            {"Flying Reindeer", SPELL_MODIFIER + 44824},
            {"Raven Lord", 32768},
            {"Call of the Phoenix", 32458},

            //PVP mounts
            {"Merciless Nether Drake", 34092},
            {"Swift Nether Drake", 30609},
            {"Vengeful Nether Drake", 37676},
            {"Brutal Nether Drake", 43516},
            
            //Engineering mounts
            {"Turbo-Charged Flying Machine", 34061},
            {"X-51 Nether-Rocket X-TREME", 35226},
            {"X-51 Nether-Rocket", 35225},

            //Netherwing Drakes mounts
            {"Azure Netherwing Drake", 32858},
            {"Cobalt Netherwing Drake", 32859},
            {"Onyx Netherwing Drake", 32857},
            {"Purple Netherwing Drake", 32860},
            {"Veridian Netherwing Drake", 32861},
            {"Violet Netherwing Drake", 32862},
            
            //Nether Ray mounts
            {"Blue Riding Nether Ray", 32319},
            {"Green Riding Nether Ray", 32314},
            {"Purple Riding Nether Ray", 32316},
            {"Red Riding Nether Ray", 32317},
            {"Silver Riding Nether Ray", 32318},
            //{"Nether Ray Fry", 38628},

            //Exalted mounts
            {"Cenarion War Hippogryph", 33999},

            //Talbuk mounts
            {"Cobalt War Talbuk", 29102},
            {"Dark War Talbuk", 29228},
            {"Silver War Talbuk", 29104},
            {"Tan War Talbuk", 29105},
            {"White War Talbuk", 29103},
            
            {"Cobalt Riding Talbuk", 31829},
            {"Dark Riding Talbuk", 28915},
            {"Silver Riding Talbuk", 31831},
            {"Tan Riding Talbuk", 31833},
            {"White Riding Talbuk", 31835},

            //Bloodelf mounts
            {"Black Hawkstrider", 29221},
            {"Blue Hawkstrider", 29220},
            {"Purple Hawkstrider", 29222},
            {"Red Hawkstrider", 28927},

            {"Swift Green Hawkstrider", 29223},
            {"Swift Pink Hawkstrider", 28936},
            {"Swift Purple Hawkstrider", 29224},
            {"Swift White Hawkstrider", 35513},

            //Alliance new race mounts
            {"Brown Elekk", 28481},
            {"Gray Elekk", 29744},
            {"Purple Elekk", 29743},

            {"Great Blue Elekk", 29745},
            {"Great Green Elekk", 29746},
            {"Great Purple Elekk", 29747},
            {"Black War Elekk", 35906},

            //Horde mounts
            {"Blue Windrider", 25475},
            {"Green Windrider", 25476},
            {"Tawny Windrider", 25474},

            {"Swift Green Windrider", 25531},
            {"Swift Purple Windrider", 25533},
            {"Swift Red Windrider", 25477},
            {"Swift Yellow Windrider", 25532},
            
            //Alliance mounts
            {"Ebon Gryphon", 25471},
            {"Golden Gryphon", 25470},
            {"Snowy Gryphon", 25472},

            {"Swift Blue Gryphon", 25473},
            {"Swift Green Gryphon", 25528},
            {"Swift Red Gryphon", 25527},
            {"Swift Purple Gryphon", 25529},

            //{"ww", 0},
            //{"ww", 0},
            //{"ww", 0},
            //{"ww", 0},
            //{"ww", 0},
            //{"ww", 0},
            //{"ww", 0},
            //{"ww", 0},
            //{"ww", 0},
            //{"ww", 0},
            //{"ww", 0},







            /*---------------------------------COMPANIONS---------------------------------*/
            
            {"Jubling", 19450},
            {"Snowshoe Rabbit", 8497},
            {"Cockatiel", 8496},
            {"Disgusting Oozeling", 20769},

            {"White Kitten", 8489},
            {"Bombay Cat", 8485},
            {"Bombay", 8485}, //Needed for Archangel version
            {"Siamese Cat", 8490},
            {"Cornish Rex Cat", 8486},

            {"Black Tabby Cat", 8491},
            {"Silver Tabby Cat", 8488},
            {"Orange Tabby Cat", 8487},

            {"Great Horned Owl", 8500},
            {"Green Wing Macaw", 8492},
            {"Hyacinth Macaw", 8494},
            {"Senegal", 8495},

            {"Dark Whelpling", 10822},
            {"Crimson Whelpling", 8499},
            {"Emerald Whelpling", 8498},
            {"Azure Whelpling", 34535},
            
            {"Ancona Chicken", 11023},
            
            {"Sprite Darter Hatchling", 11474},
            {"Prairie Chicken", 11110},
            {"Tranquil Mechanical Yeti", 21277},
            
            {"Smolderweb Hatchling", 12529},
            {"Whiskers the Rat", 23015},

            {"Pet Bombling", 11825},

            //Occassional pets
            {"Human Orphan", 18598},
            {"Orcish Orphan", 18597},

            
            {"Cockroach", 10393},
            {"Speedy", 23002},
            {"Gurky", 22114},
            {"Murky", 20371},
            {"Worg Pup", 12263},
            {"Hawk Owl", 8501},

            
            {"White Tiger Cub", SPELL_MODIFIER + 30152},

            {"Zergling", 13582},
            {"Crimson Snake", 10392},
            {"Brown Snake", 10361},
            {"Panda Cub", SPELL_MODIFIER + 26972},
            {"Wood Frog", 11027},
            {"Blood Parrot", SPELL_MODIFIER + 17567},
            {"Brown Prairie Dog", 10394},
            {"Lil' Smoky", 11826},
            {"Lil&apos; Smoky", 11826},
            {"Hippogryph Hatchling", 23713},
            {"Tree Frog", 11026},
            {"Firefly", 29960},
            {"Tiny Sporebat", 34478},
            {"Brown Rabbit", 29364},
            {"Muckbreath", 33818},

            {"Mechanical Chicken", 10398},
            {"Mechanical Squirrel", 4401},
            {"Lifelike Toad", 15996},
            {"Black Kingsnake", 10360},
            {"Bananas", 32588},
            {"Horde Battle Standard", 18607},
            {"Alliance Battle Standard", 18606},
            {"Mana Wyrmling", 29363},
            //{"ww", 0},
            
            /*
            {"Umi's Mechanical Yeti", 12928},
            {"Fel Guard Hound", 30803},
             */
        };
    }
}
