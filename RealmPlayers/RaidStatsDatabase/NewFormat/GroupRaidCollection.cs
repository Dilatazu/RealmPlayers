using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using Old_RaidCollection = VF_RaidDamageDatabase.RaidCollection;
using Old_FightDataCollection = VF_RaidDamageDatabase.FightDataCollection;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using RealmDB = VF_RaidDamageDatabase.RealmDB;

using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;

namespace VF_RDDatabase
{
    [ProtoContract]
    public class GroupRaidCollection
    {
        [ProtoMember(1)]
        public string m_GroupName;
        [ProtoMember(2)]
        private Dictionary<int, Raid> m_Raids = new Dictionary<int, Raid>();

        [ProtoMember(3)]
        public VF_RealmPlayersDatabase.WowRealm m_Realm = VF_RealmPlayersDatabase.WowRealm.Unknown;


        //public List<PlayerSummary> m_PlayerSummarys = new List<PlayerSummary>();
        //public Dictionary<string, BossSummary> m_BossSummarys = new Dictionary<string, BossSummary>();


        public string GroupName
        {
            get { return m_GroupName; }
        }
        public Dictionary<int, Raid> Raids
        {
            get { return m_Raids; }
        }
        public VF_RealmPlayersDatabase.WowRealm Realm
        {
            get { return m_Realm; }
        }
        public Raid GetRaid(int _UniqueRaidID)
        {
            Raid raid = null;
            if (m_Raids.TryGetValue(_UniqueRaidID, out raid) == false)
                return null;
            return raid;
        }
        public void InitCache()
        {
            foreach (var raid in m_Raids)
            {
                raid.Value.InitCache(this);
            }
        }
        public void Dispose()
        {
            foreach (var raid in m_Raids)
            {
                raid.Value.Dispose();
            }
        }

        public PlayerFaction GetFaction(Func<WowRealm, RealmDB> _GetRealmDB)
        {
            return GetFaction(_GetRealmDB(m_Realm));
        }
        public PlayerFaction GetFaction(RealmDB _RealmDB)
        {
            int hordeCount = 0;
            int allianceCount = 0;
            var playersFightData = m_Raids.Last().Value.BossFights.Last().PlayerFightData;
            foreach (var playerData in playersFightData)
            {
                var player = _RealmDB.FindPlayer(playerData.Item1);
                if (player != null)
                {
                    PlayerFaction playerFaction = VF_RealmPlayersDatabase.StaticValues.GetFaction(player.Character.Race);
                    if (playerFaction == PlayerFaction.Horde)
                        ++hordeCount;
                    else if (playerFaction == PlayerFaction.Alliance)
                        ++allianceCount;

                    if (hordeCount - allianceCount > 10)
                        return PlayerFaction.Horde;
                    else if (allianceCount - hordeCount > 10)
                        return PlayerFaction.Alliance;
                }
            }
            if (hordeCount > allianceCount)
                return PlayerFaction.Horde;
            else
                return PlayerFaction.Alliance;
        }


        public void GenerateSummary_ReplaceRaid(Old_RaidCollection.Raid _Raid)
        {
            Raid raid = null;
            if (m_Raids.TryGetValue(_Raid.UniqueRaidID, out raid) == true)
            {
                raid.Dispose();
                raid = new Raid(_Raid);
                m_Raids[_Raid.UniqueRaidID] = raid;
                raid.InitCache(this);
            }
            else//if (m_Raids.TryGetValue(_Raid.UniqueRaidID, out raid) == false)
            {
                Raid newRaid = new Raid(_Raid);
                newRaid.InitCache(this);
                m_Raids.Add(newRaid.UniqueRaidID, newRaid);
                raid = newRaid;
            }
            try
            {
                var bossFights = _Raid.GetAllBossFights(Hidden._GlobalInitializationData.CachedGetFightDataCollectionFunc);
                raid.Update(_Raid, bossFights);
            }
            catch (Exception ex)
            {
                VF_RaidDamageDatabase.Logger.LogException(ex);
            }
        }
        public void GenerateSummary_AddRaid(Old_RaidCollection.Raid _Raid)
        {
            Raid raid = null;
            if (m_Raids.TryGetValue(_Raid.UniqueRaidID, out raid) == false)
            {
                Raid newRaid = new Raid(_Raid);
                newRaid.InitCache(this);
                m_Raids.Add(newRaid.UniqueRaidID, newRaid);
                raid = newRaid;
            }
            try
            {
                var bossFights = _Raid.GetAllBossFights(Hidden._GlobalInitializationData.CachedGetFightDataCollectionFunc);
                raid.Update(_Raid, bossFights);
            }
            catch (Exception ex)
            {
                VF_RaidDamageDatabase.Logger.LogException(ex);
            }
        }

        ~GroupRaidCollection()
        {
            Dispose();
        }
    }
}
