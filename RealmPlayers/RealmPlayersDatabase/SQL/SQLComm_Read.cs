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
        public int GetInspectsForContributor(Contributor _Contributor)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM uploadtable up INNER JOIN playerdatatable pd ON up.id = pd.uploadid WHERE up.contributor = :ContributorID", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("ContributorID", NpgsqlDbType.Integer)).Value = _Contributor.ContributorID;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            return -1;
        }
        public int GetRealmInspectsForContributor(Contributor _Contributor, WowRealm _Realm)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM uploadtable up INNER JOIN playerdatatable pd ON up.id = pd.uploadid INNER JOIN playertable player ON player.id = pd.playerid WHERE up.contributor = :ContributorID AND player.realm = :Realm", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("ContributorID", NpgsqlDbType.Integer)).Value = _Contributor.ContributorID;
                    cmd.Parameters.Add(new NpgsqlParameter("Realm", NpgsqlDbType.Integer)).Value = (int)_Realm;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            return -1;
        }
        public DateTime GetEarliestInspectForContributor(Contributor _Contributor)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT MIN(pd.updatetime) FROM uploadtable up INNER JOIN playerdatatable pd ON up.id = pd.uploadid WHERE up.contributor = :ContributorID", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("ContributorID", NpgsqlDbType.Integer)).Value = _Contributor.ContributorID;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetTimeStamp(0).DateTime;
                        }
                    }
                }
            }
            return DateTime.MaxValue;
        }
        public DateTime GetLatestInspectForContributor(Contributor _Contributor)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT MAX(pd.updatetime) FROM uploadtable up INNER JOIN playerdatatable pd ON up.id = pd.uploadid WHERE up.contributor = :ContributorID", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("ContributorID", NpgsqlDbType.Integer)).Value = _Contributor.ContributorID;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetTimeStamp(0).DateTime;
                        }
                    }
                }
            }
            return DateTime.MinValue;
        }
        public int GetRealmInspectsTotal(WowRealm _Realm)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM playerdatatable pd INNER JOIN playertable player ON player.id = pd.playerid WHERE player.realm = :Realm", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("Realm", NpgsqlDbType.Integer)).Value = (int)_Realm;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            return -1;
        }
        public int GetInspectsTotal()
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM playerdatatable", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            return -1;
        }
    }
}
