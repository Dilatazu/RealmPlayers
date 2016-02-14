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

        public bool Update(System.Xml.XmlNode _PlayerNode, UploadID _Uploader/*Contains LastSeen*/, DateTime _LastSeen, PlayerHistory _PlayerHistory, WowVersionEnum _WowVersion, Func<int, VF.SQLUploadID> _GetSQLUploadIDFunc = null)
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
                newTalentPointsData = XMLUtility.GetChildValue(_PlayerNode, "TalentsData", "");
            }
            if(_GetSQLUploadIDFunc != null)
            {
                UpdateSQL(_GetSQLUploadIDFunc, Uploader, _LastSeen, _PlayerHistory, _WowVersion, newCharacter, newGuild, newGear, newHonor, newArena, newTalentPointsData);
            }
            return Update(_Uploader, _LastSeen, _PlayerHistory, _WowVersion, newCharacter, newGuild, newGear, newHonor, newArena, newTalentPointsData);
        }
        public void UpdateSQL(Func<int, VF.SQLUploadID> _GetSQLUploadIDFunc, UploadID _Uploader/*Contains LastSeen*/, DateTime _LastSeen, PlayerHistory _PlayerHistory, WowVersionEnum _WowVersion
            , PlayerData.CharacterData _NewCharacter, PlayerData.GuildData _NewGuild, PlayerData.GearData _NewGear, PlayerData.HonorData _NewHonor, PlayerData.ArenaData _NewArena = null, string _NewTalents = null)
        {
            using (VF.SQLComm comm = new VF.SQLComm())
            {
                VF.SQLPlayerData playerData = VF.SQLPlayerData.Invalid();
                bool validPlayerData = false;

                VF.SQLPlayerID playerID;
                bool validPlayerID = false;
                if (comm.GetPlayerID(Realm, Name, out playerID) == true)
                {
                    validPlayerID = true;
                    if (_LastSeen < LastSeen)
                    {
                        if (comm.GetPlayerDataAtTime(playerID, _LastSeen, out playerData) == true)
                        {
                            validPlayerData = true;
                        }
                    }

                    if (validPlayerData == false)
                    {
                        if (comm.GetLatestPlayerData(playerID, out playerData) == true)
                        {
                            validPlayerData = true;
                        }
                    }
                }

                if (validPlayerData == true)
                {
                    bool playerDataChanged = false;
                    if (playerData.PlayerCharacter.IsSame(_NewCharacter) == false)
                    {
                        playerData.PlayerCharacter = _NewCharacter;
                        playerDataChanged = true;
                    }

                    PlayerData.GuildData playerDataGuildData;
                    if (comm.GetPlayerGuildData(playerData, out playerDataGuildData) == true)
                    {
                        if (playerDataGuildData.IsSame(_NewGuild) == false)
                        {
                            playerData.PlayerGuildID = comm.GenerateNewPlayerGuildDataEntry(_NewGuild);
                            playerDataChanged = true;
                        }
                    }
                    else if (playerData.PlayerGuildID == 0)
                    {
                        playerData.PlayerGuildID = comm.GenerateNewPlayerGuildDataEntry(_NewGuild);
                        if (playerData.PlayerGuildID != 0)
                        {
                            playerDataChanged = true;
                        }
                    }

                    PlayerData.GearData playerDataGearData;
                    if (comm.GetPlayerGearData(playerData, out playerDataGearData) == true)
                    {
                        if (playerDataGearData.IsSame(_NewGear) == false)
                        {
                            playerData.PlayerGearID = comm.GenerateNewPlayerGearDataEntry(_NewGear, _WowVersion);
                            playerDataChanged = true;
                        }
                    }
                    else if (playerData.PlayerGearID == 0)
                    {
                        playerData.PlayerGearID = comm.GenerateNewPlayerGearDataEntry(_NewGear, _WowVersion);
                        if (playerData.PlayerGearID != 0)
                        {
                            playerDataChanged = true;
                        }
                    }

                    PlayerData.HonorData playerDataHonorData;
                    if (comm.GetPlayerHonorData(playerData, out playerDataHonorData) == true)
                    {
                        if (playerDataHonorData.IsSame(_NewHonor, _WowVersion) == false)
                        {
                            playerData.PlayerHonorID = comm.GenerateNewPlayerHonorDataEntry(_NewHonor, _WowVersion);
                            playerDataChanged = true;
                        }
                    }
                    else if (playerData.PlayerHonorID == 0)
                    {
                        playerData.PlayerHonorID = comm.GenerateNewPlayerHonorDataEntry(_NewHonor, _WowVersion);
                        if (playerData.PlayerHonorID != 0)
                        {
                            playerDataChanged = true;
                        }
                    }

                    if (_WowVersion == WowVersionEnum.TBC)
                    {
                        PlayerData.ArenaData playerDataArenaData;
                        if (comm.GetPlayerArenaData(playerData, out playerDataArenaData) == true)
                        {
                            if (playerDataArenaData.IsSame(_NewArena) == false)
                            {
                                playerData.PlayerArenaID = comm.GenerateNewPlayerArenaInfoEntry(_NewArena);
                                playerDataChanged = true;
                            }
                        }
                        else if (playerData.PlayerArenaID == 0)
                        {
                            playerData.PlayerArenaID = comm.GenerateNewPlayerArenaInfoEntry(_NewArena);
                            if (playerData.PlayerArenaID != 0)
                            {
                                playerDataChanged = true;
                            }
                        }

                        string playerDataTalentsData;
                        if (comm.GetPlayerTalentsData(playerData, out playerDataTalentsData) == true)
                        {
                            if (playerDataTalentsData == _NewTalents)
                            {
                                playerData.PlayerTalentsID = comm.GenerateNewPlayerTalentsDataEntry(_NewTalents);
                                playerDataChanged = true;
                            }
                        }
                        else if (playerData.PlayerTalentsID == 0)
                        {
                            playerData.PlayerTalentsID = comm.GenerateNewPlayerTalentsDataEntry(_NewTalents);
                            if (playerData.PlayerTalentsID != 0)
                            {
                                playerDataChanged = true;
                            }
                        }
                    }

                    if (playerDataChanged == true)
                    {
                        playerData.UpdateTime = _LastSeen;
                        playerData.UploadID = _GetSQLUploadIDFunc(0);

                        if (comm.GenerateNewPlayerDataEntry(playerData) == false)
                        {
                            Logger.ConsoleWriteLine("Failed to update PlayerData for player \"" + Name + "\"", ConsoleColor.Red);
                        }

                        if (_LastSeen > LastSeen)
                        {
                            //Also update the latestupdateid in playertable since this is the latest available data!
                            if (comm.UpdatePlayerEntry(playerData.PlayerID, playerData.UploadID) == false)
                            {
                                Logger.ConsoleWriteLine("Failed to update LatestUploadID for player \"" + Name + "\"", ConsoleColor.Red);
                            }
                        }
                    }
                }
                else
                {
                    Player newPlayer = new Player();
                    newPlayer.Name = Name;
                    newPlayer.Realm = Realm;
                    newPlayer.Character = _NewCharacter;
                    newPlayer.Guild = _NewGuild;
                    newPlayer.Gear = _NewGear;
                    newPlayer.Honor = _NewHonor;
                    newPlayer.Arena = _NewArena;
                    newPlayer.TalentPointsData = _NewTalents;
                    newPlayer.LastSeen = _LastSeen;

                    //Assume PlayerID did not exist!!!
                    if (validPlayerID == true)
                    {
                        //OK PlayerID exists, so maybe this is just an earlier data than currently exists at all for player.
                        //This case should be handled above though, so if this happens something is very weird and unexpected!
                        //This issue can be resolved automatically, it should never happen normally. But if it does...
                        Logger.ConsoleWriteLine("Unexpected problem when updating PlayerData for player \"" + Name + "\"", ConsoleColor.Red);
                        if (comm.UpdateLatestPlayerDataEntry(playerID, _GetSQLUploadIDFunc(0), newPlayer).ID == playerID.ID)
                        {
                            Logger.ConsoleWriteLine("Unexpected problem was resolved successfully!", ConsoleColor.Green);
                        }
                        else
                        {
                            Logger.ConsoleWriteLine("\n--------------------------------------------\nERROR ERROR ERRO ERROR ERROR\nWhen attempting to fix unexpected problem for player \"" + Name + "\" it did not work!!!\n--------------------------------------------\n", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        if (comm.GenerateNewPlayerEntry(_GetSQLUploadIDFunc(0), newPlayer).IsValid() == false)
                        {
                            Logger.ConsoleWriteLine("Unexpected problem when creating new SQLPlayerID for player \"" + Name + "\"", ConsoleColor.Red);
                        }
                    }
                }
            }
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
