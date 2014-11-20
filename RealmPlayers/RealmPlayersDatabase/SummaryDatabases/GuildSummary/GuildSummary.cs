using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using PlayerData = VF_RealmPlayersDatabase.PlayerData;
using Faction = VF_RealmPlayersDatabase.PlayerFaction;

namespace VF_RPDatabase
{
    [ProtoContract]
    public class GuildPlayerStatus
    {
        [ProtoMember(1)]
        private string m_GuildRank = "None";
        [ProtoMember(2)]
        private int m_GuildRankNr = -1;
        [ProtoMember(3)]
        private DateTime m_DateTime = DateTime.MinValue;

        public string GuildRank
        {
            get { return m_GuildRank; }
        }
        public int GuildRankNr
        {
            get { return m_GuildRankNr; }
        }
        public DateTime DateTime
        {
            get { return m_DateTime; }
        }
        public bool IsInGuild
        {
            get { return m_GuildRankNr != -1337 || m_GuildRank != ""; }
        }

        public GuildPlayerStatus() { }
        public GuildPlayerStatus(GuildSummary _GuildSummary, PlayerData.GuildData _GuildData, DateTime _DateTime)
        {
            m_DateTime = _DateTime;
            if (_GuildData.GuildName == _GuildSummary.GuildName)
            {
                m_GuildRank = _GuildData.GuildRank;
                m_GuildRankNr = _GuildData.GuildRankNr;
            }
            else
            {
                m_GuildRankNr = -1337;
                m_GuildRank = "";
            }
        }
        public static GuildPlayerStatus CreateNotInGuild(DateTime _DateTime)
        {
            GuildPlayerStatus newGuildPlayerStatus = new GuildPlayerStatus();
            newGuildPlayerStatus.m_DateTime = _DateTime;
            newGuildPlayerStatus.m_GuildRankNr = -1337;
            newGuildPlayerStatus.m_GuildRank = "";
            return newGuildPlayerStatus;
        }

        public bool IsSame(GuildPlayerStatus _Other)
        {
            return _Other.GuildRank == _Other.GuildRank && _Other.GuildRankNr == _Other.GuildRankNr;
        }

    }

    [ProtoContract]
    public class GuildPlayerStatusHistory
    {
        [ProtoMember(1)]
        private string m_PlayerName = "Unknown";
        [ProtoMember(2)]
        private List<GuildPlayerStatus> m_History = new List<GuildPlayerStatus>();
        
        public GuildPlayerStatusHistory() { }
        public GuildPlayerStatusHistory(string _PlayerName)
        {
            m_PlayerName = _PlayerName;
        }

