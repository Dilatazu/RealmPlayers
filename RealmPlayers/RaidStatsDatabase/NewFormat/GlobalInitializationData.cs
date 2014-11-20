using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Old_FightDataCollection = VF_RaidDamageDatabase.FightDataCollection;

namespace VF_RDDatabase.Hidden
{
    class _GlobalInitializationData
    {
        public static Func<VF_RealmPlayersDatabase.WowRealm, VF_RaidDamageDatabase.RealmDB> GetRealmDBFunc = null;
        public static Func<string, Old_FightDataCollection> CachedGetFightDataCollectionFunc = null;

        public static void Init(Func<VF_RealmPlayersDatabase.WowRealm, VF_RaidDamageDatabase.RealmDB> _GetRealmDBFunc, Func<string, Old_FightDataCollection> _CachedGetFightDataCollectionFunc)
        {
            GetRealmDBFunc = _GetRealmDBFunc;
            CachedGetFightDataCollectionFunc = _CachedGetFightDataCollectionFunc;
        }
        public static void Clear()
        {
            GetRealmDBFunc = null;
            CachedGetFightDataCollectionFunc = null;
        }
    }
}
