using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using Guild = VF_RealmPlayersDatabase.GeneratedData.Guild;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;
using PlayerItemInfo = VF_RealmPlayersDatabase.PlayerData.ItemInfo;

namespace RealmPlayersServer
{
    public partial class GuildViewer : System.Web.UI.Page
    {
        static PlayerColumn[] Table_Columns = new PlayerColumn[]{
            PlayerColumn.Rank,
            PlayerColumn.ClassColoredName,
            PlayerColumn.GuildRank,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.Total_HKs,
            PlayerColumn.HonorLastWeek,
            PlayerColumn.LastSeen,
        };
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_GuildInfoHTML = null;
        public MvcHtmlString m_ProgressInfoHTML = null;
        public MvcHtmlString m_CharactersTableHeadHTML = null;
        public MvcHtmlString m_CharactersTableBodyHTML = null;
        public MvcHtmlString m_GuildScriptData = null;

        public MvcHtmlString m_LatestItemsHTML = null;
        public MvcHtmlString m_LatestMembersHTML = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            string pageView = RealmPlayersServer.PageUtility.GetQueryString(Request, "view", "players");
            var realm = PageUtility.GetQueryRealm(Request);
            if (realm == WowRealm.Unknown) return;
            var wowVersion = StaticValues.GetWowVersion(realm);

            string guildStr = PageUtility.GetQueryString(Request, "guild");

            var realmDB = DatabaseAccess.GetRealmPlayers(this, realm);
            var guildSummaryDB = Hidden.ApplicationInstance.Instance.GetGuildSummaryDatabase();
            var guild = guildSummaryDB.GetGuildSummary(realm, guildStr);// DatabaseAccess.GetRealmGuild(this, realm, guildStr, NotLoadedDecision.RedirectAndWait);
            if (guild == null)
                return;
            guild.GenerateCache(realmDB);

            this.Title = guildStr + " @ " + StaticValues.ConvertRealmParam(realm) + " | RealmPlayers";

            m_ProgressInfoHTML = new MvcHtmlString("<div id='" + guild.GuildName.Replace(' ', '_') + "-Progress'></div>");

            var membersArray = guild.GetMembers();
            var orderedPlayersArray = membersArray.OrderBy((player) => { return player.Item2.History.Last().GuildRankNr; });
            int progressComparisonValue = 0;
            string guildProgressData = GuildList.CreateProgressStr(this, guild, realm, out progressComparisonValue);
            m_GuildScriptData = new MvcHtmlString("<script>var guildProgress = new Array();" + guildProgressData + "</script>");

            if (pageView == "players")
            {
                m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm)
                    + PageUtility.BreadCrumb_AddGuilds(realm)
                    + PageUtility.BreadCrumb_AddFinish(guild.GuildName));

                m_GuildInfoHTML = new MvcHtmlString("<h1><img src='assets/img/icons/ui-pvp-"
                    + StaticValues.GetFactionCSSName(guild.Faction) + ".png'/><span class='"
                    + StaticValues.GetFactionCSSName(guild.Faction) + "-color'>" + guild.GuildName
                    + "</span><span class='badge badge-inverse'>" + membersArray.Count + " Members</span></h1>"
                    + "<p>Guild on the server " + StaticValues.ConvertRealmViewing(realm) + "</p>"
                    + "<p><a href='/GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&guild=" + guildStr + "&view=latestevents'>View Latest Events</a></p>"
                    + "<p><a href='/RaidStats/RaidList.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&guild=" + guildStr + "'>View recorded raids on RaidStats</a></p>");

