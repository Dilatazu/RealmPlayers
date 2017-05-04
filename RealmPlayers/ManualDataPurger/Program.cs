using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using PlayerData = VF_RealmPlayersDatabase.PlayerData;
using UploadID = VF_RealmPlayersDatabase.UploadID;

using CharacterData = VF_RealmPlayersDatabase.PlayerData.CharacterData;
using GuildData = VF_RealmPlayersDatabase.PlayerData.GuildData;
using HonorData = VF_RealmPlayersDatabase.PlayerData.HonorData;
using GearData = VF_RealmPlayersDatabase.PlayerData.GearData;

using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;
using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerSex = VF_RealmPlayersDatabase.PlayerSex;

using ItemDropDatabase = VF_RealmPlayersDatabase.ItemDropDatabase;
namespace ManualDataPurger
{
    class Program
    {
        static void FixDatabase(string _Name, string _DatabaseFile, ItemDropDatabase _ItemDropDatabase, DateTime _BetweenDate_MIN, DateTime _BetweenDate_HonorMAX, DateTime _BetweenDate_ItemMax)
        {
            Dictionary<string, PlayerData.PlayerHistory> historyData = null;
            VF_RealmPlayersDatabase.Utility.LoadSerialize(_DatabaseFile, out historyData);

            List<string> gearFixed = new List<string>();
            List<string> honorFixed = new List<string>();
            List<string> triedHonorFix = new List<string>();
            foreach (var playerHistory in historyData)
            {
                var honorHistorys = playerHistory.Value.HonorHistory;
                var gearHistorys = playerHistory.Value.GearHistory;

                bool fixedHonor = false;
                bool triedFixedHonor = false;
                bool fixedGear = false;
                if (honorHistorys.Count > 0)
                {
                    if (honorHistorys.Last().Uploader.GetTime() > _BetweenDate_MIN)
                    {
                        int realLifeTimeHighestRank = 0;
                        float realRankProgress = 0;
                        int realRank = 0;
                        if (honorHistorys.Last().Uploader.GetTime() > _BetweenDate_HonorMAX)
                        {
                            for (int i = honorHistorys.Count - 1; i >= 0; --i)
                            {
                                if (honorHistorys[i].Uploader.GetTime() < _BetweenDate_HonorMAX)
                                {
                                    if (honorHistorys[i].Uploader.GetTime() > _BetweenDate_MIN)
                                    {
                                        if (honorHistorys[i].Data.LifetimeHighestRank > realLifeTimeHighestRank)
                                        {
                                            honorHistorys[i].Data.CurrentRank = realRank;
                                            honorHistorys[i].Data.CurrentRankProgress = realRankProgress;
                                            honorHistorys[i].Data.LifetimeHighestRank = realLifeTimeHighestRank;
                                            fixedHonor = true;
                                        }
                                    }
                                }
                                else
                                {
                                    realLifeTimeHighestRank = honorHistorys[i].Data.LifetimeHighestRank;
                                    realRankProgress = honorHistorys[i].Data.CurrentRankProgress;
                                    realRank = honorHistorys[i].Data.CurrentRank;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < honorHistorys.Count; ++i)
                            {
                                if (honorHistorys[i].Uploader.GetTime() > _BetweenDate_MIN)
                                {
                                    if (honorHistorys[i].Data.LifetimeHighestRank != realLifeTimeHighestRank || honorHistorys[i].Data.CurrentRank != realRank)
                                    {
                                        if (realRank != 0)
                                        {
                                            honorHistorys[i].Data.CurrentRank = realRank;
                                            honorHistorys[i].Data.CurrentRankProgress = realRankProgress;
                                            honorHistorys[i].Data.LifetimeHighestRank = realLifeTimeHighestRank;
                                            fixedHonor = true;
                                        }
                                        else
                                            triedFixedHonor = true;
                                    }
                                }
                                else
                                {
                                    realLifeTimeHighestRank = honorHistorys[i].Data.LifetimeHighestRank;
                                    realRankProgress = honorHistorys[i].Data.CurrentRankProgress;
                                    realRank = honorHistorys[i].Data.CurrentRank;
                                }
                            }
                        }
                    }
                }
                if (fixedHonor == true)
                    honorFixed.Add(playerHistory.Key);
                if (triedFixedHonor == true)
                    triedHonorFix.Add(playerHistory.Key);
                if (gearHistorys.Count > 0)
                {
                    List<int> haveItem = new List<int>();
                    if (gearHistorys.Last().Uploader.GetTime() > _BetweenDate_MIN)
                    {
                        for (int i = gearHistorys.Count - 1; i >= 0; --i)
                        {
                            if (gearHistorys[i].Uploader.GetTime() > _BetweenDate_MIN && gearHistorys[i].Uploader.GetTime() < _BetweenDate_ItemMax)
                            {
                                var removeItemsSlots = new List<ItemSlot>();
                                foreach (var item in gearHistorys[i].Data.Items)
                                {
                                    if (haveItem.Contains(item.Value.ItemID) == false)
                                    {
                                        try
                                        {
                                            var dropInfos = _ItemDropDatabase.GetDatabase()[item.Value.ItemID];
                                            foreach (var dropInfo in dropInfos)
                                            {
                                                int currBoss = (int)dropInfo.m_Boss;
                                                if ((currBoss >= (int)VF_RealmPlayersDatabase.WowBoss.PVPSetFirst && currBoss <= (int)VF_RealmPlayersDatabase.WowBoss.PVPSetLast)/*
                                                || (currBoss >= (int)VF_RealmPlayersDatabase.WowBoss.PVPOffsetFirst && currBoss <= (int)VF_RealmPlayersDatabase.WowBoss.PVPOffsetLast)*/)
                                                {
                                                    removeItemsSlots.Add(item.Key);
                                                    fixedGear = true;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        { }
                                    }
                                }
                                foreach (var removeSlot in removeItemsSlots)
                                {
                                    gearHistorys[i].Data.Items.Remove(removeSlot);
                                }
                            }
                            else
                            { 
                                foreach (var item in gearHistorys[i].Data.Items)
                                {
                                    if(haveItem.Contains(item.Value.ItemID) == false)
                                        haveItem.Add(item.Value.ItemID);
                                }
                            }
                        }
                    }
                }
                if (fixedGear == true)
                    gearFixed.Add(playerHistory.Key);
            }
            System.IO.File.WriteAllLines("Fix_" + _Name + "_FixedGear.txt", gearFixed.ToArray());
            System.IO.File.WriteAllLines("Fix_" + _Name + "_FixedHonor.txt", honorFixed.ToArray());
            System.IO.File.WriteAllLines("Fix_" + _Name + "_TriedFixHonor.txt", triedHonorFix.ToArray());
            VF_RealmPlayersDatabase.Utility.SaveSerialize(_DatabaseFile, historyData);
        }
        static void PurgeDatabase(string _Name, string _DatabaseFile, List<Tuple<int, DateTime?>> _PurgeConditions)
        {
            Dictionary<string, PlayerData.PlayerHistory> playerHistorys = null;
            VF_RealmPlayersDatabase.Utility.LoadSerialize(_DatabaseFile, out playerHistorys);
            
            Dictionary<string, PlayerData.Player> players = null;
            VF_RealmPlayersDatabase.Utility.LoadSerialize(System.IO.Path.GetDirectoryName(_DatabaseFile) + "\\PlayersData.dat", out players);

            Dictionary<string, PlayerData.ExtraData> playerExtraDatas = null;
            VF_RealmPlayersDatabase.Utility.LoadSerialize(System.IO.Path.GetDirectoryName(_DatabaseFile) + "\\PlayersExtraData.dat", out playerExtraDatas);

            List<string> affectedPlayers = new List<string>();

            foreach (var purgeCondition in _PurgeConditions)
            {
                int _ContributorID = purgeCondition.Item1;
                DateTime? _DateTime = purgeCondition.Item2;

                int contributorID = _ContributorID;
                List<string> removedPlayers = new List<string>();
                foreach (var historyData in playerHistorys)
                {
                    if (players.ContainsKey(historyData.Key))
                    {
                        int rollbackValue = 0;
                        if (_DateTime.HasValue)
                        {
                            rollbackValue = historyData.Value.RollbackPlayer(players[historyData.Key], contributorID, _DateTime.Value);
                        }
                        else
                        {
                            rollbackValue = historyData.Value.RollbackPlayer(players[historyData.Key], contributorID);
                        }
                        if (rollbackValue > 0)
                        {
                            if (historyData.Value.CharacterHistory.Count == 0)
                            {
                                Console.WriteLine("Removed character: " + historyData.Key);
                                removedPlayers.Add(historyData.Key);
                            }
                            else
                            {
                                Console.WriteLine("Rollback(" + rollbackValue + ") character: " + historyData.Key);
                            }
                            if (playerExtraDatas.ContainsKey(historyData.Key) == true)
                            {
                                var extraData = playerExtraDatas[historyData.Key];
                                try
                                {
                                    if (extraData != null 
                                        && extraData.Companions != null 
                                        && extraData.Mounts != null 
                                        && extraData.Pets != null)
                                    { 
                                        for (int i = 0; i < extraData.Companions.Count; ++i)
                                        {
                                            var companion = extraData.Companions[i];
                                            if (companion.Uploaders.Count((_Value) => _Value.GetContributorID() == contributorID && _Value.GetTime() > _DateTime.Value) == companion.Uploaders.Count())
                                            {
                                                Console.WriteLine("\tRemoved Companion: " + companion.Name);
                                                extraData.Companions.Remove(companion);
                                                --i;//Readjust i
                                                continue;
                                            }
                                        }
                                        for (int i = 0; i < extraData.Mounts.Count; ++i)
                                        {
                                            var mount = extraData.Mounts[i];
                                            if (mount == null)
                                            {
                                                Console.WriteLine("\tRemoved Null Mount");
                                                extraData.Mounts.Remove(mount);
                                                --i;//Readjust i
                                                continue;
                                            }
                                            if (mount.Uploaders.Count((_Value) => _Value.GetContributorID() == contributorID && _Value.GetTime() > _DateTime.Value) == mount.Uploaders.Count())
                                            {
                                                Console.WriteLine("\tRemoved Mount: " + mount.Mount);
                                                extraData.Mounts.Remove(mount);
                                                --i;//Readjust i
                                                continue;
                                            }
                                        }
                                        for (int i = 0; i < extraData.Pets.Count; ++i)
                                        {
                                            var pet = extraData.Pets[i];
                                            if (pet.Uploaders.Count((_Value) => _Value.GetContributorID() == contributorID && _Value.GetTime() > _DateTime.Value) == pet.Uploaders.Count())
                                            {
                                                Console.WriteLine("\tRemoved Pet: " + pet.Name);
                                                extraData.Pets.Remove(pet);
                                                --i;//Readjust i
                                                continue;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                            affectedPlayers.Add(historyData.Key + " = " + rollbackValue);
                        }
                        else if (rollbackValue < 0)
                        {
                            affectedPlayers.Add("Exception:" + historyData.Key + " = " + (-rollbackValue));
                        }
                    }
                }
                foreach (var playerName in removedPlayers)
                {
                    Console.WriteLine("Removed character: " + playerName);
                    players.Remove(playerName);
                    playerHistorys.Remove(playerName);
                    playerExtraDatas.Remove(playerName);
                }
            }
            affectedPlayers.Sort();
            Console.WriteLine("Done rollbacking characters");

            System.IO.File.WriteAllLines("Purge_" + _Name + "_AffectedPlayers.txt", affectedPlayers.ToArray());
            VF_RealmPlayersDatabase.Utility.SaveSerialize(_DatabaseFile, playerHistorys);
            VF_RealmPlayersDatabase.Utility.SaveSerialize(System.IO.Path.GetDirectoryName(_DatabaseFile) + "\\PlayersData.dat", players);
            VF_RealmPlayersDatabase.Utility.SaveSerialize(System.IO.Path.GetDirectoryName(_DatabaseFile) + "\\PlayersExtraData.dat", playerExtraDatas);
        }
        static void FixDatabaseIntegrity(string _Name, string _DatabaseFile)
        {
            Dictionary<string, PlayerData.PlayerHistory> playerHistorys = null;
            VF_RealmPlayersDatabase.Utility.LoadSerialize(_DatabaseFile, out playerHistorys);

            Dictionary<string, PlayerData.Player> players = null;
            VF_RealmPlayersDatabase.Utility.LoadSerialize(System.IO.Path.GetDirectoryName(_DatabaseFile) + "\\PlayersData.dat", out players);
            
            List<string> deletedPlayers = new List<string>();
            foreach (var historyData in playerHistorys)
            {
                if (historyData.Value.CharacterHistory.Count == 0)
                {
                    deletedPlayers.Add(historyData.Key);
                }
            }

            System.IO.File.WriteAllLines("Purge_" + _Name + "_DeletedPlayers.txt", deletedPlayers.ToArray());
            //VF_RealmPlayersDatabase.Utility.SaveSerialize(_DatabaseFile, playerHistorys);
            //VF_RealmPlayersDatabase.Utility.SaveSerialize(System.IO.Path.GetDirectoryName(_DatabaseFile) + "\\PlayersData.dat", players);
        }
        static void Main(string[] args)
        {
#if true
            //var contributor = VF_RealmPlayersDatabase.ContributorDB.GetContributor("***REMOVED***");
            //int contributorID = contributor.GetAsContributor().GetContributorID();
            PurgeDatabase("Nostalrius", "Database\\Nostalrius\\PlayersHistoryData_Now.dat"
                , new List<Tuple<int, DateTime?>> {
                    new Tuple<int, DateTime?>(0/****REMOVED****/, new DateTime(2017, 3, 16, 0, 0, 0))
                    , new Tuple<int, DateTime?>(0/****REMOVED****/, null)
                });
            //PurgeDatabase("Warsong", "Database\\Warsong\\PlayersHistoryData_Now.dat", "***REMOVED***");
            //PurgeDatabase("AlAkir", "Database\\Al_Akir\\PlayersHistoryData_Now.dat", "***REMOVED***");

            //FixDatabaseIntegrity("AlAkir", "Database\\Al_Akir\\PlayersHistoryData_Now.dat");
            Console.WriteLine("Done");
            Console.WriteLine("Done");
            Console.WriteLine("Done");
            while (true)
            {
                Console.Write(".");
                System.Threading.Thread.Sleep(10000);
            }
#elif PURGE_PVP_2014_01_16
            DateTime betweenDate_MIN = new DateTime(2014, 1, 16, 1, 0, 0);
            DateTime betweenDate_HonorMAX = new DateTime(2014, 1, 16, 15, 0, 0);
            DateTime betweenDate_ItemMAX = new DateTime(2014, 1, 17, 1, 0, 0);

            FixDatabaseIntegrity("EmeraldDream", "Database\\Emerald_Dream\\PlayersHistoryData_Now.dat");
            FixDatabaseIntegrity("Warsong", "Database\\Warsong\\PlayersHistoryData_Now.dat");
            FixDatabaseIntegrity("AlAkir", "Database\\Al_Akir\\PlayersHistoryData_Now.dat");
#else
            //ItemDropDatabase itemDropDatabase = new ItemDropDatabase("Database\\");
            //VF_RealmPlayersDatabase.ContributorHandler.Initialize("Database\\");

            //FixDatabase("EmeraldDream", "Database\\Emerald_Dream\\PlayersHistoryData_Now.dat", itemDropDatabase, betweenDate_MIN, betweenDate_HonorMAX, betweenDate_ItemMAX);
            //FixDatabase("Warsong", "Database\\Warsong\\PlayersHistoryData_Now.dat", itemDropDatabase, betweenDate_MIN, betweenDate_HonorMAX, betweenDate_ItemMAX);
            //FixDatabase("AlAkir", "Database\\Al_Akir\\PlayersHistoryData_Now.dat", itemDropDatabase, betweenDate_MIN, betweenDate_HonorMAX, betweenDate_ItemMAX);

            //PurgeDatabase("EmeraldDream", "Database\\Emerald_Dream\\PlayersHistoryData_Now.dat", "***REMOVED***");
            //PurgeDatabase("Warsong", "Database\\Warsong\\PlayersHistoryData_Now.dat", "***REMOVED***");
            //PurgeDatabase("AlAkir", "Database\\Al_Akir\\PlayersHistoryData_Now.dat", "***REMOVED***");

            //VF_RealmPlayersDatabase.ContributorHandler.RemoveContributor("***REMOVED***");
            //VF_RealmPlayersDatabase.ContributorHandler.Save("Database\\");
#endif

        }
    }
}
