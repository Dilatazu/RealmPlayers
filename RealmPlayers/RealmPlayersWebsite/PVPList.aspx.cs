using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;
namespace RealmPlayersServer
{
    public partial class PVPList : System.Web.UI.Page
    {
        static PlayerColumn[] Table_Standings_Columns = new PlayerColumn[]{
            PlayerColumn.Standing,
            PlayerColumn.Rank,
            PlayerColumn.Character_And_Guild,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.HonorLastWeek,
            PlayerColumn.HKLastWeek,
            PlayerColumn.LastSeen,
        };
        static PlayerColumn[] Table_Ranks_Columns = new PlayerColumn[]{
            PlayerColumn.Number,
            PlayerColumn.Rank,
            PlayerColumn.Character_And_Guild,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.Total_HKs,
            PlayerColumn.RankChange,
            PlayerColumn.HonorLastWeek,
            PlayerColumn.HKLastWeek,
            PlayerColumn.LastSeen,
        };
        static PlayerColumn[] Table_Ranks_Columns_TBC = new PlayerColumn[]{
            PlayerColumn.Number,
            PlayerColumn.Character_And_Guild,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.Total_HKs,
            PlayerColumn.Rating_2v2,
            PlayerColumn.Rating_3v3,
            PlayerColumn.Rating_5v5,
            PlayerColumn.LastSeen,
        };
        static PlayerColumn[] Table_Twink_Ranks_Columns = new PlayerColumn[]{
            PlayerColumn.Number,
            PlayerColumn.Rank,
            PlayerColumn.Character_And_Guild,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.Total_HKs,
            PlayerColumn.RankChange,
            PlayerColumn.HonorLastWeek,
            PlayerColumn.HKLastWeek,
            PlayerColumn.LastSeen,
        };
        static PlayerColumn[] Table_Lifetime_Kills_Columns = new PlayerColumn[]{
            PlayerColumn.Number,
            PlayerColumn.Rank,
            PlayerColumn.Character_And_Guild,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.Total_HKs,
            PlayerColumn.HonorLastWeek,
            PlayerColumn.HKLastWeek,
            PlayerColumn.LastSeen,
        };
        static PlayerColumn[] Table_Lifetime_Kills_Columns_TBC = new PlayerColumn[]{
            PlayerColumn.Number,
            PlayerColumn.Character_And_Guild,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.Total_HKs,
            PlayerColumn.Rating_2v2,
            PlayerColumn.Rating_3v3,
            PlayerColumn.Rating_5v5,
            PlayerColumn.LastSeen,
        };
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_CharListInfoHTML = null;
        public MvcHtmlString m_PageHTML = null;
        public MvcHtmlString m_PaginationHTML = null;

        public void GeneratePageDetails(string _PVPSection, int _CharCount, string _PageExplanation = "Page explanation will be here.")
        {
            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                + PageUtility.BreadCrumb_AddRealm(RealmControl.Realm)
                + PageUtility.BreadCrumb_AddFinish(_PVPSection));

            string charListInfo
                = "<h1>" + _PVPSection /*+ "<span class='badge badge-inverse'>" + _CharCount + " Characters</span>"*/
                + "</h1><p>" + _PageExplanation + "</p>";

