using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;
using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

using System.Collections.Concurrent;

namespace RealmPlayersServer.Code
{
    public class PlayerAnalyze
    {
        public static ConcurrentDictionary<string, string> sm_PlayerNameChanged = new ConcurrentDictionary<string, string>();
        public static bool HasPlayerNameChanged(Dictionary<string, PlayerHistory> _PlayersHistory, string _Name, WowVersionEnum _WowVersion, out string _OldName)
        {
            _OldName = _Name;
            PlayerHistory thisHistory = null;
            if (_PlayersHistory.TryGetValue(_Name, out thisHistory) == false)
                return false;
            
            var lastCharHistory = thisHistory.CharacterHistory.Last();
            if (lastCharHistory.Data.Level < 60)
                return false;//No one name changes a character below lvl 60 right?

            if (thisHistory.HonorHistory.Last().Data.LifetimeHK < 500)
                return false;//If someone has less than 500 HKs they probably were not important to note, makes the algorithm faster if we can skip already here

            var firstCharHistory = thisHistory.CharacterHistory.First();
            if (firstCharHistory.Data.Level < 60)
            {
                return false; //Assume false for now, only care about 60s from nowhere
                /*
                //This player have level history, check if the history makes sense
                for(int i = thisHistory.CharacterHistory.Count - 1; i >= 0; --i)
                {
                    var charHistory = thisHistory.CharacterHistory[i];
                    if(charHistory.Data.Level < 60)
                    {
                        int beforelevel60Hks = thisHistory.GetHonorItemAtTime(charHistory.Uploader.GetTime()).Data.LifetimeHK;
                        int afterlevel60HKs = thisHistory.GetHonorItemAtTime(thisHistory.CharacterHistory[i+1].Uploader.GetTime()).Data.LifetimeHK;

                        if(beforelevel60Hks > afterlevel60HKs)
                        {
                            return true;//This is very strange, return true so that we can review this at another time
                        }
                        else
                        {
                            if(beforelevel60Hks < 500 && afterlevel60HKs > 
                        }
                    }
                }
                return false;
                 */
            }
            else
            {
                var firstHonorHistory = thisHistory.HonorHistory.First();
                if (firstHonorHistory.Data.LifetimeHK < 500)
                    return false; //If someone had less than 500 HKs when transfering they probably were not important to note, makes the algorithm faster if we can skip already here
                
                DateTime earliestPlayerTime = firstHonorHistory.Uploader.GetTime();

                List<KeyValuePair<string, PlayerHistory>> candidates = new List<KeyValuePair<string, PlayerHistory>>();
                foreach (var playerHistory in _PlayersHistory)
                {
                    if (playerHistory.Value.CharacterHistory.Count < 1
                        || playerHistory.Value.HonorHistory.Count < 1
                        || playerHistory.Value.GearHistory.Count < 1)
                        continue;

                    if (playerHistory.Value.CharacterHistory.Last().Data.Class != lastCharHistory.Data.Class)
                        continue;//It is not possible to change class, so ignore all that arnt same class

                    if (playerHistory.Value.GetLatestDateTime() < earliestPlayerTime)
                    {
                        //Only possible if they were not updated after(assuming no other character have been made in the old name yet)
                        if(playerHistory.Value.HonorHistory.Last().Data.LifetimeHighestRank != firstHonorHistory.Data.LifetimeHighestRank)
                            continue; //Must have same LifetimehighestRank

                        if (playerHistory.Value.HonorHistory.Last().Data.LifetimeHK <= firstHonorHistory.Data.LifetimeHK)
                        {
                            //Only possible if they did not have more HKs(cant go backwards in lifetime HK count)

                            int hkDiff = firstHonorHistory.Data.LifetimeHK - playerHistory.Value.HonorHistory.Last().Data.LifetimeHK;
                            if(hkDiff > 300)
                                continue; //Too big difference, hard to calculate

                            if(hkDiff != 0)
                            {
                                hkDiff -= firstHonorHistory.Data.ThisWeekHK;
                                if(hkDiff != 0)
                                {
                                    hkDiff -= firstHonorHistory.Data.LastWeekHK;
                                    if(hkDiff != 0)
                                    {
                                        hkDiff += playerHistory.Value.HonorHistory.Last().Data.ThisWeekHK + playerHistory.Value.HonorHistory.Last().Data.LastWeekHK;
                                    }
                                }
                            }
                            if (hkDiff == 0)
                            {
                                //Very high chance of being the correct player

                                if(playerHistory.Key != _Name)
                                {
                                    //Only add if "not this" for obvious reasons
                                    //This check is placed all the way inside here for small performance gain
                                    candidates.Add(playerHistory);
                                }
                            }
                        }

                    }
                }
                if(candidates.Count == 0)
                    return false;//No HK matches

                var firstGearHistory = thisHistory.GearHistory.First();
                foreach (var candidate in candidates)
                {
                    var candidateLastGearHistory = candidate.Value.GearHistory.Last();
                    bool allGearSame = true;
                    foreach(var item in firstGearHistory.Data.Items)
                    {
                        if(candidateLastGearHistory.Data.Items.ContainsKey(item.Key))
                        {
                            if(candidateLastGearHistory.Data.Items[item.Key].ItemID != item.Value.ItemID)
                            {
                                allGearSame = false;
                                break;
                            }
                        }
                    }
                    if(allGearSame == true)
                    {
                        //Extremely high chance of this being the correct one! good enough for me!
                        _OldName = candidate.Key;
                        if(sm_PlayerNameChanged.ContainsKey(_Name) == false)
                        {
                            if(sm_PlayerNameChanged.TryAdd(_Name, _OldName) == true)
                            {
                                Logger.ConsoleWriteLine("Name Change Detected: " + _OldName + "->" + _Name, ConsoleColor.Green);
                            }
                        }
                        return true;
                    }
                    else
                    {
                        //Can still be the correct candidate
                        //if atleast 80% of the gear equipped exists in the candidates history we have found our guy!
                        if (StaticValues.GetFaction(candidate.Value.CharacterHistory.First().Data.Race) != StaticValues.GetFaction(lastCharHistory.Data.Race))
                        {
                            //If faction is different we need to exclude all PVP items
                            var itemDropDatabase = DatabaseAccess.GetItemDropDatabase(null, _WowVersion, NotLoadedDecision.SpinWait).GetDatabase();
                            int itemCount = 0;//firstGearHistory.Data.Items.Count;
                            foreach (var item in firstGearHistory.Data.Items)
                            {
                                List<VF_RealmPlayersDatabase.ItemDropDataItem> itemDropInfoList = null;
                                if (itemDropDatabase.TryGetValue(item.Value.ItemID, out itemDropInfoList) == true)
                                {
                                    bool wasPvpItem = false;
                                    foreach (var itemdropInfo in itemDropInfoList)
                                    {
                                        if (((int)itemdropInfo.m_Boss >= (int)VF_RealmPlayersDatabase.WowBoss.PVPOffsetFirst
                                        && (int)itemdropInfo.m_Boss <= (int)VF_RealmPlayersDatabase.WowBoss.PVPOffsetLast)
                                        || ((int)itemdropInfo.m_Boss >= (int)VF_RealmPlayersDatabase.WowBoss.PVPSetFirst
                                        && (int)itemdropInfo.m_Boss <= (int)VF_RealmPlayersDatabase.WowBoss.PVPSetLast))
                                        {
                                            ++itemCount;
                                            wasPvpItem = true;
                                            break;
                                        }
                                    }
                                    if (wasPvpItem == true)
                                        continue;
                                }
                                if (candidate.Value.HaveItem(item.Value) == true)//Pretty expensive, but whatever!
                                {
                                    ++itemCount;
                                }
                            }
                            if ((float)itemCount / (float)firstGearHistory.Data.Items.Count > 0.75f)
                            {
                                _OldName = candidate.Key;
                                if (sm_PlayerNameChanged.ContainsKey(_Name) == false)
                                {
                                    if (sm_PlayerNameChanged.TryAdd(_Name, _OldName) == true)
                                    {
                                        Logger.ConsoleWriteLine("Name Change Detected: " + _OldName + "->" + _Name, ConsoleColor.Green);
                                    }
                                }
                                return true;
                            }
                        }
                        else
                        {
                            int itemCount = 0;//firstGearHistory.Data.Items.Count;
                            foreach (var item in firstGearHistory.Data.Items)
                            {
                                if (candidate.Value.HaveItem(item.Value) == true)//Pretty expensive, but whatever!
                                {
                                    ++itemCount;
                                }
                            }
                            if ((float)itemCount / (float)firstGearHistory.Data.Items.Count > 0.75f)
                            {
                                _OldName = candidate.Key;
                                if (sm_PlayerNameChanged.ContainsKey(_Name) == false)
                                {
                                    if (sm_PlayerNameChanged.TryAdd(_Name, _OldName) == true)
                                    {
                                        Logger.ConsoleWriteLine("Name Change Detected: " + _OldName + "->" + _Name, ConsoleColor.Green);
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}