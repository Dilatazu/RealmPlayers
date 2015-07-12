using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace VF.RaidDamageWebsite
{
    public partial class RaidList : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_RaidListInfoHTML = null;

        public MvcHtmlString m_TableHeadHTML = null;
        public MvcHtmlString m_TableBodyHTML = null;

        public MvcHtmlString m_PaginationHTML = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            int pageNr = PageUtility.GetQueryInt(Request, "page", 1);
            int pageIndex = pageNr - 1;//Change range from 0 to * instead of 1 to *
            int count = PageUtility.GetQueryInt(Request, "count", 50);
            if (count > 500) count = 500;

            var realm = RealmControl.Realm;

            string guildStr = PageUtility.GetQueryString(Request, "Guild", null);
            if (guildStr != null)
            {
                this.Title = "Raids for " + guildStr + " | RaidStats";

                m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm)
                    + PageUtility.BreadCrumb_AddFinish(guildStr)
                    + PageUtility.BreadCrumb_AddFinish("Raids"));
                m_RaidListInfoHTML = new MvcHtmlString("<h1>Recorded raids for " + guildStr + "</h1><p>Sorted by the time the raid ended</p><p>"
                    + PageUtility.CreateLink("InstanceList.aspx?realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realm) + "&Guild=" + guildStr, "Instances statistics by guild") + "</p><p>"
                    + PageUtility.CreateLink("BossList.aspx?realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realm) + "&Guild=" + guildStr, "Bosses statistics by guild") + "</p><p>"
                    + (guildStr != "PUG" ? PageUtility.CreateLink(PageUtility.HOSTURL_Armory + "GuildViewer.aspx?realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realm) + "&Guild=" + guildStr, "View guild on RealmPlayers(Armory)") : "") + "</p>");
            }
            else
            {
                this.Title = "Raids | RaidStats";

                m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm) + PageUtility.BreadCrumb_AddFinish("Raids"));
                m_RaidListInfoHTML = new MvcHtmlString("<h1>All recorded raids</h1><p>Sorted by the time the raid ended</p>");
            }
            var raidCollection = ApplicationInstance.Instance.GetRaidCollection();
            var orderedRaids = raidCollection.m_Raids.OrderByDescending((_Value) => { return _Value.Value.RaidEndDate; });

            m_TableHeadHTML = new MvcHtmlString(
                PageUtility.CreateTableRow("",
                PageUtility.CreateTableColumnHead("#Nr") +
                PageUtility.CreateTableColumnHead("Guild") +
                PageUtility.CreateTableColumnHead("Raid Instance(ID)") +
                PageUtility.CreateTableColumnHead("Start Date") +
                PageUtility.CreateTableColumnHead("End Date") +
                PageUtility.CreateTableColumnHead("Realm")));

            VF_RaidDamageDatabase.RealmDB realmDB = null;
            var guildSummaryDB = ApplicationInstance.Instance.GetGuildSummaryDatabase();
            int nr = 0;

            int i = orderedRaids.Count();
            string tableBody = "";
            foreach (var raid in orderedRaids)
            {
                if (raid.Value.Realm == VF_RealmPlayersDatabase.WowRealm.Test_Server)
                    continue;//Skip Test_Server
                if (raid.Value.RaidOwnerName == "")
                    continue;//Skip RaidOwnerName == "" because something is obviously wrong!
                if ((realm == VF_RealmPlayersDatabase.WowRealm.All || realm == raid.Value.Realm) 
                    && (guildStr == null || guildStr == raid.Value.RaidOwnerName) 
                    && InstanceControl.IsFiltered(raid.Value.RaidInstance) == true)
                {
                    nr++;
                    if (nr > pageIndex * count && nr <= (pageIndex + 1) * count)
                    {
                        string raidOwnerVisual = raid.Value.RaidOwnerName;
                        var recordedBy = raid.Value.GetRecordedByPlayers();
                        if (recordedBy.Count > 0)
                        {
                            VF_RealmPlayersDatabase.PlayerFaction faction = VF_RealmPlayersDatabase.PlayerFaction.Unknown;
                            if(raid.Value.RaidOwnerName != "PUG")
                            {
                                try
                                {
                                    faction = guildSummaryDB.GetGuildSummary(raid.Value.Realm, raid.Value.RaidOwnerName).Faction;
                                }
                                catch (Exception)
                                {}
                            }
                            else
                            {
                                if (realmDB == null || realmDB.Realm != raid.Value.Realm)
                                    realmDB = ApplicationInstance.Instance.GetRealmDB(raid.Value.Realm);
                                faction = RealmPlayersServer.StaticValues.GetFaction(realmDB.GetPlayer(recordedBy.First()).Character.Race);
                            }
                            if (faction == VF_RealmPlayersDatabase.PlayerFaction.Horde)
                                raidOwnerVisual = PageUtility.CreateImage("assets/img/Horde_32.png") + raidOwnerVisual;
                            else if (faction == VF_RealmPlayersDatabase.PlayerFaction.Alliance)
                                raidOwnerVisual = PageUtility.CreateImage("assets/img/Alliance_32.png") + raidOwnerVisual;
                        }
                        tableBody += PageUtility.CreateTableRow("", PageUtility.CreateTableColumn("#" + i) +
                            PageUtility.CreateTableColumn(PageUtility.CreateLink("RaidList.aspx?realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(raid.Value.Realm) + "&Guild=" + raid.Value.RaidOwnerName, raidOwnerVisual)) +
                            PageUtility.CreateTableColumn(PageUtility.CreateLink("RaidOverview.aspx?Raid=" + raid.Value.UniqueRaidID, PageUtility.CreateImage(StaticValues._RaidInstanceImages[raid.Value.RaidInstance]) + " " + raid.Value.RaidInstance + "(" + raid.Value.RaidID + ")")) +
                            PageUtility.CreateTableColumn(raid.Value.RaidStartDate.ToLocalTime().ToString("yyy-MM-dd HH:mm:ss")) +
                            PageUtility.CreateTableColumn(raid.Value.RaidEndDate.ToLocalTime().ToString("yyy-MM-dd HH:mm:ss")) +
                            PageUtility.CreateTableColumn(RealmPlayersServer.StaticValues.ConvertRealmViewing(raid.Value.Realm)));
                    }
                    if (nr >= (pageIndex + 1) * count)
                        break;
                }
                --i;
            }
            if (nr != 0 && nr <= pageIndex * count)
            {
                pageIndex = (nr - 1) / count;
                Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "page", (pageIndex + 1).ToString()));
            }
            m_TableBodyHTML = new MvcHtmlString(tableBody);

            m_PaginationHTML = new MvcHtmlString(PageUtility.CreatePagination(Request, pageNr, ((orderedRaids.Count() - 1) / count) + 1));
        }
    }
}