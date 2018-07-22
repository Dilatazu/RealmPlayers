using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace VF_RealmPlayersDatabase
{
    [ProtoContract]
    public class PlayersOnlineDB
    {
        [ProtoContract]
        public class OnlinePlayerEntry
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(2)]
            public int Level;
            [ProtoMember(3)]
            public PlayerClass Class;
            [ProtoMember(4)]
            public PlayerRace Race;
            [ProtoMember(5)]
            public string Guild;
            [ProtoMember(6)]
            public WorldZone Zone;

            public OnlinePlayerEntry()
            { }

            public bool IsSame(OnlinePlayerEntry _Other)
            {
                if (_Other.Zone != Zone) return false;
                if (_Other.Level != Level) return false;
                if (_Other.Guild != Guild) return false;
                if (_Other.Class != Class) return false;
                if (_Other.Race != Race) return false;
                if (_Other.Name != Name) return false;
                return true;
            }
        }

        [ProtoContract]
        public class OnlineEntry
        {
            [ProtoMember(1)]
            public DateTime DateTime_StartSpan;
            [ProtoMember(2)]
            public DateTime DateTime_EndSpan;
            [ProtoMember(3)]
            public Dictionary<string, OnlinePlayerEntry> OnlinePlayers = new Dictionary<string, OnlinePlayerEntry>();
            [ProtoMember(4)]
            public List<OnlinePlayerEntry> OnlinePlayers_Duplicates = new List<OnlinePlayerEntry>();

            public OnlineEntry()
            { }

            public OnlineEntry(DateTime _StartSpan, DateTime _EndSpan)
            {
                DateTime_StartSpan = _StartSpan;
                DateTime_EndSpan = _EndSpan;
            }

            public void AddOnlinePlayer(OnlinePlayerEntry _PlayerEntry)
            {
                if(OnlinePlayers.AddIfKeyNotExist(_PlayerEntry.Name, _PlayerEntry) == false)
                {
                    //Character already exist, check for duplicates
                    var oldPlayer = OnlinePlayers[_PlayerEntry.Name];
                    if (_PlayerEntry.IsSame(oldPlayer) == true)
                        return; //Data already exists

                    foreach(var dupePlayer in OnlinePlayers_Duplicates)
                    {
                        if (dupePlayer.IsSame(_PlayerEntry) == true)
                            return; //Data already exists
                    }
                    OnlinePlayers_Duplicates.Add(_PlayerEntry);
                }
            }
        }

        [ProtoMember(1)]
        public WowRealm Realm = WowRealm.Unknown;

        [ProtoMember(2)]
        public List<OnlineEntry> OnlineEntries = new List<OnlineEntry>();

        public OnlineEntry GetOnlineEntry(DateTime _StartSpan, DateTime _EndSpan)
        {
            for(int i = OnlineEntries.Count - 1; i >= 0; --i)
            {
                if(OnlineEntries[i].DateTime_StartSpan <= _StartSpan && OnlineEntries[i].DateTime_EndSpan >= _EndSpan)
                {
                    return OnlineEntries[i];
                }
                else if(OnlineEntries[i].DateTime_StartSpan < _EndSpan)
                {
                    return null;
                }
            }
            return null;
        }
        public OnlineEntry CreateOnlineEntry(DateTime _StartSpan, DateTime _EndSpan)
        {
            var newEntry = new OnlineEntry(_StartSpan, _EndSpan);
            for (int i = OnlineEntries.Count - 1; i >= 0; --i)
            {
                if(OnlineEntries[i].DateTime_StartSpan < _StartSpan)
                {
                    OnlineEntries.Insert(i + 1, newEntry);
                    return newEntry;
                }
            }
            OnlineEntries.Insert(0, newEntry);
            return newEntry;
        }

        public PlayersOnlineDB()
        { }
    }
}
