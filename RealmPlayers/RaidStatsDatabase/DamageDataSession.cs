using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VF_RaidDamageDatabase
{
    public class DamageDataSession
    {
        public static char DamageDataSession_VERSION = (char)2;
        public List<TimeSlice> TimeSlices = new List<TimeSlice>();
        public Dictionary<int, string> UnitIDToNames = new Dictionary<int, string>();
        public List<string> RaidMembers = new List<string>();
        public DateTime StartDateTime = DateTime.MinValue;
        public int StartTime = 0;
        public int StartServerTime;
        public string Realm = "Unknown";
        public string Player = "Unknown";
        public string AddonVersion = "1.0";
        public class RaidIDEntry
        {
            public int RaidID;
            public DateTime RaidResetDate;
            public DateTime LastSeen;
        }
        public Dictionary<string, List<RaidIDEntry>> RaidIDData = new Dictionary<string, List<RaidIDEntry>>();
        public List<Tuple<DateTime, string, List<int>>> BossLoot = new List<Tuple<DateTime, string, List<int>>>();
        public List<Tuple<DateTime, string, int>> PlayerLoot = new List<Tuple<DateTime, string, int>>();
        private bool TryGetIDFromName(string _Name, out int _ID)
        {
            var val = UnitIDToNames.FirstOrDefault((_Value) => _Value.Value == _Name);
            if (val.Equals(default(KeyValuePair<int, string>)) == false)
            {
                _ID = val.Key;
                return true;
            }
            _ID = 0;
            return false;
        }

        public DamageDataSession()
        { }

        private List<int> GenerateFightUnitIDs(string _BossName)
        {
            List<int> fightUnitIDs = new List<int>();
            if (BossInformation.BossParts.ContainsKey(_BossName) == true)
            {
                foreach (var bossPart in BossInformation.BossParts[_BossName])
                {
                    try
                    {
                        fightUnitIDs.Add(UnitIDToNames.First((_Value) => { return _Value.Value == bossPart; }).Key);
                    }
                    catch (Exception)
                    { }
                }
                if (fightUnitIDs.Count == 0)
                    throw new Exception("Not supposed to happen!");
            }
            else
            {
                fightUnitIDs.Add(UnitIDToNames.First((_Value) => { return _Value.Value == _BossName; }).Key);
            }
            return fightUnitIDs;
        }
        public List<FightData> GenerateFightData(bool _SaveTrash = false)//Dictionary<string, string> _InterestingFights)
        {
            //float addonVersion = 1.0f;
            //if (AddonVersion.TryParseFloat(out addonVersion) == false)
            //    addonVersion = 1.0f;

            //if (addonVersion < 1.5f)
            //    throw new Exception("Too old AddonVersion: \"" + AddonVersion + "\"");

            List<FightData> fights = new List<FightData>();

            FightData currTrash = null;
            FightData currFight = null;
            int lastFightTime = 0;
            TimeSlice lastFightTimeSlice = null;
            for (int i = 0; i < TimeSlices.Count; ++i)
            {
                var currTimeSlice = TimeSlices[i];
                if (_SaveTrash == true)
                {
                    if (currTrash == null && currFight == null)
                    {
                        currTrash = new FightData
                        {
                            FightName = "Trash",
                            StartDateTime = StartDateTime.AddSeconds(currTimeSlice.Time - StartTime),
                            Realm = this.Realm,
                            RecordedByPlayer = this.Player,
                            AddonVersion = this.AddonVersion,
                            StartServerTime = StartServerTime + (currTimeSlice.Time - StartTime),
                        };
                    }
                    if (currTrash != null)
                    {
                        currTrash.TimeSlices.Add(currTimeSlice);
                    }
                }
                if (currTimeSlice.Event != "")
                {
                    if (currTimeSlice.IsStartEvent())
                    {
                        string bossName = "";
                        if (currTimeSlice.GetEventBoss(out bossName) == true)
                        {
                            if (currFight == null)
                            {
                                string instanceName = BossInformation.BossFights[bossName];
                                int raidID = -1;
                                DateTime raidResetDateTime = DateTime.MinValue;
                                FetchRelevantRaidID(currTimeSlice, instanceName, ref raidID, ref raidResetDateTime);
                                if (_SaveTrash == true)
                                {
                                    currTrash.FightDuration = (int)(StartDateTime.AddSeconds(currTimeSlice.Time - StartTime) - currTrash.StartDateTime).TotalSeconds;
                                    currTrash.RaidID = raidID;
                                    currTrash.RaidResetDateTime = raidResetDateTime;

                                    if (currTrash.FightDuration > 120 && currTrash.TimeSlices.Count((_Value) => _Value.ChangedUnitDatas.Count > 10) > 10)
                                    {
                                        var trashUnitDatas = currTrash.TimeSlices.Last().GetDeltaUnitDatas(currTrash.TimeSlices.First());
                                        if (trashUnitDatas.Sum((_Value) => _Value.Value.I.Dmg) > 100000)
                                        {
                                            //Save only if enough data in it
                                            fights.Add(currTrash);
                                        }
                                    }
                                    currTrash = null;
                                }
                                try
                                {
                                    currFight = new FightData { 
                                        FightName = bossName,
                                        m_FightUnitIDs = GenerateFightUnitIDs(bossName),
                                        StartDateTime = StartDateTime.AddSeconds(currTimeSlice.Time - StartTime),
                                        RaidID = raidID,
                                        RaidResetDateTime = raidResetDateTime,
                                        Realm = this.Realm,
                                        RecordedByPlayer = this.Player,
                                        AddonVersion = this.AddonVersion,
                                        StartServerTime = StartServerTime + (currTimeSlice.Time - StartTime),
                                    };
                                    lastFightTimeSlice = null;
                                    //if (i > 0)
                                    //{
                                    //    currFight.TimeSlices.Add(TimeSlices[i - 1]);
                                    //}
                                }
                                catch (Exception)
                                {
                                    currFight = null;
                                }
                            }
                            else
                            {
                                if (currTimeSlice.IsEventBoss(currFight.FightName))
                                {
                                    //Nothing needs to be done, fight is allready started!
                                }
                                else
                                {
                                    var bossUnitIDs = GenerateFightUnitIDs(bossName);
                                    //If a fight is allready ongoing we need to figure out which one has priority, figure out which one contains the most data change over the next 20 timeslices

                                    TimeSlice lastFightData1TimeSlice = null;
                                    TimeSlice lastFightData2TimeSlice = null;
                                    int lastFightData1Score = 0;
                                    int lastFightData2Score = 0;
                                    for (int u = i; u < TimeSlices.Count && u < i + 20; ++u)
                                    {
                                        if (currFight.ContainsThisFight(TimeSlices[u]) == true)
                                        {
                                            if (lastFightData1TimeSlice != null)
                                            {
                                                if (currFight.DetectActivity(lastFightData1TimeSlice, TimeSlices[u]) == true)
                                                {
                                                    ++lastFightData1Score;
                                                }
                                            }
                                            lastFightData1TimeSlice = TimeSlices[u];
                                        }
                                        if (FightData._ContainsThisFight(bossUnitIDs, TimeSlices[u]) == true)
                                        {
                                            if (lastFightData2TimeSlice != null)
                                            {
                                                if (FightData._DetectActivity(bossUnitIDs, lastFightData1TimeSlice, TimeSlices[u]) == true)
                                                {
                                                    ++lastFightData2Score;
                                                }
                                            }
                                            lastFightData2TimeSlice = TimeSlices[u];
                                        }
                                    }

                                    if (bossName == "Ragnaros" && currFight.FightName == "Majordomo Executus")
                                    {
                                        lastFightData2Score += 10;
                                    }

                                    if (lastFightData1Score >= lastFightData2Score)
                                    {
                                        //There is no reason to change since the next 20 timeslices contains more or same amount of info about the current boss
                                    }
                                    else
                                    {
                                        //The current boss has less data in the next 20 timeslices. End current boss and change so the new boss is focused!
                                        //currFight.FightDuration = -1;
                                        //currFight.PerfectSync = false;
                                        //fights.Add(currFight);
                                        currFight = null;

                                        string instanceName = BossInformation.BossFights[bossName];
                                        int raidID = -1;
                                        DateTime raidResetDateTime = DateTime.MinValue;
                                        FetchRelevantRaidID(currTimeSlice, instanceName, ref raidID, ref raidResetDateTime);
                                        try
                                        {
                                            currFight = new FightData
                                            {
                                                FightName = bossName,
                                                m_FightUnitIDs = GenerateFightUnitIDs(bossName),
                                                StartDateTime = StartDateTime.AddSeconds(currTimeSlice.Time - StartTime),
                                                RaidID = raidID,
                                                RaidResetDateTime = raidResetDateTime,
                                                Realm = this.Realm,
                                                RecordedByPlayer = this.Player,
                                                AddonVersion = this.AddonVersion,
                                                StartServerTime = StartServerTime + (currTimeSlice.Time - StartTime),
                                            };
                                            lastFightTimeSlice = null;
                                            if (i > 0)
                                            {
                                                currFight.TimeSlices.Add(TimeSlices[i - 1]);
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            currFight = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (currFight != null && (currTimeSlice.IsDeadEvent() || currTimeSlice.IsWipeEvent()))
                    {
                        if (currTimeSlice.IsEventBoss(currFight.FightName))
                        {
                            bool realDeadEvent = true;
                            if (currFight.FightName == "The Prophet Skeram")
                            {
                                int health;
                                int maxHealth;
                                if (currTimeSlice.IsDeadYellEvent()
                                || (currTimeSlice.GetEventBossHealth("The Prophet Skeram", out health, out maxHealth) && health <= 0 && maxHealth > 500000))
                                {
                                    realDeadEvent = true;
                                }
                                else
                                {
                                    string newEventStr = "";
                                    var eventSplits = currTimeSlice.Event.Split(';');
                                    foreach (var eventSplit in eventSplits)
                                    {
                                        if (eventSplit.StartsWith("Dead") == false)
                                            newEventStr += eventSplit + ";";
                                        else
                                            newEventStr += eventSplit.Replace("Dead", "AddDead") + ";";
                                    }
                                    while (newEventStr.EndsWith(";"))
                                        newEventStr = newEventStr.Substring(0, newEventStr.Length - 1);

                                    currTimeSlice.Event = newEventStr;
                                    realDeadEvent = false;
                                }
                            }
                            if (realDeadEvent == true) //Only end the fight if it truely was a Dead event (not fake one like The Prophet Skeram)
                            {
                                currFight.TimeSlices.Add(currTimeSlice);
                                for (int u = i + 1; u < TimeSlices.Count - 1; ++u)
                                {
                                    if (TimeSlices[u].IsStartEvent()) break;
                                    currFight.TimeSlices.Add(TimeSlices[u]);
                                    if (TimeSlices[u].ChangedUnitDatas.Count < 3 && /*TimeSlices[u + 1].ChangedUnitDatas.Count < 3 &&*/ TimeSlices[u].Time - currTimeSlice.Time > 5)
                                    {
                                        currFight.PerfectSync = true;
                                        break;
                                    }
                                    else if (TimeSlices[u + 1].Time - TimeSlices[u].Time > 15 && TimeSlices[u].Time - currTimeSlice.Time < 30)
                                    {
                                        currFight.PerfectSync = true;
                                        break;
                                    }
                                    int deltaChangedUnitsCount = TimeSlices[u + 1].GetDeltaUnitDatas(TimeSlices[u], true).Count((_Value) => { return _Value.Value.I.Dmg > 1000 || _Value.Value.I.EffHeal > 1000; });
                                    if (deltaChangedUnitsCount < 3 && TimeSlices[u].Time - currTimeSlice.Time > 5)
                                    {
                                        currFight.PerfectSync = true;
                                        break;
                                    }
                                    if (TimeSlices[u].Time - currTimeSlice.Time > 60)
                                    {
                                        currFight.PerfectSync = false;
                                        break;
                                    }
                                }
                                currFight.FightDuration = (int)(StartDateTime.AddSeconds(currTimeSlice.Time - StartTime) - currFight.StartDateTime).TotalSeconds;
                                if (currFight.TimeSlices.Count(
                                    (_Value) =>
                                    {
                                        foreach (var unitID in currFight.FightUnitIDs)
                                        {
                                            if (_Value.ChangedUnitDatas.Contains(unitID) == true)
                                                return true;
                                        }
                                        return false;
                                    }) >= 5)
                                {
                                    fights.Add(currFight);
                                }
                                else
                                {
                                    if (currFight.FightName == "Gothik the Harvester"
                                        || currFight.FightName == "Noth the Plaguebringer"
                                        || currFight.FightName == "Nefarian"
                                        || currFight.FightName == "Razorgore the Untamed"
                                        || currFight.FightName == "C'Thun"
                                        || currFight.FightName == "Kel'Thuzad"
                                        || currFight.FightName == "Ragnaros")
                                    {
                                        //Give these fights a second chance to since they do not have the Boss included in the entire fight, check all the adds
                                        var bossAdds = BossInformation.BossAdds[currFight.FightName];
                                        List<int> bossIDs = new List<int>();
                                        bossIDs.AddRange(currFight.FightUnitIDs);
                                        foreach (var bossAdd in bossAdds)
                                        {
                                            int bossAddID = 0;
                                            if (TryGetIDFromName(bossAdd, out bossAddID) == true)
                                            {
                                                bossIDs.Add(bossAddID);
                                            }
                                        }
                                        if (currFight.TimeSlices.Count(
                                            (_Value) =>
                                            {
                                                foreach (var unitID in bossIDs)
                                                {
                                                    if (_Value.ChangedUnitDatas.Contains(unitID) == true)
                                                        return true;
                                                }
                                                return false;
                                            }) >= 10)
                                        {
                                            fights.Add(currFight);
                                        }
                                    }
                                }
                                currFight = null;
                            }
                        }
                    }
                    else if (currFight != null && lastFightTimeSlice != null && currTimeSlice.Time - lastFightTime > 60)
                    {
                        //If the fighting "boss" has not been seen on SWStats for longer than 60 seconds it most likely means there was a wipe

                        //Unless the bossfight is Gothik the Harvester! 
                        //then we recalculate to see if any add has been changed the last 60 seconds
                        if (currFight.FightName == "Gothik the Harvester"
                            || currFight.FightName == "Noth the Plaguebringer"
                            || currFight.FightName == "Nefarian"
                            || currFight.FightName == "Razorgore the Untamed"
                            || currFight.FightName == "C'Thun"
                            || currFight.FightName == "Kel'Thuzad"
                            || currFight.FightName == "Ragnaros")
                        {
                            //Check if adds have been active the last 60 seconds, if so extend the lastFightTime!
                            var bossAdds = BossInformation.BossAdds[currFight.FightName];
                            List<Tuple<int, UnitData>> bossAddIDs = new List<Tuple<int, UnitData>>();
                            foreach (var bossAdd in bossAdds)
                            {
                                int bossAddID = 0;
                                if (TryGetIDFromName(bossAdd, out bossAddID) == true)
                                {
                                    UnitData bossAddUnitData = null;
                                    if (currTimeSlice.UnitDatas.TryGetValue(bossAddID, out bossAddUnitData) == true)
                                        bossAddIDs.Add(Tuple.Create(bossAddID, bossAddUnitData));
                                }
                            }
                            for (int u = i - 1; u >= 0; --u)
                            {
                                if (currTimeSlice.Time - TimeSlices[u].Time > 60)
                                    break;
                                bool foundAction = false;
                                foreach (var bossAdd in bossAddIDs)
                                {
                                    UnitData bossAddUnitData = null;
                                    if (TimeSlices[u].UnitDatas.TryGetValue(bossAdd.Item1, out bossAddUnitData) == true)
                                    {
                                        if (bossAdd.Item2.I.Dmg - bossAddUnitData.I.Dmg != 0
                                        || bossAdd.Item2.I.DmgTaken - bossAddUnitData.I.DmgTaken != 0
                                        || bossAdd.Item2.I.Death - bossAddUnitData.I.Death != 0)
                                        {
                                            lastFightTime = TimeSlices[u].Time;
                                            foundAction = true;
                                            break;
                                        }
                                    }
                                }
                                if (foundAction == true)
                                    break;
                            }
                        }
                        if (currTimeSlice.Time - lastFightTime > 60)//Double check incase it changed above
                        {
                            currFight.FightDuration = (int)(StartDateTime.AddSeconds(lastFightTime - StartTime) - currFight.StartDateTime).TotalSeconds;
                            if (currFight.FightDuration > 30)
                            {//Only care about fights that lasts longer than 30 seconds
                                currFight.PerfectSync = false;
                                fights.Add(currFight);
                            }
                            currFight = null;
                        }
                    }
                }
                if (currFight != null)
                {
                    if (currFight.ContainsThisFight(currTimeSlice) == true)
                    {
                        if (lastFightTimeSlice == null || currFight.DetectActivity(lastFightTimeSlice, currTimeSlice) == true)
                        {
                            lastFightTime = currTimeSlice.Time;
                        }
                        lastFightTimeSlice = currTimeSlice;
                    }

                    currFight.TimeSlices.Add(currTimeSlice);
                }
            }
            foreach (var fight in fights)
            {
                fight.RemoveUnnecessaryUnits();
            }
            return fights;
        }

        private void FetchRelevantRaidID(TimeSlice currTimeSlice, string instanceName, ref int raidID, ref DateTime raidResetDateTime)
        {
            if (RaidIDData.ContainsKey(instanceName))
            {
                if (RaidIDData[instanceName].Count == 1)
                {
                    raidID = RaidIDData[instanceName].First().RaidID;
                    raidResetDateTime = RaidIDData[instanceName].First().RaidResetDate;
                }
                else
                {
                    int raidIDEntryIndex = 0;
                    while (raidIDEntryIndex < RaidIDData[instanceName].Count - 1)
                    {
                        if (RaidIDData[instanceName][raidIDEntryIndex].LastSeen > StartDateTime.AddSeconds(currTimeSlice.Time - StartTime))
                        {
                            //Vi har hittat den entryn vi letar efter
                            break;
                        }
                        ++raidIDEntryIndex;
                    }
                    raidID = RaidIDData[instanceName][raidIDEntryIndex].RaidID;
                    raidResetDateTime = RaidIDData[instanceName][raidIDEntryIndex].RaidResetDate;
                }
            }
        }
    }
}
