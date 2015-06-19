using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerData = VF_RealmPlayersDatabase.PlayerData.Player;
using AverageStats = VF.RaidDamageWebsite.AverageOverview.AverageStats;

namespace VF.RaidDamageWebsite
{
    public partial class PlayerOverview : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_PageHTML = null;

        public static int GetDPSRank(List<Tuple<PlayerData, AverageStats>> _DataSet, string _PlayerName, out float _DPS, Predicate<PlayerData> _Predicate = null)
        {
            _DPS = 0.0f;
            var orderedByDPS = _DataSet.OrderByDescending((_Value) => _Value.Item2.m_DPS);
            int playersCounter = 0;
            foreach (var data in orderedByDPS)
            {
                if (_Predicate == null || _Predicate(data.Item1) == true)
                {
                    ++playersCounter;
                    if (data.Item1.Name == _PlayerName)
                    {
                        _DPS = data.Item2.m_DPS;
                        return playersCounter;
                    }
                }
            }
            return -1;
        }
        public static int GetHPSRank(List<Tuple<PlayerData, AverageStats>> _DataSet, string _PlayerName, out float _HPS, Predicate<PlayerData> _Predicate = null)
        {
            _HPS = 0.0f;
            var orderedByDPS = _DataSet.OrderByDescending((_Value) => _Value.Item2.m_HPS);
            int playersCounter = 0;
            foreach (var data in orderedByDPS)
            {
                if (_Predicate == null || _Predicate(data.Item1) == true)
                {
                    ++playersCounter;
                    if (data.Item1.Name == _PlayerName)
                    {
                        _HPS = data.Item2.m_HPS;
                        return playersCounter;
                    }
                }
            }
            return -1;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string playerStr = PageUtility.GetQueryString(Request, "player");
            var realm = PageUtility.GetQueryRealm(Request);
            if (realm == VF_RealmPlayersDatabase.WowRealm.Unknown)
                return;

            string guildLimit = PageUtility.GetQueryString(Request, "GuildLimit", null);

            this.Title = playerStr + " @ " + RealmPlayersServer.StaticValues.ConvertRealmParam(realm) + " | RaidStats";
            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                + PageUtility.BreadCrumb_AddFinish(realm.ToString())
                + PageUtility.BreadCrumb_AddFinish(playerStr));

            var summaryDatabase = ApplicationInstance.Instance.GetSummaryDatabase();
            if (summaryDatabase == null)
                return;

            var currPlayerSummary = summaryDatabase.GetPlayerSummary(playerStr, realm);
            if (currPlayerSummary == null ||currPlayerSummary.PlayerBossStats == null || currPlayerSummary.PlayerBossStats.Count == 0)
                return;

            var realmPlayer = ApplicationInstance.Instance.GetRealmPlayer(playerStr, realm);
            
            System.Text.StringBuilder pageBuilder = new System.Text.StringBuilder(10000);
            pageBuilder.Append("<h1>" + realmPlayer.Name + "</h1>");
            pageBuilder.Append("<h4>Character profile: " + PageUtility.CreateLink_Armory_Player_Colored(realmPlayer) + "</h4>");
            
            pageBuilder.Append("<h4>Attended raids in groups</h4>");

            var orderedBossFights = currPlayerSummary.AttendedFights.OrderByDescending((_Value) => _Value.EndDateTime);
            List<VF_RDDatabase.Raid> attendedRaids = new List<VF_RDDatabase.Raid>();
            foreach (var attendedBossFight in orderedBossFights)
            {
                attendedRaids.AddUnique(attendedBossFight.CacheRaid);
            }
            List<string> groupsAttended = new List<string>();
            attendedRaids.ForEach((_Value) => groupsAttended.AddUnique(_Value.CacheGroup.GroupName));
            foreach (var group in groupsAttended)
            {
                var raidsInGroup = attendedRaids.Where((_Value) => _Value.CacheGroup.GroupName == group); //Sorted, last raid attended is the first in list!
                var factionColor = PageUtility.GetFactionTextColor(raidsInGroup.First().CacheGroup.GetFaction(ApplicationInstance.Instance.GetRealmDB));
                pageBuilder.Append(PageUtility.CreateColorString(group, factionColor)
                    + "(" + raidsInGroup.Count() + " raids between " + raidsInGroup.Last().RaidStartDate.ToDateStr() + " and " + raidsInGroup.First().RaidEndDate.ToDateStr() + ")<br/>");
                //foreach (var raid in raidsInGroup)
                //{
                //    pageBuilder.Append(PageUtility.CreateLink_RaidOverview(raid, raid.RaidStartDate.ToString("yyyy-MM-dd")) + ", ");
                //}
                //pageBuilder.Append("<br/>");
            }

