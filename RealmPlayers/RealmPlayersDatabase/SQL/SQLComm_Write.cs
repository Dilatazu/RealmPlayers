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

namespace VF
{
    public partial class SQLComm
    {
        public SQLUploadID GenerateNewUploadEntry(Contributor _Contributor, DateTime _DateTime/* = DateTime.UtcNow*/)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO uploadtable(id, uploadtime, contributor) VALUES (DEFAULT, :UploadTime, :Contributor) RETURNING id", conn))
                {
                    {
                        var uploadTimeParam = new NpgsqlParameter("UploadTime", NpgsqlDbType.Timestamp);
                        uploadTimeParam.Value = _DateTime;
                        cmd.Parameters.Add(uploadTimeParam);
                    }
                    {
                        var contributorParam = new NpgsqlParameter("Contributor", NpgsqlDbType.Integer);
                        contributorParam.Value = _Contributor.GetContributorID();
                        cmd.Parameters.Add(contributorParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return new SQLUploadID(reader.GetInt32(0));
                        }
                    }
                }
                conn.Close();
            }
            return new SQLUploadID(0);
        }
        public SQLIngameItemID GenerateNewIngameItemEntry(SQLIngameItemInfo _ItemInfo)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO ingameitemtable(id, itemid, enchantid, suffixid, uniqueid) VALUES (DEFAULT, :ItemID, :EnchantID, :SuffixID, :UniqueID) RETURNING id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("ItemID", NpgsqlDbType.Integer)).Value = _ItemInfo.ItemID;
                    cmd.Parameters.Add(new NpgsqlParameter("EnchantID", NpgsqlDbType.Integer)).Value = _ItemInfo.EnchantID;
                    cmd.Parameters.Add(new NpgsqlParameter("SuffixID", NpgsqlDbType.Integer)).Value = _ItemInfo.SuffixID;
                    cmd.Parameters.Add(new NpgsqlParameter("UniqueID", NpgsqlDbType.Integer)).Value = _ItemInfo.UniqueID;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return new SQLIngameItemID(reader.GetInt32(0));
                        }
                    }
                }
                conn.Close();
            }
            return new SQLIngameItemID(0);
        }
        public int GenerateNewPlayerGuildDataEntry(PlayerData.GuildData _GuildData)
        {
            if (_GuildData == null)
                return 0;

            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO playerguildtable(id, guildname, guildrank, guildranknr) VALUES (DEFAULT, :GuildName, :GuildRank, :GuildRankNr) RETURNING id", conn))
                {
                    {
                        var guildNameParam = new NpgsqlParameter("GuildName", NpgsqlDbType.Text);
                        guildNameParam.Value = _GuildData.GuildName;
                        cmd.Parameters.Add(guildNameParam);
                    }
                    {
                        var guildRankrParam = new NpgsqlParameter("GuildRank", NpgsqlDbType.Text);
                        guildRankrParam.Value = _GuildData.GuildRank;
                        cmd.Parameters.Add(guildRankrParam);
                    }
                    {
                        var guildRankrNrParam = new NpgsqlParameter("GuildRankNr", NpgsqlDbType.Smallint);
                        guildRankrNrParam.Value = _GuildData.GuildRankNr;
                        cmd.Parameters.Add(guildRankrNrParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
                conn.Close();
            }
            return 0;
        }
        public int GenerateNewPlayerHonorDataEntry(PlayerData.HonorData _HonorData, WowVersionEnum _WowVersion)
        {
            if (_HonorData == null)
                return 0;

            int resultHonorID = 0;
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO playerhonortable(id, todayhk, todayhonor, yesterdayhk, yesterdayhonor, lifetimehk) VALUES (DEFAULT, :TodayHK, :TodayHonor, :YesterdayHK, :YesterdayHonor, :LifetimeHK) RETURNING id", conn))
                {
                    {
                        var todayHKParam = new NpgsqlParameter("TodayHK", NpgsqlDbType.Integer);
                        todayHKParam.Value = _HonorData.TodayHK;
                        cmd.Parameters.Add(todayHKParam);
                    }
                    {
                        var todayHonorParam = new NpgsqlParameter("TodayHonor", NpgsqlDbType.Integer);
                        if(_WowVersion == WowVersionEnum.Vanilla)
                            todayHonorParam.Value = 0;
                        else
                            todayHonorParam.Value = _HonorData.TodayHonorTBC;
                        cmd.Parameters.Add(todayHonorParam);
                    }
                    {
                        var yesterdayHKParam = new NpgsqlParameter("YesterdayHK", NpgsqlDbType.Integer);
                        yesterdayHKParam.Value = _HonorData.YesterdayHK;
                        cmd.Parameters.Add(yesterdayHKParam);
                    }
                    {
                        var yesterdayHonorParam = new NpgsqlParameter("YesterdayHonor", NpgsqlDbType.Integer);
                        yesterdayHonorParam.Value = _HonorData.YesterdayHonor;
                        cmd.Parameters.Add(yesterdayHonorParam);
                    }
                    {
                        var lifetimeHKParam = new NpgsqlParameter("LifetimeHK", NpgsqlDbType.Integer);
                        lifetimeHKParam.Value = _HonorData.LifetimeHK;
                        cmd.Parameters.Add(lifetimeHKParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            resultHonorID = reader.GetInt32(0);
                        }
                    }
                }

                if(_WowVersion == WowVersionEnum.Vanilla && resultHonorID != 0)
                {
                    using (var cmd = new NpgsqlCommand("INSERT INTO playerhonorvanillatable(playerhonorid, currentrank, currentrankprogress, todaydk, thisweekhk, thisweekhonor, lastweekhk, lastweekhonor, lastweekstanding, lifetimedk, lifetimehighestrank) VALUES (:ID, :CurrentRank, :CurrentRankProgress, :TodayDK, :ThisWeekHK, :ThisWeekHonor, :LastWeekHK, :LastWeekHonor, :LastWeekStanding, :LifetimeDK, :LifetimeHighestRank) RETURNING playerhonorid", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("ID"                 , NpgsqlDbType.Integer)).Value = (int)resultHonorID;
                        cmd.Parameters.Add(new NpgsqlParameter("CurrentRank"        , NpgsqlDbType.Smallint)).Value = _HonorData.CurrentRank;
                        cmd.Parameters.Add(new NpgsqlParameter("CurrentRankProgress", NpgsqlDbType.Real)).Value = _HonorData.CurrentRankProgress;
                        cmd.Parameters.Add(new NpgsqlParameter("TodayDK"            , NpgsqlDbType.Integer)).Value = _HonorData.TodayDK;
                        cmd.Parameters.Add(new NpgsqlParameter("ThisWeekHK"         , NpgsqlDbType.Integer)).Value = _HonorData.ThisWeekHK;
                        cmd.Parameters.Add(new NpgsqlParameter("ThisWeekHonor"      , NpgsqlDbType.Integer)).Value = _HonorData.ThisWeekHonor;
                        cmd.Parameters.Add(new NpgsqlParameter("LastWeekHK"         , NpgsqlDbType.Integer)).Value = _HonorData.LastWeekHK;
                        cmd.Parameters.Add(new NpgsqlParameter("LastWeekHonor"      , NpgsqlDbType.Integer)).Value = _HonorData.LastWeekHonor;
                        cmd.Parameters.Add(new NpgsqlParameter("LastWeekStanding"   , NpgsqlDbType.Integer)).Value = _HonorData.LastWeekStanding;
                        cmd.Parameters.Add(new NpgsqlParameter("LifetimeDK"         , NpgsqlDbType.Integer)).Value = _HonorData.LifetimeDK;
                        cmd.Parameters.Add(new NpgsqlParameter("LifetimeHighestRank", NpgsqlDbType.Smallint)).Value = _HonorData.LifetimeHighestRank;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() == true)
                            {
                                if (resultHonorID == reader.GetInt32(0))
                                    return resultHonorID;
                            }
                        }
                    }
                }
                conn.Close();
            }
            return resultHonorID;
        }
        public int GenerateNewPlayerGearDataEntry(PlayerData.GearData _GearData, WowVersionEnum _WowVersion)
        {
            if (_GearData == null)
                return 0;

            Dictionary<ItemSlot, SQLIngameItemID> ingameItemIDs = new Dictionary<ItemSlot, SQLIngameItemID>();
            foreach (ItemSlot slot in Enum.GetValues(typeof(ItemSlot)))
            {
                ingameItemIDs.Add(slot, new SQLIngameItemID(0));
            }
            foreach (var item in _GearData.Items)
            {
                SQLIngameItemID itemID = GenerateNewIngameItemEntry(new SQLIngameItemInfo(item.Value));
                ingameItemIDs.AddOrSet(item.Key, itemID);
            }

            int gearDataEntryID = 0;
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO playergeartable(id, head, neck, shoulder, shirt, chest, belt, legs, feet, wrist, gloves, finger_1, finger_2, trinket_1, trinket_2, back, main_hand, off_hand, ranged, tabard) VALUES (DEFAULT, :Head, :Neck, :Shoulder, :Shirt, :Chest, :Belt, :Legs, :Feet, :Wrist, :Gloves, :Finger_1, :Finger_2, :Trinket_1, :Trinket_2, :Back, :Main_Hand, :Off_Hand, :Ranged, :Tabard) RETURNING id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("Head", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Head];
                    cmd.Parameters.Add(new NpgsqlParameter("Neck", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Neck];
                    cmd.Parameters.Add(new NpgsqlParameter("Shoulder", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Shoulder];
                    cmd.Parameters.Add(new NpgsqlParameter("Shirt", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Shirt];
                    cmd.Parameters.Add(new NpgsqlParameter("Chest", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Chest];
                    cmd.Parameters.Add(new NpgsqlParameter("Belt", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Belt];
                    cmd.Parameters.Add(new NpgsqlParameter("Legs", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Legs];
                    cmd.Parameters.Add(new NpgsqlParameter("Feet", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Feet];
                    cmd.Parameters.Add(new NpgsqlParameter("Wrist", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Wrist];
                    cmd.Parameters.Add(new NpgsqlParameter("Gloves", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Gloves];
                    cmd.Parameters.Add(new NpgsqlParameter("Finger_1", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Finger_1];
                    cmd.Parameters.Add(new NpgsqlParameter("Finger_2", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Finger_2];
                    cmd.Parameters.Add(new NpgsqlParameter("Trinket_1", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Trinket_1];
                    cmd.Parameters.Add(new NpgsqlParameter("Trinket_2", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Trinket_2];
                    cmd.Parameters.Add(new NpgsqlParameter("Back", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Back];
                    cmd.Parameters.Add(new NpgsqlParameter("Main_Hand", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Main_Hand];
                    cmd.Parameters.Add(new NpgsqlParameter("Off_Hand", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Off_Hand];
                    cmd.Parameters.Add(new NpgsqlParameter("Ranged", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Ranged];
                    cmd.Parameters.Add(new NpgsqlParameter("Tabard", NpgsqlDbType.Integer)).Value = (int)ingameItemIDs[ItemSlot.Tabard];
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            gearDataEntryID = reader.GetInt32(0);
                        }
                    }
                }
                if(gearDataEntryID != 0 && _WowVersion != WowVersionEnum.Vanilla)
                {
                    //There may be gems we should add to the geardata!
                    Dictionary<ItemSlot, SQLGemInfo> gems = new Dictionary<ItemSlot, SQLGemInfo>();
                    foreach (var item in _GearData.Items)
                    {
                        if(item.Value.GemIDs != null)
                        {
                            SQLGemInfo gemInfo = new SQLGemInfo(item.Value.GemIDs);
                            if(gemInfo.IsNull() == false)
                            {
                                gems.Add(item.Key, gemInfo);
                            }
                        }
                    }
                    if(gems.Count > 0)
                    {
                        GenerateNewPlayerGearGemEntries(gearDataEntryID, gems);
                    }
                }
                conn.Close();
            }
            return gearDataEntryID;
        }
        public bool GenerateNewPlayerGearGemEntries(int _GearID, Dictionary<ItemSlot, SQLGemInfo> _Gems)
        {
            if (_GearID == 0 || _Gems == null || _Gems.Count == 0)
                return false;

            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmdGearGems = conn.BeginBinaryImport("COPY PlayerGearGemsTable (gearid, itemslot, gemid1, gemid2, gemid3, gemid4) FROM STDIN BINARY"))
                {
                    foreach (var gemInfo in _Gems)
                    {
                        cmdGearGems.StartRow();
                        cmdGearGems.Write(_GearID, NpgsqlDbType.Integer);
                        cmdGearGems.Write((int)gemInfo.Key, NpgsqlDbType.Smallint);
                        cmdGearGems.Write(gemInfo.Value.GemID1, NpgsqlDbType.Integer);
                        cmdGearGems.Write(gemInfo.Value.GemID2, NpgsqlDbType.Integer);
                        cmdGearGems.Write(gemInfo.Value.GemID3, NpgsqlDbType.Integer);
                        cmdGearGems.Write(gemInfo.Value.GemID4, NpgsqlDbType.Integer);
                    }
                    cmdGearGems.Close();
                }
                conn.Close();
            }
            return true;
        }

        public int GenerateNewPlayerArenaInfoEntry(PlayerData.ArenaData _ArenaData)
        {
            if (_ArenaData == null || (_ArenaData.Team2v2 == null && _ArenaData.Team3v3 == null && _ArenaData.Team5v5 == null))
                return 0;

            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                Func<PlayerData.ArenaPlayerData, int> _GenerateNewPlayerArenaTeamDataEntry = (PlayerData.ArenaPlayerData _Data) =>
                {
                    if (_Data == null) return 0;

                    using (var cmd = new NpgsqlCommand("INSERT INTO playerarenadatatable(id, teamname, teamrating, gamesplayed, gameswon, playergamesplayed, playerrating) VALUES (DEFAULT, :TeamName, :TeamRating, :GamesPlayed, :GamesWon, :PlayerGamesPlayed, :PlayerRating) RETURNING id", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("TeamName", NpgsqlDbType.Text)).Value = _Data.TeamName;
                        cmd.Parameters.Add(new NpgsqlParameter("TeamRating", NpgsqlDbType.Integer)).Value = _Data.TeamRating;
                        cmd.Parameters.Add(new NpgsqlParameter("GamesPlayed", NpgsqlDbType.Integer)).Value = _Data.GamesPlayed;
                        cmd.Parameters.Add(new NpgsqlParameter("GamesWon", NpgsqlDbType.Integer)).Value = _Data.GamesWon;
                        cmd.Parameters.Add(new NpgsqlParameter("PlayerGamesPlayed", NpgsqlDbType.Integer)).Value = _Data.PlayerPlayed;
                        cmd.Parameters.Add(new NpgsqlParameter("PlayerRating", NpgsqlDbType.Integer)).Value = _Data.PlayerRating;

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() == true)
                            {
                                return reader.GetInt32(0);
                            }
                        }
                    }

                    return 0;
                };

                int team_2v2_ID = _GenerateNewPlayerArenaTeamDataEntry(_ArenaData.Team2v2);
                int team_3v3_ID = _GenerateNewPlayerArenaTeamDataEntry(_ArenaData.Team3v3);
                int team_5v5_ID = _GenerateNewPlayerArenaTeamDataEntry(_ArenaData.Team5v5);

                using (var cmd = new NpgsqlCommand("INSERT INTO playerarenainfotable(id, team_2v2, team_3v3, team_5v5) VALUES (DEFAULT, :Team2v2, :Team3v3, :Team5v5) RETURNING id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("Team2v2", NpgsqlDbType.Integer)).Value = (int)team_2v2_ID;
                    cmd.Parameters.Add(new NpgsqlParameter("Team3v3", NpgsqlDbType.Integer)).Value = (int)team_3v3_ID;
                    cmd.Parameters.Add(new NpgsqlParameter("Team5v5", NpgsqlDbType.Integer)).Value = (int)team_5v5_ID;
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            return 0;
        }

        public int GenerateNewPlayerTalentsDataEntry(string _TalentsData)
        {
            if (_TalentsData == null || _TalentsData == "")
                return 0;

            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO playertalentsinfotable(id, talents) VALUES (DEFAULT, :Talents) RETURNING id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("Talents", NpgsqlDbType.Text)).Value = _TalentsData;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }

            return 0;
        }

        public bool GenerateNewPlayerDataEntry(SQLPlayerData _PlayerData)
        {
            if (_PlayerData.PlayerID.ID == 0 || _PlayerData.UploadID.ID == 0 || _PlayerData.PlayerCharacter == null)
                return false;

            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmdPlayerData = conn.BeginBinaryImport("COPY PlayerDataTable (playerid, uploadid, updatetime, race, class, sex, level, guildinfo, honorinfo, gearinfo, arenainfo, talentsinfo) FROM STDIN BINARY"))
                {
                    cmdPlayerData.StartRow();
                    cmdPlayerData.Write((int)_PlayerData.PlayerID, NpgsqlDbType.Integer);
                    cmdPlayerData.Write((int)_PlayerData.UploadID, NpgsqlDbType.Integer);
                    cmdPlayerData.Write(_PlayerData.UpdateTime, NpgsqlDbType.Timestamp);
                    cmdPlayerData.Write((int)_PlayerData.PlayerCharacter.Race, NpgsqlDbType.Smallint);
                    cmdPlayerData.Write((int)_PlayerData.PlayerCharacter.Class, NpgsqlDbType.Smallint);
                    cmdPlayerData.Write((int)_PlayerData.PlayerCharacter.Sex, NpgsqlDbType.Smallint);
                    cmdPlayerData.Write((int)_PlayerData.PlayerCharacter.Level, NpgsqlDbType.Smallint);
                    cmdPlayerData.Write((int)_PlayerData.PlayerGuildID, NpgsqlDbType.Integer);
                    cmdPlayerData.Write((int)_PlayerData.PlayerHonorID, NpgsqlDbType.Integer);
                    cmdPlayerData.Write((int)_PlayerData.PlayerGearID, NpgsqlDbType.Integer);
                    cmdPlayerData.Write((int)_PlayerData.PlayerArenaID, NpgsqlDbType.Integer);
                    cmdPlayerData.Write((int)_PlayerData.PlayerTalentsID, NpgsqlDbType.Integer);

                    cmdPlayerData.Close();
                }
            }
            return true;
        }

        public bool UpdatePlayerEntry(SQLPlayerID _PlayerID, SQLUploadID _UploadID)
        {
            if (_PlayerID.IsValid() == false || _UploadID.IsValid() == false)
                return false;

            int affectedRows = 0;
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE playertable SET latestuploadid = :LatestUploadID WHERE id = :ID", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("ID", NpgsqlDbType.Integer)).Value = (int)_PlayerID;
                    cmd.Parameters.Add(new NpgsqlParameter("LatestUploadID", NpgsqlDbType.Integer)).Value = (int)_UploadID;
                    affectedRows = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }

            if (affectedRows == 1)
                return true;

            if (affectedRows > 1)
            {
                VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Error, UpdatePlayerEntry somehow modified more than 1 line. This is pretty weird and unexpected!!!");
            }
            return false;
        }

        public SQLPlayerID UpdateLatestPlayerDataEntry(SQLPlayerID _PlayerID, SQLUploadID _UploadID, PlayerData.Player _PlayerData)
        {
            WowVersionEnum wowVersion = VF_RealmPlayersDatabase.StaticValues.GetWowVersion(_PlayerData.Realm);
            SQLPlayerData playerData = new SQLPlayerData();
            playerData.PlayerID = _PlayerID;
            playerData.UploadID = _UploadID;
            playerData.UpdateTime = _PlayerData.LastSeen;
            playerData.PlayerCharacter = _PlayerData.Character;
            playerData.PlayerGuildID = GenerateNewPlayerGuildDataEntry(_PlayerData.Guild);
            playerData.PlayerHonorID = GenerateNewPlayerHonorDataEntry(_PlayerData.Honor, wowVersion);
            playerData.PlayerGearID = GenerateNewPlayerGearDataEntry(_PlayerData.Gear, wowVersion);
            if (wowVersion != WowVersionEnum.Vanilla)
            {
                playerData.PlayerArenaID = GenerateNewPlayerArenaInfoEntry(_PlayerData.Arena);
                playerData.PlayerTalentsID = GenerateNewPlayerTalentsDataEntry(_PlayerData.TalentPointsData);
            }
            else
            {
                playerData.PlayerArenaID = 0;
                playerData.PlayerTalentsID = 0;
            }
            if (GenerateNewPlayerDataEntry(playerData) == true)
            {
                if(UpdatePlayerEntry(playerData.PlayerID, playerData.UploadID) == false)
                {
                    VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Error in UpdateLatestPlayerDataEntry, could not UpdatePlayerEntry!!!");
                    return SQLPlayerID.Invalid();
                }
            }
            else
            {
                VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Error in UpdateLatestPlayerDataEntry, could not GenerateNewPlayerDataEntry!!!");
                return SQLPlayerID.Invalid();
            }
            return _PlayerID;
        }
        public SQLPlayerID GenerateNewPlayerEntry(SQLUploadID _UploadID, PlayerData.Player _PlayerData)
        {
            WowVersionEnum wowVersion = VF_RealmPlayersDatabase.StaticValues.GetWowVersion(_PlayerData.Realm);
            SQLPlayerID playerID = new SQLPlayerID();
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO playertable(id, name, realm, latestuploadid) VALUES (DEFAULT, :Name, :Realm, :LatestUploadID) RETURNING id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("Name", NpgsqlDbType.Text)).Value = _PlayerData.Name;
                    cmd.Parameters.Add(new NpgsqlParameter("Realm", NpgsqlDbType.Integer)).Value = (int)_PlayerData.Realm;
                    cmd.Parameters.Add(new NpgsqlParameter("LatestUploadID", NpgsqlDbType.Integer)).Value = (int)0;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            playerID = new SQLPlayerID(reader.GetInt32(0));
                        }
                    }
                }
                conn.Close();
            }

            if (playerID.IsValid() == false)
                return playerID;

            return UpdateLatestPlayerDataEntry(playerID, _UploadID, _PlayerData);
        }

        public int GenerateMountID(string _MountName)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id FROM ingamemounttable WHERE name = :Name", conn))
                {
                    {
                        var nameParam = new NpgsqlParameter("Name", NpgsqlDbType.Text);
                        nameParam.Value = _MountName;
                        cmd.Parameters.Add(nameParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
                conn.Close();
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO ingamemounttable(id, name) VALUES(DEFAULT, :Name) RETURNING id", conn))
                {
                    {
                        var nameParam = new NpgsqlParameter("Name", NpgsqlDbType.Text);
                        nameParam.Value = _MountName;
                        cmd.Parameters.Add(nameParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
                conn.Close();
            }
            return 0;
        }

        public int GeneratePetID(string _PetName, int _PetLevel, string _PetCreatureFamily, string _PetCreatureType)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id FROM ingamepettable WHERE name = :Name AND level = :Level AND creaturefamily = :CreatureFamily AND creaturetype = :CreatureType", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("Name", NpgsqlDbType.Text)).Value = _PetName;
                    cmd.Parameters.Add(new NpgsqlParameter("Level", NpgsqlDbType.Smallint)).Value = _PetLevel;
                    cmd.Parameters.Add(new NpgsqlParameter("CreatureFamily", NpgsqlDbType.Text)).Value = _PetCreatureFamily;
                    cmd.Parameters.Add(new NpgsqlParameter("CreatureType", NpgsqlDbType.Text)).Value = _PetCreatureType;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
                conn.Close();
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO ingamepettable(id, name, level, creaturefamily, creaturetype) VALUES(DEFAULT, :Name, :Level, :CreatureFamily, :CreatureType) RETURNING id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("Name", NpgsqlDbType.Text)).Value = _PetName;
                    cmd.Parameters.Add(new NpgsqlParameter("Level", NpgsqlDbType.Smallint)).Value = _PetLevel;
                    cmd.Parameters.Add(new NpgsqlParameter("CreatureFamily", NpgsqlDbType.Text)).Value = _PetCreatureFamily;
                    cmd.Parameters.Add(new NpgsqlParameter("CreatureType", NpgsqlDbType.Text)).Value = _PetCreatureType;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
                conn.Close();
            }
            return 0;
        }

        public int GenerateCompanionID(string _CompanionName, int _CompanionLevel)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id FROM ingamecompaniontable WHERE name = :Name AND level = :Level", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("Name", NpgsqlDbType.Text)).Value = _CompanionName;
                    cmd.Parameters.Add(new NpgsqlParameter("Level", NpgsqlDbType.Smallint)).Value = _CompanionLevel;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
                conn.Close();
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO ingamecompaniontable(id, name, level) VALUES(DEFAULT, :Name, :Level) RETURNING id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("Name", NpgsqlDbType.Text)).Value = _CompanionName;
                    cmd.Parameters.Add(new NpgsqlParameter("Level", NpgsqlDbType.Smallint)).Value = _CompanionLevel;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
                conn.Close();
            }
            return 0;
        }
        public bool AddPlayerMount(VF.SQLPlayerID _PlayerID, VF.SQLUploadID _UploadID, DateTime _UpdateTime, int _MountID)
        {
            try
            {
                using (var conn = new NpgsqlConnection(g_ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO playermounttable(playerid, uploadid, updatetime, mountid) VALUES(:PlayerID, :UploadID, :UpdateTime, :MountID)", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("PlayerID", NpgsqlDbType.Integer)).Value = (int)_PlayerID;
                        cmd.Parameters.Add(new NpgsqlParameter("UploadID", NpgsqlDbType.Integer)).Value = (int)_UploadID;
                        cmd.Parameters.Add(new NpgsqlParameter("UpdateTime", NpgsqlDbType.Timestamp)).Value = _UpdateTime;
                        cmd.Parameters.Add(new NpgsqlParameter("MountID", NpgsqlDbType.Integer)).Value = _MountID;
                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return true;
            }
            catch (NpgsqlException ex)
            {
                if(ex.Code == "23505") //unique key violation
                {
                    //Ignore error, this just means the data was already in the DB!
                    return true;
                }
                else
                {
                    VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("AddPlayerMount() Failed to add PlayerID(" + (int)_PlayerID + "), UploadID(" + (int)_UploadID + ") UpdateTime(" + _UpdateTime + ") MountID(" + _MountID + ")");
                    VF_RealmPlayersDatabase.Logger.LogException(ex);
                }
            }
            return false;
        }
        public bool AddPlayerPet(VF.SQLPlayerID _PlayerID, VF.SQLUploadID _UploadID, DateTime _UpdateTime, int _PetID)
        {
            try
            {
                using (var conn = new NpgsqlConnection(g_ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO playerpettable(playerid, uploadid, updatetime, petid) VALUES(:PlayerID, :UploadID, :UpdateTime, :PetID)", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("PlayerID", NpgsqlDbType.Integer)).Value = (int)_PlayerID;
                        cmd.Parameters.Add(new NpgsqlParameter("UploadID", NpgsqlDbType.Integer)).Value = (int)_UploadID;
                        cmd.Parameters.Add(new NpgsqlParameter("UpdateTime", NpgsqlDbType.Timestamp)).Value = _UpdateTime;
                        cmd.Parameters.Add(new NpgsqlParameter("PetID", NpgsqlDbType.Integer)).Value = _PetID;
                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return true;
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code == "23505") //unique key violation
                {
                    //Ignore error, this just means the data was already in the DB!
                    return true;
                }
                else
                {
                    VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("AddPlayerPet() Failed to add PlayerID(" + (int)_PlayerID + "), UploadID(" + (int)_UploadID + ") UpdateTime(" + _UpdateTime + ") PetID(" + _PetID + ")");
                    VF_RealmPlayersDatabase.Logger.LogException(ex);
                }
            }
            return false;
        }
        public bool AddPlayerCompanion(VF.SQLPlayerID _PlayerID, VF.SQLUploadID _UploadID, DateTime _UpdateTime, int _CompanionID)
        {
            try
            {
                using (var conn = new NpgsqlConnection(g_ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO playercompaniontable(playerid, uploadid, updatetime, companionid) VALUES(:PlayerID, :UploadID, :UpdateTime, :CompanionID)", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("PlayerID", NpgsqlDbType.Integer)).Value = (int)_PlayerID;
                        cmd.Parameters.Add(new NpgsqlParameter("UploadID", NpgsqlDbType.Integer)).Value = (int)_UploadID;
                        cmd.Parameters.Add(new NpgsqlParameter("UpdateTime", NpgsqlDbType.Timestamp)).Value = _UpdateTime;
                        cmd.Parameters.Add(new NpgsqlParameter("CompanionID", NpgsqlDbType.Integer)).Value = _CompanionID;
                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return true;
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code == "23505") //unique key violation
                {
                    //Ignore error, this just means the data was already in the DB!
                    return true;
                }
                else
                {
                    VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("AddPlayerCompanion() Failed to add PlayerID(" + (int)_PlayerID + "), UploadID(" + (int)_UploadID + ") UpdateTime(" + _UpdateTime + ") CompanionID(" + _CompanionID + ")");
                    VF_RealmPlayersDatabase.Logger.LogException(ex);
                }
            }
            return false;
        }
    }
}
