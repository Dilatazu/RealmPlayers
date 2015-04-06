using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using RealmDB = VF_RaidDamageDatabase.RealmDB;
using RaidBossFight = VF_RaidDamageDatabase.RaidBossFight;
using RaidSummary = VF_RDDatabase.Raid;
using SummaryFight = VF_RDDatabase.BossFight;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;

using RPPDatabase = VF_RaidDamageDatabase.RPPDatabase;

namespace VF
{
    public class FightOverallOverviewGenerator
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

        public struct GenerateDetails
        {
            public List<VF_RealmPlayersDatabase.PlayerClass> ClassFilter;
            public PlayerFaction FactionFilter;
            public VF_RealmPlayersDatabase.WowRealm RealmFilter;
            public string GuildFilter;
            public bool ShowMultipleEntries;
            public int EntriesCount;
            public List<string> IncludePlayers;
        }
        public static string Generate(List<SummaryFight> _FightInstances, RPPDatabase _RPPDatabase, GenerateDetails _Details)
        {
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

            RealmDB realmDB = null;

            double highestPrecision = 0;
            Dictionary<string, List<PlayerFightValuePerSecond>> combinedData = new Dictionary<string, List<PlayerFightValuePerSecond>>();
            foreach (var raidFight in _FightInstances)
            {
                var currGroup = raidFight.CacheRaid.CacheGroup;

                if (_Details.RealmFilter != VF_RealmPlayersDatabase.WowRealm.All && _Details.RealmFilter != currGroup.Realm)
                    continue;

                if (realmDB == null || currGroup.Realm != realmDB.Realm)
                    realmDB = _RPPDatabase.GetRealmDB(currGroup.Realm);

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
                                && (_Details.ClassFilter == null || _Details.ClassFilter.Contains(playerData.Character.Class))
                                && (_Details.FactionFilter == PlayerFaction.Unknown || _Details.FactionFilter == StaticValues.GetFaction(playerData.Character.Race))
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

                if (raidFight.DataDetails.FightPrecision > highestPrecision)
                    highestPrecision = raidFight.DataDetails.FightPrecision;
            }

            string graphSection = "<style>" + PageUtility.CreateStatsBars_HTML_CSSCode() + "</style>";

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
                    if (_Details.ShowMultipleEntries == false)
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
                    if (players > dataPresentTypeInfo.m_Count && players > _Details.EntriesCount)
                    {
                        if (_Details.IncludePlayers == null)
                            break;
                        else if (_Details.IncludePlayers.Contains(candidate.m_Player.Name) == false)
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
                var orderedFights = _FightInstances.OrderBy((_Value) => _Value.FightDuration);
                List<PageUtility.StatsBarData> statsBars = new List<PageUtility.StatsBarData>();
                int raidCounter = 0;
                double firstClearCompareValue = 1 / (double)orderedFights.First().FightDuration;

                List<string> ignoreGuilds = new List<string>();
                foreach (var raidFight in orderedFights)
                {
                    if (_Details.RealmFilter != VF_RealmPlayersDatabase.WowRealm.All && _Details.RealmFilter != raidFight.CacheRaid.CacheGroup.Realm)
                        continue;

                    if (_Details.ShowMultipleEntries == false && _Details.GuildFilter == null)
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
                    if (++raidCounter > _Details.EntriesCount)
                        break;
                    double compareValue = 1.0 / (double)raidFight.FightDuration;

                    string factionColor = "#CCCCCC";
                    try
                    {
                        var recordedByPlayer = _RPPDatabase.GetRealmDB(raidFight.CacheRaid.CacheGroup.Realm).RD_FindPlayer(raidFight.PlayerFightData.First().Item1, raidFight);
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
                        if (raidOwner == "Team Plague")
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

            return graphSection;
        }
    }
}