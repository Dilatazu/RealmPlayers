using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using Old_RaidCollection = VF_RaidDamageDatabase.RaidCollection;
using Old_RaidCollection_Raid = VF_RaidDamageDatabase.RaidCollection_Raid;
using Old_RaidBossFight = VF_RaidDamageDatabase.RaidBossFight;

namespace VF_RDDatabase
{
    [ProtoContract]
    public class Raid
    {
        [ProtoMember(1)]
        private int m_UniqueRaidID = -1;
        [ProtoMember(2)]
        private int m_RaidID = -1;
        [ProtoMember(3)]
        private DateTime m_RaidResetDateTime = DateTime.MinValue;
        [ProtoMember(4)]
        private string m_RaidInstance = "Unknown";
        [ProtoMember(5)]
        private DateTime m_RaidStartDate = DateTime.MaxValue;
        [ProtoMember(6)]
        private DateTime m_RaidEndDate = DateTime.MinValue;
        [ProtoMember(7)]
        public List<BossFight> m_BossFights = new List<BossFight>();
        [ProtoMember(8)]
        public List<string> m_RaidMembers = new List<string>();

        public Raid() { }
        public Raid(Old_RaidCollection_Raid _Raid)
        {
            m_UniqueRaidID = _Raid.UniqueRaidID;
            m_RaidID = _Raid.RaidID;
            m_RaidResetDateTime = _Raid.RaidResetDateTime;
            m_RaidInstance = _Raid.RaidInstance;
            m_RaidStartDate = _Raid.RaidStartDate;
            m_RaidEndDate = _Raid.RaidEndDate;
        }
        public void Update(Old_RaidCollection_Raid _Raid, List<Old_RaidBossFight> _RaidBossFights)
        {
            if (m_UniqueRaidID != _Raid.UniqueRaidID 
                || m_RaidID != _Raid.RaidID 
                || m_RaidResetDateTime != _Raid.RaidResetDateTime)
                throw new Exception("Update(RaidCollection_Raid _Raid): _Raid did not match!");

            if(_Raid.RaidStartDate < m_RaidStartDate)
                m_RaidStartDate = _Raid.RaidStartDate;
            if (_Raid.RaidEndDate > m_RaidEndDate)
                m_RaidEndDate = _Raid.RaidEndDate;
            
            foreach (var bossFight in _RaidBossFights)
            {
                try
                {
                    m_RaidMembers.AddRangeUnique(bossFight.GetFightCacheData().m_FightDataCollection.m_RaidMembers);
                }
                catch (Exception)
                {}
            }

            foreach (var bossFight in _RaidBossFights)
            {
                if (bossFight.GetBossName() == "Trash")
                    continue;//Skip trash for obvious reasons
                try
                {
                    BossFight newBossFight = new BossFight(new Old_RaidBossFight[] { bossFight }, m_RaidMembers);
                    if (newBossFight == null)
                        throw new Exception("Crazy BossFight Crash Exception");
                    newBossFight.InitCache(this);
                    
                    int overlappingFightIndex = m_BossFights.FindIndex((_Value) => _Value.IsOverlapping(newBossFight));
                    if (overlappingFightIndex != -1)
                    {
                        //Overlapping fights
                        if (newBossFight.IsBetterVersionOf(m_BossFights[overlappingFightIndex]) == true)
                        {
                            m_BossFights[overlappingFightIndex] = newBossFight;
                        }
                    }
                    else
                    {
                        m_BossFights.Add(newBossFight);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("Player:") && ex.Message.EndsWith("was not found in unitDatas"))
                    {
                        VF_RaidDamageDatabase.Logger.ConsoleWriteLine("Exception in RaidID: " + m_RaidID + ", BossFight: " + bossFight.GetBossName() + ", Message: \"" + ex.Message + "\"", ConsoleColor.Red);
                    }
                    else
                        VF_RaidDamageDatabase.Logger.LogException(ex);
                }
            }
        }

        public int UniqueRaidID
        {
            get { return m_UniqueRaidID; }
        }
        public int RaidID
        {
            get { return m_RaidID; }
        }
        public DateTime RaidResetDateTime
        {
            get { return m_RaidResetDateTime; }
        }
        public string RaidInstance
        {
            get { return m_RaidInstance; }
        }
        public DateTime RaidStartDate
        {
            get { return m_RaidStartDate; }
        }
        public DateTime RaidEndDate
        {
            get { return m_RaidEndDate; }
        }
        public List<BossFight> BossFights
        {
            get { return m_BossFights; }
        }

#region Cache Data
        private GroupRaidCollection m_CacheGroup = null;

        public GroupRaidCollection CacheGroup
        {
            get { return m_CacheGroup; }
        }
        //[ProtoAfterDeserialization]
        private void InitChildCache()
        {
            foreach (var bossFights in m_BossFights)
            {
                bossFights.InitCache(this);
            }
        }
        public void InitCache(GroupRaidCollection _GroupRaidCollection)
        {
            m_CacheGroup = _GroupRaidCollection;
            InitChildCache();
        }
        public bool CacheInitialized()
        {
            return m_CacheGroup != null;
        }
        public void Dispose()
        {
            m_CacheGroup = null;
            foreach (var bossFights in m_BossFights)
            {
                bossFights.Dispose();
            }
        }
#endregion 
    }
}
