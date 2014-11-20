using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;

namespace VF_RaidDamageDatabase
{
    public class RealmDB
    {
        public VF_RealmPlayersDatabase.RealmDatabase m_RealmDB;
        public VF_RealmPlayersDatabase.WowRealm Realm
        {
            get
            {
                return m_RealmDB.Realm;
            }
        }
        public RealmDB(VF_RealmPlayersDatabase.RealmDatabase _RealmDB)
        {
            m_RealmDB = _RealmDB;
        }
        public Func<string, bool> RD_IsPlayerFunc(RaidBossFight _RaidBossFight)
        {
            return (_Value) => RD_IsPlayer(_Value, _RaidBossFight.GetFightCacheData().m_FightDataCollection.m_RaidMembers);
        }
        public bool RD_IsPlayer(string _Name, RaidBossFight _RaidBossFight)
        {
            return RD_IsPlayer(_Name, _RaidBossFight.GetFightCacheData().m_FightDataCollection.m_RaidMembers);
        }
        public bool RD_IsPlayer(string _Name, List<string> _RaidMembers)
        {
            if (_RaidMembers.Count != 0 && _RaidMembers.Contains(_Name) == false)
                return false;
            if (_Name == "Unknown")
                return false;
            if (BossInformation.BossFights.ContainsKey(_Name) == true)
                return false;
            return m_RealmDB.PlayerExist(_Name);
        }
        public Func<string, RaidBossFight.PlayerIdentifier> RD_GetPlayerIdentifierFunc(RaidBossFight _RaidBossFight)
        {
            return (_Value) => RD_GetPlayerIdentifier(_Value, _RaidBossFight.GetFightCacheData().m_FightDataCollection.m_RaidMembers);
        }
        public RaidBossFight.PlayerIdentifier RD_GetPlayerIdentifier(string _Name, RaidBossFight _RaidBossFight)
        {
            return RD_GetPlayerIdentifier(_Name, _RaidBossFight.GetFightCacheData().m_FightDataCollection.m_RaidMembers);
        }
        public RaidBossFight.PlayerIdentifier RD_GetPlayerIdentifier(string _Name, VF_RDDatabase.BossFight _BossFight)
        {
            return RD_GetPlayerIdentifier(_Name, _BossFight.CacheRaid.m_RaidMembers);
        }
        public RaidBossFight.PlayerIdentifier RD_GetPlayerIdentifier(string _Name, List<string> _RaidMembers)
        {
            if (_Name == "Unknown")
                return RaidBossFight.PlayerIdentifier.NotPlayer;
            if (BossInformation.BossFights.ContainsKey(_Name) == true)
                return RaidBossFight.PlayerIdentifier.NotPlayer;
            if (_RaidMembers.Count != 0 && _RaidMembers.Contains(_Name) == false)
                return RaidBossFight.PlayerIdentifier.NotPlayer;

            var player = m_RealmDB.FindPlayer(_Name);
            if (player == null)
                return RaidBossFight.PlayerIdentifier.NotPlayer;

            RaidBossFight.PlayerIdentifier retValue = RaidBossFight.PlayerIdentifier.Player;

            var playerClass = player.Character.Class;
            if (playerClass == PlayerClass.Mage || playerClass == PlayerClass.Warlock)
            {
                retValue = retValue | RaidBossFight.PlayerIdentifier.AOEClass;
                retValue = retValue | RaidBossFight.PlayerIdentifier.SpellClass;
            }
            if (playerClass == PlayerClass.Druid || playerClass == PlayerClass.Priest)
            {
                retValue = retValue | RaidBossFight.PlayerIdentifier.SpellClass;
            }
            if (playerClass == PlayerClass.Druid || playerClass == PlayerClass.Priest
                || playerClass == PlayerClass.Shaman || playerClass == PlayerClass.Paladin)
            {
                retValue = retValue | RaidBossFight.PlayerIdentifier.HealClass;
            }
            if (playerClass == PlayerClass.Hunter)
            {
                retValue = retValue | RaidBossFight.PlayerIdentifier.PetClass;
            }

            return retValue;
        }
        public Func<string, VF_RealmPlayersDatabase.PlayerData.Player> RD_FindPlayerFunc(RaidBossFight _RaidBossFight)
        {
            return (_Value) => RD_FindPlayer(_Value, _RaidBossFight.GetFightCacheData().m_FightDataCollection.m_RaidMembers);
        }
        public VF_RealmPlayersDatabase.PlayerData.Player RD_FindPlayer(string _Name, RaidBossFight _RaidBossFight)
        {
            return RD_FindPlayer(_Name, _RaidBossFight.GetFightCacheData().m_FightDataCollection.m_RaidMembers);
        }
        public VF_RealmPlayersDatabase.PlayerData.Player RD_FindPlayer(string _Name, VF_RDDatabase.BossFight _BossFight)
        {
            return RD_FindPlayer(_Name, _BossFight.CacheRaid.m_RaidMembers);
        }
        public VF_RealmPlayersDatabase.PlayerData.Player RD_FindPlayer(string _Name, List<string> _RaidMembers)
        {
            if (_RaidMembers.Count != 0 && _RaidMembers.Contains(_Name) == false)
                return null;
            if (_Name == "Unknown")
                return null;
            if (BossInformation.BossFights.ContainsKey(_Name) == true)
                return null;
            return m_RealmDB.FindPlayer(_Name);
        }
        public VF_RealmPlayersDatabase.PlayerData.Player FindPlayer(string _Name)
        {
            return m_RealmDB.FindPlayer(_Name);
        }
        public VF_RealmPlayersDatabase.PlayerData.Player GetPlayer(string _Name)
        {
            return m_RealmDB.GetPlayer(_Name);
        }
    }
    public class RPPDatabase
    {
        private static ThreadSafeCache sm_RealmDBCache = new ThreadSafeCache();
        private static VF_RealmPlayersDatabase.Database _NotCached_GetDatabase(string _DatabaseDirectory)
        {
            VF_RealmPlayersDatabase.Database rppDatabase = new VF_RealmPlayersDatabase.Database(_DatabaseDirectory);
            rppDatabase.PurgeRealmDBs(true, false);
            return rppDatabase;
        }
        private static VF_RealmPlayersDatabase.Database GetDatabase(string _DatabaseDirectory)// = "C:\\VF_RealmPlayersData\\RPPDatabase\\Database\\")
        {
            return sm_RealmDBCache.Get("GetDatabase", _NotCached_GetDatabase, _DatabaseDirectory);
        }

        VF_RealmPlayersDatabase.Database m_RPPDatabase;
        public RPPDatabase(string _DatabaseDirectory)
        {
            m_RPPDatabase = GetDatabase(_DatabaseDirectory);
        }
        public RealmDB GetRealmDB(VF_RealmPlayersDatabase.WowRealm _Realm)
        {
            return new RealmDB(m_RPPDatabase.GetRealm(_Realm));
        }
    }
}
