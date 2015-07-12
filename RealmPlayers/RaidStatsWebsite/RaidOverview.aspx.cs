using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace VF.RaidDamageWebsite
{
    public partial class RaidOverview : System.Web.UI.Page
    {
        class DataPresentTypeInfo
        {
            public Func<VF_RaidDamageDatabase.UnitData, double> m_GetValue;
            public string m_TypeName;
            public int m_Count;
            public Func<VF_RaidDamageDatabase.UnitData, bool> m_ValidCheck;
            public DataPresentTypeInfo(Func<VF_RaidDamageDatabase.UnitData, double> _GetValue, string _TypeName, int _Count = -1, Func<VF_RaidDamageDatabase.UnitData, bool> _ValidCheck = null)
            {
                m_GetValue = _GetValue;
                m_TypeName = _TypeName;
                m_Count = _Count;
                if (m_Count < 0)
                    m_Count = 15;
                m_ValidCheck = _ValidCheck;
                if (m_ValidCheck == null)
                    m_ValidCheck = (_Value) => { return m_GetValue(_Value) > 0; };
            }
        }

        static List<DataPresentTypeInfo> m_DataPresentTypeInfoList = new List<DataPresentTypeInfo>{
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.Dmg; }, "Damage", 25),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.DPS; }, "DPS", 10, (VF_RaidDamageDatabase.UnitData _UnitData) => { return _UnitData.Dmg > 0; }),
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.EffHeal; }, "Efficient Heal"),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.HPS; }, "HPS", 10, (VF_RaidDamageDatabase.UnitData _UnitData) => { return _UnitData.EffHeal > 0; }),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.OverHeal; }, "Overheal"),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.RawHeal; }, "Raw Heal"),
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.OverHeal; }, "Overheal"),
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.RawHeal; }, "Raw Heal"),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.ThreatValue; }, "Threat", 25),
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.DmgTaken; }, "Damage Taken", 25),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.Dmg; }, "Damage"),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.Death; }, "Deaths", 999),
        };

        public MvcHtmlString m_BreadCrumbHTML = null;
        public string m_RaidOverviewInfoHTML = "";
        public MvcHtmlString m_TableHeadHTML = null;
        public MvcHtmlString m_TableBodyHTML = null;
        public MvcHtmlString m_TrashHTML = null;

        public MvcHtmlString m_GraphSection = null;
        
        //public static bool derp = false;
        //public static void ValidateCacheOutput(HttpContext context, Object data,
        //    ref HttpValidationStatus status)
        //{
        //    if (derp == true)
        //        status = HttpValidationStatus.Valid;
        //    else
        //        status = HttpValidationStatus.Invalid;
        //    if (context.Request.QueryString["Status"] != null)
        //    {
        //        string pageStatus = context.Request.QueryString["Status"];

        //        if (pageStatus == "invalid")
        //            status = HttpValidationStatus.Invalid;
        //        else if (pageStatus == "ignore")
        //            status = HttpValidationStatus.IgnoreThisRequest;
        //        else
        //            status = HttpValidationStatus.Valid;
        //    }
        //    else
        //        status = HttpValidationStatus.Valid;
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Cache.AddValidationCallback(
            //    new HttpCacheValidateHandler(ValidateCacheOutput),
            //    null);
            //Response.Cache.SetExpires(DateTime.Now.AddSeconds(600));
            //Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            //Response.Cache.SetValidUntilExpires(true);

            bool filteredData = PageUtility.GetQueryString(Request, "Filtered", "true") == "true";
            int uniqueRaidID = PageUtility.GetQueryInt(Request, "Raid", -1);
            if (uniqueRaidID == -1)
                Response.Redirect("RaidList.aspx");

            this.Title = "Raid " + uniqueRaidID + " | RaidStats";

#if false//LIMIT_USERS_USING_LOGIN_SYSTEM
            if((DateTime.Now - ApplicationInstance.Instance.GetRaidDate(uniqueRaidID)).TotalDays > 30)
            {
                var user = Authentication.GetSessionUser(Page, true);
                if(user.IsPremium() == false)
                {
                    m_TrashHTML = new MvcHtmlString("Sorry. Raids that are older than 30 days are only viewable for Premium users!");
                    return;
                }
            }
#endif

            var orderedFights = ApplicationInstance.Instance.GetRaidBossFights(uniqueRaidID);
            if (orderedFights == null || orderedFights.Count() == 0)
                Response.Redirect("RaidList.aspx");

            var orderedTrashFights = ApplicationInstance.Instance.GetRaidTrashFights(uniqueRaidID);

            var realmDB = ApplicationInstance.Instance.GetRealmDB(orderedFights.First().GetRaid().Realm);

            var currRaid = orderedFights.First().GetRaid();
            if (currRaid.Realm == VF_RealmPlayersDatabase.WowRealm.Test_Server && PageUtility.GetQueryString(Request, "Debug") == "null")
                Response.Redirect("RaidList.aspx");
            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                + PageUtility.BreadCrumb_AddRaidList()
                + PageUtility.BreadCrumb_AddLink("RaidList.aspx?Guild=" + currRaid.RaidOwnerName + "&realm=" + StaticValues.ConvertRealmParam(realmDB.Realm), currRaid.RaidOwnerName)
                    + PageUtility.BreadCrumb_AddFinish(currRaid.RaidInstance + "(" + currRaid.RaidID.ToString() + ")"));

            bool displayLoot = false;
            if (orderedFights.FindIndex((_Value) => _Value.GetItemDrops().Count > 0) != -1)
                displayLoot = true;

            m_TableHeadHTML = new MvcHtmlString(
                PageUtility.CreateTableRow("",
                PageUtility.CreateTableColumnHead("#Nr") +
                PageUtility.CreateTableColumnHead("Boss") +
                PageUtility.CreateTableColumnHead("Players") +
                (displayLoot == true ? PageUtility.CreateTableColumnHead("Items Dropped") : "") +
                //PageUtility.CreateTableColumnHead(PageUtility.CreateTooltipText("Recorded Damage", "Recorded damage done by players during the fight")) +
                PageUtility.CreateTableColumnHead("Player Deaths") +
                PageUtility.CreateTableColumnHead("Fight Duration") +
                PageUtility.CreateTableColumnHead("Kill Time") +
                PageUtility.CreateTableColumnHead(PageUtility.CreateTooltipText("Precision", "How much percentage of the recorded fight is vs the boss intended. Calculated using the formula: Boss+Adds DmgTaken/Recorded Damage"))));


            List<string> attendingRaidPlayers = new List<string>();
            string tableBody = "";
            foreach (var fight in orderedFights)
            {
                var attendingFightPlayers = fight.GetAttendingUnits((_Name) => { return realmDB.RD_IsPlayer(_Name, fight); });
                attendingRaidPlayers.AddRange(attendingFightPlayers);

                double precision = fight.CalculatePrecision(realmDB.RD_IsPlayerFunc(fight));

                var attemptType = fight.GetFightData().GetAttemptType();
                string attemptStr = "";
                if (attemptType == VF_RaidDamageDatabase.FightData.AttemptType.KillAttempt)
                    attemptStr = "(kill)";
                else if (attemptType == VF_RaidDamageDatabase.FightData.AttemptType.WipeAttempt)
                    attemptStr = "(attempt)";

                string trashPercentageStr = "<font color='#FF0000'>???</font>";
                if (precision != 0)
                {
                    double trashPercentage = precision;
                    if (precision >= 1.0)
                        precision = 1.0;
                    trashPercentageStr = PageUtility.CreateColorisedPercentage(precision);
                }
                var realm = fight.GetRaid().Realm;
                ///////////////////////
                string lootDropped = "";
                if (attemptType == VF_RaidDamageDatabase.FightData.AttemptType.KillAttempt)
                {
                    lootDropped = LootGenerator.CreateLootDroppedData(fight.GetItemDrops(), realmDB, ApplicationInstance.Instance.GetItemSummaryDatabase(), ApplicationInstance.Instance.GetItemInfo);
                }
                ///////////////////////

                tableBody += PageUtility.CreateTableRow("", PageUtility.CreateTableColumn("#" + (fight.GetRaidBossFightIndex() + 1)) +
                    PageUtility.CreateTableColumn(PageUtility.CreateLink("FightOverview.aspx?Raid=" + uniqueRaidID + "&Fight=" + fight.GetRaidBossFightIndex(), "<font color='#f70002'>" + fight.GetFightData().FightName + "</font>") + attemptStr) +
                    PageUtility.CreateTableColumn(attendingFightPlayers.Count.ToString()) +
                    (displayLoot == true ? PageUtility.CreateTableColumn(lootDropped) : "") +
                    //PageUtility.CreateTableColumn(((int)(totalValue / 1000)).ToString() + "k") +
                    PageUtility.CreateTableColumn(((int)fight.GetTotal((_Value) => { return _Value.I.Death; }
                            , (_Value) => { return realmDB.RD_IsPlayer(_Value.Item1, fight) && _Value.Item2.I.Death > 0; })).ToString()) +
                    PageUtility.CreateTableColumn(fight.GetFightDuration().ToString() + " sec") +
                    PageUtility.CreateTableColumn(fight.GetFightData().StartDateTime.AddSeconds(fight.GetFightData().GetFightRecordDuration()).ToLocalTime().ToString("yyy-MM-dd HH:mm:ss")) +
                    PageUtility.CreateTableColumn(trashPercentageStr));//PageUtility.CreateTooltipText(trashPercentageStr, (bossPlusAddsDmgTaken != 0 ? bossPlusAddsDmgTaken.ToString() : "???") + "/" + totalValue.ToString())));
            }
            m_TableBodyHTML = new MvcHtmlString(tableBody);

            /////////////////////
            List<Tuple<string, VF_RealmPlayersDatabase.PlayerClass>> playersAttending = new List<Tuple<string, VF_RealmPlayersDatabase.PlayerClass>>();

            foreach (var playerName in attendingRaidPlayers.Distinct())
            {
                var playerData = realmDB.RD_FindPlayer(playerName, attendingRaidPlayers);
                if (playerData != null)
                    playersAttending.Add(new Tuple<string, VF_RealmPlayersDatabase.PlayerClass>(playerName, playerData.Character.Class));
            }
            string playersAttendingStr = "<h3>Players attending(" + playersAttending.Count + "):</h3>";
            var orderedPlayersAttending = playersAttending.OrderBy((_Value) => { return "" + (int)_Value.Item2 + _Value.Item1; });

            try
            {
                var lastClass = orderedPlayersAttending.First().Item2;
                foreach (var player in orderedPlayersAttending)
                {
                    if (lastClass != player.Item2)
                        playersAttendingStr += "<br />";
                    playersAttendingStr += PageUtility.CreateColorCodedName(player.Item1, player.Item2) + " ";
                    lastClass = player.Item2;
                }
            }
            catch(Exception ex)
            {
                
            }
            /////////////////////

            var recordedBy = currRaid.GetRecordedByPlayers();
            string recordedByString = "";
            foreach (var playerName in recordedBy)
            {
                var playerData = realmDB.RD_FindPlayer(playerName, attendingRaidPlayers);
                if (playerData != null)
                {
                    if (recordedByString == "")
                        recordedByString += " by ";
                    recordedByString += PageUtility.CreateColorCodedName(playerData) + ", ";
                }
            }
            if (recordedByString != "")
            {
                recordedByString = recordedByString.Substring(0, recordedByString.Length - 2); //Get rid of last ", "
                if (recordedByString.Contains(", "))
                {
                    var subStr = recordedByString.Substring(recordedByString.LastIndexOf(", "));
                    var replacement = subStr.Replace(", ", " and ");
                    recordedByString = recordedByString.Replace(subStr, replacement);
                }
            }

            var startRecordTime = orderedFights.First().GetFightData().StartDateTime.ToLocalTime();
            var endRecordTime = orderedFights.Last().GetFightData().GetEndDateTime().ToLocalTime();

            var totalRecordTime = " (" + ((int)(endRecordTime - startRecordTime).TotalMinutes) + " minutes and " + (endRecordTime - startRecordTime).Seconds + " seconds)";
            
            m_RaidOverviewInfoHTML = "<h1>" + currRaid.RaidOwnerName + " vs "
                + currRaid.RaidInstance + "(" + currRaid.RaidID.ToString() + ")<span class='badge badge-inverse'>" + orderedFights.Count() + " fights</span></h1>"
                + "<p>Fights recorded" + recordedByString + " between " + startRecordTime.ToString("yyy-MM-dd HH:mm:ss") + " and "
                + endRecordTime.ToString("yyy-MM-dd HH:mm:ss") + totalRecordTime + "</p>" + playersAttendingStr;

            string graphStyle = "<style>" + PageUtility.CreateStatsBars_HTML_CSSCode() + "</style>";
            string totalBossMeters = "<h2>Damage/Healing total(only bosses)</h2>" +
                "<p>Total for all boss fights";
            string totalTrashMeters = "<h2>Damage/Healing total(only trash)</h2>" +
                "<p>Total for all trash fights";

            if (filteredData == true)
            {
                totalBossMeters += ", unrealistic dmg/heal spikes are filtered. <a href='"
                    + PageUtility.CreateUrlWithNewQueryValue(Request, "Filtered", "false")
                    + "'>View Unfiltered</a></p><br />";
                totalTrashMeters += ", unrealistic dmg/heal spikes are filtered. <a href='"
                    + PageUtility.CreateUrlWithNewQueryValue(Request, "Filtered", "false")
                    + "'>View Unfiltered</a></p><br />";
            }
            else
            {
                totalBossMeters += ", unrealistic dmg/heal spikes are not filtered. <a href='"
                    + PageUtility.CreateUrlWithNewQueryValue(Request, "Filtered", "true")
                    + "'>View Filtered</a></p><br />";
                totalTrashMeters += ", unrealistic dmg/heal spikes are not filtered. <a href='"
                    + PageUtility.CreateUrlWithNewQueryValue(Request, "Filtered", "true")
                    + "'>View Filtered</a></p><br />";
            }

            totalBossMeters += GenerateTotalMeters(filteredData, orderedFights, realmDB, attendingRaidPlayers);
            if (orderedTrashFights.Count > 0)
            {
                totalTrashMeters += GenerateTotalMeters(filteredData, orderedTrashFights, realmDB, attendingRaidPlayers);
            }
            else
            {
                totalTrashMeters = "";
            }
            m_GraphSection = new MvcHtmlString(graphStyle + "<div class='blackframe'>" + totalBossMeters + "</div><br/><div class='blackframe'>" + totalTrashMeters + "</div>");

            //TRASH HANDLING
            {
                if (orderedTrashFights.Count > 0)
                {
                    System.Text.StringBuilder trashSection = new System.Text.StringBuilder(4000);

                    trashSection.Append("<div class='row'><div class='span12'><table class='table'><thead>");
                    trashSection.Append(PageUtility.CreateTableRow("",
                        PageUtility.CreateTableColumnHead("Trash") +
                        PageUtility.CreateTableColumnHead("Players") +
                        PageUtility.CreateTableColumnHead("Player Deaths") +
                        PageUtility.CreateTableColumnHead("Trash Duration")));

                    trashSection.Append("</thead><tbody>");

                    foreach (var fight in orderedTrashFights)
                    {
                        var attendingFightPlayers = fight.GetAttendingUnits((_Name) => { return realmDB.RD_IsPlayer(_Name, attendingRaidPlayers); });

                        var endTime = fight.GetStartDateTime().AddSeconds(fight.GetFightData().GetFightRecordDuration());
                        trashSection.Append(PageUtility.CreateTableRow("",
                                PageUtility.CreateTableColumn(PageUtility.CreateLink("FightOverview.aspx?Raid=" + uniqueRaidID + "&Trash=" + fight.GetRaidBossFightIndex(), "<font color='#f70002'>" + fight.GetStartDateTime().ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " to " + endTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "</font>")) +
                                PageUtility.CreateTableColumn(attendingFightPlayers.Count.ToString()) +
                                PageUtility.CreateTableColumn(((int)fight.GetTotal((_Value) => { return _Value.I.Death; }
                                        , (_Value) => { return realmDB.RD_IsPlayer(_Value.Item1, attendingRaidPlayers) && _Value.Item2.I.Death > 0; })).ToString()) +
                                PageUtility.CreateTableColumn((int)(endTime - fight.GetStartDateTime()).TotalMinutes + " min")
                            ));
                    }

                    trashSection.Append("</tbody></table></div></div>");

                    m_TrashHTML = new MvcHtmlString(trashSection.ToString());
                }
            }

        }

        public static string GenerateTotalMeters(bool filteredData, List<VF_RaidDamageDatabase.RaidBossFight> orderedFights, VF_RaidDamageDatabase.RealmDB realmDB, List<string> attendingRaidPlayers)
        {
            string graphSection = "";
            List<Tuple<string, VF_RaidDamageDatabase.UnitData>> fullUnitDatas = new List<Tuple<string, VF_RaidDamageDatabase.UnitData>>();
            {
                PageUtility.StatsBarStyle statsBarStyle = new PageUtility.StatsBarStyle
                {
                    m_TitleText = "",
                    m_BarTextColor = "#000",
                    m_LeftSideTitleText = "#",
                    m_RightSideTitleText = "",
                    m_BeforeBarWidth = 25,
                    m_MaxWidth = 470,
                    m_AfterBarWidth = 30
                };

                string dmgThreatSection = "<div class='span4' style='min-width: 460px;'>";
                string healSection = "<div class='span4' style='min-width: 460px;'>";
                int totalBarsCount = 0;
                int totalFightDurations = 0;
                foreach (var fight in orderedFights)
                {
                    List<Tuple<string, VF_RaidDamageDatabase.UnitData>> unitsData = null;
                    if (filteredData == true)
                    {
                        unitsData = fight.GetFilteredPlayerUnitsDataCopy(true, realmDB.RD_GetPlayerIdentifierFunc(fight));
                    }
                    else
                    {
                        unitsData = fight.GetUnitsDataCopy(true);
                    }
                    var petData = fight.GetFilteredPetUnitsData();
                    List<Tuple<string, VF_RaidDamageDatabase.UnitData>> abusingPets = new List<Tuple<string, VF_RaidDamageDatabase.UnitData>>();
                    foreach (var unitPet in petData)
                    {
                        if (fight.GetFightCacheData().m_FightDataCollection.m_RaidMembers.Contains(unitPet.Item1.Split('(').First()) == true)
                        {
                            //Player with Pet UnitPet should be banned from damagemeter or has its damage purged
                            string abusingPlayer = unitPet.Item1.Split(new string[] { "(Pet for ", ")" }, StringSplitOptions.RemoveEmptyEntries)[1];
                            abusingPets.Add(Tuple.Create(abusingPlayer, unitPet.Item2));
                        }
                    }
                    unitsData.AddRange(petData);
                    foreach (var unit in unitsData)
                    {
                        int index = fullUnitDatas.FindIndex((_Value) => { return _Value.Item1 == unit.Item1; });
                        var unitFightData = unit.Item2.CreateCopy();
                        var petsAbused = abusingPets.FindAll((_Value) => { return _Value.Item1 == unit.Item1; });
                        if (petsAbused != null && petsAbused.Count > 0)
                        {
                            foreach (var petAbused in petsAbused)
                            {
                                unitFightData.SubtractUnitData(petAbused.Item2);
                            }
                        }
                        if (index == -1)
                            fullUnitDatas.Add(new Tuple<string, VF_RaidDamageDatabase.UnitData>(unit.Item1, unitFightData));
                        else
                            fullUnitDatas[index].Item2.AddUnitData(unitFightData);
                    }
                    totalFightDurations += fight.GetFightDuration();
                }

                foreach (var dataPresentTypeInfo in m_DataPresentTypeInfoList)
                {
                    var sortedUnits = fullUnitDatas.OrderByDescending((_Unit) => { return dataPresentTypeInfo.m_GetValue(_Unit.Item2); });
                    if (sortedUnits.Count() > 0)
                    {
                        List<PageUtility.StatsBarData> statsBars = new List<PageUtility.StatsBarData>();
                        //string newBossSection = "";
                        double totalValue = 0;
                        double maxValue = 0;
                        VF_RaidDamageDatabase.UnitData.CalculateTotalAndMax(fullUnitDatas.AsReadOnly(), dataPresentTypeInfo.m_GetValue
                            , (_Value) => { return realmDB.RD_IsPlayer(_Value.Item1, attendingRaidPlayers) && dataPresentTypeInfo.m_ValidCheck(_Value.Item2); }
                            , out totalValue, out maxValue);
                        //newBossSection += "VF_CreateDmgBar(1.0, '#000000', '" + dataPresentTypeInfo.m_TypeName + "','#ffffff', '" + (totalValue / 1000).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "k total');";
                        int players = 0;
                        foreach (var unit in sortedUnits)
                        {
                            var playerData = realmDB.RD_FindPlayer(unit.Item1, attendingRaidPlayers);
                            double currValue = dataPresentTypeInfo.m_GetValue(unit.Item2);
                            if (playerData != null && currValue > 0 && dataPresentTypeInfo.m_ValidCheck(unit.Item2))
                            {
                                double percentage = (double)currValue / totalValue;

                                double displayPercentage = percentage / (maxValue / totalValue);
                                //percentage *= maxValue;
                                //if (players < dataPresentTypeInfo.m_Count)
                                {
                                    ++players;
                                    string classColor = "#CCCCCC";
                                    classColor = RealmPlayersServer.Code.Resources.VisualResources._ClassColors[playerData.Character.Class];
                                    //newBossSection += "VF_CreateDmgBar(" + displaypercentage.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", '" + classColor + "', '<player," + unit.Item1 + ">" + unit.Item1 + "(" + (currValue / totalFightDurations).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "/s)','#000000', '" + (int)currValue + "(" + string.Format("{0:0.0%}", percentage) + ")');";

                                    if (dataPresentTypeInfo.m_TypeName == "Deaths")
                                    {
                                        string rightSideText = "" + (int)currValue + " death" + (currValue > 1 ? "s" : "");
                                        statsBars.Add(new PageUtility.StatsBarData
                                        {
                                            m_BeforeBarText = "#" + players,// + " " + PageUtility.CreateLink("http://realmplayers.com/CharacterViewer.aspx?realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realmDB.Realm) + "&player=" + unit.Item1, unit.Item1),
                                            m_OnBarLeftText = PageUtility.CreateLink_RaidStats_Player(playerData),
                                            m_BarColor = classColor,
                                            m_PercentageWidth = displayPercentage,
                                            m_AfterBarText = percentage.ToStringDot("0.0%"),
                                            m_OnBarRightText = rightSideText,
                                            m_OnBarTextWidth = StaticValues.MeasureStringLength(playerData.Name + " " + rightSideText)
                                        });
                                    }
                                    else
                                    {
                                        string rightSideText = "" + (int)currValue + "(" + (currValue / totalFightDurations).ToStringDot("0") + "/s)";
                                        statsBars.Add(new PageUtility.StatsBarData
                                        {
                                            m_BeforeBarText = "#" + players,// + "(" + string.Format("{0:0.0%}", percentage) + ")",
                                            m_OnBarLeftText = PageUtility.CreateLink_RaidStats_Player(playerData),
                                            m_BarColor = classColor,
                                            m_PercentageWidth = displayPercentage,
                                            m_AfterBarText = percentage.ToStringDot("0.0%"),
                                            m_OnBarRightText = rightSideText,
                                            m_OnBarTextWidth = StaticValues.MeasureStringLength(playerData.Name + " " + rightSideText)
                                        });
                                    }
                                }
                            }
                            else if (unit.Item1.Contains("(Pet for"))
                            {
                                double percentage = (double)currValue / totalValue;

                                double displayPercentage = percentage / (maxValue / totalValue);
                                //percentage *= maxValue;
                                //if (players < dataPresentTypeInfo.m_Count)
                                {
                                    ++players;
                                    string classColor = "#00FF00";
                                    //newBossSection += "VF_CreateDmgBar(" + displaypercentage.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", '" + classColor + "', '<player," + unit.Item1 + ">" + unit.Item1 + "(" + (currValue / totalFightDurations).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "/s)','#000000', '" + (int)currValue + "(" + string.Format("{0:0.0%}", percentage) + ")');";

                                    if (dataPresentTypeInfo.m_TypeName == "Deaths")
                                    {
                                        string rightSideText = "" + (int)currValue + " death" + (currValue > 1 ? "s" : "");
                                        statsBars.Add(new PageUtility.StatsBarData
                                        {
                                            m_BeforeBarText = "#" + players,// + " " + PageUtility.CreateLink("http://realmplayers.com/CharacterViewer.aspx?realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realmDB.Realm) + "&player=" + unit.Item1, unit.Item1),
                                            m_OnBarLeftText = unit.Item1,
                                            m_BarColor = classColor,
                                            m_PercentageWidth = displayPercentage,
                                            m_AfterBarText = percentage.ToStringDot("0.0%"),
                                            m_OnBarRightText = rightSideText,
                                            m_OnBarTextWidth = StaticValues.MeasureStringLength(unit.Item1 + " " + rightSideText)
                                        });
                                    }
                                    else
                                    {
                                        string rightSideText = "" + (int)currValue + "(" + (currValue / totalFightDurations).ToStringDot("0") + "/s)";
                                        statsBars.Add(new PageUtility.StatsBarData
                                        {
                                            m_BeforeBarText = "#" + players,// + "(" + string.Format("{0:0.0%}", percentage) + ")",
                                            m_OnBarLeftText = unit.Item1,
                                            m_BarColor = classColor,
                                            m_PercentageWidth = displayPercentage,
                                            m_AfterBarText = percentage.ToStringDot("0.0%"),
                                            m_OnBarRightText = rightSideText,
                                            m_OnBarTextWidth = StaticValues.MeasureStringLength(unit.Item1 + " " + rightSideText)
                                        });
                                    }
                                }
                            }
                        }
                        //graphSection += newBossSection;
                        totalBarsCount += 1 + players;
                        statsBarStyle.m_TitleText = dataPresentTypeInfo.m_TypeName + "(" + (totalValue / 1000).ToStringDot("0.0") + "k)";
                        if (dataPresentTypeInfo.m_TypeName == "Deaths")
                        {
                            statsBarStyle.m_TitleText = dataPresentTypeInfo.m_TypeName + "(" + totalValue + " total)";
                        }

                        if (dataPresentTypeInfo.m_TypeName == "Damage" || dataPresentTypeInfo.m_TypeName == "Threat" || dataPresentTypeInfo.m_TypeName == "Damage Taken")
                            dmgThreatSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, statsBars, dataPresentTypeInfo.m_Count);
                        else
                            healSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, statsBars, dataPresentTypeInfo.m_Count);
                    }
                }
                dmgThreatSection += "</div>";
                healSection += "</div>";
                graphSection += "<div class='row'>" + dmgThreatSection + healSection + "</div>";
            }
            return GeneratePlayerDeaths(realmDB, fullUnitDatas) + "<br/><br/>" + graphSection;
        }

        private static string GeneratePlayerDeaths(VF_RaidDamageDatabase.RealmDB realmDB, List<Tuple<string, VF_RaidDamageDatabase.UnitData>> fullUnitDatas)
        {
            var playerDeaths = new List<Tuple<int, string>>();
            foreach (var data in fullUnitDatas)
            {
                if (data.Item2.I.Death > 0 || data.Item2.I.Dmg > 0 || data.Item2.I.RawHeal > 0)
                {
                    playerDeaths.Add(Tuple.Create(data.Item2.I.Death, data.Item1));
                }
            }
            if (playerDeaths.Count == 0)
                return "";
            int totalPlayerDeaths = 0;
            string playerDeathsStr = "";
            var orderedPlayerDeaths = playerDeaths.OrderByDescending((_Value) => _Value.Item1);
            int lastDeathCount = orderedPlayerDeaths.First().Item1;
            playerDeathsStr += lastDeathCount.ToString() + " Deaths: ";
            foreach (var playerDeath in orderedPlayerDeaths)
            {
                var playerData = realmDB.FindPlayer(playerDeath.Item2);
                if (playerData != null)
                {
                    if (lastDeathCount != playerDeath.Item1)
                    {
                        playerDeathsStr += "<br/>" + playerDeath.Item1.ToString() + " Deaths: ";
                        lastDeathCount = playerDeath.Item1;
                    }
                    playerDeathsStr += PageUtility.CreateColorCodedName(playerData) + ", ";
                    totalPlayerDeaths += playerDeath.Item1;
                }
            }
            return "<h3>Player deaths(total " + totalPlayerDeaths + ")</h3>" + playerDeathsStr;
        }
    }
}