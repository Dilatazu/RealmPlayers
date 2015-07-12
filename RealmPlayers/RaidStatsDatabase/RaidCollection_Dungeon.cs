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
        private List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> _GetDungeonFights(Func<string, FightDataCollection> _GetFightDataCollectionFunc, List<Tuple<int, VF_RaidDamageDatabase.FightDataCollection.FightCacheData>> _RetExtraFightDatas = null)
        {
            List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> fights = new List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData>();
            List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> extraExtraFights = new List<FightDataCollection.FightCacheData>();
            foreach (string file in m_DataFiles)
            {
                var fightCollection = _GetFightDataCollectionFunc(file);
                if (fightCollection != null)
                {
                    foreach (var fight in fightCollection.Fights)
                    {
                        if (fight.m_Fight.TimeSlices.Count == 0)
                            continue;
                        if (fight.m_Fight.TimeSlices[0].GroupMemberIDs == null)
                            continue;

                        if (fight.m_Fight.StartDateTime >= m_DungeonStartDate && fight.m_Fight.GetEndDateTime() <= m_DungeonEndDate
                        && BossInformation.BossFights[fight.m_Fight.FightName] == m_Dungeon)
                        {

                            int overlappingFightIndex = fights.FindIndex(_Value => _Value.IsOverlapping(fight));
                            if (overlappingFightIndex != -1)
                            {
                                //Overlapping fights
                                if (fight.IsBetterVersionOf(fights[overlappingFightIndex]) == true)
                                {
                                    if (_RetExtraFightDatas != null)
                                        _RetExtraFightDatas.Add(Tuple.Create(overlappingFightIndex, fights[overlappingFightIndex]));
                                    fights[overlappingFightIndex] = fight;
                                }
                                else
                                {
                                    if (_RetExtraFightDatas != null)
                                        _RetExtraFightDatas.Add(Tuple.Create(overlappingFightIndex, fight));
                                }
                            }
                            else
                            {
                                fights.Add(fight);
                            }
                            //else
                            //{
                            //    extraExtraFights.Add(fight);
                            //}

                        }
                    }
                }
            }
            if (extraExtraFights.Count > 0)
            {
                if (_RetExtraFightDatas == null)
                {
                    Logger.ConsoleWriteLine("This should never happen! Possibly big error!", ConsoleColor.Red);
                }
                foreach (var extraExtraFight in extraExtraFights)
                {
                    int overlappingFightIndex = fights.FindIndex(_Value => _Value.IsOverlapping(extraExtraFight));
                    if (overlappingFightIndex != -1)
                    {
                        //Overlapping fights
                        if (_RetExtraFightDatas != null)
                            _RetExtraFightDatas.Add(Tuple.Create(overlappingFightIndex, extraExtraFight));
                    }
                }
            }
            return fights;
        }
        private List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> _GetDungeonFightsOrdered(Func<string, FightDataCollection> _GetFightDataCollectionFunc)
        {
            //_RaidSummary CAN BE NULL
            return _GetDungeonFights(_GetFightDataCollectionFunc).OrderBy((_Value) => { return _Value.m_Fight.StartDateTime; }).ToList();
        }
        public List<RaidBossFight> GetBossFights(Func<string, FightDataCollection> _GetFightDataCollectionFunc)
        {
            if (m_BossFightsUpToDate == false)
            {
                m_BossFights.Clear();
                List<Tuple<int, VF_RaidDamageDatabase.FightDataCollection.FightCacheData>> retExtraFights = new List<Tuple<int, FightDataCollection.FightCacheData>>();
                var fights = _GetDungeonFights(_GetFightDataCollectionFunc, retExtraFights);

                List<RaidBossFight> bossFights = new List<RaidBossFight>();
                //var fightsOrdered = _GetRaidFightsOrdered(_GetFightDataCollectionFunc, _RaidSummary);
                for (int i = 0; i < fights.Count; ++i)
                {
                    List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> extraFights = new List<FightDataCollection.FightCacheData>();
                    foreach (var retExtraFight in retExtraFights)
                    {
                        if (retExtraFight.Item1 == i)
                            extraFights.Add(retExtraFight.Item2);
                    }
                    bossFights.Add(new RaidBossFight(this, i, fights[i], extraFights));
                }
                int bossFightIndex = 0;
                bossFights = bossFights.OrderBy((_Value) => _Value.GetStartDateTime()).ToList();
                foreach (var bossFight in bossFights)
                {
                    bossFight._SetRaidBossFightIndex(bossFightIndex++);
                }
                m_BossFights = bossFights;

                m_BossFightsUpToDate = true;
            }
            return m_BossFights;
        }
        public List<RaidBossFight> GetBossFights(FightDataCollection _FightDataCollection)
        {
            List<RaidBossFight> bossFights = new List<RaidBossFight>();

            var fightsOrdered = _GetDungeonFightsOrdered((string _DataFile) => { return _FightDataCollection; });
            for (int i = 0; i < fightsOrdered.Count; ++i)
                bossFights.Add(new RaidBossFight(this, i, fightsOrdered[i]));
            return bossFights;
        }
        public List<RaidBossFight> _GetBossFights()
        {
            return m_BossFights;
        }
    }
}
