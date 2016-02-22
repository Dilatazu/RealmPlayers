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
    public struct SQLPlayerGearItems
    {
        public Dictionary<ItemSlot, SQLIngameItemID> Items;
        public static SQLPlayerGearItems CreateEmpty()
        {
            SQLPlayerGearItems value = new SQLPlayerGearItems();
            value.Items = new Dictionary<ItemSlot, SQLIngameItemID>();
            return value;
        }
    }
    public struct SQLGemInfo
    {
        public int GemID1;
        public int GemID2;
        public int GemID3;
        public int GemID4;
        public SQLGemInfo(int[] _GemIDs)
        {
            GemID1 = 0;
            GemID2 = 0;
            GemID3 = 0;
            GemID4 = 0;
            for (int i = 0; i < _GemIDs.Length; ++i)
            {
                if (i == 0) GemID1 = _GemIDs[i];
                if (i == 1) GemID2 = _GemIDs[i];
                if (i == 2) GemID3 = _GemIDs[i];
                if (i == 3) GemID4 = _GemIDs[i];
            }
        }
        public bool IsNull()
        {
            return GemID1 <= 0 && GemID2 <= 0 && GemID3 <= 0 && GemID4 <= 0;
        }
    }
    public struct SQLPlayerGearGems
    {
        public Dictionary<ItemSlot, SQLGemInfo> Gems;
        public static SQLPlayerGearGems CreateEmpty()
        {
            SQLPlayerGearGems value = new SQLPlayerGearGems();
            value.Gems = new Dictionary<ItemSlot, SQLGemInfo>();
            return value;
        }
    }
    public struct SQLIngameItemInfo
    {
        public int ItemID;
        public int EnchantID;
        public int SuffixID;
        public int UniqueID;

        public SQLIngameItemInfo(PlayerData.ItemInfo _ItemInfo)
        {
            ItemID = _ItemInfo.ItemID;
            EnchantID = _ItemInfo.EnchantID;
            SuffixID = _ItemInfo.SuffixID;
            UniqueID = _ItemInfo.UniqueID;
        }
        public PlayerData.ItemInfo GenerateItemInfo(ItemSlot _Slot, int[] _GemIDs = null)
        {
            PlayerData.ItemInfo itemInfo = new PlayerData.ItemInfo();
            itemInfo.Slot = _Slot;
            itemInfo.ItemID = ItemID;
            itemInfo.EnchantID = EnchantID;
            itemInfo.SuffixID = SuffixID;
            itemInfo.UniqueID = UniqueID;
            itemInfo.GemIDs = _GemIDs;
            return itemInfo;
        }
        public PlayerData.ItemInfo GenerateItemInfo(ItemSlot _Slot, SQLGemInfo _GemInfo)
        {
            int[] gemIDs = new int[4];
            gemIDs[0] = _GemInfo.GemID1;
            gemIDs[1] = _GemInfo.GemID2;
            gemIDs[2] = _GemInfo.GemID3;
            gemIDs[3] = _GemInfo.GemID4;

            return GenerateItemInfo(_Slot, gemIDs);
        }
    }
    public partial class SQLComm
    {
        public bool GetPlayerGearItems(SQLPlayerData _PlayerData, out SQLPlayerGearItems _ResultGearItems)
        {
            var conn = GetConnection();
            OpenConnection();
            try
            {
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
                        idParam.Value = (int)_PlayerData.PlayerGearID;
                        cmd.Parameters.Add(idParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultGearItems = SQLPlayerGearItems.CreateEmpty();
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
            finally
            {
                CloseConnection();
            }
            _ResultGearItems = SQLPlayerGearItems.CreateEmpty();
            return false;
        }
        public bool GetPlayerGearGems(SQLPlayerData _PlayerData, out SQLPlayerGearGems _ResultGearGems)
        {
            var conn = GetConnection();
            OpenConnection();
            try
            {
                const int ITEMSLOT_COLUMN = 0;
                const int GEMID1_COLUMN = 1;
                const int GEMID2_COLUMN = 2;
                const int GEMID3_COLUMN = 3;
                const int GEMID4_COLUMN = 4;
                using (var cmd = new NpgsqlCommand("SELECT itemslot, gemid1, gemid2, gemid3, gemid4 FROM PlayerGearGemsTable WHERE gearid=:ID", conn))
                {
                    {
                        var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                        idParam.Value = (int)_PlayerData.PlayerGearID;
                        cmd.Parameters.Add(idParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == true)
                        {
                            _ResultGearGems = SQLPlayerGearGems.CreateEmpty();
                            while (reader.Read() == true)
                            {
                                SQLGemInfo gemInfo = new SQLGemInfo();
                                ItemSlot itemSlot = (ItemSlot)reader.GetInt16(ITEMSLOT_COLUMN);
                                gemInfo.GemID1 = reader.GetInt32(GEMID1_COLUMN);
                                gemInfo.GemID2 = reader.GetInt32(GEMID2_COLUMN);
                                gemInfo.GemID3 = reader.GetInt32(GEMID3_COLUMN);
                                gemInfo.GemID4 = reader.GetInt32(GEMID4_COLUMN);
                                _ResultGearGems.Gems.Add(itemSlot, gemInfo);
                            }
                            return true;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            _ResultGearGems = SQLPlayerGearGems.CreateEmpty();
            return false;
        }
        public bool GetIngameItems(IEnumerable<SQLIngameItemID> _IngameItems, out Dictionary<SQLIngameItemID, SQLIngameItemInfo> _ResultItems)
        {
            var distinctItems = _IngameItems.Distinct();
            int[] itemsArray = new int[distinctItems.Count()];
            int itemsCounter = 0;
            foreach (var item in distinctItems)
            {
                if (item.ID > 0)
                {
                    itemsArray[itemsCounter++] = item.ID;
                }
            }
            if (itemsCounter != itemsArray.Length)
            {
                int[] oldItemsArray = itemsArray;
                itemsArray = new int[itemsCounter];
                for (int i = 0; i < itemsCounter; ++i)
                {
                    itemsArray[i] = oldItemsArray[i];
                }
            }
            var conn = GetConnection();
            OpenConnection();
            try
            {
                const int ID_COLUMN = 0;
                const int ITEMID_COLUMN = 1;
                const int ENCHANTID_COLUMN = 2;
                const int SUFFIXID_COLUMN = 3;
                const int UNIQUEID_COLUMN = 4;
                using (var cmd = new NpgsqlCommand("SELECT id, itemid, enchantid, suffixid, uniqueid FROM IngameItemTable WHERE id = ANY(:IDs)", conn))
                {
                    {
                        var idArrayParam = new NpgsqlParameter("IDs", NpgsqlDbType.Array | NpgsqlDbType.Integer);
                        idArrayParam.Value = itemsArray;
                        cmd.Parameters.Add(idArrayParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == true)
                        {
                            _ResultItems = new Dictionary<SQLIngameItemID, SQLIngameItemInfo>();
                            while (reader.Read())
                            {
                                SQLIngameItemID sqlID = new SQLIngameItemID(reader.GetInt32(ID_COLUMN));
                                SQLIngameItemInfo itemInfo = new SQLIngameItemInfo();
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
            finally
            {
                CloseConnection();
            }
            _ResultItems = null;
            return false;
        }
    }
}
