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
    public class Player : ISerializable
    {
        [ProtoMember(1)]
        public string Name = "Unknown";
        [ProtoMember(2)]
        public WowRealm Realm = WowRealm.Unknown;
        [ProtoMember(3)]
        public CharacterData Character = new CharacterData("");
        [ProtoMember(4)]
        public GuildData Guild = new GuildData("");
        [ProtoMember(5)]
        public HonorData Honor = new HonorData("", WowVersionEnum.Vanilla);//Default
        [ProtoMember(6)]
        public GearData Gear = new GearData("", WowVersionEnum.Vanilla);//Default
        [ProtoMember(7)]
        public DateTime LastSeen = DateTime.MinValue;
        [ProtoMember(8)]
        public UploadID Uploader = UploadID.Null();
        [ProtoMember(9)]
        public ArenaData Arena = null;//Default
        [ProtoMember(10)]
        public string TalentPointsData = null;//Default

        public Player()
        {}
        public Player(string _Name, WowRealm _Realm)
        {
            Name = _Name;
            Realm = _Realm;
            LastSeen = DateTime.MinValue;
        }
        public Player(string _Name, WowRealm _Realm, CharacterDataHistoryItem _Character, GuildDataHistoryItem _Guild, HonorDataHistoryItem _Honor, GearDataHistoryItem _Gear, ArenaDataHistoryItem _Arena, TalentsDataHistoryItem _Talents)
        {
            Name = _Name;
            Realm = _Realm;
            Character = _Character.Data;
            Guild = _Guild.Data;
            Honor = _Honor.Data;
            Gear = _Gear.Data;
            Arena = _Arena.Data;
            TalentPointsData = _Talents.Data;
            Uploader = _Character.Uploader;
            if(_Guild.Uploader.GetTime() > Uploader.GetTime())
                Uploader = _Guild.Uploader;
            if(_Honor.Uploader.GetTime() > Uploader.GetTime())
                Uploader = _Honor.Uploader;
            if (_Gear.Uploader.GetTime() > Uploader.GetTime())
                Uploader = _Gear.Uploader;
            if (_Arena.Uploader.GetTime() > Uploader.GetTime())
                Uploader = _Arena.Uploader;
            if (_Talents.Uploader.GetTime() > Uploader.GetTime())
                Uploader = _Talents.Uploader;
            LastSeen = Uploader.GetTime();
        }

        public bool Update(System.Xml.XmlNode _PlayerNode, UploadID _Uploader/*Contains LastSeen*/, DateTime _LastSeen, PlayerHistory _PlayerHistory, WowVersionEnum _WowVersion)
        {
            var newCharacter = new PlayerData.CharacterData(_PlayerNode);
            var newGuild = new PlayerData.GuildData(_PlayerNode);
            var newGear = new PlayerData.GearData(_PlayerNode, _WowVersion);
            var newHonor = new PlayerData.HonorData(_PlayerNode, _WowVersion);
            
            if (newGear.Items.Count == 0 && newGuild.GuildName == "nil" && newGuild.GuildRank == "nil" && newHonor.CurrentRank == 0 && _WowVersion == WowVersionEnum.Vanilla)
                return true;

            PlayerData.ArenaData newArena = null;
            string newTalentPointsData = null;
            if (_WowVersion == WowVersionEnum.TBC)
            {
                newArena = new PlayerData.ArenaData(_PlayerNode);
                _PlayerHistory.AddToHistory(newArena, _Uploader);
                newTalentPointsData = XMLUtility.GetChildValue(_PlayerNode, "TalentsData", "");
                _PlayerHistory.AddTalentsToHistory(newTalentPointsData, _Uploader);
            }
            return Update(_Uploader, _LastSeen, _PlayerHistory, _WowVersion, newCharacter, newGuild, newGear, newHonor, newArena, newTalentPointsData);
        }
        public bool Update(UploadID _Uploader/*Contains LastSeen*/, DateTime _LastSeen, PlayerHistory _PlayerHistory, WowVersionEnum _WowVersion
            , PlayerData.CharacterData _NewCharacter, PlayerData.GuildData _NewGuild, PlayerData.GearData _NewGear, PlayerData.HonorData _NewHonor, PlayerData.ArenaData _NewArena = null, string _NewTalents = null)
        {
            if(_WowVersion == WowVersionEnum.TBC)
            {
                if(_NewArena != null) _PlayerHistory.AddToHistory(_NewArena, _Uploader);
                if(_NewTalents != null) _PlayerHistory.AddTalentsToHistory(_NewTalents, _Uploader);
            }

            _PlayerHistory.AddToHistory(_NewCharacter, _Uploader);
            _PlayerHistory.AddToHistory(_NewGuild, _Uploader);
            if (_NewGear.Items.Count > 0)
                _PlayerHistory.AddToHistory(_NewGear, _Uploader);
            _PlayerHistory.AddToHistory(_NewHonor, _Uploader);
            if (_LastSeen > LastSeen)
            {
                Uploader = _Uploader;
                LastSeen = _LastSeen;
                Character = _NewCharacter;
                Guild = _NewGuild;
                if (_NewGear.Items.Count > 0)
                    Gear = _NewGear;
                Honor = _NewHonor;
                Arena = _NewArena;
                TalentPointsData = _NewTalents;
                return false;
            }

            return true;
        }

        public string GetStandingStr()
        {
            if (Honor.LastWeekStanding == int.MaxValue || Honor.LastWeekStanding == UInt16.MaxValue)
                return "---";
            else
                return Honor.LastWeekStanding.ToString();
        }
        public float GetRankTotal()
        {
            return Honor.GetRankTotal();
        }
        public bool ReceivedStandingLastWeek()
        {
            return Honor.LastWeekStanding != int.MaxValue && Honor.LastWeekStanding != UInt16.MaxValue;
        }
        public string GetUploaderName()
        {
            Contributor contributor = ContributorDB.GetContributor(Uploader);
            if (contributor != null)
                return ContributorDB.GetContributor(Uploader).Name;

            return "null";
        }

        #region Serializing
        public Player(SerializationInfo _Info, StreamingContext _Context)
        {
            Name = _Info.GetString("Name");
            Realm = (WowRealm)_Info.GetInt32("Realm");
            Character = (CharacterData)_Info.GetValue("Character", typeof(CharacterData));
            Guild = (GuildData)_Info.GetValue("Guild", typeof(GuildData));
            Honor = (HonorData)_Info.GetValue("Honor", typeof(HonorData));
            Gear = (GearData)_Info.GetValue("Gear", typeof(GearData));
            LastSeen = (DateTime)_Info.GetValue("LastSeen", typeof(DateTime));
            Uploader = (UploadID)_Info.GetValue("Uploader", typeof(UploadID));
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Name", Name);
            _Info.AddValue("Realm", (int)Realm);
            _Info.AddValue("Character", Character);
            _Info.AddValue("Guild", Guild);
            _Info.AddValue("Honor", Honor);
            _Info.AddValue("Gear", Gear);
            _Info.AddValue("LastSeen", LastSeen);
            _Info.AddValue("Uploader", Uploader);
        }
        #endregion

        public float GetArenaRatingTotal()
        {
            if (Arena == null || (Arena.Team2v2 == null && Arena.Team3v3 == null && Arena.Team5v5 == null))
                return -30000;

            int totalRating = 0;
            if (Arena.Team2v2 != null)
                totalRating += Arena.Team2v2.PlayerRating;
            else
                totalRating += 1500;
            if (Arena.Team3v3 != null)
                totalRating += Arena.Team3v3.PlayerRating;
            else
                totalRating += 1500;
            if (Arena.Team5v5 != null)
                totalRating += Arena.Team5v5.PlayerRating;
            else
                totalRating += 1500;

            return totalRating;
        }
    }
}
