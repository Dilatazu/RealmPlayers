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

namespace VF_WoWLauncherServer
{
    class SQLMigration
    {
        public static NpgsqlConnection _Connection2;
        public static NpgsqlConnection _Connection3;
        public static void SaveFakeContributorData()
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Port=5432;Username=RealmPlayers;Password=TestPass;Database=testdb"))
            {
                conn.Open();
                Console.WriteLine("Started writing contributortable!!!");
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
                        Contributor testContributorVIP = new Contributor(i, "Test.123456");
                        WriteContributor(testContributorVIP);
                        Contributor testContributorNormal = new Contributor(i + Contributor.ContributorTrustworthyIDBound, "Test.123456");
                        WriteContributor(testContributorNormal);

                        if (i % 100 == 0)
                        {
                            Console.WriteLine("Writing contributortable progress(" + i + " / " + 3000 + ")");
                        }
                    }
                }
                Console.WriteLine("Done writing contributortable!!!");
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
            if(_WowVersion == WowVersionEnum.Vanilla)
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
            public int HonorHistoryIDCounter = 1;
            public int GuildHistoryIDCounter = 1;
            public int GearHistoryIDCounter = 1;
            public int ArenaHistoryIDCounter = 1;
            public int TalentsHistoryIDCounter = 1;

            public int IngameItemIDCounter = 1;
            public int ArenaDataIDCounter = 1;

