using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using RealmDatabase = VF_RealmPlayersDatabase.RealmDatabase;

using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;
using PlayerData = VF_RealmPlayersDatabase.PlayerData;

using PlayerExtraData = VF_RealmPlayersDatabase.PlayerData.ExtraData;

using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

public static class Generate
{
    public static MvcHtmlString Mvc(string _String)
    {
        return new MvcHtmlString(_String);
    }
}

namespace RealmPlayersServer
{
    public partial class CharacterViewer : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_CharInfoHTML = null;
        public MvcHtmlString m_FameInfoHTML = null;
        public MvcHtmlString m_InventoryInfoHTML = null;
        public MvcHtmlString m_StatsInfoHTML = null;
        public MvcHtmlString m_ItemSetsDropdownOptions = null;
        public MvcHtmlString m_ChartSection = null;
        public MvcHtmlString m_ReceivedItemsHTML = null;
        public MvcHtmlString m_GearStatsHTML = null;
        public MvcHtmlString m_ExtraDataHTML = null;

        public string m_RaidStatsPlayerOverviewLink = "";
        public string m_CharacterDesignerLink = "";

        public void GenerateCharInfo(Player _Player, PlayerHistory _PlayerHistory)
        {
            var wowVersion = StaticValues.GetWowVersion(_Player.Realm);
            var playerFaction = StaticValues.GetFaction(_Player.Character.Race);
            {
                string charInfo = "<div class='char-details span10' id='" + StaticValues.GetFactionCSSName(playerFaction) + "_bg'>";
                charInfo += PageUtility.CreatePVPRankIMG(_Player, true)
                    + "<div class='char-name'>" + _Player.Name + "</div>"
                    + "<div class='char-level'>" + _Player.Character.Level + "</div>"
                    + PageUtility.CreateLargeRaceIMG(_Player)
                    + PageUtility.CreateLargeClassIMG(_Player.Character.Class)
                    + "<div class='clearfix'></div>";
                if (_Player.Guild.GuildName != "nil")
                    charInfo += "<div class='char-guild'>" + _Player.Guild.GuildRank + " of <a href='GuildViewer.aspx?realm=" + Request.QueryString.Get("realm") + "&guild=" + System.Web.HttpUtility.HtmlEncode(_Player.Guild.GuildName) + "'>&lt;" + _Player.Guild.GuildName + "&gt;</a></div>";

                charInfo += "</div>";
                m_CharInfoHTML = new MvcHtmlString(charInfo);
                m_FameInfoHTML = new MvcHtmlString("");
            }
            {
                string statsInfo = "";
                statsInfo += "<h4 class='success'>Character Info(" + _Player.LastSeen.ToLocalTime().ToString("yyyy-MM-dd") + ")</h4>";

                if (_PlayerHistory != null && _PlayerHistory.HaveValidHistory() == true)
                {
                    statsInfo += "<h5 class='hnoextraline'>History</h5>";
                    statsInfo += "First seen: " + _PlayerHistory.GetUpdates().First().GetTime().ToLocalTime().ToString("yyyy-MM-dd") + "<br/>";
                    statsInfo += "Updated: " + _PlayerHistory.GetUpdates().Count + " times";

                    var realm = PageUtility.GetQueryRealm(Request);
                    string playerStr = PageUtility.GetQueryString(Request, "player");
                    string oldName = "";
                    var realmHistory = DatabaseAccess.GetRealmPlayersHistory(this, PageUtility.GetQueryRealm(Request), NotLoadedDecision.SpinWait);
                    if (Code.PlayerAnalyze.HasPlayerNameChanged(realmHistory, playerStr, wowVersion, out oldName) == true)
                    {
                        statsInfo += "<h5 class='hnoextraline'>Name Changes</h5>";
                        statsInfo += PageUtility.CreatePlayerLink(oldName, realm) + " -> " + PageUtility.CreatePlayerLink(playerStr, realm) + "<br/>";
                    }
                    
                    var raceOrSexChanges = _PlayerHistory.GetRaceOrSexChanges();
                    if (raceOrSexChanges.Count > 1)
                    {
                        statsInfo += "<h5 class='hnoextraline'>Race Changes</h5>";
                        var prevRaceChange = raceOrSexChanges.First();
                        for (int i = 1; i < raceOrSexChanges.Count; ++i)
                        {
                            var raceChange = raceOrSexChanges[i];
                            statsInfo += "<div><div style='float: left; width: 72px; height:20px;overflow: hidden;white-space: nowrap;'>" + raceChange.Uploader.GetTime().ToLocalTime().ToString("yyyy-MM-dd") + ": </div>"
                                + " " + PageUtility.CreateRaceIMG(prevRaceChange.Data.Race, prevRaceChange.Data.Sex, 20) + "<div style='width: 20px; height:20px;display: block; background-image: url(\"assets/img/icons/mini_cal_icon_right_arrow.gif\");background-size: 100%;float: left;'></div>"
                                + PageUtility.CreateRaceIMG(raceChange.Data.Race, raceChange.Data.Sex, 20) + "</div><br/>";
                            prevRaceChange = raceChange;
                        }
                    }
                }
                statsInfo += "<h5 class='hnoextraline'>Last Seen</h5>";
                statsInfo += "" + _Player.LastSeen.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " by " + _Player.GetUploaderName() + "<br/>";

                statsInfo += "<h4 class='success'>PVP Stats</h4>";
                if (wowVersion == WowVersionEnum.Vanilla)
                {
                    statsInfo += "<h5 class='hnoextraline'>Current</h5>";
                    statsInfo += "Rank: " + StaticValues.ConvertRankVisualWithNr(_Player.Honor.CurrentRank, playerFaction) + "<br/>";
                    statsInfo += "Rank Progress: " + _Player.Honor.CurrentRankProgress.ToString("0.0%") + "<br/>";
                    statsInfo += "Standing: #" + _Player.GetStandingStr() + "<br/>";
                    if (_PlayerHistory != null && _PlayerHistory.HaveValidHistory() == true)
                    {
                        float rankChange = StaticValues.CalculateRankChange(_Player, _PlayerHistory);
                        statsInfo += "Rank Change: " + (rankChange >= 0.0f ? "<font color='#00ff00'>+" : "<font color='#ff0000'>") + rankChange.ToString("0.0%") + "</font><br/>";
                    }
                    statsInfo += "<h5 class='hnoextraline'>Today</h5>";
                    statsInfo += "Honorable Kills: " + _Player.Honor.TodayHK + "<br/>";
                    statsInfo += "Dishonorable Kills: " + _Player.Honor.TodayDK + "<br/>";
                    statsInfo += "<h5 class='hnoextraline'>Yesterday</h5>";
                    statsInfo += "Honorable Kills: " + _Player.Honor.YesterdayHK + "<br/>";
                    statsInfo += "Honor: " + _Player.Honor.YesterdayHonor + "<br/>";
                    statsInfo += "<h5 class='hnoextraline'>This Week</h5>";
                    statsInfo += "Honorable Kills: " + _Player.Honor.ThisWeekHK + "<br/>";
                    statsInfo += "Honor: " + _Player.Honor.ThisWeekHonor + "<br/>";
                    statsInfo += "<h5 class='hnoextraline'>Last Week</h5>";
                    //statsInfo += "Last weeks Standing: #" + _Player.GetStandingStr() + "<br/>";
                    statsInfo += "Honorable Kills: " + _Player.Honor.LastWeekHK + "<br/>";
                    statsInfo += "Honor: " + _Player.Honor.LastWeekHonor + "<br/>";
                    statsInfo += "<h5 class='hnoextraline'>Lifetime</h5>";
                    statsInfo += "Honorable Kills: " + _Player.Honor.LifetimeHK + "<br/>";
                    statsInfo += "Dishonorable Kills: " + _Player.Honor.LifetimeDK + "<br/>";
                    statsInfo += "Highest Rank: " + StaticValues.ConvertRankVisualWithNr(_Player.Honor.LifetimeHighestRank, playerFaction);
                }
                else if(wowVersion == WowVersionEnum.TBC)
                {
                    statsInfo += "<h5 class='hnoextraline'>Today</h5>";
                    statsInfo += "Honorable Kills: " + _Player.Honor.TodayHK + "<br/>";
                    statsInfo += "Honor: " + _Player.Honor.TodayHonorTBC + "<br/>";
                    statsInfo += "<h5 class='hnoextraline'>Yesterday</h5>";
                    statsInfo += "Honorable Kills: " + _Player.Honor.YesterdayHK + "<br/>";
                    statsInfo += "Honor: " + _Player.Honor.YesterdayHonor + "<br/>";
                    statsInfo += "<h5 class='hnoextraline'>Lifetime</h5>";
                    statsInfo += "Honorable Kills: " + _Player.Honor.LifetimeHK + "<br/>";
                    if(_Player.Arena != null)
                    {
                        Func<PlayerData.ArenaPlayerData, string> GenerateArenaData = (_ArenaData) =>
                        {
                            string data = "";
                            data += "<h5 class='hnoextraline'>" + _ArenaData.TeamName + "</h5>";
                            data += "Rating: " + _ArenaData.TeamRating + "<br/>";
                            data += "Wins: " + _ArenaData.GamesWon + "<br/>";
                            data += "Losses: " + (_ArenaData.GamesPlayed - _ArenaData.GamesWon) + "<br/>";
                            data += "<h5 class='hnoextraline'>Personal</h5>";
                            data += "Matches Played: " + (_ArenaData.PlayerPlayed) + " / " + _ArenaData.GamesPlayed + "<br/>";
                            data += "Rating in Team: " + _ArenaData.PlayerRating + "<br/>";
                            return data;
                        };
                        if(_Player.Arena.Team2v2 != null)
                        {
                            statsInfo += "<h4 class='success'>Arena 2v2</h4>";
                            statsInfo += GenerateArenaData(_Player.Arena.Team2v2);
                        }
                        if (_Player.Arena.Team3v3 != null)
                        {
                            statsInfo += "<h4 class='success'>Arena 3v3</h4>";
                            statsInfo += GenerateArenaData(_Player.Arena.Team3v3);
                        }
                        if (_Player.Arena.Team5v5 != null)
                        {
                            statsInfo += "<h4 class='success'>Arena 5v5</h4>";
                            statsInfo += GenerateArenaData(_Player.Arena.Team5v5);
                        }

                    }
                }
                m_StatsInfoHTML = new MvcHtmlString(statsInfo);
            }
            {
                if (_PlayerHistory != null && _PlayerHistory.HaveValidHistory() == true)
                {
                    var itemSummaryDB = Hidden.ApplicationInstance.Instance.GetItemSummaryDatabase();

                    string receivedItemsInfo = "";
                    var recvItems = HistoryGenerator.GenerateLatestReceivedItems(_PlayerHistory, DateTime.MinValue);

                    var orderedRecvItems = recvItems.OrderByDescending(_Value => _Value.Key);
                    int i = 0;
                    foreach (var recvItem in orderedRecvItems)
                    {
                        if (i++ > 10) break;

                        string itemLinks = GenerateItemIcons(recvItem.Value, _Player.Realm, 3, GenerateItemIconsSetting.None, itemSummaryDB);
                        receivedItemsInfo += PageUtility.CreateTableRow(""
                            , PageUtility.CreateTableColumn(itemLinks)
                            + PageUtility.CreateTableColumn("<div style='overflow: hidden;white-space: nowrap;'>" + recvItem.Key.ToString("yyyy-MM-dd") + "</div>"));
                    }
                    m_ReceivedItemsHTML = new MvcHtmlString(receivedItemsInfo);
                }
            }
        }

