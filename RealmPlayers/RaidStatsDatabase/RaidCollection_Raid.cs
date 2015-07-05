using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace VF_RaidDamageDatabase
{
    [ProtoContract]
    public class RaidCollection_Raid
    {
        public static char Raid_VERSION = (char)1;
        [ProtoMember(1)]
        public int UniqueRaidID = -1;
        [ProtoMember(2)]
        public int RaidID = -1;
        [ProtoMember(3)]
        public DateTime RaidResetDateTime = DateTime.MinValue;
        [ProtoMember(4)]
        public string RaidInstance = "Unknown";
        [ProtoMember(5)]
        public DateTime RaidStartDate = DateTime.MaxValue;
        [ProtoMember(6)]
        public DateTime RaidEndDate = DateTime.MinValue;
        [ProtoMember(7)]
        public string RaidOwnerName = "Unknown";
        [ProtoMember(8)]
        public List<string> m_DataFiles = new List<string>();
        [ProtoMember(9)]
        public List<RaidCollection_FightDataFileInfo> m_ExistingFights = new List<RaidCollection_FightDataFileInfo>();
        [ProtoMember(10)]
        public VF_RealmPlayersDatabase.WowRealm Realm = VF_RealmPlayersDatabase.WowRealm.Emerald_Dream;

        //Cache based, do not serialize!
        private List<RaidBossFight> m_BossFights = new List<RaidBossFight>();
        private bool m_BossFightsUpToDate = false;
        private List<RaidBossFight> m_TrashFights = new List<RaidBossFight>();
        private bool m_TrashFightsUpToDate = false;

        public RaidCollection_Raid()
        { }

        public bool AddDataFile(FightDataCollection.FightCacheData _Fight, string _DataFile)
        {
            RaidCollection_FightDataFileInfo currFightDataFileInfo = null;
            int fightIndex = m_ExistingFights.FindIndex(
                (_Value) =>
                {
                    return (_Value.FightName == _Fight.m_Fight.FightName
                                && (Math.Abs((_Value.FightStartDateTime - _Fight.m_Fight.StartDateTime).TotalSeconds) < 10     // Kan göras bättre, missar för tillfället fallet då en fight duration är innuti en annan 
                                    || Math.Abs((_Value.FightEndDateTime - _Fight.m_Fight.GetEndDateTime()).TotalSeconds) < 10 // eller om dom överlappar varandra pga någon loggat ut innan fighten avslutats osv
                                )
                            );
                });
            if (fightIndex == -1)
            {
                Logger.ConsoleWriteLine("Fight: " + _Fight.m_Fight.FightName + " added to RaidCollection", ConsoleColor.Green);
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
                Logger.ConsoleWriteLine("Fight: " + _Fight.m_Fight.FightName + " not added to RaidCollection, allready exists", ConsoleColor.Red);
                currFightDataFileInfo = m_ExistingFights[fightIndex];
            }

            if (currFightDataFileInfo.IsRecordedBy(_Fight.m_Fight.RecordedByPlayer) == false)
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
                if (fight.FightStartDateTime == _FightStartDateTime)
                    return fight.GetAllRecordedBy();
            }
            return new List<string>();
        }
        private List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> _GetRaidFights(Func<string, FightDataCollection> _GetFightDataCollectionFunc, VF_RDDatabase.Raid _RaidSummary, List<Tuple<int, VF_RaidDamageDatabase.FightDataCollection.FightCacheData>> _RetExtraFightDatas = null)
        {
            //_RaidSummary CAN BE NULL
            List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> fights = new List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData>();
            List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> extraExtraFights = new List<FightDataCollection.FightCacheData>();
            foreach (string file in m_DataFiles)
            {
                var fightCollection = _GetFightDataCollectionFunc(file);

                foreach (var fight in fightCollection.Fights)
                {
                    if (fight.m_Fight.RaidID == RaidID
                    && BossInformation.BossFights[fight.m_Fight.FightName] == RaidInstance)
                    {
                        var raidSummaryIndex = int.MaxValue;
                        if (_RaidSummary != null)
                            raidSummaryIndex = _RaidSummary.BossFights.FindIndex((_Value) => _Value.StartDateTime == fight.m_Fight.StartDateTime && _Value.DataDetails.RecordedBy == fight.m_Fight.RecordedByPlayer);
                        if (raidSummaryIndex != -1 || _RetExtraFightDatas != null)
                        {
                            int overlappingFightIndex = fights.FindIndex(_Value => _Value.IsOverlapping(fight));
                            if (overlappingFightIndex != -1)
                            {
                                //Overlapping fights
                                if (fight.IsBetterVersionOf(fights[overlappingFightIndex]) == true && raidSummaryIndex != -1)
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
                            else if (raidSummaryIndex != -1)
                            {
                                fights.Add(fight);
                            }
                            else
                            {
                                extraExtraFights.Add(fight);
                            }
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
        private List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> _GetRaidTrashFights(Func<string, FightDataCollection> _GetFightDataCollectionFunc)
        {
            List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> fights = new List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData>();

            foreach (string file in m_DataFiles)
            {
                var fightCollection = _GetFightDataCollectionFunc(file);

                foreach (var fight in fightCollection.Fights)
                {
                    try
                    {
                        if (fight.m_Fight.RaidID == RaidID
                        && fight.m_Fight.FightName == "Trash")
                        {
                            int overlappingFightIndex = fights.FindIndex(_Value => _Value.IsOverlapping(fight));
                            if (overlappingFightIndex != -1)
                            {
                                //Overlapping fights
                                if (fight.IsBetterVersionOf(fights[overlappingFightIndex]) == true)
                                {
                                    fights[overlappingFightIndex] = fight;
                                }
                            }
                            else
                            {
                                fights.Add(fight);
                            }
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
            return fights;
        }
        private List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> _GetRaidFights_EVERYTHING(Func<string, FightDataCollection> _GetFightDataCollectionFunc)
        {
            List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> fights = new List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData>();

            foreach (string file in m_DataFiles)
            {
                var fightCollection = _GetFightDataCollectionFunc(file);

                foreach (var fight in fightCollection.Fights)
                {
                    if (fight.m_Fight.RaidID == RaidID)
                    {
                        fights.Add(fight);
                    }
                }
            }
            return fights;
        }
        private List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> _GetRaidFightsOrdered(Func<string, FightDataCollection> _GetFightDataCollectionFunc, VF_RDDatabase.Raid _RaidSummary)
        {
            //_RaidSummary CAN BE NULL
            return _GetRaidFights(_GetFightDataCollectionFunc, _RaidSummary).OrderBy((_Value) => { return _Value.m_Fight.StartDateTime; }).ToList();
        }
        private List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> _GetRaidTrashFightsOrdered(Func<string, FightDataCollection> _GetFightDataCollectionFunc)
        {
            return _GetRaidTrashFights(_GetFightDataCollectionFunc).OrderBy((_Value) => { return _Value.m_Fight.StartDateTime; }).ToList();
        }
        //public int GetFightIndex(VF_RaidDamageDatabase.FightDataCollection.FightCacheData _Fight, Func<string, FightDataCollection> _GetFightDataCollectionFunc)
        //{
        //    var fightsOrdered = GetRaidFightsOrdered(_GetFightDataCollectionFunc);
        //    for (int i = 0; i < fightsOrdered.Count; ++i)
        //    {
        //        if (fightsOrdered[i] == _Fight)
        //            return i+1;
        //    }
        //    return -1;
        //}
        public List<RaidBossFight> _GetBossFights()
        {
            return m_BossFights;
        }
        public List<RaidBossFight> GetAllBossFights(Func<string, FightDataCollection> _GetFightDataCollectionFunc)
        {
            List<RaidBossFight> bossFights = new List<RaidBossFight>();
            var fightsOrdered = _GetRaidFights_EVERYTHING(_GetFightDataCollectionFunc).OrderBy((_Value) => _Value.m_Fight.StartDateTime).ToList();
            for (int i = 0; i < fightsOrdered.Count; ++i)
                bossFights.Add(new RaidBossFight(this, i, fightsOrdered[i]));
            return bossFights;
        }
        public List<RaidBossFight> GetBossFights(Func<string, FightDataCollection> _GetFightDataCollectionFunc, VF_RDDatabase.Raid _RaidSummary)
        {
            //_RaidSummary CAN BE NULL
            if (m_BossFightsUpToDate == false || _RaidSummary == null)
            {
                m_BossFights.Clear();
                List<Tuple<int, VF_RaidDamageDatabase.FightDataCollection.FightCacheData>> retExtraFights = new List<Tuple<int, FightDataCollection.FightCacheData>>();
                var fights = _GetRaidFights(_GetFightDataCollectionFunc, _RaidSummary, retExtraFights);

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

                if (_RaidSummary != null)
                    m_BossFightsUpToDate = true;
                else
                    m_BossFightsUpToDate = false;
            }
            return m_BossFights;
        }
        public List<RaidBossFight> GetTrashFights(Func<string, FightDataCollection> _GetFightDataCollectionFunc)
        {
            if (m_TrashFightsUpToDate == false)
            {
                m_TrashFights.Clear();
                List<RaidBossFight> trashFights = new List<RaidBossFight>();
                var fightsOrdered = _GetRaidTrashFightsOrdered(_GetFightDataCollectionFunc);
                for (int i = 0; i < fightsOrdered.Count; ++i)
                    trashFights.Add(new RaidBossFight(this, i, fightsOrdered[i]));
                m_TrashFights = trashFights;
                m_TrashFightsUpToDate = true;
            }
            return m_TrashFights;
        }
        public List<RaidBossFight> GetBossFights(FightDataCollection _FightDataCollection, VF_RDDatabase.Raid _RaidSummary)
        {
            //_RaidSummary CAN BE NULL
            List<RaidBossFight> bossFights = new List<RaidBossFight>();

            var fightsOrdered = _GetRaidFightsOrdered((string _DataFile) => { return _FightDataCollection; }, _RaidSummary);
            for (int i = 0; i < fightsOrdered.Count; ++i)
                bossFights.Add(new RaidBossFight(this, i, fightsOrdered[i]));
            return bossFights;
        }
        public RaidBossFight GetBossFight(int _Index, Func<string, FightDataCollection> _GetFightDataCollectionFunc, VF_RDDatabase.Raid _RaidSummary)
        {
            return GetBossFights(_GetFightDataCollectionFunc, _RaidSummary)[_Index];
        }
        public RaidBossFight GetTrashFight(int _Index, Func<string, FightDataCollection> _GetFightDataCollectionFunc)
        {
            return GetTrashFights(_GetFightDataCollectionFunc)[_Index];
        }
    }
}
