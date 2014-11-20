using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;

namespace RealmPlayersServer
{
    public partial class ItemUsageInfo : System.Web.UI.Page
    {
        static PlayerColumn[] Table_Columns = new PlayerColumn[]{
            PlayerColumn.Number,
            PlayerColumn.Character_And_Guild,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.LastSeen,
        };
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_ItemUsageInfoHTML = null;
        public MvcHtmlString m_TableHeadHTML = null;
        public MvcHtmlString m_TableBodyHTML = null;
        public MvcHtmlString m_PaginationHTML = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            int pageNr = PageUtility.GetQueryInt(Request, "page", 1);
            int pageIndex = pageNr - 1;//Change range from 0 to * instead of 1 to *
            int count = PageUtility.GetQueryInt(Request, "count", 100);
            if (count > 500) count = 500;

            var realm = RealmControl.Realm;
            if (realm == VF_RealmPlayersDatabase.WowRealm.Unknown)
                return;
            var wowVersion = StaticValues.GetWowVersion(realm);
            int itemID = PageUtility.GetQueryInt(Request, "item", 19364);
            int suffixID = PageUtility.GetQueryInt(Request, "suffix", 0);
            this.Title = "\"" + itemID + "\" ItemUsage @ " + StaticValues.ConvertRealmParam(realm) + " | RealmPlayers";

            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                + PageUtility.BreadCrumb_AddFinish("ItemUsage"));

            var itemSummaryDB = Hidden.ApplicationInstance.Instance.GetItemSummaryDatabase();

            List<Tuple<DateTime, string>> players = null;
            players = itemSummaryDB.GetItemUsage(realm, itemID, suffixID);//, out players) == true)
            {
                string currentItemDatabase = DatabaseAccess.GetCurrentItemDatabaseAddress();

                var itemInfo = DatabaseAccess.GetItemInfo(itemID, wowVersion);
                m_ItemUsageInfoHTML = new MvcHtmlString("<div style='overflow: hidden; display: table;'><div style='display: table-cell;'><h1>Players with&nbsp;<h1></div>"
                    + "<div class='inventory' style='background: none; width: 58px; height: 58px; display: table-cell;'><div>"
                            + "<img class='itempic' src='" + currentItemDatabase + itemInfo.GetIconImageAddress() + "'/>"
                            + "<div class='quality' id='" + CharacterViewer.ItemQualityConversion[itemInfo.ItemQuality] + "'></div>"
                            + "<img class='itemframe' src='assets/img/icons/ItemNormalFrame.png'/>"
                            + CharacterViewer.GenerateItemLink(currentItemDatabase, itemID, suffixID, wowVersion)
                            + "</div></div>"
                    + "<span class='badge badge-inverse'>" + players.Count + " Players</span></div>" + "<p>Sorted by date \"aquired\"</p>");

                Dictionary<PlayerColumn, string[]> extraColumns = new Dictionary<PlayerColumn, string[]>();
                PlayerColumn ItemAndAquiredDateAfterColumn = PlayerColumn.Number;
                extraColumns.Add(ItemAndAquiredDateAfterColumn, new string[] { "Item", "Date Aquired" });
                m_TableHeadHTML = new MvcHtmlString(PageUtility.CreatePlayerTableHeaderRow(Table_Columns, extraColumns));

                string tableBody = "";
                int nr = 0;
                Func<Tuple<DateTime, string>, DateTime> lambdaComparison = (_Tuple) => { return _Tuple.Item1; };
                players = players.OrderBy(lambdaComparison).ToList();
                foreach (var playerDateAndName in players)
                {
                    Player player = DatabaseAccess.FindRealmPlayer(this, realm, playerDateAndName.Item2);
                    if (player != null)
                    {
                        //string itemLink = "";
                        VF_RealmPlayersDatabase.PlayerData.ItemInfo playerItemData = null;
                        try
                        {
                            playerItemData = player.Gear.Items.First((_Item) => { return _Item.Value.ItemID == itemID; }).Value;
                        }
                        catch (Exception)
                        {
                            playerItemData = null;
                        }
                        if (playerItemData != null)
                        {
                            //Data fanns i gearen som playern använder för tillfället!
                            //itemLink = currentItemDatabase + "?item=" + playerItemData.ItemID + "' rel='rand=" + playerItemData.SuffixID + ";ench=" + playerItemData.EnchantID;
                        }
                        else
                        {
                            //Måste titta igenom history!
                            PlayerHistory playerHistory = DatabaseAccess.FindRealmPlayerHistory(this, realm, playerDateAndName.Item2);
                            if (playerHistory != null)
                            {
                                for (int i = playerHistory.GearHistory.Count - 1; i >= 0; --i)
                                {
                                    try
                                    {
                                        playerItemData = playerHistory.GearHistory[i].Data.Items.First((_Item) => { return _Item.Value.ItemID == itemID; }).Value;
                                    }
                                    catch (Exception)
                                    {
                                        playerItemData = null;
                                    }
                                    if (playerItemData != null)
                                    {
                                        //Data fanns i gearen som playern använder för tillfället!
                                        //itemLink = currentItemDatabase + "?item=" + playerItemData.ItemID + "' rel='rand=" + playerItemData.SuffixID + ";ench=" + playerItemData.EnchantID;
                                        break;
                                    }
                                }
                            }
                        }
                        ++nr;
                        // style='position: absolute; margin: auto; width:58px; height:58px;'
                        if (nr > pageIndex * count && nr <= (pageIndex + 1) * count)
                        {
                            extraColumns[ItemAndAquiredDateAfterColumn] = new string[]{
                                "<div class='inventory' style='background: none; width: 58px; height: 58px;'><div>"
                                + "<img class='itempic' src='" + currentItemDatabase + itemInfo.GetIconImageAddress() + "'/>"
                                + "<div class='quality' id='" + CharacterViewer.ItemQualityConversion[itemInfo.ItemQuality] + "'></div>"
                                + "<img class='itemframe' src='assets/img/icons/ItemNormalFrame.png'/>"
                                + CharacterViewer.GenerateItemLink(currentItemDatabase, playerItemData, wowVersion)
                                + "</div></div>"
                                , playerDateAndName.Item1.ToString("yyyy-MM-dd")
                            };
                            tableBody += PageUtility.CreatePlayerRow(nr, realm, player, Table_Columns, null, extraColumns);
                        }
                        if (nr >= (pageIndex + 1) * count)
                            break;
                    }
                }
                if (nr != 0 && nr <= pageIndex * count)
                {
                    pageIndex = (nr - 1) / count;
                    Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "page", (pageIndex + 1).ToString()));
                }
                m_TableBodyHTML = new MvcHtmlString(tableBody);

                int maxPageNr = 1000000 / count;
                m_PaginationHTML = new MvcHtmlString(PageUtility.CreatePagination(Request, pageNr, ((players.Count - 1) / count) + 1));
            }

        }
    }
}