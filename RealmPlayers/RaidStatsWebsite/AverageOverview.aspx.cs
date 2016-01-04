using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;
using PlayerData = VF_RealmPlayersDatabase.PlayerData.Player;
using TotalPlayerBossStats = VF_RDDatabase.TotalPlayerBossStats;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;

namespace VF.RaidDamageWebsite
{
    public partial class AverageOverview : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_InfoTextHTML = null;
        public MvcHtmlString m_GraphSection = null;

        public class AverageStats
        {
            public float m_DPS;
            public float m_HPS;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string bossName = PageUtility.GetQueryString(Request, "Boss", "All");
            string instanceName = PageUtility.GetQueryString(Request, "Instance", null);
            string andPlayer = PageUtility.GetQueryString(Request, "AndPlayer", null);
            int showPlayerCount = PageUtility.GetQueryInt(Request, "Count", 100);
            if (showPlayerCount > 200)
                showPlayerCount = 200;
            if (VF_RaidDamageDatabase.BossInformation.BossFights.ContainsKey(bossName) == false && instanceName == null)
            {
                instanceName = bossName;
            }
            else if (instanceName != null)
                bossName = instanceName;

            this.Title = bossName + " Rankings | RaidStats";

            List<PlayerClass> classLimits = ClassControl.GetClassLimits();// FightOverallOverview.GetClassLimits(PageUtility.GetQueryString(Request, "ClassLimit", "WrIWaIWlIMaIPrIShIRoIPaIDrIHu"));
            List<PlayerFaction> factionLimits = ClassControl.GetFactionLimits();
            bool showMultipleEntries = PageUtility.GetQueryString(Request, "MultipleEntries", "false").ToLower() != "false";

            var realm = RealmControl.Realm;

            string guildLimit = PageUtility.GetQueryString(Request, "Guild", null);

            List<VF_RDDatabase.BossFight> fightInstances = new List<VF_RDDatabase.BossFight>();

            DateTime earliestCompatibleDate = new DateTime(2013, 10, 23, 0, 0, 0);
            //var raidCollection = ApplicationInstance.Instance.GetRaidCollection();

            List<Tuple<PlayerData, AverageStats>> dataset = GenerateAverageDataSet(bossName, instanceName, classLimits, factionLimits, realm, guildLimit);
            if (dataset == null)
                return;

            string infoText = "";
            string breadCrumbCommon = "";

            if (guildLimit != null)
            {
                string colorClasses = ClassControl.GetColorClassesStr();

                infoText = "<h1>Average Performance for players in " + guildLimit + " vs " + bossName + "</h1>";
                breadCrumbCommon = PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm)
                    + PageUtility.BreadCrumb_AddGuildRaidList(guildLimit);

            }
            else
            {
                infoText = "<h1>Average Performance for players vs " + bossName + "</h1>";
                breadCrumbCommon = PageUtility.BreadCrumb_AddHome()
                    + PageUtility.BreadCrumb_AddRealm(realm);
            }

            if (ClassControl.HasClassLimits() == true)
            {
                string colorClasses = ClassControl.GetColorClassesStr();
                string breadCrumb = breadCrumbCommon + PageUtility.BreadCrumb_AddThisPageWithout("Average Performance vs " + bossName, Request, "ClassLimit");

                if (ClassControl.HasFactionLimits() == true)
                {
                    breadCrumb += PageUtility.BreadCrumb_AddFinish("for " + colorClasses);
                    breadCrumb += PageUtility.BreadCrumb_AddFinish(ClassControl.GetColorFactionStr());
                }
                else
                {
                    breadCrumb += PageUtility.BreadCrumb_AddFinish("for " + colorClasses);
                }
                m_BreadCrumbHTML = new MvcHtmlString(breadCrumb);
            }
            else
            {
                if (ClassControl.HasFactionLimits() == true)
                {
                    m_BreadCrumbHTML = new MvcHtmlString(breadCrumbCommon
                        + PageUtility.BreadCrumb_AddThisPageWithout("Average Performance vs " + bossName, Request, "FactionLimit")
                        + PageUtility.BreadCrumb_AddFinish(ClassControl.GetColorFactionStr()));
                }
                else
                {
                    m_BreadCrumbHTML = new MvcHtmlString(breadCrumbCommon
                        + PageUtility.BreadCrumb_AddFinish("Average Performance vs " + bossName));
                }
            }

