using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using Old_RaidBossFight = VF_RaidDamageDatabase.RaidBossFight;
using Old_AttemptType = VF_RaidDamageDatabase.FightData.AttemptType;

using RealmDB = VF_RaidDamageDatabase.RealmDB;

namespace VF_RDDatabase
{
    public enum AttemptType
    {
        UnknownAttempt = 0,
        WipeAttempt = 1,
        KillAttempt = 2,
    }
    public enum SpecifierType
    {
        UnknownSpecifier,
        YellSpecifier,
        HealthSpecifier
    }
    public class AttemptTypeConverter
    {
        public static AttemptType Convert(Old_AttemptType _AttemptType)
        {
            switch (_AttemptType)
            {
                case VF_RaidDamageDatabase.FightData.AttemptType.WipeAttempt:
                    return AttemptType.WipeAttempt;
                case VF_RaidDamageDatabase.FightData.AttemptType.KillAttempt:
                    return AttemptType.KillAttempt;
                case VF_RaidDamageDatabase.FightData.AttemptType.UnknownAttempt:
                    return AttemptType.UnknownAttempt;
                default:
                    return AttemptType.UnknownAttempt;
            }
        }
    }


    [ProtoContract]
    public class BossFight
    {
        [ProtoContract]
        public class DataPrecisionDetails
        {
            [ProtoMember(1)]
            private float m_FightPrecision = 1.0f;
            [ProtoMember(2)]
            private bool m_HasResetsMidFight = false;
            [ProtoMember(3)]
            private bool m_ContainCorruptSWSync = false;
            [ProtoMember(4)]
            private string m_AddonVersion = "1.0";
            [ProtoMember(5)]
            private SpecifierType m_StartSpecifier = SpecifierType.UnknownSpecifier;
            [ProtoMember(6)]
            private SpecifierType m_EndSpecifier = SpecifierType.UnknownSpecifier;
            [ProtoMember(7)]
            private float m_HealthPercentageFirstSeen = 0.0f;
            [ProtoMember(8)]
            private string m_RecordedBy = "";

            public DataPrecisionDetails() { }
            public DataPrecisionDetails(Old_RaidBossFight _BossFight, RealmDB _RealmDB)
            {
                m_FightPrecision = (float)_BossFight.CalculatePrecision(_RealmDB.RD_IsPlayerFunc(_BossFight));
                m_HasResetsMidFight = _BossFight.GetFightData().HasResetsMidFight();
                m_ContainCorruptSWSync = _BossFight.GetUnrealisticPlayerSpikes(_RealmDB.RD_GetPlayerIdentifierFunc(_BossFight)).Count != 0;
                m_AddonVersion = _BossFight.GetFightData().AddonVersion;
                m_RecordedBy = _BossFight.GetFightData().RecordedByPlayer;

                var timeSlices = _BossFight.GetFightData().TimeSlices;

                float bossPart1Percentage = 0.0f;
                float bossPart2Percentage = 0.0f;

                string startY = "Start_Y";
                if (_BossFight.GetBossName() == "Razorgore the Untamed" && (_BossFight.GetStartDateTime() < new DateTime(2014, 3, 12) || m_AddonVersion == "1.8.2" || m_AddonVersion == "1.8.1" || m_AddonVersion == "1.7"))
                {
                    startY = "Start_Y=Grethok the Controller";
                }
                for (int i = 0; i < timeSlices.Count; ++i)
                {
                    if (m_StartSpecifier != SpecifierType.YellSpecifier && timeSlices[i].IsStartEvent())
                    {
                        if (timeSlices[i].IsEvent(startY) == true)
                            m_StartSpecifier = SpecifierType.YellSpecifier;
                        else
                            m_StartSpecifier = SpecifierType.HealthSpecifier;
                    }
                    if (m_HealthPercentageFirstSeen < 0.99f && timeSlices[i].IsBossHealthEvent())
                    {
                        if (_BossFight.GetBossName() == "C'Thun")
                        {
                            float eyeofcthunMax = timeSlices[i].GetTotalBossPercentage("Eye of C'Thun", true) * 0.5f;
                            if (eyeofcthunMax > bossPart1Percentage)
                                bossPart1Percentage = eyeofcthunMax;
                            float cthunMax = timeSlices[i].GetTotalBossPercentage("C'Thun", true) * 0.5f;
                            if (cthunMax > bossPart2Percentage)
                                bossPart2Percentage = cthunMax;
                            if (bossPart1Percentage + bossPart2Percentage > m_HealthPercentageFirstSeen)
                                m_HealthPercentageFirstSeen = bossPart1Percentage + bossPart2Percentage;
                        }
                        else
                        {
                            var healthPercentage = timeSlices[i].GetTotalBossPercentage(_BossFight.GetBossName());
                            if (healthPercentage > m_HealthPercentageFirstSeen)
                                m_HealthPercentageFirstSeen = healthPercentage;
                        }
                    }
                    if (m_EndSpecifier != SpecifierType.YellSpecifier && timeSlices[i].IsDeadEvent())
                    {
                        if (timeSlices[i].IsEvent("Dead_Y") == true)
                        {
                            m_EndSpecifier = SpecifierType.YellSpecifier;
                            break;
                        }
                        else
                            m_EndSpecifier = SpecifierType.HealthSpecifier;
                    }
                }
            }

