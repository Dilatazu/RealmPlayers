using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using FightDataCollection = VF_RaidDamageDatabase.FightDataCollection;
namespace VF_RaidDamageWebsite
{
    public partial class ViewFights : System.Web.UI.Page
    {
        public void CalculateChartData(VF_RaidDamageDatabase.FightData _Fight
            , VF_RaidDamageDatabase.RealmDB _RealmDB
            , FightDataCollection _FightDataCollection
            , Func<VF_RaidDamageDatabase.UnitData, double> _GetValueLambda
            , ref int _PlayerCount
            , out List<int> _RetChartSectionData, out List<int> _RetChartSectionLabels)
        {
            _RetChartSectionData = new List<int>();
            _RetChartSectionLabels = new List<int>();
            double lastTotalValue = 0;
            int firstTimeSliceIndex = _Fight.TimeSlices.FindIndex((_TimeSlice) => { return _TimeSlice.Event.StartsWith("Start"); });
            int timeStart = _Fight.TimeSlices[firstTimeSliceIndex].Time;
            int timeEnd = timeStart + _Fight.FightDuration;
            int lastTimeSliceTime = timeStart;
            double totalValue = 0;
            foreach (var timeSlice in _Fight.TimeSlices)
            {
                if (timeSlice.Time < timeStart || timeSlice.Time > timeEnd)
                    continue;
                int timeSlicePlayers = 0;
                foreach (var unit in timeSlice.UnitDatas)
                {
                    var playerData = _RealmDB.RD_FindPlayer(_FightDataCollection.GetNameFromUnitID(unit.Key), _FightDataCollection.m_RaidMembers);
                    double currValue = _GetValueLambda(unit.Value);
                    if (playerData != null && currValue > 0)
                    {
                        totalValue += currValue;
                        timeSlicePlayers++;
                    }
                }
                if (_Fight.TimeSlices.IndexOf(timeSlice) == firstTimeSliceIndex)
                {
                    lastTotalValue = totalValue;
                    totalValue = 0;
                    continue;
                }
                else if (timeSlice.Time == lastTimeSliceTime)
                {
                    totalValue = 0;
                    continue;
                }
                if (timeSlicePlayers > _PlayerCount)
                    _PlayerCount = timeSlicePlayers;
                double deltaTotal = totalValue - lastTotalValue;
                lastTotalValue = totalValue;
                totalValue = 0;
                _RetChartSectionData.Add((int)(deltaTotal / (timeSlice.Time - lastTimeSliceTime)));
                _RetChartSectionLabels.Add(timeSlice.Time - timeStart);
                lastTimeSliceTime = timeSlice.Time;
            }
        }
        public void CalculateChartData(VF_RaidDamageDatabase.FightData _Fight
            , FightDataCollection _FightDataCollection
            , VF_RealmPlayersDatabase.PlayerData.Player _Player
            , Func<VF_RaidDamageDatabase.UnitData, double> _GetValueLambda
            , out List<int> _RetChartSectionData, out List<int> _RetChartSectionLabels)
        {
            _RetChartSectionData = new List<int>();
            _RetChartSectionLabels = new List<int>();
            int unitID = -1;
            try
            {
                unitID = _FightDataCollection.GetUnitIDFromName(_Player.Name);
            }
            catch (Exception)
            {
                return;
            }
            int firstTimeSliceIndex = _Fight.TimeSlices.FindIndex((_TimeSlice) => { return _TimeSlice.Event.StartsWith("Start"); });
            int timeStart = _Fight.TimeSlices[firstTimeSliceIndex].Time;
            int timeEnd = timeStart + _Fight.FightDuration;
            int lastTimeSliceTime = timeStart;
            foreach (var timeSlice in _Fight.TimeSlices)
            {
                if (timeSlice.Time < timeStart || timeSlice.Time > timeEnd)
                    continue;
                if (timeSlice.UnitDatas.ContainsKey(unitID) == false)
                    continue;
                double currValue = _GetValueLambda(timeSlice.UnitDatas[unitID]);
                if (timeSlice.Time == lastTimeSliceTime)
                    continue;

                _RetChartSectionData.Add((int)currValue);
                _RetChartSectionLabels.Add(timeSlice.Time - timeStart);
                lastTimeSliceTime = timeSlice.Time;
            }
        }