            public int UploadTableIDCounter = 0;
        }


        public static void UploadPlayerDataHistory(NpgsqlConnection _Connection
                                        , ref SQLIDCounters _SQLIDCounters
                                        , WowVersionEnum _WowVersion
                                        , int _PlayerID
                                        , PlayerHistory _PlayerHistory
                                        , out List<KeyValuePair<UploadID, int>> _ResultPlayerUploadIDs)
        {
            _ResultPlayerUploadIDs = new List<KeyValuePair<UploadID, int>>();

            List<KeyValuePair<UploadID, int>> honorHistoryItems;
            List<KeyValuePair<UploadID, int>> guildHistoryItems;
            List<KeyValuePair<UploadID, int>> gearHistoryItems;
            List<KeyValuePair<UploadID, int>> arenaHistoryItems;
            List<KeyValuePair<UploadID, int>> talentsHistoryItems;
            List<KeyValuePair<UploadID, int>> characterHistoryItems;

            UploadHonorData(_Connection, ref _SQLIDCounters.HonorHistoryIDCounter, _WowVersion, _PlayerHistory.HonorHistory, out honorHistoryItems);
            UploadGuildData(_Connection, ref _SQLIDCounters.GuildHistoryIDCounter, _WowVersion, _PlayerHistory.GuildHistory, out guildHistoryItems);
            UploadGearData(_Connection, ref _SQLIDCounters.GearHistoryIDCounter, _WowVersion, _PlayerHistory.GearHistory, out gearHistoryItems, ref _SQLIDCounters.IngameItemIDCounter);
            UploadArenaData(_Connection, ref _SQLIDCounters.ArenaHistoryIDCounter, _WowVersion, _PlayerHistory.ArenaHistory, out arenaHistoryItems, ref _SQLIDCounters.ArenaDataIDCounter);
            UploadTalentsData(_Connection, ref _SQLIDCounters.TalentsHistoryIDCounter, _WowVersion, _PlayerHistory.TalentsHistory, out talentsHistoryItems);

            characterHistoryItems = new List<KeyValuePair<UploadID, int>>();
            int charItemIndex = 0;
            foreach (var charItem in _PlayerHistory.CharacterHistory)
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
                itemNextUploadIDs[_IterateIndex] = itemHistoryItems[_IterateIndex][itemCurrIndexs[_IterateIndex]].Key;
            };

            for (int i = 0; i < DATA_TYPE_COUNT; ++i)
            {
                itemCurrIndexs[i] = 0;
                itemCurrUploadIDs[i] = itemHistoryItems[i][itemCurrIndexs[i]].Key;
                itemNextUploadIDs[i] = itemHistoryItems[i][itemCurrIndexs[i] + 1].Key;
            }
            
            using (var cmdPlayerData = _Connection.BeginBinaryImport("COPY PlayerDataTable (playerid, uploadid, updatetime, race, class, sex, level, guildinfo, honorinfo, gearinfo, arenainfo, talentsinfo) FROM STDIN BINARY"))
            {
                int uploadTableIDCounter = _SQLIDCounters.UploadTableIDCounter;
                using (var cmdUpload = _Connection2.BeginBinaryImport("COPY UploadTable (id, uploadtime, contributor) FROM STDIN BINARY"))
                {
                    while (true)
                    {
                        int nextIterateIndex = itemNextUploadIDs.IndexOfMin((_V) => _V.GetTime());
                        if (itemNextUploadIDs[nextIterateIndex].IsNull())
                            break; //We are done

                        for (int i = 0; i < DATA_TYPE_COUNT; ++i)
                        {
                            if (i == nextIterateIndex || itemNextUploadIDs[i].GetTime() == itemCurrUploadIDs[nextIterateIndex].GetTime())
                            {
                                if(itemNextUploadIDs[i].GetContributorID() != itemCurrUploadIDs[nextIterateIndex].GetContributorID())
                                    Console.WriteLine("This is unexpected, should never happen!!! ContributorID(" + itemNextUploadIDs[i].GetContributorID() + ") != ContributorID(" + itemCurrUploadIDs[nextIterateIndex].GetContributorID() + ")");
                                //Iterate all that have same time and contributor
                                //Include the one we found!
                                IterateNextHistoryItem(i);
                            }
                        }

                        UploadID uploader = itemHistoryItems[nextIterateIndex][itemCurrIndexs[nextIterateIndex]].Key;
                        if (uploader.IsNull() == false)
                        {
                            int currUploadTableID = uploadTableIDCounter++;
                            _ResultPlayerUploadIDs.Add(new KeyValuePair<UploadID, int>(uploader, currUploadTableID));

                            cmdUpload.StartRow();
                            cmdUpload.Write(currUploadTableID, NpgsqlDbType.Integer);
                            cmdUpload.Write(uploader.GetTime(), NpgsqlDbType.Timestamp);
                            cmdUpload.Write(uploader.GetContributorID(), NpgsqlDbType.Integer);

                            int guildItemID = itemHistoryItems[GUILD_INDEX][itemCurrIndexs[GUILD_INDEX]].Value;
                            int honorItemID = itemHistoryItems[HONOR_INDEX][itemCurrIndexs[HONOR_INDEX]].Value;
                            int gearItemID = itemHistoryItems[GEAR_INDEX][itemCurrIndexs[GEAR_INDEX]].Value;
                            int arenaItemID = itemHistoryItems[ARENA_INDEX][itemCurrIndexs[ARENA_INDEX]].Value;
                            int talentsItemID = itemHistoryItems[TALENTS_INDEX][itemCurrIndexs[TALENTS_INDEX]].Value;
                            int charItemID = itemHistoryItems[CHARACTER_INDEX][itemCurrIndexs[CHARACTER_INDEX]].Value;

                            cmdPlayerData.StartRow();
                            cmdPlayerData.Write(_PlayerID, NpgsqlDbType.Integer);
                            cmdPlayerData.Write(currUploadTableID, NpgsqlDbType.Integer);
                            cmdPlayerData.Write(uploader.GetTime(), NpgsqlDbType.Timestamp);
                            cmdPlayerData.Write(_PlayerHistory.CharacterHistory[charItemID].Data.Race, NpgsqlDbType.Smallint);
                            cmdPlayerData.Write(_PlayerHistory.CharacterHistory[charItemID].Data.Class, NpgsqlDbType.Smallint);
                            cmdPlayerData.Write(_PlayerHistory.CharacterHistory[charItemID].Data.Sex, NpgsqlDbType.Smallint);
                            cmdPlayerData.Write(_PlayerHistory.CharacterHistory[charItemID].Data.Level, NpgsqlDbType.Smallint);
                            cmdPlayerData.Write(guildItemID, NpgsqlDbType.Integer);
                            cmdPlayerData.Write(honorItemID, NpgsqlDbType.Integer);
                            cmdPlayerData.Write(gearItemID, NpgsqlDbType.Integer);
                            cmdPlayerData.Write(arenaItemID, NpgsqlDbType.Integer);
                            cmdPlayerData.Write(talentsItemID, NpgsqlDbType.Integer);
                        }
                        else
                        {
                            Console.WriteLine("This is unexpected, should never happen!!!");
                        }
                    }
                }
                _SQLIDCounters.UploadTableIDCounter = uploadTableIDCounter;
            }
        }
        public static void SaveRealmDatabase(RealmDatabase _RealmDatabase)
        {
            var realmPlayersHistory = _RealmDatabase.PlayersHistory;

            int playerTableIDCounter = 0;
            int u = 0;
            SQLIDCounters counters = new SQLIDCounters();
            using (var conn = new NpgsqlConnection("Host=localhost;Port=5432;Username=RealmPlayers;Password=TestPass;Database=testdb"))
            {
                conn.Open();
                using (var conn2 = new NpgsqlConnection("Host=localhost;Port=5432;Username=RealmPlayers;Password=TestPass;Database=testdb"))
                {
                    conn2.Open();
                    _Connection2 = conn2;
                    using (var conn3 = new NpgsqlConnection("Host=localhost;Port=5432;Username=RealmPlayers;Password=TestPass;Database=testdb"))
                    {
                        conn3.Open();
                        _Connection3 = conn3;
                        using (var connPrivate = new NpgsqlConnection("Host=localhost;Port=5432;Username=RealmPlayers;Password=TestPass;Database=testdb"))
                        {
                            connPrivate.Open();
                            using (var cmdPlayer = connPrivate.BeginBinaryImport("COPY PlayerTable (id, name, realm, uploadid) FROM STDIN BINARY"))
                            {
                                foreach (var playerHistory in realmPlayersHistory)
                                {
                                    int currPlayerTableID = playerTableIDCounter++;

                                    List<KeyValuePair<UploadID, int>> uploadItems;
                                    UploadPlayerDataHistory(conn, ref counters, WowVersionEnum.Vanilla, currPlayerTableID, playerHistory.Value, out uploadItems);
                                    if (uploadItems.Count > 0)
                                    {
                                        cmdPlayer.StartRow();
                                        cmdPlayer.Write(currPlayerTableID, NpgsqlDbType.Integer);
                                        cmdPlayer.Write(playerHistory.Key, NpgsqlDbType.Text);
                                        cmdPlayer.Write(_RealmDatabase.Realm, NpgsqlDbType.Integer);
                                        cmdPlayer.Write(uploadItems.Last().Value, NpgsqlDbType.Integer);
                                    }
                                    else
                                    {
                                        Console.WriteLine("This is unexpected, uploadItems.Count was 0!!! should never happen!!!");
                                    }

                                    if (++u % 100 == 0)
                                    {
                                        Console.WriteLine("allRealmUpdates.AddRangeUnique progress(" + u + " / " + realmPlayersHistory.Count + ")");
                                    }
                                }
                            }
                        }
                        _Connection3 = null;
                    }
                    _Connection2 = null;
                }
            }
        }
    }
}
