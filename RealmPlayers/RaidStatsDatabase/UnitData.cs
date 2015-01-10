using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;
using System.Collections.ObjectModel;

using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

namespace VF_RaidDamageDatabase
{
    [ProtoContract]
    public class UnitData
    {
        public static char UnitData_VERSION = (char)1;
        [ProtoMember(1)]
        private int m_UnitID;
        [ProtoMember(2)]
        private int m_Dmg;
        [ProtoMember(3)]
        private int m_EffHeal;
        [ProtoMember(4)]
        private int m_DmgTaken;
        [ProtoMember(5)]
        private int m_OverHeal;
        [ProtoMember(6)]
        private float m_DPS;//not important
        [ProtoMember(7)]
        private float m_HPS;//not important
        [ProtoMember(8)]
        private int m_Death;
        [ProtoMember(9)]
        private int m_Decurse;
        [ProtoMember(10)]
        private float m_DmgCrit;//not important
        [ProtoMember(11)]
        private float m_HealCrit;//not important
        [ProtoMember(12)]
        private int m_EffHealRecv;
        [ProtoMember(13)]
        private int m_OverHealRecv;
        [ProtoMember(14)]
        private int m_RawHeal;//not important
        [ProtoMember(15)]
        private int m_RawHealRecv;//not important
        [ProtoMember(16)]
        private int m_ThreatValue;

        public struct C_Interface
        {
            public static C_Interface Create(UnitData _UnitData)
            {
                C_Interface newInterface;
                newInterface.m_Data = _UnitData;
                return newInterface;
            }
            private UnitData m_Data;

            public void SetNewUnitID(int _NewUnitID)
            {
                m_Data.m_UnitID = _NewUnitID;
            }
            public void SetNewThreatValue(int _NewThreatValue)
            {
                m_Data.m_ThreatValue = _NewThreatValue;
            }
            public int UnitID { get { return m_Data.m_UnitID; } }
            public int Dmg { get { return m_Data.m_Dmg; } }
            public int EffHeal { get { return m_Data.m_EffHeal; } }
            public int DmgTaken { get { return m_Data.m_DmgTaken; } }
            public int OverHeal { get { return m_Data.m_OverHeal; } }
            public int Death { get { return m_Data.m_Death; } }
            public int Decurse { get { return m_Data.m_Decurse; } }
            public int EffHealRecv { get { return m_Data.m_EffHealRecv; } }
            public int OverHealRecv { get { return m_Data.m_OverHealRecv; } }
            public int ThreatValue { get { return m_Data.m_ThreatValue; } }

            public int RawHeal { get { return EffHealRecv + OverHealRecv; } }
        }
        public struct C_InterfaceVanilla
        {
            public static C_InterfaceVanilla Create(UnitData _UnitData)
            {
                C_InterfaceVanilla newInterface;
                newInterface.m_Data = _UnitData;
                return newInterface;
            }
            private UnitData m_Data;

            public float DPS { get { return m_Data.m_DPS; } }
            public float HPS { get { return m_Data.m_HPS; } }
            public float DmgCrit { get { return m_Data.m_DmgCrit; } }
            public float HealCrit { get { return m_Data.m_HealCrit; } }
            public int RawHeal { get { return m_Data.m_RawHeal; } }
            public int RawHealRecv { get { return m_Data.m_RawHealRecv; } }


