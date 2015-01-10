using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RaidDamageDatabase
{
    public partial class UnitData
    {

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
            public void SetDmg(int _Dmg) { m_Data.m_Data.Set(DataArray.Index_Dmg, _Dmg); }
            public void SetEffHeal(int _EffHeal) { m_Data.m_Data.Set(DataArray.Index_EffHeal, _EffHeal); }
            public void SetDmgTaken(int _DmgTaken) { m_Data.m_Data.Set(DataArray.Index_DmgTaken, _DmgTaken); }
            public void SetOverHeal(int _OverHeal) { m_Data.m_Data.Set(DataArray.Index_OverHeal, _OverHeal); }
            public void SetDeath(int _Death) { m_Data.m_Data.Set(DataArray.Index_Death, _Death); }
            public void SetDecurse(int _Decurse) { m_Data.m_Data.Set(DataArray.Index_Decurse, _Decurse); }
            public void SetEffHealRecv(int _EffHealRecv) { m_Data.m_Data.Set(DataArray.Index_EffHealRecv, _EffHealRecv); }
            public void SetOverHealRecv(int _OverHealRecv) { m_Data.m_Data.Set(DataArray.Index_OverHealRecv, _OverHealRecv); }
            public void SetThreatValue(int _NewThreatValue) { m_Data.m_Data.Set(DataArray.Index_Threat, _NewThreatValue); }

            public int UnitID { get { return m_Data.m_UnitID; } }
            public int Dmg { get { return m_Data.m_Data[DataArray.Index_Dmg]; } }
            public int EffHeal { get { return m_Data.m_Data[DataArray.Index_EffHeal]; } }
            public int DmgTaken { get { return m_Data.m_Data[DataArray.Index_DmgTaken]; } }
            public int OverHeal { get { return m_Data.m_Data[DataArray.Index_OverHeal]; } }
            public int Death { get { return m_Data.m_Data[DataArray.Index_Death]; } }
            public int Decurse { get { return m_Data.m_Data[DataArray.Index_Decurse]; } }
            public int EffHealRecv { get { return m_Data.m_Data[DataArray.Index_EffHealRecv]; } }
            public int OverHealRecv { get { return m_Data.m_Data[DataArray.Index_OverHealRecv]; } }
            public int ThreatValue { get { return m_Data.m_Data[DataArray.Index_Threat]; } }

            public int RawHeal { get { return EffHeal + OverHeal; } }
            public int RawHealRecv { get { return EffHealRecv + OverHealRecv; } }
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

            public string CreateDataString()
            {
                return m_Data.I.UnitID + " " + (m_Data.I.Dmg == 0 ? "" : m_Data.I.Dmg.ToString())
                    + " " + (m_Data.I.EffHeal == 0 ? "" : m_Data.I.EffHeal.ToString())
                    + " " + (m_Data.I.DmgTaken == 0 ? "" : m_Data.I.DmgTaken.ToString())
                    + " " + (m_Data.I.OverHeal == 0 ? "" : m_Data.I.OverHeal.ToString())
                    + " " //+ (DPS == 0 ? "" : DPS.ToString())
                    + " " //+ (HPS == 0 ? "" : HPS.ToString())
                    + " " + (m_Data.I.Death == 0 ? "" : m_Data.I.Death.ToString())
                    + " " + (m_Data.I.Decurse == 0 ? "" : m_Data.I.Decurse.ToString())
                    + " " //+ (DmgCrit == 0 ? "" : DmgCrit.ToString())
                    + " " //+ (HealCrit == 0 ? "" : HealCrit.ToString())
                    + " " + (m_Data.I.EffHealRecv == 0 ? "" : m_Data.I.EffHealRecv.ToString())
                    + " " + (m_Data.I.OverHealRecv == 0 ? "" : m_Data.I.OverHealRecv.ToString())
                    + " " + (m_Data.I.RawHeal == 0 ? "" : m_Data.I.RawHeal.ToString())
                    + " " + (m_Data.I.RawHealRecv == 0 ? "" : m_Data.I.RawHealRecv.ToString())
                    + " " + (m_Data.I.ThreatValue == 0 ? "" : m_Data.I.ThreatValue.ToString());
            }
            public static void SetAccumParseData(UnitData _UnitData, string _DataString, Dictionary<int, UnitData> _UnitDatas)
            {
                string[] splitData = _DataString.Split(new char[] { ' ' }, StringSplitOptions.None);
                try
                {
                    _UnitData.m_UnitID = int.Parse(splitData[0]);
                    UnitData oldData;
                    if (_UnitDatas != null && _UnitDatas.TryGetValue(_UnitData.m_UnitID, out oldData) == true)
                    {
                        //Accumulate data to last
                        _UnitData.I.SetDmg(MergeDataInt(splitData[1], oldData.I.Dmg));
                        _UnitData.I.SetEffHeal(MergeDataInt(splitData[2], oldData.I.EffHeal));
                        _UnitData.I.SetDmgTaken(MergeDataInt(splitData[3], oldData.I.DmgTaken));
                        _UnitData.I.SetOverHeal(MergeDataInt(splitData[4], oldData.I.OverHeal));
                        //5 = DPS
                        //6 = HPS
                        _UnitData.I.SetDeath(MergeDataInt(splitData[7], oldData.I.Death));
                        _UnitData.I.SetDecurse(MergeDataInt(splitData[8], oldData.I.Decurse));
                        //9 = DmgCrit
                        //10 = HealCrit
                        _UnitData.I.SetEffHealRecv(MergeDataInt(splitData[11], oldData.I.EffHealRecv));
                        _UnitData.I.SetOverHealRecv(MergeDataInt(splitData[12], oldData.I.OverHealRecv));
                        int rawHeal = MergeDataInt(splitData[13], 0);
                        int rawHealRecv = MergeDataInt(splitData[14], 0);
                        if (rawHeal != _UnitData.I.EffHeal + _UnitData.I.OverHeal
                        || rawHealRecv != _UnitData.I.EffHealRecv + _UnitData.I.OverHealRecv)
                        {
                            //Error?
                        }
                        _UnitData.I.SetThreatValue(MergeDataInt(splitData[15], 0));
                    }
                    else
                    {
                        //This is first data so no accumulation is done
                        _UnitData.I.SetDmg(MergeDataInt(splitData[1], 0));
                        _UnitData.I.SetEffHeal(MergeDataInt(splitData[2], 0));
                        _UnitData.I.SetDmgTaken(MergeDataInt(splitData[3], 0));
                        _UnitData.I.SetOverHeal(MergeDataInt(splitData[4], 0));
                        _UnitData.I.SetDeath(MergeDataInt(splitData[7], 0));
                        _UnitData.I.SetDecurse(MergeDataInt(splitData[8], 0));
                        _UnitData.I.SetEffHealRecv(MergeDataInt(splitData[11], 0));
                        _UnitData.I.SetOverHealRecv(MergeDataInt(splitData[12], 0));
                        int rawHeal = MergeDataInt(splitData[13], 0);
                        int rawHealRecv = MergeDataInt(splitData[14], 0);
                        if (rawHeal != _UnitData.I.EffHeal + _UnitData.I.OverHeal
                        || rawHealRecv != _UnitData.I.EffHealRecv + _UnitData.I.OverHealRecv)
                        {
                            //Error?
                        }
                        _UnitData.I.SetThreatValue(MergeDataInt(splitData[15], 0));
                    }
                }
                catch (Exception)
                {
                    //throw;
                }
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

            public int FriendlyDmg { get { return m_Data.m_Data[DataArray.Index_FriendlyDmg_TBC]; } }
            public int CCBreaks { get { return m_Data.m_Data[DataArray.Index_CCBReaks_TBC]; } }
            public int Interrupts { get { return m_Data.m_Data[DataArray.Index_Interrupts_TBC]; } }
            public int Dispelled { get { return m_Data.m_Data[DataArray.Index_Dispelled_TBC]; } }
            public void SetFriendlyDmg(int _FriendlyDmg) { m_Data.m_Data.Set(DataArray.Index_FriendlyDmg_TBC, _FriendlyDmg); }
            public void SetCCBreaks(int _CCBreaks) { m_Data.m_Data.Set(DataArray.Index_CCBReaks_TBC, _CCBreaks); }
            public void SetInterrupts(int _Interrupts) { m_Data.m_Data.Set(DataArray.Index_Interrupts_TBC, _Interrupts); }
            public void SetDispelled(int _Dispelled) { m_Data.m_Data.Set(DataArray.Index_Dispelled_TBC, _Dispelled); }

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
            public static void SetAccumParseData(UnitData _UnitData, string _DataString, Dictionary<int, UnitData> _UnitDatas)
            {
                string[] splitData = _DataString.Split(new char[] { ' ' }, StringSplitOptions.None);
                try
                {
                    _UnitData.m_UnitID = int.Parse(splitData[0]);
                    UnitData oldData;
                    if (_UnitDatas != null && _UnitDatas.TryGetValue(_UnitData.m_UnitID, out oldData) == true)
                    {
                        //Accumulate data to last
                        _UnitData.I.SetDmg(MergeDataInt(splitData[1], oldData.I.Dmg));
                        _UnitData.I.SetEffHeal(MergeDataInt(splitData[2], oldData.I.EffHeal));
                        _UnitData.I.SetDmgTaken(MergeDataInt(splitData[3], oldData.I.DmgTaken));
                        _UnitData.I.SetOverHeal(MergeDataInt(splitData[4], oldData.I.OverHeal));
                        _UnitData.I.SetDeath(MergeDataInt(splitData[5], oldData.I.Death));
                        _UnitData.I2.SetFriendlyDmg(MergeDataInt(splitData[6], oldData.I2.FriendlyDmg));
                        _UnitData.I.SetDecurse(MergeDataInt(splitData[7], oldData.I.Decurse));
                        _UnitData.I2.SetCCBreaks(MergeDataInt(splitData[8], oldData.I2.CCBreaks));
                        _UnitData.I2.SetInterrupts(MergeDataInt(splitData[9], oldData.I2.Interrupts));
                        _UnitData.I2.SetDispelled(MergeDataInt(splitData[10], oldData.I2.Dispelled));
                        _UnitData.I.SetEffHealRecv(MergeDataInt(splitData[11], oldData.I.EffHealRecv));
                        _UnitData.I.SetOverHealRecv(MergeDataInt(splitData[12], oldData.I.OverHealRecv));
                        _UnitData.I.SetThreatValue(MergeDataInt(splitData[13], 0));
                    }
                    else
                    {
                        //This is first data so no accumulation is done
                        _UnitData.I.SetDmg(MergeDataInt(splitData[1], 0));
                        _UnitData.I.SetEffHeal(MergeDataInt(splitData[2], 0));
                        _UnitData.I.SetDmgTaken(MergeDataInt(splitData[3], 0));
                        _UnitData.I.SetOverHeal(MergeDataInt(splitData[4], 0));
                        _UnitData.I.SetDeath(MergeDataInt(splitData[5], 0));
                        _UnitData.I2.SetFriendlyDmg(MergeDataInt(splitData[6], 0));
                        _UnitData.I.SetDecurse(MergeDataInt(splitData[7], 0));
                        _UnitData.I2.SetCCBreaks(MergeDataInt(splitData[8], 0));
                        _UnitData.I2.SetInterrupts(MergeDataInt(splitData[9], 0));
                        _UnitData.I2.SetDispelled(MergeDataInt(splitData[10], 0));
                        _UnitData.I.SetEffHealRecv(MergeDataInt(splitData[11], 0));
                        _UnitData.I.SetOverHealRecv(MergeDataInt(splitData[12], 0));
                        _UnitData.I.SetThreatValue(MergeDataInt(splitData[13], 0));
                    }
                }
                catch (Exception)
                {
                    //throw;
                }
            }
        }
    }
}
