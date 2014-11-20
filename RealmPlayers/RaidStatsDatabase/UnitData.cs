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
    public class UnitData : ISerializable
    {
        public static char UnitData_VERSION = (char)1;
        [ProtoMember(1)]
        public int UnitID;
        [ProtoMember(2)]
        public int Dmg;
        [ProtoMember(3)]
        public int EffHeal;
        [ProtoMember(4)]
        public int DmgTaken;
        [ProtoMember(5)]
        public int OverHeal;
        [ProtoMember(6)]
        public float DPS;
        [ProtoMember(7)]
        public float HPS;
        [ProtoMember(8)]
        public int Death;
        [ProtoMember(9)]
        public int Decurse;
        [ProtoMember(10)]
        public float DmgCrit;
        [ProtoMember(11)]
        public float HealCrit;
        [ProtoMember(12)]
        public int EffHealRecv;
        [ProtoMember(13)]
        public int OverHealRecv;
        [ProtoMember(14)]
        public int RawHeal;
        [ProtoMember(15)]
        public int RawHealRecv;
        [ProtoMember(16)]
        public int ThreatValue;

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
            newUnitData.UnitID = _StartUnitData.UnitID;

            newUnitData.Dmg = _EndUnitData.Dmg - _StartUnitData.Dmg;
            newUnitData.EffHeal = _EndUnitData.EffHeal - _StartUnitData.EffHeal;
            newUnitData.DmgTaken = _EndUnitData.DmgTaken - _StartUnitData.DmgTaken;
            newUnitData.OverHeal = _EndUnitData.OverHeal - _StartUnitData.OverHeal;
            newUnitData.DPS = _EndUnitData.DPS;// -_StartUnitData.DPS;
            newUnitData.HPS = _EndUnitData.HPS;// -_StartUnitData.HPS;
            newUnitData.Death = _EndUnitData.Death - _StartUnitData.Death;
            newUnitData.Decurse = _EndUnitData.Decurse - _StartUnitData.Decurse;
            newUnitData.DmgCrit = _EndUnitData.DmgCrit;// -_StartUnitData.DmgCrit;
            newUnitData.HealCrit = _EndUnitData.HealCrit;// -_StartUnitData.HealCrit;
            newUnitData.EffHealRecv = _EndUnitData.EffHealRecv - _StartUnitData.EffHealRecv;
            newUnitData.OverHealRecv = _EndUnitData.OverHealRecv - _StartUnitData.OverHealRecv;
            newUnitData.RawHeal = _EndUnitData.RawHeal - _StartUnitData.RawHeal;
            newUnitData.RawHealRecv = _EndUnitData.RawHealRecv - _StartUnitData.RawHealRecv;
            newUnitData.ThreatValue = _EndUnitData.ThreatValue;// -_StartUnitData.ThreatValue;

            return newUnitData;
        }
        public void _AddPetDataNoClearPet(UnitData _UnitData)
        {//Adds pet data and clears the pets data
            Dmg += _UnitData.Dmg; 
            EffHeal += _UnitData.EffHeal;
            DmgTaken += _UnitData.DmgTaken;
            OverHeal += _UnitData.OverHeal;
            DPS += _UnitData.DPS;
            HPS += _UnitData.HPS;
            //Death += _UnitData.Death;
            Decurse += _UnitData.Decurse;
            //DmgCrit += _UnitData.DmgCrit;
            //HealCrit += _UnitData.HealCrit;
            EffHealRecv += _UnitData.EffHealRecv;
            OverHealRecv += _UnitData.OverHealRecv;
            RawHeal += _UnitData.RawHeal;
            RawHealRecv += _UnitData.RawHealRecv;
            ThreatValue += _UnitData.ThreatValue;
        }
        public void AddPetDataAndClearPet(UnitData _UnitData)
        {//Adds pet data and clears the pets data
            Dmg += _UnitData.Dmg; _UnitData.Dmg = 0;
            EffHeal += _UnitData.EffHeal; _UnitData.EffHeal = 0;
            DmgTaken += _UnitData.DmgTaken; _UnitData.DmgTaken = 0;
            OverHeal += _UnitData.OverHeal; _UnitData.OverHeal = 0;
            DPS += _UnitData.DPS; _UnitData.DPS = 0;
            HPS += _UnitData.HPS; _UnitData.HPS = 0;
            //Death += _UnitData.Death;
            Decurse += _UnitData.Decurse; _UnitData.Decurse = 0;
            //DmgCrit += _UnitData.DmgCrit;
            //HealCrit += _UnitData.HealCrit;
            EffHealRecv += _UnitData.EffHealRecv; _UnitData.EffHealRecv = 0;
            OverHealRecv += _UnitData.OverHealRecv; _UnitData.OverHealRecv = 0;
            RawHeal += _UnitData.RawHeal; _UnitData.RawHeal = 0;
            RawHealRecv += _UnitData.RawHealRecv; _UnitData.RawHealRecv = 0;
            ThreatValue += _UnitData.ThreatValue; _UnitData.ThreatValue = 0;
        }
        public void AddUnitData(UnitData _UnitData)
        {
            Dmg += _UnitData.Dmg;
            EffHeal += _UnitData.EffHeal;
            DmgTaken += _UnitData.DmgTaken;
            OverHeal += _UnitData.OverHeal;
            //DPS += _UnitData.DPS; _UnitData.DPS = 0;
            //HPS += _UnitData.HPS; _UnitData.HPS = 0;
            Death += _UnitData.Death;
            Decurse += _UnitData.Decurse;
            //DmgCrit += _UnitData.DmgCrit;
            //HealCrit += _UnitData.HealCrit;
            EffHealRecv += _UnitData.EffHealRecv;
            OverHealRecv += _UnitData.OverHealRecv;
            RawHeal += _UnitData.RawHeal;
            RawHealRecv += _UnitData.RawHealRecv;
            ThreatValue += _UnitData.ThreatValue;
        }
        public void SubtractUnitData(UnitData _UnitData)
        {
            Dmg -= _UnitData.Dmg;
            EffHeal -= _UnitData.EffHeal;
            DmgTaken -= _UnitData.DmgTaken;
            OverHeal -= _UnitData.OverHeal;
            //DPS += _UnitData.DPS; _UnitData.DPS = 0;
            //HPS += _UnitData.HPS; _UnitData.HPS = 0;
            Death -= _UnitData.Death;
            Decurse -= _UnitData.Decurse;
            //DmgCrit += _UnitData.DmgCrit;
            //HealCrit += _UnitData.HealCrit;
            EffHealRecv -= _UnitData.EffHealRecv;
            OverHealRecv -= _UnitData.OverHealRecv;
            RawHeal -= _UnitData.RawHeal;
            RawHealRecv -= _UnitData.RawHealRecv;
            ThreatValue -= _UnitData.ThreatValue;
        }
        private UnitData()
        { }
        public UnitData CreateCopy()
        {
            UnitData newUnitData = new UnitData();
            newUnitData.UnitID = UnitID;
            newUnitData.Dmg = Dmg;
            newUnitData.EffHeal = EffHeal;
            newUnitData.DmgTaken = DmgTaken;
            newUnitData.OverHeal = OverHeal;
            newUnitData.DPS = DPS;
            newUnitData.HPS = HPS;
            newUnitData.Death = Death;
            newUnitData.Decurse = Decurse;
            newUnitData.DmgCrit = DmgCrit;
            newUnitData.HealCrit = HealCrit;
            newUnitData.EffHealRecv = EffHealRecv;
            newUnitData.OverHealRecv = OverHealRecv;
            newUnitData.RawHeal = RawHeal;
            newUnitData.RawHealRecv = RawHealRecv;
            newUnitData.ThreatValue = ThreatValue;
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
                UnitID = int.Parse(splitData[0]);
                UnitData oldData;
                if (_UnitDatas != null && _UnitDatas.TryGetValue(UnitID, out oldData) == true)
                {
                    Dmg = MergeDataInt(splitData[1], oldData.Dmg);
                    EffHeal = MergeDataInt(splitData[2], oldData.EffHeal);
                    DmgTaken = MergeDataInt(splitData[3], oldData.DmgTaken);
                    OverHeal = MergeDataInt(splitData[4], oldData.OverHeal);
                    DPS = MergeDataFloat(splitData[5], 0);
                    HPS = MergeDataFloat(splitData[6], 0);
                    Death = MergeDataInt(splitData[7], oldData.Death);
                    Decurse = MergeDataInt(splitData[8], oldData.Decurse);
                    DmgCrit = MergeDataFloat(splitData[9], 0);
                    HealCrit = MergeDataFloat(splitData[10], 0);
                    EffHealRecv = MergeDataInt(splitData[11], oldData.EffHealRecv);
                    OverHealRecv = MergeDataInt(splitData[12], oldData.OverHealRecv);
                    RawHeal = MergeDataInt(splitData[13], oldData.RawHeal);
                    RawHealRecv = MergeDataInt(splitData[14], oldData.RawHealRecv);
                    ThreatValue = MergeDataInt(splitData[15], 0);
                }
                else
                {
                    Dmg = MergeDataInt(splitData[1], 0);
                    EffHeal = MergeDataInt(splitData[2], 0);
                    DmgTaken = MergeDataInt(splitData[3], 0);
                    OverHeal = MergeDataInt(splitData[4], 0);
                    DPS = MergeDataFloat(splitData[5], 0);
                    HPS = MergeDataFloat(splitData[6], 0);
                    Death = MergeDataInt(splitData[7], 0);
                    Decurse = MergeDataInt(splitData[8], 0);
                    DmgCrit = MergeDataFloat(splitData[9], 0);
                    HealCrit = MergeDataFloat(splitData[10], 0);
                    EffHealRecv = MergeDataInt(splitData[11], 0);
                    OverHealRecv = MergeDataInt(splitData[12], 0);
                    RawHeal = MergeDataInt(splitData[13], 0);
                    RawHealRecv = MergeDataInt(splitData[14], 0);
                    ThreatValue = MergeDataInt(splitData[15], 0);
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }
        public string CreateDataString()
        {
            return UnitID + " " + (Dmg == 0 ? "" : Dmg.ToString())
                + " " + (EffHeal == 0 ? "" : EffHeal.ToString())
                + " " + (DmgTaken == 0 ? "" : DmgTaken.ToString())
                + " " + (OverHeal == 0 ? "" : OverHeal.ToString())
                + " " + (DPS == 0 ? "" : DPS.ToString())
                + " " + (HPS == 0 ? "" : HPS.ToString())
                + " " + (Death == 0 ? "" : Death.ToString())
                + " " + (Decurse == 0 ? "" : Decurse.ToString())
                + " " + (DmgCrit == 0 ? "" : DmgCrit.ToString())
                + " " + (HealCrit == 0 ? "" : HealCrit.ToString())
                + " " + (EffHealRecv == 0 ? "" : EffHealRecv.ToString())
                + " " + (OverHealRecv == 0 ? "" : OverHealRecv.ToString())
                + " " + (RawHeal == 0 ? "" : RawHeal.ToString())
                + " " + (RawHealRecv == 0 ? "" : RawHealRecv.ToString())
                + " " + (ThreatValue == 0 ? "" : ThreatValue.ToString());
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

        #region Serializing
        public UnitData(SerializationInfo _Info, StreamingContext _Context)
        {
            //_InitData(_Info.GetString("DataString"), null);
            UnitID = _Info.GetInt32("UnitID");
            Dmg = _Info.GetInt32("Dmg");
            EffHeal = _Info.GetInt32("EffHeal");
            DmgTaken = _Info.GetInt32("DmgTaken");
            OverHeal = _Info.GetInt32("OverHeal");
            DPS = _Info.GetSingle("DPS");
            HPS = _Info.GetSingle("HPS");
            Death = _Info.GetInt32("Death");
            Decurse = _Info.GetInt32("Decurse");
            DmgCrit = _Info.GetSingle("DmgCrit");
            HealCrit = _Info.GetSingle("HealCrit");
            EffHealRecv = _Info.GetInt32("EffHealRecv");
            OverHealRecv = _Info.GetInt32("OverHealRecv");
            RawHeal = _Info.GetInt32("RawHeal");
            RawHealRecv = _Info.GetInt32("RawHealRecv");
            ThreatValue = _Info.GetInt32("ThreatValue");
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            //_Info.AddValue("DataString", CreateDataString());
            _Info.AddValue("Version", UnitData_VERSION);
            _Info.AddValue("UnitID", UnitID);
            _Info.AddValue("Dmg", Dmg);
            _Info.AddValue("EffHeal", EffHeal);
            _Info.AddValue("DmgTaken", DmgTaken);
            _Info.AddValue("OverHeal", OverHeal);
            _Info.AddValue("DPS", DPS);
            _Info.AddValue("HPS", HPS);
            _Info.AddValue("Death", Death);
            _Info.AddValue("Decurse", Decurse);
            _Info.AddValue("DmgCrit", DmgCrit);
            _Info.AddValue("HealCrit", HealCrit);
            _Info.AddValue("EffHealRecv", EffHealRecv);
            _Info.AddValue("OverHealRecv", OverHealRecv);
            _Info.AddValue("RawHeal", RawHeal);
            _Info.AddValue("RawHealRecv", RawHealRecv);
            _Info.AddValue("ThreatValue", ThreatValue);
        }
        #endregion

        //public bool IsSame(UnitData _UnitData)
        //{
        //    if (UnitID != _UnitData.UnitID || Dmg != _UnitData.Dmg 
        //    || EffHeal != _UnitData.EffHeal || OverHeal != _UnitData.OverHeal || RawHeal != _UnitData.RawHeal)
        //        return false;

        //    if(DmgTaken != _UnitData.DmgTaken || 
        //    newUnitData.DmgTaken = DmgTaken;
        //    newUnitData.DPS = DPS;
        //    newUnitData.HPS = HPS;
        //    newUnitData.Death = Death;
        //    newUnitData.Decurse = Decurse;
        //    newUnitData.DmgCrit = DmgCrit;
        //    newUnitData.HealCrit = HealCrit;
        //    newUnitData.EffHealRecv = EffHealRecv;
        //    newUnitData.OverHealRecv = OverHealRecv;
        //    newUnitData.RawHealRecv = RawHealRecv;
        //    newUnitData.ThreatValue = ThreatValue;
        //}
    }
}
