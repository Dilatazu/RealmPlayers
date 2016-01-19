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
    public partial class SQLComm
    {
        public bool GetPlayerID(WowRealm _Realm, string _PlayerName, out SQLPlayerID _ResultPlayerID)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id FROM playertable WHERE name=:Name AND realm=:Realm", conn))
                {
                    {
                        var nameParam = new NpgsqlParameter("Name", NpgsqlDbType.Text);
                        nameParam.Value = _PlayerName;
                        cmd.Parameters.Add(nameParam);
                    }
                    {
                        var realmParam = new NpgsqlParameter("Realm", NpgsqlDbType.Integer);
                        realmParam.Value = (int)_Realm;
                        cmd.Parameters.Add(realmParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultPlayerID = new SQLPlayerID(reader.GetInt32(0));
                            return true;
                        }
                    }
                }
            }
            _ResultPlayerID = SQLPlayerID.Invalid();
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
                    " INNER JOIN playerdatatable pd ON player.id = pd.playerid AND player.uploadid = pd.uploadid" +
                    " WHERE player.id=:ID", conn))
                {
                    {
                        var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                        idParam.Value = _PlayerID.ID;
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
                    " WHERE honor.id=:ID", conn))
                {
                    {
                        var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                        idParam.Value = _PlayerData.PlayerHonorID;
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
                        idParam.Value = _PlayerData.PlayerGuildID;
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
        public bool GetPlayerGearItems(SQLPlayerData _PlayerData, out SQLGearItems _ResultGearItems)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                const int HEAD_COLUMN = 0;
                const int NECK_COLUMN = 1;
                const int SHOULDER_COLUMN = 2;
                const int SHIRT_COLUMN = 3;
                const int CHEST_COLUMN = 4;
                const int BELT_COLUMN = 5;
                const int LEGS_COLUMN = 6;
                const int FEET_COLUMN = 7;
                const int WRIST_COLUMN = 8;
                const int GLOVES_COLUMN = 9;
                const int FINGER1_COLUMN = 10;
                const int FINGER2_COLUMN = 11;
                const int TRINKET1_COLUMN = 12;
                const int TRINKET2_COLUMN = 13;
                const int BACK_COLUMN = 14;
                const int MAINHAND_COLUMN = 15;
                const int OFFHAND_COLUMN = 16;
                const int RANGED_COLUMN = 17;
                const int TABARD_COLUMN = 18;
                using (var cmd = new NpgsqlCommand("SELECT head, neck, shoulder, shirt, chest, belt, legs, feet, wrist, gloves, finger_1, finger_2, trinket_1, trinket_2, back, main_hand, off_hand, ranged, tabard FROM PlayerGearTable WHERE id=:ID", conn))
                {
                    {
                        var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                        idParam.Value = _PlayerData.PlayerGearID;
                        cmd.Parameters.Add(idParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultGearItems = SQLGearItems.CreateEmpty();
                            _ResultGearItems.Items.Add(ItemSlot.Head, new SQLIngameItemID(reader.GetInt32(HEAD_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Neck, new SQLIngameItemID(reader.GetInt32(NECK_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Shoulder, new SQLIngameItemID(reader.GetInt32(SHOULDER_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Shirt, new SQLIngameItemID(reader.GetInt32(SHIRT_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Chest, new SQLIngameItemID(reader.GetInt32(CHEST_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Belt, new SQLIngameItemID(reader.GetInt32(BELT_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Legs, new SQLIngameItemID(reader.GetInt32(LEGS_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Feet, new SQLIngameItemID(reader.GetInt32(FEET_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Wrist, new SQLIngameItemID(reader.GetInt32(WRIST_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Gloves, new SQLIngameItemID(reader.GetInt32(GLOVES_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Finger_1, new SQLIngameItemID(reader.GetInt32(FINGER1_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Finger_2, new SQLIngameItemID(reader.GetInt32(FINGER2_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Trinket_1, new SQLIngameItemID(reader.GetInt32(TRINKET1_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Trinket_2, new SQLIngameItemID(reader.GetInt32(TRINKET2_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Back, new SQLIngameItemID(reader.GetInt32(BACK_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Main_Hand, new SQLIngameItemID(reader.GetInt32(MAINHAND_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Off_Hand, new SQLIngameItemID(reader.GetInt32(OFFHAND_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Ranged, new SQLIngameItemID(reader.GetInt32(RANGED_COLUMN)));
                            _ResultGearItems.Items.Add(ItemSlot.Tabard, new SQLIngameItemID(reader.GetInt32(TABARD_COLUMN)));
                            _ResultGearItems.Items.RemoveAll((_V) => _V.Value.ID == 0);
                            return true;
                        }
                    }
                }
            }
            _ResultGearItems = SQLGearItems.CreateEmpty();
            return false;
        }
        public bool GetIngameItems(IEnumerable<SQLIngameItemID> _IngameItems, out Dictionary<SQLIngameItemID, PlayerData.ItemInfo> _ResultItems)
        {
            var distinctItems = _IngameItems.Distinct();
            int[] itemsArray = new int[distinctItems.Count()];
            int itemsCounter = 0;
            foreach (var item in distinctItems)
            {
                if(item.ID > 0)
                {
                    itemsArray[itemsCounter++] = item.ID;
                }
            }
            if(itemsCounter != itemsArray.Length)
            {
                int[] oldItemsArray = itemsArray;
                itemsArray = new int[itemsCounter];
                for(int i = 0; i < itemsCounter; ++i)
                {
                    itemsArray[i] = oldItemsArray[i];
                }
            }
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                const int ID_COLUMN = 0;
                const int ITEMID_COLUMN = 1;
                const int ENCHANTID_COLUMN = 2;
                const int SUFFIXID_COLUMN = 3;
                const int UNIQUEID_COLUMN = 4;
                using (var cmd = new NpgsqlCommand("SELECT id, itemid, enchantid, suffixid, uniqueid FROM IngameItemTable WHERE id IN (:IDs)", conn))
                {
                    {
                        var idArrayParam = new NpgsqlParameter("IDs", NpgsqlDbType.Array | NpgsqlDbType.Integer);
                        idArrayParam.Value = itemsArray;
                        cmd.Parameters.Add(idArrayParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows == true)
                        {
                            _ResultItems = new Dictionary<SQLIngameItemID, VF_RealmPlayersDatabase.PlayerData.ItemInfo>();
                            while (reader.Read())
                            {
                                SQLIngameItemID sqlID = new SQLIngameItemID(reader.GetInt32(ID_COLUMN));
                                PlayerData.ItemInfo itemInfo = new PlayerData.ItemInfo();
                                itemInfo.ItemID = reader.GetInt32(ITEMID_COLUMN);
                                itemInfo.EnchantID = reader.GetInt32(ENCHANTID_COLUMN);
                                itemInfo.SuffixID = reader.GetInt32(SUFFIXID_COLUMN);
                                itemInfo.UniqueID = reader.GetInt32(UNIQUEID_COLUMN);
                                _ResultItems.AddOrSet(sqlID, itemInfo);
                            }
                            return true;
                        }
                    }
                }
            }
            _ResultItems = null;
            return false;
        }
    }
}