        private void AddGuildPlayerStatus(GuildPlayerStatus _GuildPlayerStatus)
        {
            if (m_History.Count > 0)
            {
                if (m_History.Last().DateTime < _GuildPlayerStatus.DateTime)
                {
                    //if(m_History.Last().IsSame(_GuildPlayerStatus) == false)
                        m_History.Add(_GuildPlayerStatus);
                }
                else if (m_History.First().DateTime > _GuildPlayerStatus.DateTime)
                {
                    //if (m_History.First().IsSame(_GuildPlayerStatus) == false)
                        m_History.Insert(0, _GuildPlayerStatus);
                }
                else
                {
                    for (int i = m_History.Count - 1; i >= 0; --i)
                    {
                        if (m_History[i].DateTime == _GuildPlayerStatus.DateTime)
                            break;//Data with this exact DateTime allready exists, so skip

                        if (m_History[i].DateTime < _GuildPlayerStatus.DateTime)
                        {
                            //if (m_History[i].IsSame(_GuildPlayerStatus) == false)
                            {
                                if (i + 1 < m_History.Count)//egentligen onödig check
                                {
                                    m_History.Insert(i + 1, _GuildPlayerStatus);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                m_History.Add(_GuildPlayerStatus);
            }
        }
        public void SetNotInGuild(DateTime _DateTime)
        {
            if (m_History.Count > 0)
            {
                if (m_History.First().DateTime > _DateTime && m_History.First().IsInGuild == false)
                    return;//Not necessary to add older "not in guild" data
                if (m_History.Last().DateTime < _DateTime && m_History.Last().IsInGuild == false)
                    return;//Not necessary to add newer "not in guild" data
            }
            AddGuildPlayerStatus(GuildPlayerStatus.CreateNotInGuild(_DateTime));
        }

        public void SetGuildData(PlayerData.GuildData _GuildData, DateTime _DateTime)
        {
            AddGuildPlayerStatus(new GuildPlayerStatus(m_CacheGuild, _GuildData, _DateTime));
        }
        public bool IsInGuild
        {
            get { return m_History.Count == 0 || m_History.Last().IsInGuild; }
        }

        public string PlayerName
        {
            get { return m_PlayerName; }
        }
        public List<GuildPlayerStatus> History
        {
            get { return m_History; }
        }

#region Cache Data
        private GuildSummary m_CacheGuild = null;

        public GuildSummary CacheGuild
        {
            get { return m_CacheGuild; }
        }
        public void InitCache(GuildSummary _GuildSummary)
        {
            m_CacheGuild = _GuildSummary;
        }
        public bool CacheInitialized()
        {
            return m_CacheGuild != null;
        }
        public void Dispose()
        {
            m_CacheGuild = null;
        }
#endregion 
    }

    [ProtoContract]
    public class GuildSummary
    {
        [ProtoMember(1)]
        private string m_GuildName = "Unknown";
        [ProtoMember(2)]
        private WowRealm m_Realm = WowRealm.Unknown;
        [ProtoMember(3)]
        private Faction m_Faction = Faction.Unknown;
        [ProtoMember(4)]
        private Dictionary<string, GuildPlayerStatusHistory> m_Players = new Dictionary<string, GuildPlayerStatusHistory>();

        //Cache
        private List<Tuple<PlayerData.Player, GuildPlayerStatusHistory>> m_MembersArray = new List<Tuple<PlayerData.Player, GuildPlayerStatusHistory>>();
        private bool m_MembersGenerated = false;
        public Tuple<string, int> m_GuildProgressData = Tuple.Create("", 0);
        //Cache

        public GuildSummary() { }
        public GuildSummary(string _GuildName, WowRealm _Realm)
        {
            m_GuildName = _GuildName;
            m_Realm = _Realm;
        }

        public class PlayerGuildStatusChange
        {
            private string m_Player;
            private GuildPlayerStatus m_OldStatus;
            private GuildPlayerStatus m_NewStatus;

            public PlayerGuildStatusChange(string _Player, GuildPlayerStatus _OldStatus, GuildPlayerStatus _NewStatus)
            {
                m_Player = _Player;
                m_OldStatus = _OldStatus;
                m_NewStatus = _NewStatus;
            }

            public string Player
            {
                get { return m_Player; }
            }
            public GuildPlayerStatus FromStatus
            {
                get { return m_OldStatus; }
            }
            public GuildPlayerStatus ToStatus
            {
                get { return m_NewStatus; }
            }
        }
        public List<PlayerGuildStatusChange> GenerateLatestStatusChanges(DateTime _EarliestChange)
        {
            List<PlayerGuildStatusChange> result = new List<PlayerGuildStatusChange>();
            foreach (var player in m_Players)
            {
                GuildPlayerStatus oldStatus = null;
                foreach(var playerStatus in player.Value.History)
                {
                    if (playerStatus.DateTime < _EarliestChange)
                    {
                        oldStatus = playerStatus;
                        continue;
                    }
                    if (oldStatus == null)
                    {
                        result.Add(new PlayerGuildStatusChange(player.Key, oldStatus, playerStatus));
                        oldStatus = playerStatus;
                    }
                    else if (oldStatus.GuildRank != playerStatus.GuildRank)
                    {
                        result.Add(new PlayerGuildStatusChange(player.Key, oldStatus, playerStatus));
                        oldStatus = playerStatus;
                    }
                }
            }
            return result.OrderBy((_Value) => _Value.ToStatus.DateTime).ToList();
        }

        //public void Update(string _PlayerName, PlayerData.Player _PlayerData)
        //{
        //    GuildPlayerStatusHistory playerGuildHistory = null;
        //    if (m_Players.TryGetValue(_PlayerName, out playerGuildHistory) == false)
        //        playerGuildHistory = null;

        //    if (_PlayerData.Guild.GuildName == m_GuildName)
        //    {
        //        playerGuildHistory = new GuildPlayerStatusHistory(_PlayerName);
        //        playerGuildHistory.InitCache(this);
        //        m_Players.Add(_PlayerName, playerGuildHistory);
        //        playerGuildHistory.SetGuildData(_PlayerData.Guild, _PlayerData.LastSeen);

        //        if (m_Faction == Faction.Unknown)
        //        {
        //            m_Faction = VF_RealmPlayersDatabase.StaticValues.GetFaction(_PlayerData.Character.Race);
        //        }
        //    }
        //    else if (playerGuildHistory != null)
        //    {
        //        playerGuildHistory.SetNotInGuild(_PlayerData.LastSeen);
        //    }
        //}
        public void Update(string _PlayerName, PlayerData.PlayerHistory _PlayerData, DateTime _EarliestDateTime)
        {
            GuildPlayerStatusHistory playerGuildHistory = null;
            if (m_Players.TryGetValue(_PlayerName, out playerGuildHistory) == false)
                playerGuildHistory = null;

            for(int i = 0; i <_PlayerData.GuildHistory.Count; ++i)
            {
                var guildHistory = _PlayerData.GuildHistory[i];
                if (guildHistory.Uploader.GetTime() < _EarliestDateTime)
                    continue;

                if (guildHistory.Data.GuildName == m_GuildName)
                {
                    if (playerGuildHistory == null)
                    {
                        if (m_Players.TryGetValue(_PlayerName, out playerGuildHistory) == false)
                        {
                            playerGuildHistory = new GuildPlayerStatusHistory(_PlayerName);
                            playerGuildHistory.InitCache(this);
                            m_Players.Add(_PlayerName, playerGuildHistory);

                            if (m_Faction == Faction.Unknown)
                            {
                                if(_PlayerData.CharacterHistory.Count > 0)
                                    m_Faction = VF_RealmPlayersDatabase.StaticValues.GetFaction(_PlayerData.GetCharacterItemAtTime(guildHistory.Uploader.GetTime()).Data.Race);
                            }
                        }
                    }
                    playerGuildHistory.SetGuildData(guildHistory.Data, guildHistory.Uploader.GetTime());
                }
                else if (playerGuildHistory != null)
                {
                    playerGuildHistory.SetNotInGuild(guildHistory.Uploader.GetTime());
                }
            }
        }
        public void Update(string _PlayerName, PlayerData.PlayerHistory _PlayerData)
        {
            Update(_PlayerName, _PlayerData, DateTime.MinValue);
        }

        public void GenerateCache(Dictionary<string, PlayerData.Player> _RealmDB, bool _ForceGenerate = false)
        {
            if (_RealmDB == null)
                return;

            if (m_MembersGenerated == true)
            {
                if(_ForceGenerate == false)
                    return;
            }
            List<Tuple<PlayerData.Player, GuildPlayerStatusHistory>> membersArray = new List<Tuple<PlayerData.Player, GuildPlayerStatusHistory>>();
            foreach (var guildPlayer in m_Players)
            {
                if (guildPlayer.Value.IsInGuild == true)
                {
                    PlayerData.Player currPlayer = null;
                    if (_RealmDB.TryGetValue(guildPlayer.Value.PlayerName, out currPlayer) == true)
                    {
                        if ((DateTime.UtcNow - currPlayer.Uploader.GetTime()).Days < 31)
                        {
                            membersArray.Add(Tuple.Create(currPlayer, guildPlayer.Value));
                        }
                    }
                }
            }
            m_MembersArray = membersArray;
            m_MembersGenerated = true;
        }
        public System.Collections.ObjectModel.ReadOnlyCollection<Tuple<PlayerData.Player, GuildPlayerStatusHistory>> GetMembers()
        {
            return m_MembersArray.AsReadOnly();
        }
        public long Stats_GetTotals(Func<PlayerData.Player, int> _VariableGetLambda)
        {
            long total = 0;
            foreach (var player in GetMembers())
            {
                total += _VariableGetLambda(player.Item1);
            }
            return total;
        }
        public double Stats_GetTotals(Func<PlayerData.Player, double> _VariableGetLambda)
        {
            double total = 0;
            foreach (var player in GetMembers())
            {
                total += _VariableGetLambda(player.Item1);
            }
            return total;
        }
        private int m_CacheAveragePVPRank = -1;
        public int Stats_GetAveragePVPRank()
        {
            //Only for vanilla so hardcoded lvl 60 is fine here
            if (m_CacheAveragePVPRank == -1)
            {
                int maxLevelCount = Stats_GetTotalMaxLevels();
                m_CacheAveragePVPRank = (int)(Stats_GetTotals((PlayerData.Player _Player) =>
                {
                    if (_Player.Character.Level == 60)
                        return _Player.GetRankTotal() / maxLevelCount;
                    else
                        return 0.0;
                }));
            }
            return m_CacheAveragePVPRank;
        }
        private int m_CacheAverageMemberHKs = -1;
        public int Stats_GetAverageMemberHKs()
        {
            if (m_CacheAverageMemberHKs == -1)
            {
                var wowVersion = VF_RealmPlayersDatabase.StaticValues.GetWowVersion(m_Realm);
                int maxLevel = VF_RealmPlayersDatabase.StaticValues.GetMaxLevel(wowVersion);
                int maxLevelCount = Stats_GetTotalMaxLevels(wowVersion);
                m_CacheAverageMemberHKs = (int)(Stats_GetTotals((PlayerData.Player _Player) =>
                {
                    if (_Player.Character.Level == maxLevel)
                        return (double)_Player.Honor.LifetimeHK / maxLevelCount;
                    else
                        return 0.0;
                }));
            }
            return m_CacheAverageMemberHKs;
        }
        private int m_CacheAveragePVPStanding = -1;
        public int Stats_GetAveragePVPStanding()
        {
            //Only for vanilla so hardcoded lvl 60 is fine here
            if (m_CacheAveragePVPStanding == -1)
            {
                long level60AndStandingReceivedCount = Stats_GetTotals((PlayerData.Player _Player) =>
                {
                    if (_Player.Character.Level == 60 && _Player.ReceivedStandingLastWeek())
                        return 1;
                    else
                        return 0;
                });

                if (level60AndStandingReceivedCount == 0)
                    return -1;

                m_CacheAveragePVPStanding = (int)(Stats_GetTotals((PlayerData.Player _Player) =>
                {
                    if (_Player.Character.Level == 60 && _Player.ReceivedStandingLastWeek())
                        return (double)_Player.Honor.LastWeekStanding / level60AndStandingReceivedCount;
                    else
                        return 0.0;
                }));
            }
            return m_CacheAveragePVPStanding;
        }
        public int Stats_GetTotalStandingReceivers()
        {
            //Only for vanilla so hardcoded lvl 60 is fine here
            long level60AndStandingReceivedCount = Stats_GetTotals((PlayerData.Player _Player) =>
            {
                if (_Player.Character.Level == 60 && _Player.ReceivedStandingLastWeek())
                    return 1;
                else
                    return 0;
            });
            return (int)level60AndStandingReceivedCount;
        }
        public long Stats_GetTotalHKs()
        {
            return Stats_GetTotals((PlayerData.Player _Player) => { return _Player.Honor.LifetimeHK; });
        }
        public int Stats_GetTotalMaxLevels()
        {
            var wowVersion = VF_RealmPlayersDatabase.StaticValues.GetWowVersion(m_Realm);
            return Stats_GetTotalMaxLevels(wowVersion);
        }
        public int Stats_GetTotalMaxLevels(VF_RealmPlayersDatabase.WowVersionEnum _WowVersion)
        {
            int maxLevel = VF_RealmPlayersDatabase.StaticValues.GetMaxLevel(_WowVersion);
            return (int)Stats_GetTotals((PlayerData.Player _Player) => { return ((_Player.Character.Level == maxLevel) ? 1 : 0); });
        }
        //public int Stats_GetTotalLevel70s()
        //{
        //    return (int)Stats_GetTotals((PlayerData.Player _Player) => { return ((_Player.Character.Level == 70) ? 1 : 0); });
        //}
        
        //public int GetMemberCount()
        //{
        //    return m_Players.Count((_Value) => _Value.Value.IsInGuild);
        //}


        public string GuildName
        {
            get { return m_GuildName; }
        }
        public WowRealm Realm
        {
            get { return m_Realm; }
        }
        public Faction Faction
        {
            get { return m_Faction; }
        }
        public Dictionary<string, GuildPlayerStatusHistory> Players
        {
            get { return m_Players; }
        }

        public void InitCache()
        {
            foreach (var player in m_Players)
            {
                player.Value.InitCache(this);
            }
        }
        public void Dispose()
        {
            m_MembersArray.Clear();
            foreach (var player in m_Players)
            {
                player.Value.Dispose();
            }
        }
        
        ~GuildSummary()
        {
            Dispose();
        }
    }
}
