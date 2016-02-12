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
    public class CharacterData : ISerializable
    {
        [ProtoMember(1)]
        public PlayerRace Race = PlayerRace.Unknown;
        [ProtoMember(2)]
        public PlayerClass Class = PlayerClass.Unknown;
        [ProtoMember(3)]
        public PlayerSex Sex = PlayerSex.Male;
        [ProtoMember(4)]
        public int Level = 0;

        public CharacterData()
        { }
        public CharacterData(System.Xml.XmlNode _PlayerNode)
        {
            InitData(XMLUtility.GetChildValue(_PlayerNode, "PlayerData", ""));
        }
        public CharacterData(string _PlayerDataString)
        {
            InitData(_PlayerDataString);
        }
        private void InitData(string _PlayerDataString)
        {
            try
            {
                if (_PlayerDataString == "") return;
                string[] playerData = _PlayerDataString.Split(new char[] { ':' });
                Race = StaticValues.ConvertRace(playerData[0]);
                if (StaticValues.ConvertClass(playerData[1]) != PlayerClass.Unknown)
                    Class = StaticValues.ConvertClass(playerData[1]);
                Sex = StaticValues.ConvertSex(playerData[5]);
                Level = int.Parse(playerData[6]);
            }
            catch (Exception)
            { }
        }

        public bool IsSame(CharacterData _CharacterData)
        {
            if (Race != _CharacterData.Race) return false;
            if (Class != _CharacterData.Class) return false;
            if (Sex != _CharacterData.Sex) return false;
            if (Level != _CharacterData.Level) return false;

            return true;
        }
        
        public string GetAsString()
        {
            return "{" + Race.ToString() + ", " + Class.ToString() + ", " + Sex.ToString() + ", " + Level + "}";
        }

        #region Serializing
        public CharacterData(SerializationInfo _Info, StreamingContext _Context)
        {
            Race = (PlayerRace)_Info.GetInt32("Race");
            Class = (PlayerClass)_Info.GetInt32("Class");
            Sex = (PlayerSex)_Info.GetInt32("Sex");
            Level = _Info.GetInt32("Level");
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Race", (int)Race);
            _Info.AddValue("Class", (int)Class);
            _Info.AddValue("Sex", (int)Sex);
            _Info.AddValue("Level", Level);
        }
        #endregion
    }
}