                string page = "";
                foreach (var player in orderedPlayersArray)
                {
                    page += PageUtility.CreatePlayerRow(0, realm, player.Item1, Table_Columns);
                }
                m_CharactersTableHeadHTML = new MvcHtmlString(PageUtility.CreatePlayerTableHeaderRow(Table_Columns));
                m_CharactersTableBodyHTML = new MvcHtmlString(page);
            }
            else if (pageView == "latestmembers")
            {
                m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm)
                    + PageUtility.BreadCrumb_AddGuilds(realm)
                    + PageUtility.BreadCrumb_AddGuild(realm, guild.GuildName)
                    + PageUtility.BreadCrumb_AddFinish("Latest Members"));

                m_GuildInfoHTML = new MvcHtmlString("<h1><img src='assets/img/icons/ui-pvp-"
                    + StaticValues.GetFactionCSSName(guild.Faction) + ".png'/><span class='"
                    + StaticValues.GetFactionCSSName(guild.Faction) + "-color'>" + guild.GuildName
                    + "</span><span class='badge badge-inverse'>" + membersArray.Count + " Members</span></h1>"
                    + "<p>Guild on the server " + StaticValues.ConvertRealmViewing(realm) + "</p>"
                    + "<p><a href='/GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&guild=" + guildStr + "'>View Members</a></p>"
                    + "<p><a href='/RaidStats/RaidList.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&guild=" + guildStr + "'>View recorded raids on RaidStats</a></p>");
            }
            else if (pageView == "latestitems")
            {
                m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm)
                    + PageUtility.BreadCrumb_AddGuilds(realm)
                    + PageUtility.BreadCrumb_AddGuild(realm, guild.GuildName)
                    + PageUtility.BreadCrumb_AddFinish("Latest Items"));

                m_GuildInfoHTML = new MvcHtmlString("<h1><img src='assets/img/icons/ui-pvp-"
                    + StaticValues.GetFactionCSSName(guild.Faction) + ".png'/><span class='"
                    + StaticValues.GetFactionCSSName(guild.Faction) + "-color'>" + guild.GuildName
                    + "</span><span class='badge badge-inverse'>" + membersArray.Count + " Members</span></h1>"
                    + "<p>Guild on the server " + StaticValues.ConvertRealmViewing(realm) + "</p>"
                    + "<p><a href='/GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&guild=" + guildStr + "'>View Members</a></p>"
                    + "<p><a href='/RaidStats/RaidList.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&guild=" + guildStr + "'>View recorded raids on RaidStats</a></p>");
            }
            else if (pageView == "latestevents")
            {
                m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm)
                    + PageUtility.BreadCrumb_AddGuilds(realm)
                    + PageUtility.BreadCrumb_AddGuild(realm, guild.GuildName)
                    + PageUtility.BreadCrumb_AddFinish("Latest Events"));

                m_GuildInfoHTML = new MvcHtmlString("<h1><img src='assets/img/icons/ui-pvp-"
                    + StaticValues.GetFactionCSSName(guild.Faction) + ".png'/><span class='"
                    + StaticValues.GetFactionCSSName(guild.Faction) + "-color'>" + guild.GuildName
                    + "</span><span class='badge badge-inverse'>" + membersArray.Count + " Members</span></h1>"
                    + "<p>Guild on the server " + StaticValues.ConvertRealmViewing(realm) + "</p>"
                    + "<p><a href='/GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&guild=" + guildStr + "'>View Members</a></p>"
                    + "<p><a href='/RaidStats/RaidList.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&guild=" + guildStr + "'>View recorded raids on RaidStats</a></p>");
            }

            if (pageView == "latestmembers" || pageView == "latestevents")
            {
                var guildSummary = Hidden.ApplicationInstance.Instance.GetGuildSummary(realm, guildStr);
                if (guildSummary == null)
                    return;
                
                var statusChanges = guildSummary.GenerateLatestStatusChanges(DateTime.UtcNow.AddDays(-14));

                string latestmembersStr = "";
                statusChanges.Reverse();
                foreach (var sChange in statusChanges)
                {
                    if (realmDB.ContainsKey(sChange.Player))
                    {
                        var playerData = realmDB[sChange.Player];

                        string statusChangeDescription = "";
                        if (sChange.FromStatus == null || sChange.FromStatus.IsInGuild == false)
                        {
                            if (sChange.ToStatus.IsInGuild == true)
                                statusChangeDescription = "Added to the Guild as Rank \"" + sChange.ToStatus.GuildRank + "\"";
                            else
                                continue;
                        }
                        else if (sChange.ToStatus.IsInGuild == false)
                        {
                            statusChangeDescription = "Removed from the Guild";
                        }
                        else
                        {
                            statusChangeDescription = "Changed Rank From \"" + sChange.FromStatus.GuildRank + "\" to \"" + sChange.ToStatus.GuildRank + "\"";
                        }

                        latestmembersStr += PageUtility.CreateTableRow(""
                            , PageUtility.CreateTableColumn(PageUtility.CreateColorCodedPlayerLink(realm, playerData))
                            + PageUtility.CreateTableColumn("<div style='overflow: hidden;white-space: nowrap;'>" + statusChangeDescription + "</div>")
                            + PageUtility.CreateTableColumn("<div style='overflow: hidden;white-space: nowrap;'>" + sChange.ToStatus.DateTime.ToLocalTime().ToString("yyyy-MM-dd") + "</div>"));
                    }
                }
                m_LatestMembersHTML = new MvcHtmlString(latestmembersStr);
            }

            if (pageView == "latestitems" || pageView == "latestevents")
            {
                List<Tuple<DateTime, Player, List<Tuple<PlayerItemInfo, ItemInfo>>>> latestItems = new List<Tuple<DateTime, Player, List<Tuple<PlayerItemInfo, ItemInfo>>>>();

                var realmHistory = DatabaseAccess._GetRealmPlayersHistory(this, realm, NotLoadedDecision.SpinWait);
                foreach (var data in orderedPlayersArray)
                {
                    Player player = data.Item1;
                    PlayerHistory playerHistory = null;
                    if (realmHistory.TryGetValue(player.Name, out playerHistory) == true)
                    {
                        var recvItems = HistoryGenerator.GenerateLatestReceivedItems(playerHistory, DateTime.MinValue);

                        var orderedRecvItems = recvItems.OrderByDescending(_Value => _Value.Key);
                        int i = 0;
                        foreach (var recvItem in orderedRecvItems)
                        {
                            if (i++ > 10) break;

                            List<Tuple<PlayerItemInfo, ItemInfo>> newestItems = new List<Tuple<PlayerItemInfo, ItemInfo>>();
                            foreach (var item in recvItem.Value)
                            {
                                var itemInfo = DatabaseAccess.GetItemInfo(item.ItemID, wowVersion);
                                if (itemInfo.ItemQuality >= 4) //Atleast epic(4)
                                {
                                    newestItems.Add(Tuple.Create(item, itemInfo));
                                }
                            }
                            if (newestItems.Count > 0)
                                latestItems.Add(Tuple.Create(recvItem.Key, player, newestItems));
                        }
                    }
                }

                string latestItemsStr = "";
                {
                    var itemSummaryDB = Hidden.ApplicationInstance.Instance.GetItemSummaryDatabase();

                    string currentItemDatabase = DatabaseAccess.GetCurrentItemDatabaseAddress();

                    var orderedLatestItems = latestItems.OrderByDescending((_Value) => _Value.Item1);

                    int i = 0;
                    foreach (var recvItem in orderedLatestItems)
                    {
                        if (i++ > 100) break;

                        int recvItemIndex = 0;
                        int xMax = 58 * 3;
                        int yMax = (int)((recvItem.Item3.Count - 1) / 3) * 58;

                        yMax += 58;
                        string itemLinks = "<div class='inventory' style='background: none; width: " + xMax + "px; height: " + yMax + "px;'>";
                        foreach (var item in recvItem.Item3)
                        {
                            int xPos = (recvItemIndex % 3) * 58;
                            int yPos = (int)(recvItemIndex / 3) * 58;
                            //string itemLink = currentItemDatabase + "?item=" + item.Item1.ItemID + "' rel='rand=" + item.Item1.SuffixID + ";ench=0";

                            var itemInfo = item.Item2;
                            itemLinks += "<div style='background: none; width: 58px; height: 58px;margin: " + yPos + "px " + xPos + "px;'>"
                                + "<img class='itempic' src='" + "http://realmplayers.com/" + itemInfo.GetIconImageAddress() + "'/>"
                                + "<div class='quality' id='" + CharacterViewer.ItemQualityConversion[itemInfo.ItemQuality] + "'></div>"
                                + "<img class='itemframe' src='assets/img/icons/ItemNormalFrame.png'/>"
                                + CharacterViewer.GenerateItemLink(currentItemDatabase, item.Item1, wowVersion);

                            var itemUsageCount = itemSummaryDB.GetItemUsageCount(realm, item.Item1);
                            itemLinks += "<a class='itemplayersframe' href='ItemUsageInfo.aspx?realm=" + StaticValues.ConvertRealmParam(realm) + "&item=" + item.Item1.ItemID + (item.Item1.SuffixID != 0 ? "&suffix=" + item.Item1.SuffixID : "") + "'>" + itemUsageCount + "</a>";
                            itemLinks += "</div>";
                            ++recvItemIndex;
                        }

                        itemLinks += "</div>";
                        latestItemsStr += PageUtility.CreateTableRow(""
                            , PageUtility.CreateTableColumn(itemLinks)
                            + PageUtility.CreateTableColumn(PageUtility.CreateColorCodedPlayerLink(realm, recvItem.Item2))
                            + PageUtility.CreateTableColumn("<div style='overflow: hidden;white-space: nowrap;'>" + recvItem.Item1.ToString("yyyy-MM-dd") + "</div>"));
                    }
                }
                m_LatestItemsHTML = new MvcHtmlString(latestItemsStr);
            }
        }
    }
}