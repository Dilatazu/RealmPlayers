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
    }
}
