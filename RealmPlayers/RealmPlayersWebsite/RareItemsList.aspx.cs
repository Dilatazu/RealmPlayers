using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;
using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;

namespace RealmPlayersServer
{
    public partial class RareItemsList : System.Web.UI.Page
    {
        static PlayerColumn[] Table_Columns = new PlayerColumn[]{
            PlayerColumn.Number,
        };
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_ItemUsageInfoHTML = null;
        public MvcHtmlString m_TableHeadHTML = null;
        public MvcHtmlString m_TableBodyHTML = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                + PageUtility.BreadCrumb_AddFinish("RareItemsList"));

            var realm = RealmControl.Realm;
            if (realm == WowRealm.Unknown) return;
            var wowVersion = StaticValues.GetWowVersion(realm);

            int count = PageUtility.GetQueryInt(Request, "count", 100);

            var itemSummaryDB = Hidden.ApplicationInstance.Instance.GetItemSummaryDatabase();
                
            m_ItemUsageInfoHTML = new MvcHtmlString("<h1>Rarest items<span class='badge badge-inverse'>" + count + " Items</span></h1>"
            + "<p>List contains the most rarest items on the realm.</p>");

            Dictionary<PlayerColumn, string[]> extraColumns = new Dictionary<PlayerColumn, string[]>();
            PlayerColumn ItemAndAquiredDateAfterColumn = PlayerColumn.Number;
            extraColumns.Add(ItemAndAquiredDateAfterColumn, new string[] { "Item", "Count", "First Aquired", "First Player(s)" });
            m_TableHeadHTML = new MvcHtmlString(PageUtility.CreatePlayerTableHeaderRow(Table_Columns, extraColumns));

            string currentItemDatabase = DatabaseAccess.GetCurrentItemDatabaseAddress();

            string tableBody = "";
            int nr = 1;
            Func<KeyValuePair<int, List<Tuple<DateTime, string>>>, int> lambdaComparison = (_Tuple) => { return _Tuple.Value.Count; };
                Func<Tuple<DateTime, string>, DateTime> lambdaDateTimeComparison = (_Tuple) => { return _Tuple.Item1; };
            var rareItemsList = new Dictionary<int, List<Tuple<DateTime, string>>>();//itemSummaryDB.OrderBy(lambdaComparison);
            foreach (var rareItem in rareItemsList)
            {
                if (nr >= count)
                    break;
                if(rareItem.Value.Count > 0 && Code.Resources.ItemAnalyzer.IsRareItem(rareItem.Key) == true)
                {
                    var itemInfo = DatabaseAccess.GetItemInfo(rareItem.Key, wowVersion);
                    if (itemInfo.ItemQuality < 4)
                        continue;
                    //rareItem.
                    Player player = null;
                    var orderedAquires = rareItem.Value.OrderBy(lambdaDateTimeComparison);
                    DateTime dateTimeCutoff = orderedAquires.First().Item1;
                    if (dateTimeCutoff < new DateTime(2013, 8, 1, 0, 0, 0))
                        dateTimeCutoff = new DateTime(2013, 8, 1, 0, 0, 0);
                    else
                        dateTimeCutoff = dateTimeCutoff.AddDays(3);

                    string firstPlayers = "<div style='overflow: hidden; display: table; height: 58px;'>";
                    if (orderedAquires.Count((_Tuple) => { return _Tuple.Item1 < dateTimeCutoff; }) < 5)
                    {
                        foreach (var playerAquire in orderedAquires)
                        {
                            if (playerAquire.Item1 < dateTimeCutoff)
                            {
                                player = DatabaseAccess.FindRealmPlayer(this, realm, playerAquire.Item2);
                                if (player != null)
                                {
                                    var playerFaction = StaticValues.GetFaction(player.Character.Race);
                                    firstPlayers += "<div class='" + StaticValues.GetFactionCSSName(playerFaction) + "_col' style='display: table-cell; width:160px;" /*"background-color: " + (playerFaction == PlayerFaction.Horde ? "#372727" : "#272f37")*/ + "'>";
                                    firstPlayers += "<p style='text-align:center;'><a href='CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&player=" + System.Web.HttpUtility.HtmlEncode(player.Name) + "'>" + PageUtility.CreateColorCodedName(player.Name, player.Character.Class) + "</a>";
                                    if (player.Guild.GuildName != "nil")
                                    {
                                        string visualGuildName = PageUtility.BreakLongStr(player.Guild.GuildName, 16, 8);
                                        firstPlayers += "<br>" + PageUtility.CreateLink("GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&guild=" + System.Web.HttpUtility.HtmlEncode(player.Guild.GuildName), "&lt;" + visualGuildName + "&gt;") + "</p>";
                                    }
                                    firstPlayers += "</div>";
                                }
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        firstPlayers += "Data not detailed enough<br>(more than 5 players within aquire timespan)";
                    }
                    firstPlayers += "</div>";
                    extraColumns[ItemAndAquiredDateAfterColumn] = new string[]{
                                "<div class='inventory' style='background: none; width: 58px; height: 58px;'><div>"
                                + "<img class='itempic' src='" + currentItemDatabase + itemInfo.GetIconImageAddress() + "'/>"
                                + "<div class='quality' id='" + CharacterViewer.ItemQualityConversion[itemInfo.ItemQuality] + "'></div>"
                                + "<img class='itemframe' src='assets/img/icons/ItemNormalFrame.png'/>"
                                + "<a class='itemlink' href='" + currentItemDatabase + "?item=" + rareItem.Key + "'></a>"
                                + "</div></div>"
                            , rareItem.Value.Count.ToString()
                            , " &gt; " + orderedAquires.First().Item1.ToString("yyyy-MM-dd") + "<br> &lt; " + dateTimeCutoff.ToString("yyyy-MM-dd")
                            , firstPlayers
                        };
                    tableBody += PageUtility.CreateTableRow("",
                        PageUtility.CreateTableColumn("#" + (nr++))
                        + PageUtility.CreateTableColumn(extraColumns[ItemAndAquiredDateAfterColumn][0])
                        + PageUtility.CreateTableColumn(extraColumns[ItemAndAquiredDateAfterColumn][1])
                        + PageUtility.CreateTableColumn(extraColumns[ItemAndAquiredDateAfterColumn][2])
                        + PageUtility.CreateTableColumn(extraColumns[ItemAndAquiredDateAfterColumn][3]));
                }
            }
            m_TableBodyHTML = new MvcHtmlString(tableBody);

        }
    }
}