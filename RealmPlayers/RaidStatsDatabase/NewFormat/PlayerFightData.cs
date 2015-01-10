using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using Old_UnitData = VF_RaidDamageDatabase.UnitData;

namespace VF_RDDatabase
{
    [ProtoContract]
    public class PlayerFightData
    {
        [ProtoMember(1)]
        private int m_Damage;

        [ProtoMember(2)]
        private int m_EffectiveHeal;
        [ProtoMember(3)]
        private int m_OverHeal;

        [ProtoMember(4)]
        private int m_DamageTaken;
        [ProtoMember(5)]
        private int m_EffectiveHealRecv;
        [ProtoMember(6)]
        private int m_OverHealRecv;

        [ProtoMember(7)]
        private int m_Deaths;

        [ProtoMember(8)]
        private int m_Decurses;

        public PlayerFightData() { }
        public PlayerFightData(Old_UnitData _UnitData)
        {
            m_Damage = _UnitData.I.Dmg;
            m_EffectiveHeal = _UnitData.I.EffHeal;
            m_OverHeal = _UnitData.I.OverHeal;

            m_DamageTaken = _UnitData.I.DmgTaken;
            m_EffectiveHealRecv = _UnitData.I.EffHealRecv;
            m_OverHealRecv = _UnitData.I.OverHealRecv;

            m_Deaths = _UnitData.I.Death;
            m_Decurses = _UnitData.I.Decurse;
        }



        public int Damage
        {
            get { return m_Damage; }
        }
        public int EffectiveHeal
        {
            get { return m_EffectiveHeal; }
        }
        public int OverHeal
        {
            get { return m_OverHeal; }
        }
        public int DamageTaken
        {
            get { return m_DamageTaken; }
        }
        public int EffectiveHealRecv
        {
            get { return m_EffectiveHealRecv; }
        }
        public int OverHealRecv
        {
            get { return m_OverHealRecv; }
        }
        public int Deaths
        {
            get { return m_Deaths; }
        }
        public int Decurses
        {
            get { return m_Decurses; }
        }
        public float DPS
        {
            get
            {
                return (float)m_Damage / (float)m_CacheBossFight.FightDuration;
            }
        }
        public float EffectiveHPS
        {
            get
            {
                return (float)m_EffectiveHeal / (float)m_CacheBossFight.FightDuration;
            }
        }
        public float RawHPS
        {
            get
            {
                return (float)RawHeal / (float)m_CacheBossFight.FightDuration;
            }
        }
        public float OverHPS
        {
            get
            {
                return (float)m_OverHeal / (float)m_CacheBossFight.FightDuration;
            }
        }
        public float OverHealPercentage
        {
            get
            {
                return ((float)OverHeal / (float)RawHeal);
            }
        }
        public int RawHeal
        {
            get
            {
                return m_EffectiveHeal + m_OverHeal;
            }
        }
        public int RawHealRecv
        {
            get
            {
                return m_EffectiveHealRecv + m_OverHealRecv;
            }
        }

#region Cache Data
        private BossFight m_CacheBossFight = null;
        public BossFight CacheBossFight
        {
            get { return m_CacheBossFight; }
        }
        public void InitCache(BossFight _BossFight)
        {
            m_CacheBossFight = _BossFight;
        }
        public bool CacheInitialized()
        {
            return m_CacheBossFight != null;
        }
        public void Dispose()
        {
            m_CacheBossFight = null;
        }
#endregion
    }
}
