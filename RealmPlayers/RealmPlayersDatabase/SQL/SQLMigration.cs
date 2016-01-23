using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

using VF_RealmPlayersDatabase;
using VF_RealmPlayersDatabase.PlayerData;



public static class extensions
{
    public static int IndexOfMin<TValue, TPredicateValue>(this TValue[] self, Func<TValue, TPredicateValue> _Predicate) where TPredicateValue : IComparable
    {
        if (self == null)
        {
            throw new ArgumentNullException("self");
        }

        if (self.Length == 0)
        {
            throw new ArgumentException("List is empty.", "self");
        }

        TPredicateValue min = _Predicate(self[0]);
        int minIndex = 0;

        for (int i = 1; i < self.Length; ++i)
        {
            TPredicateValue test = _Predicate(self[i]);
            if (test.CompareTo(min) < 0)
            {
                min = test;
                minIndex = i;
            }
        }

        return minIndex;
    }
}

namespace VF
{
    public class SQLMigration
    {
        public static NpgsqlConnection _Connection2;
        public static NpgsqlConnection _Connection3;
        public static void UploadFakeContributorData(SQLIDCounters _SQLCounters)
        {
            using (var conn = new NpgsqlConnection(SQLComm.g_ConnectionString))
            {
                conn.Open();
                Logger.ConsoleWriteLine("Started writing contributortable!!!");
                using (var cmd = conn.BeginBinaryImport("COPY contributortable (id, userid, name, ip) FROM STDIN BINARY"))
                {
                    Action<Contributor> WriteContributor = (Contributor _Contributor) =>
                    {
                        cmd.StartRow();
                        cmd.Write(_Contributor.ContributorID, NpgsqlDbType.Integer);
                        cmd.Write(_Contributor.UserID, NpgsqlDbType.Text);
                        cmd.Write(_Contributor.Name, NpgsqlDbType.Text);
                        cmd.Write(_Contributor.IP, NpgsqlDbType.Text);
                    };

                    for (int i = 0; i < 3000; ++i)
                    {
                        int contributorID = _SQLCounters.ContributorIDCounter++;
                        Contributor testContributorVIP = new Contributor(contributorID, "Test.123456");
                        WriteContributor(testContributorVIP);
                        //Contributor testContributorNormal = new Contributor(i + Contributor.ContributorTrustworthyIDBound, "Test.123456");
                        //WriteContributor(testContributorNormal);

                        if (i % 100 == 0)
                        {
                            Logger.ConsoleWriteLine("Writing contributortable progress(" + i + " / " + 3000 + ")");
                        }
                    }
                }
                Logger.ConsoleWriteLine("Done writing contributortable!!!");
            }
        }

