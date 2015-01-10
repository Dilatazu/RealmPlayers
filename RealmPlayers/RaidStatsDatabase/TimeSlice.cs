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
    public class TimeSlice : ISerializable
    {
        public static char TimeSlice_VERSION = (char)2;
        [ProtoMember(1)]
        public int Time;
        [ProtoMember(2)]
        public string Event;
        [ProtoMember(3)]
        public string Zone;
        [ProtoMember(4)]
        public Dictionary<int, UnitData> UnitDatas = new Dictionary<int, UnitData>();
        [ProtoMember(5)]
        public List<int> ChangedUnitDatas = new List<int>();

        public bool IsEvent(string _EventType)
        {
            var eventSplits = Event.Split(';');
            foreach (string e in eventSplits)
            {
                if (e.StartsWith(_EventType))
                    return true;
            }
            return false;
        }
        public string[] GetRawEvents()
        {
            return Event.Split(';');
        }
        public bool IsStartEvent()
        {
            return IsEvent("Start");
        }
        public bool IsDeadEvent()
        {
            return IsEvent("Dead");
        }
        public bool IsDeadYellEvent()
        {
            return IsEvent("Dead_Y");
        }
        public bool IsWipeEvent()
        {
            return IsEvent("Wipe");
        }
        public bool IsPhaseEvent()
        {
            return IsEvent("Phase");
        }
        public bool IsBossHealthEvent()
        {
            return IsEvent("BossHealth");
        }
        public bool IsResetEvent()
        {
            return IsEvent("SWReset") || IsEvent("RCReset");
        }
        public bool GetEventBoss(out string _BossName)
        {
            string firstBossName = "";
            var eventSplits = Event.Split(';');
            foreach (string e in eventSplits)
            {
                string currBoss = "";
                if (e.Contains('='))
                    currBoss = e.Split('=').Last();
                else if (e.Contains(' '))
                    currBoss = e.Substring(e.IndexOf(' ') + 1);
                    
                if(firstBossName == "")
                    firstBossName = currBoss;
                if (BossInformation.BossFights.ContainsKey(currBoss))
                {
                    _BossName = currBoss;
                    return true;
                }
            }
            _BossName = firstBossName;
            return false; 
        }
        //{
        //    string firstBossName = "";
        //    var eventSplits = Event.Split(';');
        //    foreach (string e in eventSplits)
        //    {
        //        string currBoss = "";
        //        if (e.Contains('='))
        //            currBoss = e.Split('=').Last();
        //        else if (e.Contains(' '))
        //            currBoss = e.Substring(e.IndexOf(' ') + 1);
                    
        //        if(firstBossName == "")
        //            firstBossName = currBoss;
        //        if(BossInformation.BossFights.ContainsKey(currBoss))
        //            return currBoss;
        //    }
        //    return firstBossName;
        //}
        public string GetEventPhase()
        {
            var eventSplits = Event.Split(';');
            foreach (string e in eventSplits)
            {
                string currPhase = "";
                if (e.Contains('='))
                    currPhase = e.Split('=').First();
                else if (e.Contains(' '))
                    currPhase = e.Split(' ').First();

                if (currPhase.StartsWith("Phase"))
                    return currPhase;
            }
            return "";
        }
        //public bool GetEventBossHealth(out int _Health, out int _MaxHealth)
        //{
        //    try
        //    {
        //        string bossHealth = GetEventBossHealth();
        //        if (bossHealth != "")
        //        {
        //            string[] bossHealthAndMax = bossHealth.Split('-');
        //            _Health = int.Parse(bossHealthAndMax[0]);
        //            _MaxHealth = int.Parse(bossHealthAndMax[1]);
        //            return true;
        //        }
        //    }
        //    catch (Exception)
        //    {}
        //    _Health = -1;
        //    _MaxHealth = -1;
        //    return false;
        //}

        public float GetTotalBossPercentage(string _BossName, bool _StrictBossName = false)
        {
            if (_StrictBossName == false && BossInformation.BossParts.ContainsKey(_BossName) == true)
            {
                float totalBossPercentage = 0.0f;
                var bossNames = BossInformation.BossParts[_BossName];
                foreach (var bossName in bossNames)
                {
                    int health;
                    int maxHealth;
                    if(GetEventBossHealth(bossName, out health, out maxHealth) == true)
                    {
                        totalBossPercentage += (float)(((double)health) / ((double)maxHealth)) / bossNames.Length;
                    }
                }
                return totalBossPercentage;
            }
            else
            {
                int health;
                int maxHealth;
                if (GetEventBossHealth(_BossName, out health, out maxHealth) == false)
                {
                    if (_StrictBossName == true)
                        return 0.0f;

                    var bossHealth = GetEventBossHealth();
                    if (bossHealth == "")
                        return 0.0f;

                    try
                    {
                        var bossHealthSplit = bossHealth.Split('-');
                        health = int.Parse(bossHealthSplit[0]);
                        maxHealth = int.Parse(bossHealthSplit[1]);
                    }
                    catch (Exception)
                    {
                        health = 0;
                        maxHealth = 1;
                    }
                }

                return (float)(((double)health) / ((double)maxHealth));
            }
        }
        public bool GetEventBossHealth(string _BossName, out int _Health, out int _MaxHealth)
        {
            try
            {
                string bossHealth = GetEventBossHealth(_BossName);
                if (bossHealth != "")
                {
                    string[] bossHealthAndMax = bossHealth.Split('-');
                    _Health = int.Parse(bossHealthAndMax[0]);
                    _MaxHealth = int.Parse(bossHealthAndMax[1]);
                    return true;
                }
            }
            catch (Exception)
            { }
            _Health = -1;
            _MaxHealth = -1;
            return false;
        }
        public bool GetEventBossHealthPercentage(string _BossName, out double _HealthPercentage)
        {
            int health;
            int maxHealth;
            if (GetEventBossHealth(_BossName, out health, out maxHealth) == true)
            {
                if (maxHealth != 0)
                {
                    _HealthPercentage = ((double)health) / ((double)maxHealth);
                    return true;
                }
            }
            _HealthPercentage = 0.0;
            return false;
        }
        //public bool GetEventBossHealthPercentage(out double _HealthPercentage)
        //{
        //    int health;
        //    int maxHealth;
        //    if (GetEventBossHealth(out health, out maxHealth) == true)
        //    {
        //        if (maxHealth != 0)
        //        {
        //            _HealthPercentage = ((double)health) / ((double)maxHealth);
        //            return true;
        //        }
        //    }
        //    _HealthPercentage = 0.0;
        //    return false;
        //}
        public string GetEventBossHealth(string _BossName)
        {
            var eventSplits = Event.Split(';');
            foreach (string e in eventSplits)
            {
                if (e.StartsWith("BossHealth-" + _BossName))
                    return e.Split('=').Last();
            }
            return "";
        }
        public string GetEventBossHealth()
        {
            var eventSplits = Event.Split(';');
            foreach (string e in eventSplits)
            {
                if (e.StartsWith("BossHealth"))
                    return e.Split('=').Last();
            }
            return "";
        }
        public bool IsEventBoss(string _BossName)
        {
            var eventSplits = Event.Split(';');
            foreach (string e in eventSplits)
            {
                string currBoss = "";
                if (e.Contains('='))
                    currBoss = e.Split('=').Last();
                else if (e.Contains(' '))
                    currBoss = e.Substring(e.IndexOf(' ') + 1);
                    
                if (currBoss == _BossName)
                    return true;
            }
            return false;
        }

        public TimeSlice()
        { }

        public TimeSlice(TimeSlice _PreviousTimeSlice, string _DataString, Dictionary<int, string> _UnitIDsToName, List<int> _RaidMemberIDs, string _Zone)
        {
            Dictionary<int, UnitData> previousUnitDatas = null;
            if(_PreviousTimeSlice != null)
                previousUnitDatas = _PreviousTimeSlice.UnitDatas;

            Zone = _Zone;
            string[] splitData = _DataString.Split(',');

            string[] headerAndFirst = splitData[0].Split(':');

            Time = int.Parse(headerAndFirst[0]);
            Event = headerAndFirst[1];
            //if (Event.Contains("Sir Zeliek"))
            //    Event = Event.Replace("Sir Zeliek", "The Four Horsemen");
            //else if (Event.Contains("Highlord Mograine"))
            //    Event = Event.Replace("Highlord Mograine", "The Four Horsemen");

            splitData[0] = headerAndFirst[2];

            foreach (string unitData in splitData)
            {
                if (unitData != "")
                {
                    if (unitData.Contains('='))
                    {//ID to Name convertion
                        //try
                        {
                            string[] idToNameData = unitData.Split('=');
                            string unitName = idToNameData[0];
                            int unitID = int.Parse(idToNameData[1]);
                            if (_UnitIDsToName.ContainsKey(unitID) == false)
                                _UnitIDsToName.Add(unitID, unitName);
                            else if(unitName.StartsWith("VF_PET_"))
                                _UnitIDsToName[unitID] = unitName;
                        }
                        //catch (Exception)
                        {}
                    }
                    else if(unitData.StartsWith("R"))
                    {
                        try 
	                    {	   
                            //Raid Definition for 1.8.3, TODO: Implement parsing!
                            //unitData = "R 32 92 82 71 80 93 2 3 492 1" <-- numbers are UnitIDs
                            var raidMembers = unitData.Split(' ');
                            for (int i = 1; i < raidMembers.Length; ++i)
                            {//Start from 1, skip "R"
                                int raidMemberID = int.Parse(raidMembers[i]);
                                if (_RaidMemberIDs.Contains(raidMemberID) == false)
                                    _RaidMemberIDs.Add(raidMemberID);
                            }
	                    }
	                    catch (Exception ex)
	                    {
		                    Logger.LogException(ex);
	                    }
                    }
                    else
                    {
                        try
                        {
                            UnitData newUnitData = new UnitData(unitData, previousUnitDatas);
                            if (_UnitIDsToName.ContainsKey(newUnitData.UnitID) == true)
                            {
                                UnitDatas.Add(newUnitData.UnitID, newUnitData);
                                ChangedUnitDatas.Add(newUnitData.UnitID);
                            }
                            else
                            {
                                Logger.ConsoleWriteLine("UnitID(" + newUnitData.UnitID + ") was never introduced, this UnitID is automatically discarded completely", ConsoleColor.Red);
                            }
                        }
                        catch (Exception)
                        {}
                    }
                }
            }
            if (previousUnitDatas != null)
            {
                foreach (var previousUnitData in previousUnitDatas)
                {
                    //INVESTIGATE: Possible bug here. Should add EMPTY UnitDatas, since all these are delta values and now same unitdata change will be registered several times?
                    //INVESTIGATE_RESULT: not a bug! UnitDatas automatically accumulates since it tries fetch the previous from the "previousUnitDtas" array and then add its values!
                    if (UnitDatas.ContainsKey(previousUnitData.Key) == false)
                        UnitDatas.Add(previousUnitData.Key, previousUnitData.Value.CreateCopy());
                }
            }
        }

        public Dictionary<int, UnitData> GetDeltaUnitDatas(TimeSlice _ReferenceTimeSlice, bool _OnlyChangedForThisTimeSlice = false)
        {
            Dictionary<int, UnitData> deltaUnitDatas = new Dictionary<int, UnitData>();
            if (_OnlyChangedForThisTimeSlice == false)
            {
                foreach (var unit in UnitDatas)
                {
                    if (_ReferenceTimeSlice.UnitDatas.ContainsKey(unit.Key))
                    {
                        deltaUnitDatas.Add(unit.Key, UnitData.CreateDifference(_ReferenceTimeSlice.UnitDatas[unit.Key], unit.Value));
                    }
                    else
                    {
                        deltaUnitDatas.Add(unit.Key, unit.Value);
                    }
                }
            }
            else
            {
                foreach (var unitKey in ChangedUnitDatas)
                {
                    if (UnitDatas.ContainsKey(unitKey) == true) //Remove later when old site data is removed
                    {
                        var unit = UnitDatas[unitKey];
                        if (_ReferenceTimeSlice.UnitDatas.ContainsKey(unitKey))
                        {
                            deltaUnitDatas.Add(unitKey, UnitData.CreateDifference(_ReferenceTimeSlice.UnitDatas[unitKey], unit));
                        }
                    }
                    else
                    {
                        //ERROR
                    }
                }
            }
            return deltaUnitDatas;
        }
        
        #region Serializing
        public TimeSlice(SerializationInfo _Info, StreamingContext _Context)
        {
            char version = _Info.GetChar("Version");
            Time = _Info.GetInt32("Time");
            Event = _Info.GetString("Event");
            Zone = _Info.GetString("Zone");
            UnitDatas = (Dictionary<int, UnitData>)_Info.GetValue("UnitDatas", typeof(Dictionary<int, UnitData>));
            if(version == 2)
            {
                ChangedUnitDatas = (List<int>)_Info.GetValue("ChangedUnitDatas", typeof(List<int>));
            }
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Version", TimeSlice_VERSION);
            _Info.AddValue("Time", Time);
            _Info.AddValue("Event", Event);
            _Info.AddValue("Zone", Zone);
            _Info.AddValue("UnitDatas", UnitDatas);
            _Info.AddValue("ChangedUnitDatas", ChangedUnitDatas);
        }
        #endregion
    }
}