            public float FightPrecision
            {
                get { 
                    if (m_FightPrecision > 0.0f && m_FightPrecision <= 1.0f) 
                        return m_FightPrecision; 
                    else 
                        return 0.0f; 
                }
            }
            public bool HasResetsMidFight
            {
                get { return m_HasResetsMidFight; }
            }
            public bool ContainCorruptSWSync
            {
                get { return m_ContainCorruptSWSync; }
            }
            public string AddonVersion
            {
                get { return m_AddonVersion; }
            }
            public SpecifierType StartSpecifier
            {
                get { return m_StartSpecifier; }
            }
            public SpecifierType EndSpecifier
            {
                get { return m_EndSpecifier; }
            }
            public float HealthPercentageFirstSeen
            {
                get { return m_HealthPercentageFirstSeen; }
            }
            public string RecordedBy
            {
                get { return m_RecordedBy; }
            }
        }

        [ProtoMember(1)]
        private string m_BossName;
        [ProtoMember(2)]
        private AttemptType m_AttemptType;
        [ProtoMember(3)]
        private DateTime m_StartDateTime = DateTime.MinValue;
        [ProtoMember(4)]
        private DateTime m_EndDateTime = DateTime.MaxValue;
        [ProtoMember(5)]
        private List<Tuple<string, PlayerFightData>> m_PlayerFightData = new List<Tuple<string,PlayerFightData>>();//Använd aldrig null i protobuf för listor
        [ProtoMember(6)]
        private DataPrecisionDetails m_DataPrecisionDetails = null;
        [ProtoMember(7)]
        private int m_FightDuration = -1;

