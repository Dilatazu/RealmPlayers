using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

using GearData = VF_RealmPlayersDatabase.PlayerData.GearData;

using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;
using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

namespace RealmPlayersServer
{
    [ProtoContract]
    [Serializable]
    public class ItemInfo : ISerializable
    {
        [ProtoMember(1)] 
        public string ItemID;
        [ProtoMember(2)]
        public string ItemIcon;
        [ProtoMember(3)]
        public string ItemName;
        [ProtoMember(4)]
        public int ItemQuality;//0==Poor(Grey), 1==Common(White), 2==Uncommon(Green), 3==Rare(Blue), 4==Epic(Purple), 5==Legendary(Orange), 6==Artifact(Red)
        [ProtoMember(5)]
        public int ItemModel;
        [ProtoMember(6)]
        public string AjaxTooltip;
        [ProtoMember(7)]
        public string ItemDataFetchedFrom;
        [ProtoMember(8)]
        public int ItemModelViewerID = 0;
        [ProtoMember(9)]
        public short ItemModelViewerSlot = 0;

        private int[] m_ItemSet = null;

        public string GetIconImageAddress(string sizeStr = "large")
        {
            if (ItemIcon == null)
            {
                Logger.ConsoleWriteLine("ItemIcon was null for Item: " + ItemID, ConsoleColor.Red);
                return "images/icons/" + sizeStr + "/inv_misc_questionmark.jpg";
            }
            else
                return "images/icons/" + sizeStr + "/" + ItemIcon.ToLower() + ".jpg";
        }
        //public string GetTooltipHtml(int _SuffixID, int _EnchantID)
        //{
        //    string specifiedToolTip = ItemTooltipHtml;
        //    if (_SuffixID != 0)
        //    {
        //        if (specifiedToolTip.Contains("Random Bonuses"))
        //        {
        //            specifiedToolTip = specifiedToolTip.Replace("Random Bonuses", "");
        //        }
        //    }
        //    if(_EnchantID != 0)
        //    {
        //        specifiedToolTip = specifiedToolTip.Replace("Durability", Code.EnchantIDs.GetValue(_EnchantID) + "<br />Durability");
        //    }
        //    return specifiedToolTip;
        //}
        public string GetAjaxTooltip(WowVersionEnum _WowVersion, int _ItemID, int _SuffixID, int _EnchantID, int _UniqueID, int[] _GemIDs = null, string _PiecesStr = "")
        {
            string specifiedAjaxResult = AjaxTooltip;
            if (_SuffixID != 0)
            {
                if (specifiedAjaxResult.Contains("Random Bonuses"))
                {
                    string suffixStr = Code.SuffixIDs.GetValue(_SuffixID, _UniqueID);
                    if (suffixStr.Count((char _Char) => { return _Char == '+'; }) > 1)
                        suffixStr = suffixStr.Replace(" +", "<br /><!---->+");
                    specifiedAjaxResult = specifiedAjaxResult.Replace("Random Bonuses", "<!---->" + suffixStr);
                    specifiedAjaxResult = specifiedAjaxResult.Replace(">" + ItemName + "<", ">" + ItemName + " " + Code.SuffixIDs.GetName(_SuffixID) + "<");
                }
            }
            if (_EnchantID != 0)
            {
                string enchantString = "";
                if (_WowVersion == WowVersionEnum.TBC)
                    enchantString = Code.EnchantIDs.GetValueTBC(_EnchantID);
                else
                    enchantString = Code.EnchantIDs.GetValue(_EnchantID);

                if (specifiedAjaxResult.Contains("Durability"))
                {
                    specifiedAjaxResult = specifiedAjaxResult.Replace("Durability", "<span class=\\\"q2\\\">" + enchantString + "<\\/Span><br />Durability");
                }
                else
                {
                    specifiedAjaxResult = specifiedAjaxResult.Replace("Requires", "<span class=\\\"q2\\\">" + enchantString + "<\\/Span><br />Requires");
                }
            }
            if(String.IsNullOrEmpty(_PiecesStr) == false)
            {
                string[] pieces = _PiecesStr.Split(new string[]{"%3a", ":"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string piece in pieces)
                {
                    int hrefPos = specifiedAjaxResult.IndexOf("<a href=\\\"?item=" + piece + "\\\">");
                    specifiedAjaxResult = specifiedAjaxResult.Insert(hrefPos, "<span class=\\\"q8\\\">");
                    specifiedAjaxResult = specifiedAjaxResult.Insert(specifiedAjaxResult.IndexOf("<\\/a>", hrefPos) + "<\\/a>".Length, "<\\/Span>");
                }
                int itemSetPos = specifiedAjaxResult.IndexOf("<a href=\\\"?itemset=");
                int setNumPos = specifiedAjaxResult.IndexOf("<\\/a> (", itemSetPos);
                setNumPos += "<\\/a> (".Length;
                specifiedAjaxResult = specifiedAjaxResult.Substring(0, setNumPos) + pieces.Length.ToString() + specifiedAjaxResult.Substring(setNumPos + 1);

                for (int i = 2; i <= pieces.Length; ++i)
                {
                    string setStr = ">(" + i.ToString() + ") Set: <a href";
                    int setStrPos = specifiedAjaxResult.IndexOf(setStr);
                    if (setStrPos != -1)
                        specifiedAjaxResult = specifiedAjaxResult.Insert(setStrPos, " class=\\\"q2\\\"");
                }
            }
            string jsItemStr = "" + ItemID + ((_SuffixID != 0) ? ("r" + _SuffixID) : "") + ((_UniqueID != 0) ? ("u" + _UniqueID) : "") + ((_EnchantID != 0) ? ("e" + _EnchantID) : "");
            if (_GemIDs != null && _GemIDs.Length > 0)
            {
                int prevGemStrIndex = 0;
                bool skippedGems = false;
                for (int i = 0; i < _GemIDs.Length; ++i)
                {
                    int gemStrStart = specifiedAjaxResult.IndexOf("<span class=\\\"socket-", prevGemStrIndex);
                    if (gemStrStart == -1)
                        break;
                    int gemStrEnd = specifiedAjaxResult.IndexOf("<\\/span>", gemStrStart);

                    if (_GemIDs[i] != 0)
                    {
                        var gemInfo = Code.GemIDs.GetGemInfo(_GemIDs[i]);
                        var gemItemInfo = Hidden.ApplicationInstance.Instance.GetItemInfo(gemInfo.ItemID, WowVersionEnum.TBC);
                        if (gemItemInfo == null)
                        {
                            specifiedAjaxResult = specifiedAjaxResult.Substring(0, gemStrStart) + "<span class=\\\"socket-meta\\\">" + Code.EnchantIDs.GetValueTBC(_GemIDs[i]) + specifiedAjaxResult.Substring(gemStrEnd);
                            skippedGems = true;
                        }
                        else
                        {
                            string socketType = specifiedAjaxResult.Substring(gemStrStart + "<span class=\\\"socket-".Length, 10);
                            if (socketType.StartsWith("red"))
                                socketType = "red";
                            else if (socketType.StartsWith("yellow"))
                                socketType = "yellow";
                            else if (socketType.StartsWith("blue"))
                                socketType = "blue";
                            else if (socketType.StartsWith("meta"))
                                socketType = "meta";
                            else
                                Logger.ConsoleWriteLine("Invalid SocketType: \"" + socketType + "\"");

                            if ((socketType.StartsWith("red") && ((gemInfo.Color & Code.GemIDs.ColorRed) != Code.GemIDs.ColorRed))
                            || (socketType.StartsWith("yellow") && ((gemInfo.Color & Code.GemIDs.ColorYellow) != Code.GemIDs.ColorYellow))
                            || (socketType.StartsWith("blue") && ((gemInfo.Color & Code.GemIDs.ColorBlue) != Code.GemIDs.ColorBlue))
                            || (socketType.StartsWith("meta") && ((gemInfo.Color & Code.GemIDs.ColorMeta) != Code.GemIDs.ColorMeta)))
                            {
                                skippedGems = true;
                            }
                            var gemStats = Code.EnchantIDs.GetValueTBC(_GemIDs[i]);
                            if (gemStats == "")
                                gemStats = "Unknown Gem";
                            specifiedAjaxResult = specifiedAjaxResult.Substring(0, gemStrStart) + "<span class=\\\"socket-" + socketType + "\\\" style=\\\"background: url(http://database.wow-one.com/" + gemItemInfo.GetIconImageAddress("small") + ") no-repeat left center\\\">" + gemStats + specifiedAjaxResult.Substring(gemStrEnd);
                        }
                    }
                    else
                    {
                        skippedGems = true;
                    }
                    prevGemStrIndex = gemStrStart + 1;
                }
                if (skippedGems == false && specifiedAjaxResult.IndexOf("<span class=\\\"socket-", prevGemStrIndex) == -1)
                {
                    //Means all sockets are in use
                    //Enable socket bonus
                    specifiedAjaxResult = specifiedAjaxResult.Replace("<span class=\\\"q0\\\">Socket Bonus:", "Socket Bonus:<span class=\\\"q2\\\">");
                }

                jsItemStr = jsItemStr + "g" + _GemIDs[0];
                for (int i = 1; i < _GemIDs.Length; ++i)
                {
                    jsItemStr = jsItemStr + "," + _GemIDs[i];
                }
            }
            specifiedAjaxResult = specifiedAjaxResult.Replace("registerItem(" + ItemID, "registerItem('" + jsItemStr + "'");
            return specifiedAjaxResult;
        }
        //public static List<int> sm_Wrath = new List<int>{
        //    16959, 16960, 16961, 16962, 
        //    16963, 16964, 16965, 16966,
        //};
        private int[] GenerateItemSetData()
        {
            List<int> items = new List<int>();
            int itemStopIndex = 0;
            while (itemStopIndex != -1)
            {
                int itemStartIndex = AjaxTooltip.IndexOf("<a href=\\\"?item=", itemStopIndex);
                if (itemStartIndex == -1)
                    break;
                itemStartIndex += "<a href=\\\"?item=".Length;
                itemStopIndex = AjaxTooltip.IndexOf("\\\">", itemStartIndex);
                if (itemStopIndex != -1)
                {
                    string itemID = AjaxTooltip.Substring(itemStartIndex, itemStopIndex - itemStartIndex);
                    items.Add(int.Parse(itemID));
                }
            }

            if(items.Count > 0)
                return items.ToArray();
            else
                return null;
        }
        public string GenerateSetPcsStr(IEnumerable<int> _OtherItemIDs)
        {
            if (m_ItemSet == null)
                return "";
            string pcsString = "";
            int itemID = int.Parse(ItemID);
            if (m_ItemSet.Contains(itemID))
            {
                pcsString = ";pcs=";
                foreach (var item in _OtherItemIDs)
                {
                    if (m_ItemSet.Contains(item))
                    {
                        pcsString += item + ":";
                    }
                }
                pcsString = pcsString.Substring(0, pcsString.Length - 1); //remove last unncessessary :
            }
            return pcsString;
        }
        public string GenerateSetPcsStr(GearData _GearData)
        {
            if (m_ItemSet == null)
                return "";
            string pcsString = "";
            int itemID = int.Parse(ItemID);
            if (m_ItemSet.Contains(itemID))
            {
                pcsString = ";pcs=";
                foreach (var item in _GearData.Items)
                {
                    if (m_ItemSet.Contains(item.Value.ItemID))
                    {
                        pcsString += item.Value.ItemID + ":";
                    }
                }
                pcsString = pcsString.Substring(0, pcsString.Length - 1); //remove last unncessessary :
            }
            return pcsString;
        }

        public ItemInfo()
        { }
        public ItemInfo(int _ItemID, string _AjaxItemData, string _ItemDatabaseAddress)
        {
            string[] itemData = _AjaxItemData.Split('{', '}');
            ItemID = _ItemID.ToString();
            string[] itemDataFields = itemData[1].Split(',');
            foreach (string itemDataField in itemDataFields)
            {
                try
                {
                    if (itemDataField.Contains(':'))
                    {
                        string[] itemDataFieldValues = itemDataField.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                        if (itemDataFieldValues[0] == "name_enus")
                        {
                            int firstSnuf = itemDataFieldValues[1].IndexOf('\'');
                            int secondSnuf = itemDataFieldValues[1].LastIndexOf('\'');
                            ItemName = itemDataFieldValues[1].Substring(firstSnuf + 1, secondSnuf - firstSnuf - 1);
                        }
                        else if (itemDataFieldValues[0] == "quality")
                            ItemQuality = int.Parse(itemDataFieldValues[1]);
                        else if (itemDataFieldValues[0] == "icon")
                            ItemIcon = itemDataFieldValues[1].Replace("\'", "");
                        else if (itemDataFieldValues[0] == "tooltip_enus")
                        {
                            break;
                            //itemInfo.ItemTooltipHtml = itemDataFieldValues[1].Replace("\'", "").Replace("\\", "");
                        }
                    }
                }
                catch (Exception)
                { }
            }
            try
            {
                if (itemDataFields.Last().Contains(':'))
                {
                    string[] itemDataFieldValues = itemDataFields.Last().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    if (itemDataFieldValues[0] == " model")
                    {
                        ItemModel = int.Parse(itemDataFieldValues[1].Replace("'", ""));
                    }
                }
            }
            catch (Exception)
            {}
            AjaxTooltip = _AjaxItemData;
            ItemDataFetchedFrom = _ItemDatabaseAddress;
            m_ItemSet = GenerateItemSetData();
        }
        public int GetModelID(bool _RefreshIfNotFound = true)
        {
            if (ItemModel == 0 && _RefreshIfNotFound == true)
            {
                int itemModelSlot = 0;
                GetModelViewerIDAndSlot(ItemSlot.Unknown, out ItemModel, out itemModelSlot, true);
                /*try
                {
                    string itemDatabaseAddress = "http://db.vanillagaming.org/";
                    var webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(itemDatabaseAddress + "ajax.php?item=" + ItemID);
                    webRequest.Timeout = 5000;
                    webRequest.ReadWriteTimeout = 5000;
                    
                    var webResponse = webRequest.GetResponse();
                    var responseStream = webResponse.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(responseStream);
                    string ajaxItemData = reader.ReadToEnd();
                    if (ajaxItemData.StartsWith("$WowheadPower.registerItem"))//Success?
                    {
                        string[] itemData = ajaxItemData.Split('{', '}');
                        if (itemData.Length == 3)//Success!(?)
                        {
                            string[] itemDataFields = itemData[1].Split(',');
                            if (itemDataFields.Last().Contains(':'))
                            {
                                string[] itemDataFieldValues = itemDataFields.Last().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                                if (itemDataFieldValues[0] == " model")
                                {
                                    ItemModel = int.Parse(itemDataFieldValues[1].Replace("'", ""));
                                    Logger.ConsoleWriteLine("GetModelID(): Successfully grabbed new ModelID data from aowow for ItemID: " + ItemID, ConsoleColor.Green);
                                    Hidden.ApplicationInstance.Instance.m_ItemInfoUpdated = true;
                                }
                                else
                                    throw new Exception("Error_4");
                            }
                            else
                                throw new Exception("Error_3");
                        }
                        else
                            throw new Exception("Error_2");
                    }
                    else
                        throw new Exception("Error_1");
                }
                catch (Exception ex)
                {
                    Logger.ConsoleWriteLine("GetModelID(): Failed to grab new ModelID data from aowow for ItemID: " + ItemID + ", ErrorPathMessage: " + ex.Message, ConsoleColor.Red);
                }*/
            }
            return ItemModel;
        }
        public static string GetAjaxTooltip(string _RequestString, Func<int, WowVersionEnum, ItemInfo> _GetItemInfo = null)
        {
            {
                try
                {
                    var query = System.Web.HttpUtility.ParseQueryString(_RequestString.Substring(_RequestString.IndexOf('?') + 1));
                    string itemStrRaw = query["item"];
                    string suffixStr = query["rand"];
                    string uniqStr = query["uniq"];
                    string enchStr = query["ench"];
                    string pcsStr = query["pcs"];
                    string gemsStr = query["gems"];

                    WowVersionEnum wowVersion = WowVersionEnum.Vanilla;
                    string[] itemStrSplitResult = itemStrRaw.Split('-');

                    if (itemStrSplitResult.Length > 1 && itemStrSplitResult.Last() == "1")
                        wowVersion = WowVersionEnum.TBC;
                    else if (itemStrSplitResult.Length > 1 && itemStrSplitResult.Last() == "0")
                        wowVersion = WowVersionEnum.Vanilla;

                    string itemStr = itemStrSplitResult.First();

                    int itemID = 0;
                    if (int.TryParse(itemStr, out itemID) == false)
                    {
                        return "";
                    }
                    int suffixID = 0;
                    int uniqueID = 0;
                    if (suffixStr != null)
                    {
                        string[] suffixStrSplit = suffixStr.Split('+');
                        if (int.TryParse(suffixStrSplit.First(), out suffixID) == false)
                            suffixID = 0;
                        if (suffixStrSplit.Length > 1 && int.TryParse(suffixStrSplit[1], out uniqueID) == false)
                            uniqueID = 0;
                        if (int.TryParse(uniqStr, out uniqueID) == false)
                            uniqueID = 0;
                    }

                    int enchantID = 0;
                    if (int.TryParse(enchStr, out enchantID) == false)
                        enchantID = 0;

                    int[] gems = null;
                    if (gemsStr != "" && gemsStr != null)
                    {
                        string[] gemStrSplit = gemsStr.Split(':');
                        gems = new int[gemStrSplit.Length];
                        for (int i = 0; i < gems.Length; ++i)
                            gems[i] = int.Parse(gemStrSplit[i]);
                    }

                    var itemInfo = _GetItemInfo(itemID, wowVersion);
                    if (itemInfo == null)
                    {
                        return "";
                    }
                    string ajaxResult = itemInfo.GetAjaxTooltip(wowVersion, itemID, suffixID, enchantID, uniqueID, gems, pcsStr);
                    ajaxResult = ajaxResult.Replace("registerItem('" + itemStr, "registerItem('" + itemStrRaw);
                    return ajaxResult;
                }
                catch (Exception)
                { }
            }
            return "";
        }
        #region Serializing
        public ItemInfo(SerializationInfo _Info, StreamingContext _Context)
        {
            ItemID = _Info.GetString("ItemID");
            ItemIcon = _Info.GetString("ItemIcon");
            ItemName = _Info.GetString("ItemName");
            ItemQuality = _Info.GetInt32("ItemQuality");
            ItemModel = _Info.GetInt32("ItemModel");
            AjaxTooltip = _Info.GetString("AjaxTooltip");
            ItemDataFetchedFrom = _Info.GetString("ItemDataFetchedFrom");
            _GenerateAndSetItemSetData();
        }
        [ProtoAfterDeserialization]
        public void _GenerateAndSetItemSetData()
        {
            m_ItemSet = GenerateItemSetData();
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("ItemID", ItemID);
            _Info.AddValue("ItemIcon", ItemIcon);
            _Info.AddValue("ItemName", ItemName);
            _Info.AddValue("ItemQuality", ItemQuality);
            _Info.AddValue("ItemModel", ItemModel);
            _Info.AddValue("AjaxTooltip", AjaxTooltip);
            _Info.AddValue("ItemDataFetchedFrom", ItemDataFetchedFrom);
        }
        #endregion

        private static Dictionary<ItemSlot, int> sm_ItemSlotToModelSlotConverter = new Dictionary<ItemSlot, int>
        {
            {ItemSlot.Head, 1},
            {ItemSlot.Shoulder, 3},
            {ItemSlot.Chest, 5},
            {ItemSlot.Wrist, 9},
            {ItemSlot.Gloves, 10},
            {ItemSlot.Belt, 6},
            {ItemSlot.Legs, 7},
            {ItemSlot.Feet, 8},
            {ItemSlot.Back, 16},
            {ItemSlot.Main_Hand, 13},
            {ItemSlot.Off_Hand, 14}, //Shield
            //{ItemSlot.Off_Hand, 22}, //Offhand
            {ItemSlot.Tabard, 19 },
            {ItemSlot.Shirt, 4},
            {ItemSlot.Ranged, 26}
            //20 == robe?!?

            //validSlots=[1,3,4,5,6,7,8,9,10,13,14,15,16,17,19,20,21,22,23,25,26]
            /*slotMap={
                * 1:1,
                * 3:3,
                * 4:4,
                * 5:5,
                * 20:5,
                * 6:6,
                * 7:7,
                * 8:8,
                * 9:9,
                * 10:10,
                * 16:16,
                * 19:19,
                * 13:21,
                * 17:21,
                * 25:21,
                * 26:21
                * 21:21,
                * 14:22,
                * 15:22,
                * 22:22,
                * 23:22,*/
        };
        private static List<int> sm_ItemIDsRequireWowheadModel = new List<int>
        {
            23709, //Tabard of Frost (ModelID=38284)

            18527, //Harmonous Gauntlets

            19148, //Dark Iron Helm
            17013, //Dark Iron Leggings
            19164, //Dark Iron Gauntlets
            20039, //Dark Iron Boots

            23019, //Icebane Helmet
            22940, //Icebane Pauldrons
            22699, //Icebane Leggings

            //T3
            //Frostfire Mage
            22496, 
            22497,
            22498, 
            22499, 
            22500, 
            22501, 
            22502, 
            22503, 
            //Frostfire

            //Dreadnaught Warrior
            22416,
            22417,
            22418, 
            22419, 
            22420,
            22421,
            22422, 
            22423, 
            //Dreadnaught

            //Bonescythe Rogue
            22476, 
            22477, 
            22478, 
            22479, 
            22480, 
            22481, 
            22482, 
            22483, 
            //Bonescythe

            //Cryptstalker Hunter
            22436, 
            22437, 
            22438,
            22439, 
            22440, 
            22441,
            22442, 
            22443,
            //Cryptstalker

            //Plagueheart Warlock
            22504,
            22505,
            22506,
            22507,
            22508,
            22509,
            22510,
            22511,
            //Plagueheart

            //Dreamwalker Druid
            22488,
            22489,
            22490,
            22491,
            22492,
            22493,
            22494,
            22495,
            //Dreamwalker

            //of Faith Priest
            22512,
            22513,
            22514,
            22515,
            22516,
            22517,
            22518,
            22519,
            //of Faith

            //Redemption Paladin
            22424,
            22425,
            22426,
            22427,
            22428,
            22429,
            22430,
            22431,
            //Redemption

            //Earthshatter Shaman
            22464,
            22465,
            22466,
            22467,
            22468,
            22469,
            22470,
            22471,
            //Earthshatter
            //T3

            //T2.5
            //Avenger Paladin
            21387,
            21388,
            21389,
            21390,
            21391,
            //Avenger

            //Deathdealer Rogue
            21359, 
            21360, 
            21361, 
            21362, 
            21364, 
            //Deathdealer

            //Enigma Mage
            21343,
            21344,
            21345,
            21346,
            21347,
            //Enigma

            //of the Oracle Priest
            21348,
            21349,
            21350,
            21351,
            21352,
            //Mantle of the Oracle

            //Doomcaller Warlock
            21334,
            21335,
            21336,
            21337,
            21338,
            //Doomcaller
            
            //Stormcaller Shaman
            21372,
            21373,
            21374,
            21375,
            21376,
            //Stormcaller

            //Genesis Druid
            21353,
            21354,
            21355,
            21356,
            21357,
            //Genesis

            //Conqueror Warrior
            21329,
            21330,
            21331,
            21332,
            21333,
            //Conqueror

            //Striker Hunter
            21365,
            21366,
            21367,
            21368,
            21370,
            //Striker
            //T2.5

            //ZG stuffs
            //Bloodvine Casters
            19682,
            19683,
            19684,
            //Bloodvine

            //Zandalar Augur Shaman
            19828,
            19829,
            19830,
            //Zandalar Augur

            //Zandalar Vindicator Warrior
            19822,
            19823,
            19824,
            //Zandalar Vindicator

            //Zandalar Demoniac Warlock
            19848,
            19849,
            20033,
            //Zandalar Demoniac

            //Zandalar Freethinker Paladin
            19825,
            19826,
            19827,
            //Zandalar Freethinker

            //Zandalar Haruspex Druid
            19838,
            19839,
            19840,
            //Zandalar Haruspex

            //Zandalar Illusionist Mage
            19845,
            19846,
            20034,
            //Zandalar Illusionist

            //Zandalar Madcap Rogue
            19834,
            19835,
            19836,
            //Zandalar Madcap

            //Zandalar Predator Hunter
            19831,
            19832,
            19833,
            //Zandalar Predator

            //Zandalar Confessor Priest
            19841,
            19842,
            19843,
            //Zandalar Confessor
            //ZG stuffs
            
            //Highlander's Priest, Mage, Warlock
            20047,
            20054,
            20061,
            //Highlander's

            16474, //Field Marshal's Lamellar Faceguard
            16478, //Field Marshal's Plate helm

            19870, //Hakkari Loa Cloak
            18208, //Drape of Benediction

            22385, //Titanic Legs
            23068, //Legplates of Carnage
            23070, //Leggings of Polarity

            21186, //Rockfury Bracers
            22936, //Wristguards of Vengeance

            38, //Recruit's Shirt

            21650, //AQRipper
        };
        //private static Dictionary<int, int> sm_ItemIDToModel = new Dictionary<int, int>{
        //    //Frostfire
        //    {22496, 35523},

        //    {22497, 96846},
        //    {22498, 96847},
        //    {22499, 96848},
        //    {22500, 96849},
        //    {22501, 96850},
        //    {22503, 96851},
            
        //    {22502, 109278},
        //    //Frostfire

        //    //Dreadnaught
        //    {22416, 35049},

        //    {22417, 96810},
        //    {22418, 96811},
        //    {22419, 96812},
        //    {22420, 96813},
        //    {22421, 96814},
        //    {22423, 96815},
            
        //    {22422, 109270},
        //    //Dreadnaught

        //    //Bonescythe
        //    {22476, 35054},
            
        //    {22477, 96834},
        //    {22478, 96835},
        //    {22479, 96836},
        //    {22480, 96837},
        //    {22481, 96838},
        //    {22483, 96839},

        //    {22482, 109246},
        //    //Bonescythe

        //    //Cryptstalker
        //    {22436, 35415},
            
        //    {22437, 96822},
        //    {22438, 96823},
        //    {22439, 96824},
        //    {22440, 96825},
        //    {22441, 96826},
        //    {22443, 96827},

        //    {22442, 35410},
        //    //Cryptstalker

        //    //T2.5
        //    //Deathdealer
        //    {21360, 117255},
        //    //Deathdealer

        //    //AQRipper
        //    {21650, 41491},
        //    //AQRipper
        //};
        //public int GetRealModelID()
        //{
        //    int itemID = 0;
        //    if(int.TryParse(ItemID, out itemID) == true)
        //    {
        //        if (sm_ItemIDToModel.ContainsKey(itemID))
        //        {
        //            return sm_ItemIDToModel[itemID];
        //        }
        //    }
        //    return ItemModel;
        //}
        bool UnableToGetViewerID = false;
        public bool GetModelViewerIDAndSlot(ItemSlot _ItemSlot, out int _RetModelViewerID, out int _RetModelViewerSlot, bool _RefreshIfNotFound = true)
        {
            if (ItemModelViewerID == 0 || ItemModelViewerSlot == 0)
            {
                if (_RefreshIfNotFound == false)
                {
                    _RetModelViewerID = 0;
                    _RetModelViewerSlot = 0;
                    return false;
                }
                if (_ItemSlot != ItemSlot.Unknown)
                {
                    if (sm_ItemSlotToModelSlotConverter.ContainsKey(_ItemSlot) == false)
                    {
                        _RetModelViewerID = 0;
                        _RetModelViewerSlot = 0;
                        return false;
                    }
                    if (sm_ItemIDsRequireWowheadModel.Contains(int.Parse(ItemID)) == false && _ItemSlot != ItemSlot.Chest)
                    {
                        _RetModelViewerID = GetModelID(true);
                        _RetModelViewerSlot = sm_ItemSlotToModelSlotConverter[_ItemSlot];
                        return true;
                    }
                }
                
                {
                    try
                    {
                        if (UnableToGetViewerID == true)
                            throw new Exception("");
                        var webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://www.wowhead.com/item=" + ItemID + "&xml");
                        webRequest.Timeout = 5000;
                        webRequest.ReadWriteTimeout = 5000;
                        webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.125 Safari/537.36";
                        webRequest.Proxy = null;
                        using (var webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse())
                        {
                            using (System.IO.StreamReader reader = new System.IO.StreamReader(webResponse.GetResponseStream()))
                            {
                                string xmlItemData = reader.ReadToEnd();
                                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                                doc.LoadXml(xmlItemData);
                                var rootElement = doc.DocumentElement;
                                string jsonCDATA = rootElement["item"]["json"].InnerText;
                                if (rootElement["item"]["icon"].Attributes[0].Name == "displayId")
                                    ItemModelViewerID = int.Parse(rootElement["item"]["icon"].Attributes[0].Value);
                                else
                                    throw new Exception("");

                                int slotbakIndex = jsonCDATA.IndexOf("\"slotbak\":") + "\"slotbak\":".Length;
                                string slotbak = jsonCDATA.Substring(slotbakIndex, jsonCDATA.IndexOf(',', slotbakIndex) - slotbakIndex);
                                ItemModelViewerSlot = short.Parse(slotbak);

                                //if(rootElement["item"]["inventorySlot"].Attributes[0].Name == "id")
                                //    ItemModelViewerSlot = short.Parse(rootElement["item"]["inventorySlot"].Attributes[0].Value);
                                //else
                                //    throw new Exception("");

                                Logger.ConsoleWriteLine("GetModelViewerIDAndSlot(): Successfully grabbed new data from Wowhead for ItemID: " + ItemID, ConsoleColor.Green);
                                Hidden.ApplicationInstance.Instance.m_ItemInfoUpdated = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        UnableToGetViewerID = true;
                        Logger.ConsoleWriteLine("GetModelViewerIDAndSlot(): Exception was thrown, failed to grab new data from Wowhead for ItemID: " + ItemID + ", ExceptionData: " + ex.ToString(), ConsoleColor.Red);
                        _RetModelViewerID = 0;
                        _RetModelViewerSlot = 0;
                        return false;
                    }
                }
            }
            _RetModelViewerID = ItemModelViewerID;
            _RetModelViewerSlot = ItemModelViewerSlot;
            return true;
        }
    }
}