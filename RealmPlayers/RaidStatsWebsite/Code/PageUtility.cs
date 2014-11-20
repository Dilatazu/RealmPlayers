using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using PlayerData = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;

namespace VF_RaidDamageWebsite
{
    public enum RPColor
    {
        Green = 0x00FF00,
        Yellow = 0xFFFF00,
        Red = 0xFF0000,
        White = 0xFFFFFF,
        Gray = 0xCCCCCC,
        DarkGray = 0x777777,
        HordeText = 0xF70002,
        HordeBG = 0x463232,
        AllianceText = 0x007DF7,
        AllianceBG = 0x272F37,
    }

    public class PageUtility : RealmPlayersServer.PageUtility
    {
        public static string BreadCrumb_AddRaidList()
        {
            return "<li><span class='divider'>/</span> <a href='RaidList.aspx'>Raids</a></li>";
        }
        public static string BreadCrumb_AddGuildRaidList(string _Guild)
        {
            return "<li><span class='divider'>/</span>" + CreateLink_GuildRaidList(_Guild) + "</li>";
        }
        public static string BreadCrumb_AddAverageOverview_Boss(string _BossName, WowRealm _Realm, string _GuildLimit)
        {
            return BreadCrumb_AddLink("AverageOverview.aspx?Boss=" + _BossName + "&Realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(_Realm) + "&Guild=" + _GuildLimit, "Average Performance vs " + _BossName);
        }
        public static string BreadCrumb_AddAverageOverview_Instance(string _InstanceName, WowRealm _Realm, string _GuildLimit)
        {
            return BreadCrumb_AddLink("AverageOverview.aspx?Instance=" + _InstanceName + "&Realm=" + RealmPlayersServer.StaticValues.ConvertRealmParam(_Realm) + "&Guild=" + _GuildLimit, "Average Performance vs " + _InstanceName);
        }
        public static string BreadCrumb_AddThisPageWithout(string _Name, System.Web.HttpRequest _RequestObject, string _SkipParam)
        {
            return BreadCrumb_AddLink(PageUtility.CreateUrlWithNewQueryValue(_RequestObject, _SkipParam, null), _Name);
        }
        public static string BreadCrumb_AddRaidOverview(VF_RaidDamageDatabase.RaidCollection.Raid _Raid)
        {
            return "<li><span class='divider'>/</span> <a href='RaidOverview.aspx?Raid=" + _Raid.UniqueRaidID + "'>"
                + _Raid.RaidInstance + "(" + _Raid.RaidID.ToString() + ")</a></li>";
        }
        public static string CreateLink_FightOverview(VF_RDDatabase.BossFight _BossFight, string _Content)
        {
            return PageUtility.CreateLink("FightOverview.aspx?Raid="
                + _BossFight.CacheRaid.UniqueRaidID
                + "&Fight=" + (_BossFight.StartDateTime.ToString("ddHHmmss")), _Content);
        }
        public static string CreateLink_RaidOverview(VF_RDDatabase.Raid _Raid, string _Content)
        {
            return PageUtility.CreateLink(HOSTURL_RaidStats + "RaidOverview.aspx?Raid=" + _Raid.UniqueRaidID, _Content);
        }
        public static string CreateColorisedPercentage(double _Percentage)
        {
            if (_Percentage >= 0.95)
                return "<font color='#00FF00'>" + _Percentage.ToStringDot("0%") + "</font>";
            else if (_Percentage >= 0.90)
                return "<font color='#88FF00'>" + _Percentage.ToStringDot("0%") + "</font>";
            else if (_Percentage >= 0.80)
                return "<font color='#FFFF00'>" + _Percentage.ToStringDot("0%") + "</font>";
            else if (_Percentage >= 0.70)
                return "<font color='#FF8800'>" + _Percentage.ToStringDot("0%") + "</font>";
            else
                return "<font color='#FF0000'>" + _Percentage.ToStringDot("0%") + "</font>";
        }
        public static string CreateColorisedFactor(double _Factor)
        {
            if (_Factor >= 0.95)
                return "<font color='#00FF00'>" + _Factor.ToStringDot("0.00") + "</font>";
            else if (_Factor >= 0.90)
                return "<font color='#88FF00'>" + _Factor.ToStringDot("0.00") + "</font>";
            else if (_Factor >= 0.80)
                return "<font color='#FFFF00'>" + _Factor.ToStringDot("0.00") + "</font>";
            else if (_Factor >= 0.70)
                return "<font color='#FF8800'>" + _Factor.ToStringDot("0.00") + "</font>";
            else
                return "<font color='#FF0000'>" + _Factor.ToStringDot("0.00") + "</font>";
        }
        public static string CreateColorString(string _String, System.Drawing.Color _Color)
        {
            return "<font color='#" + (_Color.ToArgb() & 0xFFFFFF).ToString("X6") + "'>" + _String + "</font>";
        }
        public static string CreateColorString(string _String, RPColor _Color)
        {
            return "<font color='#" + ((int)_Color).ToString("X6") + "'>" + _String + "</font>";
        }
        public static RPColor GetFactionTextColor(PlayerFaction _Faction)
        {
            if (_Faction == PlayerFaction.Horde) return RPColor.HordeText;
            else if (_Faction == PlayerFaction.Alliance) return RPColor.AllianceText;
            return RPColor.Gray;
        }
        public static RPColor GetFactionBGColor(PlayerFaction _Faction)
        {
            if (_Faction == PlayerFaction.Horde) return RPColor.HordeBG;
            else if (_Faction == PlayerFaction.Alliance) return RPColor.AllianceBG;
            return RPColor.DarkGray;
        }

        public class StatsBarStyle
        {
            public string m_BarTextColor = "#000";
            public string m_TitleText = "";
            public string m_LeftSideTitleText = "";
            public string m_RightSideTitleText = "";
            public int m_BeforeBarWidth = 70;
            public int m_MaxWidth = 200;
            public int m_AfterBarWidth = 70;
        }
        public class StatsBarData
        {
            public string m_BeforeBarText = "";
            public string m_OnBarLeftText = "";
            public string m_OnBarRightText = "";
            public string m_AfterBarText = "";
            public string m_BarColor = "#ccc";
            public double m_PercentageWidth = 1.0;
            public double m_OnBarTextWidth = -1.0;
        }
        public static string CreateStatsBars_Raphael(StatsBarStyle _StatsBarStyle, List<StatsBarData> _BarsData)
        {
            string graphSection = "<script>$(function () {if (g_RaphaelBarsDrawer == null) {g_RaphaelBarsDrawer = Raphael('diagramDiv', " + _StatsBarStyle.m_MaxWidth + ", " + (_BarsData.Count * 20) + ");}";

            foreach (var bar in _BarsData)
            {
                graphSection += "VF_CreateDmgBar(" + bar.m_PercentageWidth.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + ", '" + bar.m_BarColor + "', '" + bar.m_OnBarLeftText + "','" + _StatsBarStyle.m_BarTextColor + "', '" + bar.m_OnBarRightText + "');";
            }

            graphSection += "});";
            graphSection += "</script>";
            graphSection += "<div id='diagramDiv'></div>";
            return graphSection;
        }
        public static string CreateStatsBars_HTML_CSSCode()
        {
            return ".VF_StatsBars { "
                   + "    overflow: hidden;"
                   + "    background-color: #000;"
                   + "    color: #fff; "
                   + "    font: 12px Arial; "
                   + "    display: inline-block;"
                   + "    white-space: nowrap;"
                   + "    border:2px solid;"
                   + "    border-color:#000;"
                   + "}"
                   + ".VF_StatsBars .TopBar {"
                   + "    position: relative; "
                   + "    height: 19px; "
                   + "}"
                   + ".VF_StatsBars .StatsBar1, .VF_StatsBars .StatsBar2 { position: relative; height: 19px; }"
                   + ".VF_StatsBars .StatsBar1 { background-color: #333; }"
                   + ".VF_StatsBars .StatsBar2 { background-color: #222; }"

                   + ".VF_StatsBars .ClassBar, .VF_StatsBars .TopBarTitle { position: relative; height: 15px; top: 1px;"
                   + "    border:1px solid;border-color:#000;}"
                   + ".VF_StatsBars .TopBarTitle { text-align:center; overflow: hidden; }"

                   + ".VF_StatsBars .BeforeBar, .VF_StatsBars .AfterBar { position: absolute; top: 2px; }"
                   + ".VF_StatsBars .BeforeBar { text-align: right; } "
                   + ".VF_StatsBars .AfterBar { text-align: left; }"

                   + ".VF_StatsBars .OnBarLeft, .VF_StatsBars .OnBarRight { position: relative; height: 15px; }"
                   + ".VF_StatsBars .OnBarLeft { left: 2px; z-index: 5; } "
                   + ".VF_StatsBars .OnBarRight { left: -2px; top: -15px; text-align: right; z-index: 3;}";
        }
        private static int g_StatsBarCounter = 0;
        public static string CreateStatsBars_HTML(StatsBarStyle _StatsBarStyle, List<StatsBarData> _BarsData, int _ToggleViewMoreMaxCount, int _ToggleGroupID = -1)
        {
            //int beforeBarMaxSize = 0;
            //int afterBarMaxSize = 0;
            //foreach (var bar in _BarsData)
            //{
            //    if (bar.m_BeforeBarText != "")
            //    {
            //        if (beforeBarMaxSize < bar.m_BeforeBarText.Length)
            //            beforeBarMaxSize = bar.m_BeforeBarText.Length;
            //    }
            //    if (bar.m_AfterBarText != "")
            //    {
            //        if (afterBarMaxSize < bar.m_AfterBarText.Length)
            //            afterBarMaxSize = bar.m_AfterBarText.Length;
            //    }
            //}

            //int s_BeforeBarWidth = (beforeBarMaxSize * 5);
            //int s_AfterBarWidth = (afterBarMaxSize * 5);
            //if (maxWidth < s_BeforeBarWidth + s_AfterBarWidth + 10)
            //    maxWidth = s_BeforeBarWidth + s_AfterBarWidth + 10;

            int maxBarWidth = _StatsBarStyle.m_MaxWidth - (_StatsBarStyle.m_BeforeBarWidth + _StatsBarStyle.m_AfterBarWidth + 10);
            int barLeft = (_StatsBarStyle.m_BeforeBarWidth + 5);

            if (_StatsBarStyle.m_BeforeBarWidth == 0)
            {
                maxBarWidth += 5;
                barLeft = 0;
            }
            if (_StatsBarStyle.m_AfterBarWidth == 0) maxBarWidth += 5;

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(1000);

            if (g_StatsBarCounter >= 100)
                g_StatsBarCounter -= 100;
            ++g_StatsBarCounter;
            string statsBarID = "StatsBars_" + g_StatsBarCounter;

            stringBuilder.Append("<div class='VF_StatsBars' id='" + statsBarID + "'>");
            stringBuilder.Append("<style>");
            stringBuilder.Append(".VF_StatsBars .StatsBar1, .VF_StatsBars .StatsBar2 {width: " + _StatsBarStyle.m_MaxWidth + "px;}");
            stringBuilder.Append(".VF_StatsBars .BeforeBar {width: " + _StatsBarStyle.m_BeforeBarWidth + "px;}");
            stringBuilder.Append(".VF_StatsBars .ClassBar {position: absolute; left: " + barLeft + "px; color:" + _StatsBarStyle.m_BarTextColor + ";}");
            stringBuilder.Append(".VF_StatsBars .AfterBar {width: " + _StatsBarStyle.m_AfterBarWidth + "px;left: " + (_StatsBarStyle.m_MaxWidth - _StatsBarStyle.m_AfterBarWidth) + "px;}");
            stringBuilder.Append(".VF_StatsBars .ClassBar a { text-decoration: none; color:" + _StatsBarStyle.m_BarTextColor + ";}");
            stringBuilder.Append("</style>");
            stringBuilder.Append("<script>");
            stringBuilder.Append("var g_Expanded_" + statsBarID + " = false;");
            stringBuilder.Append("function Expand_" + statsBarID + "() {");
            stringBuilder.Append("if(g_Expanded_" + statsBarID + " == true) {");
            stringBuilder.Append("$('#" + statsBarID + "_DynamicShow').slideUp();");
            stringBuilder.Append("$('#" + statsBarID + "_ButtonShow').text('Show More');");
            stringBuilder.Append("} else {");
            stringBuilder.Append("$('#" + statsBarID + "_DynamicShow').slideDown();");
            stringBuilder.Append("$('#" + statsBarID + "_ButtonShow').text('Show Top " + _ToggleViewMoreMaxCount + "');");
            stringBuilder.Append("} g_Expanded_" + statsBarID + " = !g_Expanded_" + statsBarID + ";");
            stringBuilder.Append("}");
            if (_ToggleGroupID != -1)
            {
                stringBuilder.Append("var g_ExpandGroupFuncs_" + _ToggleGroupID + " = g_ExpandGroupFuncs_" + _ToggleGroupID + " || [];");
                stringBuilder.Append("g_ExpandGroupFuncs_" + _ToggleGroupID + ".push(Expand_" + statsBarID + ");");
                stringBuilder.Append("function Expand_Group_" + _ToggleGroupID + "() {");
                stringBuilder.Append("for(i = 0; i < g_ExpandGroupFuncs_" + _ToggleGroupID + ".length; ++i) {");
                stringBuilder.Append("g_ExpandGroupFuncs_" + _ToggleGroupID + "[i]();");
                stringBuilder.Append("}");
                stringBuilder.Append("}");
            }
            stringBuilder.Append("</script>");


            stringBuilder.Append("<div class='TopBar'>");
            {
                stringBuilder.Append("<div class='BeforeBar' style='text-align:center;'>" + _StatsBarStyle.m_LeftSideTitleText + "</div>");
                stringBuilder.Append("<div class='TopBarTitle'>" + _StatsBarStyle.m_TitleText + "</div>");
                stringBuilder.Append("<div class='AfterBar' style='text-align:center;'>" + _StatsBarStyle.m_RightSideTitleText + "</div>");
            }
            stringBuilder.Append("</div>");
            for (int i = 0; i < _BarsData.Count; ++i)
            {
                var bar = _BarsData[i];
                double barWidth = bar.m_PercentageWidth * maxBarWidth;

                if (i == _ToggleViewMoreMaxCount)
                {
                    stringBuilder.Append("<div id='" + statsBarID + "_DynamicShow' style='display: none;'>");
                }
                if (i % 2 == 0)
                    stringBuilder.Append("<div class='StatsBar2'>");
                else
                    stringBuilder.Append("<div class='StatsBar1'>");

                if (bar.m_BeforeBarText != "")
                    stringBuilder.Append("<div class='BeforeBar'>" + bar.m_BeforeBarText + "</div>");

                if (bar.m_OnBarTextWidth < 0)
                    bar.m_OnBarTextWidth = (bar.m_OnBarLeftText.Length + bar.m_OnBarRightText.Length) * 1.4;

                if (bar.m_OnBarTextWidth > barWidth * 1.25)// || (barWidth < 100 && bar.m_OnBarLeftText != "")
                {
                    stringBuilder.Append("<div class='ClassBar' style='border-color: rgba(0, 0, 0, 1); background-color: rgba(100, 100, 100, 1);'>");
                    {
                        stringBuilder.Append("<div class='OnBarLeft'>" + bar.m_OnBarLeftText + " " + bar.m_OnBarRightText + " &nbsp;</div>");
                        //if (bar.m_OnBarRightText != "")
                        //    graphSection += "<div class='OnBarRight'>" + bar.m_OnBarRightText + "</div>";
                    }
                    stringBuilder.Append("</div>");
                    stringBuilder.Append("<div class='ClassBar' style='background-color: " + bar.m_BarColor + ";width: " + barWidth.ToStringDot() + "px;'>");
                    stringBuilder.Append("</div>");
                }
                else
                {
                    stringBuilder.Append("<div class='ClassBar' style='background-color: " + bar.m_BarColor + ";width: " + barWidth.ToStringDot() + "px;'>");
                    {
                        stringBuilder.Append("<div class='OnBarLeft'>" + bar.m_OnBarLeftText + "</div>");
                        if (bar.m_OnBarRightText != "")
                            stringBuilder.Append("<div class='OnBarRight'>" + bar.m_OnBarRightText + "</div>");
                    }
                    stringBuilder.Append("</div>");
                }

                if (bar.m_AfterBarText != "")
                    stringBuilder.Append("<div class='AfterBar'>" + bar.m_AfterBarText + "</div>");

                stringBuilder.Append("</div>");//StatsBar
            }

            if (_BarsData.Count > _ToggleViewMoreMaxCount)
            {
                stringBuilder.Append("</div>"); //StatsBars_" + g_StatsBarCounter + "_After25
                stringBuilder.Append("<div class='TopBar'>");
                stringBuilder.Append("<div class='TopBarTitle'><a href='javascript:void(0)' onclick='" + (_ToggleGroupID != -1 ? ("Expand_Group_" + _ToggleGroupID + "();") : ("Expand_" + statsBarID + "();")) + "' id='" + statsBarID + "_ButtonShow'>Show More</a></div>");
                stringBuilder.Append("</div>");
            }

            stringBuilder.Append("</div>");
            return stringBuilder.ToString();
        }

        public static string CreateGraph(List<int> _DataX, List<int> _DataY, System.Drawing.Color _DataYColor, List<string> _Labels)
        {
            return CreateGraph(_DataX, 
                new List<Tuple<List<int>, System.Drawing.Color>>
                {
                    new Tuple<List<int>, System.Drawing.Color>(_DataY, _DataYColor)
                }, 
                _Labels);
        }
        public static string CreateGraph(List<int> _DataX, List<int> _Data1Y, System.Drawing.Color _Data1YColor, List<int> _Data2Y, System.Drawing.Color _Data2YColor, List<string> _Labels)
        {
            return CreateGraph(_DataX,
                new List<Tuple<List<int>, System.Drawing.Color>>
                {
                    new Tuple<List<int>, System.Drawing.Color>(_Data1Y, _Data1YColor),
                    new Tuple<List<int>, System.Drawing.Color>(_Data2Y, _Data2YColor)
                },
                _Labels);
        }
        public static string CreateGraph(List<int> _DataX, List<Tuple<List<int>, System.Drawing.Color>> _DataYs, List<string> _Labels)
        {
            string dataX = "";
            string dataYs = "";
            string dataYcolors = "";
            string labels = "";

            for (int i = 0; i < _DataYs.Count; ++i)
            {
                if (_DataX.Count != _DataYs[i].Item1.Count || _DataYs[i].Item1.Count != _Labels.Count)
                    return "Error, DataX did not contain as many members as DataY or Labels";
            }

            for (int i = 0; i < _DataX.Count; ++i)
            {
                labels += "'" + _Labels[i].Replace("\'", "\\'") + "',";
                dataX += _DataX[i] + ",";
            }
            for (int i = 0; i < _DataYs.Count; ++i)
            {
                dataYs += "[";
                for (int u = 0; u < _DataX.Count; ++u)
                {
                    dataYs += _DataYs[i].Item1[u] + ",";
                }
                dataYs += "],";

                int color = _DataYs[i].Item2.ToArgb() & 0xFFFFFF;
                dataYcolors += "'#" + color.ToString("X6") + "',";
            }

            string graphSection = "";
            graphSection += "<br /><div id='graphDiv'></div>";
            graphSection += "<div id='graphText'>teest</div>";
            graphSection += "<script>g_GraphLabels = [" + labels + "]; ";
            graphSection += "VF_InitializeGraph("
                + "[" + dataX + "]"
                + ", [" + dataYs + "]"
                + ", [" + dataYcolors + "]"
                + ", 935, 250, 0, 0, 'graphDiv'"
                + ", function (i) { $('#graphText')[0].innerHTML = g_GraphLabels[i]; }";
            graphSection += ");";

            graphSection += "</script>";

            return graphSection;
        }
    }
}