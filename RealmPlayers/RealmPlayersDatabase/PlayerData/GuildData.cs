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
    public class GuildData : ISerializable
    {
        [ProtoMember(1)]
        public string GuildName = "None";
        [ProtoMember(2)]
        public string GuildRank = "None";
        [ProtoMember(3)]
        public int GuildRankNr = 0;

        public GuildData()
        { }
        public GuildData(System.Xml.XmlNode _PlayerNode)
        {
            InitData(XMLUtility.GetChildValue(_PlayerNode, "PlayerData", ""));
        }
        public GuildData(string _PlayerDataString)
        {
            InitData(_PlayerDataString);
        }
        private void InitData(string _PlayerDataString)
        {
            try
            {
                if (_PlayerDataString == "") return;
                string[] playerData = _PlayerDataString.Split(new char[] { ':' });
                GuildName = playerData[2];
                GuildRank = playerData[3];
                GuildRankNr = int.Parse(playerData[4]);
            }
            catch (Exception)
            { }
        }
        public void SetData(GuildData _GuildData)
        {
            GuildName = _GuildData.GuildName;
            GuildRank = _GuildData.GuildRank;
            GuildRankNr = _GuildData.GuildRankNr;
        }
        public bool IsSame(GuildData _GuildData)
        {
            if (GuildName != _GuildData.GuildName) return false;
            if (GuildRank != _GuildData.GuildRank) return false;
            if (GuildRankNr != _GuildData.GuildRankNr) return false;

            return true;
        }
        
        #region Serializing
        public GuildData(SerializationInfo _Info, StreamingContext _Context)
        {
            GuildName = _Info.GetString("GuildName");
            GuildRank = _Info.GetString("GuildRank");
            GuildRankNr = _Info.GetInt32("GuildRankNr");
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("GuildName", GuildName);
            _Info.AddValue("GuildRank", GuildRank);
            _Info.AddValue("GuildRankNr", GuildRankNr);
        }
        #endregion
    }
}
