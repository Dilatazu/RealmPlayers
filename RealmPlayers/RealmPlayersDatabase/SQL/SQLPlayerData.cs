using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

using Contributor = VF_RealmPlayersDatabase.Contributor;
using PlayerData = VF_RealmPlayersDatabase.PlayerData;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;
using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerSex = VF_RealmPlayersDatabase.PlayerSex;
using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;
using StaticValues = VF_RealmPlayersDatabase.StaticValues;

using UploadID = VF_RealmPlayersDatabase.UploadID;

using Logger = VF_RealmPlayersDatabase.Logger;

namespace VF
{
    public struct SQLPlayerData
    {
        public SQLPlayerID PlayerID;
        public SQLUploadID UploadID;
        public DateTime UpdateTime;
        public PlayerData.CharacterData PlayerCharacter;
        public int PlayerGuildID;
        public int PlayerHonorID;
        public int PlayerGearID;
        public int PlayerArenaID;
        public int PlayerTalentsID;
        public static SQLPlayerData Invalid()
        {
            var value = new SQLPlayerData();
            value.PlayerID = SQLPlayerID.Invalid();
            return value;
        }
    }
    public partial class SQLComm
    {
        public bool GetLatestPlayerData(string _PlayerName, WowRealm _Realm, out SQLPlayerData _ResultPlayerData)
        {
            SQLPlayerID playerID;
            if (GetPlayerID(_Realm, _PlayerName, out playerID) == true)
            {
                return GetLatestPlayerData(playerID, out _ResultPlayerData);
            }
            _ResultPlayerData = SQLPlayerData.Invalid();
            return false;
        }
        public bool GetLatestPlayerData(SQLPlayerID _PlayerID, out SQLPlayerData _ResultPlayerData)
        {
            var conn = OpenConnection();
            try
            {
                const int UPLOADID_COLUMN = 0;
                const int UPDATETIME_COLUMN = 1;
                const int RACE_COLUMN = 2;
                const int CLASS_COLUMN = 3;
                const int SEX_COLUMN = 4;
                const int LEVEL_COLUMN = 5;
                const int GUILDINFO_COLUMN = 6;
                const int HONORINFO_COLUMN = 7;
                const int GEARINFO_COLUMN = 8;
                const int ARENAINFO_COLUMN = 9;
                const int TALENTSINFO_COLUMN = 10;
                using (var cmd = new NpgsqlCommand("SELECT pd.uploadid, pd.updatetime, pd.race, pd.class, pd.sex, pd.level, pd.guildinfo, pd.honorinfo, pd.gearinfo, pd.arenainfo, pd.talentsinfo FROM playertable player" +
                    " INNER JOIN playerdatatable pd ON player.id = pd.playerid AND player.latestuploadid = pd.uploadid" +
                    " WHERE player.id=:ID", conn))
                {
                    {
                        var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                        idParam.Value = (int)_PlayerID;
                        cmd.Parameters.Add(idParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultPlayerData = new SQLPlayerData();
                            _ResultPlayerData.PlayerID = _PlayerID;
                            _ResultPlayerData.UploadID = new SQLUploadID(reader.GetInt32(UPLOADID_COLUMN));
                            _ResultPlayerData.UpdateTime = reader.GetTimeStamp(UPDATETIME_COLUMN).DateTime;
                            _ResultPlayerData.PlayerCharacter = new PlayerData.CharacterData();
                            _ResultPlayerData.PlayerCharacter.Race = (PlayerRace)reader.GetInt16(RACE_COLUMN);
                            _ResultPlayerData.PlayerCharacter.Class = (PlayerClass)reader.GetInt16(CLASS_COLUMN);
                            _ResultPlayerData.PlayerCharacter.Sex = (PlayerSex)reader.GetInt16(SEX_COLUMN);
                            _ResultPlayerData.PlayerCharacter.Level = reader.GetInt16(LEVEL_COLUMN);
                            _ResultPlayerData.PlayerGuildID = reader.GetInt32(GUILDINFO_COLUMN);
                            _ResultPlayerData.PlayerHonorID = reader.GetInt32(HONORINFO_COLUMN);
                            _ResultPlayerData.PlayerGearID = reader.GetInt32(GEARINFO_COLUMN);
                            _ResultPlayerData.PlayerArenaID = reader.GetInt32(ARENAINFO_COLUMN);
                            _ResultPlayerData.PlayerTalentsID = reader.GetInt32(TALENTSINFO_COLUMN);
                            return true;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            _ResultPlayerData = SQLPlayerData.Invalid();
            return false;
        }
        public bool GetPlayerDataAtTime(string _PlayerName, WowRealm _Realm, DateTime _DateTime, out SQLPlayerData _ResultPlayerData)
        {
            SQLPlayerID playerID;
            if(GetPlayerID(_Realm, _PlayerName, out playerID) == true)
            {
                return GetPlayerDataAtTime(playerID, _DateTime, out _ResultPlayerData);
            }
            _ResultPlayerData = SQLPlayerData.Invalid();
            return false;
        }
        public bool GetPlayerDataAtTime(SQLPlayerID _PlayerID, DateTime _DateTime, out SQLPlayerData _ResultPlayerData)
        {
            var conn = OpenConnection();
            try
            {
                const int UPLOADID_COLUMN = 0;
                const int UPDATETIME_COLUMN = 1;
                const int RACE_COLUMN = 2;
                const int CLASS_COLUMN = 3;
                const int SEX_COLUMN = 4;
                const int LEVEL_COLUMN = 5;
                const int GUILDINFO_COLUMN = 6;
                const int HONORINFO_COLUMN = 7;
                const int GEARINFO_COLUMN = 8;
                const int ARENAINFO_COLUMN = 9;
                const int TALENTSINFO_COLUMN = 10;
                using (var cmd = new NpgsqlCommand("SELECT pd.uploadid, pd.updatetime, pd.race, pd.class, pd.sex, pd.level, pd.guildinfo, pd.honorinfo, pd.gearinfo, pd.arenainfo, pd.talentsinfo FROM playerdatatable pd" +
                    " WHERE pd.playerid = :ID AND pd.updatetime = (SELECT MAX(updatetime) FROM playerdatatable WHERE playerid = :ID AND updatetime < :DateTime)", conn))
                {
                    {
                        var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                        idParam.Value = (int)_PlayerID;
                        cmd.Parameters.Add(idParam);
                    }
                    {
                        var dateParam = new NpgsqlParameter("DateTime", NpgsqlDbType.Timestamp);
                        dateParam.Value = _DateTime;
                        cmd.Parameters.Add(dateParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultPlayerData = new SQLPlayerData();
                            _ResultPlayerData.PlayerID = _PlayerID;
                            _ResultPlayerData.UploadID = new SQLUploadID(reader.GetInt32(UPLOADID_COLUMN));
                            _ResultPlayerData.UpdateTime = reader.GetTimeStamp(UPDATETIME_COLUMN).DateTime;
                            _ResultPlayerData.PlayerCharacter = new PlayerData.CharacterData();
                            _ResultPlayerData.PlayerCharacter.Race = (PlayerRace)reader.GetInt16(RACE_COLUMN);
                            _ResultPlayerData.PlayerCharacter.Class = (PlayerClass)reader.GetInt16(CLASS_COLUMN);
                            _ResultPlayerData.PlayerCharacter.Sex = (PlayerSex)reader.GetInt16(SEX_COLUMN);
                            _ResultPlayerData.PlayerCharacter.Level = reader.GetInt16(LEVEL_COLUMN);
                            _ResultPlayerData.PlayerGuildID = reader.GetInt32(GUILDINFO_COLUMN);
                            _ResultPlayerData.PlayerHonorID = reader.GetInt32(HONORINFO_COLUMN);
                            _ResultPlayerData.PlayerGearID = reader.GetInt32(GEARINFO_COLUMN);
                            _ResultPlayerData.PlayerArenaID = reader.GetInt32(ARENAINFO_COLUMN);
                            _ResultPlayerData.PlayerTalentsID = reader.GetInt32(TALENTSINFO_COLUMN);
                            return true;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            _ResultPlayerData = SQLPlayerData.Invalid();
            return false;
        }
        public bool GetPlayerHonorData(SQLPlayerData _PlayerData, out PlayerData.HonorData _ResultHonorData)
        {
            var conn = OpenConnection();
            try
            {
                const int TODAYHK_COLUMN = 0;
                const int TODAYHONOR_COLUMN = 1;
                const int YESTERDAYHK_COLUMN = 2;
                const int YESTERDAYHONOR_COLUMN = 3;
                const int LIFETIMEHK_COLUMN = 4;
                const int CURRENTRANK_COLUMN = 5;
                const int CURRENTRANKPROGRESS_COLUMN = 6;
                const int TODAYDK_COLUMN = 7;
                const int THISWEEKHK_COLUMN = 8;
                const int THISWEEKHONOR_COLUMN = 9;
                const int LASTWEEKHK_COLUMN = 10;
                const int LASTWEEKHONOR_COLUMN = 11;
                const int LASTWEEKSTANDING_COLUMN = 12;
                const int LIFETIMEDK_COLUMN = 13;
                const int LIFETIMEHIGHESTRANK_COLUMN = 14;
                using (var cmd = new NpgsqlCommand("SELECT h1.todayhk, h1.todayhonor, h1.yesterdayhk, h1.yesterdayhonor, h1.lifetimehk, h2.currentrank, h2.currentrankprogress, h2.todaydk, h2.thisweekhk, h2.thisweekhonor, h2.lastweekhk, h2.lastweekhonor, h2.lastweekstanding, h2.lifetimedk, h2.lifetimehighestrank FROM playerhonortable h1" +
                    " INNER JOIN PlayerHonorVanillaTable h2 ON h1.id = h2.playerhonorid" +
                    " WHERE h1.id=:ID", conn))
                {
                    {
                        var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                        idParam.Value = (int)_PlayerData.PlayerHonorID;
                        cmd.Parameters.Add(idParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultHonorData = new PlayerData.HonorData();
                            /////////////PlayerHonorTable
                            _ResultHonorData.TodayHK = reader.GetInt32(TODAYHK_COLUMN);
                            _ResultHonorData.TodayHonorTBC = reader.GetInt32(TODAYHONOR_COLUMN);
                            _ResultHonorData.YesterdayHK = reader.GetInt32(YESTERDAYHK_COLUMN);
                            _ResultHonorData.YesterdayHonor = reader.GetInt32(YESTERDAYHONOR_COLUMN);
                            _ResultHonorData.LifetimeHK = reader.GetInt32(LIFETIMEHK_COLUMN);
                            /////////////PlayerHonorTable

                            /////////////PlayerHonorVanillaTable
                            if (reader.IsDBNull(CURRENTRANK_COLUMN) == false)
                            {
                                //Player is vanilla, we read all the vanilla values aswell!
                                _ResultHonorData.CurrentRank = reader.GetInt16(CURRENTRANK_COLUMN);
                                _ResultHonorData.CurrentRankProgress = reader.GetFloat(CURRENTRANKPROGRESS_COLUMN);
                                _ResultHonorData.TodayDK = reader.GetInt32(TODAYDK_COLUMN);
                                _ResultHonorData.ThisWeekHK = reader.GetInt32(THISWEEKHK_COLUMN);
                                _ResultHonorData.ThisWeekHonor = reader.GetInt32(THISWEEKHONOR_COLUMN);
                                _ResultHonorData.LastWeekHK = reader.GetInt32(LASTWEEKHK_COLUMN);
                                _ResultHonorData.LastWeekHonor = reader.GetInt32(LASTWEEKHONOR_COLUMN);
                                _ResultHonorData.LastWeekStanding = reader.GetInt32(LASTWEEKSTANDING_COLUMN);
                                _ResultHonorData.LifetimeDK = reader.GetInt32(LIFETIMEDK_COLUMN);
                                _ResultHonorData.LifetimeHighestRank = reader.GetInt16(LIFETIMEHIGHESTRANK_COLUMN);
                            }
                            /////////////PlayerHonorVanillaTable
                            return true;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            _ResultHonorData = null;
            return false;
        }
        public bool GetPlayerGuildData(SQLPlayerData _PlayerData, out PlayerData.GuildData _ResultGuildData)
        {
            var conn = OpenConnection();
            try
            {
                const int GUILDNAME_COLUMN = 0;
                const int GUILDRANK_COLUMN = 1;
                const int GUILDRANKNR_COLUMN = 2;
                using (var cmd = new NpgsqlCommand("SELECT guildname, guildrank, guildranknr FROM PlayerGuildTable WHERE id=:ID", conn))
                {
                    {
                        var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                        idParam.Value = (int)_PlayerData.PlayerGuildID;
                        cmd.Parameters.Add(idParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultGuildData = new PlayerData.GuildData();
                            _ResultGuildData.GuildName = reader.GetString(GUILDNAME_COLUMN);
                            _ResultGuildData.GuildRank = reader.GetString(GUILDRANK_COLUMN);
                            _ResultGuildData.GuildRankNr = reader.GetInt16(GUILDRANKNR_COLUMN);
                            return true;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            _ResultGuildData = null;
            return false;
        }
        public bool GetPlayerGearData(SQLPlayerData _PlayerData, out PlayerData.GearData _ResultGearData)
        {
            _ResultGearData = null;

            SQLPlayerGearItems gearItems;
            if (GetPlayerGearItems(_PlayerData, out gearItems) == false) return false;

            SQLPlayerGearGems gearGems;
            if (GetPlayerGearGems(_PlayerData, out gearGems) == false)
            {
                gearGems.Gems = null;
            }

            Dictionary<SQLIngameItemID, SQLIngameItemInfo> items;
            if (GetIngameItems(gearItems.Items.Values, out items) == false) return false;

            _ResultGearData = new PlayerData.GearData();
            foreach (var item in gearItems.Items)
            {
                SQLIngameItemInfo itemInfo;
                if (items.TryGetValue(item.Value, out itemInfo) == true)
                {
                    SQLGemInfo gemInfo;
                    if (gearGems.Gems != null && gearGems.Gems.TryGetValue(item.Key, out gemInfo) == true)
                    {
                        _ResultGearData.Items.Add(item.Key, itemInfo.GenerateItemInfo(item.Key, gemInfo));
                    }
                    else
                    {
                        _ResultGearData.Items.Add(item.Key, itemInfo.GenerateItemInfo(item.Key, null));
                    }
                }
            }
            return true;
        }
        public bool GetPlayerCharacterData(SQLPlayerData _PlayerData, out PlayerData.CharacterData _ResultCharacterData)
        {
            if (_PlayerData.PlayerID.Equals(SQLPlayerID.Invalid()))
            {
                _ResultCharacterData = null;
                return false;
            }
            _ResultCharacterData = new PlayerData.CharacterData();
            _ResultCharacterData.Race = _PlayerData.PlayerCharacter.Race;
            _ResultCharacterData.Class = _PlayerData.PlayerCharacter.Class;
            _ResultCharacterData.Sex = _PlayerData.PlayerCharacter.Sex;
            _ResultCharacterData.Level = _PlayerData.PlayerCharacter.Level;
            return true;
        }
        public bool GetPlayerTalentsData(SQLPlayerData _PlayerData, out string _ResultTalentsData)
        {
            if(_PlayerData.PlayerTalentsID != 0)
            {
                var conn = OpenConnection();
                try
                {
                    const int TALENTS_COLUMN = 0;
                    using (var cmd = new NpgsqlCommand("SELECT talents FROM PlayerTalentsInfoTable WHERE id=:ID", conn))
                    {
                        {
                            var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                            idParam.Value = (int)_PlayerData.PlayerTalentsID;
                            cmd.Parameters.Add(idParam);
                        }
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() == true)
                            {
                                _ResultTalentsData = reader.GetString(TALENTS_COLUMN);
                                return true;
                            }
                        }
                    }
                }
                finally
                {
                    CloseConnection();
                }
            }
            _ResultTalentsData = "";
            return false;
        }


        public int LoadPlayer(string _PlayerName, WowRealm _Realm, SQLPlayerID _PlayerID /*= SQLPlayerID.Invalid()*/, out PlayerData.Player _ResultPlayer)
        {
            PlayerData.PlayerHistory _ResultPlayerHistory;
            PlayerData.ExtraData _ResultExtraData;
            return LoadFullPlayer(_PlayerName, _Realm, _PlayerID, out _ResultPlayerHistory, out _ResultPlayer, out _ResultExtraData);
        }
        public int LoadPlayer(string _PlayerName, WowRealm _Realm, SQLPlayerID _PlayerID /*= SQLPlayerID.Invalid()*/, out PlayerData.ExtraData _ResultExtraData)
        {
            PlayerData.Player _ResultPlayer;
            PlayerData.PlayerHistory _ResultPlayerHistory;
            return LoadFullPlayer(_PlayerName, _Realm, _PlayerID, out _ResultPlayerHistory, out _ResultPlayer, out _ResultExtraData);
        }
        public int LoadPlayer(string _PlayerName, WowRealm _Realm, SQLPlayerID _PlayerID /*= SQLPlayerID.Invalid()*/, out PlayerData.PlayerHistory _ResultPlayerHistory)
        {
            PlayerData.Player _ResultPlayer;
            PlayerData.ExtraData _ResultExtraData;
            return LoadFullPlayer(_PlayerName, _Realm, _PlayerID, out _ResultPlayerHistory, out _ResultPlayer, out _ResultExtraData);
        }
        public int LoadPlayer(string _PlayerName, WowRealm _Realm, SQLPlayerID _PlayerID /*= SQLPlayerID.Invalid()*/, out PlayerData.PlayerHistory _ResultPlayerHistory, out PlayerData.Player _ResultPlayer, out PlayerData.ExtraData _ResultExtraData)
        {
            return LoadFullPlayer(_PlayerName, _Realm, _PlayerID, out _ResultPlayerHistory, out _ResultPlayer, out _ResultExtraData);
        }
        public int LoadFullPlayer(string _PlayerName, WowRealm _Realm, SQLPlayerID _PlayerID /*= SQLPlayerID.Invalid()*/, out PlayerData.PlayerHistory _ResultPlayerHistory, out PlayerData.Player _ResultPlayer, out PlayerData.ExtraData _ResultExtraData)
        {
            int playerUpdateCount = 0;
            _ResultPlayerHistory = new PlayerData.PlayerHistory();
            _ResultPlayer = new PlayerData.Player();
            _ResultExtraData = new PlayerData.ExtraData();

            WowVersionEnum wowVersion = StaticValues.GetWowVersion(_Realm);

            if (_PlayerID.IsValid() == false)
            {
                //Figure out PlayerID since it is invalid!
                if (GetPlayerID(_Realm, _PlayerName, out _PlayerID) == false || _PlayerID.IsValid() == false)
                    return -1;
            }

            //List<KeyValuePair<CharacterData, UploadID>> charHistoryItems = new List<KeyValuePair<CharacterData, UploadID>>();
            //List<KeyValuePair<HonorData, UploadID>> honorHistoryItems = new List<KeyValuePair<HonorData, UploadID>>();
            //List<KeyValuePair<GuildData, UploadID>> guildHistoryItems = new List<KeyValuePair<GuildData, UploadID>>();
            //List<KeyValuePair<GearData, UploadID>> gearHistoryItems = new List<KeyValuePair<GearData, UploadID>>();

            List<KeyValuePair<UploadID, List<KeyValuePair<ItemSlot, int>>>> allGearItems = new List<KeyValuePair<UploadID, List<KeyValuePair<ItemSlot, int>>>>();
            List<KeyValuePair<UploadID, SQLArenaInfo>> allArenaInfos = new List<KeyValuePair<UploadID, SQLArenaInfo>>();
            List<int> gearIDs = new List<int>();
            List<int> uniqueGearItems = new List<int>();
            List<int> uniqueArenaIDs = new List<int>();
            var conn = OpenConnection();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT upload.id, upload.uploadtime, upload.contributor, character.updatetime, character.race, character.class, character.sex, character.level" +
                    ", guild.guildname, guild.guildrank, guild.guildranknr" +
                    ", honor.todayhk, honor.todayhonor, honor.yesterdayhk, honor.yesterdayhonor, honor.lifetimehk" +
                    ", honor2.currentrank, honor2.currentrankprogress, honor2.todaydk, honor2.thisweekhk, honor2.thisweekhonor, honor2.lastweekhk, honor2.lastweekhonor, honor2.lastweekstanding, honor2.lifetimedk, honor2.lifetimehighestrank" +
                    ", gear.id, gear.head, gear.neck, gear.shoulder, gear.shirt, gear.chest, gear.belt, gear.legs, gear.feet, gear.wrist, gear.gloves, gear.finger_1, gear.finger_2, gear.trinket_1, gear.trinket_2, gear.back, gear.main_hand, gear.off_hand, gear.ranged, gear.tabard" +
                    ", arena.team_2v2, arena.team_3v3, arena.team_5v5" +
                    ", talents.talents" +
                    " FROM PlayerDataTable character" +
                    " INNER JOIN UploadTable upload ON character.UploadID = upload.ID" +
                    " INNER JOIN PlayerGuildTable guild ON character.GuildInfo = guild.ID" +
                    " INNER JOIN PlayerHonorTable honor ON character.HonorInfo = honor.ID" +
                    " LEFT JOIN PlayerHonorVanillaTable honor2 ON character.HonorInfo = honor2.PlayerHonorID" +
                    " INNER JOIN PlayerGearTable gear ON character.GearInfo = gear.ID" +
                    " INNER JOIN PlayerArenaInfoTable arena ON character.ArenaInfo = arena.ID" +
                    " INNER JOIN PlayerTalentsInfoTable talents ON character.TalentsInfo = talents.ID" +
                    " WHERE character.playerid = " + _PlayerID.ID + " ORDER BY character.updatetime", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        //Logger.ConsoleWriteLine("Starting read for player \"" + player.Key + "\"!");
                        while (reader.Read() == true) //while(reader.ReadRow() != -1) //-1 means end of data
                        {
                            ++playerUpdateCount;
                            //Logger.ConsoleWriteLine("Reading PlayerDataTable!");
                            //PlayerDataTable
                            int uploadID = reader.GetInt32(0); //reader.Read<int>(NpgsqlDbType.Integer);
                            DateTime uploadDate = reader.GetTimeStamp(1).DateTime; //reader.Read<DateTime>(NpgsqlDbType.Timestamp);
                            int contributorID = reader.GetInt32(2); //reader.Read<int>(NpgsqlDbType.Integer);

                            DateTime updateTime = reader.GetTimeStamp(3).DateTime; //reader.Read<DateTime>(NpgsqlDbType.Timestamp);

                            UploadID uploader = new UploadID(contributorID, updateTime);

                            PlayerData.CharacterData characterData = new PlayerData.CharacterData();
                            characterData.Race = (PlayerRace)reader.GetInt16(4); //reader.Read<int>(NpgsqlDbType.Smallint);
                            characterData.Class = (PlayerClass)reader.GetInt16(5); //reader.Read<int>(NpgsqlDbType.Smallint);
                            characterData.Sex = (PlayerSex)reader.GetInt16(6); //reader.Read<int>(NpgsqlDbType.Smallint);
                            characterData.Level = reader.GetInt16(7); //reader.Read<int>(NpgsqlDbType.Smallint);
                            _ResultPlayerHistory.AddToHistory(characterData, uploader);

                            //JOINED DATA BELOW
                            //Logger.ConsoleWriteLine("Reading PlayerGuildTable!");
                            //PlayerGuildTable
                            if (reader.IsDBNull(8)) continue; //if (reader.IsNull) continue;
                            PlayerData.GuildData guildData = new PlayerData.GuildData();
                            guildData.GuildName = reader.GetString(8); //reader.Read<string>(NpgsqlDbType.Text);
                            guildData.GuildRank = reader.GetString(9); //reader.Read<string>(NpgsqlDbType.Text);
                            guildData.GuildRankNr = reader.GetInt16(10); //reader.Read<int>(NpgsqlDbType.Smallint);
                            _ResultPlayerHistory.AddToHistory(guildData, uploader);

                            //Logger.ConsoleWriteLine("Reading PlayerHonorTable!");
                            //PlayerHonorTable
                            if (reader.IsDBNull(11)) continue; //if (reader.IsNull) continue;
                            PlayerData.HonorData honorData = new PlayerData.HonorData();
                            honorData.TodayHK = reader.GetInt32(11); //reader.Read<int>(NpgsqlDbType.Integer);
                            int todayHonorTBC = reader.GetInt32(12); //reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.YesterdayHK = reader.GetInt32(13); //reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.YesterdayHonor = reader.GetInt32(14); //reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.LifetimeHK = reader.GetInt32(15); //reader.Read<int>(NpgsqlDbType.Integer);

                            //Logger.ConsoleWriteLine("Reading PlayerHonorVanillaTable!");
                            //PlayerHonorVanillaTable
                            if (reader.IsDBNull(16))
                            {
                                honorData.CurrentRank = 0; //reader.Skip();
                                honorData.CurrentRankProgress = 0; //reader.Skip();
                                honorData.TodayDK = 0; //reader.Skip();
                                honorData.ThisWeekHK = 0; //reader.Skip();
                                honorData.ThisWeekHonor = 0; //reader.Skip();
                                honorData.LastWeekHK = 0; //reader.Skip();
                                honorData.LastWeekHonor = 0; //reader.Skip();
                                honorData.LastWeekStanding = 0; //reader.Skip();
                                honorData.LifetimeDK = 0; //reader.Skip();
                                honorData.LifetimeHighestRank = 0; //reader.Skip();

                                honorData.TodayHonorTBC = todayHonorTBC;
                                _ResultPlayerHistory.AddToHistory(honorData, uploader);
                            }
                            else
                            {
                                honorData.CurrentRank = reader.GetInt16(16); //reader.Read<int>(NpgsqlDbType.Smallint);
                                honorData.CurrentRankProgress = reader.GetFloat(17); //reader.Read<float>(NpgsqlDbType.Real);
                                honorData.TodayDK = reader.GetInt32(18); //reader.Read<int>(NpgsqlDbType.Integer);
                                honorData.ThisWeekHK = reader.GetInt32(19); //reader.Read<int>(NpgsqlDbType.Integer);
                                honorData.ThisWeekHonor = reader.GetInt32(20); //reader.Read<int>(NpgsqlDbType.Integer);
                                honorData.LastWeekHK = reader.GetInt32(21); //reader.Read<int>(NpgsqlDbType.Integer);
                                honorData.LastWeekHonor = reader.GetInt32(22); //reader.Read<int>(NpgsqlDbType.Integer);
                                honorData.LastWeekStanding = reader.GetInt32(23); //reader.Read<int>(NpgsqlDbType.Integer);
                                honorData.LifetimeDK = reader.GetInt32(24); //reader.Read<int>(NpgsqlDbType.Integer);
                                honorData.LifetimeHighestRank = reader.GetInt32(25); //reader.Read<int>(NpgsqlDbType.Smallint);
                                _ResultPlayerHistory.AddToHistory(honorData, uploader);
                            }

                            //Logger.ConsoleWriteLine("Reading PlayerGearTable!");
                            //PlayerGearTable
                            if (reader.IsDBNull(26)) continue; //if (reader.IsNull) continue;
                            int gearID = reader.GetInt32(26); //reader.Read<int>(NpgsqlDbType.Integer);

                            List<KeyValuePair<ItemSlot, int>> gearItems = new List<KeyValuePair<ItemSlot, int>>();
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Head, reader.GetInt32(27))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Neck, reader.GetInt32(28))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Shoulder, reader.GetInt32(29))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Shirt, reader.GetInt32(30))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Chest, reader.GetInt32(31))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Belt, reader.GetInt32(32))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Legs, reader.GetInt32(33))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Feet, reader.GetInt32(34))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Wrist, reader.GetInt32(35))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Gloves, reader.GetInt32(36))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Finger_1, reader.GetInt32(37))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Finger_2, reader.GetInt32(38))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Trinket_1, reader.GetInt32(39))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Trinket_2, reader.GetInt32(40))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Back, reader.GetInt32(41))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Main_Hand, reader.GetInt32(42))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Off_Hand, reader.GetInt32(43))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Ranged, reader.GetInt32(44))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Tabard, reader.GetInt32(45))); //reader.Read<int>(NpgsqlDbType.Integer)));
                            uniqueGearItems.AddRangeUnique(gearItems.Select((_V) => _V.Value));
                            allGearItems.Add(new KeyValuePair<UploadID, List<KeyValuePair<ItemSlot, int>>>(uploader, gearItems));
                            gearIDs.Add(gearID);
                            //Logger.ConsoleWriteLine("Reading PlayerArenaInfoTable!");
                            //PlayerArenaInfoTable
                            if (reader.IsDBNull(46)) continue; //if (reader.IsNull) continue;
                            SQLArenaInfo arenaInfo = new SQLArenaInfo();
                            arenaInfo.Team2v2 = reader.GetInt32(46); //reader.Read<int>(NpgsqlDbType.Integer);
                            arenaInfo.Team3v3 = reader.GetInt32(47); //reader.Read<int>(NpgsqlDbType.Integer);
                            arenaInfo.Team5v5 = reader.GetInt32(48); //reader.Read<int>(NpgsqlDbType.Integer);
                            uniqueArenaIDs.AddUnique(arenaInfo.Team2v2);
                            uniqueArenaIDs.AddUnique(arenaInfo.Team3v3);
                            uniqueArenaIDs.AddUnique(arenaInfo.Team5v5);
                            allArenaInfos.Add(new KeyValuePair<UploadID, SQLArenaInfo>(uploader, arenaInfo));
                            //Logger.ConsoleWriteLine("Reading PlayerTalentsInfoTable!");
                            //PlayerTalentsInfoTable
                            if (reader.IsDBNull(49)) continue; //if (reader.IsNull) continue;
                            string talents = reader.GetString(49); //reader.Read<string>(NpgsqlDbType.Text);
                            _ResultPlayerHistory.AddTalentsToHistory(talents, uploader);
                        }
                        reader.Dispose();
                    }
                }
                conn.Close();
                if (uniqueGearItems.Count > 0)
                {
                    Dictionary<int, PlayerData.ItemInfo> itemTranslation = new Dictionary<int, PlayerData.ItemInfo>();
                    string sqlSearchIds = String.Join(", ", uniqueGearItems);
                    //Console.WriteLine(sqlSearchIds);

                    if (sqlSearchIds != "" && sqlSearchIds != ", ")
                    {
                        Action<string> _GrabIngameItemTable = (_SearchIDs) =>
                        {
                            conn.Open();
                            try
                            {
                                using (var cmd = new NpgsqlCommand("SELECT id, itemid, enchantid, suffixid, uniqueid FROM ingameitemtable WHERE id IN (" + sqlSearchIds + ")", conn))
                                {
                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read() == true)
                                        {
                                            PlayerData.ItemInfo itemInfo = new PlayerData.ItemInfo();
                                            int itemSQLIndex = reader.GetInt32(0); //reader.Read<int>(NpgsqlDbType.Integer);
                                            itemInfo.ItemID = reader.GetInt32(1); //reader.Read<int>(NpgsqlDbType.Integer);
                                            itemInfo.EnchantID = reader.GetInt32(2); //reader.Read<int>(NpgsqlDbType.Integer);
                                            itemInfo.SuffixID = reader.GetInt32(3); //reader.Read<int>(NpgsqlDbType.Integer);
                                            itemInfo.UniqueID = reader.GetInt32(4); //reader.Read<int>(NpgsqlDbType.Integer);
                                            itemTranslation.AddIfKeyNotExist(itemSQLIndex, itemInfo);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                conn.Close();
                            }
                        };
                        try
                        {
                            int countMax = 400;
                            List<int> chunkedGearItems = new List<int>();
                            for (int i = 0; i < uniqueGearItems.Count; ++i)
                            {
                                chunkedGearItems.Add(uniqueGearItems[i]);
                                if (chunkedGearItems.Count > countMax)
                                {
                                    _GrabIngameItemTable(String.Join(", ", chunkedGearItems));
                                    chunkedGearItems.Clear();
                                }
                            }
                            if (chunkedGearItems.Count > 0)
                            {
                                _GrabIngameItemTable(String.Join(", ", chunkedGearItems));
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.ConsoleWriteLine("Recovered(hopefully) from exception (uniqueGearItemsCount=" + uniqueGearItems.Count + "): " + ex.ToString());
                            int countDiv4 = uniqueGearItems.Count / 4;
                            List<int> chunkedGearItems = new List<int>();
                            for (int i = 0; i < uniqueGearItems.Count; ++i)
                            {
                                chunkedGearItems.Add(uniqueGearItems[i]);
                                if (chunkedGearItems.Count > countDiv4)
                                {
                                    _GrabIngameItemTable(String.Join(", ", chunkedGearItems));
                                    chunkedGearItems.Clear();
                                }
                            }
                            if (chunkedGearItems.Count > 0)
                            {
                                _GrabIngameItemTable(String.Join(", ", chunkedGearItems));
                            }
                        }
                        //using (var cmd = new NpgsqlCommand("SELECT id, itemid, enchantid, suffixid, uniqueid FROM ingameitemtable WHERE id IN (" + sqlSearchIds + ")", conn))
                        //{
                        //    using (var reader = cmd.ExecuteReader())
                        //    {
                        //        if (reader.HasRows == false)
                        //            Logger.ConsoleWriteLine("UNEXPECTED ERROR!!!");

                        //    }
                        //}

                        for (int gI = 0; gI < allGearItems.Count; ++gI)
                        {
                            var gearItems = allGearItems[gI];
                            int gearID = gearIDs[gI];
                            PlayerData.GearData gearData = new PlayerData.GearData();
                            foreach (var itemSlot in gearItems.Value)
                            {
                                if (itemSlot.Value == 0) continue; //Skip 0s they means no gear was recorded at slot!
                                PlayerData.ItemInfo itemInfo = itemTranslation[itemSlot.Value];
                                itemInfo.Slot = itemSlot.Key;
                                gearData.Items.Add(itemSlot.Key, itemInfo);
                            }
                            if (wowVersion != WowVersionEnum.Vanilla)
                            {
                                conn.Open();
                                try
                                {
                                    using (var cmd = new NpgsqlCommand("SELECT itemslot, gemid1, gemid2, gemid3, gemid4 FROM PlayerGearGemsTable WHERE gearid=" + gearID, conn))
                                    {
                                        using (var reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read() == true)
                                            {
                                                if (reader.IsDBNull(0) == true) continue;
                                                ItemSlot slot = (ItemSlot)reader.GetInt16(0); //reader.Read<int>(NpgsqlDbType.Smallint);
                                                int gemID1 = reader.GetInt32(1); //reader.Read<int>(NpgsqlDbType.Integer);
                                                int gemID2 = reader.GetInt32(2); //reader.Read<int>(NpgsqlDbType.Integer);
                                                int gemID3 = reader.GetInt32(3); //reader.Read<int>(NpgsqlDbType.Integer);
                                                int gemID4 = reader.GetInt32(4); //reader.Read<int>(NpgsqlDbType.Integer);
                                                if (gemID1 != 0 || gemID2 != 0 || gemID3 != 0 || gemID4 != 0)
                                                {
                                                    gearData.Items[slot].GemIDs = new int[4] { gemID1, gemID2, gemID3, gemID4 };
                                                }
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    conn.Close();
                                }
                            }
                            _ResultPlayerHistory.AddToHistory(gearData, gearItems.Key);
                        }
                    }
                }
                if (uniqueArenaIDs.Count > 0 && (uniqueArenaIDs.Count > 1 || uniqueArenaIDs[0] != 0))
                {
                    Dictionary<int, PlayerData.ArenaPlayerData> arenaTranslation = new Dictionary<int, PlayerData.ArenaPlayerData>();
                    string sqlSearchIds = String.Join(", ", uniqueArenaIDs);

                    conn.Open();
                    try
                    {
                        using (var cmd = new NpgsqlCommand("SELECT id, teamname, teamrating, gamesplayed, gameswon, playergamesplayed, playerrating FROM PlayerArenaDataTable WHERE id IN (" + sqlSearchIds + ")", conn))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read() == true)
                                {
                                    PlayerData.ArenaPlayerData arenaPlayerData = new PlayerData.ArenaPlayerData();
                                    int arenaDataSQLIndex = reader.GetInt32(0); //reader.Read<int>(NpgsqlDbType.Integer);
                                    arenaPlayerData.TeamName = reader.GetString(1); //reader.Read<string>(NpgsqlDbType.Text);
                                    arenaPlayerData.TeamRating = reader.GetInt32(2); //reader.Read<int>(NpgsqlDbType.Integer);
                                    arenaPlayerData.GamesPlayed = reader.GetInt32(3); //reader.Read<int>(NpgsqlDbType.Integer);
                                    arenaPlayerData.GamesWon = reader.GetInt32(4); //reader.Read<int>(NpgsqlDbType.Integer);
                                    arenaPlayerData.PlayerPlayed = reader.GetInt32(5); //reader.Read<int>(NpgsqlDbType.Integer);
                                    arenaPlayerData.PlayerRating = reader.GetInt32(6); //reader.Read<int>(NpgsqlDbType.Integer);
                                    arenaTranslation.AddIfKeyNotExist(arenaDataSQLIndex, arenaPlayerData);
                                }
                            }
                        }
                    }
                    finally
                    {
                        conn.Close();
                    }

                    foreach (var arenaInfo in allArenaInfos)
                    {
                        PlayerData.ArenaData arenaData = new PlayerData.ArenaData();
                        if (arenaInfo.Value.Team2v2 == 0 || arenaTranslation.TryGetValue(arenaInfo.Value.Team2v2, out arenaData.Team2v2) == false)
                            arenaData.Team2v2 = null;
                        if (arenaInfo.Value.Team3v3 == 0 || arenaTranslation.TryGetValue(arenaInfo.Value.Team3v3, out arenaData.Team3v3) == false)
                            arenaData.Team3v3 = null;
                        if (arenaInfo.Value.Team5v5 == 0 || arenaTranslation.TryGetValue(arenaInfo.Value.Team5v5, out arenaData.Team5v5) == false)
                            arenaData.Team5v5 = null;

                        _ResultPlayerHistory.AddToHistory(arenaData, arenaInfo.Key);
                    }
                }
                if (_ResultPlayerHistory.TalentsHistory.Count == 1 && _ResultPlayerHistory.TalentsHistory[0].Data == "")
                    _ResultPlayerHistory.TalentsHistory = null; //Remove if unused

                if (_ResultPlayerHistory.GetPlayerAtTime(_PlayerName, _Realm, DateTime.MaxValue, out _ResultPlayer) == false)
                {
                    _ResultPlayer = null;
                }

                {
                    conn.Open();
                    try
                    {
                        using (var cmd = new NpgsqlCommand("SELECT upload.contributor, playerseen.updatetime, mount.name FROM playermounttable playerseen" +
                            " INNER JOIN ingamemounttable mount ON playerseen.mountid = mount.ID" +
                            " INNER JOIN uploadtable upload ON playerseen.uploadid = upload.ID" +
                            " WHERE playerseen.playerid = " + _PlayerID.ID, conn))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read() == true)
                                {
                                    int contributorID = reader.GetInt32(0); //reader.Read<int>(NpgsqlDbType.Integer);
                                    DateTime updateTime = reader.GetTimeStamp(1).DateTime; //reader.Read<DateTime>(NpgsqlDbType.Timestamp);
                                    string mountName = reader.GetString(2); //reader.Read<string>(NpgsqlDbType.Text);
                                    _ResultExtraData._AddMount(mountName, new UploadID(contributorID, updateTime));
                                }
                            }
                        }
                    }
                    finally
                    {
                        conn.Close();
                    }
                    conn.Open();
                    try
                    {
                        using (var cmd = new NpgsqlCommand("SELECT upload.contributor, playerseen.updatetime, pet.name, pet.level, pet.creaturefamily, pet.creaturetype FROM playerpettable playerseen" +
                            " INNER JOIN ingamepettable pet ON playerseen.petid = pet.ID" +
                            " INNER JOIN uploadtable upload ON playerseen.uploadid = upload.ID" +
                            " WHERE playerseen.playerid = " + _PlayerID.ID, conn))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read() == true)
                                {
                                    int contributorID = reader.GetInt32(0); //reader.Read<int>(NpgsqlDbType.Integer);
                                    DateTime updateTime = reader.GetTimeStamp(1).DateTime; //reader.Read<DateTime>(NpgsqlDbType.Timestamp);
                                    string petName = reader.GetString(2); //reader.Read<string>(NpgsqlDbType.Text);
                                    int petLevel = reader.GetInt16(3); //reader.Read<int>(NpgsqlDbType.Smallint);
                                    string petFamily = reader.GetString(4); //reader.Read<string>(NpgsqlDbType.Text);
                                    string petType = reader.GetString(5); //reader.Read<string>(NpgsqlDbType.Text);
                                    _ResultExtraData._AddPet(petName, petLevel, petFamily, petType, new UploadID(contributorID, updateTime));
                                }
                            }
                        }
                    }
                    finally
                    {
                        conn.Close();
                    }
                    conn.Open();
                    try
                    {
                        using (var cmd = new NpgsqlCommand("SELECT upload.contributor, playerseen.updatetime, companion.name, companion.level FROM playercompaniontable playerseen" +
                            " INNER JOIN ingamecompaniontable companion ON playerseen.companionid = companion.ID" +
                            " INNER JOIN uploadtable upload ON playerseen.uploadid = upload.ID" +
                            " WHERE playerseen.playerid = " + _PlayerID.ID, conn))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read() == true)
                                {
                                    int contributorID = reader.GetInt32(0); //reader.Read<int>(NpgsqlDbType.Integer);
                                    DateTime updateTime = reader.GetTimeStamp(1).DateTime; //reader.Read<DateTime>(NpgsqlDbType.Timestamp);
                                    string companionName = reader.GetString(2); //reader.Read<string>(NpgsqlDbType.Text);
                                    int companionLevel = reader.GetInt16(3); //reader.Read<int>(NpgsqlDbType.Smallint);
                                    _ResultExtraData._AddCompanion(companionName, companionLevel, new UploadID(contributorID, updateTime));
                                }
                            }
                        }
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            return playerUpdateCount;
        }

    }
}
