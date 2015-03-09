using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using ContributorDB = VF_RealmPlayersDatabase.ContributorDB;
using UploadID = VF_RealmPlayersDatabase.UploadID;
namespace RealmPlayersServer
{
    public partial class Contributors : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_ContributorsInfoHTML = null;
        public MvcHtmlString m_TableHeadHTML = null;
        public MvcHtmlString m_TableBodyHTML = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Contributors | RealmPlayers";
            var statisticsData = DatabaseAccess.GetContributorStatistics();
            if (statisticsData == null)
            {
                PageUtility.RedirectErrorLoading(this, "contributors");
                return;
            }
            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome() + PageUtility.BreadCrumb_AddFinish("Contributors"));

            var dataED = statisticsData[VF_RealmPlayersDatabase.WowRealm.Emerald_Dream];
            var dataWSG = statisticsData[VF_RealmPlayersDatabase.WowRealm.Warsong];
            var dataAlA = statisticsData[VF_RealmPlayersDatabase.WowRealm.Al_Akir];
            var dataREB = statisticsData[VF_RealmPlayersDatabase.WowRealm.Rebirth];
            var dataNRB = statisticsData[VF_RealmPlayersDatabase.WowRealm.Nostalrius];
            var dataArA = statisticsData[VF_RealmPlayersDatabase.WowRealm.Archangel];

            m_TableHeadHTML = new MvcHtmlString(PageUtility.CreateTableRow("", PageUtility.CreateTableColumnHead("#Nr") 
                + PageUtility.CreateTableColumnHead("Name") 
                + PageUtility.CreateTableColumnHead("Total inspects") 
                + PageUtility.CreateTableColumnHead("Emerald Dream")
                + PageUtility.CreateTableColumnHead("Warsong")
                + PageUtility.CreateTableColumnHead("Al'Akir")
                + PageUtility.CreateTableColumnHead("Rebirth")
                + PageUtility.CreateTableColumnHead("Nostalrius") 
                + PageUtility.CreateTableColumnHead("Archangel(TBC)")
                + PageUtility.CreateTableColumnHead("Active since") 
                + PageUtility.CreateTableColumnHead("Last active")));

            SortedList<int, string> tableRows = new SortedList<int, string>();
            string tableBody = "";

            var contributors = ContributorDB.GetAllTrustWorthyContributors();
            foreach (var data in contributors)
            {
                Code.ContributorStatisticItem statsED = null;
                Code.ContributorStatisticItem statsWSG = null;
                Code.ContributorStatisticItem statsAlA = null;
                Code.ContributorStatisticItem statsREB = null;
                Code.ContributorStatisticItem statsNRB = null;
                Code.ContributorStatisticItem statsArA = null;
                if (dataED.TryGetValue(data.ContributorID, out statsED) == false) statsED = new Code.ContributorStatisticItem(-1);
                if (dataWSG.TryGetValue(data.ContributorID, out statsWSG) == false) statsWSG = new Code.ContributorStatisticItem(-1);
                if (dataAlA.TryGetValue(data.ContributorID, out statsAlA) == false) statsAlA = new Code.ContributorStatisticItem(-1);
                if (dataREB.TryGetValue(data.ContributorID, out statsREB) == false) statsREB = new Code.ContributorStatisticItem(-1);
                if (dataNRB.TryGetValue(data.ContributorID, out statsNRB) == false) statsNRB = new Code.ContributorStatisticItem(-1);
                if (dataArA.TryGetValue(data.ContributorID, out statsArA) == false) statsArA = new Code.ContributorStatisticItem(-1);
                int inspectsED = 0;
                int inspectsWSG = 0;
                int inspectsAlA = 0;
                int inspectsREB = 0;
                int inspectsNRB = 0;
                int inspectsArA = 0;
                foreach (var inspection in statsED.m_PlayerInspects) inspectsED += inspection.Value;
                foreach (var inspection in statsWSG.m_PlayerInspects) inspectsWSG += inspection.Value;
                foreach (var inspection in statsAlA.m_PlayerInspects) inspectsAlA += inspection.Value;
                foreach (var inspection in statsREB.m_PlayerInspects) inspectsREB += inspection.Value;
                foreach (var inspection in statsNRB.m_PlayerInspects) inspectsNRB += inspection.Value;
                foreach (var inspection in statsArA.m_PlayerInspects) inspectsArA += inspection.Value;

                int totalInspects = inspectsED + inspectsWSG + inspectsAlA + inspectsREB + inspectsNRB + inspectsArA;
                DateTime earliestActive = statsED.m_EarliestActiveUTC;
                earliestActive = (statsWSG.m_EarliestActiveUTC < earliestActive ? statsWSG.m_EarliestActiveUTC : earliestActive);
                earliestActive = (statsAlA.m_EarliestActiveUTC < earliestActive ? statsAlA.m_EarliestActiveUTC : earliestActive);
                earliestActive = (statsREB.m_EarliestActiveUTC < earliestActive ? statsREB.m_EarliestActiveUTC : earliestActive);
                earliestActive = (statsNRB.m_EarliestActiveUTC < earliestActive ? statsNRB.m_EarliestActiveUTC : earliestActive);
                earliestActive = (statsArA.m_EarliestActiveUTC < earliestActive ? statsArA.m_EarliestActiveUTC : earliestActive);
                DateTime latestActive = statsED.m_LatestActiveUTC;
                latestActive = (statsWSG.m_LatestActiveUTC > latestActive ? statsWSG.m_LatestActiveUTC : latestActive);
                latestActive = (statsAlA.m_LatestActiveUTC > latestActive ? statsAlA.m_LatestActiveUTC : latestActive);
                latestActive = (statsREB.m_LatestActiveUTC > latestActive ? statsREB.m_LatestActiveUTC : latestActive);
                latestActive = (statsNRB.m_LatestActiveUTC > latestActive ? statsNRB.m_LatestActiveUTC : latestActive);
                latestActive = (statsArA.m_LatestActiveUTC > latestActive ? statsArA.m_LatestActiveUTC : latestActive);

                if (totalInspects > 0 && data.Name != "Unknown" 
                    && ((DateTime.UtcNow - latestActive).TotalDays < 15 
                    || (totalInspects > 5000 && (DateTime.UtcNow - latestActive).TotalDays < 60)))
                {
                    int keyToUse = int.MaxValue - totalInspects * 100;
                    while (tableRows.ContainsKey(keyToUse) == true)
                        keyToUse += 1;
                    tableRows.Add(keyToUse, PageUtility.CreateTableColumn(data.Name)
                        + PageUtility.CreateTableColumn(totalInspects.ToString())
                        + PageUtility.CreateTableColumn(inspectsED.ToString())
                        + PageUtility.CreateTableColumn(inspectsWSG.ToString())
                        + PageUtility.CreateTableColumn(inspectsAlA.ToString())
                        + PageUtility.CreateTableColumn(inspectsREB.ToString())
                        + PageUtility.CreateTableColumn(inspectsNRB.ToString())
                        + PageUtility.CreateTableColumn(inspectsArA.ToString())
                        + PageUtility.CreateTableColumn(earliestActive.ToString("yyy-MM-dd"))
                        + PageUtility.CreateTableColumn(StaticValues.GetTimeSinceLastSeenUTC(latestActive)));
                }
            }
            int i = 1;
            foreach (var tableRow in tableRows)
            {
                tableBody += PageUtility.CreateTableRow("", PageUtility.CreateTableColumn("#" + i++) + tableRow.Value);
            }
            m_TableBodyHTML = new MvcHtmlString(tableBody);

            m_ContributorsInfoHTML = new MvcHtmlString(
                "<h1>Contributors<span class='badge badge-inverse'>" + tableRows.Count() + " Players</span></h1>"
                + "<p>List displays the persons that contribute data to this project. Sorted by the amount of inspects they have contributed with.</p>"
                + "<br/><p>The more contributors the better! If you are interested in being a contributor to help out the project please do not hesitate contacting <a href='http://forum.realmplayers.com/memberlist.php?mode=viewprofile&u=51'>Sethzer</a> on RealmPlayers forums. Make sure you also read the thread: <a href='http://realmplayers.com:5555/viewtopic.php?f=14&t=15'>About Data Contribution</a>.</p><br/>");

        }
    }
}