            public string CreateDataString()
            {
                return m_Data.I.UnitID + " " + (m_Data.I.Dmg == 0 ? "" : m_Data.I.Dmg.ToString())
                    + " " + (m_Data.I.EffHeal == 0 ? "" : m_Data.I.EffHeal.ToString())
                    + " " + (m_Data.I.DmgTaken == 0 ? "" : m_Data.I.DmgTaken.ToString())
                    + " " + (m_Data.I.OverHeal == 0 ? "" : m_Data.I.OverHeal.ToString())
                    + " " + (DPS == 0 ? "" : DPS.ToString())
                    + " " + (HPS == 0 ? "" : HPS.ToString())
                    + " " + (m_Data.I.Death == 0 ? "" : m_Data.I.Death.ToString())
                    + " " + (m_Data.I.Decurse == 0 ? "" : m_Data.I.Decurse.ToString())
                    + " " + (DmgCrit == 0 ? "" : DmgCrit.ToString())
                    + " " + (HealCrit == 0 ? "" : HealCrit.ToString())
                    + " " + (m_Data.I.EffHealRecv == 0 ? "" : m_Data.I.EffHealRecv.ToString())
                    + " " + (m_Data.I.OverHealRecv == 0 ? "" : m_Data.I.OverHealRecv.ToString())
                    + " " + (RawHeal == 0 ? "" : RawHeal.ToString())
                    + " " + (RawHealRecv == 0 ? "" : RawHealRecv.ToString())
                    + " " + (m_Data.I.ThreatValue == 0 ? "" : m_Data.I.ThreatValue.ToString());
                //}
                //else
                //{
                //    return UnitID + " " + (Dmg == 0 ? "" : Dmg.ToString())
                //        + " " + (EffHeal == 0 ? "" : EffHeal.ToString())
                //        + " " + (DmgTaken == 0 ? "" : DmgTaken.ToString())
                //        + " " + (OverHeal == 0 ? "" : OverHeal.ToString())
                //        + " " + (Death == 0 ? "" : Death.ToString())
                //        + " " + (FriendlyDmg == 0 ? "" : FriendlyDmg.ToString())
                //        + " " + (Decurse == 0 ? "" : Decurse.ToString())
                //        + " " + (CCBreaks == 0 ? "" : CCBreaks.ToString())
                //        + " " + (Interrupts == 0 ? "" : Interrupts.ToString())
                //        + " " + (Dispelled == 0 ? "" : Dispelled.ToString())
                //        + " " + (EffHealRecv == 0 ? "" : EffHealRecv.ToString())
                //        + " " + (OverHealRecv == 0 ? "" : OverHealRecv.ToString())
                //        + " " + (ThreatValue == 0 ? "" : ThreatValue.ToString());
                //}
            }
        }
        public struct C_InterfaceTBC
        {
            public static C_InterfaceTBC Create(UnitData _UnitData)
            {
                C_InterfaceTBC newInterface;
                newInterface.m_Data = _UnitData;
                return newInterface;
            }
            private UnitData m_Data;