        public static void UploadHonorData(NpgsqlConnection _Connection
                                        , ref int _PlayerHonorTableIDCounter
                                        , WowVersionEnum _WowVersion
                                        , List<HonorDataHistoryItem> _HonorItems
                                        , out List<KeyValuePair<UploadID, int>> _ResultPlayerHonorIDs)
        {
            _ResultPlayerHonorIDs = new List<KeyValuePair<UploadID, int>>();

            if (_HonorItems == null)
                return;

            int playerHonorTableIDCounter = _PlayerHonorTableIDCounter;
            using (var cmd = _Connection.BeginBinaryImport("COPY PlayerHonorTable (id, todayhk, todayhonor, yesterdayhk, yesterdayhonor, lifetimehk) FROM STDIN BINARY"))
            {
                foreach (var playerHonor in _HonorItems)
                {
                    _ResultPlayerHonorIDs.Add(new KeyValuePair<UploadID, int>(playerHonor.Uploader, playerHonorTableIDCounter));

                    int todayhonor = 0;
                    if (_WowVersion != WowVersionEnum.Vanilla)
                        todayhonor = playerHonor.Data.TodayHonorTBC;

                    cmd.StartRow();
                    cmd.Write(playerHonorTableIDCounter++, NpgsqlDbType.Integer);
                    cmd.Write(playerHonor.Data.TodayHK, NpgsqlDbType.Integer);
                    cmd.Write(todayhonor, NpgsqlDbType.Integer);
                    cmd.Write(playerHonor.Data.YesterdayHK, NpgsqlDbType.Integer);
                    cmd.Write(playerHonor.Data.YesterdayHonor, NpgsqlDbType.Integer);
                    cmd.Write(playerHonor.Data.LifetimeHK, NpgsqlDbType.Integer);
                }
            }
            if (_WowVersion == WowVersionEnum.Vanilla)
            {
                playerHonorTableIDCounter = _PlayerHonorTableIDCounter;
                using (var cmd = _Connection.BeginBinaryImport("COPY PlayerHonorVanillaTable (playerhonorid, currentrank, currentrankprogress, todaydk, thisweekhk, thisweekhonor, lastweekhk, lastweekhonor, lastweekstanding, lifetimedk, lifetimehighestrank) FROM STDIN BINARY"))
                {
                    foreach (var playerHonor in _HonorItems)
                    {
                        cmd.StartRow();
                        cmd.Write(playerHonorTableIDCounter++, NpgsqlDbType.Integer);
                        cmd.Write(playerHonor.Data.CurrentRank, NpgsqlDbType.Smallint);
                        cmd.Write(playerHonor.Data.CurrentRankProgress, NpgsqlDbType.Real);
                        cmd.Write(playerHonor.Data.TodayDK, NpgsqlDbType.Integer);
                        cmd.Write(playerHonor.Data.ThisWeekHK, NpgsqlDbType.Integer);
                        cmd.Write(playerHonor.Data.ThisWeekHonor, NpgsqlDbType.Integer);
                        cmd.Write(playerHonor.Data.LastWeekHK, NpgsqlDbType.Integer);
                        cmd.Write(playerHonor.Data.LastWeekHonor, NpgsqlDbType.Integer);
                        cmd.Write(playerHonor.Data.LastWeekStanding, NpgsqlDbType.Integer);
                        cmd.Write(playerHonor.Data.LifetimeDK, NpgsqlDbType.Integer);
                        cmd.Write(playerHonor.Data.LifetimeHighestRank, NpgsqlDbType.Smallint);
                    }
                }
            }
            _PlayerHonorTableIDCounter = playerHonorTableIDCounter;
        }
        public static void UploadGuildData(NpgsqlConnection _Connection
                                        , ref int _PlayerGuildTableIDCounter
                                        , WowVersionEnum _WowVersion
                                        , List<GuildDataHistoryItem> _GuildItems
                                        , out List<KeyValuePair<UploadID, int>> _ResultPlayerGuildIDs)
        {
            _ResultPlayerGuildIDs = new List<KeyValuePair<UploadID, int>>();

            if (_GuildItems == null)
                return;

            int playerGuildTableIDCounter = _PlayerGuildTableIDCounter;
            using (var cmd = _Connection.BeginBinaryImport("COPY PlayerGuildTable (id, guildname, guildrank, guildranknr) FROM STDIN BINARY"))
            {
                foreach (var playerGuild in _GuildItems)
                {
                    _ResultPlayerGuildIDs.Add(new KeyValuePair<UploadID, int>(playerGuild.Uploader, playerGuildTableIDCounter));

                    cmd.StartRow();
                    cmd.Write(playerGuildTableIDCounter++, NpgsqlDbType.Integer);
                    cmd.Write(playerGuild.Data.GuildName, NpgsqlDbType.Text);
                    cmd.Write(playerGuild.Data.GuildRank, NpgsqlDbType.Text);
                    cmd.Write(playerGuild.Data.GuildRankNr, NpgsqlDbType.Smallint);
                }
            }
            _PlayerGuildTableIDCounter = playerGuildTableIDCounter;
        }
        public static Dictionary<int, List<KeyValuePair<int, ItemInfo>>> distinctItemIDs = new Dictionary<int, List<KeyValuePair<int, ItemInfo>>>();
        public static int ingameItemsDuplicateCount = 0;
        public static int ingameItemsUniqueCount = 0;
        public static void UploadGearData(NpgsqlConnection _Connection
                                        , ref int _PlayerGearTableIDCounter
                                        , WowVersionEnum _WowVersion
                                        , List<GearDataHistoryItem> _GearItems
                                        , out List<KeyValuePair<UploadID, int>> _ResultPlayerGearIDs
                                        , ref int _IngameItemTableIDCounter)
        {
            _ResultPlayerGearIDs = new List<KeyValuePair<UploadID, int>>();

            if (_GearItems == null)
                return;

            using (var cmdGearGems = _Connection.BeginBinaryImport("COPY PlayerGearGemsTable (gearid, itemslot, gemid1, gemid2, gemid3, gemid4) FROM STDIN BINARY"))
            {
                int playerGearTableIDCounter = _PlayerGearTableIDCounter;
                using (var cmdGear = _Connection2.BeginBinaryImport("COPY PlayerGearTable (id, head, neck, shoulder, shirt, chest, belt, legs, feet, wrist, gloves, finger_1, finger_2, trinket_1, trinket_2, back, main_hand, off_hand, ranged, tabard) FROM STDIN BINARY"))
                {
                    int ingameItemTableIDCounter = _IngameItemTableIDCounter;
                    using (var cmdItems = _Connection3.BeginBinaryImport("COPY IngameItemTable (id, itemid, enchantid, suffixid, uniqueid) FROM STDIN BINARY"))
                    {
                        foreach (var playerGear in _GearItems)
                        {
                            int currGearTableID = playerGearTableIDCounter++;
                            _ResultPlayerGearIDs.Add(new KeyValuePair<UploadID, int>(playerGear.Uploader, currGearTableID));

                            Func<ItemSlot, int> WriteGearItem = (ItemSlot _Slot) =>
                            {
                                ItemInfo itemInfo;
                                if (playerGear.Data.Items.TryGetValue(_Slot, out itemInfo) == false) return 0;//0 index is empty ItemInfo

                                List<KeyValuePair<int, ItemInfo>> distinctItemInfos;
                                if (distinctItemIDs.TryGetValue(itemInfo.ItemID, out distinctItemInfos) == true)
                                {
                                    foreach (var distinctItem in distinctItemInfos)
                                    {
                                        if (distinctItem.Value.IsSame(itemInfo) == true)
                                        {
                                            ++ingameItemsDuplicateCount;
                                            return distinctItem.Key;
                                        }
                                    }
                                }
                                ++ingameItemsUniqueCount;
                                distinctItemIDs.AddToList(itemInfo.ItemID, new KeyValuePair<int, ItemInfo>(ingameItemTableIDCounter, itemInfo));
                                cmdItems.StartRow();
                                cmdItems.Write(ingameItemTableIDCounter, NpgsqlDbType.Integer);
                                cmdItems.Write(itemInfo.ItemID, NpgsqlDbType.Integer);
                                cmdItems.Write(itemInfo.EnchantID, NpgsqlDbType.Integer);
                                cmdItems.Write(itemInfo.SuffixID, NpgsqlDbType.Integer);
                                cmdItems.Write(itemInfo.UniqueID, NpgsqlDbType.Integer);
                                return ingameItemTableIDCounter++;
                            };

                            cmdGear.StartRow();
                            cmdGear.Write(currGearTableID, NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Head), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Neck), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Shoulder), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Shirt), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Chest), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Belt), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Legs), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Feet), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Wrist), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Gloves), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Finger_1), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Finger_2), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Trinket_1), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Trinket_2), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Back), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Main_Hand), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Off_Hand), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Ranged), NpgsqlDbType.Integer);
                            cmdGear.Write(WriteGearItem(ItemSlot.Tabard), NpgsqlDbType.Integer);

                            if (_WowVersion == WowVersionEnum.TBC)
                            {
                                foreach (var itemInfo in playerGear.Data.Items)
                                {
                                    if (itemInfo.Value.GemIDs != null)
                                    {
                                        cmdGearGems.StartRow();
                                        cmdGearGems.Write(currGearTableID, NpgsqlDbType.Integer);
                                        cmdGearGems.Write(itemInfo.Key, NpgsqlDbType.Smallint);
                                        cmdGearGems.Write(itemInfo.Value.GemIDs[0], NpgsqlDbType.Integer);
                                        cmdGearGems.Write(itemInfo.Value.GemIDs[1], NpgsqlDbType.Integer);
                                        cmdGearGems.Write(itemInfo.Value.GemIDs[2], NpgsqlDbType.Integer);
                                        cmdGearGems.Write(itemInfo.Value.GemIDs[3], NpgsqlDbType.Integer);
                                    }
                                }
                            }
                        }
                    }
                    _IngameItemTableIDCounter = ingameItemTableIDCounter;
                }
                _PlayerGearTableIDCounter = playerGearTableIDCounter;
            }
        }
        public static void UploadArenaData(NpgsqlConnection _Connection
                                        , ref int _PlayerArenaInfoTableIDCounter
                                        , WowVersionEnum _WowVersion
                                        , List<ArenaDataHistoryItem> _ArenaItems
                                        , out List<KeyValuePair<UploadID, int>> _ResultPlayerArenaIDs
                                        , ref int _PlayerArenaDataTableIDCounter)
        {
            _ResultPlayerArenaIDs = new List<KeyValuePair<UploadID, int>>();

            if (_WowVersion == WowVersionEnum.Vanilla || _ArenaItems == null)
                return;

            int playerArenaInfoTableIDCounter = _PlayerArenaInfoTableIDCounter;
            using (var cmdArenaInfo = _Connection.BeginBinaryImport("COPY PlayerArenaInfoTable (id, team_2v2, team_3v3, team_5v5) FROM STDIN BINARY"))
            {
                int playerArenaDataTableIDCounter = _PlayerArenaDataTableIDCounter;
                using (var cmdArenaData = _Connection2.BeginBinaryImport("COPY PlayerArenaDataTable (id, teamname, teamrating, gamesplayed, gameswon, playergamesplayed, playerrating) FROM STDIN BINARY"))
                {
                    foreach (var playerArena in _ArenaItems)
                    {
                        int currArenaInfoTableID = playerArenaInfoTableIDCounter++;
                        _ResultPlayerArenaIDs.Add(new KeyValuePair<UploadID, int>(playerArena.Uploader, currArenaInfoTableID));

                        Func<ArenaPlayerData, int> WriteArenaData = (ArenaPlayerData _ArenaData) =>
                        {
                            if (_ArenaData == null) return 0;

                            cmdArenaData.StartRow();
                            cmdArenaData.Write(playerArenaDataTableIDCounter, NpgsqlDbType.Integer);
                            cmdArenaData.Write(_ArenaData.TeamName, NpgsqlDbType.Text);
                            cmdArenaData.Write(_ArenaData.TeamRating, NpgsqlDbType.Integer);
                            cmdArenaData.Write(_ArenaData.GamesPlayed, NpgsqlDbType.Integer);
                            cmdArenaData.Write(_ArenaData.GamesWon, NpgsqlDbType.Integer);
                            cmdArenaData.Write(_ArenaData.PlayerPlayed, NpgsqlDbType.Integer);
                            cmdArenaData.Write(_ArenaData.PlayerRating, NpgsqlDbType.Integer);
                            return playerArenaDataTableIDCounter++;
                        };

                        cmdArenaInfo.StartRow();
                        cmdArenaInfo.Write(currArenaInfoTableID, NpgsqlDbType.Integer);
                        cmdArenaInfo.Write(WriteArenaData(playerArena.Data.Team2v2), NpgsqlDbType.Integer);
                        cmdArenaInfo.Write(WriteArenaData(playerArena.Data.Team3v3), NpgsqlDbType.Integer);
                        cmdArenaInfo.Write(WriteArenaData(playerArena.Data.Team5v5), NpgsqlDbType.Integer);
                    }
                }
                _PlayerArenaDataTableIDCounter = playerArenaDataTableIDCounter;
            }
            _PlayerArenaInfoTableIDCounter = playerArenaInfoTableIDCounter;
        }
        public static void UploadTalentsData(NpgsqlConnection _Connection
                                        , ref int _PlayerTalentsInfoTableIDCounter
                                        , WowVersionEnum _WowVersion
                                        , List<TalentsDataHistoryItem> _TalentsItems
                                        , out List<KeyValuePair<UploadID, int>> _ResultPlayerTalentsIDs)
        {
            _ResultPlayerTalentsIDs = new List<KeyValuePair<UploadID, int>>();

            if (_WowVersion == WowVersionEnum.Vanilla || _TalentsItems == null)
                return;

            int playerTalentsInfoTableIDCounter = _PlayerTalentsInfoTableIDCounter;
            using (var cmdTalentsInfo = _Connection.BeginBinaryImport("COPY PlayerTalentsInfoTable (id, talents) FROM STDIN BINARY"))
            {
                foreach (var playerTalents in _TalentsItems)
                {
                    int currTalentsInfoTableID = playerTalentsInfoTableIDCounter++;
                    _ResultPlayerTalentsIDs.Add(new KeyValuePair<UploadID, int>(playerTalents.Uploader, currTalentsInfoTableID));

                    cmdTalentsInfo.StartRow();
                    cmdTalentsInfo.Write(currTalentsInfoTableID, NpgsqlDbType.Integer);
                    cmdTalentsInfo.Write(playerTalents.Data, NpgsqlDbType.Text);

                }
            }
            _PlayerTalentsInfoTableIDCounter = playerTalentsInfoTableIDCounter;
        }

        public class SQLIDCounters
        {
            public int PlayerTableIDCounter = 1;

            public int ContributorIDCounter = 1;

            public int HonorHistoryIDCounter = 1;
            public int GuildHistoryIDCounter = 1;
            public int GearHistoryIDCounter = 1;
            public int ArenaHistoryIDCounter = 1;
            public int TalentsHistoryIDCounter = 1;

            public int IngameItemIDCounter = 1;
            public int ArenaDataIDCounter = 1;

            public int IngameMountIDCounter = 1;
            public int IngamePetIDCounter = 1;
            public int IngameCompanionIDCounter = 1;

            int m_UploadTableIDCounter = 1;

            Dictionary<UploadID, int> m_AddedUploadIDs = new Dictionary<UploadID, int>();
            public int GenerateUploadTableID(UploadID _Uploader, NpgsqlConnection _Conn, bool _ForceNew = false)
            {
                int currUploadTableID;
                if (_ForceNew == true || m_AddedUploadIDs.TryGetValue(_Uploader, out currUploadTableID) == false)
                {
                    currUploadTableID = m_UploadTableIDCounter++;
                    using (var cmdUpload = _Conn.BeginBinaryImport("COPY UploadTable (id, uploadtime, contributor) FROM STDIN BINARY"))
                    {
                        cmdUpload.StartRow();
                        cmdUpload.Write(currUploadTableID, NpgsqlDbType.Integer);
                        cmdUpload.Write(_Uploader.GetTime(), NpgsqlDbType.Timestamp);
                        cmdUpload.Write(_Uploader.GetContributorID(), NpgsqlDbType.Integer);
                        cmdUpload.Close();
                    }
                    if (_ForceNew == true)
                    {
                        m_AddedUploadIDs.AddOrSet(_Uploader, currUploadTableID);
                    }
                    else
                    {
                        m_AddedUploadIDs.Add(_Uploader, currUploadTableID);
                    }
                }
                return currUploadTableID;
            }

            public SQLIDCounters()
            {
                using (var conn = new NpgsqlConnection(SQLComm.g_ConnectionString))
                {
                    conn.Open();
                    Func<string, string, int> FetchSeqCounter = (string _TableName, string _ColumnName) =>
                    {
                        using (var cmd = new NpgsqlCommand("SELECT currval(pg_get_serial_sequence('" + _TableName + "','" + _ColumnName + "'))", conn))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read() == true)
                                {
                                    return reader.GetInt32(0) + 1;//We need to add 1 because SQL treats the counter differently than we do!
                                }
                            }
                        }
                        Logger.ConsoleWriteLine("Unexpected Error! Could not Fetch SeqCounter!!!");
                        return 1;
                    };

                    PlayerTableIDCounter = FetchSeqCounter("playertable", "id");

                    ContributorIDCounter = FetchSeqCounter("contributortable", "id");
                    
                    HonorHistoryIDCounter = FetchSeqCounter("playerhonortable", "id");
                    GuildHistoryIDCounter = FetchSeqCounter("playerguildtable", "id");
                    GearHistoryIDCounter = FetchSeqCounter("playergeartable", "id");
                    ArenaHistoryIDCounter = FetchSeqCounter("playerarenainfotable", "id");
                    TalentsHistoryIDCounter = FetchSeqCounter("playertalentsinfotable", "id");

                    IngameItemIDCounter = FetchSeqCounter("ingameitemtable", "id");
                    ArenaDataIDCounter = FetchSeqCounter("playerarenadatatable", "id");

                    IngameMountIDCounter = FetchSeqCounter("ingamemounttable", "id");
                    IngamePetIDCounter = FetchSeqCounter("ingamepettable", "id");
                    IngameCompanionIDCounter = FetchSeqCounter("ingamecompaniontable", "id");

                    m_UploadTableIDCounter = FetchSeqCounter("uploadtable", "id");
                }
            }

            public void UploadNewSequenceCounterValues()
            {
                using (var conn = new NpgsqlConnection(SQLComm.g_ConnectionString))
                {
                    conn.Open();
                    Action<string, string, int> UploadSeqCounter = (string _TableName, string _ColumnName, int _NewSeqValue) =>
                    {
                        _NewSeqValue = _NewSeqValue - 1;//We need to subtract 1 because SQL treats the counter differently than we do!
                        using (var cmd = new NpgsqlCommand("SELECT setval(pg_get_serial_sequence('" + _TableName + "','" + _ColumnName + "'), " + _NewSeqValue + ")", conn))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read() == true)
                                {
                                    if (reader.GetInt32(0) != _NewSeqValue)
                                        Logger.ConsoleWriteLine("Unexpected Error, UploadSeqCounter failed!");
                                }
                            }
                        }
                    };

                    UploadSeqCounter("playertable", "id", PlayerTableIDCounter);

                    UploadSeqCounter("contributortable", "id", ContributorIDCounter);

                    UploadSeqCounter("playerhonortable", "id", HonorHistoryIDCounter);
                    UploadSeqCounter("playerguildtable", "id", GuildHistoryIDCounter);
                    UploadSeqCounter("playergeartable", "id", GearHistoryIDCounter);
                    UploadSeqCounter("playerarenainfotable", "id", ArenaHistoryIDCounter);
                    UploadSeqCounter("playertalentsinfotable", "id", TalentsHistoryIDCounter);

                    UploadSeqCounter("ingameitemtable", "id", IngameItemIDCounter);
                    UploadSeqCounter("playerarenadatatable", "id", ArenaDataIDCounter);

                    UploadSeqCounter("ingamemounttable", "id", IngameMountIDCounter);
                    UploadSeqCounter("ingamepettable", "id", IngamePetIDCounter);
                    UploadSeqCounter("ingamecompaniontable", "id", IngameCompanionIDCounter);

                    UploadSeqCounter("uploadtable", "id", m_UploadTableIDCounter);
                }
            }
        }


        public static void UploadPlayerDataHistory(NpgsqlConnection _Connection
                                        , ref SQLIDCounters _SQLIDCounters
                                        , WowVersionEnum _WowVersion
                                        , int _PlayerID
                                        , KeyValuePair<string, PlayerHistory> _PlayerHistory
                                        , out List<KeyValuePair<UploadID, int>> _ResultPlayerUploadIDs)
        {
            _ResultPlayerUploadIDs = new List<KeyValuePair<UploadID, int>>();

            List<KeyValuePair<UploadID, int>> honorHistoryItems;
            List<KeyValuePair<UploadID, int>> guildHistoryItems;
            List<KeyValuePair<UploadID, int>> gearHistoryItems;
            List<KeyValuePair<UploadID, int>> arenaHistoryItems;
            List<KeyValuePair<UploadID, int>> talentsHistoryItems;
            List<KeyValuePair<UploadID, int>> characterHistoryItems;

            UploadHonorData(_Connection, ref _SQLIDCounters.HonorHistoryIDCounter, _WowVersion, _PlayerHistory.Value.HonorHistory, out honorHistoryItems);
            UploadGuildData(_Connection, ref _SQLIDCounters.GuildHistoryIDCounter, _WowVersion, _PlayerHistory.Value.GuildHistory, out guildHistoryItems);
            UploadGearData(_Connection, ref _SQLIDCounters.GearHistoryIDCounter, _WowVersion, _PlayerHistory.Value.GearHistory, out gearHistoryItems, ref _SQLIDCounters.IngameItemIDCounter);
            UploadArenaData(_Connection, ref _SQLIDCounters.ArenaHistoryIDCounter, _WowVersion, _PlayerHistory.Value.ArenaHistory, out arenaHistoryItems, ref _SQLIDCounters.ArenaDataIDCounter);
            UploadTalentsData(_Connection, ref _SQLIDCounters.TalentsHistoryIDCounter, _WowVersion, _PlayerHistory.Value.TalentsHistory, out talentsHistoryItems);

            characterHistoryItems = new List<KeyValuePair<UploadID, int>>();
            int charItemIndex = 0;
            foreach (var charItem in _PlayerHistory.Value.CharacterHistory)
            {
                characterHistoryItems.Add(new KeyValuePair<UploadID, int>(charItem.Uploader, charItemIndex++));
            }

            honorHistoryItems.Insert(0, new KeyValuePair<UploadID, int>(UploadID.NullMin(), 0));
            guildHistoryItems.Insert(0, new KeyValuePair<UploadID, int>(UploadID.NullMin(), 0));
            gearHistoryItems.Insert(0, new KeyValuePair<UploadID, int>(UploadID.NullMin(), 0));
            arenaHistoryItems.Insert(0, new KeyValuePair<UploadID, int>(UploadID.NullMin(), 0));
            talentsHistoryItems.Insert(0, new KeyValuePair<UploadID, int>(UploadID.NullMin(), 0));
            characterHistoryItems.Insert(0, new KeyValuePair<UploadID, int>(UploadID.NullMin(), -1));

            honorHistoryItems.Add(new KeyValuePair<UploadID, int>(UploadID.NullMax(), 0));
            guildHistoryItems.Add(new KeyValuePair<UploadID, int>(UploadID.NullMax(), 0));
            gearHistoryItems.Add(new KeyValuePair<UploadID, int>(UploadID.NullMax(), 0));
            arenaHistoryItems.Add(new KeyValuePair<UploadID, int>(UploadID.NullMax(), 0));
            talentsHistoryItems.Add(new KeyValuePair<UploadID, int>(UploadID.NullMax(), 0));
            characterHistoryItems.Add(new KeyValuePair<UploadID, int>(UploadID.NullMax(), -1));

            const int HONOR_INDEX = 0;
            const int GUILD_INDEX = 1;
            const int GEAR_INDEX = 2;
            const int ARENA_INDEX = 3;
            const int TALENTS_INDEX = 4;
            const int CHARACTER_INDEX = 5;
            const int DATA_TYPE_COUNT = 6;
            var itemHistoryItems = new List<KeyValuePair<UploadID, int>>[DATA_TYPE_COUNT] { honorHistoryItems, guildHistoryItems, gearHistoryItems, arenaHistoryItems, talentsHistoryItems, characterHistoryItems };
            var itemCurrIndexs = new int[DATA_TYPE_COUNT];
            var itemCurrUploadIDs = new UploadID[DATA_TYPE_COUNT];
            var itemNextUploadIDs = new UploadID[DATA_TYPE_COUNT];

            Action<int> IterateNextHistoryItem = (int _IterateIndex) => {
                itemCurrIndexs[_IterateIndex] = itemCurrIndexs[_IterateIndex] + 1;
                itemCurrUploadIDs[_IterateIndex] = itemNextUploadIDs[_IterateIndex];
                itemNextUploadIDs[_IterateIndex] = itemHistoryItems[_IterateIndex][itemCurrIndexs[_IterateIndex] + 1].Key;
            };

            for (int i = 0; i < DATA_TYPE_COUNT; ++i)
            {
                itemCurrIndexs[i] = 0;
                itemCurrUploadIDs[i] = itemHistoryItems[i][itemCurrIndexs[i]].Key;
                itemNextUploadIDs[i] = itemHistoryItems[i][itemCurrIndexs[i] + 1].Key;
            }

            using (var cmdPlayerData = _Connection.BeginBinaryImport("COPY PlayerDataTable (playerid, uploadid, updatetime, race, class, sex, level, guildinfo, honorinfo, gearinfo, arenainfo, talentsinfo) FROM STDIN BINARY"))
            {
                while (true)
                {
                    int nextIterateIndex = itemNextUploadIDs.IndexOfMin((_V) => _V.GetTime());
                    if (itemNextUploadIDs[nextIterateIndex].IsNull())
                        break; //We are done

                    IterateNextHistoryItem(nextIterateIndex);
                    for (int i = 0; i < DATA_TYPE_COUNT; ++i)
                    {
                        if (i == nextIterateIndex)
                            continue;

                        if ((itemCurrIndexs[i] == 0 && itemHistoryItems[i].Count > 2) || itemNextUploadIDs[i].GetTime() == itemCurrUploadIDs[nextIterateIndex].GetTime())
                        {
                            if (!(itemCurrIndexs[i] == 0 && itemHistoryItems[i].Count > 2) && itemNextUploadIDs[i].GetContributorID() != itemCurrUploadIDs[nextIterateIndex].GetContributorID())
                                Logger.ConsoleWriteLine("This is unexpected, should never happen!!! ContributorID(" + itemNextUploadIDs[i].GetContributorID() + ") != ContributorID(" + itemCurrUploadIDs[nextIterateIndex].GetContributorID() + ")");
                            //Iterate all that have same time and contributor
                            IterateNextHistoryItem(i);
                        }
                    }

                    UploadID uploader = itemHistoryItems[nextIterateIndex][itemCurrIndexs[nextIterateIndex]].Key;
                    if (uploader.IsNull() == false)
                    {
                        int currUploadTableID = _SQLIDCounters.GenerateUploadTableID(uploader, _Connection2, true);
                        _ResultPlayerUploadIDs.Add(new KeyValuePair<UploadID, int>(uploader, currUploadTableID));

                        int guildItemID = itemHistoryItems[GUILD_INDEX][itemCurrIndexs[GUILD_INDEX]].Value;
                        int honorItemID = itemHistoryItems[HONOR_INDEX][itemCurrIndexs[HONOR_INDEX]].Value;
                        int gearItemID = itemHistoryItems[GEAR_INDEX][itemCurrIndexs[GEAR_INDEX]].Value;
                        int arenaItemID = itemHistoryItems[ARENA_INDEX][itemCurrIndexs[ARENA_INDEX]].Value;
                        int talentsItemID = itemHistoryItems[TALENTS_INDEX][itemCurrIndexs[TALENTS_INDEX]].Value;
                        int charItemID = itemHistoryItems[CHARACTER_INDEX][itemCurrIndexs[CHARACTER_INDEX]].Value;
                        CharacterData charItem = null;
                        if (charItemID != -1)
                        {
                            charItem = _PlayerHistory.Value.CharacterHistory[charItemID].Data;
                        }
                        else
                        {
                            charItem = new CharacterData();
                            charItem.Class = PlayerClass.Unknown;
                            charItem.Race = PlayerRace.Unknown;
                            charItem.Sex = PlayerSex.Unknown;
                            charItem.Level = 0;
                            Logger.ConsoleWriteLine("This is unexpected, charItemID should never be null for a proper player! Player=\"" + _PlayerHistory.Key + "\"");
                        }
                        cmdPlayerData.StartRow();
                        cmdPlayerData.Write(_PlayerID, NpgsqlDbType.Integer);
                        cmdPlayerData.Write(currUploadTableID, NpgsqlDbType.Integer);
                        cmdPlayerData.Write(uploader.GetTime(), NpgsqlDbType.Timestamp);
                        cmdPlayerData.Write(charItem.Race, NpgsqlDbType.Smallint);
                        cmdPlayerData.Write(charItem.Class, NpgsqlDbType.Smallint);
                        cmdPlayerData.Write(charItem.Sex, NpgsqlDbType.Smallint);
                        cmdPlayerData.Write(charItem.Level, NpgsqlDbType.Smallint);
                        cmdPlayerData.Write(guildItemID, NpgsqlDbType.Integer);
                        cmdPlayerData.Write(honorItemID, NpgsqlDbType.Integer);
                        cmdPlayerData.Write(gearItemID, NpgsqlDbType.Integer);
                        cmdPlayerData.Write(arenaItemID, NpgsqlDbType.Integer);
                        cmdPlayerData.Write(talentsItemID, NpgsqlDbType.Integer);
                    }
                    else
                    {
                        Logger.ConsoleWriteLine("This is unexpected, should never happen!!!");
                    }
                }
                cmdPlayerData.Close();
            }
        }

        public static void UploadPlayerExtraData(NpgsqlConnection _Connection
                                        , ref SQLIDCounters _SQLIDCounters
                                        , WowVersionEnum _WowVersion
                                        , int _PlayerID
                                        , string _PlayerNane
                                        , ExtraData _ExtraData)
        {
            if (_ExtraData == null)
                return;

            using (var cmdPlayerMount = _Connection.BeginBinaryImport("COPY PlayerMountTable (playerid, uploadid, updatetime, mountid) FROM STDIN BINARY"))
            {
                foreach (var playerMount in _ExtraData.Mounts)
                {
                    int currMountID = _SQLIDCounters.IngameMountIDCounter++;
                    using (var cmdMounts = _Connection2.BeginBinaryImport("COPY IngameMountTable (id, name) FROM STDIN BINARY"))
                    {
                        cmdMounts.StartRow();
                        cmdMounts.Write(currMountID, NpgsqlDbType.Integer);
                        cmdMounts.Write(playerMount.Mount, NpgsqlDbType.Text);
                        cmdMounts.Close();
                    }

                    foreach (var uploader in playerMount.Uploaders)
                    {
                        int currUploadTableID = _SQLIDCounters.GenerateUploadTableID(uploader, _Connection3);

                        cmdPlayerMount.StartRow();
                        cmdPlayerMount.Write(_PlayerID, NpgsqlDbType.Integer);
                        cmdPlayerMount.Write(currUploadTableID, NpgsqlDbType.Integer);
                        cmdPlayerMount.Write(uploader.GetTime(), NpgsqlDbType.Timestamp);
                        cmdPlayerMount.Write(currMountID, NpgsqlDbType.Integer);
                    }
                }
                cmdPlayerMount.Close();
            }

            using (var cmdPlayerPet = _Connection.BeginBinaryImport("COPY PlayerPetTable (playerid, uploadid, updatetime, petid) FROM STDIN BINARY"))
            {
                foreach (var playerPet in _ExtraData.Pets)
                {
                    int currPetID = _SQLIDCounters.IngamePetIDCounter++;
                    using (var cmdPets = _Connection2.BeginBinaryImport("COPY IngamePetTable (id, name, level, creaturefamily, creaturetype) FROM STDIN BINARY"))
                    {
                        cmdPets.StartRow();
                        cmdPets.Write(currPetID, NpgsqlDbType.Integer);
                        cmdPets.Write(playerPet.Name, NpgsqlDbType.Text);
                        cmdPets.Write(playerPet.Level, NpgsqlDbType.Smallint);
                        cmdPets.Write(playerPet.CreatureFamily, NpgsqlDbType.Text);
                        cmdPets.Write(playerPet.CreatureType, NpgsqlDbType.Text);
                        cmdPets.Close();
                    }

                    foreach (var uploader in playerPet.Uploaders)
                    {
                        int currUploadTableID = _SQLIDCounters.GenerateUploadTableID(uploader, _Connection3);

                        cmdPlayerPet.StartRow();
                        cmdPlayerPet.Write(_PlayerID, NpgsqlDbType.Integer);
                        cmdPlayerPet.Write(currUploadTableID, NpgsqlDbType.Integer);
                        cmdPlayerPet.Write(uploader.GetTime(), NpgsqlDbType.Timestamp);
                        cmdPlayerPet.Write(currPetID, NpgsqlDbType.Integer);
                    }
                }
                cmdPlayerPet.Close();
            }

            using (var cmdPlayerCompanion = _Connection.BeginBinaryImport("COPY PlayerCompanionTable (playerid, uploadid, updatetime, companionid) FROM STDIN BINARY"))
            {
                foreach (var playerCompanion in _ExtraData.Companions)
                {
                    int currCompanionID = _SQLIDCounters.IngameCompanionIDCounter++;
                    using (var cmdCompanions = _Connection2.BeginBinaryImport("COPY IngameCompanionTable (id, name, level) FROM STDIN BINARY"))
                    {
                        cmdCompanions.StartRow();
                        cmdCompanions.Write(currCompanionID, NpgsqlDbType.Integer);
                        cmdCompanions.Write(playerCompanion.Name, NpgsqlDbType.Text);
                        cmdCompanions.Write(playerCompanion.Level, NpgsqlDbType.Smallint);
                        cmdCompanions.Close();
                    }

                    foreach (var uploader in playerCompanion.Uploaders)
                    {
                        int currUploadTableID = _SQLIDCounters.GenerateUploadTableID(uploader, _Connection3);

                        cmdPlayerCompanion.StartRow();
                        cmdPlayerCompanion.Write(_PlayerID, NpgsqlDbType.Integer);
                        cmdPlayerCompanion.Write(currUploadTableID, NpgsqlDbType.Integer);
                        cmdPlayerCompanion.Write(uploader.GetTime(), NpgsqlDbType.Timestamp);
                        cmdPlayerCompanion.Write(currCompanionID, NpgsqlDbType.Integer);
                    }
                }
                cmdPlayerCompanion.Close();
            }
        }

        public static void UploadRealmDatabase(SQLIDCounters _SQLCounters, RealmDatabase _RealmDatabase)
        {
            var realmPlayers = _RealmDatabase.Players;
            var realmPlayersHistory = _RealmDatabase.PlayersHistory;
            var realmPlayersExtraData = _RealmDatabase.PlayersExtraData;

            if(_SQLCounters.ContributorIDCounter < 1000)
            {
                VF.SQLMigration.UploadFakeContributorData(_SQLCounters);
            }

            int u = 0;
            using (var conn = new NpgsqlConnection(SQLComm.g_ConnectionString))
            {
                conn.Open();
                using (var conn2 = new NpgsqlConnection(SQLComm.g_ConnectionString))
                {
                    conn2.Open();
                    _Connection2 = conn2;
                    using (var conn3 = new NpgsqlConnection(SQLComm.g_ConnectionString))
                    {
                        conn3.Open();
                        _Connection3 = conn3;
                        using (var connPrivate = new NpgsqlConnection(SQLComm.g_ConnectionString))
                        {
                            connPrivate.Open();
                            foreach (var playerHistory in realmPlayersHistory)
                            {
                                Player thisPlayer;
                                if (realmPlayers.TryGetValue(playerHistory.Key, out thisPlayer) == false)
                                {
                                    thisPlayer = null;
                                    Logger.ConsoleWriteLine("Player \"" + playerHistory.Key + "\" was not found!!!", ConsoleColor.Red);
                                }

                                ExtraData thisPlayerExtraData;
                                if (realmPlayersExtraData.TryGetValue(playerHistory.Key, out thisPlayerExtraData) == false)
                                    thisPlayerExtraData = null;

                                UploadID earliestUploader = playerHistory.Value.GetEarliestUploader();
                                if (thisPlayer.Uploader.GetTime() < earliestUploader.GetTime())
                                    earliestUploader = thisPlayer.Uploader;

                                if (playerHistory.Value.GearHistory.Count == 0)
                                {
                                    playerHistory.Value.AddToHistory(thisPlayer.Gear, earliestUploader);
                                }
                                if (playerHistory.Value.GuildHistory.Count == 0)
                                {
                                    playerHistory.Value.AddToHistory(thisPlayer.Guild, earliestUploader);
                                }
                                if (playerHistory.Value.HonorHistory.Count == 0)
                                {
                                    playerHistory.Value.AddToHistory(thisPlayer.Honor, earliestUploader);
                                }
                                if (playerHistory.Value.CharacterHistory.Count == 0)
                                {
                                    playerHistory.Value.AddToHistory(thisPlayer.Character, earliestUploader);
                                }
                                if (thisPlayer.Arena != null && (playerHistory.Value.ArenaHistory == null || playerHistory.Value.ArenaHistory.Count == 0))
                                {
                                    playerHistory.Value.AddToHistory(thisPlayer.Arena, earliestUploader);
                                }
                                if (thisPlayer.TalentPointsData != null && (playerHistory.Value.TalentsHistory == null || playerHistory.Value.TalentsHistory.Count == 0))
                                {
                                    playerHistory.Value.AddTalentsToHistory(thisPlayer.TalentPointsData, earliestUploader);
                                }

                                int currPlayerTableID = _SQLCounters.PlayerTableIDCounter++;

                                List<KeyValuePair<UploadID, int>> uploadItems;
                                UploadPlayerDataHistory(conn, ref _SQLCounters, StaticValues.GetWowVersion(_RealmDatabase.Realm), currPlayerTableID, playerHistory, out uploadItems);
                                if (uploadItems.Count > 0)
                                {
                                    using (var cmdPlayer = connPrivate.BeginBinaryImport("COPY PlayerTable (id, name, realm, latestuploadid) FROM STDIN BINARY"))
                                    {
                                        cmdPlayer.StartRow();
                                        cmdPlayer.Write(currPlayerTableID, NpgsqlDbType.Integer);
                                        cmdPlayer.Write(playerHistory.Key, NpgsqlDbType.Text);
                                        cmdPlayer.Write(_RealmDatabase.Realm, NpgsqlDbType.Integer);
                                        cmdPlayer.Write(uploadItems.Last().Value, NpgsqlDbType.Integer);
                                    }
                                    if (thisPlayerExtraData != null)
                                        UploadPlayerExtraData(conn, ref _SQLCounters, StaticValues.GetWowVersion(_RealmDatabase.Realm), currPlayerTableID, playerHistory.Key, thisPlayerExtraData);
                                }
                                else
                                {
                                    Logger.ConsoleWriteLine("This is unexpected, uploadItems.Count was 0!!! should never happen!!! But did for player \"" + playerHistory.Key + "\"");
                                }

                                if (++u % 100 == 0)
                                {
                                    Logger.ConsoleWriteLine("player save iteration progress(" + u + " / " + realmPlayersHistory.Count + ")");
                                }
                            }
                        }
                        _Connection3 = null;
                    }
                    _Connection2 = null;
                }
            }

            Logger.ConsoleWriteLine("Items duplicate skipped due to Optimizaiton count = " + ingameItemsDuplicateCount);
            Logger.ConsoleWriteLine("Saved Items Count = " + ingameItemsUniqueCount);
        }

        public static RealmDatabase LoadRealmDatabase(WowRealm _Realm)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            Logger.ConsoleWriteLine("Started Loading Database FROM SQL" + _Realm.ToString(), ConsoleColor.Green);
            RealmDatabase realmDatabase = new RealmDatabase(_Realm);

            using (var conn = new NpgsqlConnection(SQLComm.g_ConnectionString))
            {
                conn.Open();
                Dictionary<string, int> playerIDs = new Dictionary<string, int>();
                using (var reader = conn.BeginBinaryExport("COPY (SELECT id, name FROM PlayerTable WHERE realm = " + (int)_Realm + ") TO STDIN BINARY"))
                {
                    int u = 0;
                    while (reader.StartRow() != -1) //-1 means end of data
                    {
                        int id = reader.Read<int>(NpgsqlDbType.Integer);
                        string name = reader.Read<string>(NpgsqlDbType.Text);
                        playerIDs.Add(name, id);
                        if (++u % 100 == 0)
                        {
                            Logger.ConsoleWriteLine("player load iteration progress(" + u + " / ???)");
                        }
                    }
                    reader.Dispose();
                }
                conn.Close();

                int playerProgress = 0;
                int playerProgressMax = playerIDs.Count;
                foreach (var player in playerIDs)
                {
                    PlayerHistory playerHistory = new PlayerHistory();
                    ++playerProgress;

                    //List<KeyValuePair<CharacterData, UploadID>> charHistoryItems = new List<KeyValuePair<CharacterData, UploadID>>();
                    //List<KeyValuePair<HonorData, UploadID>> honorHistoryItems = new List<KeyValuePair<HonorData, UploadID>>();
                    //List<KeyValuePair<GuildData, UploadID>> guildHistoryItems = new List<KeyValuePair<GuildData, UploadID>>();
                    //List<KeyValuePair<GearData, UploadID>> gearHistoryItems = new List<KeyValuePair<GearData, UploadID>>();

                    List<KeyValuePair<UploadID, List<KeyValuePair<ItemSlot, int>>>> allGearItems = new List<KeyValuePair<UploadID, List<KeyValuePair<ItemSlot, int>>>>();
                    List<int> uniqueGearItems = new List<int>();
                    conn.Open();
                    using (var reader = conn.BeginBinaryExport("COPY (SELECT upload.id, upload.uploadtime, upload.contributor, character.updatetime, character.race, character.class, character.sex, character.level" +
                        ", guild.guildname, guild.guildrank, guild.guildranknr" +
                        ", honor.todayhk, honor.todayhonor, honor.yesterdayhk, honor.yesterdayhonor, honor.lifetimehk" +
                        ", honor2.currentrank, honor2.currentrankprogress, honor2.todaydk, honor2.thisweekhk, honor2.thisweekhonor, honor2.lastweekhk, honor2.lastweekhonor, honor2.lastweekstanding, honor2.lifetimedk, honor2.lifetimehighestrank" +
                        ", gear.head, gear.neck, gear.shoulder, gear.shirt, gear.chest, gear.belt, gear.legs, gear.feet, gear.wrist, gear.gloves, gear.finger_1, gear.finger_2, gear.trinket_1, gear.trinket_2, gear.back, gear.main_hand, gear.off_hand, gear.ranged, gear.tabard" +
                        ", arena.team_2v2, arena.team_3v3, arena.team_5v5" +
                        ", talents.talents" +
                        " FROM PlayerDataTable character" +
                        " INNER JOIN UploadTable upload ON character.UploadID = upload.ID" +
                        " INNER JOIN PlayerGuildTable guild ON character.GuildInfo = guild.ID" +
                        " INNER JOIN PlayerHonorTable honor ON character.HonorInfo = honor.ID" +
                        " INNER JOIN PlayerHonorVanillaTable honor2 ON character.HonorInfo = honor2.PlayerHonorID" +
                        " INNER JOIN PlayerGearTable gear ON character.GearInfo = gear.ID" +
                        " INNER JOIN PlayerArenaInfoTable arena ON character.ArenaInfo = arena.ID" +
                        " INNER JOIN PlayerTalentsInfoTable talents ON character.TalentsInfo = talents.ID" +
                        " WHERE character.playerid = " + player.Value + " ORDER BY character.updatetime) TO STDIN BINARY"))
                    {
                        //Logger.ConsoleWriteLine("Starting read for player \"" + player.Key + "\"!");
                        int u = 0;
                        while (reader.StartRow() != -1) //-1 means end of data
                        {
                            ++u;
                            //Logger.ConsoleWriteLine("Reading PlayerDataTable!");
                            //PlayerDataTable
                            int uploadID = reader.Read<int>(NpgsqlDbType.Integer);
                            DateTime uploadDate = reader.Read<DateTime>(NpgsqlDbType.Timestamp);
                            int contributorID = reader.Read<int>(NpgsqlDbType.Integer);

                            DateTime updateTime = reader.Read<DateTime>(NpgsqlDbType.Timestamp);

                            UploadID uploader = new UploadID(contributorID, updateTime);

                            CharacterData characterData = new CharacterData();
                            characterData.Race = (PlayerRace)reader.Read<int>(NpgsqlDbType.Smallint);
                            characterData.Class = (PlayerClass)reader.Read<int>(NpgsqlDbType.Smallint);
                            characterData.Sex = (PlayerSex)reader.Read<int>(NpgsqlDbType.Smallint);
                            characterData.Level = reader.Read<int>(NpgsqlDbType.Smallint);
                            playerHistory.AddToHistory(characterData, uploader);

                            //JOINED DATA BELOW
                            //Logger.ConsoleWriteLine("Reading PlayerGuildTable!");
                            //PlayerGuildTable
                            if (reader.IsNull) continue;
                            GuildData guildData = new GuildData();
                            guildData.GuildName = reader.Read<string>(NpgsqlDbType.Text);
                            guildData.GuildRank = reader.Read<string>(NpgsqlDbType.Text);
                            guildData.GuildRankNr = reader.Read<int>(NpgsqlDbType.Smallint);
                            playerHistory.AddToHistory(guildData, uploader);

                            //Logger.ConsoleWriteLine("Reading PlayerHonorTable!");
                            //PlayerHonorTable
                            if (reader.IsNull) continue;
                            HonorData honorData = new HonorData();
                            honorData.TodayHK = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.TodayHonorTBC = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.YesterdayHK = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.YesterdayHonor = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.LifetimeHK = reader.Read<int>(NpgsqlDbType.Integer);

                            //Logger.ConsoleWriteLine("Reading PlayerHonorVanillaTable!");
                            //PlayerHonorVanillaTable
                            if (reader.IsNull) continue;
                            honorData.CurrentRank = reader.Read<int>(NpgsqlDbType.Smallint);
                            honorData.CurrentRankProgress = reader.Read<float>(NpgsqlDbType.Real);
                            honorData.TodayDK = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.ThisWeekHK = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.ThisWeekHonor = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.LastWeekHK = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.LastWeekHonor = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.LastWeekStanding = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.LifetimeDK = reader.Read<int>(NpgsqlDbType.Integer);
                            honorData.LifetimeHighestRank = reader.Read<int>(NpgsqlDbType.Smallint);
                            playerHistory.AddToHistory(honorData, uploader);

                            //Logger.ConsoleWriteLine("Reading PlayerGearTable!");
                            //PlayerGearTable
                            if (reader.IsNull) continue;
                            List<KeyValuePair<ItemSlot, int>> gearItems = new List<KeyValuePair<ItemSlot, int>>();
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Head, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Neck, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Shoulder, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Shirt, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Chest, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Belt, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Legs, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Feet, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Wrist, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Gloves, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Finger_1, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Finger_2, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Trinket_1, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Trinket_2, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Back, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Main_Hand, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Off_Hand, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Ranged, reader.Read<int>(NpgsqlDbType.Integer)));
                            gearItems.Add(new KeyValuePair<ItemSlot, int>(ItemSlot.Tabard, reader.Read<int>(NpgsqlDbType.Integer)));
                            uniqueGearItems.AddRangeUnique(gearItems.Select((_V) => _V.Value));
                            allGearItems.Add(new KeyValuePair<UploadID, List<KeyValuePair<ItemSlot, int>>>(uploader, gearItems));
                            //Logger.ConsoleWriteLine("Reading PlayerArenaInfoTable!");
                            //PlayerArenaInfoTable
                            if (reader.IsNull) continue;
                            int team2v2 = reader.Read<int>(NpgsqlDbType.Integer);
                            int team3v3 = reader.Read<int>(NpgsqlDbType.Integer);
                            int team5v5 = reader.Read<int>(NpgsqlDbType.Integer);

                            //Logger.ConsoleWriteLine("Reading PlayerTalentsInfoTable!");
                            //PlayerTalentsInfoTable
                            if (reader.IsNull) continue;
                            string talents = reader.Read<string>(NpgsqlDbType.Text);
                        }
                        if (playerProgress % 100 == 0)
                        {
                            Logger.ConsoleWriteLine("" + playerProgress + " / " + playerProgressMax + ":\tplayer \"" + player.Key + "\"\thad " + u + " updates!" +
                                "\n\tCharacterHistoryCount=" + playerHistory.CharacterHistory.Count +
                                "\n\tHonorHistoryCount=" + playerHistory.HonorHistory.Count +
                                "\n\tGuildHistoryCount=" + playerHistory.GuildHistory.Count);
                        }
                        reader.Dispose();
                    }
                    conn.Close();
                    if (uniqueGearItems.Count > 0)
                    {
                        Dictionary<int, ItemInfo> itemTranslation = new Dictionary<int, ItemInfo>();
                        string sqlSearchIds = String.Join(", ", uniqueGearItems);
                        //Console.WriteLine(sqlSearchIds);

                        if (sqlSearchIds != "" && sqlSearchIds != ", ")
                        {
                            conn.Open();
                            using (var reader = conn.BeginBinaryExport("COPY (SELECT id, itemid, enchantid, suffixid, uniqueid FROM ingameitemtable WHERE id IN (" + sqlSearchIds + ")) TO STDIN BINARY"))
                            {
                                while (reader.StartRow() != -1)
                                {
                                    ItemInfo itemInfo = new ItemInfo();
                                    int itemSQLIndex = reader.Read<int>(NpgsqlDbType.Integer);
                                    itemInfo.ItemID = reader.Read<int>(NpgsqlDbType.Integer);
                                    itemInfo.EnchantID = reader.Read<int>(NpgsqlDbType.Integer);
                                    itemInfo.SuffixID = reader.Read<int>(NpgsqlDbType.Integer);
                                    itemInfo.UniqueID = reader.Read<int>(NpgsqlDbType.Integer);
                                    itemTranslation.AddIfKeyNotExist(itemSQLIndex, itemInfo);
                                }
                            }
                            conn.Close();
                            //using (var cmd = new NpgsqlCommand("SELECT id, itemid, enchantid, suffixid, uniqueid FROM ingameitemtable WHERE id IN (" + sqlSearchIds + ")", conn))
                            //{
                            //    using (var reader = cmd.ExecuteReader())
                            //    {
                            //        if (reader.HasRows == false)
                            //            Logger.ConsoleWriteLine("UNEXPECTED ERROR!!!");

                            //    }
                            //}

                            foreach (var gearItems in allGearItems)
                            {
                                GearData gearData = new GearData();
                                foreach (var itemSlot in gearItems.Value)
                                {
                                    if (itemSlot.Value == 0) continue; //Skip 0s they means no gear was recorded at slot!
                                    ItemInfo itemInfo = itemTranslation[itemSlot.Value];
                                    itemInfo.Slot = itemSlot.Key;
                                    gearData.Items.Add(itemSlot.Key, itemInfo);

                                }
                                playerHistory.AddToHistory(gearData, gearItems.Key);
                            }
                        }
                    }
                    realmDatabase.PlayersHistory.Add(player.Key, playerHistory);
                    Player playerData = new Player();
                    if (playerHistory.GetPlayerAtTime(player.Key, _Realm, DateTime.MaxValue, out playerData) == true)
                    {
                        realmDatabase.Players.Add(player.Key, playerData);
                    }

                    {
                        ExtraData extraData = new ExtraData();
                        conn.Open();
                        using (var reader = conn.BeginBinaryExport("COPY (SELECT upload.contributor, playerseen.updatetime, mount.name FROM playermounttable playerseen" +
                            " INNER JOIN ingamemounttable mount ON playerseen.mountid = mount.ID" +
                            " INNER JOIN uploadtable upload ON playerseen.uploadid = upload.ID" +
                            " WHERE playerseen.playerid = " + player.Value + ") TO STDIN BINARY"))
                        {
                            while (reader.StartRow() != -1)
                            {
                                int contributorID = reader.Read<int>(NpgsqlDbType.Integer);
                                DateTime updateTime = reader.Read<DateTime>(NpgsqlDbType.Timestamp);
                                string mountName = reader.Read<string>(NpgsqlDbType.Text);
                                extraData._AddMount(mountName, new UploadID(contributorID, updateTime));
                            }
                        }
                        conn.Close();
                        conn.Open();
                        using (var reader = conn.BeginBinaryExport("COPY (SELECT upload.contributor, playerseen.updatetime, pet.name, pet.level, pet.creaturefamily, pet.creaturetype FROM playerpettable playerseen" +
                            " INNER JOIN ingamepettable pet ON playerseen.petid = pet.ID" +
                            " INNER JOIN uploadtable upload ON playerseen.uploadid = upload.ID" +
                            " WHERE playerseen.playerid = " + player.Value + ") TO STDIN BINARY"))
                        {
                            while (reader.StartRow() != -1)
                            {
                                int contributorID = reader.Read<int>(NpgsqlDbType.Integer);
                                DateTime updateTime = reader.Read<DateTime>(NpgsqlDbType.Timestamp);
                                string petName = reader.Read<string>(NpgsqlDbType.Text);
                                int petLevel = reader.Read<int>(NpgsqlDbType.Smallint);
                                string petFamily = reader.Read<string>(NpgsqlDbType.Text);
                                string petType = reader.Read<string>(NpgsqlDbType.Text);
                                extraData._AddPet(petName, petLevel, petFamily, petType, new UploadID(contributorID, updateTime));
                            }
                        }
                        conn.Close();
                        conn.Open();
                        using (var reader = conn.BeginBinaryExport("COPY (SELECT upload.contributor, playerseen.updatetime, companion.name, companion.level FROM playercompaniontable playerseen" +
                            " INNER JOIN ingamecompaniontable companion ON playerseen.companionid = companion.ID" +
                            " INNER JOIN uploadtable upload ON playerseen.uploadid = upload.ID" +
                            " WHERE playerseen.playerid = " + player.Value + ") TO STDIN BINARY"))
                        {
                            while (reader.StartRow() != -1)
                            {
                                int contributorID = reader.Read<int>(NpgsqlDbType.Integer);
                                DateTime updateTime = reader.Read<DateTime>(NpgsqlDbType.Timestamp);
                                string companionName = reader.Read<string>(NpgsqlDbType.Text);
                                int companionLevel = reader.Read<int>(NpgsqlDbType.Smallint);
                                extraData._AddCompanion(companionName, companionLevel, new UploadID(contributorID, updateTime));
                            }
                        }
                        conn.Close();
                        realmDatabase.PlayersExtraData.Add(player.Key, extraData);
                    }
                }
            }

            Logger.ConsoleWriteLine("Done with loading Database FROM SQL " + _Realm.ToString() + ", it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
            return realmDatabase;
        }
    }
}