        private enum GenerateItemIconsSetting
        {
            None,
            CenterDiv
        }
        private static string GenerateItemIcons(List<PlayerData.ItemInfo> _Items, WowRealm _Realm, int _MaxColumn, GenerateItemIconsSetting _Setting = GenerateItemIconsSetting.None, VF_RPDatabase.ItemSummaryDatabase _ItemSummaryDB = null)
        {
            if (_Items.Count <= 0) return "";
            if (_MaxColumn <= 0) _MaxColumn = 1;
            WowVersionEnum _WowVersion = StaticValues.GetWowVersion(_Realm);
            string currentItemDatabase = DatabaseAccess.GetCurrentItemDatabaseAddress();

            int recvItemIndex = 0;
            int xMax = 58 * _MaxColumn;
            int yMax = (int)((_Items.Count - 1) / _MaxColumn) * 58;

            yMax += 58;
            string itemLinks = "<div class='inventory' style='background: none; width: " + xMax + "px; height: " + yMax + "px;" + (_Setting == GenerateItemIconsSetting.CenterDiv ? "margin-left: auto;margin-right: auto;" : "") + "'>";
            foreach (var item in _Items)
            {
                int xPos = (recvItemIndex % _MaxColumn) * 58;
                int yPos = (int)(recvItemIndex / _MaxColumn) * 58;

                var itemInfo = DatabaseAccess.GetItemInfo(item.ItemID, _WowVersion);
                if (itemInfo != null)
                {
                    itemLinks += "<div style='background: none; width: 58px; height: 58px;margin: " + yPos + "px " + xPos + "px;'>"
                        + "<img class='itempic' src='" + currentItemDatabase + itemInfo.GetIconImageAddress() + "'/>"
                        + "<div class='quality' id='" + CharacterViewer.ItemQualityConversion[itemInfo.ItemQuality] + "'></div>"
                        + "<img class='itemframe' src='assets/img/icons/ItemNormalFrame.png'/>"
                        + GenerateItemLink(currentItemDatabase, item, _WowVersion);

                    if(_ItemSummaryDB != null)
                    {
                        int usageCount = _ItemSummaryDB.GetItemUsageCount(_Realm, item);
                        itemLinks += "<a class='itemplayersframe' href='ItemUsageInfo.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&item=" + item.ItemID + (item.SuffixID != 0 ? "&suffix=" + item.SuffixID : "") + "'>" + usageCount + "</a>";
                    }

                    itemLinks += "</div>";
                }
                else
                {
                    Logger.ConsoleWriteLine("ItemInfo could not be found for ItemID: " + item.ItemID, ConsoleColor.Red);
                }
                ++recvItemIndex;
            }
            itemLinks += "</div>";
            return itemLinks;
        }
        public static Dictionary<ItemSlot, string> ItemSlotToIDConversion = new Dictionary<ItemSlot, string>
        {
            {ItemSlot.Head, "slot-head"},
            {ItemSlot.Neck, "slot-amu"},
            {ItemSlot.Shoulder, "slot-shoulder"},
            {ItemSlot.Back, "slot-back"},
            {ItemSlot.Chest, "slot-chest"},
            {ItemSlot.Shirt, "slot-shirt"},
            {ItemSlot.Tabard, "slot-tabard"},
            {ItemSlot.Wrist, "slot-wrist"},
            {ItemSlot.Gloves, "slot-hands"},
            {ItemSlot.Belt, "slot-waist"},
            {ItemSlot.Legs, "slot-legs"},
            {ItemSlot.Feet, "slot-boots"},
            {ItemSlot.Finger_1, "slot-ring1"},
            {ItemSlot.Finger_2, "slot-ring2"},
            {ItemSlot.Trinket_1, "slot-trinket1"},
            {ItemSlot.Trinket_2, "slot-trinket2"},
            {ItemSlot.Main_Hand, "slot-mainhand"},
            {ItemSlot.Off_Hand, "slot-offhand"},
            {ItemSlot.Ranged, "slot-ranged"},
        };
        public static Dictionary<int, string> ItemQualityConversion = new Dictionary<int, string> 
        {
            {0, "poor"},
            {1, "common"},
            {2, "uncommon"},
            {3, "rare"},
            {4, "epic"},
            {5, "legendary"},
        };