            public int FriendlyDmg { get { return 0; } }
            public int CCBreaks { get { return 0; } }
            public int Interrupts { get { return 0; } }
            public int Dispelled { get { return 0; } }
            public string CreateDataString()
            {
                return m_Data.I.UnitID + " " + (m_Data.I.Dmg == 0 ? "" : m_Data.I.Dmg.ToString())
                    + " " + (m_Data.I.EffHeal == 0 ? "" : m_Data.I.EffHeal.ToString())
                    + " " + (m_Data.I.DmgTaken == 0 ? "" : m_Data.I.DmgTaken.ToString())
                    + " " + (m_Data.I.OverHeal == 0 ? "" : m_Data.I.OverHeal.ToString())
                    + " " + (m_Data.I.Death == 0 ? "" : m_Data.I.Death.ToString())
                    + " " + (FriendlyDmg == 0 ? "" : FriendlyDmg.ToString())
                    + " " + (m_Data.I.Decurse == 0 ? "" : m_Data.I.Decurse.ToString())
                    + " " + (CCBreaks == 0 ? "" : CCBreaks.ToString())
                    + " " + (Interrupts == 0 ? "" : Interrupts.ToString())
                    + " " + (Dispelled == 0 ? "" : Dispelled.ToString())
                    + " " + (m_Data.I.EffHealRecv == 0 ? "" : m_Data.I.EffHealRecv.ToString())
                    + " " + (m_Data.I.OverHealRecv == 0 ? "" : m_Data.I.OverHealRecv.ToString())
                    + " " + (m_Data.I.ThreatValue == 0 ? "" : m_Data.I.ThreatValue.ToString());
            }
        }
        public C_Interface Interface
        {
            get { return C_Interface.Create(this); }
        }
        public C_Interface I
        {
            get { return C_Interface.Create(this); }
        }
        public C_InterfaceVanilla InterfaceVanilla
        {
            get { return C_InterfaceVanilla.Create(this); }
        }
        public C_InterfaceVanilla I1
        {
            get { return C_InterfaceVanilla.Create(this); }
        }
        public C_InterfaceTBC InterfaceTBC
        {
            get { return C_InterfaceTBC.Create(this); }
        }
        public C_InterfaceTBC I2
        {
            get { return C_InterfaceTBC.Create(this); }
        }
        //public int GetData(string _DataName)
        //{
        //    if (_DataName == "Dmg") return Dmg;
        //    else if (_DataName == "EffHeal") return EffHeal;
        //    else if (_DataName == "DmgTaken") return DmgTaken;
        //    else if (_DataName == "OverHeal") return OverHeal;
        //    else if (_DataName == "Death") return Death;
        //    else if (_DataName == "Decurse") return Decurse;
        //    else if (_DataName == "EffHealRecv") return EffHealRecv;
        //    else if (_DataName == "OverHealRecv") return OverHealRecv;
        //    else if (_DataName == "RawHeal") return RawHeal;
        //    else if (_DataName == "RawHealRecv") return RawHealRecv;
        //    else if (_DataName == "ThreatValue") return ThreatValue;
        //    else
        //        throw new Exception("Error");
        //}
        public static bool CalculateTotalAndMax(ReadOnlyCollection<Tuple<string, UnitData>> _UnitData, Func<VF_RaidDamageDatabase.UnitData, double> _GetValue
            , Func<Tuple<string, UnitData>, bool> _ValidCheck
            , out double _TotalValue, out double _MaxValue)
        {
            _TotalValue = 0.0;
            _MaxValue = 0.0;
            foreach (var unit in _UnitData)
            {
                double currValue = _GetValue(unit.Item2);
                if (currValue > 0 && _ValidCheck(unit))
                {
                    _TotalValue += currValue;
                    if (currValue > _MaxValue)
                        _MaxValue = currValue;
                }
            }
            return true;
        }
        public static bool CalculateTotalAndMax(Dictionary<string, UnitData> _UnitData, Func<VF_RaidDamageDatabase.UnitData, double> _GetValue
            , Func<KeyValuePair<string, UnitData>, bool> _ValidCheck
            , out double _TotalValue, out double _MaxValue)
        {
            _TotalValue = 0.0;
            _MaxValue = 0.0;
            foreach (var unit in _UnitData)
            {
                double currValue = _GetValue(unit.Value);
                if (currValue > 0 && _ValidCheck(unit))
                {
                    _TotalValue += currValue;
                    if (currValue > _MaxValue)
                        _MaxValue = currValue;
                }
            }
            return true;
        }
        public static UnitData CreateDifference(UnitData _StartUnitData, UnitData _EndUnitData)
        {
            UnitData newUnitData = new UnitData();
            newUnitData.m_UnitID = _StartUnitData.m_UnitID;

            newUnitData.m_Dmg = _EndUnitData.m_Dmg - _StartUnitData.m_Dmg;
            newUnitData.m_EffHeal = _EndUnitData.m_EffHeal - _StartUnitData.m_EffHeal;
            newUnitData.m_DmgTaken = _EndUnitData.m_DmgTaken - _StartUnitData.m_DmgTaken;
            newUnitData.m_OverHeal = _EndUnitData.m_OverHeal - _StartUnitData.m_OverHeal;
            newUnitData.m_DPS = _EndUnitData.m_DPS;// -_StartUnitData.DPS;
            newUnitData.m_HPS = _EndUnitData.m_HPS;// -_StartUnitData.HPS;
            newUnitData.m_Death = _EndUnitData.m_Death - _StartUnitData.m_Death;
            newUnitData.m_Decurse = _EndUnitData.m_Decurse - _StartUnitData.m_Decurse;
            newUnitData.m_DmgCrit = _EndUnitData.m_DmgCrit;// -_StartUnitData.DmgCrit;
            newUnitData.m_HealCrit = _EndUnitData.m_HealCrit;// -_StartUnitData.HealCrit;
            newUnitData.m_EffHealRecv = _EndUnitData.m_EffHealRecv - _StartUnitData.m_EffHealRecv;
            newUnitData.m_OverHealRecv = _EndUnitData.m_OverHealRecv - _StartUnitData.m_OverHealRecv;
            newUnitData.m_RawHeal = _EndUnitData.m_RawHeal - _StartUnitData.m_RawHeal;
            newUnitData.m_RawHealRecv = _EndUnitData.m_RawHealRecv - _StartUnitData.m_RawHealRecv;
            newUnitData.m_ThreatValue = _EndUnitData.m_ThreatValue;// -_StartUnitData.ThreatValue;

            return newUnitData;
        }
        public void _AddPetDataNoClearPet(UnitData _UnitData)
        {//Adds pet data and clears the pets data
            m_Dmg += _UnitData.m_Dmg;
            m_EffHeal += _UnitData.m_EffHeal;
            m_DmgTaken += _UnitData.m_DmgTaken;
            m_OverHeal += _UnitData.m_OverHeal;
            m_DPS += _UnitData.m_DPS;
            m_HPS += _UnitData.m_HPS;
            //Death += _UnitData.Death;
            m_Decurse += _UnitData.m_Decurse;
            //DmgCrit += _UnitData.DmgCrit;
            //HealCrit += _UnitData.HealCrit;
            m_EffHealRecv += _UnitData.m_EffHealRecv;
            m_OverHealRecv += _UnitData.m_OverHealRecv;
            m_RawHeal += _UnitData.m_RawHeal;
            m_RawHealRecv += _UnitData.m_RawHealRecv;
            m_ThreatValue += _UnitData.m_ThreatValue;
        }
        public void AddPetDataAndClearPet(UnitData _UnitData)
        {//Adds pet data and clears the pets data
            m_Dmg += _UnitData.m_Dmg; _UnitData.m_Dmg = 0;
            m_EffHeal += _UnitData.m_EffHeal; _UnitData.m_EffHeal = 0;
            m_DmgTaken += _UnitData.m_DmgTaken; _UnitData.m_DmgTaken = 0;
            m_OverHeal += _UnitData.m_OverHeal; _UnitData.m_OverHeal = 0;
            m_DPS += _UnitData.m_DPS; _UnitData.m_DPS = 0;
            m_HPS += _UnitData.m_HPS; _UnitData.m_HPS = 0;
            //Death += _UnitData.Death;
            m_Decurse += _UnitData.m_Decurse; _UnitData.m_Decurse = 0;
            //DmgCrit += _UnitData.DmgCrit;
            //HealCrit += _UnitData.HealCrit;
            m_EffHealRecv += _UnitData.m_EffHealRecv; _UnitData.m_EffHealRecv = 0;
            m_OverHealRecv += _UnitData.m_OverHealRecv; _UnitData.m_OverHealRecv = 0;
            m_RawHeal += _UnitData.m_RawHeal; _UnitData.m_RawHeal = 0;
            m_RawHealRecv += _UnitData.m_RawHealRecv; _UnitData.m_RawHealRecv = 0;
            m_ThreatValue += _UnitData.m_ThreatValue; _UnitData.m_ThreatValue = 0;
        }
        public void AddUnitData(UnitData _UnitData)
        {
            m_Dmg += _UnitData.m_Dmg;
            m_EffHeal += _UnitData.m_EffHeal;
            m_DmgTaken += _UnitData.m_DmgTaken;
            m_OverHeal += _UnitData.m_OverHeal;
            //DPS += _UnitData.DPS; _UnitData.DPS = 0;
            //HPS += _UnitData.HPS; _UnitData.HPS = 0;
            m_Death += _UnitData.m_Death;
            m_Decurse += _UnitData.m_Decurse;
            //DmgCrit += _UnitData.DmgCrit;
            //HealCrit += _UnitData.HealCrit;
            m_EffHealRecv += _UnitData.m_EffHealRecv;
            m_OverHealRecv += _UnitData.m_OverHealRecv;
            m_RawHeal += _UnitData.m_RawHeal;
            m_RawHealRecv += _UnitData.m_RawHealRecv;
            m_ThreatValue += _UnitData.m_ThreatValue;
        }
        public void SubtractUnitData(UnitData _UnitData)
        {
            m_Dmg -= _UnitData.m_Dmg;
            m_EffHeal -= _UnitData.m_EffHeal;
            m_DmgTaken -= _UnitData.m_DmgTaken;
            m_OverHeal -= _UnitData.m_OverHeal;
            //DPS += _UnitData.DPS; _UnitData.DPS = 0;
            //HPS += _UnitData.HPS; _UnitData.HPS = 0;
            m_Death -= _UnitData.m_Death;
            m_Decurse -= _UnitData.m_Decurse;
            //DmgCrit += _UnitData.DmgCrit;
            //HealCrit += _UnitData.HealCrit;
            m_EffHealRecv -= _UnitData.m_EffHealRecv;
            m_OverHealRecv -= _UnitData.m_OverHealRecv;
            m_RawHeal -= _UnitData.m_RawHeal;
            m_RawHealRecv -= _UnitData.m_RawHealRecv;
            m_ThreatValue -= _UnitData.m_ThreatValue;
        }
        private UnitData()
        { }
        public UnitData CreateCopy()
        {
            UnitData newUnitData = new UnitData();
            newUnitData.m_UnitID = m_UnitID;
            newUnitData.m_Dmg = m_Dmg;
            newUnitData.m_EffHeal = m_EffHeal;
            newUnitData.m_DmgTaken = m_DmgTaken;
            newUnitData.m_OverHeal = m_OverHeal;
            newUnitData.m_DPS = m_DPS;
            newUnitData.m_HPS = m_HPS;
            newUnitData.m_Death = m_Death;
            newUnitData.m_Decurse = m_Decurse;
            newUnitData.m_DmgCrit = m_DmgCrit;
            newUnitData.m_HealCrit = m_HealCrit;
            newUnitData.m_EffHealRecv = m_EffHealRecv;
            newUnitData.m_OverHealRecv = m_OverHealRecv;
            newUnitData.m_RawHeal = m_RawHeal;
            newUnitData.m_RawHealRecv = m_RawHealRecv;
            newUnitData.m_ThreatValue = m_ThreatValue;
            return newUnitData;
        }
        public UnitData(string _DataString, Dictionary<int, UnitData> _UnitDatas)
        {
            _InitData(_DataString, _UnitDatas);   
        }
        private void _InitData(string _DataString, Dictionary<int, UnitData> _UnitDatas)
        {
            string[] splitData = _DataString.Split(new char[] { ' ' }, StringSplitOptions.None);
            try
            {
                m_UnitID = int.Parse(splitData[0]);
                UnitData oldData;
                if (_UnitDatas != null && _UnitDatas.TryGetValue(m_UnitID, out oldData) == true)
                {
                    m_Dmg = MergeDataInt(splitData[1], oldData.m_Dmg);
                    m_EffHeal = MergeDataInt(splitData[2], oldData.m_EffHeal);
                    m_DmgTaken = MergeDataInt(splitData[3], oldData.m_DmgTaken);
                    m_OverHeal = MergeDataInt(splitData[4], oldData.m_OverHeal);
                    m_DPS = MergeDataFloat(splitData[5], 0);
                    m_HPS = MergeDataFloat(splitData[6], 0);
                    m_Death = MergeDataInt(splitData[7], oldData.m_Death);
                    m_Decurse = MergeDataInt(splitData[8], oldData.m_Decurse);
                    m_DmgCrit = MergeDataFloat(splitData[9], 0);
                    m_HealCrit = MergeDataFloat(splitData[10], 0);
                    m_EffHealRecv = MergeDataInt(splitData[11], oldData.m_EffHealRecv);
                    m_OverHealRecv = MergeDataInt(splitData[12], oldData.m_OverHealRecv);
                    m_RawHeal = MergeDataInt(splitData[13], oldData.m_RawHeal);
                    m_RawHealRecv = MergeDataInt(splitData[14], oldData.m_RawHealRecv);
                    m_ThreatValue = MergeDataInt(splitData[15], 0);
                }
                else
                {
                    m_Dmg = MergeDataInt(splitData[1], 0);
                    m_EffHeal = MergeDataInt(splitData[2], 0);
                    m_DmgTaken = MergeDataInt(splitData[3], 0);
                    m_OverHeal = MergeDataInt(splitData[4], 0);
                    m_DPS = MergeDataFloat(splitData[5], 0);
                    m_HPS = MergeDataFloat(splitData[6], 0);
                    m_Death = MergeDataInt(splitData[7], 0);
                    m_Decurse = MergeDataInt(splitData[8], 0);
                    m_DmgCrit = MergeDataFloat(splitData[9], 0);
                    m_HealCrit = MergeDataFloat(splitData[10], 0);
                    m_EffHealRecv = MergeDataInt(splitData[11], 0);
                    m_OverHealRecv = MergeDataInt(splitData[12], 0);
                    m_RawHeal = MergeDataInt(splitData[13], 0);
                    m_RawHealRecv = MergeDataInt(splitData[14], 0);
                    m_ThreatValue = MergeDataInt(splitData[15], 0);
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }
        public static int MergeDataInt(string _DataStr, int _OldData)
        {
            int result = 0;
            if (int.TryParse(_DataStr, out result) == false)
                return _OldData;
            return _OldData + result;
        }
        public static float MergeDataFloat(string _DataStr, float _OldData)
        {
            float result = 0;
            if (float.TryParse(_DataStr.Replace(".", ","), out result) == false)
                return _OldData;
            return _OldData + result;
        }
    }
}
