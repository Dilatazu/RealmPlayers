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

        public RaidCollection()
        { }

        public void AddFightCollection(FightDataCollection _Fights, string _DataFileName, List<RaidCollection_Raid> _ReturnRaidsModified = null)
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
                RaidCollection_Raid currRaid = null;
                if (match.Equals(default(KeyValuePair<int, RaidCollection_Raid>)) == false)
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
