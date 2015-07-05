using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace VF_RaidDamageDatabase
{
    [ProtoContract]
    public class RaidCollection_Dungeon
    {
        [ProtoMember(1)]
        public int m_UniqueDungeonID = -1;
        [ProtoMember(2)]
        public string m_Dungeon = "Unknown";
        [ProtoMember(3)]
        public DateTime m_DungeonStartDate = DateTime.MaxValue;
        [ProtoMember(4)]
        public DateTime m_DungeonEndDate = DateTime.MinValue;
        [ProtoMember(5)]
        public List<string> m_DataFiles = new List<string>();
        [ProtoMember(6)]
        public List<RaidCollection_FightDataFileInfo> m_ExistingFights = new List<RaidCollection_FightDataFileInfo>();
        [ProtoMember(7)]
        public List<string> m_GroupMembers = new List<string>();
        [ProtoMember(8)]
        public VF_RealmPlayersDatabase.WowRealm Realm = VF_RealmPlayersDatabase.WowRealm.Emerald_Dream;

        //Cache based, do not serialize!
        private List<RaidBossFight> m_BossFights = new List<RaidBossFight>();
        private bool m_BossFightsUpToDate = false;
        private List<RaidBossFight> m_TrashFights = new List<RaidBossFight>();
        private bool m_TrashFightsUpToDate = false;

        public RaidCollection_Dungeon()
        { }

        public bool AddDataFile(FightDataCollection.FightCacheData _Fight, string _DataFile)
        {
            RaidCollection_FightDataFileInfo currFightDataFileInfo = null;
            int fightIndex = m_ExistingFights.FindIndex(
                (_Value) => {
                    return (_Value.FightName == _Fight.m_Fight.FightName
                                && (Math.Abs((_Value.FightStartDateTime - _Fight.m_Fight.StartDateTime).TotalSeconds) < 10     // Kan göras bättre, missar för tillfället fallet då en fight duration är innuti en annan 
                                    || Math.Abs((_Value.FightEndDateTime - _Fight.m_Fight.GetEndDateTime()).TotalSeconds) < 10 // eller om dom överlappar varandra pga någon loggat ut innan fighten avslutats osv
                                )
                            ); 
                });
            if (fightIndex == -1)
            {
                Logger.ConsoleWriteLine("Fight: " + _Fight.m_Fight.FightName + " added to DungeonCollection", ConsoleColor.Green);
                currFightDataFileInfo = new RaidCollection_FightDataFileInfo
                {
                    FightStartDateTime = _Fight.m_Fight.StartDateTime,
                    FightName = _Fight.m_Fight.FightName,
                    _FightEndDateTime = _Fight.m_Fight.GetEndDateTime(),
                    AttemptType = _Fight.m_Fight.GetAttemptType()
                };

                m_ExistingFights.Add(currFightDataFileInfo);
            }
            else
            {
                Logger.ConsoleWriteLine("Fight: " + _Fight.m_Fight.FightName + " not added to DungeonCollection, allready exists", ConsoleColor.Red);
                currFightDataFileInfo = m_ExistingFights[fightIndex];
            }

            if(currFightDataFileInfo.IsRecordedBy(_Fight.m_Fight.RecordedByPlayer) == false)
            {
                currFightDataFileInfo.AddRecordedBy(_Fight.m_Fight.RecordedByPlayer, _DataFile);
                if (m_DataFiles.Contains(_DataFile) == false)
                {
                    m_DataFiles.Add(_DataFile);
                    m_BossFightsUpToDate = false;
                }
                return true;
            }
            return false;
        }
        public List<string> GetRecordedByPlayers()
        {
            List<string> retList = new List<string>();
            foreach (var fight in m_ExistingFights)
            {
                retList.AddRange(fight.GetAllRecordedBy());
            }
            return retList.Distinct().ToList();
        }
        public List<string> GetRecordedByPlayers(DateTime _FightStartDateTime)
        {
            foreach (var fight in m_ExistingFights)
            {
                if(fight.FightStartDateTime == _FightStartDateTime)
                     return fight.GetAllRecordedBy();
            }
            return new List<string>();
        }
        public List<RaidBossFight> _GetBossFights()
        {
            return m_BossFights;
        }
    }
}
