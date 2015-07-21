using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerSex = VF_RealmPlayersDatabase.PlayerSex;

using PlayerItemInfo = VF_RealmPlayersDatabase.PlayerData.ItemInfo;

namespace RealmPlayersServer
{
    public partial class CharacterDesigner : System.Web.UI.Page
    {
        public static System.Collections.Concurrent.ConcurrentDictionary<string, string> g_CachedShortURLs = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        public MvcHtmlString m_CharacterDesignerInfo = null;
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_InventoryInfoHTML = null;
        public MvcHtmlString m_GearStatsHTML = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            string dataStr = PageUtility.GetQueryString(Request, "data");
            string charStr = PageUtility.GetQueryString(Request, "char");
            string itemsStr = PageUtility.GetQueryString(Request, "items");
            WowRealm realm = PageUtility.GetQueryRealm(Request);
            if (realm == WowRealm.Unknown)
                realm = WowRealm.All;
            var wowVersion = PageUtility.GetQueryWowVersion(Request);
            if (PageUtility.GetQueryString(Request, "generateShortURL") != "null")
            {
                var fullURL = "http://realmplayers.com/CharacterDesigner.aspx?char=" + charStr + "&items=" + itemsStr;
                string shortURL = "";
                if (g_CachedShortURLs.TryGetValue(fullURL, out shortURL) == false)
                {
                    shortURL = VF.URLShortener.CreateShortURL(fullURL);
                    if (shortURL == "")
                        Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "generateShortURL", "null"));

