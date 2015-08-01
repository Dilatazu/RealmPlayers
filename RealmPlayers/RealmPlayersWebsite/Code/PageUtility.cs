using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RealmPlayersServer.Code.Resources;

using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;
using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerSex = VF_RealmPlayersDatabase.PlayerSex;

using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using Guild = VF_RealmPlayersDatabase.GeneratedData.Guild;

namespace RealmPlayersServer
{
    public static class PageUtilityExtension
    {
        public static string SiteVersion = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
        public static System.Web.Mvc.MvcHtmlString HTMLGetSiteVersion(string _Filename)
        {
            return new System.Web.Mvc.MvcHtmlString(_GetSiteVersion(_Filename));
        }
        private static string _GetSiteVersion(string _Filename)
        {
            if (_Filename.EndsWith("js"))
                return "<script src='" + _Filename + "?version=" + SiteVersion + "' type='text/javascript'></script>";
            else if (_Filename.EndsWith("css"))
                return "<link href='" + _Filename + "?version=" + SiteVersion + "' rel='stylesheet'/>";
            else
                return _Filename;
        }
        public static System.Web.Mvc.MvcHtmlString InitializeItemTooltip()
        {
            return new System.Web.Mvc.MvcHtmlString("<script>"
                + "g_WowIconImageHoster = '" + Hidden.ApplicationInstance.Instance.GetCurrentItemDatabaseAddress() + "';"
                + "</script>");
        }
        public static System.Web.Mvc.MvcHtmlString HTMLAddResources(params string[] _Filenames)
        {
            string result = "";
            foreach(string fileName in _Filenames)
            {
                result += _GetSiteVersion(fileName);
            }
            return new System.Web.Mvc.MvcHtmlString(result);
        }
    }
    public enum PlayerColumn
    {
        Level_And_Race_And_Class,
        Rank,
        Faction_And_Name,
        Name,
        Guild,
        Server,
        LastSeen,
        Level,
        Standing,
        Total_HKs,
        Race_And_Class,
        Character_And_Guild,
        HonorLastWeek,
        Number,
        GuildRank,
        GuildRank_And_Nr,
        RankChange,
        ClassColoredName,
        HKLastWeek,
        Rating_2v2,
        Rating_3v3,
        Rating_5v5,
        Total_DKs,
    }
    public enum GuildColumn
    {
        Number,
        Name,
        Progress,
        Server,
        MemberCount,
        TotalHKs,
        AverageRank,
        AverageStanding,
        Level60MemberCount,
        AverageMemberHKs,
        Level70MemberCount,
    }
    public class PageUtility
    {
        public static string HOSTURL_Armory = "";
        public static string HOSTURL_RaidStats = "";
        public static string CreateLink_RaidOverview(int _UniqueID, string _Content)
        {
            return PageUtility.CreateLink(HOSTURL_RaidStats + "RaidOverview.aspx?Raid=" + _UniqueID, _Content);
        }
        public static string CreateLink_GuildRaidList(string _Guild)
        {
            return PageUtility.CreateLink(HOSTURL_RaidStats + "RaidList.aspx?Guild=" + _Guild, _Guild);
        }
        public static string CreateLink_Armory_Guild(string _Guild, WowRealm _Realm)
        {
            return CreateLink_Armory_Guild(_Guild, _Realm, _Guild);
        }
        public static string CreateLink_Armory_Guild(string _Guild, WowRealm _Realm, string _Content)
        {
            return PageUtility.CreateLink(HOSTURL_Armory + "GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&guild=" + _Guild, _Content);
        }
        public static string CreateLink_Armory_Player_Colored(Player _RealmPlayer)
        {
            return CreateLink_Armory_Player(_RealmPlayer.Name, _RealmPlayer.Realm, CreateColorCodedName(_RealmPlayer.Name, _RealmPlayer.Character.Class));
        }
        public static string CreateLink_Armory_Player_NotColored(Player _RealmPlayer)
        {
            return CreateLink_Armory_Player(_RealmPlayer.Name, _RealmPlayer.Realm);
        }
        public static string CreateLink_Armory_Player(string _Player, WowRealm _Realm)
        {
            return CreateLink_Armory_Player(_Player, _Realm, _Player);
        }
        public static string CreateLink_Armory_Player(string _Player, WowRealm _Realm, string _Content)
        {
            return PageUtility.CreateLink(HOSTURL_Armory + "CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&player=" + _Player, _Content);
        }
        public static string CreateLink_RaidStats_Player(Player _RealmPlayer)
        {
            return CreateLink_RaidStats_Player(_RealmPlayer.Name, _RealmPlayer.Realm);
        }
        public static string CreateLink_RaidStats_Player(string _Player, WowRealm _Realm)
        {
            return CreateLink_RaidStats_Player(_Player, _Realm, _Player);
        }
        public static string CreateLink_RaidStats_Player(string _Player, WowRealm _Realm, string _Content)
        {
            return PageUtility.CreateLink(HOSTURL_RaidStats + "PlayerOverview.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&player=" + _Player, _Content);
        }
        public static string GetQueryString(System.Web.HttpRequest _RequestObject, string _QueryName, string _DefaultValue = "null")
        {
            string result = _RequestObject.QueryString.Get(_QueryName);
            if (result == null)
                result = _DefaultValue;
            return result;
        }
        public static string GetQueryString(Func<string, string> _GetQueryFunc, string _QueryName, string _DefaultValue = "null")
        {
            string result = _GetQueryFunc(_QueryName);
            if (result == null)
                result = _DefaultValue;
            return result;
        }
        public static WowRealm GetQueryRealm(System.Web.HttpRequest _RequestObject)
        {
            string realmStr = GetQueryString(_RequestObject, "realm", "ED");
            return VF_RealmPlayersDatabase.StaticValues.ConvertRealm(realmStr);
        }
        public static VF_RealmPlayersDatabase.WowVersionEnum GetQueryWowVersion(System.Web.HttpRequest _RequestObject)
        {
            string wowVersionStr = GetQueryString(_RequestObject, "wowversion", "Vanilla").ToUpper();
            if (wowVersionStr == "VANILLA" || wowVersionStr == "CLASSIC")
                return VF_RealmPlayersDatabase.WowVersionEnum.Vanilla;
            else if (wowVersionStr == "TBC")
                return VF_RealmPlayersDatabase.WowVersionEnum.TBC;
            else if (wowVersionStr == "WOTLK")
                return VF_RealmPlayersDatabase.WowVersionEnum.WOTLK;
            else
                return VF_RealmPlayersDatabase.WowVersionEnum.Unknown;
        }
        public static int GetQueryInt(System.Web.HttpRequest _RequestObject, string _QueryName, int _DefaultValue = 0)
        {
            string resultStr = GetQueryString(_RequestObject, _QueryName, "");
            int result = _DefaultValue;
            if (int.TryParse(resultStr, out result) == false) result = _DefaultValue;
            return result;
        }
        public static int GetQueryInt(Func<string, string> _GetQueryFunc, string _QueryName, int _DefaultValue = 0)
        {
            string resultStr = GetQueryString(_GetQueryFunc, _QueryName, "");
            int result = _DefaultValue;
            if (int.TryParse(resultStr, out result) == false) result = _DefaultValue;
            return result;
        }
        public static bool GetQueryFromToInt(System.Web.HttpRequest _RequestObject, string _QueryName, out int _Value1, out int _Value2, char _Separator = '-', int _DefaultValue = -1)
        {
            string fromTo = GetQueryString(_RequestObject, _QueryName, "");
            if (fromTo == "")
            {
                _Value1 = _DefaultValue;
                _Value2 = _DefaultValue;
                return false;
            }
            else
            {
                if (int.TryParse(fromTo.Split(_Separator).First(), out _Value1) == false)
                    _Value1 = _DefaultValue;
                if (int.TryParse(fromTo.Split(_Separator).Last(), out _Value2) == false)
                    _Value2 = _DefaultValue;
                return true;
            }
        }
        public static void RedirectErrorLoading(System.Web.UI.Page _Page, string _WaitFor)
        {
            _Page.Response.Redirect("Error.aspx?reason=loading&wait=" + _WaitFor + "&return=" + System.Web.HttpUtility.UrlEncode(_Page.Request.RawUrl));
        }
        public static string CreateUrlWithNewQueryValues(System.Web.HttpRequest _RequestObject, KeyValuePair<string, string>[] _QueryNameValues)
        {
            var nameValues = HttpUtility.ParseQueryString(_RequestObject.QueryString.ToString());
            foreach(var _QuerySet in _QueryNameValues)
            {
                if (_QuerySet.Value != null)
                {
                    nameValues.Set(_QuerySet.Key, _QuerySet.Value);
                }
                else
                {
                    nameValues.Remove(_QuerySet.Key);
                }
            }
            string url = _RequestObject.Url.AbsolutePath;
            string updatedQueryString = "?" + nameValues.ToString();
            return url + updatedQueryString;
        }
        public static string CreateUrlWithNewQueryValue(System.Web.HttpRequest _RequestObject, string _QueryName, string _QueryValue)
        {
            var nameValues = HttpUtility.ParseQueryString(_RequestObject.QueryString.ToString());
            if (_QueryValue != null)
            {
                nameValues.Set(_QueryName, _QueryValue);
            }
            else
            {
                nameValues.Remove(_QueryName);
            }
            string url = _RequestObject.Url.AbsolutePath;
            string updatedQueryString = "?" + nameValues.ToString();
            return url + updatedQueryString;
        }
        public static string CreatePagination(System.Web.HttpRequest _RequestObject, int _CurrPage, int _MaxPage)
        {
            if (_MaxPage == 1)
                return "";
            string paginationSrc = "<div class='pagination text-center'><ul>";

            paginationSrc += "<li" + (_CurrPage == 1 ? " class='disabled'" : "") + "><a href='" + PageUtility.CreateUrlWithNewQueryValue(_RequestObject, "page", "1") + "'>First</a></li>";
            paginationSrc += "<li" + (_CurrPage == 1 ? " class='disabled'" : "") + "><a href='" + PageUtility.CreateUrlWithNewQueryValue(_RequestObject, "page", (_CurrPage - 1).ToString()) + "'>Prev</a></li>";
            for (int i = _CurrPage - 4; i < _CurrPage + 5; ++i)
            {
                int pageIndex = i;
                if (_CurrPage - 4 < 1)
                    pageIndex += 1 - (_CurrPage - 4);
                if (pageIndex > 0 && pageIndex <= _MaxPage)
                {
                    paginationSrc += "<li" + (_CurrPage == pageIndex ? " class='disabled'" : "") + "><a href='" + PageUtility.CreateUrlWithNewQueryValue(_RequestObject, "page", pageIndex.ToString()) + "'>" + pageIndex + "</a></li>";
                }
            }
            paginationSrc += "<li" + (_CurrPage == _MaxPage ? " class='disabled'" : "") + "><a href='" + PageUtility.CreateUrlWithNewQueryValue(_RequestObject, "page", (_CurrPage + 1).ToString()) + "'>Next</a></li>";
            paginationSrc += "<li" + (_CurrPage == _MaxPage ? " class='disabled'" : "") + "><a href='" + PageUtility.CreateUrlWithNewQueryValue(_RequestObject, "page", _MaxPage.ToString()) + "'>Last</a></li>";

            paginationSrc += "</ul></div>";
            return paginationSrc;
        }
        public static string CreateTableRow(string _ClassName, string _Content)
        {
            if(String.IsNullOrEmpty(_ClassName))
                return "<tr>" + _Content + "</tr>";
            return "<tr class=\"" + _ClassName + "\">" + _Content + "</tr>";
        }
        public static string CreateTableColumnHead(string _Content)
        {
            return "<th>" + _Content + "</th>";
        }
        public static string CreateTableColumn(string _Content)
        {
            return "<td>" + _Content + "</td>";
        }
        public static string CreateImage(string _Source, string _ToolTip = "")
        {
            if (_ToolTip == "")
                return "<img src=\"" + _Source + "\"/>";
            else
                return "<img src=\"" + _Source + "\" data-toggle=\"tooltip\" title=\"" + _ToolTip + "\" />";
        }
        public static string CreateLink(string _Link, string _Content)
        {
            //if(_ClassName != "")
            //    return "<a class=\"" + _ClassName + "\"href=\"" + _Link + "\">" + _Content + "</a>";
            return "<a href=\"" + _Link + "\">" + _Content + "</a>";
        }
        public static string CreateDiv(string _ClassName, string _Content)
        {
            if (String.IsNullOrEmpty(_ClassName))
                return "<div>" + _Content + "</div>";
            return "<div class=\"" + _ClassName + "\">" + _Content + "</div>";
        }
        public static string CreateTooltipText(string _Text, string _Tooltip)
        {
            return "<div title=\"" + _Tooltip + "\">" + _Text + "</div>";
        }
        //Old functions, should be refactored/removed maybe later
        public static string CreateClassIMG(PlayerClass _Class)
        {
            return "<div class='vf-classimage' id='vf-ci_" + _Class.ToString().ToLower() + "'></div>";
        }
        public static string CreateLargeClassIMG(PlayerClass _Class)
        {
            return "<div class='vf-largeclassimage' id='vf-ci_" + _Class.ToString().ToLower() + "'></div>";
        }
        public static string CreateRaceIMG(PlayerRace _Race, PlayerSex _Sex)
        {
            return "<div class='vf-raceimage' id='vf-ri_" + _Sex.ToString().ToLower() + "_" + _Race.ToString().Replace("_", "").ToLower() + "'"
            + "></div>";
        }
        public static string CreateLargeRaceIMG(Player _Player)
        {
            return "<div class='vf-largeraceimage' id='vf-ri_" + _Player.Character.Sex.ToString().ToLower() + "_" + _Player.Character.Race.ToString().Replace("_", "").ToLower() + "'"
            + "></div>";
        }
        public static string CreatePVPRankIMG(Player _Player, bool _RankTooltip = false)
        {
            string pvprankImage = "<div class='vf-pvprankimage' id='vf-pri_" + _Player.Honor.CurrentRank.ToString("00") + "'";
            if (_RankTooltip == true)
            {
                pvprankImage += " data-toggle='tooltip' title='" + StaticValues.ConvertRankVisualWithNr(_Player.Honor.CurrentRank, StaticValues.GetFaction(_Player.Character.Race)) + " +" + _Player.Honor.CurrentRankProgress.ToString("0.0%") + "'";
            }
            return pvprankImage + "></div>";
        }
        public static string CreateLargePVPRankIMG(Player _Player, bool _RankTooltip = false)
        {
            string pvprankImage = "<div class='vf-largepvprankimage' id='vf-pri_" + _Player.Honor.CurrentRank.ToString("00") + "'";
            if(_RankTooltip == true)
            {
                pvprankImage += " data-toggle='tooltip' title='" + StaticValues.ConvertRankVisualWithNr(_Player.Honor.CurrentRank, StaticValues.GetFaction(_Player.Character.Race)) + " +" + _Player.Honor.CurrentRankProgress.ToString("0.0%") + "'";
            }
            return pvprankImage + "></div>";
        }
        public static string CreateRaceIMG(PlayerRace _Race, PlayerSex _Sex, int _ImageSize)
        {
            return "<div class='vf-raceimage' id='vf-ri_" + _Sex.ToString().ToLower() + "_" + _Race.ToString().Replace("_", "").ToLower() + "'"
            + " style='width: " + _ImageSize + "px; height: " + _ImageSize + "px;'></div>";
        }
        public static string CreatePlayerLink(string _Player, WowRealm _Realm)
        {
            return "<a style='text-decoration: none;' href='/CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&player=" + _Player + "'>" + _Player + "</a>";
        }
        public static string CreateColorCodedName(string _Name, PlayerFaction _Faction)
        {
            string color = VisualResources._FactionColors[_Faction];
            return "<font color=\"" + color + "\">" + _Name + "</font>";
        }
        public static string CreateColorCodedName(string _Name, PlayerClass _Class)
        {
            string color = VisualResources._ClassColors[_Class];
            return "<font color=\"" + color + "\">" + _Name + "</font>";
        }
        public static string CreateColorCodedName(VF_RealmPlayersDatabase.PlayerData.Player _Player)
        {
            string color = VisualResources._ClassColors[_Player.Character.Class];
            return "<font color=\"" + color + "\">" + _Player.Name + "</font>";
        }
        public static string CreateColorCodedPlayerLink(WowRealm _Realm, Player _Player, bool _GlobalURL = false)
        {
            string color = VisualResources._ClassColors[_Player.Character.Class];
            if (_GlobalURL == true)
                return "<a href='realmplayers.com/CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&player=" + _Player.Name + "'><font color='" + color + "'>" + _Player.Name + "</font></a>";
            else
                return "<a href='/CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&player=" + _Player.Name + "'><font color='" + color + "'>" + _Player.Name + "</font></a>";
        }
        //Old functions, should be refactored/removed maybe later