            pageBuilder.Append("<h4><a href='BossList.aspx?realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realmPlayer.Realm) + "&player=" + realmPlayer.Name + "'>Best boss performances</a></h4>");

            //pageBuilder.Append("<p><h3>vs Everything</h3>");
            //var averageAll = AverageOverview.GenerateAverageDataSet("All", null, null, realm, guildLimit);
            //float averageDPS, averageHPS;
            //var dpsRank = GetDPSRank(averageAll, realmPlayer.Name, out averageDPS);
            //var hpsRank = GetHPSRank(averageAll, realmPlayer.Name, out averageHPS);
            //pageBuilder.Append("<h4>Average DPS rank: " + PageUtility.CreateLink("AverageOverview.aspx?Instance=All&realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realmPlayer.Realm) + "&andplayer=" + realmPlayer.Name, "#" + dpsRank) + " with DPS " + averageDPS.ToStringDot("0.0") + "</h4>");
            //pageBuilder.Append("<h4>Average HPS rank: " + PageUtility.CreateLink("AverageOverview.aspx?Instance=All&realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(realmPlayer.Realm) + "&andplayer=" + realmPlayer.Name, "#" + hpsRank) + " with HPS " + averageHPS.ToStringDot("0.0") + "</h4>");
            //pageBuilder.Append("</p>");
            //pageBuilder.Append("<br />");

            var realmProgress = GetRealmProgress(realm);
            string instanceSummary = "";
            if (instanceSummary == "" && (int)realmProgress >= (int)VF_RealmPlayersDatabase.WowInstance.Naxxramas)
            {
                instanceSummary = GenerateSummaryForInstance(currPlayerSummary, realmPlayer, "Naxxramas - All Quarters", realm, guildLimit);
            }
            if (instanceSummary == "" && (int)realmProgress >= (int)VF_RealmPlayersDatabase.WowInstance.Temple_Of_Ahn_Qiraj)
            {
                instanceSummary = GenerateSummaryForInstance(currPlayerSummary, realmPlayer, "Ahn'Qiraj Temple", realm, guildLimit);
            }
            if (instanceSummary == "" && (int)realmProgress >= (int)VF_RealmPlayersDatabase.WowInstance.Blackwing_Lair)
            {
                instanceSummary = GenerateSummaryForInstance(currPlayerSummary, realmPlayer, "Blackwing Lair", realm, guildLimit);
            }
            if(instanceSummary == "")
            {
                instanceSummary = GenerateSummaryForInstance(currPlayerSummary, realmPlayer, "Molten Core", realm, guildLimit);
            }
            if (instanceSummary == "")
            {
                instanceSummary = GenerateSummaryForInstance(currPlayerSummary, realmPlayer, "Zul'Gurub", realm, guildLimit);
            }
            pageBuilder.Append(instanceSummary);
            
            /////////////////////////////////////////////
            pageBuilder.Append("<h2>Attended raids</h2>");
            foreach (var attendedRaid in attendedRaids)
            {
                pageBuilder.Append(PageUtility.CreateLink_RaidOverview(attendedRaid, attendedRaid.RaidInstance + " @ " + attendedRaid.RaidStartDate) + "<br/>");
            }

            //pageBuilder.Append("<h2>Average Performance vs</h2>");
            //foreach (var bossData in currPlayerSummary.PlayerBossStats)
            //{
            //    if (bossData.Value.GetSamplesCount() < 3)
            //        continue;
            //    List<VF_RDDatabase.PlayerFightData> dpsSamplesUsed = null;
            //    List<VF_RDDatabase.PlayerFightData> hpsSamplesUsed = null;
            //    List<VF_RDDatabase.PlayerFightData> deathSamplesUsed = null;
            //    float averageDPS = bossData.Value.GetAverageDPS(5, 6, 3, out dpsSamplesUsed);
            //    float averageEffectiveHPS = bossData.Value.GetAverageEffectiveHPS(5, 6, 3, out hpsSamplesUsed);
            //    float averageRawHPS = bossData.Value.GetAverageRawHPS(5, 6, 3);
            //    float averageDeaths = bossData.Value.GetAverageDeaths(5, 6, 3, out deathSamplesUsed);
                
            //    pageBuilder.Append("<h3>" + bossData.Key + ":</h3>");
            //    if (averageDPS > 100 || averageEffectiveHPS < 20)
            //    {
            //        pageBuilder.Append("<br/>DPS(<font color='#ff0000'>" + averageDPS.ToStringDot("0.0") + "</font>) Samples(");
            //        foreach (var dpsSample in dpsSamplesUsed)
            //        {
            //            pageBuilder.Append(PageUtility.CreateLink_FightOverview(dpsSample.CacheBossFight, dpsSample.CacheBossFight.StartDateTime.ToString("yyyy-MM-dd")) + ", ");
            //        }
            //        pageBuilder.Length = pageBuilder.Length - 2;
            //        pageBuilder.Append(")");
            //    }
            //    if(averageEffectiveHPS >= 20)
            //    {
            //        pageBuilder.Append("<br/>Effective HPS(<font color='#ff0000'>" + averageEffectiveHPS.ToStringDot("0.0") + "</font>) Samples(");
            //        foreach (var hpsSample in hpsSamplesUsed)
            //        {
            //            pageBuilder.Append(PageUtility.CreateLink_FightOverview(hpsSample.CacheBossFight, hpsSample.CacheBossFight.StartDateTime.ToString("yyyy-MM-dd")) + ", ");
            //        }
            //        pageBuilder.Length = pageBuilder.Length - 2;
            //        pageBuilder.Append(")");
            //    }
            //    pageBuilder.Append("<br/>Deaths(<font color='#ff0000'>" + averageDeaths.ToStringDot("0%") + "</font>) Samples(");
            //    foreach (var deathSample in deathSamplesUsed)
            //    {
            //        pageBuilder.Append(PageUtility.CreateLink_FightOverview(deathSample.CacheBossFight, deathSample.CacheBossFight.StartDateTime.ToString("yyyy-MM-dd")) + ", ");
            //    }
            //    pageBuilder.Length = pageBuilder.Length - 2;
            //    pageBuilder.Append(")<br/>");
            //}

            m_PageHTML = new MvcHtmlString(pageBuilder.ToString());
        }
        private static VF_RealmPlayersDatabase.WowInstance GetRealmProgress(VF_RealmPlayersDatabase.WowRealm _Realm)
        {
            if (_Realm == VF_RealmPlayersDatabase.WowRealm.Warsong)
                return VF_RealmPlayersDatabase.WowInstance.Naxxramas;
            else if (_Realm == VF_RealmPlayersDatabase.WowRealm.Al_Akir || _Realm == VF_RealmPlayersDatabase.WowRealm.Emerald_Dream)
                return VF_RealmPlayersDatabase.WowInstance.Temple_Of_Ahn_Qiraj;
            else if (_Realm == VF_RealmPlayersDatabase.WowRealm.Nefarian || _Realm == VF_RealmPlayersDatabase.WowRealm.NostalGeek || _Realm == VF_RealmPlayersDatabase.WowRealm.Rebirth)
                return VF_RealmPlayersDatabase.WowInstance.Blackwing_Lair;
            
            return VF_RealmPlayersDatabase.WowInstance.Molten_Core;
        }
        private static string GenerateSummaryForInstance(VF_RDDatabase.PlayerSummary _PlayerSummary, PlayerData _Player, string _Instance, VF_RealmPlayersDatabase.WowRealm _Realm, string _GuildLimit = null)
        {
            string realmStr = RealmPlayersServer.StaticValues.ConvertRealmParam(_Realm);
            System.Text.StringBuilder page = new System.Text.StringBuilder("");

            int bossesData = 0;
            foreach (var boss in BossInformation.BossesInInstanceNoOptional[_Instance])
            {
                if (_PlayerSummary.PlayerBossStats.ContainsKey(boss) == false)
                    continue;

                if (bossesData == 0)
                {
                    page.Append("<table class='table'><thead><tr><th>Boss</th><th>DPS #Rank</th><th>HPS #Rank</th><th>Average DPS</th><th>Average HPS</th><th>Top DPS</th><th>Top HPS</th><th>Kill Count</th></thead><tbody>");
                }
                ++bossesData;

                List<VF_RDDatabase.PlayerFightData> dpsSamplesUsed = null;
                List<VF_RDDatabase.PlayerFightData> hpsSamplesUsed = null;

                var playerBossData = _PlayerSummary.PlayerBossStats[boss];
                if (playerBossData.m_PlayerFightDatas.Count < 3 || (_Instance != "Molten Core" && playerBossData.m_PlayerFightDatas.Count < 4))
                    return "";
                playerBossData.GetAverageDPS(5, 6, 3, out dpsSamplesUsed);
                playerBossData.GetAverageEffectiveHPS(5, 6, 3, out hpsSamplesUsed);

                float bwlBossDPS = 0.0f;
                float bwlBossHPS = 0.0f;
                var bwlBossDataSet = AverageOverview.GenerateAverageDataSet(boss, null, null, _Realm, _GuildLimit);
                var bwlBossDPSRank = GetDPSRank(bwlBossDataSet, _PlayerSummary.Name, out bwlBossDPS);
                var bwlBossDPSRankClass = GetDPSRank(bwlBossDataSet, _PlayerSummary.Name, out bwlBossDPS, (PlayerData _Character) => { return _Character.Character.Class == _Player.Character.Class; });
                var bwlBossHPSRank = GetHPSRank(bwlBossDataSet, _PlayerSummary.Name, out bwlBossHPS);
                var bwlBossHPSRankClass = GetHPSRank(bwlBossDataSet, _PlayerSummary.Name, out bwlBossHPS, (PlayerData _Character) => { return _Character.Character.Class == _Player.Character.Class; });
                //page += "<h5>" + boss + "</h5>";
                //page += "DPS Rank: #" + bwlBossDPSRank + " with DPS " + bwlBossDPS.ToStringDot("0.0");

                string dpsSamplesStr = "";

                if (dpsSamplesUsed.Count > 0)
                {
                    dpsSamplesStr = " (";

                    for (int i = 0; i < dpsSamplesUsed.Count; ++i)
                    {
                        dpsSamplesStr += PageUtility.CreateLink_FightOverview(dpsSamplesUsed[i].CacheBossFight, dpsSamplesUsed[i].DPS.ToStringDot("0.0")) + ", ";
                    }
                    dpsSamplesStr = dpsSamplesStr.Substring(0, dpsSamplesStr.Length - 2);
                    dpsSamplesStr += ")";
                }

                string hpsSamplesStr = "";
                if (hpsSamplesUsed.Count > 0)
                {
                    hpsSamplesStr = " (";

                    for (int i = 0; i < hpsSamplesUsed.Count; ++i)
                    {
                        hpsSamplesStr += PageUtility.CreateLink_FightOverview(hpsSamplesUsed[i].CacheBossFight, hpsSamplesUsed[i].EffectiveHPS.ToStringDot("0.0")) + ", ";
                    }
                    hpsSamplesStr = hpsSamplesStr.Substring(0, hpsSamplesStr.Length - 2);
                    hpsSamplesStr += ")";
                }

                page.Append("<tr>");
                page.Append("<td>" + boss + "</td>");
                if (bwlBossDPS != 0.0f || bwlBossHPS != 0.0f)
                {
                    page.Append("<td>" + PageUtility.CreateLink("AverageOverview.aspx?Boss=" + boss + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name, PageUtility.CreateColorString("#" + bwlBossDPSRank, RPColor.Red)) + " / ");
                    page.Append(PageUtility.CreateLink("AverageOverview.aspx?Boss=" + boss + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name + "&ClassLimit=" + ClassControl.GetClassLimitStr(_Player.Character.Class), PageUtility.CreateColorString("#" + bwlBossDPSRankClass, _Player.Character.Class)) + "</td>");
                    page.Append("<td>" + PageUtility.CreateLink("AverageOverview.aspx?Boss=" + boss + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name, PageUtility.CreateColorString("#" + bwlBossHPSRank, RPColor.Green)) + " / ");
                    page.Append(PageUtility.CreateLink("AverageOverview.aspx?Boss=" + boss + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name + "&ClassLimit=" + ClassControl.GetClassLimitStr(_Player.Character.Class), PageUtility.CreateColorString("#" + bwlBossHPSRankClass, _Player.Character.Class)) + "</td>");
                }
                else
                {
                    page.Append("<td>#??(inactive)</td>");
                    page.Append("<td>#??(inactive)</td>");
                }
                page.Append("<td>" + PageUtility.CreateLink("AverageOverview.aspx?Boss=" + boss + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name, PageUtility.CreateColorString(bwlBossDPS.ToStringDot("0.0"), RPColor.Red))
                    + dpsSamplesStr  
                    + "</td>");
                page.Append("<td>" + PageUtility.CreateLink("AverageOverview.aspx?Boss=" + boss + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name, PageUtility.CreateColorString(bwlBossHPS.ToStringDot("0.0"), RPColor.Green))
                    + hpsSamplesStr  
                    + "</td>");
                page.Append("<td>" + PageUtility.CreateLink("FightOverallOverview.aspx?FightName=" + boss + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name, "view") + "</td>");
                page.Append("<td>" + PageUtility.CreateLink("FightOverallOverview.aspx?FightName=" + boss + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name, "view") + "</td>");
                page.Append("<td>" + _PlayerSummary.AttendedFights.Count((_Value) => _Value.BossName == boss) + "</td>");
                page.Append("</tr>");
            }
            if (bossesData == 0)
            {
                return "";
            }
            else if (bossesData > 0)
            {
                page.Append("</tbody></table>");
            }

            string averagePageData = "<p><h3>Active Ranking in " + _Instance + "</h3>"; 
            { 
                var bwlDataSet = AverageOverview.GenerateAverageDataSet(_Instance, null, null, _Realm, _GuildLimit);

                float bwlDPS = 0.0f;
                string bwlDPSRank = GetDPSRank(bwlDataSet, _PlayerSummary.Name, out bwlDPS).ToString();
                string bwlDPSRankClass = GetDPSRank(bwlDataSet, _PlayerSummary.Name, out bwlDPS, (PlayerData _Character) => { return _Character.Character.Class == _Player.Character.Class; }).ToString();


                float bwlHPS = 0.0f;
                string bwlHPSRank = GetHPSRank(bwlDataSet, _PlayerSummary.Name, out bwlHPS).ToString();
                string bwlHPSRankClass = GetHPSRank(bwlDataSet, _PlayerSummary.Name, out bwlHPS, (PlayerData _Character) => { return _Character.Character.Class == _Player.Character.Class; }).ToString();

                if (bwlDPS != 0.0f || bwlHPS != 0.0f)
                {
                    string bossIdentifierStr = BossesControl.MC_ALL;
                    if (_Instance == "Molten Core")
                    {
                        bossIdentifierStr = BossesControl.MC_ALL;
                    }
                    else if (_Instance == "Blackwing Lair")
                    {
                        bossIdentifierStr = BossesControl.BWL_ALL;
                    }
                    else if (_Instance == "Ahn'Qiraj Temple")
                    {
                        bossIdentifierStr = BossesControl.AQ40_ALL;
                    }
                    else if (_Instance == "Naxxramas - All Quarters")
                    {
                        bossIdentifierStr = BossesControl.NAXX_ALL_QUARTERS;
                    }

                    bwlDPSRank = PageUtility.CreateLink("Ranking.aspx?Bosses=" + bossIdentifierStr + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name, PageUtility.CreateColorString("#" + bwlDPSRank, RPColor.Red));
                    bwlHPSRank = PageUtility.CreateLink("Ranking.aspx?Bosses=" + bossIdentifierStr + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name, PageUtility.CreateColorString("#" + bwlHPSRank, RPColor.Green));

                    bwlDPSRankClass = PageUtility.CreateLink("Ranking.aspx?Bosses=" + bossIdentifierStr + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name + "&ClassLimit=" + ClassControl.GetClassLimitStr(_Player.Character.Class)
                        , PageUtility.CreateColorString("#" + bwlDPSRankClass, _Player.Character.Class));
                    bwlHPSRankClass = PageUtility.CreateLink("Ranking.aspx?Bosses=" + bossIdentifierStr + "&realm=" + realmStr + "&andplayer=" + _PlayerSummary.Name + "&ClassLimit=" + ClassControl.GetClassLimitStr(_Player.Character.Class)
                        , PageUtility.CreateColorString("#" + bwlHPSRankClass, _Player.Character.Class));
                    

                    averagePageData += "<h5>DPS Rank: " + bwlDPSRank + " / " + bwlDPSRankClass + " with DPS " + bwlDPS.ToStringDot("0.0") + "</h5>";
                    averagePageData += "<h5>HPS Rank: " + bwlHPSRank + " / " + bwlHPSRankClass + " with HPS " + bwlHPS.ToStringDot("0.0") + "</h5>";
                }
                else
                {
                    averagePageData += "<h5>DPS Rank: #??(inactive)</h5>";
                    averagePageData += "<h5>HPS Rank: #??(inactive)</h5>";
                }
            }

            return averagePageData + page.ToString();
        }
    }
}