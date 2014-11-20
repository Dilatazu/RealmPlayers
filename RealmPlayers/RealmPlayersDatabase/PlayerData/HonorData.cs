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
    [Serializable()]
    public class HonorData : ISerializable
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

        #region Serializing
        public HonorData(SerializationInfo _Info, StreamingContext _Context)
        {
            Single dataVersion = 1.3f;
            foreach (SerializationEntry entry in _Info)
            {
                switch (entry.Name)
                {
                    case "TodayHK":
                    case "TodayDK":
                    case "YesterdayHK":
                    case "YesterdayHonor":
                        if(dataVersion < 1.4f)
                            dataVersion = 1.4f;
                        break;
                    case "DataVersion":
                        dataVersion = 2.0f;
                        break;
                }
                if (dataVersion == 2.0f)
                    break;
            }
            if (dataVersion == 2.0f)
            {
                byte realVersion = _Info.GetByte("DataVersion");
                if (realVersion == 1) //11 727KB
                {
                    CurrentRank = _Info.GetByte("CurrentRank");
                    CurrentRankProgress = _Info.GetSingle("CurrentRankProgress");

                    TodayHK = _Info.GetUInt16("TodayHK");
                    TodayDK = _Info.GetUInt16("TodayDK");
                    YesterdayHK = _Info.GetUInt16("YesterdayHK");
                    YesterdayHonor = _Info.GetInt32("YesterdayHonor");

                    ThisWeekHK = _Info.GetUInt16("ThisWeekHK");
                    ThisWeekHonor = _Info.GetInt32("ThisWeekHonor");
                    LastWeekHK = _Info.GetUInt16("LastWeekHK");
                    LastWeekHonor = _Info.GetInt32("LastWeekHonor");
                    LastWeekStanding = _Info.GetUInt16("LastWeekStanding");
                    LifetimeHK = _Info.GetInt32("LifetimeHK");
                    LifetimeDK = _Info.GetInt32("LifetimeDK");
                    LifetimeHighestRank = _Info.GetByte("LifetimeHighestRank");
                }
                else if (realVersion == 2) //11 766KB
                {
                    throw new Exception("Shouldnt happen!");
                    //byte[] rawDataArray = (byte[])_Info.GetValue("RawData", typeof(byte[]));
                    //int bC = 0;
                    //CurrentRank = rawDataArray[bC++];
                    //CurrentRankProgress = (float)rawDataArray[bC++] + ((float)rawDataArray[bC++] / 100.0f);

                    //TodayHK = BitConverter.ToUInt16(rawDataArray, bC); bC += 2;
                    //TodayDK = BitConverter.ToUInt16(rawDataArray, bC); bC += 2;
                    //YesterdayHK = BitConverter.ToUInt16(rawDataArray, bC); bC += 2;
                    //YesterdayHonor = (int)ReadUInt24(rawDataArray, bC); bC += 3;

                    //ThisWeekHK = BitConverter.ToUInt16(rawDataArray, bC); bC += 2;
                    //ThisWeekHonor = (int)ReadUInt24(rawDataArray, bC); bC += 3;

                    //LastWeekHK = BitConverter.ToUInt16(rawDataArray, bC); bC += 2;
                    //LastWeekHonor = (int)ReadUInt24(rawDataArray, bC); bC += 3;
                    //LastWeekStanding = BitConverter.ToUInt16(rawDataArray, bC); bC += 2;
                    //LifetimeHK = (int)ReadUInt24(rawDataArray, bC); bC += 3;
                    //LifetimeDK = (int)ReadUInt24(rawDataArray, bC); bC += 3;
                    //LifetimeHighestRank = rawDataArray[bC++];
                }
                else
                    throw new Exception("Shouldnt happen!");
            }
            else //11 732KB
            {
                CurrentRank = _Info.GetInt32("CurrentRank");
                CurrentRankProgress = _Info.GetSingle("CurrentRankProgress");
                if (dataVersion >= 1.4f)
                {
                    TodayHK = _Info.GetInt32("TodayHK");
                    TodayDK = _Info.GetInt32("TodayDK");
                    YesterdayHK = _Info.GetInt32("YesterdayHK");
                    YesterdayHonor = _Info.GetInt32("YesterdayHonor");
                }
                else
                {
                    TodayHK = 0;
                    TodayDK = 0;
                    YesterdayHK = 0;
                    YesterdayHonor = 0;
                }
                ThisWeekHK = _Info.GetInt32("ThisWeekHK");
                ThisWeekHonor = _Info.GetInt32("ThisWeekHonor");
                LastWeekHK = _Info.GetInt32("LastWeekHK");
                LastWeekHonor = _Info.GetInt32("LastWeekHonor");
                LastWeekStanding = _Info.GetInt32("LastWeekStanding");
                LifetimeHK = _Info.GetInt32("LifetimeHK");
                LifetimeDK = _Info.GetInt32("LifetimeDK");
                LifetimeHighestRank = _Info.GetInt32("LifetimeHighestRank");
            }
        }

        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            //DataVersion 2.0f + 2 här == 11 766KB
            {
                //_Info.AddValue("DataVersion", (byte)2);
                //byte[] rawDataArray = new byte[31];
                //int bC = 0;
                //rawDataArray[bC++] = (byte)CurrentRank;
                //rawDataArray[bC++] = (byte)((int)CurrentRankProgress);
                //rawDataArray[bC++] = 0;//TODO

                //var addBytes = BitConverter.GetBytes((UInt16)TodayHK);
                //rawDataArray[bC++] = addBytes[0]; rawDataArray[bC++] = addBytes[1];
                //addBytes = BitConverter.GetBytes((UInt16)TodayDK);
                //rawDataArray[bC++] = addBytes[0]; rawDataArray[bC++] = addBytes[1];
                //addBytes = BitConverter.GetBytes((UInt16)YesterdayHK);
                //rawDataArray[bC++] = addBytes[0]; rawDataArray[bC++] = addBytes[1];
                //SaveUInt24(ref rawDataArray, (uint)YesterdayHonor, bC); bC += 3;

                //addBytes = BitConverter.GetBytes((UInt16)ThisWeekHK);
                //rawDataArray[bC++] = addBytes[0]; rawDataArray[bC++] = addBytes[1];
                //SaveUInt24(ref rawDataArray, (uint)ThisWeekHonor, bC); bC += 3;

                //addBytes = BitConverter.GetBytes((UInt16)LastWeekHK);
                //rawDataArray[bC++] = addBytes[0]; rawDataArray[bC++] = addBytes[1];
                //SaveUInt24(ref rawDataArray, (uint)LastWeekHonor, bC); bC += 3;
                //addBytes = BitConverter.GetBytes((UInt16)LastWeekStanding);
                //rawDataArray[bC++] = addBytes[0]; rawDataArray[bC++] = addBytes[1];
                //SaveUInt24(ref rawDataArray, (uint)LifetimeHK, bC); bC += 3;
                //SaveUInt24(ref rawDataArray, (uint)LifetimeDK, bC); bC += 3;
                //rawDataArray[bC++] = (byte)LifetimeHighestRank;
                //_Info.AddValue("RawData", rawDataArray);
            }

            //DataVersion 2.0f + 1 här == 11 727KB
            {
                _Info.AddValue("DataVersion", (byte)1);
                _Info.AddValue("CurrentRank", (byte)CurrentRank);
                _Info.AddValue("CurrentRankProgress", CurrentRankProgress);
                _Info.AddValue("TodayHK", (UInt16)TodayHK);
                _Info.AddValue("TodayDK", (UInt16)TodayDK);
                _Info.AddValue("YesterdayHK", (UInt16)YesterdayHK);
                _Info.AddValue("YesterdayHonor", YesterdayHonor);
                _Info.AddValue("ThisWeekHK", (UInt16)ThisWeekHK);
                _Info.AddValue("ThisWeekHonor", ThisWeekHonor);
                _Info.AddValue("LastWeekHK", (UInt16)LastWeekHK);
                _Info.AddValue("LastWeekHonor", LastWeekHonor);
                _Info.AddValue("LastWeekStanding", (UInt16)LastWeekStanding);
                _Info.AddValue("LifetimeHK", LifetimeHK);
                _Info.AddValue("LifetimeDK", LifetimeDK);
                _Info.AddValue("LifetimeHighestRank", (byte)LifetimeHighestRank);
            }

            //DataVersion 1.4 här == 11 732KB
            {
                //_Info.AddValue("CurrentRank", CurrentRank);
                //_Info.AddValue("CurrentRankProgress", CurrentRankProgress);
                //_Info.AddValue("TodayHK", TodayHK);
                //_Info.AddValue("TodayDK", TodayDK);
                //_Info.AddValue("YesterdayHK", YesterdayHK);
                //_Info.AddValue("YesterdayHonor", YesterdayHonor);
                //_Info.AddValue("ThisWeekHK", ThisWeekHK);
                //_Info.AddValue("ThisWeekHonor", ThisWeekHonor);
                //_Info.AddValue("LastWeekHK", LastWeekHK);
                //_Info.AddValue("LastWeekHonor", LastWeekHonor);
                //_Info.AddValue("LastWeekStanding", LastWeekStanding);
                //_Info.AddValue("LifetimeHK", LifetimeHK);
                //_Info.AddValue("LifetimeDK", LifetimeDK);
                //_Info.AddValue("LifetimeHighestRank", LifetimeHighestRank);
            }
        }
        #endregion
    }
}
