using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using FightCacheData = VF_RaidDamageDatabase.FightDataCollection.FightCacheData;
using RaidCollection = VF_RaidDamageDatabase.RaidCollection;
using RaidBossFight = VF_RaidDamageDatabase.RaidBossFight;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;


namespace VF.RaidDamageWebsite
{
    public partial class FightOverallOverview : System.Web.UI.Page
    {

        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_InfoTextHTML = null;
        public MvcHtmlString m_GraphSection = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            string fightName = PageUtility.GetQueryString(Request, "FightName");
            List<PlayerClass> classLimits = ClassControl.GetClassLimits();//PageUtility.GetQueryString(Request, "ClassLimit", "WrIWaIWlIMaIPrIShIRoIPaIDrIHu"));
            List<PlayerFaction> factionLimits = ClassControl.GetFactionLimits();
            bool showMultipleEntries = PageUtility.GetQueryString(Request, "MultipleEntries", "false").ToLower() != "false";

            string guildLimit = PageUtility.GetQueryString(Request, "Guild", null);
            string playerLimit = PageUtility.GetQueryString(Request, "PlayerLimit", null);
            string andPlayer = PageUtility.GetQueryString(Request, "AndPlayer", null);
            int showEntriesCount = PageUtility.GetQueryInt(Request, "Count", 50);
            if (showEntriesCount > 100)
                showEntriesCount = 100;

            var realm = RealmControl.Realm;

            var realmDB = ApplicationInstance.Instance.GetRealmDB(VF_RealmPlayersDatabase.WowRealm.Emerald_Dream);
            //var raidCollection = ApplicationInstance.Instance.GetRaidCollection();
            
            var summaryDatabase = ApplicationInstance.Instance.GetSummaryDatabase();
            if (summaryDatabase == null)
                return;

            if (guildLimit != null)
            {
                this.Title = fightName + " Highscore for " + guildLimit + " | RaidStats";
            }
            else if (playerLimit != null)
            {
                this.Title = fightName + " Highscore for " + playerLimit + " | RaidStats";
            }
            else
            {
                this.Title = fightName + " Highscore | RaidStats";
            }
            IEnumerable<VF_RaidDamageDatabase.Models.PurgedPlayer> purgePlayers = ApplicationInstance.Instance.GetPurgedPlayers(realm);

