using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;

using Old_RaidCollection = VF_RaidDamageDatabase.RaidCollection;
using Old_FightDataCollection = VF_RaidDamageDatabase.FightDataCollection;

using Utility = VF_RaidDamageDatabase.Utility;

using BossInformation = VF_RaidDamageDatabase.BossInformation;

namespace VF_RDDatabase
{
    public struct BossKillInfo
    {
        public DateTime KillStartDateTime = DateTime.MinValue;
        public DateTime KillEndDateTime = DateTime.MinValue;
        public bool IsOverlapping(BossKillInfo _KillInfo)
        {
            if ((_KillInfo.KillStartDateTime > KillStartDateTime
                    && _KillInfo.KillStartDateTime < KillEndDateTime)
                || (KillStartDateTime > _KillInfo.KillStartDateTime
                    && KillStartDateTime < _KillInfo.KillEndDateTime))
                return false;
            return true;
        }
    }
    public struct InstanceClearInfo
    {
        public DateTime ClearStartDateTime = DateTime.MinValue;
        public DateTime ClearEndDateTime = DateTime.MinValue;
        public bool IsOverlapping(InstanceClearInfo _ClearInfo)
        {
            if ((_ClearInfo.ClearStartDateTime > ClearStartDateTime 
                    && _ClearInfo.ClearStartDateTime < ClearEndDateTime)
                || (ClearStartDateTime > _ClearInfo.ClearStartDateTime
                    && ClearStartDateTime < _ClearInfo.ClearEndDateTime))
                return false;
            return true;
        }
    }
    public class GroupInfo
    {
        public VF_RealmPlayersDatabase.WowRealm Realm;
        public string GroupName;
        public Dictionary<string, List<BossKillInfo>> FastestBossKills;
        public Dictionary<string, List<InstanceClearInfo>> FastestInstanceClears;
        public void Update(GroupRaidCollection _GroupRC)
        {
            foreach(var raid in _GroupRC.Raids)
            {
                var instanceRuns = BossInformation.InstanceRuns[raid.Value.RaidInstance];
                foreach(var instanceRun in instanceRuns)
                {
                    var instanceClearData = VF_RaidDamageDatabase.RaidInstanceClearData.Generate(raid, instanceRun.Value);
                    if(instanceClearData != null)
                    {
                        InstanceClearInfo instanceClearInfo = new InstanceClearInfo {
                                ClearStartDateTime = instanceClearData.m_RaidStartClearTime,
                                ClearEndDateTime = instanceClearData.m_RaidEndClearTime
                        };
                        bool foundDuplicate = false;
                        if(FastestInstanceClears.ContainsKey(instanceRun.Key) == true)
                        {
                            var fastestClears = FastestInstanceClears[instanceRun.Key];
                            for(int i = 0; i < fastestClears.Count; ++i)
                            {
                                if (fastestClears[i].IsOverlapping(instanceClearInfo) == true)
                                {
                                    fastestClears[i] = instanceClearInfo;
                                    foundDuplicate = true;
                                }
                            }
                        }
                        if (foundDuplicate == false)
                        {
                            FastestInstanceClears.AddToList(instanceRun.Key, instanceClearInfo);
                        }
                    }
                }
            }
        }
    }
    [ProtoContract]
    public class GroupSummaryDatabase
    {
        [ProtoMember(1)]
        private Dictionary<string, GroupInfo> m_GroupInfos = new Dictionary<string, GroupInfo>();

        public Dictionary<string, GroupInfo> GroupInfos
        {
            get { return m_GroupInfos; }
        }

        public GroupInfo GetGroupInfo(WowRealm _Realm, string _GroupName)
        {
            GroupInfo retValue = null;
            if (m_GroupInfos.TryGetValue("" + (int)_Realm + _GroupName, out retValue) == false)
                return null;

            return retValue;
        }
        private void AddGroupInfo(GroupInfo _GroupInfo)
        {
            m_GroupInfos.Add("" + (int)_GroupInfo.Realm + _GroupInfo.GroupName, _GroupInfo);
        }
        public void UpdateDatabase(SummaryDatabase _SummaryDatabase)
        {
            foreach(var groupRC in _SummaryDatabase.GroupRCs)
            {
                GroupInfo groupInfo = GetGroupInfo(groupRC.Value.Realm, groupRC.Value.GroupName);
                if(groupInfo == null)
                {
                    groupInfo = new GroupInfo();
                    groupInfo.Realm = groupRC.Value.Realm;
                    groupInfo.GroupName = groupRC.Value.GroupName;
                    AddGroupInfo(groupInfo);
                }

                groupInfo.Update(groupRC.Value);
            }
        }
        public static GroupSummaryDatabase GenerateSummaryDatabase(SummaryDatabase _SummaryDatabase)
        {
            GroupSummaryDatabase newDatabase = new GroupSummaryDatabase();
            newDatabase.UpdateDatabase(_SummaryDatabase);
            return newDatabase;
        }
        public static GroupSummaryDatabase LoadSummaryDatabase(string _RootDirectory)
        {
            GroupSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\GroupSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (VF.Utility.LoadSerialize(databaseFile, out database) == false)
                    database = null;
            }
            return database;
        }
    }
}
