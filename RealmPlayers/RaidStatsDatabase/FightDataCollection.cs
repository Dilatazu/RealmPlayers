using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace VF_RaidDamageDatabase
{
    [ProtoContract]
    [Serializable]
    public class FightDataCollection : ISerializable
    {
        public class FightCacheData
        {
            public FightDataCollection m_FightDataCollection;
            public FightData m_Fight;
            public FightCacheData(FightData _Fight, FightDataCollection _FightDataCollection)
            {
                m_Fight = _Fight;
                m_FightDataCollection = _FightDataCollection;
            }
            public bool IsOverlapping(FightCacheData _FightCacheData)
            {
                var aFightStart = m_Fight.StartDateTime;
                var aFightEnd = m_Fight.GetEndDateTime();
                var bFightStart = _FightCacheData.m_Fight.StartDateTime;
                var bFightEnd = _FightCacheData.m_Fight.GetEndDateTime();

                return (aFightStart >= bFightStart && aFightStart <= bFightEnd) || (aFightEnd >= bFightStart && aFightEnd <= bFightEnd)
                    || (bFightStart >= aFightStart && bFightStart <= aFightEnd) || (bFightEnd >= aFightStart && bFightEnd <= aFightEnd);
            }
            public bool IsBetterVersionOf(FightCacheData _FightCacheData)
            {
                var aFightStart = m_Fight.StartDateTime;
                var aFightEnd = m_Fight.GetEndDateTime();
                var aFightDuration = m_Fight.GetFightRecordDuration();
                var bFightStart = _FightCacheData.m_Fight.StartDateTime;
                var bFightEnd = _FightCacheData.m_Fight.GetEndDateTime();
                var bFightDuration = _FightCacheData.m_Fight.GetFightRecordDuration();

                if (aFightDuration > bFightDuration || aFightStart.AddSeconds(5) < bFightStart)
                    return true;
                else
                    return false;
            }
            //public List<Tuple<string, UnitData>> GetUnitsDataList(bool _MergePetData, int _TimeStart = -1, int _TimeEnd = -1)
            //{
            //    var unitsDataList = m_Fight.GenerateUnitsDataList(
            //        m_Fight.GenerateInterestingUnits(2, m_FightDataCollection.m_UnitIDToNames)
            //        , m_FightDataCollection.m_UnitIDToNames, _TimeStart, _TimeEnd);

            //    if (_MergePetData == true)
            //    {
            //        foreach (var unitData in unitsDataList)
            //        {
            //            if (unitData.Item1.StartsWith("VF_PET_"))
            //            {
            //                string owner = unitData.Item1.Split('_').Last();
            //                int unitOwnerIndex = unitsDataList.FindIndex((_Value) => { return _Value.Item1 == owner; });
            //                if (unitOwnerIndex != -1)
            //                {
            //                    unitsDataList[unitOwnerIndex].Item2.AddPetDataAndClearPet(unitData.Item2);
            //                }
            //            }
            //        }
            //    }

            //    return unitsDataList;
            //}
            public List<string> GetAttendingUnits(ReadOnlyCollection<Tuple<string, UnitData>> _UnitData, Func<string, bool> _Predicate = null)
            {
                List<string> attendingUnits = new List<string>();
                foreach (var unit in _UnitData)
                {
                    if (unit.Item2.Death > 0 || unit.Item2.Dmg > 0 || unit.Item2.RawHeal > 0)
                    {
                        if (unit.Item1 == "Unknown")
                            continue;
                        if (_Predicate == null || _Predicate(unit.Item1))
                            attendingUnits.Add(unit.Item1);
                    }
                }
                return attendingUnits;
            }
            public string GetUnitName(UnitData _Unit)
            {
                string unitName = "";
                if (m_FightDataCollection.m_UnitIDToNames.TryGetValue(_Unit.UnitID, out unitName) == true)
                    return unitName;
                else
                    return "Unknown";
            }
            public bool GetUnitName(UnitData _Unit, out string _UnitName)
            {
                return m_FightDataCollection.m_UnitIDToNames.TryGetValue(_Unit.UnitID, out _UnitName);
            }
            public UnitData GetUnitData(string _Unit)
            {
                var unitsDataList = m_Fight.GenerateUnitsDataList(m_Fight.GenerateInterestingUnits(2, m_FightDataCollection.m_UnitIDToNames), m_FightDataCollection.m_UnitIDToNames);
                int index = unitsDataList.FindIndex((_Value) => { return _Value.Item1 == _Unit; });
                if (index == -1)
                    return null;
                else
                    return unitsDataList[index].Item2;
            }
            public int GetBossPlusAddsDmgTaken()
            {
                return GetBossPlusAddsDmgTaken(null);
            }
            public int GetBossPlusAddsDmgTaken(List<Tuple<string, int>> _RetDmgTakenList)
            {
                var bossUnitData = GetUnitData(m_Fight.FightName);
                int bossPlusAddsDmgTaken = 0;
                if (bossUnitData != null)
                {
                    bossPlusAddsDmgTaken = bossUnitData.DmgTaken;
                    if (_RetDmgTakenList != null)
                        _RetDmgTakenList.Add(new Tuple<string, int>(m_Fight.FightName, bossUnitData.DmgTaken));
                }

                string[] bossAdds = null;
                if (BossInformation.BossAdds.TryGetValue(m_Fight.FightName, out bossAdds) == true)
                {
                    foreach (var bossAdd in bossAdds)
                    {
                        var bossAddUnitData = GetUnitData(bossAdd);
                        if (bossAddUnitData != null)
                        {
                            bossPlusAddsDmgTaken += bossAddUnitData.DmgTaken;
                            if (_RetDmgTakenList != null)
                                _RetDmgTakenList.Add(new Tuple<string, int>(bossAdd, bossAddUnitData.DmgTaken));
                        }
                    }
                }
                return bossPlusAddsDmgTaken;
            }
            //public double GetTotal(Func<VF_RaidDamageDatabase.UnitData, double> _GetValue
            //    , Func<Tuple<string, VF_RaidDamageDatabase.UnitData>, bool> _ValidCheck)
            //{
            //    double totalValue;
            //    double maxValue;
            //    if (GetTotalAndMax(_GetValue, _ValidCheck, out totalValue, out maxValue))
            //        return totalValue;
            //    else
            //        return 0.0;
            //}
            //public bool GetTotal(Func<VF_RaidDamageDatabase.UnitData, double> _GetValue
            //    , Func<Tuple<string, VF_RaidDamageDatabase.UnitData>, bool> _ValidCheck
            //    , out double _TotalValue)
            //{
            //    double maxValue;
            //    return GetTotalAndMax(_GetValue, _ValidCheck, out _TotalValue, out maxValue);
            //}
            //public bool GetTotal(Func<VF_RaidDamageDatabase.UnitData, double> _GetValue
            //    , Func<string, bool> _NameValidCheck
            //    , Func<VF_RaidDamageDatabase.UnitData, bool> _ValueValidCheck
            //    , out double _TotalValue)
            //{
            //    double maxValue;
            //    return GetTotalAndMax(_GetValue, _NameValidCheck, _ValueValidCheck, out _TotalValue, out maxValue);
            //}
            //public bool GetTotalAndMax(Func<VF_RaidDamageDatabase.UnitData, double> _GetValue
            //    , Func<Tuple<string, VF_RaidDamageDatabase.UnitData>, bool> _ValidCheck
            //    , out double _TotalValue, out double _MaxValue)
            //{
            //    var unitData = GetUnitsDataList(true);
            //    return UnitData.CalculateTotalAndMax(unitData, _GetValue, _ValidCheck, out _TotalValue, out _MaxValue);
            //}
            //public bool GetTotalAndMax(Func<UnitData, double> _GetValue
            //    , Func<string, bool> _NameValidCheck
            //    , Func<UnitData, bool> _ValueValidCheck
            //    , out double _TotalValue, out double _MaxValue)
            //{
            //    return GetTotalAndMax(_GetValue, (_Value) => { return _NameValidCheck(_Value.Item1) && _ValueValidCheck(_Value.Item2); }
            //        , out _TotalValue, out _MaxValue);
            //}
        }
        public static char FightDataCollection_VERSION = (char)1;
        List<FightCacheData> m_FightCacheDatas = new List<FightCacheData>();
        [ProtoMember(1)]
        List<FightData> m_FightDatas = new List<FightData>();
        [ProtoMember(2)]
        public Dictionary<int, string> m_UnitIDToNames = new Dictionary<int, string>();
        [ProtoMember(3)]
        public List<string> m_RaidMembers = new List<string>();
        [ProtoMember(4)]
        public List<Tuple<DateTime, string, List<int>>> m_BossLoot = new List<Tuple<DateTime, string, List<int>>>();
        [ProtoMember(5)]
        public List<Tuple<DateTime, string, int>> m_PlayerLoot = new List<Tuple<DateTime, string, int>>();

        public List<FightCacheData> Fights
        {
            get { return m_FightCacheDatas; }
        }
        public bool IsRaidMember(string _Player)
        {
            return m_RaidMembers.Count == 0 || m_RaidMembers.Contains(_Player);
        }

        public string GetNameFromUnitID(int _UnitID)
        {
            if (m_UnitIDToNames.ContainsKey(_UnitID) == false)
                return "Unknown";
            return m_UnitIDToNames[_UnitID];
        }
        public int GetUnitIDFromName(string _Name)
        {
            return m_UnitIDToNames.First((_Value) => { return _Value.Value == _Name; }).Key;
        }

        private int _GenerateUnitID(string _Name)
        {
            int newKey = -1;
            try
            {
                newKey = m_UnitIDToNames.First((_Value) => { return _Value.Value == _Name; }).Key;
            }
            catch (Exception)
            { newKey = -1; }

            if (newKey == -1)
            {
                newKey = m_UnitIDToNames.OrderBy((_Value) => { return _Value.Key; }).Last().Key + 1;
            }
            if (m_UnitIDToNames.ContainsKey(newKey) == true)
            {
                if(m_UnitIDToNames[newKey] != _Name)
                    throw new Exception("This should never happen!");
            }
            else
            {
                m_UnitIDToNames.Add(newKey, _Name);
            }
            return newKey;
        }
        public FightDataCollection()
        { }
        private FightDataCollection(List<DamageDataSession> _DataSessions)//, Dictionary<string, string> _InterestingFights)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            foreach (var dataSession in _DataSessions)
            {
                List<FightData> fights = null;
                try
                {
                    fights = dataSession.GenerateFightData(true);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    fights = dataSession.GenerateFightData();
                }
                m_RaidMembers.AddRangeUnique(dataSession.RaidMembers);
                m_BossLoot.AddRange(dataSession.BossLoot);
                m_PlayerLoot.AddRange(dataSession.PlayerLoot);
                List<TimeSlice> processedTimeSlices1 = new List<TimeSlice>();
                List<TimeSlice> processedTimeSlices2 = new List<TimeSlice>();
                foreach (var fight in fights)
                {
                    Dictionary<int, int> replacingUnitIDs = new Dictionary<int, int>();
                    foreach (var timeSlice in fight.TimeSlices)
                    {
                        if (processedTimeSlices1.Contains(timeSlice) == true)
                        {
                            Logger.ConsoleWriteLine("Skipped processing a timeslice1(" + timeSlice.Time + "), already processed!", ConsoleColor.Yellow);
                            continue;//Do not process a timeslice more than once!
                        }
                        processedTimeSlices1.Add(timeSlice);
                        foreach (var unitData in timeSlice.UnitDatas)
                        {
                            string unitName = dataSession.UnitIDToNames[unitData.Key];
                            if (m_UnitIDToNames.ContainsKey(unitData.Key) == true)
                            {
                                //Globala lookup innehåller keyn redan
                                if (unitName != m_UnitIDToNames[unitData.Key])
                                {
                                    //Namnet är inte samma i globala lookup.
                                    //Skapa nytt m_UnitIDToNames åt oss (_GenerateUnitID) och byt ut vårat ID på alla ställen

                                    if (replacingUnitIDs.ContainsKey(unitData.Key) == false)
                                    {
                                        int newKey = _GenerateUnitID(unitName);
                                        replacingUnitIDs.Add(unitData.Key, newKey);
                                    }
                                }
                            }
                            else
                            {
                                //Key existerar inte redan

                                //Ta reda på om namnet redan existerar i globala lookup
                                var foundName = m_UnitIDToNames.FirstOrDefault((_Value) => _Value.Value == unitName);
                                if (foundName.Equals(default(KeyValuePair<int, string>)) == false)
                                {
                                    //Namnet existerade i globala lookup så vi använder oss utav detta namnets key
                                    if (replacingUnitIDs.ContainsKey(unitData.Key) == false)
                                        replacingUnitIDs.Add(unitData.Key, foundName.Key);
                                    else if (replacingUnitIDs[unitData.Key] != foundName.Key)
                                        throw new Exception("replacingUnitIDs[unitData.Key](" + replacingUnitIDs[unitData.Key] + ") != foundName.Key(" + foundName.Key + "), should never happen!!!");
                                }
                                else
                                {// if (dataSession.UnitIDToNames.ContainsKey(unitData.Key) == true)
                                    //Om inte namnet existerar i globala lookup så roffar vi åt oss namnet
                                    m_UnitIDToNames.Add(unitData.Key, unitName);
                                }
                            }

                            /*
                            if (m_UnitIDToNames.ContainsKey(unitData.Key) == true)
                            {
                                //Globala lookup innehåller keyn redan
                                if (unitName == m_UnitIDToNames[unitData.Key])
                                {
                                    //Namnet är redan samma som i globala lookup, inget behövs göras
                                }
                                else
                                {
                                    //Namnet är inte samma i globala lookup.
                                    //Byt ut så att vi använder globala lookup namnet istället
                                    if (replacingUnitIDs.ContainsKey(unitData.Key) == false)
                                    {
                                        int newKey = _GenerateUnitID(unitName);
                                        replacingUnitIDs.Add(unitData.Key, newKey);
                                    }
                                    else
                                    {
                                        //Utbytet är redan tillagt
                                    }

                                    //var foundName = m_UnitIDToNames.FirstOrDefault((_Value) => _Value.Value == unitName);
                                    //if (foundName.Equals(default(KeyValuePair<int, string>)) == false)
                                    //{
                                    //    if (replacingUnitIDs.ContainsKey(unitData.Key) == false)
                                    //        replacingUnitIDs.Add(unitData.Key, foundName.Key);
                                    //    else if (replacingUnitIDs[unitData.Key] != foundName.Key)
                                    //        throw new Exception("replacingUnitIDs[unitData.Key](" + replacingUnitIDs[unitData.Key] + ") != foundName.Key(" + foundName.Key + "), should never happen!!!");
                                    //}
                                    //else 
                                }
                            }
                            else
                            {
                                //Globala lookup innehåller inte keyn, vi kan antingen lägga till den direkt och säga den är våran
                                //Men eftersom vi vill ha 1-1 mappning så gör vi inte detta.

                                //Ta reda på om namnet redan existerar i den globala lookupen
                                var foundName = m_UnitIDToNames.FirstOrDefault((_Value) => _Value.Value == unitName);
                                if (foundName.Equals(default(KeyValuePair<int, string>)) == false)
                                {
                                    //Namnet existerade i globala lookup så vi använder oss utav detta namnets key
                                    if (replacingUnitIDs.ContainsKey(unitData.Key) == false)
                                        replacingUnitIDs.Add(unitData.Key, foundName.Key);
                                    else if (replacingUnitIDs[unitData.Key] != foundName.Key)
                                        throw new Exception("replacingUnitIDs[unitData.Key](" + replacingUnitIDs[unitData.Key] + ") != foundName.Key(" + foundName.Key + "), should never happen!!!");
                                }
                                else
                                {// if (dataSession.UnitIDToNames.ContainsKey(unitData.Key) == true)
                                    //Om inte namnet existerar i globala lookup så roffar vi åt oss namnet
                                    m_UnitIDToNames.Add(unitData.Key, unitName);
                                }
                            }*/
                        }
                    }
                /*}
                foreach (var fight in fights)
                {*/
                    List<KeyValuePair<int, int>> tempReplacingUnitIDs2 = new List<KeyValuePair<int, int>>(replacingUnitIDs.ToList());
                    List<KeyValuePair<int, int>> sortedReplacingUnitIDs = new List<KeyValuePair<int, int>>();
                    List<KeyValuePair<int, int>> swapUnitIDs = new List<KeyValuePair<int, int>>();
                    while (sortedReplacingUnitIDs.Count + (swapUnitIDs.Count * 2) != replacingUnitIDs.Count)
                    {
                        for (int i = 0; i < tempReplacingUnitIDs2.Count; ++i)
                        {
                            var currIobj = tempReplacingUnitIDs2[i];
                            var indexOfDependency = tempReplacingUnitIDs2.FindIndex((_Value) => _Value.Key == currIobj.Value);
                            if (indexOfDependency != -1)
                            {
                                //Dependency
                                //check if its circular dependency(SWAP TIME)
                                if (tempReplacingUnitIDs2[indexOfDependency].Value == currIobj.Key
                                && tempReplacingUnitIDs2[indexOfDependency].Key == currIobj.Value)
                                {
                                    //Circular dependency, lets swap them instead.
                                    sortedReplacingUnitIDs.Add(currIobj);
                                    sortedReplacingUnitIDs.Add(new KeyValuePair<int, int>(currIobj.Value, currIobj.Key));
                                    if (indexOfDependency > i)
                                    {//Always remove highest index first!
                                        tempReplacingUnitIDs2.RemoveAt(indexOfDependency);
                                        tempReplacingUnitIDs2.RemoveAt(i);
                                        --i;
                                    }
                                    else
                                    {//Always remove highest index first!
                                        tempReplacingUnitIDs2.RemoveAt(i);
                                        tempReplacingUnitIDs2.RemoveAt(indexOfDependency);
                                        i -= 2;
                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {
                                //Inte dependency, placera först!
                                sortedReplacingUnitIDs.Add(currIobj);
                                tempReplacingUnitIDs2.RemoveAt(i);
                                --i;
                            }
                        }
                        if (timer.Elapsed.TotalMinutes > 2)
                            throw new Exception("Timeout - Stuck in infinite ReplaceUnitID loop");
                    }
                    foreach (var replacingUnitID in replacingUnitIDs)
                    {
                        var obj = sortedReplacingUnitIDs.FirstOrDefault((_Value) => _Value.Key == replacingUnitID.Key);
                        if (obj.Equals(default(KeyValuePair<int, int>)) == false)
                        {
                            if (obj.Value != replacingUnitID.Value)
                                throw new Exception(Utility.GetMethodAndLineNumber() + " -> SortedReplacingUnitIDs was not correctly generated!!!");
                        }
                        else
                        {
                            //var obj2 = swapUnitIDs.First((_Value) => _Value.Key == replacingUnitID.Key || _Value.Key == replacingUnitID.Value);
                            //if(obj2.Value == replacingUnitID.Value || obj2.Value == replacingUnitID.Key)
                            //{
                            //    //No probs
                            //}
                            //else
                            {
                                throw new Exception(Utility.GetMethodAndLineNumber() + " -> SwapUnitIDs was not correctly generated!!!");
                            }
                        }
                    }
                    //Correcting the UnitIDs
                    List<int> newFightUnitIDs = new List<int>();
                    foreach (var unitID in fight.FightUnitIDs)
                    {
                        if (replacingUnitIDs.ContainsKey(unitID) == true)
                        {
                            newFightUnitIDs.Add(replacingUnitIDs[unitID]);
                        }
                        else
                        {
                            newFightUnitIDs.Add(unitID);
                        }
                    }
                    fight.m_FightUnitIDs = newFightUnitIDs;

                    //replacingUnitIDs = null;
                    foreach (var timeSlice in fight.TimeSlices)
                    {
                        if (processedTimeSlices2.Contains(timeSlice) == true)
                        {
                            Logger.ConsoleWriteLine("Skipped processing a timeslice2(" + timeSlice.Time + "), already processed!", ConsoleColor.Yellow);
                            continue;//Do not process a timeslice more than once!
                        }
                        processedTimeSlices2.Add(timeSlice);

                        List<int> allreadySwapped = new List<int>();

                        for (int i = 0; i < sortedReplacingUnitIDs.Count; ++i)
                        {
                            var replacingUnitID = sortedReplacingUnitIDs[i];
                            if (timeSlice.UnitDatas.ContainsKey(replacingUnitID.Key) == true)
                            {
                                if (timeSlice.UnitDatas.ContainsKey(replacingUnitID.Value) == false)
                                {
                                    timeSlice.UnitDatas.Add(replacingUnitID.Value, timeSlice.UnitDatas[replacingUnitID.Key]);
                                    timeSlice.UnitDatas.Remove(replacingUnitID.Key);
                                    timeSlice.UnitDatas[replacingUnitID.Value].UnitID = replacingUnitID.Value;
                                    int changedUnitIDIndex = timeSlice.ChangedUnitDatas.FindIndex((_Val) => _Val == replacingUnitID.Key);
                                    if (changedUnitIDIndex != -1)
                                    {
                                        timeSlice.ChangedUnitDatas[changedUnitIDIndex] = replacingUnitID.Value;
                                    }
                                }
                                else
                                {
                                    if (replacingUnitIDs.ContainsKey(replacingUnitID.Value) == true && replacingUnitIDs[replacingUnitID.Value] == replacingUnitID.Key)
                                    {
                                        //Swapp dags om vi inte redan gjort
                                        //throw new Exception("Meen... Hur troligt är det att 2 ska swappas egentligen?...");
                                        if (allreadySwapped.Contains(replacingUnitID.Key) == false)
                                        {
                                            if (allreadySwapped.Contains(replacingUnitID.Value) == true)
                                                throw new Exception(Utility.GetMethodAndLineNumber() + " -> allreadySwapped.Contains(replacingUnitID.Value) == true: Detta borde aldrig kunna ske heller!");
                                            //Swap time!
                                            var temp = timeSlice.UnitDatas[replacingUnitID.Key];
                                            timeSlice.UnitDatas[replacingUnitID.Key] = timeSlice.UnitDatas[replacingUnitID.Value];
                                            timeSlice.UnitDatas[replacingUnitID.Key].UnitID = replacingUnitID.Key;
                                            timeSlice.UnitDatas[replacingUnitID.Value] = temp;
                                            timeSlice.UnitDatas[replacingUnitID.Value].UnitID = replacingUnitID.Value;

                                            int changedUnitIDIndex1 = timeSlice.ChangedUnitDatas.FindIndex((_Val) => _Val == replacingUnitID.Key);
                                            if (changedUnitIDIndex1 != -1)
                                            {
                                                timeSlice.ChangedUnitDatas[changedUnitIDIndex1] = replacingUnitID.Value;
                                            }
                                            int changedUnitIDIndex2 = timeSlice.ChangedUnitDatas.FindIndex((_Val) => _Val == replacingUnitID.Value);
                                            if (changedUnitIDIndex2 != -1)
                                            {
                                                timeSlice.ChangedUnitDatas[changedUnitIDIndex2] = replacingUnitID.Key;
                                            }

                                            allreadySwapped.Add(replacingUnitID.Key);
                                            allreadySwapped.Add(replacingUnitID.Value);
                                        }
                                        else
                                        {
                                            if (allreadySwapped.Contains(replacingUnitID.Key) == false)
                                                throw new Exception(Utility.GetMethodAndLineNumber() + " -> allreadySwapped.Contains(replacingUnitID.Key) == false: Detta borde aldrig kunna ske heller!");
                                        }
                                    }
                                    else
                                    {
                                        //Det var tydligen bara en dependency...

                                    //}
                                    //else
                                    //{
                                        throw new Exception(Utility.GetMethodAndLineNumber() + " -> replacingUnitIDs.ContainsKey(replacingUnitID.Value) == false: Detta borde inte ske!");
                                    }
                                }
                            }
                        }
                    }
                    //Correcting the UnitIDs

                    m_FightCacheDatas.Add(new FightCacheData(fight, this));
                    m_FightDatas.Add(fight);
                }
            }
        }
        public static FightDataCollection GenerateFights(List<DamageDataSession> _DataSessions)//, Dictionary<string, string> _InterestingFights)
        {
            return new FightDataCollection(_DataSessions);//, _InterestingFights);
        }

        #region Serializing
        //[ProtoMember(1)]
        //public List<FightData> _m_FightDatas
        //{
        //    get { return m_FightDatas; }
        //    set { m_FightDatas = value; }
        //}
        //[ProtoMember(2)]
        //public Dictionary<int, string> _m_UnitIDToNames
        //{
        //    get{return m_UnitIDToNames;}
        //    set{m_UnitIDToNames = value;}
        //}
        //public FightDataCollection(SerializationInfo _Info, StreamingContext _Context)
        //{
        //    _m_FightDatas = (List<FightData>)_Info.GetValue("m_FightDatas", typeof(List<FightData>));
        //    m_UnitIDToNames = (Dictionary<int, string>)_Info.GetValue("m_UnitIDToNames", typeof(Dictionary<int, string>));
        //    GenerateFightCacheDatas();
        //}
        [ProtoAfterDeserialization]
        public void GenerateFightCacheDatas()
        {
            foreach (var fight in m_FightDatas)
            {
                m_FightCacheDatas.Add(new FightCacheData(fight, this));
            }
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Version", FightDataCollection_VERSION);
            _Info.AddValue("m_FightDatas", m_FightDatas);
            _Info.AddValue("m_UnitIDToNames", m_UnitIDToNames);
        }
        #endregion
    }
}