            if(instanceName != null)
            {
                infoText += "<p>Instance fight average is calculated by averaging every boss fight averages from " + instanceName + ".<br/>";
                infoText += "Boss fight average is calculated by taking the 5 best performing(for the player) encounters out of the last 6 attended.</p>";
                infoText += "<p>If there are less than 3 encounters for the player for any of the bosses in the database or the last attended encounter was more than 1 month ago, the player will not be included in the list.</p>";
            }
            else
            {
                infoText += "<p>Boss fight average is calculated by taking the 5 best performing(for the player) encounters out of the last 6 attended.</p>";
                infoText += "<p>If there are less than 3 encounters for the player in the database or the last attended encounter was more than 1 month ago, the player will not be included in the list.</p>";
            }

            m_InfoTextHTML = new MvcHtmlString(infoText);

            if (dataset.Count == 0)
                return;

            string graphSection = "<style>" + PageUtility.CreateStatsBars_HTML_CSSCode() + "</style>";

            PageUtility.StatsBarStyle statsBarStyle = new PageUtility.StatsBarStyle
            {
                m_TitleText = "Average DPS",
                m_BarTextColor = "#000",
                m_LeftSideTitleText = "#",
                m_RightSideTitleText = "",
                m_BeforeBarWidth = 30,
                m_MaxWidth = 400,
                m_AfterBarWidth = 0
            };
            {
                int maxCount = showPlayerCount;

                var orderedByDPS = dataset.OrderByDescending((_Value) => _Value.Item2.m_DPS);
                float highestDPS = orderedByDPS.First().Item2.m_DPS;
                int players = 0;
                List<PageUtility.StatsBarData> dpsStatsBars = new List<PageUtility.StatsBarData>();
                foreach (var data in orderedByDPS)
                {
                    if (++players > maxCount || data.Item2.m_DPS < 1)
                    {
                        if (andPlayer == null)
                            break;
                        else if (data.Item1.Name != andPlayer)
                            continue;
                    }
                    float averageDPS = data.Item2.m_DPS;
                    double displayPercentage = averageDPS / highestDPS;
                    string rightSideText = averageDPS.ToStringDot("0.0") + "/s";
                    dpsStatsBars.Add(new PageUtility.StatsBarData
                    {
                        m_BeforeBarText = "#" + players,
                        m_OnBarLeftText = PageUtility.CreateLink_RaidStats_Player(data.Item1),
                        m_BarColor = PageUtility.GetClassColor(data.Item1),
                        m_PercentageWidth = displayPercentage,
                        m_AfterBarText = "",
                        //m_BarTextColor = "#000",
                        m_OnBarRightText = rightSideText,
                        m_OnBarTextWidth = StaticValues.MeasureStringLength(data.Item1.Name + " " + rightSideText)
                    });
                }
                var orderedByHPS = dataset.OrderByDescending((_Value) => _Value.Item2.m_HPS);
                float highestHPS = orderedByHPS.First().Item2.m_HPS;
                players = 0;
                List<PageUtility.StatsBarData> hpsStatsBars = new List<PageUtility.StatsBarData>();
                foreach (var data in orderedByHPS)
                {
                    if (++players > maxCount || data.Item2.m_HPS < 1)
                    {
                        if (andPlayer == null)
                            break;
                        else if (data.Item1.Name != andPlayer)
                            continue;
                    }
                    float averageHPS = data.Item2.m_HPS;
                    double displayPercentage = averageHPS / highestHPS;

                    string rightSideText = averageHPS.ToStringDot("0.0") + "/s";
                    hpsStatsBars.Add(new PageUtility.StatsBarData
                    {
                        m_BeforeBarText = "#" + players,
                        m_OnBarLeftText = PageUtility.CreateLink_RaidStats_Player(data.Item1),
                        m_BarColor = PageUtility.GetClassColor(data.Item1),
                        m_PercentageWidth = displayPercentage,
                        m_AfterBarText = "",
                        //m_BarTextColor = "#000",
                        m_OnBarRightText = rightSideText,
                        m_OnBarTextWidth = StaticValues.MeasureStringLength(data.Item1.Name + " " + rightSideText)
                    });
                }

                while(dpsStatsBars.Count > hpsStatsBars.Count)
                {
                    hpsStatsBars.Add(new PageUtility.StatsBarData{
                        m_AfterBarText = "",
                        m_BarColor = "#CCCCCC",
                        m_BeforeBarText = "",
                        m_OnBarLeftText = "",
                        m_OnBarRightText = "",
                        m_PercentageWidth = 0.0,
                    });
                }
                while(hpsStatsBars.Count > dpsStatsBars.Count)
                {
                    dpsStatsBars.Add(new PageUtility.StatsBarData{
                        m_AfterBarText = "",
                        m_BarColor = "#CCCCCC",
                        m_BeforeBarText = "",
                        m_OnBarLeftText = "",
                        m_OnBarRightText = "",
                        m_PercentageWidth = 0.0,
                    });
                }
                statsBarStyle.m_TitleText = "Average DPS vs " + bossName;
                graphSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, dpsStatsBars, 50, 1);
                graphSection += "&nbsp;&nbsp;&nbsp;&nbsp;";
                statsBarStyle.m_TitleText = "Average Effective HPS vs " + bossName;
                graphSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, hpsStatsBars, 50, 1);

            }

            m_GraphSection = new MvcHtmlString(graphSection);
        }

        public static List<Tuple<PlayerData, AverageStats>> GenerateAverageDataSet(string bossOrInstanceName, List<PlayerClass> classLimits = null, List<PlayerFaction> factionLimits = null, WowRealm realm = WowRealm.All, string guildLimit = null)
        {
            return GenerateAverageDataSet(bossOrInstanceName, bossOrInstanceName, classLimits, factionLimits, realm, guildLimit);
        }
        public static List<Tuple<PlayerData, AverageStats>> GenerateAverageDataSet(string bossName, string instanceName, List<PlayerClass> classLimits, List<PlayerFaction> factionLimits, WowRealm realm, string guildLimit)
        {
            var summaryDatabase = ApplicationInstance.Instance.GetSummaryDatabase();
            if (summaryDatabase == null)
                return null;

            var realmDB = ApplicationInstance.Instance.GetRealmDB(VF_RealmPlayersDatabase.WowRealm.Nostalrius);

            var orderedPlayers = summaryDatabase.PlayerSummaries.OrderBy(_Value => _Value.Key); //Order by realm to avoid changing realmDB all the time

            List<Tuple<PlayerData, AverageStats>> dataset = new List<Tuple<PlayerData, AverageStats>>();

            //DateTime activePlayerDateTimeLimit = DateTime.Now.AddMonths(-2);
            //DateTime activePlayerDateTimeLimit = DateTime.Now.AddMonths(-2);

            Func<TotalPlayerBossStats, float> getDPSFunc = (_Value) => _Value.GetAverageDPS(5, 6, 3);
            Func<TotalPlayerBossStats, float> getEffectiveHPSFunc = (_Value) => _Value.GetAverageEffectiveHPS(5, 6, 3);
            foreach (var playerSummary in orderedPlayers)
            {
                if (playerSummary.Value.AttendedFights.Count == 0)
                    continue;
                var currGroup = playerSummary.Value.AttendedFights.Last().CacheRaid.CacheGroup;

                if (guildLimit != null && guildLimit != currGroup.GroupName)
                    continue;

                if (realm != VF_RealmPlayersDatabase.WowRealm.All && realm != currGroup.Realm)
                    continue;

                if (currGroup.Realm != realmDB.Realm)
                    realmDB = ApplicationInstance.Instance.GetRealmDB(currGroup.Realm);

                AverageStats averageStats = new AverageStats();
                //if (bossName == "All")
                //{
                //    averageStats.m_DPS = playerSummary.Value.GenerateTotalAverageData(10, getDPSFunc, null, activePlayerDateTimeLimit);
                //    averageStats.m_HPS = playerSummary.Value.GenerateTotalAverageData(10, getEffectiveHPSFunc, null, activePlayerDateTimeLimit);
                //}
                //else 
                if (instanceName == "Naxxramas - All Quarters")
                {
                    Func<string, bool> includeBossFunc = (_Value) => { return VF_RaidDamageDatabase.BossInformation.BossFights[_Value] == "Naxxramas" && _Value != "Sapphiron" && _Value != "Kel'Thuzad"; };
                    averageStats.m_DPS = playerSummary.Value.GenerateTotalAverageData(13, getDPSFunc, includeBossFunc);
                    averageStats.m_HPS = playerSummary.Value.GenerateTotalAverageData(13, getEffectiveHPSFunc, includeBossFunc);
                }
                else if (instanceName == "AQ40" || instanceName == "Ahn'Qiraj Temple")
                {
                    Func<string, bool> includeBossFunc = (_Value) => { return VF_RaidDamageDatabase.BossInformation.BossFights[_Value] == "Ahn'Qiraj Temple"; };
                    averageStats.m_DPS = playerSummary.Value.GenerateTotalAverageData(9, getDPSFunc, includeBossFunc);
                    averageStats.m_HPS = playerSummary.Value.GenerateTotalAverageData(9, getEffectiveHPSFunc, includeBossFunc);
                }
                else if (instanceName == "BWL" || instanceName == "Blackwing Lair")
                {
                    Func<string, bool> includeBossFunc = (_Value) => { return VF_RaidDamageDatabase.BossInformation.BossFights[_Value] == "Blackwing Lair"; };
                    averageStats.m_DPS = playerSummary.Value.GenerateTotalAverageData(8, getDPSFunc, includeBossFunc);
                    averageStats.m_HPS = playerSummary.Value.GenerateTotalAverageData(8, getEffectiveHPSFunc, includeBossFunc);
                }
                else if (instanceName == "MC" || instanceName == "Molten Core")
                {
                    Func<string, bool> includeBossFunc = (_Value) => { return VF_RaidDamageDatabase.BossInformation.BossFights[_Value] == "Molten Core"; };
                    averageStats.m_DPS = playerSummary.Value.GenerateTotalAverageData(10, getDPSFunc, includeBossFunc);
                    averageStats.m_HPS = playerSummary.Value.GenerateTotalAverageData(10, getEffectiveHPSFunc, includeBossFunc);
                }
                else if (instanceName == "ZG" || instanceName == "Zul'Gurub")
                {
                    Func<string, bool> includeBossFunc = (_Value) =>
                    {
                        return VF_RaidDamageDatabase.BossInformation.BossFights[_Value] == "Zul'Gurub"
                            && _Value != "Gahz'ranka" //Doesnt work so lets skip it
                            && _Value != "Gri'lek" && _Value != "Renataki" && _Value != "Hazza'rah" && _Value != "Wushoolay"; //Not reliable
                    };
                    averageStats.m_DPS = playerSummary.Value.GenerateTotalAverageData(8, getDPSFunc, includeBossFunc);
                    averageStats.m_HPS = playerSummary.Value.GenerateTotalAverageData(8, getEffectiveHPSFunc, includeBossFunc);
                }
                else if (instanceName == "All")
                {
                    float bwlDPS, bwlHPS;
                    float mcDPS, mcHPS;
                    float zgDPS, zgHPS;
                    {
                        //BWL
                        Func<string, bool> includeBossFunc = (_Value) => { return VF_RaidDamageDatabase.BossInformation.BossFights[_Value] == "Blackwing Lair"; };
                        bwlDPS = playerSummary.Value.GenerateTotalAverageData(8, getDPSFunc, includeBossFunc);
                        bwlHPS = playerSummary.Value.GenerateTotalAverageData(8, getEffectiveHPSFunc, includeBossFunc);
                    }
                    {
                        //MC
                        Func<string, bool> includeBossFunc = (_Value) => { return VF_RaidDamageDatabase.BossInformation.BossFights[_Value] == "Molten Core"; };
                        mcDPS = playerSummary.Value.GenerateTotalAverageData(10, getDPSFunc, includeBossFunc);
                        mcHPS = playerSummary.Value.GenerateTotalAverageData(10, getEffectiveHPSFunc, includeBossFunc);
                    }
                    {
                        //ZG
                        Func<string, bool> includeBossFunc = (_Value) =>
                        {
                            return VF_RaidDamageDatabase.BossInformation.BossFights[_Value] == "Zul'Gurub"
                                && _Value != "Gahz'ranka" //Doesnt work so lets skip it
                                && _Value != "Gri'lek" && _Value != "Renataki" && _Value != "Hazza'rah" && _Value != "Wushoolay"; //Not reliable
                        };
                        zgDPS = playerSummary.Value.GenerateTotalAverageData(8, getDPSFunc, includeBossFunc);
                        zgHPS = playerSummary.Value.GenerateTotalAverageData(8, getEffectiveHPSFunc, includeBossFunc);
                    }

                    {
                        //Average it
                        //averageStats.m_DPS = (bwlDPS * 0.5f + mcDPS * 0.3f + zgDPS * 0.2f);
                        //averageStats.m_HPS = (bwlHPS * 0.5f + mcHPS * 0.3f + zgHPS * 0.2f);

                        averageStats.m_DPS = (bwlDPS + mcDPS + zgDPS) / 3;
                        averageStats.m_HPS = (bwlHPS + mcHPS + zgHPS) / 3;
                    }
                }
                else
                {
                    var bossStats = playerSummary.Value.GetBossStats(bossName);
                    if (bossStats == null)
                        continue;
                    if ((bossStats.m_PlayerFightDatas.Count > 0 
                        && bossStats.m_PlayerFightDatas.First().CacheBossFight.StartDateTime > DateTime.Now.AddMonths(-1))
                        || playerSummary.Value.GetLatestAttendedBossFightDateTime(bossName) > DateTime.Now.AddMonths(-1))
                    {
                        averageStats.m_DPS = getDPSFunc(bossStats);
                        averageStats.m_HPS = getEffectiveHPSFunc(bossStats);
                    }
                }

                var playerData = realmDB.FindPlayer(playerSummary.Value.Name);
                if (playerData != null && (classLimits == null || classLimits.Contains(playerData.Character.Class))
                    && (factionLimits == null || factionLimits.Contains(StaticValues.GetFaction(playerData.Character.Race))))
                {
                    dataset.Add(Tuple.Create(playerData, averageStats));
                }
            }
            return dataset;
        }
    }
}