                    g_CachedShortURLs.TryAdd(fullURL, shortURL);
                    g_CachedShortURLs.TryAdd(shortURL, fullURL);
                }
                //if (Request.Url.Host == "localhost")
                //    Response.Redirect("localhost:4633/CharacterDesigner.aspx?char=" + charStr + "&items=" + itemsStr);
                //else
                Response.Redirect("CharacterDesigner.aspx?data=" + shortURL.Substring("http://goo.gl/".Length));
            }
            if (dataStr == "null" && itemsStr == "null")
                return;
            if (dataStr != "null" && itemsStr == "null")
            {
                try
                {
                    string fullURL = "";
                    if (g_CachedShortURLs.TryGetValue("http://goo.gl/" + dataStr, out fullURL) == false)
                    {
                        fullURL = VF.URLShortener.GetFullURL("http://goo.gl/" + dataStr);
                        if (fullURL == "")
                            return;
                        g_CachedShortURLs.TryAdd("http://goo.gl/" + dataStr, fullURL);
                    }
                    var url = new Uri(fullURL);
                    var queryString = System.Web.HttpUtility.ParseQueryString(url.Query);
                    itemsStr = queryString.Get("items");
                    if (itemsStr == null)
                        return;
                    var tempcharStr = queryString.Get("char");
                    if (tempcharStr != null)
                        charStr = tempcharStr;
                }
                catch (Exception)
                {
                    return;
                }
            }
            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddFinish("Character Designer"));

            List<PlayerItemInfo> itemsList = new List<PlayerItemInfo>();
            var itemsStrSplit = itemsStr.Split(',', '+', ' ');

            Func<PlayerItemInfo, PlayerItemInfo, bool> slotEqualFunc = (_Value1, _Value2) => _Value1.Slot == _Value2.Slot;
            foreach (string itemLink in itemsStrSplit)
            {
                try
                {
                    itemsList.AddUnique(new PlayerItemInfo(itemLink, wowVersion), slotEqualFunc);
                }
                catch (Exception)
                {
                    try
                    {
                        var splittedLink = itemLink.Split(':', 'x');
                        if (splittedLink.Length > 1)
                        {
                            PlayerItemInfo itemInfo = new PlayerItemInfo("1:0:0:0:0:0:0:0:0", wowVersion);
                            if (Enum.TryParse(splittedLink[0], true, out itemInfo.Slot) == false)
                                itemInfo.Slot = (ItemSlot)int.Parse(splittedLink[0]);

                            itemInfo.ItemID = int.Parse(splittedLink[1]);

                            if (splittedLink.Length > 2)
                                itemInfo.EnchantID = int.Parse(splittedLink[2]);
                            if (splittedLink.Length > 3)
                                itemInfo.SuffixID = int.Parse(splittedLink[3]);
                            if (splittedLink.Length > 4)
                                itemInfo.UniqueID = int.Parse(splittedLink[4]);
                            if (splittedLink.Length > 5 && wowVersion == WowVersionEnum.TBC)
                            {
                                itemInfo.GemIDs = new int[4];
                                itemInfo.GemIDs[0] = int.Parse(splittedLink[5]);
                                itemInfo.GemIDs[1] = 0;
                                itemInfo.GemIDs[2] = 0;
                                itemInfo.GemIDs[3] = 0;
                                if(splittedLink.Length > 6)
                                    itemInfo.GemIDs[1] = int.Parse(splittedLink[6]);
                                if (splittedLink.Length > 7)
                                    itemInfo.GemIDs[2] = int.Parse(splittedLink[7]);
                                if (splittedLink.Length > 8)
                                    itemInfo.GemIDs[3] = int.Parse(splittedLink[8]);
                            }

                            itemsList.AddUnique(itemInfo, slotEqualFunc);
                        }
                    }
                    catch (Exception)
                    {}
                }
            }
            PlayerRace playerRace = PlayerRace.Orc;
            PlayerClass playerClass = PlayerClass.Warrior;
            PlayerSex playerSex = PlayerSex.Male;

            if(charStr != "null")
            {
                try 
	            {
                    var charStrSplit = charStr.Split(',', '+', ' ');
                    if(charStrSplit.Length > 0)
                    {
                        if (Enum.TryParse(charStrSplit[0], true, out playerRace) == false)
                            playerRace = (PlayerRace)int.Parse(charStrSplit[0]);
                    }
                    if(charStrSplit.Length > 1)
                    {
                        if (Enum.TryParse(charStrSplit[1], true, out playerClass) == false)
                            playerClass = (PlayerClass)int.Parse(charStrSplit[1]);
                    }
                    if(charStrSplit.Length > 2)
                    {
                        if (Enum.TryParse(charStrSplit[2], true, out playerSex) == false)
                            playerSex = (PlayerSex)int.Parse(charStrSplit[2]);
                    }
	            }
	            catch (Exception)
	            {}
            }
            if (playerRace == PlayerRace.Unknown)
                playerRace = PlayerRace.Orc;
            if (playerClass == PlayerClass.Unknown)
                playerClass = PlayerClass.Warrior;
            if (playerSex == PlayerSex.Unknown)
                playerSex = PlayerSex.Male;

            m_InventoryInfoHTML = new MvcHtmlString(CreateInventoryInfo(itemsList, playerRace, playerClass, playerSex, realm, wowVersion));
            GenerateGearStats(itemsList, wowVersion);
            if (dataStr != "null")
            {
                m_CharacterDesignerInfo = new MvcHtmlString("Short link for this Character: <br /><a href='http://realmplayers.com/vchar/" + dataStr + ".aspx'>http://realmplayers.com/vchar/" + dataStr + ".aspx</a> or <a href='http://goo.gl/" + dataStr + "'>http://goo.gl/" + dataStr + "</a>");
            }
            else
            {
                m_CharacterDesignerInfo = new MvcHtmlString("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "generateShortURL", "true") + "'>Click here to create a short link for this Character</a>");
            }
            
            if (IsPostBack == false)
            {
                foreach (var item in itemsList)
                {
                    int gemIDcount = item.GetGemIDCount();
                    string itemStr = item.ItemID.ToString();
                    if (item.EnchantID != 0 || item.SuffixID != 0 || item.UniqueID != 0 || gemIDcount != 0)
                    {
                        itemStr += "x" + item.EnchantID.ToString();
                        if (item.SuffixID != 0 || item.UniqueID != 0 || gemIDcount != 0)
                            itemStr += "x" + item.SuffixID.ToString();
                        if (item.UniqueID != 0 || gemIDcount != 0)
                            itemStr += "x" + item.UniqueID.ToString();
                        if (gemIDcount != 0)
                        {
                            for (int i = 0; i < gemIDcount; ++i)
                            {
                                itemStr += "x" + item.GemIDs[i].ToString();
                            }
                        }
                    }
                    switch (item.Slot)
                    {
                        case ItemSlot.Head:
                            txtHeadSlot.Text = itemStr;
                            break;
                        case ItemSlot.Neck:
                            txtNeckSlot.Text = itemStr;
                            break;
                        case ItemSlot.Shoulder:
                            txtShoulderSlot.Text = itemStr;
                            break;
                        case ItemSlot.Shirt:
                            txtShirtSlot.Text = itemStr;
                            break;
                        case ItemSlot.Chest:
                            txtChestSlot.Text = itemStr;
                            break;
                        case ItemSlot.Belt:
                            txtBeltSlot.Text = itemStr;
                            break;
                        case ItemSlot.Legs:
                            txtLegsSlot.Text = itemStr;
                            break;
                        case ItemSlot.Feet:
                            txtFeetSlot.Text = itemStr;
                            break;
                        case ItemSlot.Wrist:
                            txtWristSlot.Text = itemStr;
                            break;
                        case ItemSlot.Gloves:
                            txtGlovesSlot.Text = itemStr;
                            break;
                        case ItemSlot.Finger_1:
                            txtRing1Slot.Text = itemStr;
                            break;
                        case ItemSlot.Finger_2:
                            txtRing2Slot.Text = itemStr;
                            break;
                        case ItemSlot.Trinket_1:
                            txtTrinket1Slot.Text = itemStr;
                            break;
                        case ItemSlot.Trinket_2:
                            txtTrinket2Slot.Text = itemStr;
                            break;
                        case ItemSlot.Back:
                            txtBackSlot.Text = itemStr;
                            break;
                        case ItemSlot.Main_Hand:
                            txtMainhandSlot.Text = itemStr;
                            break;
                        case ItemSlot.Off_Hand:
                            txtOffhandSlot.Text = itemStr;
                            break;
                        case ItemSlot.Ranged:
                            txtRangedSlot.Text = itemStr;
                            break;
                        case ItemSlot.Tabard:
                            txtTabardSlot.Text = itemStr;
                            break;
                        default:
                            break;
                    }
                }

                ddlRace.SelectedValue = playerRace.ToString();
                if (StaticValues.GetFaction(playerRace) == VF_RealmPlayersDatabase.PlayerFaction.Horde)
                    ddlRace.Style.Add("color", "#ff4546");
                else
                    ddlRace.Style.Add("color", "#45a3ff");

                ddlClass.SelectedValue = playerClass.ToString();
                ddlClass.Style.Add("color", PageUtility.GetClassColor(playerClass));

                ddlSex.SelectedValue = playerSex.ToString();
            }
        }


        public string CreateInventoryInfo(List<PlayerItemInfo> _Items, PlayerRace _Race, PlayerClass _Class, PlayerSex _Sex, WowRealm _Realm = WowRealm.All, WowVersionEnum _WowVersion = WowVersionEnum.Vanilla)
        {
            string invInfo = "";

            string currentItemDatabase = DatabaseAccess.GetCurrentItemDatabaseAddress();

            var itemSummaryDB = Hidden.ApplicationInstance.Instance.GetItemSummaryDatabase();

            string modelEqString = "";
            var itemIDs = _Items.Select((_Value) => _Value.ItemID);
            foreach (var item in _Items)
            {
                var itemInfo = DatabaseAccess.GetItemInfo(item.ItemID, _WowVersion);
                if (itemInfo == null)
                    invInfo += "<div class='equipment-slot' id='" + CharacterViewer.ItemSlotToIDConversion[item.Slot] + "'><div class='quality' id='epic'></div><img src='assets/img/icons/inv_misc_questionmark.png'/></div>";
                else
                {
                    string currInvInfo = "<div class='equipment-slot' id='" + CharacterViewer.ItemSlotToIDConversion[item.Slot] + "'>"
                                + "<img class='itempic' src='" + "http://realmplayers.com/" + itemInfo.GetIconImageAddress() + "'/>"
                                + "<div class='quality' id='" + CharacterViewer.ItemQualityConversion[itemInfo.ItemQuality] + "'></div>"
                                + "<img class='itemframe' src='assets/img/icons/ItemNormalFrame.png'/>"
                                + CharacterViewer.GenerateItemLink(currentItemDatabase, item, _WowVersion, itemInfo.GenerateSetPcsStr(itemIDs));

                    if (item.Slot == ItemSlot.Head && ModelViewerOptions.HideHead == true)
                    { }
                    else
                    {

                        if ((_Class == VF_RealmPlayersDatabase.PlayerClass.Hunter && (item.Slot == ItemSlot.Main_Hand || item.Slot == ItemSlot.Off_Hand))
                        || (_Class != VF_RealmPlayersDatabase.PlayerClass.Hunter && item.Slot == ItemSlot.Ranged))
                        { }
                        else
                        {
                            int modelViewerID = 0;
                            int modelViewerSlot = 0;
                            if (itemInfo.GetModelViewerIDAndSlot(item.Slot, out modelViewerID, out modelViewerSlot, true) == true)
                            {
                                if (item.Slot == ItemSlot.Off_Hand)
                                {
                                    if (itemInfo.AjaxTooltip.Contains("Shield") == false)
                                        modelViewerSlot = 22;
                                }
                                if (modelViewerID != 0)
                                    modelEqString = modelEqString + "," + modelViewerSlot + "," + modelViewerID;
                            }
                        }
                    }
                    //List<Tuple<DateTime, string>> players = null;
                    if (_Realm != WowRealm.All)
                    {
                        int usageCount = itemSummaryDB.GetItemUsageCount(_Realm, item);
                        currInvInfo += "<a class='itemplayersframe' href='ItemUsageInfo.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&item=" + item.ItemID + (item.SuffixID != 0 ? "&suffix=" + item.SuffixID : "") + "'>" + usageCount + "</a>";
                    }
                    currInvInfo += "</div>";
                    invInfo += currInvInfo;
                }
            }
            invInfo += "<img style='position: absolute;z-index: 1;' src='assets/img/bg/CharacterBackgroundTransparent.png'></img>";

            string modelViewerQuery = PageUtility.GetQueryString(Request, "modelviewer", "unknown").ToLower();
            if (modelViewerQuery != "false" && modelEqString != "" && ModelViewerOptions.View3DModel == true)
            {
                if (modelEqString[0] == ',')//För att bli av med det första ","
                    modelEqString = modelEqString.Substring(1);
                //modelEqString = "1,33743,3,33653,5,33650,6,31110,7,31115,8,31111,9,31127,10,33651,13,25629";
                string modelCharString = ((_Race == VF_RealmPlayersDatabase.PlayerRace.Undead) ? "Scourge" : _Race.ToString().Replace("_", "")) + _Sex.ToString();// "orcmale";

                invInfo += "<div style='z-index: 0; position: absolute; margin: 33px 77px; width:385px; height:512px;'><object type='application/x-shockwave-flash' data='http://wow.zamimg.com/modelviewer/ZAMviewerfp11.swf' width='385' height='512' id='dsjkgbdsg2346' style='visibility: visible;'>"
                            + "<param name='quality' value='low'>"
                            + "<param name='allowscriptaccess' value='always'>"
                            + "<param name='allowfullscreen' value='false'>"
                            + "<param name='menu' value='false'>"
                            + "<param name='bgcolor' value='#181818'>"
                            + "<param name='wmode' value='direct'>"
                            + "<param name='flashvars' value='hd=false&amp;model="
                                + modelCharString
                            + "&amp;modelType=16&amp;contentPath=http://wow.zamimg.com/modelviewer/&amp;equipList="
                                + modelEqString
                            + "'>"
                        + "</object></div>";

            }
            else
                invInfo += "<img class='characterimage' src='" + StaticValues.GetRaceCrestImageUrl(_Race) + "'></img>";

            Hidden.ApplicationInstance.Instance.BackupItemInfos();
            return invInfo;
        }

        public void GenerateGearStats(List<PlayerItemInfo> _Items, WowVersionEnum _WowVersion)
        {
            //m_GearStatsHTML
            {
                string gearStatsStr = "<h4 style='width:555px;text-align:center;'>Total Item Stats</h4>";//<div style='width:555px;text-align:center;'><h3>Total Gear Stats</h3></div>
                gearStatsStr += "<table style='width:555px;'><tbody><tr><td style='vertical-align:top'>";
                var gearStats = Code.GearAnalyze.GenerateGearStats(_Items, _WowVersion);

                #region GearStats Ordering
                var gearStatsSorted = gearStats.OrderBy((_Value) =>
                {
                    switch (_Value.StatType)
                    {
                        case RealmPlayersServer.Code.ItemStatType.Armor:
                            return 0;
                        case RealmPlayersServer.Code.ItemStatType.Stamina:
                            return 10;
                        case RealmPlayersServer.Code.ItemStatType.Agility:
                            return 20;
                        case RealmPlayersServer.Code.ItemStatType.Strength:
                            return 30;
                        case RealmPlayersServer.Code.ItemStatType.Spirit:
                            return 40;
                        case RealmPlayersServer.Code.ItemStatType.Intellect:
                            return 50;
                        case RealmPlayersServer.Code.ItemStatType.Health:
                            return 60;
                        case RealmPlayersServer.Code.ItemStatType.Mana:
                            return 70;
                        case RealmPlayersServer.Code.ItemStatType.Mp5:
                            return 80;
                        case RealmPlayersServer.Code.ItemStatType.Defense:
                            return 90;
                        case RealmPlayersServer.Code.ItemStatType.Block_Chance:
                            return 100;
                        case RealmPlayersServer.Code.ItemStatType.Dodge_Chance:
                            return 120;
                        case RealmPlayersServer.Code.ItemStatType.Parry_Chance:
                            return 130;
                        case RealmPlayersServer.Code.ItemStatType.Block_Value:
                            return 140;
                        case RealmPlayersServer.Code.ItemStatType.Attack_Power:
                            return 150;
                        case RealmPlayersServer.Code.ItemStatType.Attack_Speed:
                            return 155;
                        case RealmPlayersServer.Code.ItemStatType.Crit_Chance:
                            return 160;
                        case RealmPlayersServer.Code.ItemStatType.Hit_Chance:
                            return 170;
                        case RealmPlayersServer.Code.ItemStatType.Spell_Damage:
                            return 180;
                        case RealmPlayersServer.Code.ItemStatType.Spell_Damage_and_Healing:
                            return 190;
                        case RealmPlayersServer.Code.ItemStatType.Spell_Healing:
                            return 200;
                        case RealmPlayersServer.Code.ItemStatType.Frost_Spell_Damage:
                            return 210;
                        case RealmPlayersServer.Code.ItemStatType.Fire_Spell_Damage:
                            return 220;
                        case RealmPlayersServer.Code.ItemStatType.Shadow_Spell_Damage:
                            return 230;
                        case RealmPlayersServer.Code.ItemStatType.Nature_Spell_Damage:
                            return 240;
                        case RealmPlayersServer.Code.ItemStatType.Spell_Crit_Chance:
                            return 250;
                        case RealmPlayersServer.Code.ItemStatType.Spell_Hit_Chance:
                            return 260;
                        case RealmPlayersServer.Code.ItemStatType.Shadow_Resistance:
                            return 500;
                        case RealmPlayersServer.Code.ItemStatType.Fire_Resistance:
                            return 510;
                        case RealmPlayersServer.Code.ItemStatType.Frost_Resistance:
                            return 520;
                        case RealmPlayersServer.Code.ItemStatType.Nature_Resistance:
                            return 530;
                        case RealmPlayersServer.Code.ItemStatType.Arcane_Resistance:
                            return 540;
                        case RealmPlayersServer.Code.ItemStatType.All_Resistances:
                            return 550;
                        //Not really in use
                        case RealmPlayersServer.Code.ItemStatType.Unknown:
                            return -2;
                        case RealmPlayersServer.Code.ItemStatType.Item_Level:
                            return 99999;
                        default:
                            return -1;
                    }
                }).ToList();
                #endregion

                int column2Start = (int)Math.Ceiling((float)gearStatsSorted.Count / 3.0f);
                int column3Start = (int)Math.Floor(2.0f * ((float)gearStatsSorted.Count / 3.0f));
                int counter = 0;
                foreach (var gearStat in gearStatsSorted)
                {
                    if (gearStat.StatType == Code.ItemStatType.Item_Level)
                        continue;

                    if (counter == column2Start || counter == column3Start)
                        gearStatsStr += "</td><td style='vertical-align:top'>";
                    ++counter;

                    string valuePrefix = "";
                    string statTypeCompareStr = gearStat.StatType.ToString().ToLower();
                    if (statTypeCompareStr.Contains("chance"))
                        valuePrefix = "%";
                    else if (statTypeCompareStr.Contains("speed"))
                        valuePrefix = "%";

                    gearStatsStr += gearStat.StatType.ToString().Replace('_', ' ') + ": <font color='#fff'>" + gearStat.NormalValue + valuePrefix + "</font>";
                    if (gearStat.EnchantValue != 0)
                        gearStatsStr += " + <font color='#00ff00'>" + gearStat.EnchantValue + valuePrefix + "</font>";
                    gearStatsStr += "<br />";
                }
                //gearStatsStr += "</td><td style='vertical-align:top'>";
                //foreach (var gearStat in gearStatsEnchants)
                //{
                //    gearStatsStr += gearStat.Type.ToString().Replace('_', ' ') + ": 0 + <font color='#00ff00'>" + gearStat.Value + "</font><br />";
                //}
                ////foreach (var gearStat in gearStats)
                ////{
                ////    if (gearStat.Enchant == true)
                ////    {
                ////        gearStatsStr += gearStat.Type.ToString().Replace('_', ' ') + ": " + gearStat.Value + "<br />";
                ////    }
                ////}
                gearStatsStr += "</td></tr></tbody></table>";
                m_GearStatsHTML = new MvcHtmlString(gearStatsStr);
            }
        }

        private string GetLeanItemLink(ItemSlot _Slot, string _ItemLink)
        {
            if (_ItemLink == "0")
                return "";
            var newItemLink = _ItemLink;
            try
            {
                while ((newItemLink.EndsWith(":0") || newItemLink.EndsWith("x0")) && newItemLink.Length > 2)
                {
                    newItemLink = newItemLink.Substring(0, newItemLink.Length - 2);
                }
                while(newItemLink.EndsWith(":") || newItemLink.EndsWith("x"))
                    newItemLink = newItemLink.Substring(0, newItemLink.Length - 1);
                if (newItemLink == "0" || newItemLink == "")
                    return "";
                return _Slot.ToString() + "x" + newItemLink;
            }
            catch (Exception)
            {
                return _Slot.ToString() + "x" + _ItemLink;
            }
        }
        public void AddLeanItemLink(ref string _ItemStr, ItemSlot _Slot, string _ItemLink)
        {
            var leanItemLink = GetLeanItemLink(_Slot, _ItemLink);
            if (leanItemLink != "")
                _ItemStr = _ItemStr + leanItemLink + "+";
        }
        protected void CharacterDesignerRefresh_Click(object sender, EventArgs e)
        {
            string itemsStr = "";
            AddLeanItemLink(ref itemsStr, ItemSlot.Head, txtHeadSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Neck, txtNeckSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Shoulder, txtShoulderSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Back, txtBackSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Chest, txtChestSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Shirt, txtShirtSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Tabard, txtTabardSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Wrist, txtWristSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Gloves, txtGlovesSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Belt, txtBeltSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Legs, txtLegsSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Feet, txtFeetSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Finger_1, txtRing1Slot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Finger_2, txtRing2Slot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Trinket_1, txtTrinket1Slot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Trinket_2, txtTrinket2Slot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Main_Hand, txtMainhandSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Off_Hand, txtOffhandSlot.Text);
            AddLeanItemLink(ref itemsStr, ItemSlot.Ranged, txtRangedSlot.Text);

            string charStr = ddlRace.SelectedValue + "+" + ddlClass.SelectedValue + "+" + ddlSex.SelectedValue;

            Response.Redirect("CharacterDesigner.aspx?char=" + charStr + "&items=" + itemsStr);
        }
    }
}