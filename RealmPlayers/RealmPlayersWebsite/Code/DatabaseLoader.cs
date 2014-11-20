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
        public static DateTime GetLastDatabaseUpdateTimeUTC()
        {
            var lastDatabaseUpdateTime = System.IO.File.GetLastWriteTimeUtc(Constants.RPPDbDir + "Database\\Emerald_Dream\\PlayersData.dat");
            var wsgUpdateTime = System.IO.File.GetLastWriteTimeUtc(Constants.RPPDbDir + "Database\\Warsong\\PlayersData.dat");
            var aaUpdateTime = System.IO.File.GetLastWriteTimeUtc(Constants.RPPDbDir + "Database\\Al_Akir\\PlayersData.dat");
            if (wsgUpdateTime > lastDatabaseUpdateTime)
                lastDatabaseUpdateTime = wsgUpdateTime;
            if (aaUpdateTime > lastDatabaseUpdateTime)
                lastDatabaseUpdateTime = aaUpdateTime;
            return lastDatabaseUpdateTime;
        }
        public static RPPDatabase LoadRPPDatabase(bool _FirstTimeLoading = false)
        {
            GC.Collect();
            RPPDatabase database = new RPPDatabase(Constants.RPPDbDir + "Database\\", new DateTime(2012, 5, 1, 0, 0, 0));

            database.PurgeRealmDBs(true, true, (_FirstTimeLoading == false));
            GC.Collect();
            return database;
        }
    }
}