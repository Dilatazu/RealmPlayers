using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using RaidBossFight = VF_RaidDamageDatabase.RaidBossFight;
using AttemptType = VF_RaidDamageDatabase.FightData.AttemptType;

namespace VF.RaidDamageWebsite
{
    public partial class FightOverview : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_FightOverviewInfoHTML = null;

        public MvcHtmlString m_GraphSection = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            bool DEBUG_Website = PageUtility.GetQueryString(Request, "Debug", "null") != "null";
            bool filteredData = PageUtility.GetQueryString(Request, "Filtered", "true") == "true";
            if (/*Request.UserHostAddress == "85.24.168.194" || */Request.UserHostAddress == "::1")
            {
                DEBUG_Website = PageUtility.GetQueryString(Request, "Debug", "null") != "false";
            }
            int uniqueRaidID = PageUtility.GetQueryInt(Request, "Raid", -1);
            if (uniqueRaidID == -1)
                Response.Redirect("RaidList.aspx");

            string bossNumberStr = PageUtility.GetQueryString(Request, "Fight", "-1");
            string trashNumberStr = PageUtility.GetQueryString(Request, "Trash", "-1");
            int fightVersion = PageUtility.GetQueryInt(Request, "Version", -1);

            if (bossNumberStr == "-1" && trashNumberStr == "-1")
                Response.Redirect("RaidOverview.aspx?Raid=" + uniqueRaidID);

            if (bossNumberStr != "-1")
            {
                this.Title = "Fight " + bossNumberStr + " in raid " + uniqueRaidID + " | RaidStats";

                RaidBossFight interestingFight = null;
                if (bossNumberStr.Length == 8)
                {
                    //DateID
                    var bossFights = ApplicationInstance.Instance.GetRaidBossFights(uniqueRaidID);
                    foreach (var bossFight in bossFights)
                    {
                        if (bossFight.GetStartDateTime().ToString("ddHHmmss") == bossNumberStr)
                        {
                            interestingFight = bossFight;
                            break;
                        }
                    }
                }
                else
                {
                    interestingFight = ApplicationInstance.Instance.GetRaidBossFight(uniqueRaidID, int.Parse(bossNumberStr));
                }
                if (fightVersion >= 1)// && fightVersion > interestingFight.GetExtraFightVersionCount())
                {
                    interestingFight = interestingFight.GetExtraFightVersion(fightVersion - 1);
                    if (interestingFight == null)
                        Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "Version", "0"));
                }
                GenerateBossFightPage(DEBUG_Website, filteredData, uniqueRaidID, interestingFight);
            }
            else if (trashNumberStr != "-1")
            {
                this.Title = "Trash " + trashNumberStr + " in raid " + uniqueRaidID + " | RaidStats";

                RaidBossFight interestingFight = null;
                interestingFight = ApplicationInstance.Instance.GetRaidTrashFight(uniqueRaidID, int.Parse(trashNumberStr));
                GenerateBossFightPage(DEBUG_Website, filteredData, uniqueRaidID, interestingFight);
            }
        }

        private void GenerateBossFightPage(bool DEBUG_Website, bool filteredData, int uniqueRaidID, RaidBossFight interestingFight)
        {
            if (interestingFight == null)
                Response.Redirect("RaidList.aspx");

            var realmDB = ApplicationInstance.Instance.GetRealmDB(interestingFight.GetRaid().Realm);

            if (interestingFight != null)
            {
                //int fightDuration = timeEnd - timeStart;
                //if (fightDuration <= 0)
                //{
                //    if (timeEnd < timeStart)
                //        fightDuration = interestingFight.GetFightData().GetFightRecordDuration() - timeStart;
                //    else
                //        fightDuration = interestingFight.GetFightData().GetFightRecordDuration();
                //}
                m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRaidList()
                + PageUtility.BreadCrumb_AddLink("RaidList.aspx?Guild=" + interestingFight.GetRaid().RaidOwnerName + "&realm=" + StaticValues.ConvertRealmParam(realmDB.Realm), interestingFight.GetRaid().RaidOwnerName)
                    + PageUtility.BreadCrumb_AddRaidOverview(interestingFight.GetRaid())
                    + PageUtility.BreadCrumb_AddFinish(interestingFight.GetBossName()));

                var raidSummary = ApplicationInstance.Instance.GetSummaryDatabase().GetRaid(uniqueRaidID);
                m_FightOverviewInfoHTML = new MvcHtmlString(FightOverviewGenerator.Generate(realmDB, raidSummary, interestingFight
                    , new FightOverviewGenerator.GenerateDetails 
                        { FilterSpikes = filteredData
                        , DebugInfo = DEBUG_Website
                        , DebugBuff = PageUtility.GetQueryString(Request, "DebugBuff", null)
                        , FilterSpikesURL = PageUtility.CreateUrlWithNewQueryValue(Request, "Filtered", "true")
                        , NoFilterSpikesURL = PageUtility.CreateUrlWithNewQueryValue(Request, "Filtered", "false")
                        , VersionChangeURL = PageUtility.CreateUrlWithNewQueryValue(Request, "Version", "versionchangeid")
                        , ItemSummaryDatabase = ApplicationInstance.Instance.GetItemSummaryDatabase()
                        , GetItemInfoFunc = ApplicationInstance.Instance.GetItemInfo
                    }));

            }
            else
            {
                Response.Redirect("RaidOverview.aspx?Raid=" + uniqueRaidID);
            }
        }
    }
}