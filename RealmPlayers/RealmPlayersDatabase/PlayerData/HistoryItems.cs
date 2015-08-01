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
    [Serializable]
    public struct CharacterDataHistoryItem : ISerializable
    {
        [ProtoMember(1)]
        public CharacterData Data;
        [ProtoMember(2)]
        public UploadID Uploader;

        //public CharacterDataHistoryItem()
        //{ }
        public CharacterDataHistoryItem(CharacterData _Character, UploadID _Uploader)
        {
            Data = _Character;
            Uploader = _Uploader;
        }

        #region Serializing
        public CharacterDataHistoryItem(SerializationInfo _Info, StreamingContext _Context)
        {
            Data = (CharacterData)_Info.GetValue("Data", typeof(CharacterData));
            Uploader = (UploadID)_Info.GetValue("Uploader", typeof(UploadID));
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Data", Data);
            _Info.AddValue("Uploader", Uploader);
        }
        #endregion

        public static bool IsSame(CharacterDataHistoryItem _Item1, CharacterDataHistoryItem _Item2)
        {
            return _Item1.Data.IsSame(_Item2.Data);
        }
        public static bool Time1BiggerThan2(CharacterDataHistoryItem _Item1, CharacterDataHistoryItem _Item2)
        {
            return _Item1.Uploader.GetTime() > _Item2.Uploader.GetTime(); 
        }
        public static bool CopyUploader2To1(CharacterDataHistoryItem _Data1, CharacterDataHistoryItem _Data2) 
        { 
            _Data1.Uploader = _Data2.Uploader; 
            return true;
        }
    }
    [ProtoContract]
    [Serializable]
    public struct GuildDataHistoryItem : ISerializable
    {
        [ProtoMember(1)]
        public GuildData Data;
        [ProtoMember(2)]
        public UploadID Uploader;

        //public GuildDataHistoryItem()
        //{ }
        public GuildDataHistoryItem(GuildData _Guild, UploadID _Uploader)
        {
            Data = _Guild;
            Uploader = _Uploader;
        }

        #region Serializing
        public GuildDataHistoryItem(SerializationInfo _Info, StreamingContext _Context)
        {
            Data = (GuildData)_Info.GetValue("Data", typeof(GuildData));
            Uploader = (UploadID)_Info.GetValue("Uploader", typeof(UploadID));
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Data", Data);
            _Info.AddValue("Uploader", Uploader);
        }
        #endregion

        public static bool IsSame(GuildDataHistoryItem _Item1, GuildDataHistoryItem _Item2)
        {
            return _Item1.Data.IsSame(_Item2.Data);
        }
        public static bool Time1BiggerThan2(GuildDataHistoryItem _Item1, GuildDataHistoryItem _Item2)
        {
            return _Item1.Uploader.GetTime() > _Item2.Uploader.GetTime();
        }
        public static bool CopyUploader2To1(GuildDataHistoryItem _Data1, GuildDataHistoryItem _Data2)
        {
            _Data1.Uploader = _Data2.Uploader;
            return true;
        }
    }
    [ProtoContract]
    [Serializable]
    public struct HonorDataHistoryItem : ISerializable
    {
        [ProtoMember(1)]
        public HonorData Data;
        [ProtoMember(2)]
        public UploadID Uploader;

        //public HonorDataHistoryItem()
        //{ }
        public HonorDataHistoryItem(HonorData _Honor, UploadID _Uploader)
        {
            Data = _Honor;
            Uploader = _Uploader;
        }

        #region Serializing
        public HonorDataHistoryItem(SerializationInfo _Info, StreamingContext _Context)
        {
            Data = (HonorData)_Info.GetValue("Data", typeof(HonorData));
            Uploader = (UploadID)_Info.GetValue("Uploader", typeof(UploadID));
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Data", Data);
            _Info.AddValue("Uploader", Uploader);
        }
        #endregion

        public static bool IsSame(HonorDataHistoryItem _Item1, HonorDataHistoryItem _Item2)
        {
            return _Item1.Data.IsSame(_Item2.Data);
        }
        public static bool Time1BiggerThan2(HonorDataHistoryItem _Item1, HonorDataHistoryItem _Item2)
        {
            return _Item1.Uploader.GetTime() > _Item2.Uploader.GetTime();
        }
        public static bool CopyUploader2To1(HonorDataHistoryItem _Data1, HonorDataHistoryItem _Data2)
        {
            _Data1.Uploader = _Data2.Uploader;
            return true;
        }
    }
    [ProtoContract]
    [Serializable]
    public struct GearDataHistoryItem : ISerializable
    {
        [ProtoMember(1)]
        public GearData Data;
        [ProtoMember(2)]
        public UploadID Uploader;

        //public GearDataHistoryItem()
        //{ }
        public GearDataHistoryItem(GearData _Gear, UploadID _Uploader)
        {
            Data = _Gear;
            Uploader = _Uploader;
        }

        #region Serializing
        public GearDataHistoryItem(SerializationInfo _Info, StreamingContext _Context)
        {
            Data = (GearData)_Info.GetValue("Data", typeof(GearData));
            Uploader = (UploadID)_Info.GetValue("Uploader", typeof(UploadID));
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Data", Data);
            _Info.AddValue("Uploader", Uploader);
        }
        #endregion

        public static bool IsSame(GearDataHistoryItem _Item1, GearDataHistoryItem _Item2)
        {
            return _Item1.Data.IsSame(_Item2.Data);
        }
        public static bool Time1BiggerThan2(GearDataHistoryItem _Item1, GearDataHistoryItem _Item2)
        {
            return _Item1.Uploader.GetTime() > _Item2.Uploader.GetTime();
        }
        public static bool CopyUploader2To1(GearDataHistoryItem _Data1, GearDataHistoryItem _Data2)
        {
            _Data1.Uploader = _Data2.Uploader;
            return true;
        }
    }

    [ProtoContract]
    public struct ArenaDataHistoryItem
    {
        [ProtoMember(1)]
        public ArenaData Data;
        [ProtoMember(2)]
        public UploadID Uploader;

        public ArenaDataHistoryItem(ArenaData _Arena, UploadID _Uploader)
        {
            Data = _Arena;
            Uploader = _Uploader;
        }

        public static bool IsSame(ArenaDataHistoryItem _Item1, ArenaDataHistoryItem _Item2)
        {
            return _Item1.Data.IsSame(_Item2.Data);
        }
        public static bool Time1BiggerThan2(ArenaDataHistoryItem _Item1, ArenaDataHistoryItem _Item2)
        {
            return _Item1.Uploader.GetTime() > _Item2.Uploader.GetTime();
        }
        public static bool CopyUploader2To1(ArenaDataHistoryItem _Data1, ArenaDataHistoryItem _Data2)
        {
            _Data1.Uploader = _Data2.Uploader;
            return true;
        }
    }

    [ProtoContract]
    public struct TalentsDataHistoryItem
    {
        [ProtoMember(1)]
        public string Data;
        [ProtoMember(2)]
        public UploadID Uploader;

        public TalentsDataHistoryItem(string _Talents, UploadID _Uploader)
        {
            Data = _Talents;
            Uploader = _Uploader;
        }

        public static bool IsSame(TalentsDataHistoryItem _Item1, TalentsDataHistoryItem _Item2)
        {
            return _Item1.Data == _Item2.Data;
        }
        public static bool Time1BiggerThan2(TalentsDataHistoryItem _Item1, TalentsDataHistoryItem _Item2)
        {
            return _Item1.Uploader.GetTime() > _Item2.Uploader.GetTime();
        }
        public static bool CopyUploader2To1(TalentsDataHistoryItem _Data1, TalentsDataHistoryItem _Data2)
        {
            _Data1.Uploader = _Data2.Uploader;
            return true;
        }
    }
}
