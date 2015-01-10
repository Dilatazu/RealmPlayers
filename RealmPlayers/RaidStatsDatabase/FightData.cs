using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace VF_RaidDamageDatabase
{
    [ProtoContract]
    [Serializable]
    public class FightData : ISerializable
    {
        public static char FightData_VERSION = (char)2;
        [ProtoMember(1)]
        public string FightName = "Unknown";
        [ProtoMember(2)]
        private int FightUnitID = -1;
        [ProtoMember(3)]
        public List<TimeSlice> TimeSlices = new List<TimeSlice>();
        [ProtoMember(4)]
        public DateTime StartDateTime = DateTime.MinValue;
        [ProtoMember(5)]
        public int FightDuration = 0;
        [ProtoMember(6)]
        public bool PerfectSync = false;
        [ProtoMember(7)]
        public int RaidID = -1;
        [ProtoMember(8)]
        public DateTime RaidResetDateTime = DateTime.MinValue;
        [ProtoMember(9)]
        public string Realm = "Emerald Dream";
        [ProtoMember(10)]
        public string RecordedByPlayer = "Unknown";
        [ProtoMember(11)]
        public string AddonVersion = "1.0";
        [ProtoMember(12)]
        public List<int> m_FightUnitIDs = new List<int>();
        [ProtoMember(13)]
        public int StartServerTime = 0;

        public List<int> FightUnitIDs
        {
            get
            {
                if (m_FightUnitIDs.Count == 0 && FightUnitID != -1)
                    m_FightUnitIDs = new List<int>(new int[] { FightUnitID });
                return m_FightUnitIDs;
            }
        }

        public static bool _DetectActivity(List<int> _FightUnitIDs, TimeSlice _TimeSlice1, TimeSlice _TimeSlice2)
        {
            foreach (var unitID in _FightUnitIDs)
            {
                UnitData lastFightUnitData = null;
                if (_TimeSlice1.UnitDatas.TryGetValue(unitID, out lastFightUnitData) == false)
                    lastFightUnitData = null;

                UnitData thisFightUnitData = null;
                if (_TimeSlice2.UnitDatas.TryGetValue(unitID, out thisFightUnitData) == false)
                    thisFightUnitData = null;

                if (lastFightUnitData == null)
                {
                    if (thisFightUnitData != null)
                        return true;
                    else
                        continue;
                }
                else
                {
                    if (thisFightUnitData == null)
                        continue;
                    else
                    {
                        //Both are not null
                        if (thisFightUnitData.I.Dmg - lastFightUnitData.I.Dmg != 0
                        || thisFightUnitData.I.DmgTaken - lastFightUnitData.I.DmgTaken != 0
                        || thisFightUnitData.I.Death - lastFightUnitData.I.Death != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool _ContainsThisFight(List<int> _FightUnitIDs, TimeSlice _TimeSlice)
        {
            foreach (var unitID in _FightUnitIDs)
            {
                if (_TimeSlice.UnitDatas.ContainsKey(unitID))
                    return true;
            }
            return false;
        }
        public bool DetectActivity(TimeSlice _TimeSlice1, TimeSlice _TimeSlice2)
        {
            return _DetectActivity(FightUnitIDs, _TimeSlice1, _TimeSlice2);
        }
        public bool ContainsThisFight(TimeSlice _TimeSlice)
        {
            return _ContainsThisFight(FightUnitIDs, _TimeSlice);
        }

        public FightData()
        { }

        public bool HasResetsMidFight()
        {
            bool foundStart = false;
            foreach (var timeSlice in TimeSlices)
            {
                if (foundStart == false && timeSlice.IsStartEvent())
                    foundStart = true;

                if (foundStart && timeSlice.IsResetEvent())
                    return true;
                else if (timeSlice.IsDeadEvent() && timeSlice.IsEventBoss(FightName))
                    break;
            }
            return false;
        }
        public DateTime GetEndDateTime()
        {
            return StartDateTime.AddSeconds(FightDuration);
        }
        public int GetFightRecordDuration()
        {
            if (PerfectSync == true)
                return FightDuration;
            else
            {
                return TimeSlices.Last().Time - TimeSlices.First().Time;
            }
        }

        public List<Tuple<string, UnitData>> GenerateUnitsDataList(List<string> _InterestingUnits, Dictionary<int, string> _UnitIDToNames, int _StartTime = -1, int _EndTime = -1)
        {
            if (_StartTime > _EndTime)
                _EndTime = -1;

            int firstTimeSliceTime = 0;
            if (_StartTime != -1 || _EndTime != -1)
                firstTimeSliceTime = TimeSlices.First().Time;

            if (_EndTime == -1)
                _EndTime = int.MaxValue;
            List<Tuple<string, UnitData>> unitsData = new List<Tuple<string, UnitData>>();
            foreach(string unitName in _InterestingUnits)
            {
                try
                {
                    int unitID = _UnitIDToNames.First((_Value) => { return _Value.Value == unitName; }).Key;
                    UnitData startUnitData = null;
                    foreach (var timeSlice in TimeSlices)
                    {
                        if ((timeSlice.Time - firstTimeSliceTime) >= _StartTime && timeSlice.UnitDatas.ContainsKey(unitID) == true)
                        {
                            startUnitData = timeSlice.UnitDatas[unitID];
                            break;
                        }
                    }
                    //var petIDs = _UnitIDToNames.Where((_Value) => { 
                    //    if(_Value.Value.StartsWith("VF_PET_"))
                    //    {
                    //        if (_Value.Value.Split('_').Last() == unitName)
                    //            return true;
                    //        else
                    //            return false;
                    //    }
                    //    return false;
                    //});
                    if (startUnitData != null)
                    {
                        UnitData endUnitData = null;
                        for (int i = TimeSlices.Count - 1; i >= 0; --i)
                        {
                            var timeSlice = TimeSlices[i];
                            if ((timeSlice.Time - firstTimeSliceTime) <= _EndTime && timeSlice.UnitDatas.ContainsKey(unitID) == true)
                            {
                                endUnitData = timeSlice.UnitDatas[unitID];
                                break;
                            }
                        }
                        int deadTimeSliceIndex = TimeSlices.FindIndex((_TimeSlice) => { return _TimeSlice.Event == "Dead " + FightName || _TimeSlice.Event == "Wipe " + FightName; });

                        int highestThreat = 0;
                        if (deadTimeSliceIndex >= 0)
                        {
                            int deadTimeSliceTime = TimeSlices[deadTimeSliceIndex].Time;
                            for (int i = deadTimeSliceIndex; i >= 0; --i)
                            {
                                if((TimeSlices[i].Time - firstTimeSliceTime) > _EndTime)
                                {
                                    deadTimeSliceTime = TimeSlices[i].Time;
                                    continue;
                                } 
                                if (Math.Abs(deadTimeSliceTime - TimeSlices[i].Time) > 15)
                                    break;
                                if (TimeSlices[i].UnitDatas.ContainsKey(unitID) == true)
                                {
                                    if (TimeSlices[i].UnitDatas[unitID].I.ThreatValue > highestThreat)
                                        highestThreat = TimeSlices[i].UnitDatas[unitID].I.ThreatValue;
                                }
                            }
                        }
                        if (endUnitData != null)
                        {
                            try
                            {
                                UnitData totalUnitData = UnitData.CreateDifference(startUnitData, endUnitData);
                                if (highestThreat > 0)
                                    totalUnitData.I.SetNewThreatValue(highestThreat);
                                //string unitName = m_DataSession.UnitIDToNames[unitID];
                                unitsData.Add(new Tuple<string, UnitData>(unitName, totalUnitData));
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
                catch (Exception)
                {}
            }
            return unitsData;
        }
        public List<string> GenerateInterestingUnits(int _TimeSliceThreshold, Dictionary<int, string> _UnitIDToNames)
        {
            Dictionary<int, int> interestingUnits = new Dictionary<int, int>();
            foreach (var timeSlice in TimeSlices)
            {
                foreach(var unitData in timeSlice.UnitDatas)
                {
                    //if (unitData.Key == FightUnitID)
                    //    continue;
                    if (interestingUnits.ContainsKey(unitData.Key) == false)
                        interestingUnits.Add(unitData.Key, 0);
                    interestingUnits[unitData.Key] = interestingUnits[unitData.Key] + 1;
                }
            }
            List<string> result = new List<string>();
            foreach (var unit in interestingUnits)
            {
                if (unit.Value >= _TimeSliceThreshold)
                {
                    if(_UnitIDToNames.ContainsKey(unit.Key))
                        result.Add(_UnitIDToNames[unit.Key]);
                }
            }
            return result;
        }
        public void RemoveUnnecessaryUnits()
        {
            Dictionary<int, bool> unitsInFight = new Dictionary<int,bool>();
            foreach (var timeSlice in TimeSlices)
            {
                foreach (var unit in timeSlice.ChangedUnitDatas)
                {
                    if (unitsInFight.ContainsKey(unit) == false)
                        unitsInFight.Add(unit, true);
                }
            }
            if (unitsInFight.Count > 0)
            {
                foreach (var timeSlice in TimeSlices)
                {
                    timeSlice.UnitDatas.RemoveKeys((_Key) => { return unitsInFight.ContainsKey(_Key) == false; });
                }
            }
        }
        public enum AttemptType
        {
            WipeAttempt,
            KillAttempt,
            UnknownAttempt,
            TrashAttempt,
        }
        public AttemptType GetAttemptType()
        {
            if (FightName == "Trash")
                return AttemptType.TrashAttempt;
            string deadBossString = "Dead " + FightName;
            string wipeBossString = "Wipe " + FightName;
            for (int i = TimeSlices.Count - 1; i >= 0; --i)
            {
                if (TimeSlices[i].Event.StartsWith("Dead"))
                    return AttemptType.KillAttempt;
                else if (TimeSlices[i].Event.StartsWith("Wipe"))
                    return AttemptType.WipeAttempt;
            }
            return AttemptType.UnknownAttempt;
        }
        #region Serializing
        public FightData(SerializationInfo _Info, StreamingContext _Context)
        {
            char version = _Info.GetChar("Version");
            FightName = _Info.GetString("FightName");
            FightUnitID = _Info.GetInt32("FightUnitID");
            TimeSlices = (List<TimeSlice>)_Info.GetValue("TimeSlices", typeof(List<TimeSlice>));
            StartDateTime = _Info.GetDateTime("StartDateTime");
            FightDuration = _Info.GetInt32("FightDuration");
            PerfectSync = _Info.GetBoolean("PerfectSync");
            if (version == 2)
            {
                RaidID = _Info.GetInt32("RaidID");
                RaidResetDateTime = _Info.GetDateTime("RaidResetDateTime");
            }
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Version", FightData_VERSION);
            _Info.AddValue("FightName", FightName);
            _Info.AddValue("FightUnitID", FightUnitID);
            _Info.AddValue("TimeSlices", TimeSlices);
            _Info.AddValue("StartDateTime", StartDateTime);
            _Info.AddValue("FightDuration", FightDuration);
            _Info.AddValue("PerfectSync", PerfectSync);
            _Info.AddValue("RaidID", RaidID);
            _Info.AddValue("RaidResetDateTime", RaidResetDateTime);
        }
        #endregion
    }
}
