using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using VF_RaidDamageDatabase;

namespace VF.RaidDamageWebsite
{
    public partial class InstanceList : System.Web.UI.Page
    {
        public MvcHtmlString m_PageInfoHTML = null;
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_GraphsHTML = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            bool showMultipleEntries = PageUtility.GetQueryString(Request, "MultipleEntries", "false").ToLower() != "false";

            int maxCount = PageUtility.GetQueryInt(Request, "Count", 50);
            if (maxCount > 100)
                maxCount = 100;

            string guildLimit = PageUtility.GetQueryString(Request, "Guild", null);

            var realm = RealmControl.Realm;

            string graphSection = "";

            string breadCrumbCommon = "";
            if (guildLimit != null)
            {
                showMultipleEntries = true;
                this.Title = "Instance records for " + guildLimit + " | RaidStats";

                breadCrumbCommon = PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm)
                    + PageUtility.BreadCrumb_AddLink("RaidList.aspx?Guild=" + guildLimit, guildLimit);
                  //  + PageUtility.BreadCrumb_AddFinish("Instances"));

                m_PageInfoHTML = new MvcHtmlString("<h1>Raid Instances for " + guildLimit + "</h1>"
                    + "<p>Fastest instance clears by " + guildLimit + ". Sorted by difficulty.</p>"
                    + "<p>Note that this does only show data from the guild " + guildLimit + ".<br />If you want to see for all guilds click " + PageUtility.CreateLink("InstanceList.aspx?Realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realm), "here") + "</p>");
            }
            else
            {
                if (showMultipleEntries == false)
                {
                    graphSection += "<p>Currently <u>not</u> showing multiple entries per guild. <a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "MultipleEntries", "true") + "'>Click here if you want to show multiple entries per guild</a></p>";
                }
                else
                {
                    graphSection += "<p>Currently showing multiple entries per guild. <a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "MultipleEntries", "false") + "'>Click here if you do not want to show multiple entries per guild</a></p>";
                }

                this.Title = "Instance records | RaidStats";

                breadCrumbCommon = PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm);
                //    + PageUtility.BreadCrumb_AddFinish("Instances"));

                m_PageInfoHTML = new MvcHtmlString("<h1>Raid Instances</h1>"
                    + "<p>Fastest instance clears by guilds. Sorted by difficulty.</p>");
            }

            m_BreadCrumbHTML = new MvcHtmlString(breadCrumbCommon
                    + PageUtility.BreadCrumb_AddFinish("Instances"));

            graphSection += "<style>" + PageUtility.CreateStatsBars_HTML_CSSCode() + "</style>";

            PageUtility.StatsBarStyle statsBarStyle = new PageUtility.StatsBarStyle
            {
                m_TitleText = "",
                m_BarTextColor = "#000",
                m_LeftSideTitleText = "#",
                m_RightSideTitleText = "",
                m_BeforeBarWidth = 100,
                m_MaxWidth = 700,
                m_AfterBarWidth = 30
            };

            //var raidCollection = ApplicationInstance.Instance.GetRaidCollection();
            var summaryDatabase = ApplicationInstance.Instance.GetSummaryDatabase();
            if (summaryDatabase == null)
                return;
            