        public static string GetClassColor(PlayerClass _Class)
        {
            return Code.Resources.VisualResources._ClassColors[_Class];
        }
        public static string GetClassColor(Player _Player)
        {
            return Code.Resources.VisualResources._ClassColors[_Player.Character.Class];
        }

        public static string CreateLink_CharacterViewer(string _Player, WowRealm _Realm)
        {
            return CreateLink("http://realmplayers.com/CharacterViewer.aspx?realm="
                + RealmPlayersServer.StaticValues.ConvertRealmParam(_Realm) + "&player=" + _Player, _Player);
        }
        public static string CreateLink_CharacterViewer(Player _Player)
        {
            return CreateLink("http://realmplayers.com/CharacterViewer.aspx?realm="
                + RealmPlayersServer.StaticValues.ConvertRealmParam(_Player.Realm) + "&player=" + _Player.Name, _Player.Name);
        }

        public static string CreatePlayerTableHeaderRow(PlayerColumn[] _Columns, Dictionary<PlayerColumn, string[]> _ExtraColumns = null, HttpRequest _RequestObject = null)
        {
            string tableHeaderColumns = "";
            foreach (PlayerColumn column in _Columns)
            {
                string columnData = "null";
                switch (column)
                {
                    case PlayerColumn.Level_And_Race_And_Class:
                        columnData = "Info";
                        break;
                    case PlayerColumn.Rank:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "Rank"), "Rank");
                        else
                            columnData = "Rank";
                        break;
                    case PlayerColumn.Faction_And_Name:
                        columnData = "Name";
                        break;
                    case PlayerColumn.Name:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "Name"), "Name");
                        else
                            columnData = "Name";
                        break;
                    case PlayerColumn.ClassColoredName:
                        columnData = "Name";
                        break;
                    case PlayerColumn.Guild:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "Guild"), "Guild");
                        else
                            columnData = "Guild";
                        break;
                    case PlayerColumn.Server:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "Server"), "Server");
                        else
                            columnData = "Server";
                        break;
                    case PlayerColumn.LastSeen:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "Seen"), "<div title='Last Seen'>Seen</div>");
                        else
                            columnData = "<div title='Last Seen'>Seen</div>";
                        break;
                    case PlayerColumn.Level:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "Level"), "Level");
                        else
                            columnData = "Level";
                        break;
                    case PlayerColumn.Standing:
                        columnData = "<div title='#Standing'>#</div>";
                        break;
                    case PlayerColumn.Total_HKs:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "TotalHKs"), "Total HKs");
                        else
                            columnData = "Total HKs";
                        break;
                    case PlayerColumn.HonorLastWeek:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "Honor"), "<div title='Honor received Last Week'>Honor</div>");
                        else
                            columnData = "<div title='Honor received Last Week'>Honor</div>";
                        break;
                    case PlayerColumn.Race_And_Class:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "RaceClass"), "Race/Class");
                        else
                            columnData = "Race/Class";
                        break;
                    case PlayerColumn.Character_And_Guild:
                        columnData = "Character" + System.Net.WebUtility.HtmlEncode("<Guild>");
                        break;
                    case PlayerColumn.Number:
                        columnData = "#Nr";
                        break;
                    case PlayerColumn.GuildRank:
                        columnData = "Guild Rank";
                        break;
                    case PlayerColumn.GuildRank_And_Nr:
                        columnData = "Guild Rank(Nr)";
                        break;
                    case PlayerColumn.RankChange:
                        columnData = "Rank Change";
                        break;
                    case PlayerColumn.HKLastWeek:
                        if (_RequestObject != null)
                            columnData = CreateLink(CreateUrlWithNewQueryValue(_RequestObject, "sort", "HKs"), "<div title='HKs received Last Week'>HKs</div>");
                        else
                            columnData = "<div title='HKs received Last Week'>HKs</div>";
                        break;
                    case PlayerColumn.Rating_2v2:
                        columnData = "Rating 2v2";
                        break;
                    case PlayerColumn.Rating_3v3:
                        columnData = "Rating 3v3";
                        break;
                    case PlayerColumn.Rating_5v5:
                        columnData = "Rating 5v5";
                        break;
                    case PlayerColumn.Total_DKs:
                        columnData = "Total DKs";
                        break;
                    default:
                        break;
                }
                tableHeaderColumns += CreateTableColumnHead(columnData);
                if (_ExtraColumns != null)
                {
                    if (_ExtraColumns.ContainsKey(column))
                    {
                        foreach (string extraColumn in _ExtraColumns[column])
                        {
                            tableHeaderColumns += CreateTableColumnHead(extraColumn);
                        }
                    }
                }
            }
            return CreateTableRow(null, tableHeaderColumns);
        }
        public static string BreakLongStr(string _String, int _MaxSectionSize, int _MinSectionSize)
        {
            string resultStr = _String;
            if (_String.Length > _MaxSectionSize)
            {
                var wordwrapIndex = _String.IndexOf(' ', _MinSectionSize);
                if (wordwrapIndex > _MaxSectionSize || wordwrapIndex == -1)
                {
                    if (_MaxSectionSize > _String.Count() / 2)
                        wordwrapIndex = _String.Count() / 2;
                    else
                        wordwrapIndex = _MaxSectionSize;
                }
                resultStr = _String.Substring(0, wordwrapIndex) + " " + _String.Substring(wordwrapIndex);
            }
            return resultStr;
        }
        public static string CreatePlayerRankDiv(int _Rank, float _RankProgress, PlayerFaction _Faction)
        {
            if (_Rank == 0)
                return "";
            else
                return "<div class='pvp-rank' id='pvprank" + _Rank.ToString("00") + "' data-toggle='tooltip' title='" + StaticValues.ConvertRankVisualWithNr(_Rank, _Faction) + " +" + _RankProgress.ToString("0.0%") + "'></div>";
        }
        public static string CreatePlayerRow(int _Number, WowRealm _Realm, Player _Player, PlayerColumn[] _Columns, PlayerHistory _PlayerHistory = null, Dictionary<PlayerColumn, string[]> _ExtraColumns = null)
        {
            PlayerFaction playerFaction = StaticValues.GetFaction(_Player.Character.Race);

            string tableColumns = "";
            foreach (PlayerColumn column in _Columns)
            {
                string columnData = "null";
                switch (column)
                {
                    case PlayerColumn.Rank:
                        columnData = CreatePlayerRankDiv(_Player.Honor.CurrentRank, _Player.Honor.CurrentRankProgress, playerFaction);
                        break;
                    case PlayerColumn.Faction_And_Name:
                        columnData = CreateImage(StaticValues.GetFactionIMG(playerFaction))
                            + CreateLink("CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&player=" + System.Web.HttpUtility.HtmlEncode(_Player.Name), _Player.Name);
                        break;
                    case PlayerColumn.Name:
                        columnData = CreateLink("CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&player=" + System.Web.HttpUtility.HtmlEncode(_Player.Name), _Player.Name);
                        break;
                    case PlayerColumn.ClassColoredName:
                        columnData = CreateLink("CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&player=" + System.Web.HttpUtility.HtmlEncode(_Player.Name), CreateColorCodedName(_Player.Name, _Player.Character.Class));
                        break;
                    case PlayerColumn.Guild:
                        if (_Player.Guild.GuildName != "nil")
                        {
                            string visualGuildName = BreakLongStr(_Player.Guild.GuildName, 16, 8);
                            columnData = CreateLink("GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&guild=" + System.Web.HttpUtility.HtmlEncode(_Player.Guild.GuildName), "&lt;" + visualGuildName + "&gt;");
                        }
                        else
                            columnData = "";
                        break;
                    case PlayerColumn.Server:
                        columnData = StaticValues.ConvertRealmViewing(_Realm);
                        break;
                    case PlayerColumn.LastSeen:
                        columnData = "<div title=\"Seen by " + _Player.GetUploaderName() + " " + _Player.LastSeen.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "\">" + StaticValues.GetTimeSinceLastSeenUTC(_Player.LastSeen) + "</div>";
                        break;
                    case PlayerColumn.Level_And_Race_And_Class:
                        columnData = _Player.Character.Level + " "
                            + CreateImage("assets/img/icons/IconSmall_" + _Player.Character.Race.ToString().Replace("_", "") + "_" + _Player.Character.Sex.ToString() + ".gif")
                            + CreateImage("assets/img/icons/IconSmall_" + _Player.Character.Class.ToString() + ".gif");
                        break;
                    case PlayerColumn.Level:
                        columnData = _Player.Character.Level.ToString();
                        break;
                    case PlayerColumn.Standing:
                        columnData = _Player.GetStandingStr();
                        break;
                    case PlayerColumn.Total_HKs:
                        columnData = _Player.Honor.LifetimeHK.ToString();
                        break;
                    case PlayerColumn.HonorLastWeek:
                        if (_Player.Honor.LifetimeHK > 50 && _Player.LastSeen.DayOfWeek == DayOfWeek.Sunday && _Player.Honor.LastWeekHK == 0 && _Player.Honor.LastWeekHonor == 0 && _Player.Honor.ThisWeekHK == 0 && _Player.Honor.ThisWeekHonor == 0)
                            columnData = "???";
                        else
                        {
                            int value = _Player.Honor.LastWeekHonor;
                            if (value > 1000)
                                columnData = (value / 1000).ToString() + "k";
                            else
                                columnData = value.ToString();
                        }
                        break;
                    case PlayerColumn.Race_And_Class:
                        columnData = CreateRaceIMG(_Player.Character.Race, _Player.Character.Sex) + CreateClassIMG(_Player.Character.Class);
                        break;
                    case PlayerColumn.Character_And_Guild:
                        columnData = CreateLink("CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&player=" + System.Web.HttpUtility.HtmlEncode(_Player.Name), CreateColorCodedName(_Player.Name, _Player.Character.Class));
                        if (_Player.Guild.GuildName != "nil")
                        {
                            string visualGuildName = BreakLongStr(_Player.Guild.GuildName, 16, 8);
                            columnData += "<br>" + CreateLink("GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "&guild=" + System.Web.HttpUtility.HtmlEncode(_Player.Guild.GuildName), "&lt;" + visualGuildName + "&gt;");
                        }
                        break;
                    case PlayerColumn.Number:
                        columnData = "#" + _Number;
                        break;
                    case PlayerColumn.GuildRank:
                        columnData = _Player.Guild.GuildRank;
                        break;
                    case PlayerColumn.GuildRank_And_Nr:
                        columnData = _Player.Guild.GuildRank + "(" + _Player.Guild.GuildRankNr + ")";
                        break;
                    case PlayerColumn.RankChange:
                        if (_PlayerHistory != null)
                        {
                            float rankChange = StaticValues.CalculateRankChange(_Player, _PlayerHistory);
                            columnData = (rankChange >= 0.0f ? "<font color='#00ff00'>+" : "<font color='#ff0000'>") + rankChange.ToString("0.0%") + "</font>";
                        }
                        else
                            columnData = "??.?%";
                        break;
                    case PlayerColumn.HKLastWeek:
                        if (_Player.Honor.LifetimeHK > 50 && _Player.LastSeen.DayOfWeek == DayOfWeek.Sunday && _Player.Honor.LastWeekHK == 0 && _Player.Honor.LastWeekHonor == 0 && _Player.Honor.ThisWeekHK == 0 && _Player.Honor.ThisWeekHonor == 0)
                            columnData = "???";
                        else
                            columnData = "<font color='#00ff00'>+" + _Player.Honor.LastWeekHK + "</font>";
                        break;
                    case PlayerColumn.Rating_2v2:
                        if (_Player.Arena != null && _Player.Arena.Team2v2 != null)
                        {
                            columnData = _Player.Arena.Team2v2.PlayerRating.ToString();
                        }
                        else
                        {
                            columnData = "----"; ;
                        }
                        break;
                    case PlayerColumn.Rating_3v3:
                        if (_Player.Arena != null && _Player.Arena.Team3v3 != null)
                        {
                            columnData = _Player.Arena.Team3v3.PlayerRating.ToString();
                        }
                        else
                        {
                            columnData = "----";
                        }
                        break;
                    case PlayerColumn.Rating_5v5:
                        if (_Player.Arena != null && _Player.Arena.Team5v5 != null)
                        {
                            columnData = _Player.Arena.Team5v5.PlayerRating.ToString();
                        }
                        else
                        {
                            columnData = "----";
                        }
                        break;
                    case PlayerColumn.Total_DKs:
                        columnData = _Player.Honor.LifetimeDK.ToString();
                        break;
                    default:
                        break;
                }
                tableColumns += CreateTableColumn(columnData);
                if (_ExtraColumns != null)
                {
                    if (_ExtraColumns.ContainsKey(column))
                    {
                        foreach (string extraColumn in _ExtraColumns[column])
                        {
                            tableColumns += CreateTableColumn(extraColumn);
                        }
                    }
                }
            }
            return CreateTableRow(StaticValues.GetFactionCSSName(playerFaction) + "_row", tableColumns);
        }
        public static string CreateGuildTableHeaderRow(GuildColumn[] _Columns)
        {
            string tableHeaderColumns = "";
            foreach (GuildColumn column in _Columns)
            {
                string columnData = "null";
                switch (column)
                {
                    case GuildColumn.Number:
                        columnData = "#Nr";
                        break;
                    case GuildColumn.Name:
                        columnData = "Guild";
                        break;
                    case GuildColumn.Progress:
                        columnData = "Progress";
                        break;
                    case GuildColumn.Server:
                        columnData = "Server";
                        break;
                    case GuildColumn.MemberCount:
                        columnData = "Members Count";
                        break;
                    case GuildColumn.TotalHKs:
                        columnData = "Total HKs";
                        break;
                    case GuildColumn.AverageRank:
                        columnData = "Average PVP Rank";
                        break;
                    case GuildColumn.AverageStanding:
                        columnData = "Average PVP Standing";
                        break;
                    case GuildColumn.Level60MemberCount:
                        columnData = "Level 60 Members Count";
                        break;
                    case GuildColumn.AverageMemberHKs:
                        columnData = "Average Member HKs";
                        break;
                    case GuildColumn.Level70MemberCount:
                        columnData = "Level 70 Members Count";
                        break;
                    default:
                        break;
                }
                tableHeaderColumns += CreateTableColumnHead(columnData);
            }
            return CreateTableRow(null, tableHeaderColumns);
        }
        public static string CreateGuildRow(int _Number, VF_RPDatabase.GuildSummary _Guild, GuildColumn[] _Columns)
        {
            string tableColumns = "";
            foreach (GuildColumn column in _Columns)
            {
                string columnData = "null";
                switch (column)
                {
                    case GuildColumn.Number:
                        columnData = "#" + _Number;
                        break;
                    case GuildColumn.Name:
                        string visualGuildName = BreakLongStr(_Guild.GuildName, 16, 8);
                        columnData = CreateLink("GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Guild.Realm) + "&guild=" + System.Web.HttpUtility.HtmlEncode(_Guild.GuildName), "&lt;" + visualGuildName + "&gt;");
                        break;
                    case GuildColumn.Progress:
                        columnData = "ToBeImplemented";
                        break;
                    case GuildColumn.Server:
                        columnData = StaticValues.ConvertRealmViewing(_Guild.Realm);
                        break;
                    case GuildColumn.MemberCount:
                        columnData = _Guild.GetMembers().Count.ToString();
                        break;
                    case GuildColumn.TotalHKs:
                        columnData = _Guild.Stats_GetTotalHKs().ToString();
                        break;
                    case GuildColumn.AverageRank:
                        {
                            columnData = _Guild.Stats_GetAveragePVPRank().ToString();
                        }
                        break;
                    case GuildColumn.AverageStanding:
                        {
                            columnData = _Guild.Stats_GetAveragePVPStanding().ToString();
                        }
                        break;
                    case GuildColumn.AverageMemberHKs:
                        columnData = _Guild.Stats_GetAverageMemberHKs().ToString();
                        break;
                    case GuildColumn.Level60MemberCount:
                        columnData = _Guild.Stats_GetTotalMaxLevels().ToString();
                        break;
                    case GuildColumn.Level70MemberCount:
                        columnData = _Guild.Stats_GetTotalMaxLevels().ToString();
                        break;
                    default:
                        break;
                }
                if (column == GuildColumn.Progress)
                    tableColumns += "<td id='" + _Guild.GuildName.Replace(' ', '_') + "-Progress'></td>";
                else
                    tableColumns += CreateTableColumn(columnData);
            }
            return CreateTableRow(StaticValues.GetFactionCSSName(_Guild.Faction) + "_row", tableColumns);
        }
        //public static System.Web.Mvc.MvcHtmlString GenerateBreadcrumbs(params string[] _Path)
        //{
        //    string breadcrumbs = "";
        //    foreach(string data in _Path)
        //    {
        //        if(data == "Home")
        //        {
        //            breadcrumbs += "<li><a href='Index.aspx'>Home</a> <span class='divider'>/</span></li>";
        //        }
        //        else if(data == "Guilds")
        //        {
        //            breadcrumbs += "<li><a href='GuildList.aspx'>Guilds</a> <span class='divider'>/</span></li>";
        //        }
        //        else if(data == "Characters")
        //        {
        //            breadcrumbs += "<li><a href='CharacterList.aspx'>Guilds</a> <span class='divider'>/</span></li>";
        //        }
        //    }
            
        //    <li><a href="guilds_index.html">Guilds</a> <span class="divider">/</span></li>
        //    <li class="active">Delirium</li>
        //    return new System.Web.Mvc.MvcHtmlString("");
        //}
        public static string BreadCrumb_AddGuilds(WowRealm _Realm)
        {
            return "<li><span class='divider'>/</span> <a href='GuildList.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm) + "'>Guilds</a></li>";
        }
        public static string BreadCrumb_AddGuild(WowRealm _Realm, string _Guild)
        {
            return "<li><span class='divider'>/</span> <a href='GuildViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_Realm)
                + "&guild=" + _Guild + "'>" + _Guild + "</a></li>";
        }
        public static string BreadCrumb_AddFinish(string _Name)
        {
            return "<li class='active'><span class='divider'>/</span>" + _Name + "</li>";
        }
        public static string BreadCrumb_AddRealm(WowRealm _Realm)
        {
            return "<li class='active'><span class='divider'>/</span> " + StaticValues.ConvertRealmViewing(_Realm) + "</li>";
        }
        public static string BreadCrumb_AddLink(string _Link, string _Text)
        {
            return "<li><span class='divider'>/</span> <a href='" + _Link + "'>" + _Text + "</a></li>";
        }
        public static string BreadCrumb_AddLink(WowRealm _Realm)
        {
            return "<li class='active'><span class='divider'>/</span> " + StaticValues.ConvertRealmViewing(_Realm) + "</li>";
        }
        public static string BreadCrumb_AddHome()
        {
            return "<li><a href='Index.aspx'>Home</a></li>";
        }
        /// <summary>
        /// Gets the ID of the post back control.
        /// 
        /// See: http://geekswithblogs.net/mahesh/archive/2006/06/27/83264.aspx
        /// </summary>
        /// <param name = "page">The page.</param>
        /// <returns></returns>
        public static string GetPostBackControlId(System.Web.UI.MasterPage page)
        {
            if (!page.IsPostBack)
                return string.Empty;

            System.Web.UI.Control control = null;
            // first we will check the "__EVENTTARGET" because if post back made by the controls
            // which used "_doPostBack" function also available in Request.Form collection.
            string controlName = page.Request.Params["__EVENTTARGET"];
            if (!String.IsNullOrEmpty(controlName))
            {
                control = page.FindControl(controlName);
            }
            else
            {
                // if __EVENTTARGET is null, the control is a button type and we need to
                // iterate over the form collection to find it

                // ReSharper disable TooWideLocalVariableScope
                string controlId;
                System.Web.UI.Control foundControl;
                // ReSharper restore TooWideLocalVariableScope

                foreach (string ctl in page.Request.Form)
                {
                    // handle ImageButton they having an additional "quasi-property" 
                    // in their Id which identifies mouse x and y coordinates
                    if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                    {
                        controlId = ctl.Substring(0, ctl.Length - 2);
                        foundControl = page.FindControl(controlId);
                    }
                    else
                    {
                        foundControl = page.FindControl(ctl);
                    }

                    if (!(foundControl is System.Web.UI.WebControls.Button || foundControl is System.Web.UI.WebControls.ImageButton)) continue;

                    control = foundControl;
                    break;
                }
            }

            return control == null ? String.Empty : control.ID;
        }

        internal static string CreateTalentSpeccLink(VF_RealmPlayersDatabase.WowVersionEnum _WowVersion, PlayerClass _Class, string _TalentsData)
        {
            /*var g_file_classes = {
                  6 : "deathknight",
                  11 : "druid",
                  3 : "hunter",
                  8 : "mage",
                  2 : "paladin",
                  5 : "priest",
                  4 : "rogue",
                  7 : "shaman",
                  9 : "warlock",
                  1: "warrior",
                  31: "druid",
                  23: "hunter",
                  28: "mage",
                  22: "paladin",
                  25: "priest",
                  24: "rogue",
                  27: "shaman",
                  29: "warlock",
                  21: "warrior"
              };*/

            int talentCalculatorClassID = 1;
            int[] talentTreeSizes = new int[] { 22, 21, 22 };
            string[] talentTreeNames = new string[] { "Arms", "Fury", "Protection" };

            if(_WowVersion == VF_RealmPlayersDatabase.WowVersionEnum.Vanilla)
            {
                switch (_Class)
                {
                    case PlayerClass.Unknown:
                        break;
                    case PlayerClass.Druid:
                        talentCalculatorClassID = 11;
                        talentTreeSizes = new int[] { 16, 16, 15 };
                        talentTreeNames = new string[] { "Balance", "Feral", "Restoration" };
                        break;
                    case PlayerClass.Warrior:
                        talentCalculatorClassID = 1;
                        talentTreeSizes = new int[] { 18, 17, 17 };
                        talentTreeNames = new string[] { "Arms", "Fury", "Prot" };
                        break;
                    case PlayerClass.Shaman:
                        talentCalculatorClassID = 7;
                        talentTreeSizes = new int[] { 15, 16, 15 };
                        talentTreeNames = new string[] { "Elemental", "Enhancement", "Restoration" };
                        break;
                    case PlayerClass.Priest:
                        talentCalculatorClassID = 5;
                        talentTreeSizes = new int[] { 15, 16, 16 };
                        talentTreeNames = new string[] { "Discipline", "Holy", "Shadow" };
                        break;
                    case PlayerClass.Mage:
                        talentCalculatorClassID = 8;
                        talentTreeSizes = new int[] { 16, 16, 17 };
                        talentTreeNames = new string[] { "Arcane", "Fire", "Frost" };
                        break;
                    case PlayerClass.Rogue:
                        talentCalculatorClassID = 4;
                        talentTreeSizes = new int[] { 15, 19, 17 };
                        talentTreeNames = new string[] { "Assassination", "Combat", "Subtlety" };
                        break;
                    case PlayerClass.Warlock:
                        talentCalculatorClassID = 9;
                        talentTreeSizes = new int[] { 17, 17, 16 };
                        talentTreeNames = new string[] { "Affliction", "Demonology", "Destruction" };
                        break;
                    case PlayerClass.Hunter:
                        talentCalculatorClassID = 3;
                        talentTreeSizes = new int[] { 16, 14, 16 };
                        talentTreeNames = new string[] { "Beast Mastery", "Marksmanship", "Survival" };
                        break;
                    case PlayerClass.Paladin:
                        talentCalculatorClassID = 2;
                        talentTreeSizes = new int[] { 14, 15, 15 };
                        talentTreeNames = new string[] { "Holy", "Protection", "Retribution" };
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (_Class)
                {
                    case PlayerClass.Unknown:
                        break;
                    case PlayerClass.Druid:
                        talentCalculatorClassID = 31;
                        talentTreeSizes = new int[] { 21, 21, 20 };
                        talentTreeNames = new string[] { "Balance", "Feral", "Restoration" };
                        break;
                    case PlayerClass.Warrior:
                        talentCalculatorClassID = 21;
                        talentTreeSizes = new int[] { 23, 21, 22 };
                        talentTreeNames = new string[] { "Arms", "Fury", "Protection" };
                        break;
                    case PlayerClass.Shaman:
                        talentCalculatorClassID = 27;
                        talentTreeSizes = new int[] { 20, 21, 20 };
                        talentTreeNames = new string[] { "Elemental", "Enhancement", "Restoration" };
                        break;
                    case PlayerClass.Priest:
                        talentCalculatorClassID = 25;
                        talentTreeSizes = new int[] { 22, 21, 21 };
                        talentTreeNames = new string[] { "Discipline", "Holy", "Shadow" };
                        break;
                    case PlayerClass.Mage:
                        talentCalculatorClassID = 28;
                        talentTreeSizes = new int[] { 23, 22, 22 };
                        talentTreeNames = new string[] { "Arcane", "Fire", "Frost" };
                        break;
                    case PlayerClass.Rogue:
                        talentCalculatorClassID = 24;
                        talentTreeSizes = new int[] { 21, 24, 22 };
                        talentTreeNames = new string[] { "Assassination", "Combat", "Subtlety" };
                        break;
                    case PlayerClass.Warlock:
                        talentCalculatorClassID = 29;
                        talentTreeSizes = new int[] { 21, 22, 21 };
                        talentTreeNames = new string[] { "Affliction", "Demonology", "Destruction" };
                        break;
                    case PlayerClass.Hunter:
                        talentCalculatorClassID = 23;
                        talentTreeSizes = new int[] { 21, 20, 23 };
                        talentTreeNames = new string[] { "Beast Mastery", "Marksmanship", "Survival" };
                        break;
                    case PlayerClass.Paladin:
                        talentCalculatorClassID = 22;
                        talentTreeSizes = new int[] { 20, 22, 22 };
                        talentTreeNames = new string[] { "Holy", "Protection", "Retribution" };
                        break;
                    default:
                        break;
                }
            }

            string talentSpeccSummary = "??/??/??";
            if (_TalentsData.Length == talentTreeSizes[0] + talentTreeSizes[1] + talentTreeSizes[2])
            {
                string specc1 = _TalentsData.Substring(0, talentTreeSizes[0]);
                string specc2 = _TalentsData.Substring(talentTreeSizes[0], talentTreeSizes[1]);
                string specc3 = _TalentsData.Substring(talentTreeSizes[0] + talentTreeSizes[1], talentTreeSizes[2]);
                
                int specc1Sum = specc1.Sum((_Value) => int.Parse("" + _Value));
                int specc2Sum = specc2.Sum((_Value) => int.Parse("" + _Value));
                int specc3Sum = specc3.Sum((_Value) => int.Parse("" + _Value));

                talentSpeccSummary = specc1Sum.ToString() + "/" + specc2Sum.ToString() + "/" + specc3Sum.ToString();

                if(specc1Sum >= specc2Sum)
                {
                    if(specc1Sum >= specc3Sum)
                    {
                        talentSpeccSummary = talentTreeNames[0] + " (" + talentSpeccSummary + ")";
                    }
                    else
                    {
                        talentSpeccSummary = talentTreeNames[2] + " (" + talentSpeccSummary + ")";
                    }
                }
                else
                {
                    if (specc2Sum >= specc3Sum)
                    {
                        talentSpeccSummary = talentTreeNames[1] + " (" + talentSpeccSummary + ")";
                    }
                    else
                    {
                        talentSpeccSummary = talentTreeNames[2] + " (" + talentSpeccSummary + ")";
                    }
                }
            }

            return PageUtility.CreateLink("Talents.aspx#" + talentCalculatorClassID + "-" + _TalentsData, talentSpeccSummary);
        }
    }
}