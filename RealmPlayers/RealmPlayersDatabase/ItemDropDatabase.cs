using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace VF_RealmPlayersDatabase
{
    [ProtoContract]
    [Serializable]
    public class ItemDropDataItem : ISerializable
    {
        [ProtoMember(1)]
        public WowBoss m_Boss;
        [ProtoMember(2)]
        public float m_DropChance;

        public ItemDropDataItem()
        { }
        public ItemDropDataItem(WowBoss _Boss, float _DropChance)
        {
            m_Boss = _Boss;
            m_DropChance = _DropChance;
        }
        #region Serializing
        public ItemDropDataItem(SerializationInfo _Info, StreamingContext _Context)
        {
            m_Boss = (WowBoss)_Info.GetInt32("m_Boss");
            m_DropChance = _Info.GetSingle("m_DropChance");
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("m_Boss", (int)m_Boss);
            _Info.AddValue("m_DropChance", m_DropChance);
        }
        #endregion
    }
    public class ItemDropDatabase
    {
        private static string[] sm_rawDBFiles = new string[] { "AL_Dungeons.lua", "AL_WorldBosses.lua", "AL_ItemSets.lua", "AL_BG.lua" };
        private static string[] sm_rawDBSections = new string[] { "AtlasLootItems = ", "AtlasLootSetItems = ", "AtlasLootWBItems = ", "AtlasLootBGItems = " };

        private Dictionary<int, List<ItemDropDataItem>> m_Database = null;
        public Dictionary<int, List<ItemDropDataItem>> GetDatabase()
        {
            return m_Database;
        }

        public static bool DatabaseExists(string _RootPath)
        {
            return System.IO.File.Exists(_RootPath + "\\ItemDropDatabase.dat");
        }
        public ItemDropDatabase(string _RootPath)
        {
            try
            {
                if(System.IO.File.Exists(_RootPath + "\\ItemDropDatabase.dat") == true)
                {
                    Utility.LoadSerialize(_RootPath + "\\ItemDropDatabase.dat", out m_Database);
                }
                else
                {
                    m_Database = new Dictionary<int, List<ItemDropDataItem>>();
                    foreach (string rawDBFile in sm_rawDBFiles)
                    {
                        if (System.IO.File.Exists(_RootPath + "\\ItemDropRawData\\" + rawDBFile) == true)
                        {
                            try
                            {
                                string atlasLootItems;
                                {
                                    var allText = System.IO.File.ReadAllText(_RootPath + "\\ItemDropRawData\\" + rawDBFile);
                                    var splittedText = allText.Split(sm_rawDBSections, StringSplitOptions.None);
                                    atlasLootItems = splittedText[1];
                                }
                                var instancesData = atlasLootItems.Split(';');
                                foreach (var instanceData in instancesData)
                                {
                                    var bossNameAndData = instanceData.Split(new string[] { " = {", "}," }, StringSplitOptions.None);

                                    var bossName = bossNameAndData[0].Split(' ', '\t').Last();

                                    WowBoss wowBoss;
                                    if (_WowBossConvert.TryGetValue(bossName, out wowBoss) == false)
                                    {
                                        if (bossName.StartsWith("PVP"))
                                        {
                                            if (Enum.TryParse(bossName.Replace("PVP", "PVPEpic"), out wowBoss) == false)
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            //Not interesting, move on
                                            continue;
                                        }
                                    }
                                    try
                                    {
                                        for (int i = 1; i < bossNameAndData.Length - 1; ++i)
                                        {
                                            var data = bossNameAndData[i].Split('{', ',', '}');
                                            //data[0] == tabtabspaces (example: \t\t) 
                                            //data[1] == itemID (example:  28329)
                                            //data[2] == texture (example: "INV_Bracer_09")
                                            //data[3 to n] == quality and name (example: "=q4=Netherwind Bindings")
                                            //data[n+1 to m] == crap (example: "=q1=#m1# =ds=#c3#")
                                            //data[m+1] == droprate (example: "11.31%")

                                            int itemID = 0;
                                            if (int.TryParse(data[1], out itemID) == false) itemID = 0;

                                            if (itemID != 0)
                                            {
                                                int u = 2;
                                                string texture = RebuildSplittedSection(data, ref u, '\"', ",");
                                                string qualityAndName = RebuildSplittedSection(data, ref u, '\"', ",");
                                                string extraInfo = RebuildSplittedSection(data, ref u, '\"', ",");
                                                //if (bossName.StartsWith("T3"))
                                                //{
                                                //    //string[] extraInfoSplit = extraInfo.Split(new string[]{".."}, StringSplitOptions.None);
                                                //    //foreach (string currInfo in extraInfoSplit)
                                                //    //{
                                                //    //    try
                                                //    //    {
                                                //    //        if (currInfo.StartsWith("AtlasLootBossNames[\"Naxxramas\"]["))
                                                //    //        {
                                                //    //            string bossIndex = currInfo.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries)[2];

                                                //    //        }
                                                //    //    }
                                                //    //    catch (Exception)
                                                //    //    {
                                                            
                                                //    //        throw;
                                                //    //    }
                                                //    //}
                                                //}
                                                string dropRateStr = RebuildSplittedSection(data, ref u, '\"', ",");
                                                float dropRate;
                                                if (float.TryParse(dropRateStr.Replace("%", "").Replace('.',','), out dropRate) == false) dropRate = 0.0f;

                                                if (m_Database.ContainsKey(itemID) == false)
                                                    m_Database.Add(itemID, new List<ItemDropDataItem>(2));
                                                m_Database[itemID].Add(new ItemDropDataItem(wowBoss, dropRate));

                                                WowBoss extraWowBoss = wowBoss;
                                                if (bossName.StartsWith("T3"))
                                                {
                                                    if (texture.StartsWith("INV_Crown") || texture.StartsWith("INV_Helmet"))
                                                        extraWowBoss = WowBoss.Thaddius;
                                                    else if (texture.StartsWith("INV_Gauntlets"))
                                                        extraWowBoss = WowBoss.Maexxna;
                                                    else if (texture.StartsWith("INV_Pants"))
                                                        extraWowBoss = WowBoss.Loatheb;
                                                    else if (texture.StartsWith("INV_Chest"))
                                                        extraWowBoss = WowBoss.The_Four_Horsemen;
                                                    else if (texture.StartsWith("INV_Jewelry_Ring"))
                                                        extraWowBoss = WowBoss.Kel_Thuzad;
                                                }
                                                else if (bossName.StartsWith("AQ40"))
                                                {
                                                    if (texture.StartsWith("INV_Helmet"))
                                                        extraWowBoss = WowBoss.The_Twin_Emperors;
                                                    else if (texture.StartsWith("INV_Chest"))
                                                        extraWowBoss = WowBoss.C_Thun;
                                                    else if (texture.StartsWith("INV_Pants"))
                                                        extraWowBoss = WowBoss.Ouro;
                                                }

                                                if (extraWowBoss != wowBoss)
                                                {
                                                    m_Database[itemID].Add(new ItemDropDataItem(extraWowBoss, 0.0f));
                                                }
                                            }
                                            else
                                            {
                                                int u = 2;
                                                string texture = RebuildSplittedSection(data, ref u, '\"', ",");
                                                if (texture != "")
                                                {
                                                    if (bossName.StartsWith("PVP"))
                                                    {
                                                        string qualityAndName = RebuildSplittedSection(data, ref u, '\"', ",");
                                                        string crap = RebuildSplittedSection(data, ref u, '\"', ",");

                                                        if (crap == "=q5=#pvps1#")
                                                        {
                                                            WowBoss newWowBoss;
                                                            if (Enum.TryParse(bossName.Replace("PVP", "PVPEpic"), out newWowBoss) == true)
                                                                wowBoss = newWowBoss;
                                                        }
                                                        else if (crap == "=q5=#pvps2#")
                                                        {
                                                            WowBoss newWowBoss;
                                                            if (Enum.TryParse(bossName.Replace("PVP", "PVPRare"), out newWowBoss) == true)
                                                                wowBoss = newWowBoss;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogException(ex);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogException(ex);
                            }
                        }
                    }
                    Utility.SaveSerialize(_RootPath + "\\ItemDropDatabase.dat", m_Database);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            if (m_Database == null)
                m_Database = new Dictionary<int, List<ItemDropDataItem>>();
        }
        string RebuildSplittedSection(string[] _StringArray, ref int _StringArrayOffset, char _SectionChar, string _SplitDelimiter)
        {
            int sectionCharCount = 0;
            string builtString = "";
            int u = _StringArrayOffset;
            while (u < _StringArray.Length)
            {
                sectionCharCount += _StringArray[u].Count((_Char) => { return _Char == _SectionChar; });
                if (sectionCharCount > 2)
                {
                    builtString += GetSectionString(_StringArray[u], _SectionChar, false);
                    ++u;
                    break;
                    //throw new Exception("SectionCharCount was larger than 2");
                }
                else if (sectionCharCount == 2)
                {
                    builtString += GetSectionString(_StringArray[u], _SectionChar, false);
                    ++u;
                    break;
                }
                else
                {
                    builtString += GetSectionString(_StringArray[u], _SectionChar, true) + _SplitDelimiter;
                }
                ++u;
            }
            _StringArrayOffset = u;
            return builtString;
        }
        string GetSectionString(string _String, char _SectionChar, bool _PrioSecond = false)
        {
            int firstSnuff = _String.IndexOf(_SectionChar);
            int secondSnuff = _String.IndexOf(_SectionChar, firstSnuff + 1);
            if (firstSnuff != -1 && secondSnuff != -1)
            {
                return _String.Substring(firstSnuff + 1, secondSnuff - firstSnuff - 1);
            }
            else
            {
                if (_PrioSecond == false && firstSnuff != -1)
                {
                    secondSnuff = firstSnuff;
                    firstSnuff = -1;
                }
                if (firstSnuff != -1)
                {
                    return _String.Substring(firstSnuff + 1);
                }
                else if (secondSnuff != -1)
                {
                    return _String.Substring(0, secondSnuff);
                }
                else
                    return _String;
            }
        }
        private static Dictionary<string, WowBoss> _WowBossConvert = new Dictionary<string, WowBoss>
        {
            {"BWLRazorgore", WowBoss.Razorgore_The_Untamed},
            {"BWLVaelastrasz", WowBoss.Vaelstrasz_The_Corrupt},
            {"BWLLashlayer", WowBoss.Broodlord_Lashlayer},
            {"BWLFiremaw", WowBoss.Firemaw},
            {"BWLEbonroc", WowBoss.Ebonroc},
            {"BWLFlamegor", WowBoss.Flamegor},
            {"BWLChromaggus", WowBoss.Chromaggus},
            {"BWLNefarian", WowBoss.Nefarian},
            {"BWLTrashMobs", WowBoss.BWLTrashMobs},
            
            {"MCLucifron", WowBoss.Lucifron},
            {"MCMagmadar", WowBoss.Magmadar},
            {"MCGehennas", WowBoss.Gehennas},
            {"MCGarr", WowBoss.Garr},
            {"MCShazzrah", WowBoss.Shazzrah},
            {"MCGeddon", WowBoss.Baron_Geddon},
            {"MCGolemagg", WowBoss.Golemagg_The_Incinerator},
            {"MCSulfuron", WowBoss.Sulfuron_Harbinger},
            {"MCMajordomo", WowBoss.Majordomo_Executus},
            {"MCRagnaros", WowBoss.Ragnaros},
            {"MCTrashMobs", WowBoss.MCTrashMobs},
            {"MCRANDOMBOSSDROPPS", WowBoss.MCRandomBoss},
            
            {"Onyxia", WowBoss.Onyxia},
            
            {"ZGJeklik", WowBoss.High_Priestess_Jeklik},
            {"ZGVenoxis", WowBoss.High_Priest_Venoxis},
            {"ZGMarli", WowBoss.High_Priestess_Mar_Li},
            {"ZGMandokir", WowBoss.Broodlord_Mandokir},
            {"ZGGrilek", WowBoss.Gri_Lek_Of_The_Iron_Blood},
            {"ZGHazzarah", WowBoss.Hazzarah_The_Dreamweaver},
            {"ZGRenataki", WowBoss.Renataki_Of_The_Thousand_Blades},
            {"ZGWushoolay", WowBoss.Wushoolay_the_Storm_Witch},
            {"ZGGahzranka", WowBoss.Gahz_Ranka},
            {"ZGThekal", WowBoss.High_Priest_Thekal},
            {"ZGArlokk", WowBoss.High_Priestess_Arlokk},
            {"ZGJindo", WowBoss.Jin_Do_The_Hexxer},
            {"ZGHakkar", WowBoss.Hakkar_The_Soulflayer},
            {"ZGShared", WowBoss.ZGShared},
            {"ZGTrash", WowBoss.ZGTrashMobs},
            
            {"AQ20Kurinnaxx", WowBoss.Kurinnaxx},
            {"AQ20CAPTIAN", WowBoss.Event_Captain},
            {"AQ20Rajaxx", WowBoss.General_Rajaxx},
            {"AQ20Moam", WowBoss.Moam},
            {"AQ20Buru", WowBoss.Buru_The_Gorger},
            {"AQ20Ayamiss", WowBoss.Ayamiss_The_Hunter},
            {"AQ20Ossirian", WowBoss.Ossirian_The_Unscarred},
            {"AQ20Andorov", WowBoss.Lieutenant_General_Andorov},
            
            {"AQ40Skeram", WowBoss.The_Prophet_Skeram},
            {"AQ40Vem", WowBoss.Three_Bugs},
            {"AQ40Sartura", WowBoss.Battleguard_Sartura},
            {"AQ40Fankriss", WowBoss.Fankriss_The_Unyielding},
            {"AQ40Viscidus", WowBoss.Viscidus},
            {"AQ40Huhuran", WowBoss.Huhuran},
            {"AQ40Emperors", WowBoss.The_Twin_Emperors},
            {"AQ40Ouro", WowBoss.Ouro},
            {"AQ40CThun", WowBoss.C_Thun},
            {"AQ40Trash", WowBoss.AQ40Trash},
            {"AQBroodRings", WowBoss.AQBroodRep},
            
            {"AQOpening", WowBoss.AQOpening},

            {"NAXPatchwerk", WowBoss.Patchwerk},
            {"NAXGrobbulus", WowBoss.Grobbulus},
            {"NAXGluth", WowBoss.Gluth},
            {"NAXThaddius", WowBoss.Flamegor},
            {"NAXAnubRekhan", WowBoss.Anub_Rekhan},
            {"NAXGrandWidowFaerlina", WowBoss.Grand_Widow_Faerlina},
            {"NAXMaexxna", WowBoss.Maexxna},
            {"NAXInstructorRazuvious", WowBoss.Instructor_Razuvious},
            {"NAXGothikderHarvester", WowBoss.Gothik_The_Harvester},
            {"NAXTheFourHorsemen", WowBoss.The_Four_Horsemen},
            {"NAXNothderPlaguebringer", WowBoss.Noth_The_Plaguebringer},
            {"NAXHeiganderUnclean", WowBoss.Heigan_The_Unclean},
            {"NAXLoatheb", WowBoss.Loatheb},
            {"NAXSapphiron", WowBoss.Sapphiron},
            {"NAXKelThuzard", WowBoss.Kel_Thuzad},
            {"NAXTrash", WowBoss.NAXTrashMobs},

            {"KKazzak", WowBoss.Kazzak},
            {"DTaerar", WowBoss.Taerar},
            {"DEmeriss", WowBoss.Emeriss},
            {"DLethon", WowBoss.Lethon},
            {"DYsondre", WowBoss.Ysondre},
            {"AAzuregos", WowBoss.Azuregos},
            
            {"T3Mage", WowBoss.T3Mage},
            {"T3Warlock", WowBoss.T3Warlock},
            {"T3Priest", WowBoss.T3Priest},
            {"T3Rogue", WowBoss.T3Rogue},
            {"T3Druid", WowBoss.T3Druid},
            {"T3Hunter", WowBoss.T3Hunter},
            {"T3Paladin", WowBoss.T3Paladin},
            {"T3Shaman", WowBoss.T3Shaman},
            {"T3Warrior", WowBoss.T3Warrior},
            
            {"T2Mage", WowBoss.T2Mage},
            {"T2Warlock", WowBoss.T2Warlock},
            {"T2Priest", WowBoss.T2Priest},
            {"T2Rogue", WowBoss.T2Rogue},
            {"T2Druid", WowBoss.T2Druid},
            {"T2Hunter", WowBoss.T2Hunter},
            {"T2Paladin", WowBoss.T2Paladin},
            {"T2Shaman", WowBoss.T2Shaman},
            {"T2Warrior", WowBoss.T2Warrior},
            
            {"T1Mage", WowBoss.T1Mage},
            {"T1Warlock", WowBoss.T1Warlock},
            {"T1Priest", WowBoss.T1Priest},
            {"T1Rogue", WowBoss.T1Rogue},
            {"T1Druid", WowBoss.T1Druid},
            {"T1Hunter", WowBoss.T1Hunter},
            {"T1Paladin", WowBoss.T1Paladin},
            {"T1Shaman", WowBoss.T1Shaman},
            {"T1Warrior", WowBoss.T1Warrior},
            
            {"ZGMage", WowBoss.ZGMage},
            {"ZGWarlock", WowBoss.ZGWarlock},
            {"ZGPriest", WowBoss.ZGPriest},
            {"ZGRogue", WowBoss.ZGRogue},
            {"ZGDruid", WowBoss.ZGDruid},
            {"ZGHunter", WowBoss.ZGHunter},
            {"ZGPaladin", WowBoss.ZGPaladin},
            {"ZGShaman", WowBoss.ZGShaman},
            {"ZGWarrior", WowBoss.ZGWarrior},
            
            {"AQ20Mage", WowBoss.AQ20Mage},
            {"AQ20Warlock", WowBoss.AQ20Warlock},
            {"AQ20Priest", WowBoss.AQ20Priest},
            {"AQ20Rogue", WowBoss.AQ20Rogue},
            {"AQ20Druid", WowBoss.AQ20Druid},
            {"AQ20Hunter", WowBoss.AQ20Hunter},
            {"AQ20Paladin", WowBoss.AQ20Paladin},
            {"AQ20Shaman", WowBoss.AQ20Shaman},
            {"AQ20Warrior", WowBoss.AQ20Warrior},
            
            {"AQ40Mage", WowBoss.AQ40Mage},
            {"AQ40Warlock", WowBoss.AQ40Warlock},
            {"AQ40Priest", WowBoss.AQ40Priest},
            {"AQ40Rogue", WowBoss.AQ40Rogue},
            {"AQ40Druid", WowBoss.AQ40Druid},
            {"AQ40Hunter", WowBoss.AQ40Hunter},
            {"AQ40Paladin", WowBoss.AQ40Paladin},
            {"AQ40Shaman", WowBoss.AQ40Shaman},
            {"AQ40Warrior", WowBoss.AQ40Warrior},

            {"PVPWeapons1", WowBoss.PVPWeapons},
            {"PVPWeapons2", WowBoss.PVPWeapons},

            {"Legendaries", WowBoss.Legendary},
            
            {"AVFriendly", WowBoss.AVFriendly},
            {"AVHonored", WowBoss.AVHonored},
            {"AVRevered", WowBoss.AVRevered},
            {"AVExalted", WowBoss.AVExalted},

            {"ABFriendly", WowBoss.ABFriendly},
            {"ABHonored", WowBoss.ABHonored},
            {"ABRevered", WowBoss.ABRevered},
            {"ABExalted", WowBoss.ABExalted},

            {"WSGFriendly", WowBoss.WSGFriendly},
            {"WSGHonored", WowBoss.WSGHonored},
            {"WSGRevered", WowBoss.WSGRevered},
            {"WSGExalted", WowBoss.WSGExalted},
        };
    }
}
