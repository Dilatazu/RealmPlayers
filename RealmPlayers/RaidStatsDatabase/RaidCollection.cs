using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace VF_RaidDamageDatabase
{
    [ProtoContract]
    public class RaidCollection
    {
        public static char RaidCollection_VERSION = (char)1;
        [ProtoMember(1)]
        int m_UniqueRaidIDCounter = 0;
        [ProtoMember(2)]
        public Dictionary<int, RaidCollection_Raid> m_Raids = new Dictionary<int, RaidCollection_Raid>();
        [ProtoMember(3)]
        int m_UniqueDungeonIDCounter = 0;
        [ProtoMember(4)]
        public Dictionary<int, RaidCollection_Dungeon> m_Dungeons = new Dictionary<int, RaidCollection_Dungeon>();

        public RaidCollection()
        { }

        public void AddFightCollection(FightDataCollection _Fights, string _DataFileName, List<RaidCollection_Raid> _ReturnRaidsModified = null, List<RaidCollection_Dungeon> _ReturnDungeonsModified = null)
        {
            List<int> raidsAdded = new List<int>();
            List<int> dungeonsAdded = new List<int>();
            foreach(var fight in _Fights.Fights)
            {
                var realm = VF_RealmPlayersDatabase.StaticValues.ConvertRealm(fight.m_Fight.Realm);
                if (BossInformation.BossFights.ContainsKey(fight.m_Fight.FightName) == false && fight.m_Fight.FightName != "Trash")
                {
                    Logger.ConsoleWriteLine("Fightname(" + fight.m_Fight.FightName + ") is not a BossFight!", ConsoleColor.Red);
                    continue;
                }
                bool isDungeon = false;
                if (fight.m_Fight.RaidID == -1) isDungeon = true;

                if (isDungeon == false && fight.m_Fight.RaidID == -1)
                {
                    Logger.ConsoleWriteLine("Fightname(" + fight.m_Fight.FightName + ") was RaidID -1 so it is skipped!", ConsoleColor.Yellow);
                    continue;//Skip RaidIDs that are -1
                }

                if (isDungeon == true)
                {
                    Dictionary<string, int> zoneSlices = new Dictionary<string,int>();
                    foreach(var timeSlice in fight.m_Fight.TimeSlices)
                    {
                        if (BossInformation.IsDungeonZone(timeSlice.Zone) == true)
                        {
                            if (zoneSlices.ContainsKey(timeSlice.Zone) == true)
                            {
                                zoneSlices[timeSlice.Zone] = zoneSlices[timeSlice.Zone] + 1;
                            }
                            else
                            {
                                zoneSlices[timeSlice.Zone] = 1;
                            }
                        }
                    }
                    var orderedZones = zoneSlices.OrderByDescending((_Value) => _Value.Value);
                    if(orderedZones.Count() > 0)
                    {
                        string dungeonZone = orderedZones.First().Key;
                        List<string> groupMembers = new List<string>();
                        foreach (var timeSlice in fight.m_Fight.TimeSlices)
                        {
                            if (timeSlice.Zone == dungeonZone)
                            {
                                if (timeSlice.GroupMemberIDs != null)
                                {
                                    foreach (var groupMemberID in timeSlice.GroupMemberIDs)
                                    {
                                        groupMembers.AddUnique(_Fights.GetNameFromUnitID(groupMemberID));
                                    }
                                }
                            }
                        }
                        var match = m_Dungeons.FirstOrDefault((_Value) =>
                        {
                            if (_Value.Value.Realm == realm && _Value.Value.m_Dungeon == dungeonZone
                                && Math.Abs((_Value.Value.m_DungeonStartDate - fight.m_Fight.StartDateTime).TotalHours) < 4)
                            {
                                if(groupMembers.Count > _Value.Value.m_GroupMembers.Count)
                                {
                                    foreach (var groupMember in groupMembers)
                                    {
                                        if(_Value.Value.m_GroupMembers.Contains(groupMember) == false)
                                        {
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var groupMember in _Value.Value.m_GroupMembers)
                                    {
                                        if(groupMembers.Contains(groupMember) == false)
                                        {
                                            return false;
                                        }
                                    }
                                }
                                return true;
                            }
                            return false;
                        });
                        RaidCollection_Dungeon currDungeon = null;
                        if (match.Equals(default(KeyValuePair<int, RaidCollection_Dungeon>)) == false)
                            currDungeon = match.Value;
                        
                        if (currDungeon == null)
                        {
                            currDungeon = new RaidCollection_Dungeon();
                            if (fight.m_Fight.FightName != "Trash")
                            {
                                currDungeon.m_Dungeon = BossInformation.BossFights[fight.m_Fight.FightName];
                            }
                            else
                            {
                                currDungeon.m_Dungeon = dungeonZone;
                            }
                            currDungeon.m_UniqueDungeonID = ++m_UniqueDungeonIDCounter;
                            currDungeon.Realm = realm;
                            m_Dungeons.Add(currDungeon.m_UniqueDungeonID, currDungeon);
                            dungeonsAdded.Add(currDungeon.m_UniqueDungeonID);
                        }
                        if (currDungeon.AddDataFile(fight, _DataFileName) == true)
                        {
                            currDungeon.m_GroupMembers.AddRangeUnique(groupMembers);
                            if (_ReturnRaidsModified != null)
                            {
                                if (_ReturnDungeonsModified.Contains(currDungeon) == false)
                                    _ReturnDungeonsModified.Add(currDungeon);
                            }
                            if (fight.m_Fight.FightName != "Trash")
                            {
                                if (fight.m_Fight.StartDateTime < currDungeon.m_DungeonStartDate)
                                    currDungeon.m_DungeonStartDate = fight.m_Fight.StartDateTime;
                                if (fight.m_Fight.GetEndDateTime() > currDungeon.m_DungeonEndDate)
                                    currDungeon.m_DungeonEndDate = fight.m_Fight.GetEndDateTime();
                            }
                        }
                    }
                }
                else
                { 
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
                    RaidCollection_Raid currRaid = null;
                    if(match.Equals(default(KeyValuePair<int, RaidCollection_Raid>)) == false)
                        currRaid = match.Value;

                    if (currRaid == null)
                    {
                        currRaid = new RaidCollection_Raid();
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
                            if (raidDefineFight != null && raidDefineFight.Equals(default(KeyValuePair<int, RaidCollection_Raid>)) == false)
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
        }
    }
}