            m_CharListInfoHTML = new MvcHtmlString(charListInfo);
        }
        public string CreateStandingsTable(WowRealm _Realm, int _Count, Func<Player, bool> _RequirementLambda)
        {
            int pageIndex = PageUtility.GetQueryInt(Request, "page", 1) - 1;//Change range from 0 to * instead of 1 to *

            string commonTable = "<table id='characters-table' class='table'>";
            commonTable += "<thead>" + PageUtility.CreatePlayerTableHeaderRow(Table_Standings_Columns) + "</thead>";
            string hordeTable = commonTable + "<tbody>";
            string allianceTable = commonTable + "<tbody>";
            Func<KeyValuePair<string, Player>, double> sortLambda = (KeyValuePair<string, Player> player) =>
            {
                double daysSinceLastSeen = (DateTime.UtcNow - player.Value.LastSeen).TotalDays;
                if (daysSinceLastSeen > 7)
                    return int.MaxValue;
                return (double)player.Value.Honor.LastWeekStanding + (daysSinceLastSeen / 10.0);
            };
            var playerArray = DatabaseAccess.GetRealmPlayers(this, _Realm, NotLoadedDecision.RedirectAndWait).OrderBy(sortLambda);

            DateTime lastRankUpdateDateUTC = StaticValues.CalculateLastRankUpdadeDateUTC(_Realm);
           
            int hordeCount = 0;
            int allianceCount = 0;
            foreach (var player in playerArray)
            {
                if ((player.Value.LastSeen - lastRankUpdateDateUTC).TotalHours > 0)
                {
                    var playerFaction = StaticValues.GetFaction(player.Value.Character.Race);
                    if (playerFaction == VF_RealmPlayersDatabase.PlayerFaction.Horde
                        && hordeCount < (pageIndex + 1) * _Count
                        && _RequirementLambda(player.Value) == true)
                    {
                        hordeCount++;
                        if (hordeCount > pageIndex * _Count && hordeCount <= (pageIndex + 1) * _Count)
                        {
                            hordeTable += PageUtility.CreatePlayerRow(hordeCount, _Realm, player.Value, Table_Standings_Columns);
                        }
                    }
                    else if (playerFaction == VF_RealmPlayersDatabase.PlayerFaction.Alliance
                        && allianceCount < (pageIndex + 1) * _Count
                        && _RequirementLambda(player.Value) == true)
                    {
                        allianceCount++;
                        if (allianceCount > pageIndex * _Count && allianceCount <= (pageIndex + 1) * _Count)
                        {
                            allianceTable += PageUtility.CreatePlayerRow(allianceCount, _Realm, player.Value, Table_Standings_Columns);
                        }
                    }
                }
                if (hordeCount >= (pageIndex + 1) * _Count && allianceCount >= (pageIndex + 1) * _Count)
                    break;
            }
            hordeTable += "</tbody></table>";
            allianceTable += "</tbody></table>";
            string pageTable = "";
            pageTable += PageUtility.CreateDiv("span6", hordeTable);
            pageTable += PageUtility.CreateDiv("span6", allianceTable);
            pageTable = PageUtility.CreateDiv("row", pageTable);

            if ((hordeCount != 0 && hordeCount < pageIndex * _Count) && (allianceCount != 0 && allianceCount < pageIndex * _Count))
            {
                pageIndex = (hordeCount - 1) / _Count;
                if (pageIndex < (allianceCount - 1) / _Count)
                    pageIndex = (allianceCount - 1) / _Count;
                Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "page", (pageIndex + 1).ToString()));
            }
            return pageTable;
        }
        public string CreatePVPTable(WowRealm _Realm, int _Count, PlayerColumn[] _Columns, Func<KeyValuePair<string, Player>, double> _SortLambda, Func<Player, bool> _RequirementLambda)
        {
            int pageIndex = PageUtility.GetQueryInt(Request, "page", 1) - 1;//Change range from 0 to * instead of 1 to *

            bool needHistory = false;
            if(_Columns.Contains(PlayerColumn.RankChange))
                needHistory = true;

            string table = "<table id='characters-table' class='table'>";
            table += "<thead>" + PageUtility.CreatePlayerTableHeaderRow(_Columns) + "</thead>";
            table += "<tbody>";

            var playerArray = DatabaseAccess.GetRealmPlayers(this, _Realm, NotLoadedDecision.RedirectAndWait).OrderByDescending(_SortLambda);
            var playersHistoryArray = DatabaseAccess.GetRealmPlayersHistory(this, _Realm, NotLoadedDecision.ReturnNull);
            int nr = 0;
            foreach (var player in playerArray)
            {
                if (_RequirementLambda(player.Value) == true)
                {
                    nr++;
                    if (nr > pageIndex * _Count && nr <= (pageIndex + 1) * _Count)
                    {
                        if (needHistory == true)
                        {
                            VF_RealmPlayersDatabase.PlayerData.PlayerHistory playerHistory = null;
                            if (playersHistoryArray != null)
                            {
                                if (playersHistoryArray.TryGetValue(player.Key, out playerHistory) == false)
                                    playerHistory = null;
                            }
                            table += PageUtility.CreatePlayerRow(nr, _Realm, player.Value, _Columns, playerHistory);
                        }
                        else
                            table += PageUtility.CreatePlayerRow(nr, _Realm, player.Value, _Columns);
                    }
                }
                if (nr >= (pageIndex + 1) * _Count)
                    break;
            }
            table += "</tbody></table>";

            if (nr != 0 && nr <= pageIndex * _Count)
            {
                pageIndex = (nr - 1) / _Count;
                Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "page", (pageIndex + 1).ToString()));
            }
            return table;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string sectionStr = PageUtility.GetQueryString(Request, "section", "ranks");
            int count = PageUtility.GetQueryInt(Request, "count", 100);
            if (count > 500) count = 500;

            var realm = RealmControl.Realm;
            if (realm == WowRealm.Unknown)
                return;
            var wowVersion = StaticValues.GetWowVersion(realm);

            PlayerColumn[] lifetimeKillsColumns = Table_Lifetime_Kills_Columns;
            PlayerColumn[] rankColumns = Table_Ranks_Columns;
            Func<KeyValuePair<string, Player>, double> rankSorterLambda = (KeyValuePair<string, Player> _Player) => { return _Player.Value.GetRankTotal(); };
            Func<Player, bool> rankShowLambda = (Player _Player) => { return _Player.ReceivedStandingLastWeek() == true && ((DateTime.UtcNow - _Player.LastSeen).TotalDays <= 7.0); };
            if (wowVersion == VF_RealmPlayersDatabase.WowVersionEnum.TBC)
            {
                lifetimeKillsColumns = Table_Lifetime_Kills_Columns_TBC;
                rankColumns = Table_Ranks_Columns_TBC;
                rankSorterLambda = (KeyValuePair<string, Player> _Player) => { 
                    return _Player.Value.GetArenaRatingTotal(); 
                };
                rankShowLambda = (Player _Player) => {
                    return _Player.Arena != null;
                };
            }
            if (sectionStr == "standings")
            {
                this.Title = "Standings @ " + StaticValues.ConvertRealmParam(realm) + " | RealmPlayers";

                m_PageHTML = new MvcHtmlString(CreateStandingsTable(realm, count, (player) => { return player.ReceivedStandingLastWeek(); }));

                if (realm == WowRealm.Nostalrius)
                {
                    DateTime lowerValue = StaticValues.CalculateLastRankUpdadeDateUTC(realm);
                    GeneratePageDetails("Standings", count * 2, "Last weeks standings for players. Only shows players who received a standing last week. This list resets when the realm calculates new standings for players every wednesday 13:59~ servertime.");
                }
                else if(wowVersion == VF_RealmPlayersDatabase.WowVersionEnum.TBC)
                {
                    GeneratePageDetails("Standings", count * 2, "TBC works with a different PVP system so there are no standings.");
                }
                else
                {
                    DateTime lowerValue = StaticValues.CalculateLastRankUpdadeDateUTC(realm);
                    GeneratePageDetails("Standings", count * 2, "Last weeks standings for players. Only shows players who received a standing last week. This list resets when the realm calculates new standings for players every saturday 23:59~ servertime.");
                }
            }
            else if (sectionStr == "twink_ranks")
            {
                this.Title = "Twinks @ " + StaticValues.ConvertRealmParam(realm) + " | RealmPlayers";

                m_PageHTML = new MvcHtmlString(CreatePVPTable(realm, count, Table_Twink_Ranks_Columns
                    , (KeyValuePair<string, Player> _Player) => { return _Player.Value.GetRankTotal(); }
                    , (Player _Player) => { return _Player.Character.Level > 10 && _Player.Character.Level < 60 && _Player.Honor.CurrentRank > 0 && (DateTime.UtcNow - _Player.LastSeen).TotalDays <= 30; }));
                GeneratePageDetails("Twinks", count, "PVP Ranks for players that are below lvl 60. This is meant to show active twinks on the realm. Only shows players that was seen less than 30 days ago");
            }
            else if (sectionStr == "lifetime_kills")
            {
                this.Title = "Lifetime Kills @ " + StaticValues.ConvertRealmParam(realm) + " | RealmPlayers";

                m_PageHTML = new MvcHtmlString(CreatePVPTable(realm, count, lifetimeKillsColumns
                    , (KeyValuePair<string, Player> _Player) => { return _Player.Value.Honor.LifetimeHK; }
                    , (Player _Player) => { return true; }));
                
                GeneratePageDetails("Lifetime Kills", count, "List sorted by the amount of total honorable kills received. This is to show the players who have pvped the most overall.");
            }
            else if (sectionStr == "highest_ranks")
            {
                this.Title = "Highest Ranks @ " + StaticValues.ConvertRealmParam(realm) + " | RealmPlayers";

                if(wowVersion == VF_RealmPlayersDatabase.WowVersionEnum.TBC)
                {
                    m_PageHTML = new MvcHtmlString("");
                    GeneratePageDetails("Highest Ranks", count, "TBC works with a different PVP system so there are no lifetime highest ranks.");
                }
                else
                { 
                    var playerSummaryDB = Hidden.ApplicationInstance.Instance.GetPlayerSummaryDatabase();
                    var pvpSummaries = playerSummaryDB.GetPVPSummaries(realm);
                    DateTime referenceDateTime = new DateTime(2010, 1, 1);
                    var orderedPVPSummaries = pvpSummaries.OrderByDescending((_Value) => ((int)_Value.Value.m_HighestRank.Key) * 100000 - (_Value.Value.m_HighestRank.Value != DateTime.MinValue ? (int)((_Value.Value.m_HighestRank.Value - referenceDateTime).TotalDays) : 0));
                    //////////////////////////////////
                    int pageIndex = PageUtility.GetQueryInt(Request, "page", 1) - 1;//Change range from 0 to * instead of 1 to *

                    PlayerColumn[] playerColumns = new PlayerColumn[]{
                        PlayerColumn.Number,
                        PlayerColumn.Character_And_Guild,
                        PlayerColumn.Race_And_Class,
                        PlayerColumn.Total_HKs,
                        PlayerColumn.LastSeen,
                    };
                    Dictionary<PlayerColumn, string[]> extraColumns = new Dictionary<PlayerColumn, string[]>();

                    extraColumns[PlayerColumn.Number] = new string[] { "Rank", "Date Achieved" };
                    //extraColumns[PlayerColumn.Total_HKs] = new string[] { "Weeks with standing" };

                    string table = "<table class='table'>";
                    table += "<thead>" + PageUtility.CreatePlayerTableHeaderRow(playerColumns, extraColumns) + "</thead>";
                    table += "<tbody>";
                    var playerArray = DatabaseAccess.GetRealmPlayers(this, realm, NotLoadedDecision.RedirectAndWait);
                    int nr = 0;
                    foreach (var pvpSummary in orderedPVPSummaries)
                    {
                        nr++;
                        if (nr > pageIndex * count && nr <= (pageIndex + 1) * count)
                        {
                            Player playerData = null;
                            if (playerArray.TryGetValue(playerSummaryDB.GetPlayer(pvpSummary), out playerData) == true)
                            {
                                VF_RealmPlayersDatabase.PlayerFaction playerFaction = StaticValues.GetFaction(playerData.Character.Race);
                                int rank = (int)pvpSummary.Value.m_HighestRank.Key;
                                float rankProgress = pvpSummary.Value.m_HighestRank.Key - (float)rank;
                                extraColumns[PlayerColumn.Number] = new string[] { PageUtility.CreatePlayerRankDiv(rank, rankProgress, playerFaction), pvpSummary.Value.m_HighestRank.Value.ToString("yyyy-MM-dd") };
                                //extraColumns[PlayerColumn.Total_HKs] = new string[] { pvpSummary.Value.m_ActivePVPWeeks.ToString() };
                                table += PageUtility.CreatePlayerRow(nr, realm, playerData, playerColumns, null, extraColumns);
                            }
                        }
                        if (nr >= (pageIndex + 1) * count)
                            break;
                    }
                    table += "</tbody></table>";

                    if (nr != 0 && nr <= pageIndex * count)
                    {
                        pageIndex = (nr - 1) / count;
                        Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "page", (pageIndex + 1).ToString()));
                    }
                    m_PageHTML = new MvcHtmlString(table);
                    //////////////////////////////////
                    if (realm == WowRealm.Nostalrius)
                    {
                        GeneratePageDetails("Highest Ranks", count, "Highest lifetime achieved PVP Ranks for players, sorted by date of achievment<br /><br /><font color='red'>Currently the realm \"Nostalrius Begins\" has an inspection bug which among other things makes people look like they have higher \"lifetime highest rank\" than they should. So the data below will be incorrect until Nostalrius development team has solved this bug for the realm.</font>");
                    }
                    else
                    {
                        GeneratePageDetails("Highest Ranks", count, "Highest lifetime achieved PVP Ranks for players, sorted by date of achievment");
                    }
                }
            }
            else// if (sectionStr == "ranks")
            {
                this.Title = "Ranks @ " + StaticValues.ConvertRealmParam(realm) + " | RealmPlayers";

                if (realm == WowRealm.Nostalrius)
                {
                    rankShowLambda = (Player _Player) =>
                    {
                        return (_Player.LastSeen - DateTime.UtcNow).TotalDays < 14;
                    };
                    GeneratePageDetails("Ranks", count, "Highest PVP Ranks for active players.");
                }
                else if(wowVersion == VF_RealmPlayersDatabase.WowVersionEnum.TBC)
                {
                    GeneratePageDetails("Arena Ranking", count, "Shows the highest rated arena teams, sorted by a sum of the total rating for 5v5, 3v3 and 2v2.");
                }
                else
                {
                    GeneratePageDetails("Ranks", count, "Highest PVP Ranks for players. Only lists players who received standing last week. This is because no rank decay would make the list extremely boring(filled with players that dont even pvp).");
                }
                m_PageHTML = new MvcHtmlString(CreatePVPTable(realm, count, rankColumns
                    , rankSorterLambda, rankShowLambda));
            }
            //href='javascript:navigateWithNewQuery(\"page\",\"1\")'
            int pageNr = PageUtility.GetQueryInt(Request, "page", 1);
            int maxPageNr = 1000000 / count;
            m_PaginationHTML = new MvcHtmlString(PageUtility.CreatePagination(Request, pageNr, maxPageNr));
        }
    }
}