using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace VF.RaidDamageWebsite
{
    public partial class DungeonList : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_DungeonListInfoHTML = null;

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

            this.Title = "Raids | RaidStats";

            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                + PageUtility.BreadCrumb_AddRealm(realm) + PageUtility.BreadCrumb_AddFinish("Dungeons"));
            m_DungeonListInfoHTML = new MvcHtmlString("<h1>All recorded dungeons</h1><p>Sorted by the time the dungeon ended</p>");

            var raidCollection = ApplicationInstance.Instance.GetRaidCollection();
            var orderedDungeons = raidCollection.m_Dungeons.OrderByDescending((_Value) => { return _Value.Value.m_DungeonEndDate; });
            int nr = 0;

            int i = orderedDungeons.Count();
            string tableBody = "";
            foreach (var dungeon in orderedDungeons)
            {
                if (dungeon.Value.Realm == VF_RealmPlayersDatabase.WowRealm.Test_Server)
                    continue;//Skip Test_Server
                if ((realm == VF_RealmPlayersDatabase.WowRealm.All || realm == dungeon.Value.Realm))
                {
                    nr++;
                    if (nr > pageIndex * count && nr <= (pageIndex + 1) * count)
                    {
                        string dungeonOwnerVisual = "\"" + dungeon.Value.m_GroupMembers.MergeToStringVF("\", \"") + "\"";

                        tableBody += PageUtility.CreateTableRow("", PageUtility.CreateTableColumn("#" + i) +
                            PageUtility.CreateTableColumn(dungeonOwnerVisual) +
                            PageUtility.CreateTableColumn(PageUtility.CreateLink("DungeonOverview.aspx?Dungeon=" + dungeon.Value.m_UniqueDungeonID, dungeon.Value.m_Dungeon)) +
                            PageUtility.CreateTableColumn(dungeon.Value.m_DungeonStartDate.ToLocalTime().ToString("yyy-MM-dd HH:mm:ss")) +
                            PageUtility.CreateTableColumn(dungeon.Value.m_DungeonEndDate.ToLocalTime().ToString("yyy-MM-dd HH:mm:ss")) +
                            PageUtility.CreateTableColumn(RealmPlayersServer.StaticValues.ConvertRealmViewing(dungeon.Value.Realm)));
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

            m_PaginationHTML = new MvcHtmlString(PageUtility.CreatePagination(Request, pageNr, ((orderedDungeons.Count() - 1) / count) + 1));
        }
    }
}