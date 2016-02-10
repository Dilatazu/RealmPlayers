using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;
using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;

using RPPDatabase = VF_RealmPlayersDatabase.Database;
using RealmDatabase = VF_RealmPlayersDatabase.RealmDatabase;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;

namespace RealmPlayersServer
{
    public class DatabaseLoader
    {
        public static RPPDatabase LoadRPPDatabase(bool _FirstTimeLoading = false)
        {
            GC.Collect();
            RPPDatabase database = new RPPDatabase(Constants.RPPDbDir + "Database\\", new DateTime(2016, 1, 1, 0, 0, 0));

            database.PurgeRealmDBs(true, true, (_FirstTimeLoading == false));
            GC.Collect();
            return database;
        }
        public static bool ReloadRPPDatabase(RPPDatabase _RPPDatabase, object _SynchronizationLockObject)
        {
            GC.Collect();
            bool reloadResult = _RPPDatabase.ReloadAllRealmDBs(Constants.RPPDbDir + "Database\\"
                , true, _SynchronizationLockObject, new DateTime(2016, 1, 1, 0, 0, 0));
            GC.Collect();
            return reloadResult;
        }
    }
}