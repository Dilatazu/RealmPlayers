using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;

using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;
using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;

namespace RealmPlayersServer
{
    public partial class CharacterList : System.Web.UI.Page
    {
        static PlayerColumn[] Table_Columns = new PlayerColumn[]{
            PlayerColumn.Rank,
            PlayerColumn.Name,
            PlayerColumn.Guild,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.Total_HKs,
            PlayerColumn.HonorLastWeek,
            PlayerColumn.HKLastWeek,
            PlayerColumn.Server,
            PlayerColumn.LastSeen,
        };
        static GuildColumn[] Table_GuildColumns = new GuildColumn[]{
            GuildColumn.Name,
            GuildColumn.MemberCount,
            GuildColumn.Level60MemberCount,
            GuildColumn.TotalHKs,
            GuildColumn.AverageRank,
            GuildColumn.Server,
        };
        static PlayerColumn[] Table_ColumnsTBC = new PlayerColumn[]{
            PlayerColumn.Name,
            PlayerColumn.Guild,
            PlayerColumn.Level,
            PlayerColumn.Race_And_Class,
            PlayerColumn.Total_HKs,
            PlayerColumn.Rating_2v2,
            PlayerColumn.Rating_3v3,
            PlayerColumn.Rating_5v5,
            PlayerColumn.Server,
            PlayerColumn.LastSeen,
        };
        static GuildColumn[] Table_GuildColumnsTBC = new GuildColumn[]{
            GuildColumn.Name,
            GuildColumn.MemberCount,
            GuildColumn.Level70MemberCount,
            GuildColumn.TotalHKs,
            GuildColumn.Server,
        };
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_CharListInfoHTML = null;
        public MvcHtmlString m_GuildResultTableHTML = null;
        public MvcHtmlString m_TableHeadHTML = null;
        public MvcHtmlString m_TableBodyHTML = null;
        public MvcHtmlString m_PaginationHTML = null;

