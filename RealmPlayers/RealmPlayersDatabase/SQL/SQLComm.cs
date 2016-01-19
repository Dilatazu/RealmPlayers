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
        public static string g_ConnectionString = "Host=localhost;Port=5432;Username=RealmPlayers;Password=" + VF.HiddenStrings.SQLDatabase_Password + ";Database=testdb2";
        
        
    }
}
