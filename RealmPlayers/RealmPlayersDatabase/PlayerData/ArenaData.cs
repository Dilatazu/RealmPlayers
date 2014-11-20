using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace VF_RealmPlayersDatabase.PlayerData
{
    [ProtoContract]
    public class ArenaPlayerData
    {
        [ProtoMember(1)]
        public string TeamName = "None";
        [ProtoMember(2)]
        public int TeamRating = 0;
        [ProtoMember(3)]
        public int GamesPlayed = 0;
        [ProtoMember(4)]
        public int GamesWon = 0;
        [ProtoMember(5)]
        public int PlayerPlayed = 0;
        [ProtoMember(6)]
        public int PlayerRating = 0;

        public bool IsSame(ArenaPlayerData _ArenaPlayerData)
        {
            if (TeamName != _ArenaPlayerData.TeamName) return false;
            if (TeamRating != _ArenaPlayerData.TeamRating) return false;
            if (GamesPlayed != _ArenaPlayerData.GamesPlayed) return false;
            if (GamesWon != _ArenaPlayerData.GamesWon) return false;
            if (PlayerPlayed != _ArenaPlayerData.PlayerPlayed) return false;
            if (PlayerRating != _ArenaPlayerData.PlayerRating) return false;

            return true;
        }
    }
    [ProtoContract]
    public class ArenaData
    {
        [ProtoMember(1)]
        public ArenaPlayerData Team2v2 = null;
        [ProtoMember(2)]
        public ArenaPlayerData Team3v3 = null;
        [ProtoMember(3)]
        public ArenaPlayerData Team5v5 = null;

        public ArenaData()
        { }
        public ArenaData(System.Xml.XmlNode _PlayerNode)
        {
            InitData(XMLUtility.GetChildValue(_PlayerNode, "ArenaData", ""));
        }
        public ArenaData(string _ArenaDataString)
        {
            InitData(_ArenaDataString);
        }
        private void InitData(string _ArenaDataString)
        {
            try
            {
                if (_ArenaDataString == "") return;
                string[] arenaDatas = _ArenaDataString.Split(new char[] { ',' });
                foreach (string arenaData in arenaDatas)
                {
                    string[] arenaPlayerData = arenaData.Split(new char[] { ':' });
                    int teamSize = int.Parse(arenaPlayerData[1]);
                    ArenaPlayerData newData = new ArenaPlayerData();
                    newData.TeamName = arenaPlayerData[0];
                    newData.TeamRating = int.Parse(arenaPlayerData[2]);
                    newData.GamesPlayed = int.Parse(arenaPlayerData[3]);
                    newData.GamesWon = int.Parse(arenaPlayerData[4]);
                    newData.PlayerPlayed = int.Parse(arenaPlayerData[5]);
                    newData.PlayerRating = int.Parse(arenaPlayerData[6]);
                    if (teamSize == 2)
                        Team2v2 = newData;
                    else if (teamSize == 3)
                        Team3v3 = newData;
                    else if (teamSize == 5)
                        Team5v5 = newData;
                    else
                        throw new Exception("Unknown Arena Team Size: \"" + teamSize + "\"");
                }
            }
            catch (Exception)
            { }
        }
        public bool IsSame(ArenaData _ArenaData)
        {
            if ((Team2v2 == null && _ArenaData.Team2v2 != null) || (Team2v2 != null && _ArenaData.Team2v2 == null))
                return false;
            if ((Team3v3 == null && _ArenaData.Team3v3 != null) || (Team3v3 != null && _ArenaData.Team3v3 == null))
                return false;
            if ((Team5v5 == null && _ArenaData.Team5v5 != null) || (Team5v5 != null && _ArenaData.Team5v5 == null))
                return false;

            if (Team2v2 != null && Team2v2.IsSame(_ArenaData.Team2v2) == false)
                return false;
            if (Team3v3 != null && Team3v3.IsSame(_ArenaData.Team3v3) == false)
                return false;
            if (Team5v5 != null && Team5v5.IsSame(_ArenaData.Team5v5) == false)
                return false;

            return true;
        }
    }
}
