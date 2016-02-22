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
    public partial class SQLComm : IDisposable
    {
        public static string g_ConnectionString = "Host=localhost;Port=5433;Username=RealmPlayers;Password=" + VF.HiddenStrings.SQLDatabase_Password + ";Database=RealmPlayersDB";

        NpgsqlConnection m_Connection = null;

        public NpgsqlConnection GetConnection()
        {
            if (m_Connection == null)
            {
                m_Connection = new NpgsqlConnection(g_ConnectionString);
                m_Connection.Open();
            }
            return m_Connection;
        }
        public void Dispose()
        {
            if(m_Connection != null)
            {
                m_Connection.Dispose();
                m_Connection = null;
            }
        }
    }
}
