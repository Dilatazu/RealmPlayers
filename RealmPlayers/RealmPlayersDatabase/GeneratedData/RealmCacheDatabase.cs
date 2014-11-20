using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VF_RealmPlayersDatabase.GeneratedData
{
    public class RealmCacheDatabase
    {
        RealmDatabase m_DatabasePointer;
        private Dictionary<string, Guild> m_Guilds = new Dictionary<string, Guild>();
        private volatile bool m_CurrentlyGeneratingGuilds = false;
        private Dictionary<int, List<Tuple<DateTime, string>>> m_ItemsUsed = new Dictionary<int, List<Tuple<DateTime, string>>>();
        private object m_LockObj = new object();

        //public Guild GetGuild(string _Name)
        //{
        //    if (m_Guilds.ContainsKey(_Name) == false)
        //    {
        //        Monitor.Enter(m_LockObj);
        //        if (m_Guilds.ContainsKey(_Name) == false)
        //            m_Guilds.Add(_Name, new Guild(_Name, m_DatabasePointer));
        //        Monitor.Exit(m_LockObj);
        //    }
        //    return m_Guilds[_Name];
        //}
        //public IEnumerable<KeyValuePair<string, Guild>> GetGuilds()
        //{
        //    if (m_CurrentlyGeneratingGuilds == true)
        //    {
        //        Dictionary<string, Guild> retDictionary = null;
        //        Monitor.Enter(m_LockObj);
        //        retDictionary = new Dictionary<string, Guild>(m_Guilds);
        //        Monitor.Exit(m_LockObj);
        //        return retDictionary;
        //    }
        //    return m_Guilds;
        //}
        //public Dictionary<int, List<Tuple<DateTime, string>>> GetItemsUsed()
        //{
        //    if (m_CurrentlyGeneratingItemsUsed == true)
        //        return new Dictionary<int, List<Tuple<DateTime, string>>>();
        //    else
        //        return m_ItemsUsed;
        //}
        //public bool IsItemsUsedLoaded()
        //{
        //    if(m_ItemsUsed.Count > 0 && m_CurrentlyGeneratingItemsUsed == false)
        //        return true;
        //    else
        //        return false;
        //}
        //public void GenerateGuilds(bool _ForceGenerate = false)
        //{
        //    if (m_Guilds.Count == 0 || _ForceGenerate == true)
        //    {
        //        Monitor.Enter(m_LockObj);
        //        if (m_CurrentlyGeneratingGuilds == false)
        //        {
        //            m_CurrentlyGeneratingGuilds = true;
        //            Monitor.Exit(m_LockObj);
        //            foreach (var player in m_DatabasePointer.Players)
        //            {
        //                if (player.Value.Guild.GuildName != "nil")
        //                    GetGuild(player.Value.Guild.GuildName);
        //            }
        //            m_CurrentlyGeneratingGuilds = false;
        //        }
        //        else
        //            Monitor.Exit(m_LockObj);
        //    }
        //}
        //volatile bool m_CurrentlyGeneratingItemsUsed = false;
        //public void GenerateItemsUsed()
        //{
        //    Monitor.Enter(m_LockObj);
        //    if (m_ItemsUsed.Count == 0 && m_CurrentlyGeneratingItemsUsed == false)
        //    {
        //        m_CurrentlyGeneratingItemsUsed = true;
        //        Monitor.Exit(m_LockObj);
        //        try
        //        {
        //            foreach (var player in m_DatabasePointer.Players)
        //            {
        //                foreach (var item in player.Value.Gear.Items)
        //                {
        //                    if (m_ItemsUsed.ContainsKey(item.Value.ItemID) == false)
        //                        m_ItemsUsed.Add(item.Value.ItemID, new List<Tuple<DateTime, string>>());

        //                    m_ItemsUsed[item.Value.ItemID].Add(new Tuple<DateTime, string>(player.Value.LastSeen, player.Key));
        //                }
        //            }
        //            foreach (var itemUsed in m_ItemsUsed)
        //            {
        //                foreach (var player in m_DatabasePointer.PlayersHistory)
        //                {
        //                    bool doneWithPlayer = false;
        //                    foreach (var gearHistory in player.Value.GearHistory)
        //                    {
        //                        //int itemCount = 0;
        //                        foreach (var item in gearHistory.Data.Items)
        //                        {
        //                            if (item.Value.ItemID == itemUsed.Key)
        //                            {
        //                                var valueIndex = itemUsed.Value.FindIndex((_Tuple) => { return _Tuple.Item2 == player.Key; });
        //                                if (valueIndex == -1)
        //                                    itemUsed.Value.Add(new Tuple<DateTime, string>(gearHistory.Uploader.GetTime(), player.Key));
        //                                else if (itemUsed.Value[valueIndex].Item1 > gearHistory.Uploader.GetTime())
        //                                    itemUsed.Value[valueIndex] = new Tuple<DateTime, string>(gearHistory.Uploader.GetTime(), player.Key);
        //                                //itemCount++;
        //                                doneWithPlayer = true;
        //                                break;
        //                            }
        //                        }
        //                        //if (itemCount == 2)
        //                        //    itemUsed.Value.Add(player.Key);
        //                        if (doneWithPlayer)
        //                            break;
        //                    }
        //                }
        //                //itemUsed.Value = itemUsed.Value.Where((Tuple<DateTime, string>  _Tuple) => { return true; }).ToList();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.LogException(ex);
        //        }
        //        m_CurrentlyGeneratingItemsUsed = false;
        //    }
        //    else
        //        Monitor.Exit(m_LockObj);
        //}
        public RealmCacheDatabase(RealmDatabase _DatabasePointer)
        {
            m_DatabasePointer = _DatabasePointer;
        }
    }
}