        public BossFight() { }
        public BossFight(Old_RaidBossFight[] _BossFight, List<string> _RaidMembers)
        {
            Old_RaidBossFight bestBossFight = null;
            foreach (var bossFight in _BossFight)
            {
                if(bestBossFight == null 
                || (bossFight.GetStartDateTime() < bestBossFight.GetStartDateTime() && bossFight.GetFightDuration() >= bestBossFight.GetFightDuration()))
                {
                    bestBossFight = bossFight;
                }
            }
            _Contructor(bestBossFight, _RaidMembers);
        }
        public static DateTime EARLIESTCOMPATIBLEDATE = new DateTime(2013, 10, 23, 0, 0, 0);
        public bool IsQualityHigh(bool _IncludeCompatibleDateCheck = false)
        {
            if (_IncludeCompatibleDateCheck == true && m_StartDateTime < EARLIESTCOMPATIBLEDATE)
                return false;

            if (m_DataPrecisionDetails == null)
                return false;//Happens for a raid 2013-10-03ish

            bool basicQuality = (m_DataPrecisionDetails.HasResetsMidFight == false
                && AttemptType == VF_RDDatabase.AttemptType.KillAttempt);
            if (basicQuality == false)
                return false;

            if (m_DataPrecisionDetails.HealthPercentageFirstSeen < 0.97f)
            {
                if (m_BossName == "Vaelastrasz the Corrupt"
                && m_DataPrecisionDetails.HealthPercentageFirstSeen >= 0.29f)
                {
                    //return true;
                }
                else if (m_DataPrecisionDetails.StartSpecifier == SpecifierType.YellSpecifier && m_DataPrecisionDetails.HealthPercentageFirstSeen > 0.90f)
                {
                    //return true;
                }
                else
                {
                    return false;
                }
            }

            if (m_DataPrecisionDetails.StartSpecifier != SpecifierType.YellSpecifier)
            {
                if (VF_RaidDamageDatabase.BossInformation.BossWithStartYell.Contains(BossName))
                    return false;
            }
            if (m_DataPrecisionDetails.EndSpecifier != SpecifierType.YellSpecifier)
            {
                if (VF_RaidDamageDatabase.BossInformation.BossWithEndYell.Contains(BossName))
                    return false;
            }

            return true;
        }
        public bool IsOverlapping(BossFight _BossFight)
        {
            if (m_BossName != _BossFight.m_BossName)
                return false;
            var aFightStart = m_StartDateTime;
            var aFightEnd = m_EndDateTime;
            var bFightStart = _BossFight.m_StartDateTime;
            var bFightEnd = _BossFight.m_EndDateTime;

            return (aFightStart >= bFightStart && aFightStart <= bFightEnd) || (aFightEnd >= bFightStart && aFightEnd <= bFightEnd)
                || (bFightStart >= aFightStart && bFightStart <= aFightEnd) || (bFightEnd >= aFightStart && bFightEnd <= aFightEnd);
        }
        public bool IsBetterVersionOf(BossFight _BossFight)
        {
            try
            {
                var aQualityHigh = IsQualityHigh();
                var aFightStart = m_StartDateTime;
                var aFightEnd = m_EndDateTime;
                var aFightDuration = m_FightDuration;
                var bQualityHigh = _BossFight.IsQualityHigh();
                var bFightStart = _BossFight.m_StartDateTime;
                var bFightEnd = _BossFight.m_EndDateTime;
                var bFightDuration = _BossFight.m_FightDuration;

                if (aQualityHigh == true && bQualityHigh == false)
                    return true;
                else if (aQualityHigh == false && bQualityHigh == true)
                    return false;

                if (m_PlayerFightData.Sum((_Value) => (long)_Value.Item2.Damage) 
                    + m_PlayerFightData.Sum((_Value) => (long)_Value.Item2.EffectiveHeal)
                    + m_PlayerFightData.Sum((_Value) => (long)_Value.Item2.DamageTaken) >
                    _BossFight.m_PlayerFightData.Sum((_Value) => (long)_Value.Item2.Damage)
                    + _BossFight.m_PlayerFightData.Sum((_Value) => (long)_Value.Item2.EffectiveHeal)
                    + _BossFight.m_PlayerFightData.Sum((_Value) => (long)_Value.Item2.DamageTaken))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                VF_RaidDamageDatabase.Logger.LogException(ex);
            }
            return false;
        }
        private void _Contructor(Old_RaidBossFight _BossFight, List<string> _RaidMembers)
        {
            try
            {
                m_PlayerFightData = new List<Tuple<string, PlayerFightData>>();
                m_BossName = _BossFight.GetBossName();
                m_AttemptType = AttemptTypeConverter.Convert(_BossFight.GetFightData().GetAttemptType());
                m_StartDateTime = _BossFight.GetStartDateTime();
                m_EndDateTime = _BossFight.GetFightData().GetEndDateTime();
                m_FightDuration = _BossFight.GetFightDuration();

                var realmDB = Hidden._GlobalInitializationData.GetRealmDBFunc(_BossFight.GetRaid().Realm);
                var unitsData = _BossFight.GetFilteredPlayerUnitsData(true, realmDB.RD_GetPlayerIdentifierFunc(_BossFight));
                var petData = _BossFight.GetFilteredPetUnitsData();
                List<Tuple<string, VF_RaidDamageDatabase.UnitData>> abusingPets = new List<Tuple<string, VF_RaidDamageDatabase.UnitData>>();
                foreach (var unitPet in petData)
                {
                    if (_RaidMembers.Contains(unitPet.Item1.Split('(').First()) == true)
                    {
                        //Player with Pet UnitPet should be banned from damagemeter or has its damage purged
                        string abusingPlayer = unitPet.Item1.Split(new string[]{ "(Pet for ", ")" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        abusingPets.Add(Tuple.Create(abusingPlayer, unitPet.Item2));
                    }
                }
                foreach (var unitData in unitsData)
                {
                    if (unitData.Item2.I.Death > 0 || unitData.Item2.I.Dmg > 0 || unitData.Item2.I.RawHeal > 0)
                    {
                        if (unitData.Item1 == "Unknown")
                            continue;

                        var petsAbused = abusingPets.FindAll((_Value) => { return _Value.Item1 == unitData.Item1; });
                        if (petsAbused != null && petsAbused.Count > 0)
                        {
                            var unitFightData = unitData.Item2.CreateCopy();

                            foreach(var petAbused in petsAbused)
                            {
                                unitFightData.SubtractUnitData(petAbused.Item2);
                            }
                            PlayerFightData newFightData = new PlayerFightData(unitFightData);
                            newFightData.InitCache(this);
                            m_PlayerFightData.Add(Tuple.Create(unitData.Item1, newFightData));
                        }
                        else
                        {
                            PlayerFightData newFightData = new PlayerFightData(unitData.Item2);
                            newFightData.InitCache(this);
                            m_PlayerFightData.Add(Tuple.Create(unitData.Item1, newFightData));
                        }
                    }
                }
                m_DataPrecisionDetails = new DataPrecisionDetails(_BossFight, realmDB);
            }
            catch (Exception ex)
            {
                VF_RaidDamageDatabase.Logger.LogException(ex);
            }
        }

        public string BossName
        {
            get { return m_BossName; }
        }
        public AttemptType AttemptType
        {
            get { return m_AttemptType; }
        }

        public DateTime StartDateTime
        {
            get { return m_StartDateTime; }
        }
        public DateTime EndDateTime
        {
            get { return m_EndDateTime; }
        }
        public int FightDuration
        {
            get { return m_FightDuration;}// return (int)((m_EndDateTime - m_StartDateTime).TotalSeconds); }
        }
        public List<Tuple<string, PlayerFightData>> PlayerFightData
        {
            get { return m_PlayerFightData; }
        }
        public DataPrecisionDetails DataDetails
        {
            get { return m_DataPrecisionDetails; }
        }








#region Cache Data
        private Raid m_CacheRaid = null;

        public Raid CacheRaid
        {
            get { return m_CacheRaid; }
        }
        //[ProtoAfterDeserialization]
        private void InitChildCache()
        {
            foreach (var fightData in m_PlayerFightData)
            {
                fightData.Item2.InitCache(this);
            }
        }
        public void InitCache(Raid _Raid)
        {
            m_CacheRaid = _Raid;
            InitChildCache();
        }
        public bool CacheInitialized()
        {
            return m_CacheRaid != null;
        }
        public void Dispose()
        {
            m_CacheRaid = null;
            foreach (var fightData in m_PlayerFightData)
            {
                fightData.Item2.Dispose();
            }
        }
#endregion
    }
}
