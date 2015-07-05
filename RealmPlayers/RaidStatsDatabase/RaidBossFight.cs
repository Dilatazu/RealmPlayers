using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace VF_RaidDamageDatabase
{
    public class RaidBossFight
    {
        RaidCollection_Raid m_Raid = null;
        int m_RaidBossFightIndex = 0;
        FightDataCollection.FightCacheData m_FightData = null;
        List<FightDataCollection.FightCacheData> m_ExtraFightDataVersions = new List<FightDataCollection.FightCacheData>();
        bool m_IsExtraDataVersion = false;
        ThreadSafeCache m_Cache = new ThreadSafeCache();

        public RaidBossFight(RaidCollection_Raid _Raid, int _RaidBossFightIndex, FightDataCollection.FightCacheData _FightData, List<FightDataCollection.FightCacheData> _ExtraFightDataVersions = null, bool _IsExtraDataVersion = false)
        {
            m_Raid = _Raid;
            m_RaidBossFightIndex = _RaidBossFightIndex;
            m_FightData = _FightData;
            if (_ExtraFightDataVersions != null)
                m_ExtraFightDataVersions = _ExtraFightDataVersions;
            m_IsExtraDataVersion = _IsExtraDataVersion;
        }

        public bool IsExtraFightDataVersion()
        {
            return m_IsExtraDataVersion;
        }
        public int GetExtraFightVersionCount()
        {
            return m_ExtraFightDataVersions.Count;
        }
        public RaidBossFight GetExtraFightVersion(int _VersionNumber)
        {
            if (m_ExtraFightDataVersions == null || _VersionNumber < 0 || _VersionNumber >= m_ExtraFightDataVersions.Count)
                return null;
            
            List<FightDataCollection.FightCacheData> otherFightDataVersions = new List<FightDataCollection.FightCacheData>();
            otherFightDataVersions.Add(m_FightData);
            foreach(var dataVersion in m_ExtraFightDataVersions)
            {
                otherFightDataVersions.Add(dataVersion);
            }

            return new RaidBossFight(m_Raid, 0x10000 * _VersionNumber + (m_RaidBossFightIndex & 0xFFFF), m_ExtraFightDataVersions[_VersionNumber], otherFightDataVersions, true);
        }
        public FightData GetFightData()
        {
            return m_FightData.m_Fight;
        }
        public FightDataCollection.FightCacheData GetFightCacheData()
        {
            return m_FightData;
        }
        public RaidCollection_Raid GetRaid()
        {
            return m_Raid;
        }
        public int GetRaidBossFightIndex()
        {
            return m_RaidBossFightIndex;
        }
        public void _SetRaidBossFightIndex(int _RaidBossFightIndex)
        {
            m_RaidBossFightIndex = _RaidBossFightIndex;
        }
        public string GetRaidOwner()
        {
            return m_Raid.RaidOwnerName;
        }
        public bool GetTryCount(out int _TryNumber, out int _TotalTries)
        {
            _TryNumber = 1;
            _TotalTries = 0;
            var bossFights = m_Raid._GetBossFights();
            if (bossFights == null)
                return false;
            foreach (var fight in bossFights)
            {
                if (fight.GetFightData().FightName == GetFightData().FightName)
                    ++_TotalTries;
                if (ReferenceEquals(fight, this))
                    _TryNumber = _TotalTries;
            }
            return true;
        }
        public List<string> GetAttendingUnits(Func<string, bool> _Predicate = null)
        {
            var unitData = GetUnitsData(true);
            return GetFightCacheData().GetAttendingUnits(unitData, _Predicate);
        }
        public List<Tuple<string, int>> GetItemDrops()
        {
            try
            {
                DateTime lootBetweenThreshold_Min = GetStartDateTime().AddSeconds(GetFightDuration());
                DateTime lootBetweenThreshold_Max = lootBetweenThreshold_Min.AddMinutes(3);
                List<int> itemDrops = new List<int>();
                if (GetBossName() == "Twin Emperors")
                {
                    var bossLoot = GetFightCacheData().m_FightDataCollection.m_BossLoot;
                    int boss1LootIndex = bossLoot.FindIndex((_Value) => _Value.Item2 == "Emperor Vek'nilash");
                    if (boss1LootIndex != -1)
                        itemDrops = new List<int>(bossLoot[boss1LootIndex].Item3);
                    int boss2LootIndex = bossLoot.FindIndex((_Value) => _Value.Item2 == "Emperor Vek'lor");
                    if (boss2LootIndex != -1)
                        itemDrops.AddRange(bossLoot[boss2LootIndex].Item3);
                }
                else
                {
                    var bossLoot = GetFightCacheData().m_FightDataCollection.m_BossLoot;
                    int bossLootIndex = bossLoot.FindIndex((_Value) => _Value.Item2 == GetBossName());
                    if (bossLootIndex != -1)
                        itemDrops = new List<int>(bossLoot[bossLootIndex].Item3);
                }

                List<Tuple<string, int>> receiveItems = new List<Tuple<string, int>>();
                {
                    var lootsReceived = GetFightCacheData().m_FightDataCollection.m_PlayerLoot.FindAll((_Value) => _Value.Item1 > lootBetweenThreshold_Min && _Value.Item1 < lootBetweenThreshold_Max);
                    foreach (var lootReceived in lootsReceived)
                    {
                        receiveItems.AddUnique(Tuple.Create(lootReceived.Item2, lootReceived.Item3), (_Value1, _Value2) => _Value1.Item1 == _Value2.Item1 && _Value1.Item2 == _Value2.Item2);
                    }
                }

                foreach (var extraFightData in m_ExtraFightDataVersions)
                {
                    if (GetBossName() == "Twin Emperors")
                    {
                        var extraBossLoot = extraFightData.m_FightDataCollection.m_BossLoot;
                        int boss1LootIndex = extraBossLoot.FindIndex((_Value) => _Value.Item2 == "Emperor Vek'nilash");
                        if (boss1LootIndex != -1)
                            itemDrops.AddRangeUnique(extraBossLoot[boss1LootIndex].Item3);
                        int boss2LootIndex = extraBossLoot.FindIndex((_Value) => _Value.Item2 == "Emperor Vek'lor");
                        if (boss2LootIndex != -1)
                            itemDrops.AddRangeUnique(extraBossLoot[boss2LootIndex].Item3);
                    }
                    else
                    {
                        var extraBossLoot = extraFightData.m_FightDataCollection.m_BossLoot;
                        int extraBossLootIndex = extraBossLoot.FindIndex((_Value) => _Value.Item2 == GetBossName());
                        if (extraBossLootIndex != -1)
                        {
                            itemDrops.AddRangeUnique(extraBossLoot[extraBossLootIndex].Item3);
                        }
                    }
                    var lootsReceived = extraFightData.m_FightDataCollection.m_PlayerLoot.FindAll((_Value) => _Value.Item1 > lootBetweenThreshold_Min && _Value.Item1 < lootBetweenThreshold_Max);
                    foreach (var lootReceived in lootsReceived)
                    {
                        receiveItems.AddUnique(Tuple.Create(lootReceived.Item2, lootReceived.Item3), (_Value1, _Value2) => _Value1.Item1 == _Value2.Item1 && _Value1.Item2 == _Value2.Item2);
                    }
                }

                if (itemDrops.Count < 2)
                {
                    foreach (var receiveItem in receiveItems)
                    {
                        if (receiveItem.Item2 != 20725)//Nexus Crystal
                        {
                            itemDrops.AddUnique(receiveItem.Item2);
                        }
                    }
                }
                List<Tuple<string, int>> result = new List<Tuple<string,int>>();
                foreach(var item in itemDrops)
                {
                    bool anonymousReceiver = true;
                    foreach (var recvItem in receiveItems)
                    {
                        if (recvItem.Item2 == item)
                        {
                            result.Add(Tuple.Create(recvItem.Item1, recvItem.Item2));
                            receiveItems.Remove(recvItem);
                            anonymousReceiver = false;
                            break;
                        }
                    }
                    if (anonymousReceiver == true)
                    {
                        result.Add(Tuple.Create("Unknown", item));
                    }
                }

                return result;
            }
	        catch (Exception ex)
	        {
		        Logger.LogException(ex);
                return new List<Tuple<string, int>>();
	        }
        }
        public class FightDetail
        {
            public int Time = 0;
            public Dictionary<string, UnitData> UnitDatas;
            public List<string> Events = new List<string>();
        }
        public List<FightDetail> _NotCached_GetFightDetails()
        {
            List<FightDetail> fightDetails = new List<FightDetail>();

            fightDetails.Add(new FightDetail
            {
                Time = 0,
                UnitDatas = new Dictionary<string, UnitData>(),
                Events = m_FightData.m_Fight.TimeSlices.First().Event.Split(';').ToList(),
            });
            int firstTimeSliceTime = m_FightData.m_Fight.TimeSlices.First().Time;
            for(int i = 1; i < m_FightData.m_Fight.TimeSlices.Count; ++i)
            {
                var prevTimeSlice = m_FightData.m_Fight.TimeSlices[i-1];
                var thisTimeSlice = m_FightData.m_Fight.TimeSlices[i];

                var deltaUnitDatas = thisTimeSlice.GetDeltaUnitDatas(prevTimeSlice, false);
                Dictionary<string, UnitData> unitDatas = new Dictionary<string, UnitData>();
                foreach (var unitData in deltaUnitDatas)
                {
                    string unitName;
                    if (m_FightData.GetUnitName(unitData.Value, out unitName) == true)
                    {
                        try
                        {
                            unitDatas.Add(unitName, unitData.Value.CreateCopy());
                        }
                        catch (Exception)
                        {
                            throw new Exception("Could not add UnitData for: \"" + unitName + "\"");
                        }
                    }
                }

                if (true)//_MergePetData == true)
                {
                    foreach (var unitData in unitDatas)
                    {
                        if (unitData.Key.StartsWith("VF_PET_") && unitData.Key.Contains("VFUnknown") == false && unitData.Value.I.Dmg >= 0 && unitData.Value.I.RawHeal >= 0)
                        {
                            string owner = unitData.Key.Split('_').Last();
                            UnitData ownerUnitData = null;
                            if(unitDatas.TryGetValue(owner, out ownerUnitData) == true)
                            {
                                ownerUnitData.AddPetDataAndClearPet(unitData.Value);
                            }
                        }
                    }
                }
                fightDetails.Add(new FightDetail
                {
                    Time = thisTimeSlice.Time - firstTimeSliceTime,
                    UnitDatas = unitDatas,
                    Events = thisTimeSlice.Event.Split(';').ToList(),
                });
            }
            return fightDetails;
        }
        public List<FightDetail> GetFightDetails()
        {
            return m_Cache.Get("GetFightDetails", _NotCached_GetFightDetails);
        }
        public List<FightDetail> GetFilteredFightDetails(Func<string, PlayerIdentifier> _PlayerIdentifier)
        {
            var unrealisticPlayerSpikes = GetUnrealisticPlayerSpikes(_PlayerIdentifier);
            if (unrealisticPlayerSpikes.Count == 0)
                return GetFightDetails();

            var fightDetails = new List<FightDetail>(GetFightDetails());
            for (int i = 0; i < fightDetails.Count; ++i)
            {
                foreach (var uPS in unrealisticPlayerSpikes)
                {
                    if (uPS.Time == fightDetails[i].Time)
                    {
                        var newFightDetail = fightDetails[i];
                        newFightDetail = new FightDetail();

                        newFightDetail.Time = fightDetails[i].Time;
                        newFightDetail.Events = fightDetails[i].Events;
                        newFightDetail.UnitDatas = new Dictionary<string, UnitData>();
                        foreach (var unitData in fightDetails[i].UnitDatas)
                        {
                            if (unitData.Key == uPS.Player)
                            {
                                var newUnitData = unitData.Value.CreateCopy();
                                newUnitData.SubtractUnitData(uPS.UnitFrameData);
                                newFightDetail.UnitDatas.Add(unitData.Key, newUnitData);
                            }
                            else
                            {
                                newFightDetail.UnitDatas.Add(unitData.Key, unitData.Value);
                            }
                        }
                        fightDetails[i] = newFightDetail;
                    }
                }
            }

            return fightDetails;
        }
        public bool DetectIsCorruptSWSync(Func<string, bool> _PlayerIdentifier)
        {
            int lastTime = 0;
            var fightDetails = GetFightDetails();
            int maxCheckTime = 30;
            for (int i = 0; i < fightDetails.Count; ++i)
            {
                var fightDetail = fightDetails[i];
                int deltaTime = fightDetail.Time - lastTime;
                if (fightDetail.Time > maxCheckTime)
                    break;

                if (fightDetail.UnitDatas.Count > 0 && deltaTime > 0)
                {
                    double dps = fightDetail.UnitDatas.OrderByDescending((_Value) => (_PlayerIdentifier(_Value.Key) ? _Value.Value.I.Dmg : 0)).First().Value.I.Dmg / deltaTime;
                    if (dps > 6000)
                        return true;
                    lastTime = fightDetail.Time;
                }
            }
            return false;
        }
        public bool DetectMissingPartOfFight()
        {
            try
            {
                var significantTimeSlices = GetSignificantTimeSlices();
                double bossStartHP = 1.0;
                if(GetBossName() == "Vaelastrasz the Corrupt")
                    bossStartHP = 0.3;
                if (GetBossHealthPercentageAtTimeSlice(significantTimeSlices.FirstBossHealthSlice) < 0.95 * bossStartHP)
                    return true;
            }
            catch (Exception)
            {}
            return false;
        }

        public struct PlayerSpike
        {
            public string Player;
            public int Time;
            public int DeltaTime;
            public double SpikeDmgFactor;
            public double SpikeHealFactor;
            public UnitData UnitFrameData;

            public int DmgValue
            {
                get { return UnitFrameData.I.Dmg; }
            }
            public int HealValue
            {
                get { return UnitFrameData.I.EffHeal; }
            }
        }
        public enum PlayerIdentifier
        {
            NotPlayer = 0,
            Player = 1,
            AOEClass = 2,
            HealClass = 8,
            SpellClass = 16,
            PetClass = 32
        }
        public List<PlayerSpike> GetUnrealisticPlayerSpikes(Func<string, PlayerIdentifier> _PlayerIdentifier)
        {
            List<PlayerSpike> retList = new List<PlayerSpike>();
            if (GetBossName() == "Thaddius")
                return retList;
            int lastTime = 0;
            var fightDetails = GetFightDetails();
            var unitDatas = GetUnitsDataAsDictionary(true);
            int fightDuration = GetFightDuration();

            var significantTimeSlices = GetSignificantTimeSlices();

            int endTimeSliceTime = GetTimeAtTimeSlice(significantTimeSlices.EndTimeSlice);

            Dictionary<string, Tuple<UnitData, int>> lastUnitDataTimes = new Dictionary<string, Tuple<UnitData, int>>();
            for (int i = 0; i < fightDetails.Count; ++i)
            {
                var fightDetail = fightDetails[i];
                
                float currentBossPercentage = GetBossHealthAtTime(fightDetail.Time);
                
                bool aoeHeavy = false;
                bool spellVulnerable = false;
                bool lastTimeSlice = false;
                if (GetBossName() == "Nefarian" && currentBossPercentage < 0.22f)
                    aoeHeavy = true;
                else if (GetBossName() == "Chromaggus" && currentBossPercentage < 0.90f)
                    spellVulnerable = true;
                if (fightDetail.Time < GetTimeAtTimeSlice(significantTimeSlices.StartTimeSlice))
                {
                    lastTime = fightDetail.Time;
                    continue;//Lets not look at spikes outside data set
                }
                else if (fightDetail.Time > endTimeSliceTime)
                {
                    break;//Lets not look at spikes outside data set
                }
                else if (fightDetail.Time >= endTimeSliceTime)
                {
                    lastTimeSlice = true;
                }
                int deltaTime = fightDetail.Time - lastTime;
                if (deltaTime <= 0)
                    deltaTime = 1;
                double fightDeltaTimePercentage = ((double)deltaTime / (double)fightDuration);
                double fightDeltaCompareTimes_10 = fightDeltaTimePercentage * 10;
                {
                    foreach (var unitData in fightDetail.UnitDatas)
                    {
                        var playerIdentifier = _PlayerIdentifier(unitData.Key);
                        if (playerIdentifier != PlayerIdentifier.NotPlayer)
                        {
                            Tuple<UnitData, int> lastUnitDataTime = null;
                            if (lastUnitDataTimes.TryGetValue(unitData.Key, out lastUnitDataTime) == false)
                            {
                                lastUnitDataTime = Tuple.Create(unitData.Value, fightDetails[0].Time);
                                lastUnitDataTimes.AddOrSet(unitData.Key, lastUnitDataTime);
                            }
                            else if (lastUnitDataTime.Item1.I.Dmg != unitData.Value.I.Dmg || lastUnitDataTime.Item1.I.RawHeal != unitData.Value.I.RawHeal)
                            {
                                lastUnitDataTimes.AddOrSet(unitData.Key, Tuple.Create(unitData.Value, fightDetail.Time));
                            }

                            int playerDeltaTime = fightDetail.Time - lastUnitDataTime.Item2;

                            int frameDmg = unitData.Value.I.Dmg;
                            int frameHeal = unitData.Value.I.EffHeal;

                            int dmgMultiplier = 1;
                            int healMultiplier = 1;

                            if (aoeHeavy == true)
                            {
                                if (playerIdentifier.HasFlag(PlayerIdentifier.AOEClass))
                                    dmgMultiplier = dmgMultiplier * 4;
                                else
                                    dmgMultiplier = dmgMultiplier * 2;

                                if (playerIdentifier.HasFlag(PlayerIdentifier.HealClass))
                                    healMultiplier = healMultiplier * 2;
                            }
                            else if (spellVulnerable == true)
                            {
                                if (playerIdentifier.HasFlag(PlayerIdentifier.SpellClass))
                                    dmgMultiplier = dmgMultiplier * 3;
                            }
                            if (lastTimeSlice == true)
                            {
                                dmgMultiplier = dmgMultiplier * 5;
                                healMultiplier = healMultiplier * 5;
                                if (playerIdentifier.HasFlag(PlayerIdentifier.PetClass))
                                    dmgMultiplier = dmgMultiplier * 100;
                            }
                            if ((frameDmg < 3000 * deltaTime * dmgMultiplier && frameHeal < 3000 * deltaTime * healMultiplier && deltaTime <= 10)
                                || (frameDmg < 5000 && frameHeal < 5000) || (frameDmg < 1000 * playerDeltaTime && frameHeal < 1000 * playerDeltaTime && playerDeltaTime < 50))
                            {
                                //Ignorera spikes under 3k dps/hps troligtvis bara data som inte synkats i tid eller andra små grejjer
                            }
                            else
                            {
                                UnitData totalUnitData = null;// unitDatas[unitData.Key];
                                if (unitDatas.TryGetValue(unitData.Key, out totalUnitData) == false)
                                {
                                    retList.Add(new PlayerSpike
                                    {
                                        Player = "PossiblyUnknownSpike",
                                        Time = fightDetail.Time,
                                        DeltaTime = deltaTime,
                                        SpikeDmgFactor = -1.0,
                                        SpikeHealFactor = -1.0,
                                        UnitFrameData = unitData.Value.CreateCopy()
                                    });
                                    continue;// throw new Exception("Player: \"" + unitData.Key + "\" was not found in unitDatas");
                                }
                                var approxTotalDPS = totalUnitData.I.Dmg / fightDuration;

                                double fightDeltaCompareTimes_Dmg = fightDeltaCompareTimes_10 * dmgMultiplier;
                                double fightDeltaCompareTimes_Heal = fightDeltaCompareTimes_10 * healMultiplier;

                                if (approxTotalDPS < 100)//We are at very low dps numbers so accept pretty much anything
                                    fightDeltaCompareTimes_Dmg = fightDeltaCompareTimes_Dmg * 500.0;
                                else if (approxTotalDPS < 200)
                                    fightDeltaCompareTimes_Dmg = fightDeltaCompareTimes_Dmg * 50.0;
                                else if (approxTotalDPS < 300)
                                    fightDeltaCompareTimes_Dmg = fightDeltaCompareTimes_Dmg * 3.0;
                                else if (approxTotalDPS < 500)
                                    fightDeltaCompareTimes_Dmg = fightDeltaCompareTimes_Dmg * 1.5;

                                if (frameDmg > (totalUnitData.I.Dmg - frameDmg) * fightDeltaCompareTimes_Dmg
                                || frameHeal > (totalUnitData.I.EffHeal - frameHeal) * fightDeltaCompareTimes_Heal)
                                {
                                    //Spike
                                    retList.Add(new PlayerSpike
                                    {
                                        Player = unitData.Key,
                                        Time = fightDetail.Time,
                                        DeltaTime = deltaTime,
                                        SpikeDmgFactor = frameDmg / (fightDeltaTimePercentage * (totalUnitData.I.Dmg - frameDmg)),
                                        SpikeHealFactor = frameHeal / (fightDeltaTimePercentage * (totalUnitData.I.EffHeal - frameHeal)),
                                        UnitFrameData = unitData.Value.CreateCopy()
                                    });//Tuple.Create(unitData.Key, fightDetail.Time));
                                }
                            }
                        }
                    }
                }
                lastTime = fightDetail.Time;
            }
            return retList;
        }
        private int GetTimeAtTimeSlice(int _TimeSlice)
        {
            return m_FightData.m_Fight.TimeSlices[_TimeSlice].Time - m_FightData.m_Fight.TimeSlices.First().Time;
        }
        private float GetBossHealthAtTime(int _Time)
        {
            bool foundTimeFrame = false;
            int firstTime = m_FightData.m_Fight.TimeSlices.First().Time;
            for(int i = m_FightData.m_Fight.TimeSlices.Count - 1; i >= 0; --i)
            {
                var timeSlice = m_FightData.m_Fight.TimeSlices[i];
                if (timeSlice.Time - firstTime <= _Time)
                {
                    foundTimeFrame = true;
                }
                if (foundTimeFrame == true)
                {
                    double bossHealth = 0.0f;
                    if (timeSlice.GetEventBossHealthPercentage(GetBossName(), out bossHealth) == true)
                        return (float)bossHealth;
                }
            }
            return 1.0f;
        }
        //private float GetPlayerActivity(string _Player) //future idea
        //{
        //    throw new Exception("NOT IMPLEMENTED");
        //    //for (int i = m_FightData.m_Fight.TimeSlices.Count - 1; i >= 0; --i)
        //    //{

        //    //}
        //}
        private double GetBossHealthPercentageAtTimeSlice(int _TimeSlice)
        {
            double bossHealth = 0.0;
            m_FightData.m_Fight.TimeSlices[_TimeSlice].GetEventBossHealthPercentage(GetBossName(), out bossHealth);
            return bossHealth;
        }
        private bool _FixStartAndEndTimeSliceIndexes(ref int _StartTimeSlice, ref int _EndTimeSlice)
        {
            if (m_FightData.m_Fight.FightName == "Trash")
            {
                _StartTimeSlice = 0;
                _EndTimeSlice = m_FightData.m_Fight.TimeSlices.Count - 1;
                return true;
            }
            if (_StartTimeSlice < 0 || _EndTimeSlice < 0 
            || _StartTimeSlice >= m_FightData.m_Fight.TimeSlices.Count || _EndTimeSlice >= m_FightData.m_Fight.TimeSlices.Count
            || _EndTimeSlice <= _StartTimeSlice)
            {
                var significantTimeSlices = GetSignificantTimeSlices();
                if (_StartTimeSlice < 0 || _StartTimeSlice >= m_FightData.m_Fight.TimeSlices.Count)
                    _StartTimeSlice = significantTimeSlices.StartTimeSlice;
                if(_EndTimeSlice < 0 || _EndTimeSlice >= m_FightData.m_Fight.TimeSlices.Count || _EndTimeSlice < _StartTimeSlice)
                    _EndTimeSlice = significantTimeSlices.EndTimeSlice;
                return true;
            }
            return true;
        }
        private Dictionary<int, UnitData> _GetDeltaUnitDatas(int _StartTimeSlice, int _EndTimeSlice)
        {
            var startTimeSlice = _GetTimeSlice(_StartTimeSlice);
            var endTimeSlice = _GetTimeSlice(_EndTimeSlice);
            var deltaUnits = endTimeSlice.GetDeltaUnitDatas(startTimeSlice);
            for (int i = _StartTimeSlice; i < _EndTimeSlice; ++i)
            {
                var currMinTimeSlice = _GetTimeSlice(i);
                foreach (var unit in currMinTimeSlice.UnitDatas)
                {
                    if (deltaUnits.ContainsKey(unit.Key) == false)
                    {
                        for(int u = _EndTimeSlice; u > i; --u)
                        {
                            var currMaxTimeSlice = _GetTimeSlice(u);
                            if(currMaxTimeSlice.UnitDatas.ContainsKey(unit.Key) == true)
                            {
                                deltaUnits.Add(unit.Key, UnitData.CreateDifference(unit.Value, currMaxTimeSlice.UnitDatas[unit.Key]));
                                break;
                            }
                        }
                    }
                }
            }
            return deltaUnits;
        }
        public List<Tuple<string, UnitData>> _NotCached_GetUnitsData(bool _MergePetData, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            _FixStartAndEndTimeSliceIndexes(ref _StartTimeSlice, ref _EndTimeSlice);
            //if(GetFightData().TimeSlices.Count > _EndTimeSlice + 1)
            //    _EndTimeSlice += 1;
            var deltaUnits = _GetDeltaUnitDatas(_StartTimeSlice, _EndTimeSlice);// _GetTimeSlice(_EndTimeSlice).GetDeltaUnitDatas(_GetTimeSlice(_StartTimeSlice));

            List<Tuple<string, UnitData>> unitsData = new List<Tuple<string, UnitData>>();
            foreach (var unitData in deltaUnits)
            {
                var unitName = m_FightData.m_FightDataCollection.GetNameFromUnitID(unitData.Key);
                if (unitName != "Unknown")
                {
                    //var firstOrDefault = unitsData.FirstOrDefault((_Value) => _Value.Item1 == unitName);
                    //if (firstOrDefault != default(Tuple<string, UnitData>))
                    //{
                    //    break;
                    //}
                    unitsData.Add(new Tuple<string, UnitData>(unitName, unitData.Value.CreateCopy()));
                }
            }

            if (_MergePetData == true)
            {
                List<Tuple<string, UnitData>> petDatas = new List<Tuple<string,UnitData>>();
                foreach (var unitData in unitsData)
                {
                    if (unitData.Item1.StartsWith("VF_PET_") && unitData.Item1.Contains("VFUnknown") == false && unitData.Item2.I.Dmg >= 0)
                    {
                        string[] splitData = unitData.Item1.Split('_');
                        string owner = splitData.Last();
                        string petName = (splitData.Length >= 2 ? splitData[splitData.Length - 2] : "FaultyPetName");
                        int unitOwnerIndex = unitsData.FindIndex((_Value) => { return _Value.Item1 == owner; });
                        if (unitOwnerIndex != -1)
                        {
                            petDatas.Add(Tuple.Create(petName + "(Pet for " + owner + ")", unitData.Item2.CreateCopy()));
                            unitsData[unitOwnerIndex].Item2.AddPetDataAndClearPet(unitData.Item2);
                        }
                    }
                }
                foreach (var petData in petDatas)
                {
                    unitsData.Add(petData);
                }
            }

            return unitsData;
        }
        public ReadOnlyCollection<Tuple<string, UnitData>> GetUnitsData(bool _MergePetData, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            return m_Cache.Get("GetUnitsData", _NotCached_GetUnitsData, _MergePetData, _StartTimeSlice, _EndTimeSlice).AsReadOnly();
        }
        public Dictionary<string, UnitData> GetUnitsDataAsDictionary(bool _MergePetData, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            var unitsData = m_Cache.Get("GetUnitsData", _NotCached_GetUnitsData, _MergePetData, _StartTimeSlice, _EndTimeSlice);

            string failString = "";
            foreach(var unitData in unitsData)
            {
                if (unitsData.Count((_Value) => unitData.Item1 == _Value.Item1) > 1)
                {
                    failString = failString + ", " + unitData.Item1;
                }
            }
            if(failString != "")
                Logger.ConsoleWriteLine("Failed GetUnitsDataAsDictionary, Duplicates of: " + failString);
            return unitsData.ToDictionary();
        }
        public List<Tuple<string, UnitData>> GetUnitsDataCopy(bool _MergePetData, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            return new List<Tuple<string,UnitData>>(m_Cache.Get("GetUnitsData", _NotCached_GetUnitsData, _MergePetData, _StartTimeSlice, _EndTimeSlice));
        }
        public List<Tuple<string, UnitData>> _NotCached_GetFilteredPlayerUnitsData(bool _MergePetData, Func<string, PlayerIdentifier> _PlayerIdentifier, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            List<Tuple<string, UnitData>> playerUnits = new List<Tuple<string,UnitData>>();
            var unitsData = GetUnitsData(_MergePetData, _StartTimeSlice, _EndTimeSlice);
            var unrealisticPlayerSpikes = GetUnrealisticPlayerSpikes(_PlayerIdentifier);
            foreach(var unitData in unitsData)
            {
                if(_PlayerIdentifier(unitData.Item1) != PlayerIdentifier.NotPlayer)
                {
                    UnitData newUnitData = unitData.Item2.CreateCopy();
                    foreach (var uPS in unrealisticPlayerSpikes)
                    {
                        if(uPS.Player == unitData.Item1)
                        {
                            newUnitData.SubtractUnitData(uPS.UnitFrameData);
                        }
                    }
                    playerUnits.Add(Tuple.Create(unitData.Item1, newUnitData));
                }
            }
            return playerUnits;
        }
        public ReadOnlyCollection<Tuple<string, UnitData>> GetFilteredPlayerUnitsData(bool _MergePetData, Func<string, PlayerIdentifier> _PlayerIdentifier, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            return m_Cache.Get("GetFilteredPlayerUnitsData", _NotCached_GetFilteredPlayerUnitsData, _MergePetData, _PlayerIdentifier, _StartTimeSlice, _EndTimeSlice).AsReadOnly();
        }
        public List<Tuple<string, UnitData>> GetFilteredPlayerUnitsDataCopy(bool _MergePetData, Func<string, PlayerIdentifier> _PlayerIdentifier, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            return new List<Tuple<string, UnitData>>(m_Cache.Get("GetFilteredPlayerUnitsData", _NotCached_GetFilteredPlayerUnitsData, _MergePetData, _PlayerIdentifier, _StartTimeSlice, _EndTimeSlice));
        }
        public List<Tuple<string, UnitData>> _NotCached_GetFilteredPetUnitsData(int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            List<Tuple<string, UnitData>> petUnits = new List<Tuple<string, UnitData>>();
            var unitsData = GetUnitsData(true, _StartTimeSlice, _EndTimeSlice);
            Func<string, PlayerIdentifier> petIdentifier = (string _Name) => { if (_Name.Contains("(Pet for ")) return PlayerIdentifier.Player; else return PlayerIdentifier.NotPlayer; };
            var unrealisticPlayerSpikes = GetUnrealisticPlayerSpikes(petIdentifier);
            foreach (var unitData in unitsData)
            {
                if (petIdentifier(unitData.Item1) != PlayerIdentifier.NotPlayer)
                {
                    UnitData newUnitData = unitData.Item2.CreateCopy();
                    foreach (var uPS in unrealisticPlayerSpikes)
                    {
                        if (uPS.Player == unitData.Item1)
                        {
                            newUnitData.SubtractUnitData(uPS.UnitFrameData);
                        }
                    }
                    petUnits.Add(Tuple.Create(unitData.Item1, newUnitData));
                }
            }
            return petUnits;
        }
        public ReadOnlyCollection<Tuple<string, UnitData>> GetFilteredPetUnitsData(int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            return m_Cache.Get("GetFilteredPetUnitsData", _NotCached_GetFilteredPetUnitsData, _StartTimeSlice, _EndTimeSlice).AsReadOnly();
        }
        public List<Tuple<string, int>> GetUnitsDPS(bool _MergePetData)
        {
            List<Tuple<string, int>> retList = new List<Tuple<string, int>>();
            var unitsData = GetUnitsData(_MergePetData);

            int fightDuration = GetFightDuration();

            foreach (var unitData in unitsData)
            {
                retList.Add(Tuple.Create(unitData.Item1, unitData.Item2.I.Dmg / fightDuration));
            }
            return retList;
        }
        public UnitData GetUnitData(string _Unit, int _StartTimeSlice = -1, int _EndTimeSlice = -1, bool _MergePetData = true)
        {
            var unitsData = GetUnitsData(_MergePetData, _StartTimeSlice, _EndTimeSlice);
            var value = unitsData.FirstOrDefault((_Value) => { return _Value.Item1 == _Unit; });
            if (value == null)
                return null;
            else
                return value.Item2;
        }
        public int GetBossPlusAddsDmgTaken(int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            return GetBossPlusAddsDmgTaken(null, _StartTimeSlice, _EndTimeSlice);
        }
        public int GetBossPlusAddsDmgTaken(List<Tuple<string, int>> _RetDmgTakenList, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            if (_EndTimeSlice == -1)
            {
                _FixStartAndEndTimeSliceIndexes(ref _StartTimeSlice, ref _EndTimeSlice);
                _EndTimeSlice = m_FightData.m_Fight.TimeSlices.Count - 1;
            }
            else
            {
                _FixStartAndEndTimeSliceIndexes(ref _StartTimeSlice, ref _EndTimeSlice);
            }
            var bossUnitData = GetUnitData(m_FightData.m_Fight.FightName, _StartTimeSlice, _EndTimeSlice, true);
            int bossPlusAddsDmgTaken = 0;
            if (bossUnitData != null)
            {
                bossPlusAddsDmgTaken = bossUnitData.I.DmgTaken;
                if (_RetDmgTakenList != null)
                    _RetDmgTakenList.Add(new Tuple<string, int>(m_FightData.m_Fight.FightName, bossUnitData.I.DmgTaken));
            }

            string[] bossAdds = null;
            if (BossInformation.BossAdds.TryGetValue(m_FightData.m_Fight.FightName, out bossAdds) == true)
            {
                foreach (var bossAdd in bossAdds)
                {
                    var bossAddUnitData = GetUnitData(bossAdd, _StartTimeSlice, _EndTimeSlice);
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
        public double CalculatePrecision(Func<string, bool> _PlayerIdentifier)
        {
            int bossPlusAddsDmgTaken;
            int dmgTotalValue;
            int healTotalValue;
            double healPrecision = CalculateHealPrecision(_PlayerIdentifier, out healTotalValue, -1, m_FightData.m_Fight.TimeSlices.Count - 1);
            double dmgPrecision = CalculateDmgPrecision(_PlayerIdentifier, out bossPlusAddsDmgTaken, out dmgTotalValue);
            if (healPrecision > 0.80)
                healPrecision = 1.0;
            return (dmgPrecision * healPrecision);
        }
        public double CalculateDmgPrecision(Func<string, bool> _PlayerIdentifier, out int _BossPlusAddsDmgTaken, out int _TotalValue, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            _FixStartAndEndTimeSliceIndexes(ref _StartTimeSlice, ref _EndTimeSlice);
            var unitsData = GetUnitsData(true, _StartTimeSlice, _EndTimeSlice);

            int bossPlusAddsDmgTaken = GetBossPlusAddsDmgTaken(_StartTimeSlice, m_FightData.m_Fight.TimeSlices.Count - 1);
            double maxValue = 0;
            double totalValue = 0;
            UnitData.CalculateTotalAndMax(unitsData, (_Value) => { return _Value.I.Dmg; }, (_Value) => { return _PlayerIdentifier(_Value.Item1) && _Value.Item2.I.Dmg > 0; }, out totalValue, out maxValue);
            double precision = ((double)bossPlusAddsDmgTaken / (double)totalValue);
            _BossPlusAddsDmgTaken = bossPlusAddsDmgTaken;
            _TotalValue = (int)totalValue;
            if (precision > 1.0)
                precision = 1.0;
            return precision;
        }
        public double CalculateHealPrecision(Func<string, bool> _PlayerIdentifier, out int _TotalValue, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            _FixStartAndEndTimeSliceIndexes(ref _StartTimeSlice, ref _EndTimeSlice);
            var unitsData = GetUnitsData(true, _StartTimeSlice, _EndTimeSlice);

            double dmgTakenMaxValue = 0;
            double dmgTakenTotalValue = 0;
            UnitData.CalculateTotalAndMax(unitsData, (_Value) => { return _Value.I.EffHealRecv; }, (_Value) => { return _PlayerIdentifier(_Value.Item1) && _Value.Item2.I.EffHealRecv > 0; }, out dmgTakenTotalValue, out dmgTakenMaxValue);

            /*if (GetBossName() == "Instructor Razuvious")
            {
             * //Misslyckat försök att försöka fixa det
             * //kör på default Healing Precision är 1 alltid på denna fighten längre ner
                var bossUnitData = GetUnitData("Instructor Razuvious", _StartTimeSlice, _EndTimeSlice);
                var bossAddUnitData = GetUnitData("Death Knight Understudy", _StartTimeSlice, _EndTimeSlice);
                dmgTakenTotalValue = bossUnitData.Dmg + bossAddUnitData.Dmg;
            }*/
            double healMaxValue = 0;
            double healTotalValue = 0;
            UnitData.CalculateTotalAndMax(unitsData, (_Value) => { return _Value.I.EffHeal; }, (_Value) => { return _PlayerIdentifier(_Value.Item1) && _Value.Item2.I.EffHeal > 0; }, out healTotalValue, out healMaxValue);
            double precision = ((double)dmgTakenTotalValue / (double)healTotalValue);
            _TotalValue = (int)healTotalValue;
            if (precision > 1.0 || GetBossName() == "Instructor Razuvious")
                precision = 1.0;
            return precision;
        }
        public double GetTotal(Func<VF_RaidDamageDatabase.UnitData, double> _GetValue
                , Func<Tuple<string, VF_RaidDamageDatabase.UnitData>, bool> _ValidCheck, int _StartTimeSlice = -1, int _EndTimeSlice = -1)
        {
            _FixStartAndEndTimeSliceIndexes(ref _StartTimeSlice, ref _EndTimeSlice);
            var unitsData = GetUnitsData(true, _StartTimeSlice, _EndTimeSlice);
            double totalValue;
            double maxValue;
            if (UnitData.CalculateTotalAndMax(unitsData, _GetValue, _ValidCheck, out totalValue, out maxValue))
                return totalValue;
            else
                return 0.0;
        }
        //private TimeSlice GetNextEventTimeSlice(int _StartTimeSliceIndex, out string _BossHealth)
        //{
        //    for (int i = _StartTimeSliceIndex; i < m_FightData.m_Fight.TimeSlices.Count; ++i)
        //    {
        //        var thisTimeSlice = m_FightData.m_Fight.TimeSlices[i];
        //        if(thisTimeSlice.is
        //    }
        //}
        private TimeSlice _GetTimeSlice(int _Index)
        {
            return m_FightData.m_Fight.TimeSlices[_Index];
        }
        public enum InterstingTimeSliceType
        {
            BossHealth95,
            BossHealth50,
            BossHealth19,
            BossHealth05,
        }
        public class SignificantTimeSlices
        {
            public int StartTimeSlice = -1;
            public int EndTimeSlice = -1;
            public int FirstBossHealthSlice = -1;
            public int LastBossHealthSlice = -1;
            public Dictionary<string, int> DeathTimeSlices = new Dictionary<string, int>();
            public Dictionary<InterstingTimeSliceType, int> InterestingTimeSlices = new Dictionary<InterstingTimeSliceType, int>();
        }
        public int GetFightDuration(SignificantTimeSlices _Data, out double _Precision)
        {
            if (_Data.StartTimeSlice != -1)
            {
                if (_Data.EndTimeSlice != -1)
                {
                    int timeStartEndDifference = _GetTimeSlice(_Data.EndTimeSlice).Time - _GetTimeSlice(_Data.StartTimeSlice).Time;
                    _Precision = 1.0;
                    //double bossHealthPercentage = 0.0;
                    //_GetTimeSlice(_Data.FirstBossHealthSlice).GetEventBossHealthPercentage(out bossHealthPercentage);
                    //if (bossHealthPercentage < 0.999)
                    //    return (int)((double)timeStartEndDifference * (2.0 - bossHealthPercentage));
                    return timeStartEndDifference;
                }
                else
                    _Precision = 0.5;
            }
            else
                _Precision = 0.0;
            return m_FightData.m_Fight.GetFightRecordDuration();
        }
        public int GetFightDuration(out double _Precision)
        {
            var data = GetSignificantTimeSlices();
            return GetFightDuration(data, out _Precision);
        }
        public int GetFightDuration()
        {
            double precision;
            return GetFightDuration(out precision);
        }
        public DateTime GetStartDateTime()
        {
            return m_FightData.m_Fight.StartDateTime;
        }
        public string GetBossName()
        {
            return m_FightData.m_Fight.FightName;
        }
        public SignificantTimeSlices GetSignificantTimeSlices()
        {
            return m_Cache.Get("GetSignificantTimeSlices", _NotCached_GetSignificantTimeSlices);
        }
        public SignificantTimeSlices _NotCached_GetSignificantTimeSlices()
        {
            SignificantTimeSlices output = new SignificantTimeSlices();

            int optionalEndTimeSlice = -1;
            int optionalStartTimeSlice = -1;
            string bossName = GetBossName();
            for (int i = 0; i < m_FightData.m_Fight.TimeSlices.Count; ++i)
            {
                var thisTimeSlice = _GetTimeSlice(i);

                if(i > 0)
                {
                    var prevTimeSlice = _GetTimeSlice(i-1);
                    var deltaUnitDatas = thisTimeSlice.GetDeltaUnitDatas(prevTimeSlice, true);
                    foreach (var unitData in deltaUnitDatas)
                    {
                        if (unitData.Value.I.Death > 0)
                        {
                            string unitName = m_FightData.GetUnitName(unitData.Value);
                            while (output.DeathTimeSlices.AddIfKeyNotExist(unitName, i) == false)
                            {
                                unitName += "#";
                            }
                        }
                    }
                }
                if (thisTimeSlice.IsBossHealthEvent())
                {
                    if (output.FirstBossHealthSlice == -1)
                        output.FirstBossHealthSlice = i;

                    output.LastBossHealthSlice = i;

                    double healthPercentage;
                    if (thisTimeSlice.GetEventBossHealthPercentage(bossName, out healthPercentage) == true)
                    {
                        if (healthPercentage < 0.06)
                            output.InterestingTimeSlices.AddIfKeyNotExist(InterstingTimeSliceType.BossHealth05, i);
                        else if (healthPercentage < 0.20)
                            output.InterestingTimeSlices.AddIfKeyNotExist(InterstingTimeSliceType.BossHealth19, i);
                        else if (healthPercentage < 0.51)
                            output.InterestingTimeSlices.AddIfKeyNotExist(InterstingTimeSliceType.BossHealth50, i);
                        else if (healthPercentage < 0.96)
                            output.InterestingTimeSlices.AddIfKeyNotExist(InterstingTimeSliceType.BossHealth95, i);
                    }
                }
                if (thisTimeSlice.IsStartEvent())
                {
                    if (thisTimeSlice.IsEvent("Start_Y=" + bossName)) //YellEvent - accuracy 100%
                    {
                        if (output.StartTimeSlice == -1) output.StartTimeSlice = i;
                    }
                    else if (thisTimeSlice.IsEvent("Start_T=" + bossName)) //TargetHealthScan every 0.5 sec //händer inte om YellEvent existerar för bossen
                    {
                        if (output.StartTimeSlice == -1) output.StartTimeSlice = i;
                    }
                    else if (thisTimeSlice.IsEvent("Start_C=" + bossName)) //CombatMsgScan every 0.5 sec
                    {
                        if (output.StartTimeSlice == -1) output.StartTimeSlice = i;
                    }
                    else if (thisTimeSlice.IsEvent("Start_S=" + bossName)) //Normal 5 or 10 sec scan
                    {
                        if (output.StartTimeSlice == -1) output.StartTimeSlice = i;
                    }
                    else if (thisTimeSlice.IsEvent("Start_U=" + bossName)) //Unknown Yell Event //Doesnt really mean anything
                    {

                    }
                    else if (thisTimeSlice.IsEvent("Start"))
                    {
                        if (optionalStartTimeSlice == -1) optionalStartTimeSlice = i;
                    }
                    else
                    {
                        //???
                    }
                }
                else if (thisTimeSlice.IsDeadEvent())
                {
                    if (thisTimeSlice.IsEvent("Dead_C=" + bossName) //Hostile Dead Event - accuracy 100%
                    || thisTimeSlice.IsEvent("Dead_Y=" + bossName)) //YellEvent - accuracy 100%
                    {
                        if (output.EndTimeSlice == -1) output.EndTimeSlice = i;
                    }
                    else if (thisTimeSlice.IsEvent("Dead_T=" + bossName)) //TargetHealthScan every 0.5 sec
                    {
                        if (output.EndTimeSlice == -1) output.EndTimeSlice = i;
                    }
                    else if (thisTimeSlice.IsEvent("Dead_S=" + bossName)) //Normal 5 or 10 sec scan
                    {
                        if (output.EndTimeSlice == -1) output.EndTimeSlice = i;
                    }
                    else if (thisTimeSlice.IsEvent("Dead=" + bossName))
                    {
                        if (optionalEndTimeSlice == -1) optionalEndTimeSlice = i;
                    }
                    else
                    {
                        //???
                    }
                }
                else if(thisTimeSlice.IsWipeEvent())
                {
                    if (thisTimeSlice.IsEvent("Wipe_K=" + bossName)) //Checks KTM for 0 threat of everyone every 5 to 10 sec. Must also not have gotten a recent boss combat update. reliable but not exact accuracy
                    {
                        if (output.EndTimeSlice == -1) output.EndTimeSlice = i;
                    }
                    else if (thisTimeSlice.IsEvent("Wipe"))
                    {
                        if (optionalEndTimeSlice == -1) optionalEndTimeSlice = i;
                    }
                    else
                    {
                        //???
                    }
                }
            }
            if (output.StartTimeSlice == -1)
                output.StartTimeSlice = optionalStartTimeSlice;
            if (output.EndTimeSlice == -1)
                output.EndTimeSlice = optionalEndTimeSlice;

            if (output.StartTimeSlice == -1)
                output.StartTimeSlice = 0;
            if (output.EndTimeSlice == -1)
                output.EndTimeSlice = m_FightData.m_Fight.TimeSlices.Count - 1;

            if (m_FightData.m_Fight.FightName == "Trash")
            {
                output.StartTimeSlice = 0;
                output.EndTimeSlice = m_FightData.m_Fight.TimeSlices.Count - 1;
            }
            return output;
        }
    }
}
