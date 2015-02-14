using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using RaidBossFight = VF_RaidDamageDatabase.RaidBossFight;
using AttemptType = VF_RaidDamageDatabase.FightData.AttemptType;

namespace VF_RaidDamageWebsite
{
    public partial class FightOverview : System.Web.UI.Page
    {
        public class DataPresentTypeInfo
        {
            public Func<VF_RaidDamageDatabase.UnitData, double> m_GetValue;
            public string m_TypeName;
            public int m_Count;
            public Func<VF_RaidDamageDatabase.UnitData, bool> m_ValidCheck;
            public DataPresentTypeInfo(Func<VF_RaidDamageDatabase.UnitData, double> _GetValue, string _TypeName, int _Count = -1, Func<VF_RaidDamageDatabase.UnitData, bool> _ValidCheck = null)
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
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.Dmg; }, "Damage", 25),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.DPS; }, "DPS", 10, (VF_RaidDamageDatabase.UnitData _UnitData) => { return _UnitData.Dmg > 0; }),
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.EffHeal; }, "Efficient Heal"),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.HPS; }, "HPS", 10, (VF_RaidDamageDatabase.UnitData _UnitData) => { return _UnitData.EffHeal > 0; }),
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.OverHeal; }, "Overheal"),
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.RawHeal; }, "Raw Heal"),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.ThreatValue; }, "Threat", 25),
            new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.I.DmgTaken; }, "Damage Taken", 25),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.Dmg; }, "Damage"),
            //new DataPresentTypeInfo((VF_RaidDamageDatabase.UnitData _UnitData) => { return (double)_UnitData.Dmg; }, "Damage"),
        };

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

        Dictionary<char, string> BuffIconParser = new Dictionary<char, string>{
            {'a', "WTF_"},
            {'b', "Ability_"},
            {'c', "Racial_"},
            {'d', "Creature_"},
            {'e', "Warrior_"},
            {'f', "Paladin_"},
            {'g', "Hunter_"},
            {'h', "Druid_"},
            {'i', "Rogue_"},
            {'j', "Shaman_"},
            {'k', "Mage_"},
            {'l', "Mount_"},
            {'m', "Pet_"},
            {'n', "INV_"},
            {'o', "Potion_"},
            {'p', "Spell_"},
            {'q', "Fire_"},
            {'r', "Frost_"},
            {'s', "Holy_"},
            {'t', "Nature_"},
            {'u', "Shadow_"},
            {'v', "Sword_"},
            {'w', "Misc_"},
            {'x', "Food_"},
            {'y', "Jewelry_"},
            {'z', "Drink_"},
        };

        string GetBuffIconImage(string _Buff)
        {
            string realBuffName = "";
            bool fresh = true;
            for (int i = 0; i < _Buff.Length; ++i)
            {
                if(fresh == true)
                {
                    string value;
                    if (BuffIconParser.TryGetValue(_Buff[i], out value) == true)
                    {
                        realBuffName += value;
                    }
                    else
                    {
                        realBuffName += _Buff[i];
                        fresh = false;
                    }
                }
                else
                {
                    realBuffName += _Buff[i];
                    if (_Buff[i] == '_')
                        fresh = true;
                    else
                        fresh = false;
                }
            }
            return "<img src='http://realmplayers.com/Assets/wowicons/43x43/" + realBuffName + ".png'></img>";
        }

        private void GenerateBossFightPage(bool DEBUG_Website, bool filteredData, int uniqueRaidID, RaidBossFight interestingFight)
        {
            if (interestingFight == null)
                Response.Redirect("RaidList.aspx");

            var realmDB = ApplicationInstance.Instance.GetRealmDB(interestingFight.GetRaid().Realm);

            int tryNumber = 1;
            int totalTries = 0;
            interestingFight.GetTryCount(out tryNumber, out totalTries);
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
                + PageUtility.BreadCrumb_AddLink("RaidList.aspx?Guild=" + interestingFight.GetRaid().RaidOwnerName, interestingFight.GetRaid().RaidOwnerName)
                    + PageUtility.BreadCrumb_AddRaidOverview(interestingFight.GetRaid())
                    + PageUtility.BreadCrumb_AddFinish(interestingFight.GetBossName()));

                var attemptType = interestingFight.GetFightData().GetAttemptType();
                string attemptStr = "";
                if (attemptType == AttemptType.KillAttempt)
                    attemptStr = "(kill)";
                else if (attemptType == AttemptType.WipeAttempt)
                    attemptStr = "(wipe)";
                else if (attemptType == AttemptType.TrashAttempt)
                    attemptStr = "(trash)";

                string fightOverViewInfo = "<h1>" + interestingFight.GetRaid().RaidOwnerName + " vs ";

                if (attemptType != AttemptType.TrashAttempt)
                {
                    fightOverViewInfo += PageUtility.CreateLink("FightOverallOverview.aspx?FightName=" + interestingFight.GetFightData().FightName, interestingFight.GetFightData().FightName)
                        + "(" + interestingFight.GetRaid().RaidID.ToString() + ")<span class='badge badge-inverse'>attempt " + tryNumber + "/" + totalTries + " " + attemptStr + "</span>";

                    fightOverViewInfo += "</h1><p>Fight started " + interestingFight.GetStartDateTime().ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                     + " and fight duration was " + interestingFight.GetFightDuration() + " seconds.</p><p>" + (interestingFight.GetFightData().HasResetsMidFight() ? "SW_Stats Reset occured mid fight thus the data on this page may not be accurate" : "") + "</p>";
                }
                else//if(attemptType == AttemptType.TrashAttempt)
                {
                    fightOverViewInfo += "Trash" + (interestingFight.GetRaidBossFightIndex() + 1) + "(" + interestingFight.GetRaid().RaidID.ToString() + ")";
                    fightOverViewInfo += "</h1><p>Trash between " + interestingFight.GetStartDateTime().ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " and " + interestingFight.GetFightData().GetEndDateTime().ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "</p>";
                }

                string recordedByStr = "#CCCCCC";
                try
                {
                    recordedByStr = PageUtility.CreateColorCodedName(realmDB.GetPlayer(interestingFight.GetFightData().RecordedByPlayer));
                }
                catch (Exception)
                {
                    recordedByStr = "<font color='#CCCCCC'>" + interestingFight.GetFightData().RecordedByPlayer + "</font>";
                }
                fightOverViewInfo += "<p>Fight was recorded by " + recordedByStr + " using addon version " + "<font color='#00FF00'>" + interestingFight.GetFightData().AddonVersion + "</font></p>";

                string enemyUnits = "<h3>Enemy units:</h3>";
                {
                    List<Tuple<string, int>> dmgTakenList = new List<Tuple<string, int>>();
                    int bossPlusAddsDmgTaken = interestingFight.GetBossPlusAddsDmgTaken(dmgTakenList);

                    double totalValue = interestingFight.GetTotal((_Value) => { return _Value.I.Dmg; }, (_Value) => { return realmDB.RD_IsPlayer(_Value.Item1, interestingFight) && _Value.Item2.I.Dmg > 0; });
                    if (totalValue < bossPlusAddsDmgTaken)
                        totalValue = bossPlusAddsDmgTaken;
                    var dumgTakenSorted = dmgTakenList.OrderByDescending((_Value) => { if (_Value.Item1 == interestingFight.GetFightData().FightName) return int.MaxValue; return _Value.Item2; });
                    foreach (var dmgTakenObj in dumgTakenSorted)
                    {
                        if (dmgTakenObj.Item1 == interestingFight.GetFightData().FightName)
                            enemyUnits += "<font color='#00FF00'>"
                                + ((double)dmgTakenObj.Item2 / totalValue).ToString("0%", System.Globalization.CultureInfo.InvariantCulture)
                                + "(" + ((double)dmgTakenObj.Item2 / 1000.0).ToStringDot("0.0") + "k)"
                                + "</font>"
                                + " " + dmgTakenObj.Item1 + "(Boss)<br />";
                        else
                            enemyUnits += "<font color='#FFFF00'>"
                                + ((double)dmgTakenObj.Item2 / totalValue).ToString("0%", System.Globalization.CultureInfo.InvariantCulture)
                                + "(" + ((double)dmgTakenObj.Item2 / 1000.0).ToStringDot("0.0") + "k)"
                                + "</font>"
                                + " " + dmgTakenObj.Item1 + "(Add)<br />";
                    }
                    enemyUnits += "<font color='#FF0000'>"
                        + (1.0 - ((double)bossPlusAddsDmgTaken / totalValue)).ToString("0%", System.Globalization.CultureInfo.InvariantCulture)
                        + "(" + ((double)(totalValue - bossPlusAddsDmgTaken) / 1000.0).ToStringDot("0.0") + "k)"
                        + "</font>"
                        + " (Trash)";
                }


                string playerDeaths = "";
                var fightEvents = VF_RaidDamageDatabase.FightEvent.GenerateFightEvents(interestingFight);
                int lastDeathTime = 0;
                foreach (var fightEvent in fightEvents)
                {
                    if (fightEvent.m_FightEvent == VF_RaidDamageDatabase.FightEventEnum.UnitDeath)
                    {
                        var unitName = fightEvent.m_StringData;
                        var playerData = realmDB.RD_FindPlayer(unitName, interestingFight);
                        if (playerData != null)
                        {
                            if (lastDeathTime != fightEvent.m_TimeIntoTheFight)
                            {
                                if (playerDeaths == "")
                                    playerDeaths = "<h3>Player deaths:</h3>";
                                else
                                    playerDeaths += "<br />";

                                playerDeaths += "After " + fightEvent.m_TimeIntoTheFight + "~ seconds: ";
                                lastDeathTime = fightEvent.m_TimeIntoTheFight;
                            }
                            else
                                playerDeaths += ", ";
                            playerDeaths += PageUtility.CreateColorCodedName(unitName, playerData.Character.Class);
                        }
                    }
                }
                /////////////////////
                string unrealisticPlayerSpikes = "";
                var unrealisticPlayerSpikesList = interestingFight.GetUnrealisticPlayerSpikes(realmDB.RD_GetPlayerIdentifierFunc(interestingFight));
                if (true)//unrealisticPlayerSpikesList.Count < 10)
                {
                    foreach (var uPS in unrealisticPlayerSpikesList)
                    {
                        if (unrealisticPlayerSpikes != "")
                            unrealisticPlayerSpikes += "<br />";
                        unrealisticPlayerSpikes += uPS.Player
                            + " Dmg(" + uPS.DmgValue + "),"
                            + " Heal(" + uPS.HealValue + ") @"
                            + " Time(" + uPS.Time + "),"
                            + " Factor(" + uPS.SpikeDmgFactor.ToStringDot("0.00") + "," + uPS.SpikeHealFactor.ToStringDot("0.00") + ")";
                    }
                    if (unrealisticPlayerSpikes != "")
                    {
                        if (filteredData == true)
                        {
                            unrealisticPlayerSpikes = "<h3>Filtered unrealistic player Dmg/Heal spikes:</h3>" + unrealisticPlayerSpikes;
                            unrealisticPlayerSpikes += "<br /><a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Filtered", "false") + "'>View Unfiltered</a>";
                        }
                        else
                        {
                            unrealisticPlayerSpikes = "<h3>Unrealistic player Dmg/Heal spikes:</h3>" + unrealisticPlayerSpikes;
                            unrealisticPlayerSpikes += "<br /><a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Filtered", "true") + "'>View Filtered</a>";
                        }
                    }
                }
                else
                {
                    if (filteredData == true)
                    {
                        unrealisticPlayerSpikes = "<h3>Filtered unrealistic player Dmg/Heal spikes:</h3>Too many unrealistic spikes, nothing is filtered";
                        filteredData = false;
                    }
                    else
                    {
                        unrealisticPlayerSpikes = "<h3>Unrealistic player Dmg/Heal spikes:</h3>Too many unrealistic spikes, can not list them all";
                    }
                }
                /////////////////////
                List<Tuple<string, VF_RealmPlayersDatabase.PlayerClass>> playersAttending = new List<Tuple<string, VF_RealmPlayersDatabase.PlayerClass>>();

                var attendingUnits = interestingFight.GetAttendingUnits();
                foreach (var unitName in attendingUnits)
                {
                    var playerData = realmDB.RD_FindPlayer(unitName, interestingFight);
                    if (playerData != null)
                        playersAttending.Add(new Tuple<string, VF_RealmPlayersDatabase.PlayerClass>(unitName, playerData.Character.Class));
                }
                string playersAttendingStr = "<h3>Players attending(" + playersAttending.Count + "):</h3>";
                var orderedPlayersAttending = playersAttending.OrderBy((_Value) => { return "" + (int)_Value.Item2 + _Value.Item1; });
                var lastClass = orderedPlayersAttending.First().Item2;
                foreach (var player in orderedPlayersAttending)
                {
                    if (lastClass != player.Item2)
                        playersAttendingStr += "<br />";
                    playersAttendingStr += PageUtility.CreateColorCodedName(player.Item1, player.Item2) + " ";
                    lastClass = player.Item2;
                }
                /////////////////////

                ///////////////////////
                string buffInfo = "";
                if (PageUtility.GetQueryString(Request, "DebugBuff", "null") != "null")
                {
                    string debugBuffName = PageUtility.GetQueryString(Request, "DebugBuff", "null");
                    if (playersAttending.Exists((_Value) => { return _Value.Item1 == debugBuffName; }) == true)
                    {
                        int playerNameID = interestingFight.GetFightCacheData().m_FightDataCollection.GetUnitIDFromName(debugBuffName);
                        Dictionary<int, int> accumulatedTime = new Dictionary<int, int>();
                        Dictionary<int, int> accumulatedTime2 = new Dictionary<int, int>();
                        int prevTimeSliceTime = interestingFight.GetFightData().TimeSlices.First().Time;
                        foreach (var timeSlice in interestingFight.GetFightData().TimeSlices)
                        {
                            int deltaTime = timeSlice.Time - prevTimeSliceTime;
                            if (timeSlice.UnitBuffs != null)
                            {
                                foreach (var unitBuff in timeSlice.UnitBuffs)
                                {
                                    if (unitBuff.Key == playerNameID)
                                    {
                                        if (unitBuff.Value == null)
                                            continue;
                                        foreach (var buff in unitBuff.Value)
                                        {
                                            if (accumulatedTime.ContainsKey(buff.BuffID) == false)
                                            {
                                                accumulatedTime.Add(buff.BuffID, 0);
                                            }
                                            accumulatedTime[buff.BuffID] += deltaTime;
                                        }
                                    }
                                }
                            }
                            if (timeSlice.UnitDebuffs != null)
                            {
                                foreach (var unitBuff in timeSlice.UnitDebuffs)
                                {
                                    if (unitBuff.Key == playerNameID)
                                    {
                                        if (unitBuff.Value == null)
                                            continue;
                                        foreach (var buff in unitBuff.Value)
                                        {
                                            if (accumulatedTime2.ContainsKey(buff.BuffID) == false)
                                            {
                                                accumulatedTime2.Add(buff.BuffID, 0);
                                            }
                                            accumulatedTime2[buff.BuffID] += deltaTime;
                                        }
                                    }
                                }
                            }
                        }

                        buffInfo = "<h3>Buffs used by " + debugBuffName + ":</h3>";
                        var orderedAccTime = accumulatedTime.OrderByDescending((_Value) => _Value.Value);
                        foreach (var accum in orderedAccTime)
                        {
                            buffInfo += GetBuffIconImage(interestingFight.GetFightCacheData().m_FightDataCollection.m_BuffIDToNames[accum.Key])
                                + " buff during " + accum.Value + " seconds of the fight!<br />";
                        }
                        buffInfo += "<h3>Debuffs:</h3>";
                        var orderedAccTime2 = accumulatedTime2.OrderByDescending((_Value) => _Value.Value);
                        foreach (var accum in orderedAccTime2)
                        {
                            buffInfo += GetBuffIconImage(interestingFight.GetFightCacheData().m_FightDataCollection.m_BuffIDToNames[accum.Key])
                                + " debuff during " + accum.Value + " seconds of the fight!<br />";
                        }
                    }
                    else
                    {
                        var buffIDToNames = interestingFight.GetFightCacheData().m_FightDataCollection.m_BuffIDToNames;
                        if (buffIDToNames != null && buffIDToNames.Count > 0)
                        {
                            buffInfo = "<h3>Buffs used:</h3>";
                            foreach (var buff in buffIDToNames)
                            {
                                buffInfo += GetBuffIconImage(buff);
                            }
                        }
                    }
                }
                ///////////////////////

                var realm = interestingFight.GetRaid().Realm;
                ///////////////////////
                string lootDropped = "";
                if (attemptType == VF_RaidDamageDatabase.FightData.AttemptType.KillAttempt)
                {
                    lootDropped = "<h3>Boss Loot:</h3>";
                    lootDropped += RaidOverview.CreateLootDroppedData(interestingFight.GetItemDrops(), realmDB);
                }
                ///////////////////////

                m_FightOverviewInfoHTML = new MvcHtmlString(fightOverViewInfo + playersAttendingStr + buffInfo + lootDropped + enemyUnits + playerDeaths + unrealisticPlayerSpikes);

                string graphSection = "";
                {
                    List<int> dataX = new List<int>();
                    List<int> dataY1 = new List<int>();
                    List<int> dataY2 = new List<int>();
                    List<string> labels = new List<string>();

                    var fightDetails = interestingFight.GetFightDetails();
                    if (filteredData == true)
                    {
                        fightDetails = interestingFight.GetFilteredFightDetails(realmDB.RD_GetPlayerIdentifierFunc(interestingFight));
                    }

                    bool properStarted = false;
                    for (int i = 0; i < fightDetails.Count; ++i)
                    {
                        bool lastEvent = false;

                        var fightDetail = fightDetails[i];

                        int sliceDuration = 1;
                        if (i != 0)
                        {
                            sliceDuration = fightDetail.Time - fightDetails[i - 1].Time;
                            if (sliceDuration < 1)
                                sliceDuration = 1;
                        }

                        string events = "";
                        List<Tuple<string, int, int>> bossHealths = new List<Tuple<string, int, int>>();
                        int currHealth = 0;
                        int maxHealth = 0;
                        foreach (var fightEvent in fightDetail.Events)
                        {
                            try
                            {
                                if (fightEvent.StartsWith("BossHealth="))
                                {
                                    var bossHealth_CurrAndMax = fightEvent.Split(new string[] { "BossHealth=", "-" }, StringSplitOptions.RemoveEmptyEntries);
                                    currHealth = int.Parse(bossHealth_CurrAndMax[0]);
                                    maxHealth = int.Parse(bossHealth_CurrAndMax[1]);
                                }
                                else if (fightEvent.StartsWith("BossHealth-"))
                                {
                                    var bossHealth_NameCurrAndMax = fightEvent.Split(new string[] { "BossHealth-", "-", "=" }, StringSplitOptions.RemoveEmptyEntries);
                                    bossHealths.Add(Tuple.Create(bossHealth_NameCurrAndMax[0], int.Parse(bossHealth_NameCurrAndMax[1]), int.Parse(bossHealth_NameCurrAndMax[2])));
                                }
                                else if (DEBUG_Website == true)
                                {
                                    events = events + fightEvent + ", ";
                                }
                                else
                                {
                                    if (fightEvent.StartsWith("Start"))
                                    {
                                        string startType = fightEvent.Split(new char[] { '=', ' ' })[0];
                                        if (fightEvent.Contains("=" + interestingFight.GetBossName()) && properStarted == false)
                                        {
                                            //if (startType == "Start" || startType == "Start_C" || startType == "Start_S")
                                            {
                                                events = events + "Fight Started, ";
                                                dataX.Clear();
                                                dataY1.Clear();
                                                dataY2.Clear();
                                                labels.Clear();
                                                properStarted = true;
                                            }
                                        }
                                    }
                                    else if (fightEvent.StartsWith("Phase"))
                                    {
                                        string phaseType = fightEvent.Split(new char[] { '=', ' ' })[0];
                                        if (phaseType.EndsWith("_Y"))
                                            phaseType = phaseType.Substring(0, phaseType.Length - 2);
                                        events = events + phaseType + ", ";
                                    }
                                    else if (fightEvent.StartsWith("Dead"))
                                    {
                                        if (fightEvent.Contains("=" + interestingFight.GetBossName()))
                                        {
                                            string deadType = fightEvent.Split(new char[] { '=', ' ' })[0];
                                            //if (deadType == "Dead" || deadType == "Dead_C" || deadType == "Dead_S")
                                            events = events + "Fight Ended, ";
                                            lastEvent = true;
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            { }
                        }
                        string deathEvents = "";
                        int totalDmg = 0;
                        int totalHeal = 0;
                        int topDPSDmg = 0;
                        int topHPSHeal = 0;
                        string topDPSer = "";
                        string topHPSer = "";
                        foreach (var unitData in fightDetail.UnitDatas)
                        {
                            var unitPlayer = realmDB.RD_FindPlayer(unitData.Key, interestingFight);
                            if (unitPlayer != null)
                            {
                                totalDmg += unitData.Value.I.Dmg;
                                totalHeal += unitData.Value.I.EffHeal;
                                if (unitData.Value.I.Death > 0)
                                {
                                    deathEvents += PageUtility.CreateColorCodedName(unitData.Key, unitPlayer.Character.Class) + ",";
                                }
                                if (unitData.Value.I.Dmg > topDPSDmg)
                                {
                                    topDPSDmg = unitData.Value.I.Dmg;
                                    topDPSer = PageUtility.CreateColorCodedName(unitData.Key, unitPlayer.Character.Class);
                                }
                                if (unitData.Value.I.EffHeal > topHPSHeal)
                                {
                                    topHPSHeal = unitData.Value.I.EffHeal;
                                    topHPSer = PageUtility.CreateColorCodedName(unitData.Key, unitPlayer.Character.Class);
                                }
                            }
                        }
                        int totalDPS = totalDmg / sliceDuration;
                        int totalHPS = totalHeal / sliceDuration;

                        string bossHealthStr = "BossHealth: ?%(?k/?k)";
                        if (bossHealths.Count > 0)
                        {
                            bossHealthStr = "BossHealth: ";

                            foreach (var bossHealth in bossHealths)
                            {
                                int cBH = bossHealth.Item2;
                                int cBMH = bossHealth.Item3;
                                if (cBH != 0)
                                {
                                    bossHealthStr = bossHealthStr + bossHealth.Item1 + " = "
                                        + ((double)cBH / (double)cBMH).ToStringDot("0.0%") + "(" + (cBH / 1000) + "k/" + (cBMH / 1000) + "k), ";
                                }
                            }
                            if (bossHealthStr == "BossHealth: ")
                                bossHealthStr = "BossHealth: ?%(?k/?k)";
                        }
                        else if (maxHealth != 0)
                            bossHealthStr = "BossHealth: " + ((double)currHealth / (double)maxHealth).ToStringDot("0.0%") + "(" + (currHealth / 1000) + "k/" + (maxHealth / 1000) + "k)";

                        events = "Time: " + fightDetail.Time + "&nbsp;&nbsp;&nbsp;" + bossHealthStr + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Events: " + events;
                        if (deathEvents != "")
                            events += "&nbsp;&nbsp;&nbsp;Deaths: " + deathEvents + "";
                        events += "<br /><br />";

                        events += "RaidDPS: " + PageUtility.CreateColorString(totalDPS.ToString(), System.Drawing.Color.Red)
                            + "&nbsp;&nbsp;&nbsp;RaidHPS: " + PageUtility.CreateColorString(totalHPS.ToString(), System.Drawing.Color.Green) + "&nbsp;&nbsp;&nbsp;";
                        events += "TopDPS: " + topDPSer + "(" + PageUtility.CreateColorString((topDPSDmg / sliceDuration).ToString(), System.Drawing.Color.Red) + ")"
                            + "&nbsp;&nbsp;&nbsp;TopHPS: " + topHPSer + "(" + PageUtility.CreateColorString((topHPSHeal / sliceDuration).ToString(), System.Drawing.Color.Green) + ")";


                        events += "<br /><br />";
                        dataX.Add(fightDetail.Time);
                        labels.Add(events);
                        dataY1.Add(totalDPS);
                        dataY2.Add(totalHPS);
                        if (lastEvent == true)
                            break;
                    }
                    graphSection += "<div class='fame' style='min-width: 935px; max-width: 935px'>" + PageUtility.CreateGraph(dataX, dataY1, System.Drawing.Color.Red, dataY2, System.Drawing.Color.Green, labels) + "</div>";
                }

                List<Tuple<string, VF_RaidDamageDatabase.UnitData>> unitsData = null;
                if (filteredData == true)
                {
                    unitsData = interestingFight.GetFilteredPlayerUnitsDataCopy(true, realmDB.RD_GetPlayerIdentifierFunc(interestingFight));
                }
                else
                {
                    unitsData = interestingFight.GetUnitsDataCopy(true);
                }
                var petData = interestingFight.GetFilteredPetUnitsData();
                List<Tuple<string, VF_RaidDamageDatabase.UnitData>> abusingPets = new List<Tuple<string, VF_RaidDamageDatabase.UnitData>>();
                
                List<string> entireRaidMembers;
                var summaryDBRaid = ApplicationInstance.Instance.GetSummaryDatabase().GetRaid(uniqueRaidID);
                if (summaryDBRaid != null)
                {
                    entireRaidMembers = summaryDBRaid.m_RaidMembers;
                }
                else
                {
                    entireRaidMembers = interestingFight.GetFightCacheData().m_FightDataCollection.m_RaidMembers;
                }
                foreach (var unitPet in petData)
                {
                    if (entireRaidMembers.Contains(unitPet.Item1.Split('(').First()) == true)
                    {
                        //Player with Pet UnitPet should be banned from damagemeter or has its damage purged
                        string abusingPlayer = unitPet.Item1.Split(new string[] { "(Pet for ", ")" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        abusingPets.Add(Tuple.Create(abusingPlayer, unitPet.Item2));
                    }
                }
                foreach (var abusingPet in abusingPets)
                {
                    for (int i = 0; i < unitsData.Count; ++i)
                    {
                        if (unitsData[i].Item1 == abusingPet.Item1)
                        {
                            var unitFightData = unitsData[i].Item2.CreateCopy();
                            unitFightData.SubtractUnitData(abusingPet.Item2);
                            unitsData[i] = Tuple.Create(abusingPet.Item1, unitFightData);
                        }
                    }
                }
                unitsData.AddRange(petData);
                graphSection += "<style>" + PageUtility.CreateStatsBars_HTML_CSSCode() + "</style>";
                {
                    PageUtility.StatsBarStyle statsBarStyle = new PageUtility.StatsBarStyle
                    {
                        m_TitleText = "",
                        m_BarTextColor = "#000",
                        m_LeftSideTitleText = "#",
                        m_RightSideTitleText = "",
                        m_BeforeBarWidth = 25,
                        m_MaxWidth = 470,
                        m_AfterBarWidth = 30
                    };

                    string dmgThreatSection = "<div class='span4' style='min-width: 460px;'>";
                    string healSection = "<div class='span4' style='min-width: 460px;'>";
                    foreach (var dataPresentTypeInfo in sm_DataPresentTypeInfoList)
                    {
                        var sortedUnits = unitsData.OrderByDescending((_Unit) => { return dataPresentTypeInfo.m_GetValue(_Unit.Item2); });
                        if (sortedUnits.Count() > 0)
                        {
                            List<PageUtility.StatsBarData> statsBars = new List<PageUtility.StatsBarData>();
                            //string newBossSection = "";
                            double totalValue = 0;
                            double maxValue = 0;
                            int bossDmgTaken = -1;
                            VF_RaidDamageDatabase.UnitData.CalculateTotalAndMax(unitsData.AsReadOnly(), dataPresentTypeInfo.m_GetValue
                                , (_Value) => { return realmDB.RD_IsPlayer(_Value.Item1, interestingFight) && dataPresentTypeInfo.m_ValidCheck(_Value.Item2); }
                                , out totalValue, out maxValue);
                            //newBossSection += "VF_CreateDmgBar(1.0, '#000000', '" + dataPresentTypeInfo.m_TypeName + "','#ffffff', '" + (totalValue / 1000).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "k total');";
                            int players = 0;
                            foreach (var unit in sortedUnits)
                            {
                                if (unit.Item1 == interestingFight.GetBossName())
                                {
                                    bossDmgTaken = unit.Item2.I.DmgTaken;
                                }
                                var playerData = realmDB.RD_FindPlayer(unit.Item1, interestingFight);
                                double currValue = dataPresentTypeInfo.m_GetValue(unit.Item2);
                                if (playerData != null && currValue > 0 && dataPresentTypeInfo.m_ValidCheck(unit.Item2))
                                {
                                    double percentage = (double)currValue / totalValue;

                                    double displayPercentage = percentage / (maxValue / totalValue);
                                    //percentage *= maxValue;
                                    //if (players < dataPresentTypeInfo.m_Count)
                                    {
                                        ++players;
                                        string classColor = "#CCCCCC";
                                        classColor = RealmPlayersServer.Code.Resources.VisualResources._ClassColors[playerData.Character.Class];
                                        //newBossSection += "VF_CreateDmgBar(" + displayPercentage.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", '" + classColor + "', '<player," + unit.Item1 + ">" + unit.Item1 + "(" + (currValue / interestingFight.m_Fight.FightDuration).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "/s)','#000000', '" + (int)currValue + "(" + string.Format("{0:0.0%}", percentage) + ")');";
                                        
                                        string rightSideText = "" + (int)currValue + "(" + (currValue / interestingFight.GetFightDuration()).ToStringDot("0") + "/s)";
                                        statsBars.Add(new PageUtility.StatsBarData
                                        {
                                            m_BeforeBarText = "#" + players,// + "(" + string.Format("{0:0.0%}", percentage) + ")",
                                            m_OnBarLeftText = PageUtility.CreateLink_RaidStats_Player(playerData),
                                            m_BarColor = classColor,
                                            m_PercentageWidth = displayPercentage,
                                            m_AfterBarText = percentage.ToStringDot("0.0%"),
                                            m_OnBarRightText = rightSideText,
                                            m_OnBarTextWidth = StaticValues.MeasureStringLength(playerData.Name + " " + rightSideText)
                                        });
                                    }
                                }
                                else if (unit.Item1.Contains("(Pet for"))
                                {
                                    double percentage = (double)currValue / totalValue;

                                    double displayPercentage = percentage / (maxValue / totalValue);
                                    //percentage *= maxValue;
                                    //if (players < dataPresentTypeInfo.m_Count)
                                    {
                                        ++players;
                                        string classColor = "#00FF00";
                                        //newBossSection += "VF_CreateDmgBar(" + displayPercentage.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", '" + classColor + "', '<player," + unit.Item1 + ">" + unit.Item1 + "(" + (currValue / interestingFight.m_Fight.FightDuration).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "/s)','#000000', '" + (int)currValue + "(" + string.Format("{0:0.0%}", percentage) + ")');";

                                        string rightSideText = "" + (int)currValue + "(" + (currValue / interestingFight.GetFightDuration()).ToStringDot("0") + "/s)";
                                        statsBars.Add(new PageUtility.StatsBarData
                                        {
                                            m_BeforeBarText = "#" + players,// + "(" + string.Format("{0:0.0%}", percentage) + ")",
                                            m_OnBarLeftText = unit.Item1,
                                            m_BarColor = classColor,
                                            m_PercentageWidth = displayPercentage,
                                            m_AfterBarText = percentage.ToStringDot("0.0%"),
                                            m_OnBarRightText = rightSideText,
                                            m_OnBarTextWidth = StaticValues.MeasureStringLength(unit.Item1 + " " + rightSideText)
                                        });
                                    }
                                }
                            }
                            //graphSection += newBossSection;
                            statsBarStyle.m_TitleText = dataPresentTypeInfo.m_TypeName + "(" + (totalValue / 1000).ToStringDot("0.0") + "k)";
                            if (dataPresentTypeInfo.m_TypeName == "Damage" || dataPresentTypeInfo.m_TypeName == "Threat" || dataPresentTypeInfo.m_TypeName == "Damage Taken")
                                dmgThreatSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, statsBars, dataPresentTypeInfo.m_Count);
                            else
                                healSection += PageUtility.CreateStatsBars_HTML(statsBarStyle, statsBars, dataPresentTypeInfo.m_Count);
                        }
                    }
                    //graphSection = "<script>$(function () {if (g_RaphaelBarsDrawer == null) {g_RaphaelBarsDrawer = Raphael('diagramDiv', 700, " + (totalBarsCount * 20) + ");}"
                    //    + graphSection;
                    dmgThreatSection += "</div>";
                    healSection += "</div";
                    graphSection += "<div class='row'>" + dmgThreatSection;// +"<div class='span4'></div>";// +"<div class='span1' style='min-width: 50px;'></div>";
                    graphSection += healSection + "</div>";
                }

                //graphSection += "});";
                //graphSection += "</script>";
                //graphSection += "<div id='diagramDiv'></div>";
                m_GraphSection = new MvcHtmlString(graphSection);
            }
            else
            {
                Response.Redirect("RaidOverview.aspx?Raid=" + uniqueRaidID);
            }
        }
    }
}