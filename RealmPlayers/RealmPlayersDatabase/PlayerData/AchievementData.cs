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
    public class AchievementData : ISerializable
    {
        #region Serializing
        public AchievementData(SerializationInfo _Info, StreamingContext _Context)
        {
            
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            
        }
        #endregion
    }
}
