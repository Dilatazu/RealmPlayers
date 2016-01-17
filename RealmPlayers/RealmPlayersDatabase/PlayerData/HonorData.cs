using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace VF_RealmPlayersDatabase.PlayerData
{
    [ProtoContract]
    public class HonorData
    {
        [ProtoMember(1)]
        public int CurrentRank = 0;
        [ProtoMember(2)]
        public Single CurrentRankProgress = 0;
        [ProtoMember(3)]
        public int TodayHK = 0;
        [ProtoMember(4)]
        public int TodayDK = 0;
        [ProtoMember(5)]
        public int YesterdayHK = 0;
        [ProtoMember(6)]
        public int YesterdayHonor = 0;
        [ProtoMember(7)]
        public int ThisWeekHK = 0;
        [ProtoMember(8)]
        public int ThisWeekHonor = 0;
        [ProtoMember(9)]
        public int LastWeekHK = 0;
        [ProtoMember(10)]
        public int LastWeekHonor = 0;
        [ProtoMember(11)]
        public int LastWeekStanding = int.MaxValue;
        [ProtoMember(12)]
        public int LifetimeHK = 0;
        [ProtoMember(13)]
        public int LifetimeDK = 0;
        [ProtoMember(14)]
        public int LifetimeHighestRank = 0;

        public string GetAsString()
        {
            return "{" + CurrentRank + ", " + CurrentRankProgress + ", " + TodayHK + ", " + TodayDK + ", " + YesterdayHK + ", " + YesterdayHonor + ", " + ThisWeekHK + ", " + ThisWeekHonor + ", " + LastWeekHK + ", " + LastWeekHonor + ", " + LastWeekStanding + ", " + LifetimeHK + ", " + LifetimeDK + ", " + LifetimeHighestRank + "}";
        }

        public int TodayHonorTBC
        {
            //Använd ThisWeekHonor, denna används inte i Vanilla så det är lungt
            get { return ThisWeekHonor; }
            set { ThisWeekHonor = value; }
        }
        public HonorData()
        { }
        public HonorData(System.Xml.XmlNode _PlayerNode, WowVersionEnum _WowVersion)
        {
            InitData(XMLUtility.GetChildValue(_PlayerNode, "HonorData", ""), _WowVersion);
        }
        public HonorData(string _HonorDataString, WowVersionEnum _WowVersion)
        {
            InitData(_HonorDataString, _WowVersion);
        }
        private void InitData(string _HonorDataString, WowVersionEnum _WowVersion)
        {
            try
            {
                if (_HonorDataString == "") return;
                string[] honorData = _HonorDataString.Split(new char[] { ':' });
                if (_WowVersion == WowVersionEnum.Vanilla)
                {
                    CurrentRank = int.Parse(honorData[0]);
                    CurrentRankProgress = Single.Parse(honorData[1].Replace('.', ','));
                    ThisWeekHK = int.Parse(honorData[2]);
                    ThisWeekHonor = int.Parse(honorData[3]);
                    LastWeekHK = int.Parse(honorData[4]);
                    LastWeekHonor = int.Parse(honorData[5]);
                    LastWeekStanding = int.Parse(honorData[6]);
                    if (LastWeekStanding == 0) LastWeekStanding = int.MaxValue;
                    LifetimeHK = int.Parse(honorData[7]);
                    LifetimeDK = int.Parse(honorData[8]);
                    LifetimeHighestRank = int.Parse(honorData[9]);
                    TodayHK = int.Parse(honorData[10]);
                    TodayDK = int.Parse(honorData[11]);
                    YesterdayHK = int.Parse(honorData[12]);
                    YesterdayHonor = int.Parse(honorData[13]);
                }
                else if (_WowVersion == WowVersionEnum.TBC)
                {
                    TodayHK = int.Parse(honorData[0]);
                    TodayHonorTBC = int.Parse(honorData[1]);
                    YesterdayHK = int.Parse(honorData[2]);
                    YesterdayHonor = int.Parse(honorData[3]);
                    LifetimeHK = int.Parse(honorData[4]);
                }
                else
                {
                    throw new Exception("Error, unknown WowVersion: \"" + _WowVersion.ToString() + "\"");
                }
            }
            catch (Exception)
            { }
        }
        public void SetData(HonorData _HonorData)
        {
            CurrentRank = _HonorData.CurrentRank;
            CurrentRankProgress = _HonorData.CurrentRankProgress;
            TodayHK = _HonorData.TodayHK;
            TodayDK = _HonorData.TodayDK;
            YesterdayHK = _HonorData.YesterdayHK;
            YesterdayHonor = _HonorData.YesterdayHonor;
            ThisWeekHK = _HonorData.ThisWeekHK;
            ThisWeekHonor = _HonorData.ThisWeekHonor;
            LastWeekHK = _HonorData.LastWeekHK;
            LastWeekHonor = _HonorData.LastWeekHonor;
            LastWeekStanding = _HonorData.LastWeekStanding;
            LifetimeHK = _HonorData.LifetimeHK;
            LifetimeDK = _HonorData.LifetimeDK;
            LifetimeHighestRank = _HonorData.LifetimeHighestRank;
        }
        public bool IsSame(HonorData _HonorData)
        {
            if(CurrentRank != _HonorData.CurrentRank) return false;
            if (CurrentRankProgress != _HonorData.CurrentRankProgress) return false;
            if (TodayHK != _HonorData.TodayHK) return false;
            if (TodayDK != _HonorData.TodayDK) return false;
            if (YesterdayHK != _HonorData.YesterdayHK) return false;
            if (YesterdayHonor != _HonorData.YesterdayHonor) return false;
            if(ThisWeekHK != _HonorData.ThisWeekHK) return false;
            if(ThisWeekHonor != _HonorData.ThisWeekHonor) return false;
            if(LastWeekHK != _HonorData.LastWeekHK) return false;
            if(LastWeekHonor != _HonorData.LastWeekHonor) return false;
            if(LastWeekStanding != _HonorData.LastWeekStanding) return false;
            if(LifetimeHK != _HonorData.LifetimeHK) return false;
            if(LifetimeDK != _HonorData.LifetimeDK) return false;
            if(LifetimeHighestRank != _HonorData.LifetimeHighestRank) return false;

            return true;
        }
        public bool IsRealisticChange(HonorData _HonorData, TimeSpan _TimeSpan)
        {
            //TODO: Implement
            return true;
        }
        public float GetRankTotal()
        {
            return (float)CurrentRank + CurrentRankProgress;
        }

        //struct rawData
        //{
        //    public byte Data[] = new byte[50];
        //}

        public static UInt32 ReadUInt24(byte[] _Array, int _StartIndex)
        {
            return (UInt32)_Array[_StartIndex] | ((UInt32)_Array[_StartIndex + 1] << 8) | ((UInt32)_Array[_StartIndex + 2] << 16);
        }
        public static void SaveUInt24(ref byte[] _Array, UInt32 _Value, int _StartIndex)
        {
            _Array[_StartIndex] = (byte)(_Value & 0xFF);
            _Array[_StartIndex+1] = (byte)(_Value >> 8);
            _Array[_StartIndex+2] = (byte)(_Value >> 16);
        }

    }
}
