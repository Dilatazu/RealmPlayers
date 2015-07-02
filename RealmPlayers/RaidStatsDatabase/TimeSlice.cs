using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;
namespace VF_RaidDamageDatabase
{
    [ProtoContract]
    public struct BuffInfo
    {
        [ProtoMember(1)]
        public int BuffID;
        [ProtoMember(2)]
        public int LastUpdatedTimeSlice;
    }
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
        [ProtoMember(6)]
        public Dictionary<int, List<BuffInfo>> UnitBuffs = null;
        [ProtoMember(7)]
        public int TimeSliceCounter = 0;
        [ProtoMember(8)]
        public Dictionary<int, List<BuffInfo>> UnitDebuffs = null;

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

        public TimeSlice(TimeSlice _PreviousTimeSlice, string _DataString, Dictionary<int, string> _UnitIDsToName, List<int> _RaidMemberIDs, string _Zone, WowVersionEnum _WowVersion, Dictionary<int, string> _BuffIDsToName, string _AddonVersion)
        {
            Dictionary<int, UnitData> previousUnitDatas = null;
            if(_PreviousTimeSlice != null)
            {
                previousUnitDatas = _PreviousTimeSlice.UnitDatas;
                if (_PreviousTimeSlice.UnitBuffs != null)
                {
                    UnitBuffs = new Dictionary<int, List<BuffInfo>>();
                    foreach (var buffData in _PreviousTimeSlice.UnitBuffs)
                    {
                        if (buffData.Value != null)
                        {
                            foreach (BuffInfo buffInfo in buffData.Value)
                            {
                                UnitBuffs.AddToList(buffData.Key, buffInfo);
                            }
                        }
                    }
                }
                if (_PreviousTimeSlice.UnitDebuffs != null)
                {
                    UnitDebuffs = new Dictionary<int, List<BuffInfo>>();
                    foreach (var buffData in _PreviousTimeSlice.UnitDebuffs)
                    {
                        if (buffData.Value != null)
                        {
                            foreach (BuffInfo buffInfo in buffData.Value)
                            {
                                UnitDebuffs.AddToList(buffData.Key, buffInfo);
                            }
                        }
                    }
                }
                TimeSliceCounter = _PreviousTimeSlice.TimeSliceCounter + 1;
            }

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
                            if(unitData.StartsWith("B."))
                            {
                                if (_AddonVersion != "1.8.9" && _AddonVersion != "1.9.0")
                                {
                                    try
                                    {
                                        //BuffID Definition for 1.8.9, TODO: Implement parsing!
                                        //unitData = "B.AGGRO_TEST_WHATEVER=3"
                                        string[] idToBuffData = unitData.Substring(2).Split('=');
                                        string buffName = idToBuffData[0];
                                        int buffID = int.Parse(idToBuffData[1]);
                                        _BuffIDsToName.AddOrSet(buffID, buffName);
                                    }
                                    catch (Exception)
                                    { }
                                }
                            }
                            else
                            {
                                string[] idToNameData = unitData.Split('=');
                                string unitName = idToNameData[0];
                                int unitID = int.Parse(idToNameData[1]);
                                if (_UnitIDsToName.ContainsKey(unitID) == false)
                                    _UnitIDsToName.Add(unitID, unitName);
                                else if (unitName.StartsWith("VF_PET_"))
                                    _UnitIDsToName[unitID] = unitName;
                            }
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
                    else if (unitData.StartsWith("Z"))
                    {
                        try
                        {
                            //Zone Definition for 1.9.7, TODO: Implement parsing!
                            //unitData = "Z Molten Core"
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                    }
                    else if (unitData.StartsWith("B") || unitData.StartsWith("D"))
                    {
                        if (_AddonVersion != "1.8.9" && _AddonVersion != "1.9.0")
                        {
                            bool isBuff = unitData.StartsWith("B");
                            bool isDebuff = unitData.StartsWith("D");
                            try
                            {
                                if (isBuff == isDebuff)
                                    throw new Exception("UNEXPECTED ERROR");
                                //Buff Data for 1.8.9, TODO: Implement parsing!
                                //"B12 3 2 4 5" <-- 12 is playerID, other numbers are BuffIDs
                                string dataString = "";
                                if (unitData.StartsWith("BF"))
                                {
                                    //Means that there was 16 buffs active, which means the buffs info might not be 100% accurate!
                                    //TODO: do something useful with this information!
                                    dataString = unitData.Substring(2);
                                }
                                else
                                {
                                    dataString = unitData.Substring(1);
                                }

                                string[] buffData = dataString.Split('.');
                                if (buffData.Length == 4 || buffData.Length == 5)
                                {
                                    int bSI = 0;
                                    int unitID = int.Parse(buffData[bSI++]);
                                    int buffCount = -1;
                                    if (buffData.Length == 5)
                                        buffCount = int.Parse(buffData[bSI++]);
                                    string[] eqBuffs = buffData[bSI++].Split(' ');
                                    string[] subBuffs = buffData[bSI++].Split(' ');
                                    string[] addBuffs = buffData[bSI++].Split(' ');

                                    List<BuffInfo> currUnitBuffs = null;
                                    if (isBuff == true)
                                    {
                                        if (UnitBuffs == null)
                                        {
                                            UnitBuffs = new Dictionary<int, List<BuffInfo>>();
                                        }
                                        if (UnitBuffs.ContainsKey(unitID) == false)
                                            UnitBuffs.Add(unitID, new List<BuffInfo>());

                                        currUnitBuffs = UnitBuffs[unitID];
                                    }
                                    else if (isDebuff == true)
                                    {
                                        if (UnitDebuffs == null)
                                        {
                                            UnitDebuffs = new Dictionary<int, List<BuffInfo>>();
                                        }
                                        if (UnitDebuffs.ContainsKey(unitID) == false)
                                            UnitDebuffs.Add(unitID, new List<BuffInfo>());

                                        currUnitBuffs = UnitDebuffs[unitID];
                                    }

                                    try
                                    {
                                        bool unexpectedAddError = false;
                                        bool unexpectedSubError = false;
                                        bool unexpectedEqError = false;
                                        foreach (var buff in addBuffs)
                                        {
                                            if (buff == "")
                                                continue;
                                            BuffInfo buffInfo;
                                            buffInfo.BuffID = int.Parse(buff);
                                            buffInfo.LastUpdatedTimeSlice = this.TimeSliceCounter;
                                            int buffIndex = currUnitBuffs.FindIndex((_Value) => _Value.BuffID == buffInfo.BuffID);
                                            if (buffIndex >= 0)
                                            {
                                                unexpectedAddError = true;
                                                currUnitBuffs[buffIndex] = buffInfo;
                                            }
                                            else
                                            {
                                                currUnitBuffs.Add(buffInfo);
                                            }
                                        }
                                        foreach (var buff in subBuffs)
                                        {
                                            if (buff == "")
                                                continue;
                                            int buffID = int.Parse(buff);
                                            int buffIndex = currUnitBuffs.FindIndex((_Value) => _Value.BuffID == buffID);
                                            if (buffIndex >= 0)
                                            {
                                                currUnitBuffs.RemoveAt(buffIndex);
                                            }
                                            else
                                            {
                                                unexpectedSubError = true;
                                            }
                                        }
                                        foreach (var buff in eqBuffs)
                                        {
                                            if (buff == "")
                                                continue;
                                            BuffInfo buffInfo;
                                            buffInfo.BuffID = int.Parse(buff);
                                            buffInfo.LastUpdatedTimeSlice = this.TimeSliceCounter;
                                            int buffIndex = currUnitBuffs.FindIndex((_Value) => _Value.BuffID == buffInfo.BuffID);
                                            if (buffIndex >= 0)
                                            {
                                                currUnitBuffs[buffIndex] = buffInfo;
                                            }
                                            else
                                            {
                                                unexpectedEqError = true;
                                                currUnitBuffs.Add(buffInfo);
                                            }
                                        }
                                        if (unexpectedAddError)
                                            Logger.ConsoleWriteLine("Buff already exists when adding UnitBuff!!! unitData = \"" + unitData + "\"", ConsoleColor.Red);
                                        if (unexpectedSubError)
                                            Logger.ConsoleWriteLine("Buff does not exists when removing UnitBuff!!! unitData = \"" + unitData + "\"", ConsoleColor.Red);
                                        if (unexpectedEqError)
                                            Logger.ConsoleWriteLine("Buff does not exists when equaling UnitBuff!!! unitData = \"" + unitData + "\"", ConsoleColor.Red);

                                        const int ADDON_REFRESH_EQ_RATE = 20 * 2;//20 is from refreshrate in addon and we double it just to be safe
                                        List<int> removeBuffIDs = new List<int>();
                                        foreach (var buffInfo in currUnitBuffs)
                                        {
                                            if (this.TimeSliceCounter - buffInfo.LastUpdatedTimeSlice > ADDON_REFRESH_EQ_RATE)
                                            {
                                                removeBuffIDs.Add(buffInfo.BuffID);
                                            }
                                        }
                                        foreach (var removeBuffID in removeBuffIDs)
                                        {
                                            int buffIndex = currUnitBuffs.FindIndex((_Value) => _Value.BuffID == removeBuffID);
                                            currUnitBuffs.RemoveAt(buffIndex);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogException(ex);
                                    }
                                }
                                else
                                    Logger.ConsoleWriteLine("Unexpected error when parsing UnitBuffs!!! unitData = \"" + unitData + "\"", ConsoleColor.Red);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            UnitData newUnitData = UnitData.Create(unitData, previousUnitDatas, _WowVersion);
                            if (_UnitIDsToName.ContainsKey(newUnitData.I.UnitID) == true)
                            {
                                UnitDatas.Add(newUnitData.I.UnitID, newUnitData);
                                ChangedUnitDatas.Add(newUnitData.I.UnitID);
                            }
                            else
                            {
                                Logger.ConsoleWriteLine("UnitID(" + newUnitData.I.UnitID + ") was never introduced, this UnitID is automatically discarded completely", ConsoleColor.Red);
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
