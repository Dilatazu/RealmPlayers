using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlayerData = VF_RealmPlayersDatabase.PlayerData;
using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;

namespace VF
{
    public struct SQLPlayerID
    {
        public int ID;
        public SQLPlayerID(int _ID)
        {
            ID = _ID;
        }
        public static SQLPlayerID Invalid()
        {
            return new SQLPlayerID(-1);
        }
    }
    public struct SQLUploadID
    {
        public int ID;
        public SQLUploadID(int _ID)
        {
            ID = _ID;
        }
        public static SQLUploadID Invalid()
        {
            return new SQLUploadID(-1);
        }
    }
    public struct SQLIngameItemID
    {
        public int ID;
        public SQLIngameItemID(int _ID)
        {
            ID = _ID;
        }
        public static SQLIngameItemID Invalid()
        {
            return new SQLIngameItemID(-1);
        }
    }
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
    public struct SQLGearItems
    {
        public Dictionary<ItemSlot, SQLIngameItemID> Items;
        public static SQLGearItems CreateEmpty()
        {
            SQLGearItems value = new SQLGearItems();
            value.Items = new Dictionary<ItemSlot, SQLIngameItemID>();
            return value;
        }
    }
}