        public static string GenerateItemLink(string _ItemDatabase, int _ItemID, int _SuffixID, WowVersionEnum _WowVersion, string _PiecesStr = "")
        {
            return "<a class='itemlink' href='" + _ItemDatabase + "?item=" + _ItemID + (_WowVersion == WowVersionEnum.TBC ? "-1" : "-0") + (_SuffixID != 0 ? "' rel='rand=" + _SuffixID : "") + "'></a>";
        }
        public static string GenerateItemLink(string _ItemDatabase, PlayerData.ItemInfo _ItemInfo, WowVersionEnum _WowVersion, string _PiecesStr = "")
        {
            if(_ItemInfo == null)
                return GenerateItemLink(_ItemDatabase, 1217, 0, _WowVersion, _PiecesStr);

            string itemLink = "<a class='itemlink' href='"
                + _ItemDatabase + "?item=" + _ItemInfo.ItemID + (_WowVersion == WowVersionEnum.TBC ? "-1" : "-0")
                + "' rel='rand=" + _ItemInfo.SuffixID + (_ItemInfo.SuffixID < 0 ? ";uniq=" + _ItemInfo.UniqueID : "")
                + ";ench=" + _ItemInfo.EnchantID;

            if (_ItemInfo.GemIDs != null)
            {
                string gemIDstring = ";gems=";
                bool anyGems = false;
                for (int i = 0; i < 4; ++i)
                {
                    if (i > 0) gemIDstring += ":"; //Dont add ":" before the first GemID
                    gemIDstring += _ItemInfo.GemIDs[i];
                    if (_ItemInfo.GemIDs[i] != 0)
                        anyGems = true;
                }
                if (anyGems == true)
                {
                    while (gemIDstring.EndsWith(":0"))
                        gemIDstring = gemIDstring.Substring(0, gemIDstring.Length - 2);
                    itemLink += gemIDstring;
                }
            }
            return itemLink +_PiecesStr + "'></a>";
        }
        public string CreateInventoryInfo(Player _Player, PlayerHistory _PlayerHistory, WowRealm _Realm)
        {
            var wowVersion = StaticValues.GetWowVersion(_Player.Realm);
            int gearSetIndex = PageUtility.GetQueryInt(Request, "itemset", -1);

            string currentItemDatabase = DatabaseAccess.GetCurrentItemDatabaseAddress();
            string invInfo = "";

            var gear = _Player.Gear;
            List<PlayerHistory.GearSet> gearSets = null;

            if (gearSetIndex > -1 && _PlayerHistory == null)
                PageUtility.RedirectErrorLoading(this, StaticValues.ConvertRealmParam(_Realm) + "-history");

            if (_PlayerHistory != null && _PlayerHistory.HaveValidHistory() == true)
                gearSets = _PlayerHistory.GetMostCommonGearSets().Where((_Value) => { return (DateTime.UtcNow - _Value.LastUsed).TotalDays < 14; }).ToList();

            if (gearSetIndex > -1)
            {
                try
                {
                    gear = gearSets[gearSetIndex].Gear;
                }
                catch (Exception)
                { }
            }
            if (gear.Items.Count == 0)
            {
                if (gearSets != null)
                {
                    foreach (var gearSet in gearSets)
                    {
                        if (gearSet.Gear.Items.Count != 0)
                        {
                            gear = gearSet.Gear;
                            break;
                        }
                    }
                }
            }
            GenerateGearStats(gear, wowVersion);
            var itemSummaryDB = Hidden.ApplicationInstance.Instance.GetItemSummaryDatabase();
            //var cacheItemsUsed = DatabaseAccess.GetRealmItemsUsed(this, _Realm);

            var designerItemsStr = "";

            string modelEqString = "";
            foreach (var item in gear.Items)
            {
                designerItemsStr += item.Value.Slot.ToString() + "x" + item.Value.ItemID;
                int gemIDcount = item.Value.GetGemIDCount();
                if (item.Value.EnchantID != 0 || item.Value.SuffixID != 0 || item.Value.UniqueID != 0 || gemIDcount != 0)
                {
                    designerItemsStr += "x" + item.Value.EnchantID.ToString();
                    if (item.Value.SuffixID != 0 || item.Value.UniqueID != 0 || gemIDcount != 0)
                        designerItemsStr += "x" + item.Value.SuffixID.ToString();
                    if (item.Value.UniqueID != 0 || gemIDcount != 0)
                        designerItemsStr += "x" + item.Value.UniqueID.ToString();
                    if (gemIDcount != 0)
                    {
                        for (int i = 0; i < gemIDcount; ++i)
                        {
                            designerItemsStr += "x" + item.Value.GemIDs[i].ToString();
                        }
                    }
                }
                designerItemsStr += "+";
                var itemInfo = DatabaseAccess.GetItemInfo(item.Value.ItemID, wowVersion);
                if (itemInfo == null)
                    invInfo += "<div class='equipment-slot' id='" + ItemSlotToIDConversion[item.Key] + "'><div class='quality' id='epic'></div><img src='assets/img/icons/inv_misc_questionmark.png'/></div>";
                else
                {
                    string currInvInfo = "<div class='equipment-slot' id='" + ItemSlotToIDConversion[item.Key] + "'>"
                                + "<img class='itempic' src='" + currentItemDatabase + itemInfo.GetIconImageAddress() + "'/>"
                                + "<div class='quality' id='" + ItemQualityConversion[itemInfo.ItemQuality] + "'></div>"
                                + "<img class='itemframe' src='assets/img/icons/ItemNormalFrame.png'/>"
                                + GenerateItemLink(currentItemDatabase, item.Value, wowVersion, itemInfo.GenerateSetPcsStr(_Player.Gear));

                    if (item.Key == ItemSlot.Head && ModelViewerOptions.HideHead == true)
                    { }
                    else
                    {

                        if ((_Player.Character.Class == VF_RealmPlayersDatabase.PlayerClass.Hunter && (item.Key == ItemSlot.Main_Hand || item.Key == ItemSlot.Off_Hand))
                        || (_Player.Character.Class != VF_RealmPlayersDatabase.PlayerClass.Hunter && item.Key == ItemSlot.Ranged))
                        { }
                        else
                        {
                            int modelViewerID = 0;
                            int modelViewerSlot = 0;
                            if (itemInfo.GetModelViewerIDAndSlot(item.Key, out modelViewerID, out modelViewerSlot, true) == true)
                            {
                                if (item.Key == ItemSlot.Off_Hand)
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
                    int usageCount = itemSummaryDB.GetItemUsageCount(_Realm, item.Value);
                    //if (cacheItemsUsed.TryGetValue(item.Value.ItemID, out players) == true)
                    //{
                    currInvInfo += "<a class='itemplayersframe' href='ItemUsageInfo.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&item=" + item.Value.ItemID + (item.Value.SuffixID != 0 ? "&suffix=" + item.Value.SuffixID : "") + "'>" + usageCount + "</a>";
                    //}
                    currInvInfo += "</div>";
                    invInfo += currInvInfo;
                }
            }
            invInfo += "<img style='position: absolute;z-index: 1;max-width: 555px;' src='assets/img/bg/CharacterBackgroundTransparent.png'></img>";

            m_CharacterDesignerLink = PageUtility.CreateLink("CharacterDesigner.aspx?char=" + _Player.Character.Race + "+" + _Player.Character.Class + "+" + _Player.Character.Sex + "&items=" + designerItemsStr + "&wowversion=" + wowVersion.ToString(), "Character Designer");

            string modelViewerQuery = PageUtility.GetQueryString(Request, "modelviewer", "unknown").ToLower();
            if (modelViewerQuery != "false" && modelEqString != "" && ModelViewerOptions.View3DModel == true)
            {
                if (modelEqString[0] == ',')//För att bli av med det första ","
                    modelEqString = modelEqString.Substring(1);
                //modelEqString = "1,33743,3,33653,5,33650,6,31110,7,31115,8,31111,9,31127,10,33651,13,25629";
                string modelCharString = ((_Player.Character.Race == VF_RealmPlayersDatabase.PlayerRace.Undead) ? "Scourge" : _Player.Character.Race.ToString().Replace("_", "")) + _Player.Character.Sex.ToString();// "orcmale";

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
                invInfo += "<img class='characterimage' src='" + StaticValues.GetRaceCrestImageUrl(_Player.Character.Race) + "'></img>";

            {//Put it in block just for showing its totally seperate
                string itemDropDownOptions = "<option value='-1'" + ((gearSetIndex == -1) ? "selected='selected'" : "") + ">Latest</option>";
                if (gearSets != null && gearSets.Count > 1)
                {
                    for (int i = 0; i < gearSets.Count && i < 7; ++i)
                        itemDropDownOptions += "<option value='" + i + "'" + ((gearSetIndex == i) ? "selected='selected'" : "") + ">ItemSet " + (i + 1) + "</option>";
                }
                m_ItemSetsDropdownOptions = new MvcHtmlString(itemDropDownOptions);
            }//Put it in block just for showing its totally seperate
            Hidden.ApplicationInstance.Instance.BackupItemInfos();
            return invInfo;
        }
        public void GenerateGearStats(PlayerData.GearData _Gear, VF_RealmPlayersDatabase.WowVersionEnum _WowVersion)
        {
            //m_GearStatsHTML
            {
                string gearStatsStr = "<h4 style='width:555px;text-align:center;'>Total Item Stats</h4>";//<div style='width:555px;text-align:center;'><h3>Total Gear Stats</h3></div>
                gearStatsStr += "<table style='width:555px;'><tbody><tr><td style='vertical-align:top'>";
                var gearStats = Code.GearAnalyze.GenerateGearStats(_Gear, _WowVersion);

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
                        case RealmPlayersServer.Code.ItemStatType.Hp5:
                            return 63;
                        case RealmPlayersServer.Code.ItemStatType.Mana:
                            return 70;
                        case RealmPlayersServer.Code.ItemStatType.Mp5:
                            return 80;
                        case RealmPlayersServer.Code.ItemStatType.Defense:
                            return 90;
                        case RealmPlayersServer.Code.ItemStatType.Defense_Rating:
                            return 92;
                        case RealmPlayersServer.Code.ItemStatType.Resilience_Rating:
                            return 95;
                        case RealmPlayersServer.Code.ItemStatType.Block_Chance:
                            return 100;
                        case RealmPlayersServer.Code.ItemStatType.Shield_Block_Rating:
                            return 102;
                        case RealmPlayersServer.Code.ItemStatType.Dodge_Chance:
                            return 120;
                        case RealmPlayersServer.Code.ItemStatType.Dodge_Rating:
                            return 122;
                        case RealmPlayersServer.Code.ItemStatType.Parry_Chance:
                            return 130;
                        case RealmPlayersServer.Code.ItemStatType.Parry_Rating:
                            return 132;
                        case RealmPlayersServer.Code.ItemStatType.Block_Value:
                            return 140;
                        case RealmPlayersServer.Code.ItemStatType.Attack_Power:
                            return 150;
                        case RealmPlayersServer.Code.ItemStatType.Armor_Penetration:
                            return 153;
                        case RealmPlayersServer.Code.ItemStatType.Attack_Speed:
                            return 154;
                        case RealmPlayersServer.Code.ItemStatType.Ranged_Attack_Power:
                            return 155;
                        case RealmPlayersServer.Code.ItemStatType.Haste_Rating:
                            return 156;
                        case RealmPlayersServer.Code.ItemStatType.Crit_Chance:
                            return 160;
                        case RealmPlayersServer.Code.ItemStatType.Critical_Strike_Rating:
                            return 162;
                        case RealmPlayersServer.Code.ItemStatType.Hit_Chance:
                            return 170;
                        case RealmPlayersServer.Code.ItemStatType.Hit_Rating:
                            return 172;
                        case RealmPlayersServer.Code.ItemStatType.Expertise_Rating:
                            return 175;
                        case RealmPlayersServer.Code.ItemStatType.Increased_Critical_Damage:
                            return 178;
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
                        case RealmPlayersServer.Code.ItemStatType.Spell_Critical_Strike_Rating:
                            return 251;
                        case RealmPlayersServer.Code.ItemStatType.Spell_Hit_Chance:
                            return 260;
                        case RealmPlayersServer.Code.ItemStatType.Spell_Hit_Rating:
                            return 261;
                        case RealmPlayersServer.Code.ItemStatType.Spell_Haste_Rating:
                            return 263;
                        case RealmPlayersServer.Code.ItemStatType.Spell_Penetration:
                            return 265;
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
                    if (gearStat.StatType == Code.ItemStatType.Item_Level || gearStat.StatType == Code.ItemStatType.Increased_Critical_Damage)
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
                    if (gearStat.GemValue != 0)
                        gearStatsStr += " + <font color='#00BBBB'>" + gearStat.GemValue + valuePrefix + "</font>";
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
                if (PageUtility.GetQueryString(Request, "TestingGearAnalyze", null) != null)
                {
                    foreach (var item in _Gear.Items)
                    {
                        gearStatsStr += "<br />" + item.Value.Slot.ToString() + "(" + item.Value.ItemID + ")<br />";
                        PlayerData.GearData itemGearData = new PlayerData.GearData();
                        itemGearData.Items.Add(item.Key, item.Value);
                        var itemStats = Code.GearAnalyze.GenerateGearStats(itemGearData, _WowVersion);

                        foreach (var itemStat in itemStats)
                        {
                            string valuePrefix = "";
                            string statTypeCompareStr = itemStat.StatType.ToString().ToLower();
                            if (statTypeCompareStr.Contains("chance"))
                                valuePrefix = "%";
                            else if (statTypeCompareStr.Contains("speed"))
                                valuePrefix = "%";

                            gearStatsStr += itemStat.StatType.ToString().Replace('_', ' ') + ": <font color='#fff'>" + itemStat.NormalValue + valuePrefix + "</font>";
                            if (itemStat.GemValue != 0)
                                gearStatsStr += " + <font color='#00BBBB'>" + itemStat.GemValue + valuePrefix + "</font>";
                            if (itemStat.EnchantValue != 0)
                                gearStatsStr += " + <font color='#00ff00'>" + itemStat.EnchantValue + valuePrefix + "</font>";
                            gearStatsStr += "<br />";
                        }
                    }
                }
                m_GearStatsHTML = new MvcHtmlString(gearStatsStr);
            }
        }
        public void GenerateExtraStats(PlayerExtraData _PlayerExtraData, WowRealm _Realm)
        {
            if (_PlayerExtraData == null)
                return;
            string extraStats = "<table style='width:555px;'><tbody>";
            extraStats += "<tr><td style='vertical-align:top'>";
            int extraDataColumns = 0;
            if (_PlayerExtraData.Mounts.Count > 0) ++extraDataColumns;
            if (_PlayerExtraData.Pets.Count > 0) ++extraDataColumns;
            if (_PlayerExtraData.Companions.Count > 0) ++extraDataColumns;

            var itemSummaryDB = Hidden.ApplicationInstance.Instance.GetItemSummaryDatabase();

            if (_PlayerExtraData.Mounts.Count > 0)
            {
                //Mounts
                string extraMounts = "";
                extraStats += "<h4 style='text-align:center;'>Mounts</h4>";
                List<PlayerData.ItemInfo> mountItems = new List<PlayerData.ItemInfo>();
                foreach (var mount in _PlayerExtraData.Mounts)
                {
                    int itemID = VF.ItemTranslations.FindItemID(mount.Mount);
                    if(itemID > 0)
                    {
                        mountItems.Add(new PlayerData.ItemInfo{ Slot = ItemSlot.Unknown, ItemID = itemID, SuffixID = 0, EnchantID = 0, UniqueID = 0, GemIDs = null});
                    }
                    else
                    {
                        extraMounts += mount.Mount + "<br/>";
                    }
                    //extraStats += mount.Mount + "<br/>";
                }
                int columCount = mountItems.Count > (9 / extraDataColumns) ? (9 / extraDataColumns) : mountItems.Count;
                extraStats += GenerateItemIcons(mountItems, _Realm, columCount, GenerateItemIconsSetting.CenterDiv, itemSummaryDB);
                if (extraMounts != "")
                {
                    extraStats += extraMounts;
                }
            }
            extraStats += "</td>";
            extraStats += "<td style='vertical-align:top'>";
            if (_PlayerExtraData.Pets.Count > 0)
            {
                //Pets
                extraStats += "<h4 style='text-align:center;'>Pets</h4>";
                foreach (var pet in _PlayerExtraData.Pets)
                {
                    extraStats += pet.Name + "(lvl " + pet.Level + " " + pet.CreatureFamily + ")<br/>";
                }
            }
            extraStats += "</td>";
            extraStats += "<td style='vertical-align:top'>";
            if (_PlayerExtraData.Companions.Count > 0)
            {
                //Companions
                string extraCompanions = "";
                extraStats += "<h4 style='text-align:center;'>Companions</h4>";
                List<PlayerData.ItemInfo> companionItems = new List<PlayerData.ItemInfo>();
                foreach (var companion in _PlayerExtraData.Companions)
                {
                    int itemID = VF.ItemTranslations.FindItemID(companion.Name);
                    if (itemID > 0)
                    {
                        companionItems.Add(new PlayerData.ItemInfo { Slot = ItemSlot.Unknown, ItemID = itemID, SuffixID = 0, EnchantID = 0, UniqueID = 0, GemIDs = null });
                    }
                    else
                    {
                        extraCompanions += companion.Name + "<br/>";
                    }
                }
                int columCount = companionItems.Count > (9 / extraDataColumns) ? (9 / extraDataColumns) : companionItems.Count;
                extraStats += GenerateItemIcons(companionItems, _Realm, columCount, GenerateItemIconsSetting.CenterDiv, itemSummaryDB);
                if (extraCompanions != "")
                {
                    extraStats += extraCompanions;
                }
            }
            extraStats += "</td></tr>";
            extraStats += "</tbody></table>";
            m_ExtraDataHTML = new MvcHtmlString(extraStats);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string playerStr = PageUtility.GetQueryString(Request, "player");
            string dateStr = Request.QueryString.Get("date");
            int uploadNR = PageUtility.GetQueryInt(Request, "uploadnumber", -1);
            DateTime date = DateTime.MaxValue;
            if (dateStr != null)
            {
                //"2000L01L01S01C01C01"
                dateStr = dateStr.Replace("L", "-");
                dateStr = dateStr.Replace("S", " ");
                dateStr = dateStr.Replace("C", ":");
                //"2000-01-01 01:01:01"
                if (System.DateTime.TryParse(dateStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AdjustToUniversal, out date) == false)
                    date = DateTime.MaxValue;
            }
            var realm = PageUtility.GetQueryRealm(Request);
            if (realm == VF_RealmPlayersDatabase.WowRealm.Unknown)
                return;

            m_RaidStatsPlayerOverviewLink = PageUtility.CreateLink_RaidStats_Player(playerStr, realm, "Player on RaidStats");

            this.Title = playerStr + " @ " + StaticValues.ConvertRealmParam(realm) + " | RealmPlayers";
            Player player;
            PlayerHistory playerHistory = null;
            if(DatabaseAccess.TryGetRealmPlayersHistory(this, realm) != null)
                playerHistory = DatabaseAccess.FindRealmPlayerHistory(this, realm, playerStr);
            
            string m_PageExtraInfo = "";

            if (date == DateTime.MaxValue && uploadNR != -1)
            {
                date = playerHistory.GetDateAtUploadNr(uploadNR, DateTime.MaxValue);
            }
            if (date == DateTime.MaxValue)
            {
                player = DatabaseAccess.FindRealmPlayer(this, realm, playerStr, NotLoadedDecision.RedirectAndWait);
                if (player == null)
                    return;
            }
            else
            {
                if (playerHistory == null)
                    PageUtility.RedirectErrorLoading(this, StaticValues.ConvertRealmParam(realm) + "-history");
                if (playerHistory.GetPlayerAtTime(playerStr, realm, date, out player) == false)
                    return;
                if(PageUtility.GetQueryString(Request, "gearuploadid") == "debug")
                {
                    var uploader = playerHistory.GetGearItemAtTime(date).Uploader;
                    m_PageExtraInfo = "<br />UploadID(" + uploader.GetContributorID() + ", " + uploader.GetTime().Ticks.ToString() + " " + uploader.GetTime().Kind.ToString() + ")";
                }
            }

            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome() 
                + PageUtility.BreadCrumb_AddRealm(realm)
                + (player.Guild.GuildName != "nil" ? (PageUtility.BreadCrumb_AddGuilds(realm) + PageUtility.BreadCrumb_AddGuild(realm, player.Guild.GuildName)) : "")
                + PageUtility.BreadCrumb_AddFinish(player.Name));
            GenerateCharInfo(player, playerHistory);
            m_InventoryInfoHTML = new MvcHtmlString(CreateInventoryInfo(player, playerHistory, realm));
            GenerateExtraStats(DatabaseAccess.FindRealmPlayerExtraData(this, realm, playerStr, NotLoadedDecision.ReturnNull), realm);
            string chartSection = "";
            if (playerHistory != null && playerHistory.HaveValidHistory() == true)
            {
                string graphStr = PageUtility.GetQueryString(Request, "graph");
                if ((graphStr == "HKs" || graphStr == "Honor"))
                {

                    List<int> chartSectionData = new List<int>();
                    List<DateTime> chartSectionLabels = new List<DateTime>();
                    var honorList = HistoryGenerator.GeneratePlayerHonor(playerHistory);
                    honorList.Reverse();
                    var lastDate = "yyyy-MM-dd";
                    foreach (var honorItem in honorList)
                    {
                        var currDate = honorItem.m_DateTime.ToString("yyyy-MM-dd");
                        if (currDate != lastDate)// || (int)lastBasedOn <= (int)honorItem.m_BasedOn)
                        {
                            lastDate = currDate;

                            if ((honorList.First().m_DateTime - honorItem.m_DateTime).Days < 31)
                            {
                                if (graphStr == "HKs")
                                    chartSectionData.Insert(0, honorItem.m_HKValue);
                                else
                                    chartSectionData.Insert(0, honorItem.m_HonorValue);

                                chartSectionLabels.Insert(0, honorItem.m_DateTime);
                            }
                        }
                    }

                    chartSection += "<script src='/assets/js/charts/raphael-min.js'></script>"
                        + "<script src='/assets/js/charts/popup.js'></script>"
                        + "<script src='/assets/js/charts/chart.js'></script>"
                        + "<script>";

                    string datas = "";
                    string labels = "";
                    string labelDetails = "";
                    var cultureInfo = new System.Globalization.CultureInfo("en-US");
                    foreach (var dateTime in chartSectionLabels)
                    {
                        labelDetails += dateTime.Day + ":' " + dateTime.ToString("y", cultureInfo) + "',";
                        labels += dateTime.Day + ",";
                    }
                    foreach (var hks in chartSectionData)
                    {
                        datas += hks + ",";
                    }
                    chartSection += "g_LabelsDetails = {" + labelDetails + "}; ";
                    chartSection += "InitializeChart("
                        + "[" + labels + "]"
                        + ", [" + datas + "]"
                        + ", 550, 250, 20, 10, 'graphDiv'"
                        + ", function (data) { return data + ' " + graphStr + "'; }"
                        + ", function (lbl) { return lbl + g_LabelsDetails[lbl]; });";

                    chartSection += "</script>"
                        + "<div id='graphDiv'></div>";
                }
                else
                {
                    graphStr = "NoGraph";
                }
                chartSection += "<select style='width: 140px; margin-top: 10px;' onchange='navigateWithNewQuery(\"graph\", this.options[this.selectedIndex].value)'>"
                + "<option value='NoGraph' " + ((graphStr == "NoGraph") ? "selected='selected'" : "") + ">No Graph</option>"
                + "<option value='HKs' " + ((graphStr == "HKs") ? "selected='selected'" : "") + ">HKs</option>"
                + "<option value='Honor' " + ((graphStr == "Honor") ? "selected='selected'" : "") + ">Honor</option>"
                + "</select>";
            }
            m_ChartSection = new MvcHtmlString(chartSection + m_PageExtraInfo);
        }
    }
}