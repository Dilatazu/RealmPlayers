using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace VF_RealmPlayersDatabase
{
    [ProtoContract]
    [Serializable]
    public struct UploadID : ISerializable
    {
        [ProtoMember(1)]
        int ContributorID;
        [ProtoMember(2)]
        DateTime Time;

        //public UploadID()
        //{ 
        //    ContributorID = -1;
        //    Time = DateTime.Now;
        //}
        public UploadID(int _ContributorID, DateTime _Time)
        {
            ContributorID = _ContributorID;
            Time = _Time;
        }
        public int GetContributorID()
        {
            return ContributorID;
        }
        public DateTime GetTime()
        {
            return Time;
        }
        public bool IsNull()
        {
            return ContributorID == -1;
        }
        public static UploadID Null()
        {
            return new UploadID(-1, DateTime.Now);
        }
        
        #region Serializing
        public UploadID(SerializationInfo _Info, StreamingContext _Context)
        {
            ContributorID = _Info.GetInt32("ContributorID");
            Time = (DateTime)_Info.GetValue("Time", typeof(DateTime));
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("ContributorID", ContributorID);
            _Info.AddValue("Time", Time);
        }
        #endregion
    }
}