        public MvcHtmlString m_ChartSection = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("/RaidList.aspx");
            return;
            /*var fightFile = RealmPlayersServer.PageUtility.GetQueryString(Request, "FightCollection");
            var typeStr = RealmPlayersServer.PageUtility.GetQueryString(Request, "Type", "Dmg");
            var compareToStr = RealmPlayersServer.PageUtility.GetQueryString(Request, "CompareTo", "null");
            var playerStr = RealmPlayersServer.PageUtility.GetQueryString(Request, "Player", "null");
            var playerCompareStr = RealmPlayersServer.PageUtility.GetQueryString(Request, "PlayerCompare", "null");
            var fightCollections = ApplicationInstance.Instance.GetFightsFileList();
            if (fightCollections.Contains(fightFile))
            {
                var fightDataCollection = ApplicationInstance.Instance.GetFightDataCollection(fightFile);
                FightDataCollection compareToFightData = null;
                if (fightCollections.Contains(compareToStr))
                    compareToFightData = ApplicationInstance.Instance.GetFightDataCollection(compareToStr);
                VF_RaidDamageDatabase.RealmDB realmDB = null;

                string graphSection = "";

                graphSection += "Raid: <select style='width: 360px; margin-top: 10px; ' onchange='navigateWithNewQuery(\"FightCollection\", this.options[this.selectedIndex].value)'>";
                graphSection += "<option value='null' " + ((fightFile == null) ? "selected='selected'" : "") + ">None</option>";
                foreach (string fight in fightCollections)
                {
                    graphSection += "<option value='" + fight + "' " + ((fightFile == fight) ? "selected='selected'" : "") + ">" + fight + "</option>";
                }
                graphSection += "</select>";

                graphSection += " View: <select style='width: 140px; margin-top: 10px; ' onchange='navigateWithNewQuery(\"type\", this.options[this.selectedIndex].value)'>"
                    + "<option value='Dmg' " + ((typeStr == "Dmg") ? "selected='selected'" : "") + ">Dmg</option>"
                    + "<option value='DmgTaken' " + ((typeStr == "DmgTaken") ? "selected='selected'" : "") + ">DmgTaken</option>"
                    + "<option value='Heal' " + ((typeStr == "Heal") ? "selected='selected'" : "") + ">Effective Heal</option>"
                    + "<option value='RaidDPS' " + ((typeStr == "RaidDPS") ? "selected='selected'" : "") + ">Raid DPS</option>"
                    + "<option value='RaidHPS' " + ((typeStr == "RaidHPS") ? "selected='selected'" : "") + ">Raid HPS</option>"
                    + "<option value='DPS' " + ((typeStr == "DPS") ? "selected='selected'" : "") + ">DPS</option>"
                    + "<option value='HPS' " + ((typeStr == "HPS") ? "selected='selected'" : "") + ">HPS</option>"
                    + "<option value='Threat' " + ((typeStr == "Threat") ? "selected='selected'" : "") + ">Threat</option>"
                    + "<option value='Death' " + ((typeStr == "Death") ? "selected='selected'" : "") + ">Death</option>"
                    + "<option value='Overheal' " + ((typeStr == "Overheal") ? "selected='selected'" : "") + ">Overheal</option>"
                    + "<option value='RawHeal' " + ((typeStr == "RawHeal") ? "selected='selected'" : "") + ">Raw Heal</option>"
                + "</select>";

                if (typeStr == "RaidDPS" || typeStr == "RaidHPS")
                {
                    graphSection += " Compare To: <select style='width: 360px; margin-top: 10px; ' onchange='navigateWithNewQuery(\"compareto\", this.options[this.selectedIndex].value)'>";
                    graphSection += "<option value='null' " + ((compareToFightData == null) ? "selected='selected'" : "") + ">None</option>";
                    foreach (string fight in fightCollections)
                    {
                        graphSection += "<option value='" + fight + "' " + ((compareToStr == fight) ? "selected='selected'" : "") + ">" + fight + "</option>";
                    }
                    graphSection += "</select>";
                }
                else if (typeStr == "DPS" || typeStr == "HPS")
                {
                    graphSection += " Compare To: <select style='width: 360px; margin-top: 10px; ' onchange='navigateWithNewQuery(\"playercompare\", this.options[this.selectedIndex].value)'>";
                    graphSection += "<option value='null' " + ((playerCompareStr == "null") ? "selected='selected'" : "") + ">None</option>";
                    var unitsDataList = fightDataCollection.Fights.Last().GetUnitsDataList(true);
                    foreach (var unitsData in unitsDataList)
                    {
                        if(realmDB.RD_FindPlayer(unitsData.Item1) != null)
                            graphSection += "<option value='" + unitsData.Item1 + "' " + ((playerCompareStr == unitsData.Item1) ? "selected='selected'" : "") + ">" + unitsData.Item1 + "</option>";
                    }
                    graphSection += "</select>";
                }

                Func<VF_RaidDamageDatabase.UnitData, double> getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.Dmg; };
                if (typeStr == "Dmg")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.Dmg; };
                else if (typeStr == "DmgTaken")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.DmgTaken; };
                else if (typeStr == "Heal")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.EffHeal; };
                else if (typeStr == "DPS")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { if(_UnitData.Dmg == 0) return 0.0; return (double)_UnitData.DPS; };
                else if (typeStr == "HPS")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { if (_UnitData.RawHeal == 0) return 0.0; return (double)_UnitData.HPS; };
                else if (typeStr == "Threat")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.ThreatValue; };
                else if (typeStr == "Death")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.Death; };
                else if (typeStr == "Overheal")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.OverHeal; };
                else if (typeStr == "RawHeal")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.RawHeal; };
                else if (typeStr == "RaidDPS")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.Dmg; };
                else if (typeStr == "RaidHPS")
                    getValueLambda = (VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.EffHeal; };
                else
                    return;

                VF_RealmPlayersDatabase.PlayerData.Player graphPlayer = null;
                VF_RealmPlayersDatabase.PlayerData.Player graphPlayerCompare = null;
                if ((typeStr == "DPS" || typeStr == "HPS") && playerStr != "null")
                {
                    graphPlayer = realmDB.RD_FindPlayer(playerStr);

                    if (playerCompareStr != "null")
                    {
                        graphPlayerCompare = realmDB.RD_FindPlayer(playerCompareStr);
                    }
                }
                if (typeStr == "RaidDPS" || typeStr == "RaidHPS" || ((typeStr == "DPS" || typeStr == "HPS") && graphPlayer != null))
                {
                    int graphIndex = 0;

                    if (graphPlayer != null)
                    {
                        graphSection += "<h1>" + typeStr + " data for player: "
                            + RealmPlayersServer.PageUtility.CreateColorCodedName(graphPlayer.Name, graphPlayer.Character.Class)
                            + (graphPlayerCompare == null ? "" : ("(blue) vs " + RealmPlayersServer.PageUtility.CreateColorCodedName(graphPlayerCompare.Name, graphPlayerCompare.Character.Class) + "(green)"))
                            + "<br />(from fights: <a href='javascript:void(0)' onclick='navigateWithNewQuery(\"player\",\"null\");'>" + fightFile + "</a>)</h1>";
                    }
                    else
                    {
                        graphSection += "<h1>" + typeStr + " data for fights: " + fightFile
                            + (compareToFightData == null ? "" : ("(blue) vs " + compareToStr + "(green)"))
                            + "</h1>";
                    }

                    foreach (var fight in fightDataCollection.Fights)
                    {
                        int players = 0;
                        //var unitsData = fight.GetUnitsDataList();
                        //foreach (var unit in unitsData)
                        //{
                        //    var playerData = realmDB.RD_FindPlayer(unit.Item1);
                        //    if (playerData != null)
                        //        players++;
                        //}

                        List<int> chartSectionData = null;
                        List<int> chartSectionLabels = null;
                        List<int> chartSectionSecondData = null;

                        if (graphPlayer != null)
                        {
                            CalculateChartData(fight.m_Fight, fightDataCollection, graphPlayer, getValueLambda, out chartSectionData, out chartSectionLabels);
                            players = 99;
                        }
                        else
                        {
                            CalculateChartData(fight.m_Fight, realmDB, fightDataCollection, getValueLambda, ref players, out chartSectionData, out chartSectionLabels);
                        }

                        VF_RaidDamageDatabase.FightDataCollection.FightCacheData compareToFight = null;
                        if(graphPlayerCompare != null || compareToFightData != null)
                        {
                            List<int> chartSectionSecondDataTemp = null;
                            List<int> chartSectionSecondLabels = null;
                            int secondPlayers = 0;

                            if (graphPlayerCompare != null)
                            {
                                CalculateChartData(fight.m_Fight, fightDataCollection, graphPlayerCompare, getValueLambda, out chartSectionSecondDataTemp, out chartSectionSecondLabels);
                                secondPlayers = 99;
                            }
                            else if (compareToFightData != null)
                            {
                                for (int i = compareToFightData.Fights.Count - 1; i >= 0; --i)
                                {//Backwards because we only want to compare to "kill" fights
                                    compareToFight = compareToFightData.Fights[i];
                                    if (compareToFight.m_Fight.FightName == fight.m_Fight.FightName)
                                    {
                                        if (graphPlayer != null)
                                        {
                                            CalculateChartData(compareToFight.m_Fight, compareToFightData, graphPlayer, getValueLambda, out chartSectionSecondDataTemp, out chartSectionSecondLabels);
                                            secondPlayers = 99;
                                        }
                                        else
                                        {
                                            CalculateChartData(compareToFight.m_Fight, realmDB, compareToFightData, getValueLambda, ref secondPlayers, out chartSectionSecondDataTemp, out chartSectionSecondLabels);
                                        }
                                        break;
                                    }
                                }
                            }

                            if (chartSectionSecondDataTemp != null)
                            {
                                chartSectionSecondData = new List<int>();
                                for(int i = 0; i < chartSectionLabels.Count; ++i)
                                {
                                    int firstIndex = chartSectionSecondLabels.FindIndex((_Value) => { return _Value >= chartSectionLabels[i]; });
                                    int nextDataValue = 0;
                                    if (firstIndex != -1)
                                    {
                                        int labelDiff = chartSectionSecondLabels[firstIndex] - chartSectionLabels[i];
                                        if (labelDiff > 0)
                                        {
                                            double deltaDiff = 0;
                                            if (firstIndex == 0)//Special case
                                            {
                                                deltaDiff = (double)(chartSectionSecondDataTemp[firstIndex + 1] - chartSectionSecondDataTemp[firstIndex]);
                                                if (deltaDiff < chartSectionSecondDataTemp[firstIndex])
                                                    deltaDiff /= (double)(chartSectionSecondLabels[firstIndex + 1] - chartSectionSecondLabels[firstIndex]);
                                                else
                                                    deltaDiff = 0;
                                            }
                                            else
                                            {
                                                deltaDiff = (double)(chartSectionSecondDataTemp[firstIndex] - chartSectionSecondDataTemp[firstIndex - 1]);
                                                if (deltaDiff < chartSectionSecondDataTemp[firstIndex])
                                                    deltaDiff /= (double)(chartSectionSecondLabels[firstIndex] - chartSectionSecondLabels[firstIndex - 1]);
                                                else
                                                    deltaDiff = 0;
                                            }
                                            nextDataValue = chartSectionSecondDataTemp[firstIndex] + (int)(deltaDiff * labelDiff);
                                        }
                                        else
                                        {
                                            nextDataValue = chartSectionSecondDataTemp[firstIndex];
                                        }
                                    }
                                    else
                                    {
                                        nextDataValue = 0;
                                    }
                                    if (nextDataValue < 0)
                                        nextDataValue = 0;
                                    chartSectionSecondData.Add(nextDataValue);
                                }
                                if (chartSectionSecondData.Count != chartSectionData.Count)
                                    throw new Exception("This should never happen!");
                            }
                        }

                        if (players > 5 && chartSectionLabels.Count > 1)
                        {
                            string datas = "";
                            string labels = "";
                            string labelDetails = "";
                            string secondDatas = "";

                            foreach (var time in chartSectionLabels)
                            {
                                labelDetails += time + ":' seconds into the fight',";
                                labels += time + ",";
                            }
                            foreach (var d in chartSectionData)
                            {
                                datas += d + ",";
                            }
                            if (chartSectionSecondData != null)
                            {
                                foreach (var d in chartSectionSecondData)
                                {
                                    secondDatas += d + ",";
                                }
                            }
                            graphSection += "<script>g_LabelsDetails" + graphIndex + " = {" + labelDetails + "}; ";
                            graphSection += "InitializeChart("
                                + "[" + labels + "]"
                                + ", [" + datas + "]"
                                + ", 550, 250, 20, 10, 'graph" + graphIndex + "Div'"
                                + ", function (data) { return data + ' " + typeStr + "'; }"
                                + ", function (lbl) { return lbl + g_LabelsDetails" + graphIndex + "[lbl]; }";
                            if (chartSectionSecondData != null)
                            {
                                graphSection += ", [" + secondDatas + "]";
                            }
                            graphSection += ");";

                            graphSection += "</script>"
                                + "<h2>" + fight.m_Fight.FightName + " @ " + fight.m_Fight.StartDateTime.ToLocalTime() + ", Duration: " + fight.m_Fight.FightDuration + " sec";
                            if (compareToFight != null)
                            {
                                graphSection += " vs " + compareToFight.m_Fight.FightDuration + " sec";
                            }
                            graphSection += "</h2><br />" + "<div id='graph" + graphIndex + "Div'></div>";

                            graphIndex++;
                        }
                    }
                }
                else
                {
                    graphSection += "<script>$(function () {";
                    {
                        foreach (var fight in fightDataCollection.Fights)
                        {
                            var unitsData = fight.GetUnitsDataList(true);
                            var sortedUnits = unitsData.OrderByDescending((_Unit) => { return getValueLambda(_Unit.Item2); });
                            if (sortedUnits.Count() > 0)
                            {
                                string newBossSection = "";
                                double totalValue = 0;
                                double maxValue = 0;
                                foreach (var unit in sortedUnits)
                                {
                                    var playerData = realmDB.RD_FindPlayer(unit.Item1);
                                    double currValue = getValueLambda(unit.Item2);
                                    if (playerData != null && currValue > 0)
                                    {
                                        totalValue += currValue;
                                        if (currValue > maxValue)
                                            maxValue = currValue;
                                    }
                                }
                                newBossSection += "VF_CreateDmgBar(1.0, '#000000', '" + fight.m_Fight.FightName + " @ " + fight.m_Fight.StartDateTime.ToLocalTime() + "','#ffffff', '" + totalValue.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " total(" + typeStr + "), " + fight.m_Fight.FightDuration + " sec');";
                                int players = 0;
                                foreach (var unit in sortedUnits)
                                {
                                    var playerData = realmDB.RD_FindPlayer(unit.Item1);
                                    double currValue = getValueLambda(unit.Item2);
                                    if (playerData != null && currValue > 0)
                                    {
                                        double percentage = (double)currValue / totalValue;

                                        double displaypercentage = percentage / (maxValue / totalValue);
                                        //percentage *= maxValue;
                                        if (displaypercentage > 0.05f && players < 15)
                                        {
                                            ++players;
                                            string classColor = "#CCCCCC";
                                            classColor = RealmPlayersServer.Code.Resources.VisualResources._ClassColors[playerData.Character.Class];
                                            newBossSection += "VF_CreateDmgBar(" + displaypercentage.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", '" + classColor + "', '<player," + unit.Item1 + ">" + unit.Item1 + "','#000000', '" + (int)currValue + "(" + string.Format("{0:0.0%}", percentage) + ")');";
                                        }
                                    }
                                }
                                if (players > 5)
                                    graphSection += newBossSection;
                            }
                        }
                    }
                    graphSection += "});";
                    graphSection += "</script>";
                    graphSection += "<div id='diagramDiv'></div>";
                }

                m_ChartSection = new MvcHtmlString(graphSection);
            }*/
        }
    }
}