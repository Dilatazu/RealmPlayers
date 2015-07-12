using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace VF.RaidDamageWebsite
{
    public partial class DungeonOverview : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public string m_RaidOverviewInfoHTML = "";
        public MvcHtmlString m_TableHeadHTML = null;
        public MvcHtmlString m_TableBodyHTML = null;
        public MvcHtmlString m_TrashHTML = null;

        public MvcHtmlString m_GraphSection = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {

            bool filteredData = PageUtility.GetQueryString(Request, "Filtered", "true") == "true";
            int uniqueDungeonID = PageUtility.GetQueryInt(Request, "Dungeon", -1);
            if (uniqueDungeonID == -1)
                Response.Redirect("DungeonList.aspx");

            this.Title = "Dungeon " + uniqueDungeonID + " | RaidStats";


            var orderedFights = ApplicationInstance.Instance.GetDungeonBossFights(uniqueDungeonID);
            if (orderedFights == null || orderedFights.Count() == 0)
                Response.Redirect("DungeonList.aspx");

            var realmDB = ApplicationInstance.Instance.GetRealmDB(orderedFights.First().GetDungeon().Realm);

            var currDungeon = orderedFights.First().GetDungeon();
            if (currDungeon.Realm == VF_RealmPlayersDatabase.WowRealm.Test_Server && PageUtility.GetQueryString(Request, "Debug") == "null")
                Response.Redirect("DungeonList.aspx");
            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                + PageUtility.BreadCrumb_AddDungeonList()
                    + PageUtility.BreadCrumb_AddFinish(currDungeon.m_Dungeon + "(" + currDungeon.m_UniqueDungeonID.ToString() + ")"));

            bool displayLoot = false;
            if (orderedFights.FindIndex((_Value) => _Value.GetItemDrops().Count > 0) != -1)
                displayLoot = true;

            m_TableHeadHTML = new MvcHtmlString(
                PageUtility.CreateTableRow("",
                PageUtility.CreateTableColumnHead("#Nr") +
                PageUtility.CreateTableColumnHead("Boss") +
                PageUtility.CreateTableColumnHead("Players") +
                (displayLoot == true ? PageUtility.CreateTableColumnHead("Items Dropped") : "") +
                PageUtility.CreateTableColumnHead("Player Deaths") +
                PageUtility.CreateTableColumnHead("Fight Duration") +
                PageUtility.CreateTableColumnHead("Kill Time") +
                PageUtility.CreateTableColumnHead(PageUtility.CreateTooltipText("Precision", "How much percentage of the recorded fight is vs the boss intended. Calculated using the formula: Boss+Adds DmgTaken/Recorded Damage"))));

            List<string> attendingDungeonPlayers = new List<string>();
            string tableBody = "";
            foreach (var fight in orderedFights)
            {
                var attendingFightPlayers = fight.GetAttendingUnits((_Name) => { return realmDB.RD_IsPlayer(_Name, fight); });
                attendingDungeonPlayers.AddRange(attendingFightPlayers);

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
                var realm = fight.GetDungeon().Realm;
                ///////////////////////
                string lootDropped = "";
                if (attemptType == VF_RaidDamageDatabase.FightData.AttemptType.KillAttempt)
                {
                    lootDropped = LootGenerator.CreateLootDroppedData(fight.GetItemDrops(), realmDB, ApplicationInstance.Instance.GetItemSummaryDatabase(), ApplicationInstance.Instance.GetItemInfo);
                }
                ///////////////////////

                tableBody += PageUtility.CreateTableRow("", PageUtility.CreateTableColumn("#" + (fight.GetRaidBossFightIndex() + 1)) +
                    PageUtility.CreateTableColumn(/*PageUtility.CreateLink("FightOverview.aspx?Dungeon=" + uniqueDungeonID + "&Fight=" + fight.GetRaidBossFightIndex(), */"<font color='#f70002'>" + fight.GetFightData().FightName + "</font>"/*)*/ + attemptStr) +
                    PageUtility.CreateTableColumn(attendingFightPlayers.Count.ToString()) +
                    (displayLoot == true ? PageUtility.CreateTableColumn(lootDropped) : "") +
                    PageUtility.CreateTableColumn(((int)fight.GetTotal((_Value) => { return _Value.I.Death; }
                            , (_Value) => { return realmDB.RD_IsPlayer(_Value.Item1, fight) && _Value.Item2.I.Death > 0; })).ToString()) +
                    PageUtility.CreateTableColumn(fight.GetFightDuration().ToString() + " sec") +
                    PageUtility.CreateTableColumn(fight.GetFightData().StartDateTime.AddSeconds(fight.GetFightData().GetFightRecordDuration()).ToLocalTime().ToString("yyy-MM-dd HH:mm:ss")) +
                    PageUtility.CreateTableColumn(trashPercentageStr));
            }
            m_TableBodyHTML = new MvcHtmlString(tableBody);

            /////////////////////
            List<Tuple<string, VF_RealmPlayersDatabase.PlayerClass>> playersAttending = new List<Tuple<string, VF_RealmPlayersDatabase.PlayerClass>>();

            foreach (var playerName in attendingDungeonPlayers.Distinct())
            {
                var playerData = realmDB.RD_FindPlayer(playerName, attendingDungeonPlayers);
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
            catch (Exception ex)
            {

            }
            /////////////////////

            var recordedBy = currDungeon.GetRecordedByPlayers();
            string recordedByString = "";
            foreach (var playerName in recordedBy)
            {
                var playerData = realmDB.RD_FindPlayer(playerName, attendingDungeonPlayers);
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

            m_RaidOverviewInfoHTML = "<h1>\"" + currDungeon.m_GroupMembers.MergeToStringVF("\", \"") + "\" vs "
                + currDungeon.m_Dungeon + "(" + currDungeon.m_UniqueDungeonID.ToString() + ")<span class='badge badge-inverse'>" + orderedFights.Count() + " fights</span></h1>"
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

            totalBossMeters += RaidOverview.GenerateTotalMeters(filteredData, orderedFights, realmDB, attendingDungeonPlayers);
            
            m_GraphSection = new MvcHtmlString(graphStyle + "<div class='blackframe'>" + totalBossMeters + "</div><br/><div class='blackframe'>" + totalTrashMeters + "</div>");

        }
    }
}