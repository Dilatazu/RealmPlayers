using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;
using System.Collections.ObjectModel;

/*
 * FightDataCollection is the class that contains all parsed data from an uploaded lua database file that was uploaded by one person at one time
 * This data is saved as one file and can thus be loaded separetely whenever needed to access data from a raid that this "fightdatafile" contains.
 * A FightDataCollection can contains multiple RaidInstance clears spread out over several weeks. There is no real limit, 
 * except that it must have all come from the same lua database file.
 * m_FightCacheDatas is a list of FightCacheData, each FightCacheData is basically just a reference to both the FightDataCollection 
 * and specific FightData that is wanted to get accessed. The FightCacheData also contains some nice functions that are generally needed.
 * Do note that m_FightCacheData is generated everytime the FightDataCollection is loaded, all the real data is contained in m_FightDatas which is an array of FightData
 * m_UnitIDToNames is a conversion table for ID to name conversions that is needed to convert IDs within timeslices etc that exists within the FightDatas.
 * m_RaidMembers contains a list of all the players that have been in a raid at the same time as the player during any of the FightDatas.
 * m_BossLoot is the loot that was detected when the player is rightclicking a dead boss corpse and get a loot window. Thus it only contains what items but not who received it.
 * m_PlayerLoot is all the items that was detected as received by any player. 
 * m_BuffIDToNames is a conversion table for ID to buff names that is needed to convert BuffIDs within FightDatas.
 * 
 * FightDataCollection is created by using the function "FightDataCollection.GenerateFights()" which takes an array of DamageDataSessions
 * this basically just calls the constructor which contains all the logic.
 * The constructor loops through all the DamageDataSessions, it makes sure to generate new NameIDs and BuffIDs since they can be different between DamageDataSessions.
 * The "DamageDataSession.GenerateFightData()" function is called for each session, this generates a list of FightData, every FightData is added to the FightDataCollection.
 * After FightData has been generated it also makes sure to translate all the IDs that needs to be translated to ensure a uniform ID conversion. 
 * And it also adds the data such as BossLoot and PlayerLoot to the FightDataCollection so that it contains all relevant data. 
 * DamageDataSessions are not used after this step so everything that should be saved needs to be saved within this FightDataCollection class.
 */

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
                var aFightStart = m_Fight.StartDateTime.AddSeconds(-35);
                var aFightEnd = m_Fight.GetEndDateTime().AddSeconds(35);
                var bFightStart = _FightCacheData.m_Fight.StartDateTime.AddSeconds(-35);
                var bFightEnd = _FightCacheData.m_Fight.GetEndDateTime().AddSeconds(35);

                return m_Fight.FightName == _FightCacheData.m_Fight.FightName && ((aFightStart >= bFightStart && aFightStart <= bFightEnd) || (aFightEnd >= bFightStart && aFightEnd <= bFightEnd)
                    || (bFightStart >= aFightStart && bFightStart <= aFightEnd) || (bFightEnd >= aFightStart && bFightEnd <= aFightEnd));
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
                    if (unit.Item2.I.Death > 0 || unit.Item2.I.Dmg > 0 || unit.Item2.I.RawHeal > 0)
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
                if (m_FightDataCollection.m_UnitIDToNames.TryGetValue(_Unit.I.UnitID, out unitName) == true)
                    return unitName;
                else
                    return "Unknown";
            }
            public bool GetUnitName(UnitData _Unit, out string _UnitName)
            {
                return m_FightDataCollection.m_UnitIDToNames.TryGetValue(_Unit.I.UnitID, out _UnitName);
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
                    bossPlusAddsDmgTaken = bossUnitData.I.DmgTaken;
                    if (_RetDmgTakenList != null)
                        _RetDmgTakenList.Add(new Tuple<string, int>(m_Fight.FightName, bossUnitData.I.DmgTaken));
                }

                string[] bossAdds = null;
                if (BossInformation.BossAdds.TryGetValue(m_Fight.FightName, out bossAdds) == true)
                {
                    foreach (var bossAdd in bossAdds)
                    {
                        var bossAddUnitData = GetUnitData(bossAdd);
                        if (bossAddUnitData != null)
                        {
                            bossPlusAddsDmgTaken += bossAddUnitData.I.DmgTaken;
                            if (_RetDmgTakenList != null)
                                _RetDmgTakenList.Add(new Tuple<string, int>(bossAdd, bossAddUnitData.I.DmgTaken));
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
        [ProtoMember(6)]
        public List<string> m_BuffIDToNames = new List<string>();

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
            List<string> unitIDNames = new List<string>();
            //var timer = System.Diagnostics.Stopwatch.StartNew();
            foreach (var dataSession in _DataSessions)
            {
                if(VF_RealmPlayersDatabase.StaticValues.Disabled_UploadRealmNames.Contains(dataSession.Realm) == true
                || VF_RealmPlayersDatabase.StaticValues.DeadRealms.Contains(VF_RealmPlayersDatabase.StaticValues.ConvertRealm(dataSession.Realm)) == true)
                {
                    if (dataSession.TimeSlices.Count > 20)
                    {
                        //Only log message if there actually is any data here...
                        Logger.ConsoleWriteLine("Realm \"" + dataSession.Realm + "\" is disabled, so not adding fights for session!");
                    }
                    continue;
                }
                else if(dataSession.StartDateTime < DateTime.UtcNow.AddMonths(-1))
                {
                    if(dataSession.TimeSlices.Count > 20)
                    {
                        //Only log message if there actually is any data here...
                        Logger.ConsoleWriteLine("Raid datasession was older than 1 month! Skipping!");
                    }
                    continue;
                }
                Dictionary<int, int> buffIDTranslationTable = new Dictionary<int,int>();
                foreach(var buffID in dataSession.BuffIDToNames)
                {
                    int index = m_BuffIDToNames.IndexOf(buffID.Value);
                    if(index < 0)
                    {
                        index = m_BuffIDToNames.Count;
                        m_BuffIDToNames.Add(buffID.Value);
                    }
                    buffIDTranslationTable.Add(buffID.Key, index);
                }

                Dictionary<int, int> nameIDTranslationTable = new Dictionary<int, int>();
                foreach (var unitID in dataSession.UnitIDToNames)
                {
                    int index = unitIDNames.IndexOf(unitID.Value);
                    if (index < 0)
                    {
                        index = unitIDNames.Count;
                        unitIDNames.Add(unitID.Value);
                        m_UnitIDToNames.Add(index, unitID.Value);
                    }
                    nameIDTranslationTable.Add(unitID.Key, index);
                }

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
                List<TimeSlice> processedTimeSlices = new List<TimeSlice>();
                List<int> skippedTimeSlices = new List<int>();
                foreach (var fight in fights)
                {
                    int TFUDViolations = 0;
                    int TUDViolations = 0;
                    int TCUDViolations = 0;
                    int TGMIDViolations = 0;
                    int TUBBIViolations = 0;
                    int TUBViolations = 0;
                    int TUDBBIViolations = 0;
                    int TUDBViolations = 0;

                    //Correcting the UnitID and BuffIDs
                    List<int> translatedFightUnitIDs = new List<int>();
                    foreach (var unitID in fight.FightUnitIDs)
                    {
                        if (nameIDTranslationTable.ContainsKey(unitID) == true)
                        {
                            translatedFightUnitIDs.Add(nameIDTranslationTable[unitID]);
                        }
                        else
                        {
                            ++TFUDViolations;
                        }
                    }
                    fight.m_FightUnitIDs = translatedFightUnitIDs;
                    
                    foreach (var timeSlice in fight.TimeSlices)
                    {
                        if (processedTimeSlices.Contains(timeSlice) == true)
                        {
                            skippedTimeSlices.Add(timeSlice.Time);
                            continue;//Do not process a timeslice more than once!
                        }
                        processedTimeSlices.Add(timeSlice);

                        //Fixing UnitDatas
                        {
                            Dictionary<int, UnitData> translatedUnitDatas = new Dictionary<int, UnitData>();
                            foreach (var unitData in timeSlice.UnitDatas)
                            {
                                if (nameIDTranslationTable.ContainsKey(unitData.Key) == true)
                                {
                                    int newUnitID = nameIDTranslationTable[unitData.Key];
                                    unitData.Value.I.SetNewUnitID(newUnitID);
                                    translatedUnitDatas.Add(newUnitID, unitData.Value);
                                }
                                else
                                {
                                    ++TUDViolations;
                                }
                            }
                            timeSlice.UnitDatas = translatedUnitDatas;
                        }
                        //Fixing UnitDatas

                        //Fixing ChangedUnitDatas
                        {
                            List<int> translatedChangedUnitDatas = new List<int>();
                            foreach (var changedUnitID in timeSlice.ChangedUnitDatas)
                            {
                                if (nameIDTranslationTable.ContainsKey(changedUnitID) == true)
                                {
                                    translatedChangedUnitDatas.Add(nameIDTranslationTable[changedUnitID]);
                                }
                                else
                                {
                                    ++TCUDViolations;
                                }
                            }
                            timeSlice.ChangedUnitDatas = translatedChangedUnitDatas;
                        }
                        //Fixing ChangedUnitDatas

                        //Fixing GroupMemberIDs
                        if (timeSlice.GroupMemberIDs != null)
                        {
                            List<int> translatedGroupMemberIDs = new List<int>();
                            foreach (var unitID in timeSlice.GroupMemberIDs)
                            {
                                if (nameIDTranslationTable.ContainsKey(unitID) == true)
                                {
                                    translatedGroupMemberIDs.Add(nameIDTranslationTable[unitID]);
                                }
                                else
                                {
                                    ++TGMIDViolations;
                                }
                            }
                            timeSlice.GroupMemberIDs = translatedGroupMemberIDs;
                        }
                        //Fixing GroupMemberIDs

                        //Fixing UnitBuffs
                        if (timeSlice.UnitBuffs != null)
                        {
                            Dictionary<int, List<BuffInfo>> translatedUnitBuffs = new Dictionary<int, List<BuffInfo>>();
                            foreach (var unitBuff in timeSlice.UnitBuffs)
                            {
                                if (nameIDTranslationTable.ContainsKey(unitBuff.Key) == true)
                                {
                                    var translatedBuffInfos = new List<BuffInfo>();
                                    foreach (var buffInfo in unitBuff.Value)
                                    {
                                        if (buffIDTranslationTable.ContainsKey(buffInfo.BuffID) == true)
                                        {
                                            translatedBuffInfos.Add(new BuffInfo { BuffID = buffIDTranslationTable[buffInfo.BuffID], LastUpdatedTimeSlice = buffInfo.LastUpdatedTimeSlice });
                                        }
                                        else
                                        {
                                            ++TUBBIViolations;
                                        }
                                    }
                                    translatedUnitBuffs.Add(nameIDTranslationTable[unitBuff.Key], translatedBuffInfos);
                                }
                                else
                                {
                                    ++TUBViolations;
                                }
                            }
                            timeSlice.UnitBuffs = translatedUnitBuffs;
                        }
                        //Fixing UnitBuffs

                        //Fixing UnitDebuffs
                        if (timeSlice.UnitDebuffs != null)
                        {
                            Dictionary<int, List<BuffInfo>> translatedUnitDebuffs = new Dictionary<int, List<BuffInfo>>();
                            foreach (var unitBuff in timeSlice.UnitDebuffs)
                            {
                                if (nameIDTranslationTable.ContainsKey(unitBuff.Key) == true)
                                {
                                    var translatedBuffInfos = new List<BuffInfo>();
                                    foreach (var buffInfo in unitBuff.Value)
                                    {
                                        if (buffIDTranslationTable.ContainsKey(buffInfo.BuffID) == true)
                                        {
                                            translatedBuffInfos.Add(new BuffInfo { BuffID = buffIDTranslationTable[buffInfo.BuffID], LastUpdatedTimeSlice = buffInfo.LastUpdatedTimeSlice });
                                        }
                                        else
                                        {
                                            ++TUDBBIViolations;
                                        }
                                    }
                                    translatedUnitDebuffs.Add(nameIDTranslationTable[unitBuff.Key], translatedBuffInfos);
                                }
                                else
                                {
                                    ++TUDBViolations;
                                }
                            }
                            timeSlice.UnitDebuffs = translatedUnitDebuffs;
                        }
                        //Fixing UnitDebuffs

                    }
                    //Correcting the UnitID and BuffIDs

                    if (TFUDViolations > 0)
                    {
                        Logger.ConsoleWriteLine("when generating translatedFightUnitIDs: "
                            + TFUDViolations + "x THIS IS SERIOUS AND SHOULD NEVER HAPPEN!!! Fight(" + fight.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.Red);
                    }
                    if (TUDViolations > 0)
                    {
                        Logger.ConsoleWriteLine("when generating translatedUnitDatas: "
                            + TUDViolations + "x THIS IS SERIOUS AND SHOULD NEVER HAPPEN!!! Fight(" + fight.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.Red);
                    }
                    if (TCUDViolations > 0)
                    {
                        Logger.ConsoleWriteLine("when generating translatedChangedUnitDatas: "
                            + TCUDViolations + "x THIS IS SERIOUS AND SHOULD NEVER HAPPEN!!! Fight(" + fight.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.Red);
                    }
                    if (TGMIDViolations > 0)
                    {
                        Logger.ConsoleWriteLine("when generating translatedGroupMemberIDsDatas: "
                            + TGMIDViolations + "x THIS IS SERIOUS AND SHOULD NEVER HAPPEN!!! Fight(" + fight.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.Red);
                    }
                    if (TUBBIViolations > 0)
                    {
                        Logger.ConsoleWriteLine("when generating translatedUnitBuffs.BuffInfo: "
                            + TUBBIViolations + "x THIS IS SERIOUS AND SHOULD NEVER HAPPEN!!! Fight(" + fight.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.Red);
                    }
                    if (TUBViolations > 0)
                    {
                        Logger.ConsoleWriteLine("when generating translatedUnitBuffs: "
                            + TUBViolations + "x THIS IS SERIOUS AND SHOULD NEVER HAPPEN!!! Fight(" + fight.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.Red);
                    }
                    if (TUDBBIViolations > 0)
                    {
                        Logger.ConsoleWriteLine("when generating translatedUnitDebuffs.BuffInfo: "
                            + TUDBBIViolations + "x THIS IS SERIOUS AND SHOULD NEVER HAPPEN!!! Fight(" + fight.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.Red);
                    }
                    if (TUDBViolations > 0)
                    {
                        Logger.ConsoleWriteLine("when generating translatedUnitDebuffs: "
                            + TUDBViolations + "x THIS IS SERIOUS AND SHOULD NEVER HAPPEN!!! Fight(" + fight.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.Red);
                    }

                    m_FightCacheDatas.Add(new FightCacheData(fight, this));
                    m_FightDatas.Add(fight);
                }
                if (skippedTimeSlices.Count > 0)
                {
                    string skipText = "Skipped processing " + skippedTimeSlices.Count + " timeslices: ";
                    foreach (int timeSlice in skippedTimeSlices)
                    {
                        skipText += timeSlice.ToString() + ", ";
                    }
                    Logger.ConsoleWriteLine(skipText + ", already processed!", ConsoleColor.Yellow);
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
