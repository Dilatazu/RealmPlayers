using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace VF_RaidDamageDatabase
{
    [ProtoContract]
    [Serializable]
    public class RaidCollection : ISerializable
    {
        public static char RaidCollection_VERSION = (char)1;
        [ProtoMember(1)]
        int m_UniqueRaidIDCounter = 0;
        [ProtoMember(2)]
        public Dictionary<int, Raid> m_Raids = new Dictionary<int, Raid>();

        public RaidCollection()
        { }

        public void AddFightCollection(FightDataCollection _Fights, string _DataFileName, List<RaidCollection.Raid> _ReturnRaidsModified = null)
        {
            List<int> raidsAdded = new List<int>();
            foreach(var fight in _Fights.Fights)
            {
                var realm = VF_RealmPlayersDatabase.StaticValues.ConvertRealm(fight.m_Fight.Realm);
                if (BossInformation.BossFights.ContainsKey(fight.m_Fight.FightName) == false && fight.m_Fight.FightName != "Trash")
                {
                    Logger.ConsoleWriteLine("Fightname(" + fight.m_Fight.FightName + ") is not a BossFight!", ConsoleColor.Red);
                    continue;
                }
                if (fight.m_Fight.RaidID == -1)
                {
                    Logger.ConsoleWriteLine("Fightname(" + fight.m_Fight.FightName + ") was RaidID -1 so it is skipped!", ConsoleColor.Yellow);
                    continue;//Skip RaidIDs that are -1
                }

                var match = m_Raids.FirstOrDefault((_Value) => 
                {
                    if (_Value.Value.RaidID == fight.m_Fight.RaidID)
                    {
                        if (_Value.Value.RaidID != -1)
                        {
                            if ((_Value.Value.RaidResetDateTime - fight.m_Fight.RaidResetDateTime).Days == 0 && _Value.Value.Realm == realm)
                            {
                                return true;
                            }
                        }
                        //else
                        //{
                        //    throw new Exception("Does not support this anymore!");
                        //    if (raidsAdded.Contains(_Value.Key))
                        //    {
                        //        if (_Value.Value.RaidInstance == BossInformation.BossFights[fight.m_Fight.FightName])
                        //        {
                        //            return true;
                        //        }
                        //    }
                        //}
                    }
                    return false;
                });
                RaidCollection.Raid currRaid = null;
                if(match.Equals(default(KeyValuePair<int, Raid>)) == false)
                    currRaid = match.Value;

                if (currRaid == null)
                {
                    currRaid = new RaidCollection.Raid();
                    currRaid.RaidID = fight.m_Fight.RaidID;
                    currRaid.RaidResetDateTime = fight.m_Fight.RaidResetDateTime;
                    currRaid.RaidOwnerName = "";// _DataFileName.Split('\\', '/').Last().Split('_').First();
                    if (fight.m_Fight.FightName != "Trash")
                    {
                        currRaid.RaidInstance = BossInformation.BossFights[fight.m_Fight.FightName];
                    }
                    else
                    {
                        var raidDefineFight = _Fights.Fights.FirstOrDefault((_Value) =>
                        {
                            return _Value.m_Fight.RaidID == fight.m_Fight.RaidID
                                && (_Value.m_Fight.RaidResetDateTime - fight.m_Fight.RaidResetDateTime).Days == 0
                                && VF_RealmPlayersDatabase.StaticValues.ConvertRealm(_Value.m_Fight.Realm) == realm
                                && _Value.m_Fight.FightName != "Trash";
                        });
                        if (raidDefineFight != null && raidDefineFight.Equals(default(KeyValuePair<int, Raid>)) == false)
                            currRaid.RaidInstance = BossInformation.BossFights[raidDefineFight.m_Fight.FightName];
                        else
                            continue;//Skip this Trash!
                    }
                    currRaid.UniqueRaidID = ++m_UniqueRaidIDCounter;
                    currRaid.Realm = realm;
                    m_Raids.Add(currRaid.UniqueRaidID, currRaid);
                    raidsAdded.Add(currRaid.UniqueRaidID);
                }
                if (currRaid.AddDataFile(fight, _DataFileName) == true)
                {
                    if (_ReturnRaidsModified != null)
                    {
                        if (_ReturnRaidsModified.Contains(currRaid) == false)
                            _ReturnRaidsModified.Add(currRaid);
                    }
                    if (fight.m_Fight.FightName != "Trash")
                    {
                        if (fight.m_Fight.StartDateTime < currRaid.RaidStartDate)
                            currRaid.RaidStartDate = fight.m_Fight.StartDateTime;
                        if (fight.m_Fight.GetEndDateTime() > currRaid.RaidEndDate)
                            currRaid.RaidEndDate = fight.m_Fight.GetEndDateTime();
                    }
                }
            }
        }
        
        #region Serializing
        public RaidCollection(SerializationInfo _Info, StreamingContext _Context)
        {
            //char version = _Info.GetChar("Version");
            m_UniqueRaidIDCounter = _Info.GetInt32("m_UniqueRaidIDCounter");
            m_Raids = (Dictionary<int, Raid>)_Info.GetValue("m_Raids", typeof(Dictionary<int, Raid>));
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("Version", RaidCollection_VERSION);
            _Info.AddValue("m_UniqueRaidIDCounter", m_UniqueRaidIDCounter);
            _Info.AddValue("m_Raids", m_Raids);
        }
        #endregion


        [ProtoContract]
        public class FightDataFileInfo
        {
            [ProtoMember(1)]
            public DateTime FightStartDateTime; //Sort of unique identifier for particular fight
            [ProtoMember(2)]
            public string FightName; //For easy to find
            [ProtoMember(3)]
            public List<Tuple<string, string>> _RecordedByPlayers = new List<Tuple<string, string>>();//Players/(Key)/DataFile(Value) that is recorded
            [ProtoMember(4)]
            public DateTime _FightEndDateTime = DateTime.MinValue; //Sort of unique identifier for particular fight
            [ProtoMember(5)]
            public FightData.AttemptType AttemptType = FightData.AttemptType.UnknownAttempt;

            public string GetDataFileRecordedBy(string _Player)
            {
                int index = _RecordedByPlayers.FindIndex(_Value => _Value.Item1 == _Player);
                if (index == -1)
                    return "";
                else
                    return _RecordedByPlayers[index].Item2;
            }
            public string GetFirstDataFile()
            {
                return _RecordedByPlayers.First().Item2;
            }
            public List<string> GetAllDataFiles()
            {
                List<string> retList = new List<string>();
                foreach(var data in _RecordedByPlayers)
                {
                    retList.Add(data.Item2);
                }
                return retList;
            }
            public List<string> GetAllRecordedBy()
            {
                List<string> retList = new List<string>();
                foreach (var data in _RecordedByPlayers)
                {
                    retList.Add(data.Item1);
                }
                return retList;
            }
            public bool IsRecordedBy(string _Player)
            {
                return GetDataFileRecordedBy(_Player) != "";
            }
            public void AddRecordedBy(string _Player, string _DataFile)
            {
                _RecordedByPlayers.Add(Tuple.Create(_Player, _DataFile));
            }

            public DateTime FightEndDateTime
            {
                get
                {
                    if (_FightEndDateTime == DateTime.MinValue)
                        return FightStartDateTime;
                    else
                        return _FightEndDateTime;
                }
            }
        }

        [ProtoContract]
        [Serializable]
        public class Raid : ISerializable
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
            public List<FightDataFileInfo> m_ExistingFights = new List<FightDataFileInfo>();
            [ProtoMember(10)]
            public VF_RealmPlayersDatabase.WowRealm Realm  = VF_RealmPlayersDatabase.WowRealm.Emerald_Dream;

            //Cache based, do not serialize!
            private List<RaidBossFight> m_BossFights = new List<RaidBossFight>();
            private bool m_BossFightsUpToDate = false;
            private List<RaidBossFight> m_TrashFights = new List<RaidBossFight>();
            private bool m_TrashFightsUpToDate = false;

            public Raid()
            { }

            public bool AddDataFile(FightDataCollection.FightCacheData _Fight, string _DataFile)
            {
                FightDataFileInfo currFightDataFileInfo = null;
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
                    Logger.ConsoleWriteLine("Fight: " + _Fight.m_Fight.FightName + " added to RaidCollection", ConsoleColor.Green);
                    currFightDataFileInfo = new FightDataFileInfo
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
                            if(_RaidSummary != null)
                                raidSummaryIndex = _RaidSummary.BossFights.FindIndex((_Value) => _Value.StartDateTime == fight.m_Fight.StartDateTime && _Value.DataDetails.RecordedBy == fight.m_Fight.RecordedByPlayer);
                            if (raidSummaryIndex != -1 || _RetExtraFightDatas != null)
                            {
                                int overlappingFightIndex = fights.FindIndex(_Value => _Value.IsOverlapping(fight));
                                if (overlappingFightIndex != -1)
                                {
                                    //Overlapping fights
                                    if (fight.IsBetterVersionOf(fights[overlappingFightIndex]) == true && raidSummaryIndex != -1)
                                    {
                                        if(_RetExtraFightDatas != null)
                                            _RetExtraFightDatas.Add(Tuple.Create(overlappingFightIndex, fights[overlappingFightIndex]));
                                        fights[overlappingFightIndex] = fight;
                                    }
                                    else
                                    {
                                        if(_RetExtraFightDatas != null)
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
                    List<Tuple<int,VF_RaidDamageDatabase.FightDataCollection.FightCacheData>> retExtraFights = new List<Tuple<int,FightDataCollection.FightCacheData>>();
                    var fights = _GetRaidFights(_GetFightDataCollectionFunc, _RaidSummary, retExtraFights);

                    List<RaidBossFight> bossFights = new List<RaidBossFight>();
                    //var fightsOrdered = _GetRaidFightsOrdered(_GetFightDataCollectionFunc, _RaidSummary);
                    for (int i = 0; i < fights.Count; ++i)
                    {
                        List<VF_RaidDamageDatabase.FightDataCollection.FightCacheData> extraFights = new List<FightDataCollection.FightCacheData>();
                        foreach (var retExtraFight in retExtraFights)
                        {
                            if(retExtraFight.Item1 == i)
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

            #region Serializing
            public Raid(SerializationInfo _Info, StreamingContext _Context)
            {
                //char version = _Info.GetChar("Version");
                UniqueRaidID = _Info.GetInt32("UniqueRaidID");
                RaidID = _Info.GetInt32("RaidID");
                RaidResetDateTime = _Info.GetDateTime("RaidResetDateTime");
                RaidInstance = _Info.GetString("RaidInstance");
                RaidStartDate = _Info.GetDateTime("RaidStartDate");
                RaidEndDate = _Info.GetDateTime("RaidEndDate");
                RaidOwnerName = _Info.GetString("RaidOwnerName");
                m_DataFiles = (List<string>)_Info.GetValue("m_DataFiles", typeof(List<string>));
            }
            public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
            {
                _Info.AddValue("Version", Raid_VERSION);
                _Info.AddValue("UniqueRaidID", UniqueRaidID);
                _Info.AddValue("RaidID", RaidID);
                _Info.AddValue("RaidResetDateTime", RaidResetDateTime);
                _Info.AddValue("RaidInstance", RaidInstance);
                _Info.AddValue("RaidStartDate", RaidStartDate);
                _Info.AddValue("RaidEndDate", RaidEndDate);
                _Info.AddValue("RaidOwnerName", RaidOwnerName);
                _Info.AddValue("m_DataFiles", m_DataFiles);
            }
            #endregion
        }
    }
}
