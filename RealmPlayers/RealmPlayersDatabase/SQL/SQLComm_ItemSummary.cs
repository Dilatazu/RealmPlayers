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


namespace VF
{
    public partial class SQLComm 
    {
        public PlayerData.ItemInfo GetLatestItemInfoForPlayer(string _Player, WowRealm _Realm, int _ItemID)
        {
            PlayerData.ItemInfo latestItemInfo = new PlayerData.ItemInfo();
            latestItemInfo.Slot = VF_RealmPlayersDatabase.ItemSlot.Unknown;
            latestItemInfo.ItemID = _ItemID;
            DateTime latestItemInfoDate = DateTime.MinValue;

            var conn = GetConnection();
            conn.Open();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT DISTINCT ii.enchantid, ii.suffixid, ii.uniqueid, MAX(pdt.updatetime) FROM PlayerTable player"
                + " INNER JOIN PlayerDataTable pdt ON pdt.PlayerID = player.ID"
                + " INNER JOIN PlayerGearTable pgt ON pgt.ID = pdt.gearinfo INNER JOIN IngameItemTable ii"
                + " ON ii.ID = pgt.head OR ii.ID = pgt.neck OR ii.ID = pgt.shoulder OR ii.ID = pgt.shirt"
                + " OR ii.ID = pgt.chest OR ii.ID = pgt.belt OR ii.ID = pgt.legs OR ii.ID = pgt.feet"
                + " OR ii.ID = pgt.wrist OR ii.ID = pgt.gloves OR ii.ID = pgt.finger_1 OR ii.ID = pgt.finger_2"
                + " OR ii.ID = pgt.trinket_1 OR ii.ID = pgt.trinket_2 OR ii.ID = pgt.back"
                + " OR ii.ID = pgt.main_hand OR ii.ID = pgt.off_hand OR ii.ID = pgt.ranged"
                + " OR ii.ID = pgt.tabard WHERE ii.ItemID = :ItemID AND player.name = :PlayerName AND player.realm = :Realm GROUP BY ii.enchantid, ii.suffixid, ii.uniqueid", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("ItemID", NpgsqlDbType.Integer)).Value = _ItemID;
                    cmd.Parameters.Add(new NpgsqlParameter("PlayerName", NpgsqlDbType.Text)).Value = _Player;
                    cmd.Parameters.Add(new NpgsqlParameter("Realm", NpgsqlDbType.Integer)).Value = (int)_Realm;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read() == true && reader.IsDBNull(3) == false)
                        {
                            DateTime currItemInfoDate = reader.GetTimeStamp(3).DateTime;
                            if (currItemInfoDate > latestItemInfoDate)
                            {
                                latestItemInfoDate = currItemInfoDate;
                                latestItemInfo.EnchantID = reader.GetInt32(0);
                                latestItemInfo.SuffixID = reader.GetInt32(1);
                                latestItemInfo.UniqueID = reader.GetInt32(2);
                            }
                        }
                    }
                }
            }
            finally
            {
                conn.Close();
            }

            if (latestItemInfoDate != DateTime.MinValue)
            {
                return latestItemInfo;
            }

            return null;
        }

        public bool UpsertItemOwner(int _ItemID, int _SuffixID, VF.SQLPlayerID _PlayerID, VF.SQLIngameItemID? _LatestItemInfo = null, DateTime? _DateAquired = null)
        {
            if (_LatestItemInfo.HasValue == false) _LatestItemInfo = new SQLIngameItemID(0);
            if (_DateAquired.HasValue == false) _DateAquired = DateTime.MaxValue;

            var conn = GetConnection();
            conn.Open();
            try
            {
                using (var cmd = new NpgsqlCommand("INSERT INTO ItemOwnerTable(ItemID, SuffixID, PlayerID, DateAquired, LatestItemInfo) VALUES(:ItemID, :SuffixID, :PlayerID, :DateAquired, :LatestItemInfo) ON CONFLICT ON CONSTRAINT itemownertable_pkey DO UPDATE SET DateAquired = EXCLUDED.DateAquired, LatestItemInfo  = EXCLUDED.LatestItemInfo", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("ItemID", NpgsqlDbType.Integer)).Value = _ItemID;
                    cmd.Parameters.Add(new NpgsqlParameter("SuffixID", NpgsqlDbType.Integer)).Value = _SuffixID;
                    cmd.Parameters.Add(new NpgsqlParameter("PlayerID", NpgsqlDbType.Integer)).Value = (int)_PlayerID;
                    cmd.Parameters.Add(new NpgsqlParameter("DateAquired", NpgsqlDbType.Timestamp)).Value = _DateAquired.Value;
                    cmd.Parameters.Add(new NpgsqlParameter("LatestItemInfo", NpgsqlDbType.Integer)).Value = (int)_LatestItemInfo.Value;

                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows == 1)
                        return true;
                }
            }
            finally
            {
                conn.Close();
            }
            return false;
        }
    }
}
