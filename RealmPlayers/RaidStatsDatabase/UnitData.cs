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
    public partial class UnitData
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
        private int m_PM6;//not important
        [ProtoMember(7)]
        private int m_PM7;//not important
        [ProtoMember(8)]
        private int m_Death;
        [ProtoMember(9)]
        private int m_Decurse;
        [ProtoMember(10)]
        private int m_PM10;//not important
        [ProtoMember(11)]
        private int m_PM11;//not important
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

        private struct DataArray
        {
            public static DataArray Create(UnitData _UnitData)
            {
                DataArray newDataArray;
                newDataArray.m_Data = _UnitData;
                return newDataArray;
            }
            private UnitData m_Data;

            public int this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1: return m_Data.m_Dmg;
                        case 2: return m_Data.m_EffHeal;
                        case 3: return m_Data.m_DmgTaken;
                        case 4: return m_Data.m_OverHeal;
                        case 5: return m_Data.m_PM6;
                        case 6: return m_Data.m_PM7;
                        case 7: return m_Data.m_Death;
                        case 8: return m_Data.m_Decurse;
                        case 9: return m_Data.m_PM10;
                        case 10: return m_Data.m_PM11;
                        case 11: return m_Data.m_EffHealRecv;
                        case 12: return m_Data.m_OverHealRecv;
                        case 13: return m_Data.m_RawHeal;
                        case 14: return m_Data.m_RawHealRecv;
                        case 15: return m_Data.m_ThreatValue;
                        default: return 0;
                    }
                }
            }
            public void Set(int i, int _Value)
            {
                switch (i)
                {
                    case 1: m_Data.m_Dmg = _Value; return;
                    case 2: m_Data.m_EffHeal = _Value; return;
                    case 3: m_Data.m_DmgTaken = _Value; return;
                    case 4: m_Data.m_OverHeal = _Value; return;
                    case 5: m_Data.m_PM6 = _Value; return;
                    case 6: m_Data.m_PM7 = _Value; return;
                    case 7: m_Data.m_Death = _Value; return;
                    case 8: m_Data.m_Decurse = _Value; return;
                    case 9: m_Data.m_PM10 = _Value; return;
                    case 10: m_Data.m_PM11 = _Value; return;
                    case 11: m_Data.m_EffHealRecv = _Value; return;
                    case 12: m_Data.m_OverHealRecv = _Value; return;
                    case 13: m_Data.m_RawHeal = _Value; return;
                    case 14: m_Data.m_RawHealRecv = _Value; return;
                    case 15: m_Data.m_ThreatValue = _Value; return;
                    default: return;
                }
            }
            public const int Count = 16;
            public const int Count_Accum = 15;

            public const int Index_Dmg = 1;
            public const int Index_EffHeal = 2;
            public const int Index_DmgTaken = 3;
            public const int Index_OverHeal = 4;
            public const int Index_FriendlyDmg_TBC = 5;
            public const int Index_CCBReaks_TBC = 6;
            public const int Index_Death = 7;
            public const int Index_Decurse = 8;
            public const int Index_Interrupts_TBC = 9;
            public const int Index_Dispelled_TBC = 10;
            public const int Index_EffHealRecv = 11;
            public const int Index_OverHealRecv = 12;
            public const int Index_Threat = 15;
        }
        private DataArray m_Data
        {
            get { return DataArray.Create(this); }
        }

        public static UnitData Create(string _DataString, Dictionary<int, UnitData> _UnitDatas, WowVersionEnum _WowVersion)
        {
            UnitData newData = new UnitData();
            if(_WowVersion == WowVersionEnum.Vanilla)
            {
                C_InterfaceVanilla.SetAccumParseData(newData, _DataString, _UnitDatas);
                return newData;
            }
            else //if (_WowVersion == WowVersionEnum.TBC)
            {
                C_InterfaceTBC.SetAccumParseData(newData, _DataString, _UnitDatas);
                return newData;
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

            for (int i = 1; i < DataArray.Count_Accum; ++i)
            {
                newUnitData.m_Data.Set(i, _EndUnitData.m_Data[i] - _StartUnitData.m_Data[i]);
            }
            newUnitData.I.SetThreatValue(_EndUnitData.m_ThreatValue);

            return newUnitData;
        }
        public void _AddPetDataNoClearPet(UnitData _UnitData)
        {//Adds pet data and clears the pets data
            for (int i = 1; i < DataArray.Count; ++i)
            {
                if (i == DataArray.Index_Death)
                    continue;//Skip Death
                m_Data.Set(i, m_Data[i] + _UnitData.m_Data[i]);
            }
        }
        public void AddPetDataAndClearPet(UnitData _UnitData)
        {//Adds pet data and clears the pets data
            for (int i = 1; i < DataArray.Count; ++i)
            {
                if (i == DataArray.Index_Death)
                    continue;//Skip Death
                m_Data.Set(i, m_Data[i] + _UnitData.m_Data[i]);
                _UnitData.m_Data.Set(i, 0);
            }
        }
        public void AddUnitData(UnitData _UnitData)
        {
            for (int i = 1; i < DataArray.Count; ++i)
            {
                m_Data.Set(i, m_Data[i] + _UnitData.m_Data[i]);
            }
        }
        public void SubtractUnitData(UnitData _UnitData)
        {
            for (int i = 1; i < DataArray.Count; ++i)
            {
                m_Data.Set(i, m_Data[i] - _UnitData.m_Data[i]);
            }
        }
        private UnitData()
        { }
        public UnitData CreateCopy()
        {
            UnitData newUnitData = new UnitData();
            newUnitData.m_UnitID = m_UnitID;
            for (int i = 1; i < DataArray.Count; ++i)
            {
                newUnitData.m_Data.Set(i, m_Data[i]);
            }
            return newUnitData;
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
