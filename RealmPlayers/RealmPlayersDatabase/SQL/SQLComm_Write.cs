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
        public SQLUploadID GenerateNewUploadEntry(Contributor _Contributor)
        {
            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO uploadtable(id, uploadtime, contributor) VALUES (DEFAULT, :UploadTime, :Contributor) RETURNING id", conn))
                {
                    {
                        var uploadTimeParam = new NpgsqlParameter("UploadTime", NpgsqlDbType.Timestamp);
                        uploadTimeParam.Value = DateTime.UtcNow;
                        cmd.Parameters.Add(uploadTimeParam);
                    }
                    {
                        var contributorParam = new NpgsqlParameter("Contributor", NpgsqlDbType.Integer);
                        contributorParam.Value = _Contributor.GetContributorID();
                        cmd.Parameters.Add(contributorParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        return new SQLUploadID(reader.GetInt32(0));
                    }
                }
            }
        }
    }
}
