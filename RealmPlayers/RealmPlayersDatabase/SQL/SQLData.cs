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
        public bool IsValid()
        {
            return ID > 0;
        }
        public static SQLPlayerID Invalid()
        {
            return new SQLPlayerID(-1);
        }
        static public explicit operator int (SQLPlayerID _SQLPlayerID)
        {
            return _SQLPlayerID.ID;
        }
    }
    public struct SQLUploadID
    {
        public int ID;
        public SQLUploadID(int _ID)
        {
            ID = _ID;
        }
        public bool IsValid()
        {
            return ID > 0;
        }
        public static SQLUploadID Invalid()
        {
            return new SQLUploadID(-1);
        }
        static public explicit operator int (SQLUploadID _SQLUploadID)
        {
            return _SQLUploadID.ID;
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
        static public explicit operator int (SQLIngameItemID _IngameItemID)
        {
            return _IngameItemID.ID;
        }
    }
}