        public string CreateCharListInfo(string _SearchStr, int _CharCount)
        {
            string charListInfo
                = "<h1>Search results for &quot;" + _SearchStr + "&quot;<span class='badge badge-inverse'>" + _CharCount + " Matches Found</span></h1>"
                + "<p>List contains the guilds/players with a name matching search string &quot;" + _SearchStr + "&quot;. This is website wide search containing matches from all the different realms.</p>";

            return charListInfo;
        }
        public enum PlayersMatchingSortBy
        {
            SortBy_NameSearch,
            SortBy_Rank,
            SortBy_Name,
            SortBy_Guild,
            SortBy_Level,
            SortBy_RaceClass,
            SortBy_TotalHKs,
            SortBy_Honor,
            SortBy_HKs,
            SortBy_Server,
            SortBy_Seen,
        }
        public Player[] _NotCached_FindPlayersMatching(string _PartOfName, WowRealm _Realm, string _Race, string _Class, string _Level, PlayersMatchingSortBy _SortBy)
        {
            int lowerLevel = int.MinValue;
            int upperLevel = int.MaxValue;
            if (_Level != "All")
            {
                try
                {
                    string[] levelsStr = _Level.SplitVF("to");
                    if (levelsStr.Length >= 2)
                    {
                        lowerLevel = int.Parse(levelsStr[0]);
                        upperLevel = int.Parse(levelsStr[1]);
                    }
                }
                catch (Exception)
                { }
            }
            PlayerRace[] races = null;
            if (_Race != "All")
            {
                try
                {
                    if (_Race == "Horde")
                        races = new PlayerRace[] { PlayerRace.Orc, PlayerRace.Undead, PlayerRace.Troll, PlayerRace.Tauren, PlayerRace.Blood_Elf };
                    else if (_Race == "Alliance")
                        races = new PlayerRace[] { PlayerRace.Human, PlayerRace.Night_Elf, PlayerRace.Gnome, PlayerRace.Dwarf, PlayerRace.Draenei };
                    else
                    {
                        PlayerRace currRace = StaticValues.ConvertRace(_Race);
                        if (currRace != PlayerRace.Unknown)
                            races = new PlayerRace[] { currRace };
                    }
                }
                catch (Exception)
                { }
            }
            PlayerClass[] classes = null;
            if (_Class != "All")
            {
                var currClass = StaticValues.ConvertClass(_Class);
                if (currClass != PlayerClass.Unknown)
                    classes = new PlayerClass[] { currClass };
            }

            List<Player> playerMatchList = new List<Player>(100000);
            foreach (var _RealmDB in DatabaseAccess.GetRealmDBs(this))
            {
                if (_Realm != WowRealm.All && _Realm != _RealmDB.Key)
                    continue;

                foreach (var player in _RealmDB.Value.Players)
                {
                    if (races != null && races.Contains(player.Value.Character.Race) == false)
                        continue;
                    if (classes != null && classes.Contains(player.Value.Character.Class) == false)
                        continue;
                    if (player.Value.Character.Level < lowerLevel || player.Value.Character.Level > upperLevel)
                        continue;
                    if (player.Value.Character.Race == PlayerRace.Unknown || player.Value.Character.Class == PlayerClass.Unknown)
                        continue;
                    if (_PartOfName == "" || player.Key.ToLower().Contains(_PartOfName) == true)
                    {
                        playerMatchList.Add(player.Value);
                    }
                }
            }
            if (_SortBy == PlayersMatchingSortBy.SortBy_NameSearch)
            {
                if (_PartOfName.Length >= 2)
                {
                    string nameFormattedSearchStr = char.ToUpper(_PartOfName[0]) + _PartOfName.Substring(1).ToLower();
                    return playerMatchList.OrderByDescending(_Player => {
                        var sortValue = _Player.LastSeen;
                        if(_Player.Name.StartsWith(nameFormattedSearchStr))
                        {
                            sortValue = sortValue.AddYears(2);
                            if(_Player.Name == nameFormattedSearchStr)
                            {
                                sortValue = sortValue.AddYears(10);
                            }
                        }
                        return sortValue;
                    }).ToArray();
                }
                else
                {
                    return playerMatchList.OrderBy(_Player => _Player.Name).ToArray();
                }
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_Rank)
            {
                return playerMatchList.OrderByDescending(_Player => _Player.GetRankTotal()).ToArray();
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_Name)
            {
                return playerMatchList.OrderBy(_Player => _Player.Name).ToArray();
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_Guild)
            {
                return playerMatchList.OrderBy(_Player => _Player.Guild.GuildName).ToArray();
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_Level)
            {
                return playerMatchList.OrderByDescending(_Player => _Player.Character.Level).ToArray();
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_RaceClass)
            {
                return playerMatchList.OrderBy(_Player => ((int)_Player.Character.Race) * 100 + (int)_Player.Character.Class).ToArray();
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_TotalHKs)
            {
                return playerMatchList.OrderByDescending(_Player => _Player.Honor.LifetimeHK).ToArray();
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_Honor)
            {
                return playerMatchList.OrderByDescending(_Player => _Player.Honor.LastWeekHonor).ToArray();
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_HKs)
            {
                return playerMatchList.OrderByDescending(_Player => _Player.Honor.LastWeekHK).ToArray();
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_Server)
            {
                return playerMatchList.OrderByDescending(_Player => _Player.Realm).ToArray();
            }
            else if (_SortBy == PlayersMatchingSortBy.SortBy_Seen)
            {
                return playerMatchList.OrderByDescending(_Player => _Player.LastSeen).ToArray();
            }
            else
            {
                return playerMatchList.ToArray();
            }
        }
        public Player[] FindPlayersMatching(string _PartOfName, string _Realm, string _Race, string _Class, string _Level, string _SortBy = "NameSearch")
        {
            //FIX PARAMS
            _PartOfName = _PartOfName.ToLower();

            WowRealm realm = StaticValues.ConvertRealm(_Realm);
            if (realm == WowRealm.Unknown)
                realm = WowRealm.All;

            int lowerLevel = int.MinValue;
            int upperLevel = int.MaxValue;
            if (_Level != "All")
            {
                try
                {
                    string[] levelsStr = _Level.SplitVF("to");
                    if (levelsStr.Length >= 2)
                    {
                        lowerLevel = int.Parse(levelsStr[0]);
                        upperLevel = int.Parse(levelsStr[1]);
                    }
                }
                catch (Exception)
                { }
            }

            if (lowerLevel < 1 || upperLevel > 60 || upperLevel < lowerLevel)
                _Level = "All";
            else
                _Level = lowerLevel + "to" + upperLevel;

            PlayerRace[] races = null;
            if (_Race != "All")
            {
                try
                {
                    if (_Race == "Horde")
                        races = new PlayerRace[] { PlayerRace.Orc, PlayerRace.Undead, PlayerRace.Troll, PlayerRace.Tauren, PlayerRace.Blood_Elf };
                    else if (_Race == "Alliance")
                        races = new PlayerRace[] { PlayerRace.Human, PlayerRace.Night_Elf, PlayerRace.Gnome, PlayerRace.Dwarf, PlayerRace.Draenei };
                    else
                    {
                        PlayerRace currRace = StaticValues.ConvertRace(_Race);
                        if (currRace != PlayerRace.Unknown)
                        {
                            races = new PlayerRace[] { currRace };
                            _Race = currRace.ToString();
                        }

                    }
                }
                catch (Exception)
                { }
            }
            if (races == null)
                _Race = "All";

            PlayerClass[] classes = null;
            if (_Class != "All")
            {
                var currClass = StaticValues.ConvertClass(_Class);
                if (currClass != PlayerClass.Unknown)
                {
                    classes = new PlayerClass[] { currClass };
                    _Class = currClass.ToString();
                }
            }

            if (classes == null)
                _Class = "All";

            PlayersMatchingSortBy sortBy = PlayersMatchingSortBy.SortBy_NameSearch;
            if(Enum.TryParse("SortBy_" + _SortBy, true, out sortBy) == false)
                sortBy = PlayersMatchingSortBy.SortBy_NameSearch;
            //FIX PARAMS

            if(_PartOfName.Length >= 3 || (_Race != "All" && _Class != "All"))
                return _NotCached_FindPlayersMatching(_PartOfName, realm, _Race, _Class, _Level, sortBy);
            else
                return Hidden.ApplicationInstance.Instance.m_ThreadSafeCache.Get("FindPlayersMatching", _NotCached_FindPlayersMatching, _PartOfName, realm, _Race, _Class, _Level, sortBy);
            
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            int pageNr = PageUtility.GetQueryInt(Request, "page", 1);
            int pageIndex = pageNr - 1;//Change range from 0 to * instead of 1 to *
            int count = PageUtility.GetQueryInt(Request, "count", 100);
            if (count > 500) count = 500;

            string searchStr = PageUtility.GetQueryString(Request, "search", "null");
            string searchRealm = PageUtility.GetQueryString(Request, "realm", "All");
            string searchRace = PageUtility.GetQueryString(Request, "race", "All");
            string searchClass = PageUtility.GetQueryString(Request, "class", "All");
            string searchLevel = PageUtility.GetQueryString(Request, "level", "All");
            string searchSort = PageUtility.GetQueryString(Request, "sort", "NameSearch");

            if(searchStr != "")
                this.Title = "Search \"" + searchStr + "\" | RealmPlayers";
            else
                this.Title = "Search | RealmPlayers";
            //List<Tuple<WowRealm, Player>>

            WowRealm realm = StaticValues.ConvertRealm(searchRealm);
            if (realm == WowRealm.Unknown)
                realm = WowRealm.All;

            int nr = 0;

            if (searchStr.Length >= 4 || searchStr.Contains(' '))
            {
                var guildSummaryDB = Hidden.ApplicationInstance.Instance.GetGuildSummaryDatabase();

                var searchStrLower = searchStr.ToLower();
                var guildArray = guildSummaryDB.GetGuilds(realm).Where((_Value) => _Value.Value.GuildName.ToLower().Contains(searchStrLower));

                if (guildArray.Count() > pageIndex * count)
                {
                    Dictionary<string, Player> realmDB = null;
                    if(realm != WowRealm.All)
                        realmDB = DatabaseAccess.GetRealmPlayers(this, realm);
                    var realmDBRealm = realm;

                    System.Text.StringBuilder guildResultTable = new System.Text.StringBuilder(10000);
                    guildResultTable.Append("<h3>Guilds matching &quot;" + searchStr + "&quot;</h3>");
                    guildResultTable.Append("<table id=\"guild-table\" class=\"table\"><thead>");
                    guildResultTable.Append(PageUtility.CreateGuildTableHeaderRow(Table_GuildColumns));
                    guildResultTable.Append("</thead><tbody>");
                    var orderedGuildArray = guildArray.OrderBy((_Value) => _Value.Key);
                    foreach (var guild in orderedGuildArray)
                    {
                        if (realmDBRealm != guild.Value.Realm)
                        {
                            realmDB = DatabaseAccess.GetRealmPlayers(this, guild.Value.Realm);
                            realmDBRealm = guild.Value.Realm;
                        }
                        guild.Value.GenerateCache(realmDB);
                    }
                    orderedGuildArray = orderedGuildArray.OrderByDescending((_Value) => _Value.Value.GetMembers().Count);
                    foreach (var guild in orderedGuildArray)
                    {
                        if (guild.Value.GetMembers().Count < 2)
                            continue;
                        nr++;
                        if (nr > pageIndex * count && nr <= (pageIndex + 1) * count)
                        {
                            guildResultTable.Append(PageUtility.CreateGuildRow(0, guild.Value, Table_GuildColumns));
                        }
                        if (nr >= (pageIndex + 1) * count)
                            break;
                    }
                    guildResultTable.Append("</tbody></table>");
                    guildResultTable.Append("<h3>Players matching &quot;" + searchStr + "&quot;</h3>");
                    if(nr > 0)
                        m_GuildResultTableHTML = new MvcHtmlString(guildResultTable.ToString());
                }
                else
                {
                    m_GuildResultTableHTML = null;
                    nr += guildArray.Count();
                }
            }

            var playersList = FindPlayersMatching(searchStr, searchRealm, searchRace, searchClass, searchLevel, searchSort);//.OrderBy((_Value) => { return ((int)_Value.Item1).ToString() + _Value.Item2.Name; });
            System.Text.StringBuilder page = new System.Text.StringBuilder(10000);
            
            IEnumerable<Player> orderedPlayersList = playersList;
            //if (playersList.Count() > 500 || searchStr.Length < 2)
            //    orderedPlayersList = playersList;//.OrderBy(_Player => _Player.Item2.Name);
            //else
            //if (searchStr.Length >= 1)
            //{
            //    string nameFormattedSearchStr = char.ToUpper(searchStr[0]) + searchStr.Substring(1).ToLower();
            //    orderedPlayersList = playersList.OrderByDescending(_Player => _Player.LastSeen.AddYears(_Player.Name.StartsWith(nameFormattedSearchStr) ? 2 : 0));
            //}
            //else
            //{
            //    orderedPlayersList = playersList.OrderBy(_Player => _Player.Name);
            //}
            var playerListCount = playersList.Count();
            foreach (Player player in orderedPlayersList)
            {
                if (playerListCount < count && player.Character.Level < 10 && searchStr.Length <= (double)player.Name.Length * 0.7)
                    continue;

                nr++;
                if (nr > pageIndex * count && nr <= (pageIndex + 1) * count)
                {
                    page.Append(PageUtility.CreatePlayerRow(0, player.Realm, player, Table_Columns));
                }
                if (nr >= (pageIndex + 1) * count)
                    break;
            }
            if (nr != 0 && nr <= pageIndex * count)
            {
                pageIndex = (nr - 1) / count;
                Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "page", (pageIndex + 1).ToString()));
            }

            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome() + PageUtility.BreadCrumb_AddFinish("Characters"));
            m_CharListInfoHTML = new MvcHtmlString(CreateCharListInfo(searchStr, playersList.Count()));
            m_TableHeadHTML = new MvcHtmlString(PageUtility.CreatePlayerTableHeaderRow(Table_Columns, null, Request));
            m_TableBodyHTML = new MvcHtmlString(page.ToString());

            m_PaginationHTML = new MvcHtmlString(PageUtility.CreatePagination(Request, pageNr, ((playersList.Count()-1)/count) + 1));
        }
    }
}