            Dictionary<string, List<RaidInstanceClearData>> raidInstanceClears = new Dictionary<string,List<RaidInstanceClearData>>();
            foreach (var groupRC in summaryDatabase.GroupRCs)
            {
                if (realm != VF_RealmPlayersDatabase.WowRealm.All && realm != groupRC.Value.Realm)
                    continue;

                if (guildLimit != null && guildLimit != groupRC.Value.GroupName)
                    continue;

                foreach (var raid in groupRC.Value.Raids)
                {
                    Dictionary<string, string[]> instanceRuns;
                    if (BossInformation.InstanceRuns.TryGetValue(raid.Value.RaidInstance, out instanceRuns) == false)
                        continue;
                    foreach (var instanceRun in instanceRuns)
                    {
                        var instanceClearData = RaidInstanceClearData.Generate(raid, instanceRun.Value);
                        if(instanceClearData != null)
                        {
                            raidInstanceClears.AddToList(instanceRun.Key, instanceClearData);
                        }
                    }
                }
            }
            var orderedInstanceClears = raidInstanceClears.OrderBy((_Value) =>
            {
                if (_Value.Key == "Zul'Gurub") return 0;
                else if (_Value.Key == "Ruins of Ahn'Qiraj") return 10;
                else if (_Value.Key == "Molten Core") return 20;
                else if (_Value.Key == "Blackwing Lair") return 30;
                else if (_Value.Key == "Temple of Ahn'Qiraj") return 40;
                else if (_Value.Key == "Naxxramas - Arachnid Quarter") return 50;
                else if (_Value.Key == "Naxxramas - Construct Quarter") return 60;
                else if (_Value.Key == "Naxxramas - Plague Quarter") return 70;
                else if (_Value.Key == "Naxxramas - Military Quarter") return 80;
                else if (_Value.Key == "Naxxramas - All Quarters") return 85;
                else if (_Value.Key == "Naxxramas") return 90;
                else return 100;
            });
            foreach (var riclears in orderedInstanceClears)
            {
                List<PageUtility.StatsBarData> statsBars = new List<PageUtility.StatsBarData>();

                var orderedClears = riclears.Value.OrderBy((_Value) => {return _Value.GetTimeSpan(); });

                int clearNr = 0;
                double firstClearCompareValue = 1 / orderedClears.First().GetTimeSpan().TotalSeconds;

                List<string> ignoreGuilds = new List<string>();
                foreach (var riclear in orderedClears)
                {
                    if (showMultipleEntries == false)
                    {
                        if (ignoreGuilds.Contains(riclear.m_Raid.CacheGroup.GroupName) == true)
                        {
                            continue;
                        }
                        else
                        {
                            ignoreGuilds.Add(riclear.m_Raid.CacheGroup.GroupName);
                        }
                    }

                    if (++clearNr > maxCount)
                        break;

                    double compareValue = 1 / riclear.GetTimeSpan().TotalSeconds;
                    string totalTimeStr = "" + (int)riclear.GetTimeSpan().TotalMinutes + " mins";//ser bäst ut
                    /*if (riclear.GetTimeSpan().Days > 0)
                        totalTimeStr += riclear.GetTimeSpan().Days + " days ";
                    if (riclear.GetTimeSpan().Hours > 0)
                        totalTimeStr += riclear.GetTimeSpan().Hours + " hours ";
                    if (riclear.GetTimeSpan().Minutes > 0)
                        totalTimeStr += riclear.GetTimeSpan().Minutes + " mins ";
                    if (riclear.GetTimeSpan().Seconds > 0)
                        totalTimeStr += riclear.GetTimeSpan().Seconds + " secs ";*/
                    string factionColor = "#CCCCCC";

                    try 
	                {
                        var recordedByPlayer = ApplicationInstance.Instance.GetRealmDB(riclear.m_Raid.CacheGroup.Realm).RD_FindPlayer(riclear.m_Raid.BossFights.Last().PlayerFightData.First().Item1, riclear.m_Raid.m_RaidMembers);
                        var guildFaction = VF_RealmPlayersDatabase.StaticValues.GetFaction(recordedByPlayer.Character.Race);
                        if (guildFaction == VF_RealmPlayersDatabase.PlayerFaction.Horde)
                        {
                            factionColor = "#A75757";
                        }
                        else if (guildFaction == VF_RealmPlayersDatabase.PlayerFaction.Alliance)
                        {
                            factionColor = "#575fA7";
                        }
                        else
                            factionColor = "#FFFFFF";
	                }
	                catch (Exception)
	                {
                        factionColor = "#CCCCCC";
	                }

                    statsBars.Add(new PageUtility.StatsBarData
                    {
                        m_BeforeBarText = "#" + clearNr + " (" + PageUtility.CreateLink("RaidOverview.aspx?Raid=" 
                        + riclear.m_Raid.UniqueRaidID
                        , riclear.m_Raid.RaidStartDate.ToString("yyyy-MM-dd")) + ")",
                        m_OnBarLeftText = PageUtility.CreateLink("RaidList.aspx?Guild=" + riclear.m_Raid.CacheGroup.GroupName, riclear.m_Raid.CacheGroup.GroupName),
                        /*PageUtility.CreateLink("http://realmplayers.com/GuildViewer.aspx?realm=" 
                            + RealmPlayersServer.StaticValues.ConvertRealmParam(riclear.m_Raid.Realm) 
                            + "&guild=" + riclear.m_Raid.RaidOwnerName, riclear.m_Raid.RaidOwnerName)*/
                        m_BarColor = factionColor,
                        m_PercentageWidth = compareValue / firstClearCompareValue,
                        m_AfterBarText = PageUtility.CreateColorisedFactor(1.0),
                        //m_BarTextColor = "#000",
                        m_OnBarRightText = totalTimeStr,
                        m_OnBarTextWidth = StaticValues.MeasureStringLength(riclear.m_Raid.CacheGroup.GroupName + " " + totalTimeStr)
                    });
                }

                statsBarStyle.m_TitleText = riclears.Key;
                graphSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, statsBars, 25);
            }

            m_GraphsHTML = new MvcHtmlString(graphSection);
        }

    }
}