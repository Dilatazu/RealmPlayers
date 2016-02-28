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

        //public NpgsqlConnection _GetConnection()
        //{
        //    return m_Connection;
        //}
        public NpgsqlConnection OpenConnection()
        {
            if (m_Connection == null)
            {
                m_Connection = new NpgsqlConnection(g_ConnectionString);
                m_Connection.Open();
            }
            else if (m_Connection != null)
            {
                if (m_Connection.State == System.Data.ConnectionState.Closed
                || m_Connection.State == System.Data.ConnectionState.Broken)
                {
                    try
                    {
                        m_Connection.Open();
                    }
                    catch (Exception ex)
                    { VF_RealmPlayersDatabase.Logger.LogException(ex); }
                }
            }
            return m_Connection;
        }
        public void CloseConnection()
        {
            if(m_Connection != null)
            {
                if (m_Connection.State != System.Data.ConnectionState.Closed)
                {
                    try
                    {
                        m_Connection.Close();
                    }
                    catch (Exception ex)
                    { VF_RealmPlayersDatabase.Logger.LogException(ex); }
                }
            }
        }
        public void Dispose()
        {
            if(m_Connection != null)
            {
                CloseConnection();
                m_Connection.Dispose();
                m_Connection = null;
            }
        }
    }
}
