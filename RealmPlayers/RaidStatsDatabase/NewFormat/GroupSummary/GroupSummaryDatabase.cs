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
                try
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
                catch (Exception ex)
                {
                    VF_RaidDamageDatabase.Logger.LogException(ex);
                }
            }
        }
        public static GroupSummaryDatabase GenerateSummaryDatabase(SummaryDatabase _SummaryDatabase)
        {
            GroupSummaryDatabase newDatabase = new GroupSummaryDatabase();
            newDatabase.UpdateDatabase(_SummaryDatabase);
            return newDatabase;
        }
        public static void UpdateSummaryDatabase(string _RootDirectory, SummaryDatabase _SummaryDatabase)
        {
            GroupSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\GroupSummaryDatabase.dat";
            //if (System.IO.File.Exists(databaseFile) == true)
            //{
            //    if (VF.Utility.LoadSerialize(databaseFile, out database) == false)
            //        database = null;
            //}
            //if (database != null)
            //{
            //    database.UpdateDatabase(_SummaryDatabase);
            //}
            //else
            {
                database = GenerateSummaryDatabase(_SummaryDatabase);
            }
            VF.Utility.SaveSerialize(databaseFile, database);
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
