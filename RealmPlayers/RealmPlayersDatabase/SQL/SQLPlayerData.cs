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
using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerSex = VF_RealmPlayersDatabase.PlayerSex;
using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;

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
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
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
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
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
            _ResultPlayerData = SQLPlayerData.Invalid();
            return false;
        }
        public bool GetPlayerHonorData(SQLPlayerData _PlayerData, out PlayerData.HonorData _ResultHonorData)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
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
            _ResultHonorData = null;
            return false;
        }
        public bool GetPlayerGuildData(SQLPlayerData _PlayerData, out PlayerData.GuildData _ResultGuildData)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
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
                using (var conn = new NpgsqlConnection(g_ConnectionString))
                {
                    conn.Open();
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
            }
            _ResultTalentsData = "";
            return false;
        }
    }
}