            var fightInstances = summaryDatabase.GetHSElligibleBossFights(fightName, realm, guildLimit, null, purgePlayers);
            //Remove fights that have too low precision
            if (fightInstances.Count > 0)
            {
                string breadCrumbCommon = "";
                if (guildLimit != null)
                {
                    breadCrumbCommon = PageUtility.BreadCrumb_AddHome()
                        + PageUtility.BreadCrumb_AddRealm(realm)
                        + PageUtility.BreadCrumb_AddLink("RaidList.aspx?Guild=" + guildLimit, guildLimit) 
                        + PageUtility.BreadCrumb_AddLink("BossList.aspx?Guild=" + guildLimit, "Bosses");
                }
                else if (playerLimit != null)
                {
                    breadCrumbCommon = PageUtility.BreadCrumb_AddHome()
                        + PageUtility.BreadCrumb_AddRealm(realm)
                        + PageUtility.BreadCrumb_AddLink("PlayerOverview.aspx?realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realm) + "&player=" + playerLimit, playerLimit)
                        + PageUtility.BreadCrumb_AddLink("BossList.aspx?realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realm) + "&player=" + playerLimit, "Bosses");
                }
                else
                {
                    breadCrumbCommon = PageUtility.BreadCrumb_AddHome()
                        + PageUtility.BreadCrumb_AddRealm(realm) 
                        + PageUtility.BreadCrumb_AddLink("BossList.aspx", "Bosses");
                }

                if (ClassControl.HasClassLimits() == true)
                {
                    string colorClasses = ClassControl.GetColorClassesStr();
                    string breadCrumb = breadCrumbCommon + PageUtility.BreadCrumb_AddThisPageWithout(fightName, Request, "ClassLimit");

                    if (ClassControl.HasFactionLimits() == true)
                    {
                        breadCrumb += PageUtility.BreadCrumb_AddFinish("vs " + colorClasses);
                        breadCrumb += PageUtility.BreadCrumb_AddFinish("Only " + ClassControl.GetColorFactionStr());
                    }
                    else
                    {
                        breadCrumb += PageUtility.BreadCrumb_AddFinish("vs " + colorClasses);
                    }
                    m_BreadCrumbHTML = new MvcHtmlString(breadCrumb);
                }
                else
                {
                    if (ClassControl.HasFactionLimits() == true)
                    {
                        m_BreadCrumbHTML = new MvcHtmlString(breadCrumbCommon
                            + PageUtility.BreadCrumb_AddThisPageWithout(fightName, Request, "FactionLimit")
                            + PageUtility.BreadCrumb_AddFinish("Only " + ClassControl.GetColorFactionStr()));
                    }
                    else
                    {
                        m_BreadCrumbHTML = new MvcHtmlString(breadCrumbCommon
                            + PageUtility.BreadCrumb_AddFinish(fightName));
                    }
                }

                string graphSection = "<h1>Highscore for ";
                if(playerLimit != null)
                {
                    graphSection += playerLimit;
                }
                else
                {
                    graphSection += "players";
                }
                graphSection += " vs " + fightName + "</h1><p>Fights with unrealistic dmg spikes(SW_Stats reset bug) are disqualified from this list.</p>";
                //graphSection += "<p>View Highscore for class: ";
                //foreach (var classLimit in ClassLimitConverter)
                //{
                //    graphSection += PageUtility.CreateLink(PageUtility.CreateUrlWithNewQueryValue(Request, "ClassLimit", classLimit.Key), PageUtility.CreateColorCodedName(classLimit.Value.ToString(), classLimit.Value)) + ", ";
                //}
                if (playerLimit != null)
                {
                    showMultipleEntries = true;//Force it to true
                }
                else
                {
                    if (showMultipleEntries == false)
                    {
                        graphSection += "<p>Currently <u>not</u> showing multiple entries per player/guild. <a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "MultipleEntries", "true") + "'>Click here if you want to show multiple entries per entities</a></p>";
                    }
                    else
                    {
                        graphSection += "<p>Currently showing multiple entries per player/guild. <a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "MultipleEntries", "false") + "'>Click here if you do not want to show multiple entries per entities</a></p>";
                    }
                }
                //if (showEntriesCount < 50)
                //{
                //    graphSection += "<p>Currently showing " + showEntriesCount + " entries per data table. <a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Count", "50") + "'>Click here if you want to show up to 50</a></p>";
                //}
                //else if (showEntriesCount >= 50)
                //{
                //    graphSection += "<p>Currently showing " + showEntriesCount + " entries per data table. <a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Count", "25") + "'>Click here if you want to show only 25</a></p>";
                //}

                if (guildLimit != null)
                {
                    graphSection += "<p>Note that this does only show data from the guild " + guildLimit + ".<br />If you want to see for all guilds click " + PageUtility.CreateLink(PageUtility.CreateUrlWithNewQueryValue(Request, "Guild", "null"), "here") + "</p>";
                }
                //graphSection += "</p>";
                m_InfoTextHTML = new MvcHtmlString(graphSection);

                PlayerFaction factionFilter = PlayerFaction.Unknown;
                if (factionLimits != null && factionLimits.Count == 1)
                    factionFilter = factionLimits[0];

                List<string> includePlayers = null;
                if(andPlayer != null)
                {
                    includePlayers = new List<string> { andPlayer };
                }
                m_GraphSection = new MvcHtmlString(VF.FightOverallOverviewGenerator.Generate(fightInstances, ApplicationInstance.Instance.GetRPPDatabase(), new VF.FightOverallOverviewGenerator.GenerateDetails { ClassFilter = classLimits, EntriesCount = showEntriesCount, ShowMultipleEntries = showMultipleEntries, RealmFilter = realm, GuildFilter = guildLimit, PlayerFilter = playerLimit, FactionFilter = factionFilter, IncludePlayers = includePlayers }));
            }
        }
    }
}