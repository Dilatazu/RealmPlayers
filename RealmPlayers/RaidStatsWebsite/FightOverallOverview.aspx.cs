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


namespace VF_RaidDamageWebsite
{
    public partial class FightOverallOverview : System.Web.UI.Page
    {
        public class DataPresentTypeInfo
        {
            public Func<VF_RDDatabase.PlayerFightData, double> m_GetValue;
            public string m_TypeName;
            public int m_Count;
            public Func<VF_RDDatabase.PlayerFightData, bool> m_ValidCheck;
            public DataPresentTypeInfo(Func<VF_RDDatabase.PlayerFightData, double> _GetValue, string _TypeName, int _Count = -1, Func<VF_RDDatabase.PlayerFightData, bool> _ValidCheck = null)
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

        public static List<DataPresentTypeInfo> sm_DataPresentTypeInfoList = new List<DataPresentTypeInfo>{
            new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.Damage; }, "Damage", 25),
            //new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.DPS; }, "DPS", 10, (VF_RDDatabase.PlayerFightData _UnitData) => { return _UnitData.Dmg > 0; }),
            new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.EffectiveHeal; }, "Effective Heal"),
            //new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.HPS; }, "HPS", 10, (VF_RDDatabase.PlayerFightData _UnitData) => { return _UnitData.EffHeal > 0; }),
            new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.OverHeal; }, "Overheal"),
            new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.RawHeal; }, "Raw Heal"),
            //new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.ThreatValue; }, "Threat", 25),
            new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.DamageTaken; }, "Damage Taken", 25),
            //new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.Dmg; }, "Damage"),
            //new DataPresentTypeInfo((VF_RDDatabase.PlayerFightData _UnitData) => { return (double)_UnitData.Dmg; }, "Damage"),
        };


        class PlayerFightValuePerSecond
        {
            public VF_RDDatabase.BossFight m_RaidBossFight;
            public VF_RealmPlayersDatabase.PlayerData.Player m_Player;
            public float m_ValuePerSecond;
        }

        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_InfoTextHTML = null;
        public MvcHtmlString m_GraphSection = null;

        //public static Dictionary<string, PlayerClass> ClassLimitConverter = new Dictionary<string,PlayerClass>{
        //    {"Wr", PlayerClass.Warrior},
        //    {"Wl", PlayerClass.Warlock},
        //    {"Ma", PlayerClass.Mage},
        //    {"Pr", PlayerClass.Priest},
        //    {"Sh", PlayerClass.Shaman},
        //    {"Ro", PlayerClass.Rogue},
        //    {"Pa", PlayerClass.Paladin},
        //    {"Dr", PlayerClass.Druid},
        //    {"Hu", PlayerClass.Hunter},
        //};

        //public static string GetClassLimitStr(List<PlayerClass> _Classes)
        //{
        //    string retString = "";
        //    foreach(var _Class in _Classes)
        //    {
        //        retString = retString + GetClassLimitStr(_Class) + "I";
        //    }
        //    return retString;
        //}
        //public static string GetClassLimitStr(PlayerClass _Class)
        //{
        //    var firstOrDefault = ClassLimitConverter.FirstOrDefault((_Value) => _Value.Value == _Class);
        //    if (firstOrDefault.Equals(default(KeyValuePair<string, PlayerClass>)) == false)
        //        return firstOrDefault.Key;
        //    return "";
        //}
        //public static List<PlayerClass> GetClassLimits(string _ClassLimitStr)
        //{
        //    if (_ClassLimitStr == "")
        //        return null;
        //    List<PlayerClass> retList = new List<PlayerClass>();
        //    string[] classLimits = _ClassLimitStr.Split('I');
        //    foreach (var classLimit in classLimits)
        //    {
        //        PlayerClass playerClass;
        //        if(ClassLimitConverter.TryGetValue(classLimit, out playerClass) == true)
        //            retList.Add(playerClass);
        //    }
        //    if (retList.Count == 0)
        //        return null;
        //    else if (retList.Count >= ClassLimitConverter.Count)
        //    {
        //        foreach (var classLimit in ClassLimitConverter)
        //        {
        //            if (retList.Contains(classLimit.Value) == false)
        //                return retList;
        //        }
        //        return null;
        //    }
        //    return retList;
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            string fightName = PageUtility.GetQueryString(Request, "FightName");
            List<PlayerClass> classLimits = ClassControl.GetClassLimits();//PageUtility.GetQueryString(Request, "ClassLimit", "WrIWaIWlIMaIPrIShIRoIPaIDrIHu"));
            List<PlayerFaction> factionLimits = ClassControl.GetFactionLimits();
            bool showMultipleEntries = PageUtility.GetQueryString(Request, "MultipleEntries", "false").ToLower() != "false";

            string guildLimit = PageUtility.GetQueryString(Request, "Guild", null);
            string andPlayer = PageUtility.GetQueryString(Request, "AndPlayer", null);
            int showEntriesCount = PageUtility.GetQueryInt(Request, "Count", 50);
            if (showEntriesCount > 100)
                showEntriesCount = 100;

            List<VF_RDDatabase.BossFight> fightInstances = new List<VF_RDDatabase.BossFight>();

            DateTime earliestCompatibleDate = new DateTime(2013, 10, 23, 0, 0, 0);
            var realm = RealmControl.Realm;

            var realmDB = ApplicationInstance.Instance.GetRealmDB(VF_RealmPlayersDatabase.WowRealm.Emerald_Dream);
            //var raidCollection = ApplicationInstance.Instance.GetRaidCollection();
            double highestPrecision = 0;
            double totalPrecision = 0;
            var summaryDatabase = ApplicationInstance.Instance.GetSummaryDatabase();
            if (summaryDatabase == null)
                return;

            if (guildLimit != null)
            {
                this.Title = fightName + " Highscore for " + guildLimit + " | RaidStats";
            }
            else
            {
                this.Title = fightName + " Highscore | RaidStats";
            }

            foreach (var groupRC in summaryDatabase.GroupRCs)
            {
                if (guildLimit != null && guildLimit != groupRC.Value.GroupName)
                    continue;

                foreach (var raid in groupRC.Value.Raids)
                {
                    foreach (var bossFight in raid.Value.BossFights)
                    {
                        if (bossFight.BossName == fightName && bossFight.IsQualityHigh()
                        && bossFight.StartDateTime > earliestCompatibleDate)
                        {
                            double precision = bossFight.DataDetails.FightPrecision;// fight.CalculatePrecision(realmDB.RD_IsPlayer);
                            fightInstances.Add(bossFight);

                            if (precision > highestPrecision)
                                highestPrecision = precision;
                            totalPrecision += precision;
                        }
                    }
                }
            }
            double averagePrecision = totalPrecision / fightInstances.Count;
            double acceptablePrecisionMin = averagePrecision - 0.05;
            //Remove fights that have too low precision
            fightInstances.RemoveAll((_Value) => { return _Value.DataDetails.FightPrecision < acceptablePrecisionMin; });
            if (fightInstances.Count > 0)
            {
                Dictionary<string, List<PlayerFightValuePerSecond>> combinedData = new Dictionary<string, List<PlayerFightValuePerSecond>>();
                foreach (var raidFight in fightInstances)
                {
                    var currGroup = raidFight.CacheRaid.CacheGroup;

                    if (realm != VF_RealmPlayersDatabase.WowRealm.All && realm != currGroup.Realm)
                        continue;

                    if (currGroup.Realm != realmDB.Realm)
                        realmDB = ApplicationInstance.Instance.GetRealmDB(currGroup.Realm);

                    var unitsData = raidFight.PlayerFightData;// currBossFight.GetFilteredPlayerUnitsData(true, realmDB.RD_GetPlayerIdentifier);
                    foreach (var dataPresentTypeInfo in sm_DataPresentTypeInfoList)
                    {
                        var sortedUnits = unitsData.OrderByDescending((_Unit) => { return dataPresentTypeInfo.m_GetValue(_Unit.Item2); });
                        if (sortedUnits.Count() > 0)
                        {
                            foreach (var unit in sortedUnits)
                            {
                                var playerData = realmDB.RD_FindPlayer(unit.Item1, raidFight);
                                double currValue = dataPresentTypeInfo.m_GetValue(unit.Item2);
                                if (playerData != null && currValue > 0 
                                    && (classLimits == null || classLimits.Contains(playerData.Character.Class))
                                    && (factionLimits == null || factionLimits.Contains(StaticValues.GetFaction(playerData.Character.Race))) 
                                    && dataPresentTypeInfo.m_ValidCheck(unit.Item2))
                                {
                                    float valuePerSecond = (float)(currValue / raidFight.FightDuration);
                                    combinedData.AddToList(dataPresentTypeInfo.m_TypeName
                                        , new PlayerFightValuePerSecond
                                        {
                                            m_RaidBossFight = raidFight,
                                            m_Player = playerData,
                                            m_ValuePerSecond = valuePerSecond
                                        });
                                }
                            }
                        }
                    }
                }
                PageUtility.StatsBarStyle statsBarStyle = new PageUtility.StatsBarStyle
                {
                    m_TitleText = "",
                    m_BarTextColor = "#000",
                    m_LeftSideTitleText = "#",
                    m_RightSideTitleText =
                        //"<div style='overflow: hidden; white-space: nowrap;' title='Fight Precision Comparison. \nComparison vs highest precision, higher value may mean more accurate data.'>FPC</div>"
                        //"<div style='overflow: hidden; white-space: nowrap; display:inline-block; '>" 
                        PageUtility.CreateTooltipText("FPC", "Fight Precision Comparison. \nComparison vs highest precision, higher value may mean more accurate data."),
                    //+ "</div>",
                    m_BeforeBarWidth = 100,
                    m_MaxWidth = 700,
                    m_AfterBarWidth = 30
                };

                string breadCrumbCommon = "";
                if (guildLimit != null)
                {
                    breadCrumbCommon = PageUtility.BreadCrumb_AddHome()
                        + PageUtility.BreadCrumb_AddRealm(realm)
                        + PageUtility.BreadCrumb_AddLink("RaidList.aspx?Guild=" + guildLimit, guildLimit) 
                        + PageUtility.BreadCrumb_AddLink("BossList.aspx?Guild=" + guildLimit, "Bosses");
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

                string graphSection = "<h1>Highscore for players vs " + fightName + "</h1><p>Fights with unrealistic dmg spikes(SW_Stats reset bug) are disqualified from this list.</p>";
                //graphSection += "<p>View Highscore for class: ";
                //foreach (var classLimit in ClassLimitConverter)
                //{
                //    graphSection += PageUtility.CreateLink(PageUtility.CreateUrlWithNewQueryValue(Request, "ClassLimit", classLimit.Key), PageUtility.CreateColorCodedName(classLimit.Value.ToString(), classLimit.Value)) + ", ";
                //}
                if (showMultipleEntries == false)
                {
                    graphSection += "<p>Currently <u>not</u> showing multiple entries per player/guild. <a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "MultipleEntries", "true") + "'>Click here if you want to show multiple entries per entities</a></p>";
                }
                else
                {
                    graphSection += "<p>Currently showing multiple entries per player/guild. <a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "MultipleEntries", "false") + "'>Click here if you do not want to show multiple entries per entities</a></p>";
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
                graphSection = "<style>" + PageUtility.CreateStatsBars_HTML_CSSCode() + "</style>";

                string dmgHealSection = "";// "<div class='span4' style='min-width: 460px;'>";
                string killTimesSection = "";// "<div class='span4' style='min-width: 460px;'>";
                int totalBarsCount = 0;
                foreach (var dataPresentTypeInfo in sm_DataPresentTypeInfoList)
                {
                    if (dataPresentTypeInfo.m_TypeName == "Damage Taken")
                        continue;
                    if (combinedData.ContainsKey(dataPresentTypeInfo.m_TypeName) == false)
                        continue;
                    var candidatesList = combinedData[dataPresentTypeInfo.m_TypeName];
                    List<PageUtility.StatsBarData> statsBars = new List<PageUtility.StatsBarData>();
                    //string newBossSection = "";
                    //newBossSection += "VF_CreateDmgBar(1.0, '#000000', '" + dataPresentTypeInfo.m_TypeName + "','#ffffff', 'only fights with precision higher than " + acceptablePrecisionMin.ToString("0%", System.Globalization.CultureInfo.InvariantCulture) + "');";
                    var orderedCandidates = candidatesList.OrderByDescending((_Value) => { return _Value.m_ValuePerSecond; });
                    int players = 0;
                    double highestValuePerSecond = orderedCandidates.First().m_ValuePerSecond;

                    List<string> ignorePlayers = new List<string>();
                    foreach (var candidate in orderedCandidates)
                    {
                        if (showMultipleEntries == false)
                        {
                            if (ignorePlayers.Contains(candidate.m_Player.Name) == true)
                            {
                                continue;
                            }
                            else
                            {
                                ignorePlayers.Add(candidate.m_Player.Name);
                            }
                        }
                        ++players;
                        if (players > dataPresentTypeInfo.m_Count && players > showEntriesCount)
                        {
                            if (andPlayer == null)
                                break;
                            else if (candidate.m_Player.Name != andPlayer)
                                continue;
                        }

                        double displayPercentage = candidate.m_ValuePerSecond / highestValuePerSecond;

                        var playerData = candidate.m_Player;
                        if (playerData != null)
                        {
                            //newBossSection += "VF_CreateDmgBar(" + displayPercentage.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", '" + classColor + "', '<player," + candidate.m_PlayerName + ">" + candidate.m_PlayerName + "(" + candidate.m_Fight.m_Fight.StartDateTime.ToString("yyyy-MM-dd") + ")','#000000', '" + candidate.m_ValuePerSecond.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "/s');";
                            string fightLengthText = " (" + PageUtility.CreateLink_FightOverview(candidate.m_RaidBossFight, candidate.m_RaidBossFight.FightDuration + " seconds") + ")";
                            string rightSideText = candidate.m_ValuePerSecond.ToStringDot("0.0") + "/s";
                            statsBars.Add(new PageUtility.StatsBarData
                            {
                                m_BeforeBarText = "#" + players + " (" + PageUtility.CreateLink_RaidOverview(candidate.m_RaidBossFight.CacheRaid, candidate.m_RaidBossFight.StartDateTime.ToString("yyyy-MM-dd")) + ")",
                                m_OnBarLeftText = PageUtility.CreateLink_RaidStats_Player(candidate.m_Player.Name, candidate.m_RaidBossFight.CacheRaid.CacheGroup.Realm) + fightLengthText,
                                m_BarColor = PageUtility.GetClassColor(playerData),
                                m_PercentageWidth = displayPercentage,
                                m_AfterBarText = PageUtility.CreateColorisedFactor(candidate.m_RaidBossFight.DataDetails.FightPrecision / highestPrecision),
                                //m_BarTextColor = "#000",
                                m_OnBarRightText = rightSideText,
                                m_OnBarTextWidth = StaticValues.MeasureStringLength(candidate.m_Player.Name + " (" + candidate.m_RaidBossFight.FightDuration + " seconds) " + rightSideText)
                            });
                        }
                    }

                    //graphSection += newBossSection;
                    totalBarsCount += 1 + players;
                    statsBarStyle.m_TitleText = dataPresentTypeInfo.m_TypeName;
                    if (dataPresentTypeInfo.m_TypeName == "Damage" || dataPresentTypeInfo.m_TypeName == "Threat" || dataPresentTypeInfo.m_TypeName == "Damage Taken")
                        dmgHealSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, statsBars, dataPresentTypeInfo.m_Count);
                    else
                        dmgHealSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, statsBars, dataPresentTypeInfo.m_Count);
                }
                {
                    var orderedFights = fightInstances.OrderBy((_Value) => _Value.FightDuration);
                    List<PageUtility.StatsBarData> statsBars = new List<PageUtility.StatsBarData>();
                    int raidCounter = 0;
                    double firstClearCompareValue = 1 / (double)orderedFights.First().FightDuration;

                    List<string> ignoreGuilds = new List<string>();
                    foreach (var raidFight in orderedFights)
                    {
                        if (realm != VF_RealmPlayersDatabase.WowRealm.All && realm != raidFight.CacheRaid.CacheGroup.Realm)
                            continue;

                        if (showMultipleEntries == false && guildLimit == null)
                        {
                            if (ignoreGuilds.Contains(raidFight.CacheRaid.CacheGroup.GroupName) == true)
                            {
                                continue;
                            }
                            else
                            {
                                ignoreGuilds.Add(raidFight.CacheRaid.CacheGroup.GroupName);
                            }
                        }
                        if (++raidCounter > showEntriesCount)
                            break;
                        double compareValue = 1.0 / (double)raidFight.FightDuration;

                        string factionColor = "#CCCCCC";
                        try 
	                    {
                            var recordedByPlayer = ApplicationInstance.Instance.GetRealmDB(raidFight.CacheRaid.CacheGroup.Realm).RD_FindPlayer(raidFight.PlayerFightData.First().Item1, raidFight);
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
                        if (factionColor == "#CCCCCC")
                        {
                            var raidOwner = raidFight.CacheRaid.CacheGroup.GroupName;
                            if(raidOwner == "Team Plague")
                                factionColor = "#575fA7";
                            else if (raidOwner == "Dreamstate")
                                factionColor = "#A75757";
                            else if (raidOwner == "Ridin Dirty")
                                factionColor = "#A75757";
                        }
                        string rightSideText = "" + (int)raidFight.FightDuration + " secs";
                        statsBars.Add(new PageUtility.StatsBarData
                        {
                            m_BeforeBarText = "#" + raidCounter + " (" + PageUtility.CreateLink_RaidOverview(raidFight.CacheRaid, raidFight.StartDateTime.ToString("yyyy-MM-dd")) + ")",
                            m_OnBarLeftText = PageUtility.CreateLink_GuildRaidList(raidFight.CacheRaid.CacheGroup.GroupName),
                            m_BarColor = factionColor,
                            m_PercentageWidth = compareValue / firstClearCompareValue,
                            m_AfterBarText = PageUtility.CreateColorisedFactor(raidFight.DataDetails.FightPrecision / highestPrecision),
                            //m_BarTextColor = "#000",
                            m_OnBarRightText = rightSideText,
                            m_OnBarTextWidth = StaticValues.MeasureStringLength(raidFight.CacheRaid.CacheGroup.GroupName + " " + rightSideText)
                        });
                    }
                    statsBarStyle.m_TitleText = "Kill Times";
                    killTimesSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, statsBars, 15);
                }

                graphSection += dmgHealSection + killTimesSection;
                //dmgHealSection += "</div>";
                //killTimesSection += "</div";
                //graphSection += "<div class='row'>" + dmgHealSection;// +"<div class='span4'></div>";// +"<div class='span1' style='min-width: 50px;'></div>";
                //graphSection += killTimesSection + "</div>";

                m_GraphSection = new MvcHtmlString(graphSection);
            }
        }
    }
}