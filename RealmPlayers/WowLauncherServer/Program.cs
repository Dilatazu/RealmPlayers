using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using VF_RealmPlayersDatabase;
using VF_RealmPlayersDatabase.PlayerData;
using UploaderCommunication = VF_RealmPlayersDatabase.UploaderCommunication;

namespace VF_WoWLauncherServer
{
    static class Program
    {
        public static string g_RPPDBFolder = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RPPDatabase\\";
        public static string g_RDDBFolder = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\";

        public static RPPDatabaseHandler g_RPPDatabaseHandler;
        public static RDDatabaseHandler g_RDDatabaseHandler;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool debugMode = false;
            if (System.IO.Directory.Exists(g_RPPDBFolder) == false)
            {
                g_RPPDBFolder = g_RPPDBFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                g_RDDBFolder = g_RDDBFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                AddonDatabaseService.g_AddonUploadDataFolder = AddonDatabaseService.g_AddonUploadDataFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                AddonDatabaseService.g_AddonUploadStatsFolder = AddonDatabaseService.g_AddonUploadStatsFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                RPPDatabaseHandler.g_AddonContributionsBackupFolder = RPPDatabaseHandler.g_AddonContributionsBackupFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);

                RDDatabaseHandler.g_AddonContributionsBackupFolder = RDDatabaseHandler.g_AddonContributionsBackupFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                debugMode = true;
            }
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            VF_WoWLauncher.ConsoleUtility.CreateConsole();

            /*Some Testing*/

            WowRealm[] ALL_REALMS = new WowRealm[] {
                WowRealm.Archangel,
                WowRealm.Kronos,
                WowRealm.NostalGeek,
                WowRealm.Nostalrius,
                WowRealm.NostalriusPVE,
                WowRealm.Emerald_Dream,
                WowRealm.Warsong,
                WowRealm.Al_Akir,
                WowRealm.Valkyrie,
                WowRealm.VanillaGaming,
                WowRealm.Nefarian,
                WowRealm.Rebirth,
                WowRealm.WarsongTBC };
            //List<WowRealm> realmsToTry = new List<WowRealm> { WowRealm.VanillaGaming, WowRealm.Al_Akir, WowRealm.Nefarian, WowRealm.NostalGeek, WowRealm.Nostalrius, WowRealm.Warsong, WowRealm.Archangel, WowRealm.NostalriusPVE, WowRealm.Kronos, WowRealm.Rebirth };
            foreach(var realmToTry in ALL_REALMS)
            {
                try
                {
                    const bool MIGRATE_DATABASE = false;
                    WowVersionEnum wowVersion = StaticValues.GetWowVersion(realmToTry);

                    RealmDatabase sqlRealm = null;

                    if (MIGRATE_DATABASE == false)
                    {
                        Logger.ConsoleWriteLine("Starting Load " + realmToTry.ToString() + " from SQL!!!");
                        sqlRealm = VF.SQLMigration.LoadRealmDatabase(realmToTry);
                        Logger.ConsoleWriteLine("Done Loading " + realmToTry.ToString() + " from SQL!!!");
                    }

                    RealmDatabase binRealm = new RealmDatabase(realmToTry);
                    Logger.ConsoleWriteLine("Started Loading " + realmToTry.ToString() + " from binary!!!");
                    binRealm.LoadDatabase("D:\\VF_RealmPlayersData\\RPPDatabase\\Database\\" + binRealm.Realm.ToString(), new DateTime(2012, 5, 1, 0, 0, 0));//new DateTime(2015, 9, 1, 0, 0, 0));//, 
                    Logger.ConsoleWriteLine("Loading...");
                    binRealm.WaitForLoad(RealmDatabase.LoadStatus.EverythingLoaded);
                    Logger.ConsoleWriteLine("Everything Loaded " + realmToTry.ToString() + " from binary!!!");
                    if (MIGRATE_DATABASE == true)
                    {
                        Logger.ConsoleWriteLine("Starting Saving " + realmToTry.ToString() + " to SQL!!!");
                        VF.SQLMigration.SQLIDCounters sqlCounters = new VF.SQLMigration.SQLIDCounters();
                        VF.SQLMigration.UploadRealmDatabase(ref sqlCounters, binRealm);
                        sqlCounters.UploadNewSequenceCounterValues();
                        Logger.ConsoleWriteLine("Done Saving " + realmToTry.ToString() + " to SQL!!!");
                    }
                    //else
                    {
                        if (sqlRealm == null)
                        {
                            Logger.ConsoleWriteLine("Starting Load " + realmToTry.ToString() + " from SQL!!!");
                            sqlRealm = VF.SQLMigration.LoadRealmDatabase(realmToTry);
                            Logger.ConsoleWriteLine("Done Loading " + realmToTry.ToString() + " from SQL!!!");
                        }

                        Logger.ConsoleWriteLine("Starting Comparing realm datas for " + realmToTry.ToString() + "!!!");
                        foreach (var binPlayer in binRealm.Players)
                        {
                            try
                            {
                                Player sqlPlayer = null;
                                if (sqlRealm.Players.TryGetValue(binPlayer.Key, out sqlPlayer) == false)
                                {
                                    sqlPlayer = null;
                                    Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" did not exist for SQLRealm!");
                                    continue;
                                }
                                ExtraData binExtraData = null;
                                if (binRealm.PlayersExtraData.TryGetValue(binPlayer.Key, out binExtraData) == false)
                                {
                                    binExtraData = null;
                                    //Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" did not have extra data in BINRealm!");
                                }
                                ExtraData sqlExtraData = null;
                                if (sqlRealm.PlayersExtraData.TryGetValue(binPlayer.Key, out sqlExtraData) == false)
                                {
                                    sqlExtraData = null;
                                    Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" did not have extra data in SQLRealm!");
                                }
                                PlayerHistory binHistory = null;
                                if (binRealm.PlayersHistory.TryGetValue(binPlayer.Key, out binHistory) == false)
                                {
                                    binHistory = null;
                                    Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" did not have history in BINRealm!");
                                }
                                PlayerHistory sqlHistory = null;
                                if (sqlRealm.PlayersHistory.TryGetValue(binPlayer.Key, out sqlHistory) == false)
                                {
                                    sqlHistory = null;
                                    Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" did not have history in SQLRealm!");
                                }

                                if (sqlPlayer != null && binPlayer.Value != null)
                                {
                                    if (binPlayer.Value.Guild.IsSame(sqlPlayer.Guild) == false)
                                    {
                                        Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Guild data was not same!");
                                    }
                                    if (binPlayer.Value.Character.IsSame(sqlPlayer.Character) == false)
                                    {
                                        Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Character data was not same!");
                                    }
                                    if (binPlayer.Value.Honor.IsSame(sqlPlayer.Honor, wowVersion) == false)
                                    {
                                        Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Honor data was not same!");
                                    }
                                    if (binPlayer.Value.Gear.IsSame(sqlPlayer.Gear) == false)
                                    {
                                        string gearItemsDebugInfo = "";
                                        List<ItemSlot> diffCheckedSlots = new List<ItemSlot>();
                                        foreach (ItemSlot slot in Enum.GetValues(typeof(ItemSlot)))
                                        {
                                            ItemInfo binItem = null;
                                            ItemInfo sqlItem = null;
                                            if (binPlayer.Value.Gear.Items.TryGetValue(slot, out binItem) == false) binItem = null;
                                            if (sqlPlayer.Gear.Items.TryGetValue(slot, out sqlItem) == false) sqlItem = null;

                                            if (binItem == null && sqlItem != null)
                                            {
                                                //lets just assume this is not an error!
                                                //gearItemsDebugInfo += "\n\tSQL" + sqlItem.GetAsString() + "!=BIN{null}";
                                            }
                                            else if (binItem != null && sqlItem == null)
                                            {
                                                gearItemsDebugInfo += "\n\tSQL{null}!=" +
                                                    "BIN" + binItem.GetAsString();
                                            }
                                            else if (binItem != null && sqlItem != null)
                                            {
                                                if (sqlItem.IsSame(binItem) == false)
                                                {
                                                    gearItemsDebugInfo += "\n\tSQL" + sqlItem.GetAsString() + "!=" +
                                                        "BIN" + binItem.GetAsString();
                                                }
                                            }
                                            else
                                            {
                                                if (binItem != null || sqlItem != null)
                                                {
                                                    Logger.ConsoleWriteLine("ERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\n, this is unexpected and should never happen!\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR");
                                                }
                                            }
                                        }
                                        if(gearItemsDebugInfo != "")
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Gear data was not same! GearDifferences:" + gearItemsDebugInfo);
                                        }
                                    }
                                    if (binPlayer.Value.Arena != null)
                                    {
                                        if(sqlPlayer.Arena != null)
                                        {
                                            if(binPlayer.Value.Arena.IsSame(sqlPlayer.Arena) == false) Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Arena data was not same!");
                                        }
                                        else if (sqlPlayer.Arena == null)
                                        {
                                            if(binPlayer.Value.Arena.Team2v2 != null) Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Arena data was not same! BIN Team2v2 != null");
                                            if(binPlayer.Value.Arena.Team3v3 != null) Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Arena data was not same! BIN Team3v3 != null");
                                            if(binPlayer.Value.Arena.Team5v5 != null) Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Arena data was not same! BIN Team5v5 != null");
                                        }
                                    }
                                    if (binPlayer.Value.TalentPointsData != null && binPlayer.Value.TalentPointsData != sqlPlayer.TalentPointsData)
                                    {
                                        if(sqlPlayer.TalentPointsData == null && binPlayer.Value.TalentPointsData == "")
                                        {
                                            //Skip this case, we dont care, null == ""
                                        }
                                        else
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Talents data was not same!");
                                        }
                                    }
                                }
                                if (sqlHistory != null && binHistory != null)
                                {
                                    if (sqlHistory.CharacterHistory.Count > binHistory.CharacterHistory.Count || sqlHistory.CharacterHistory.Count == 0)
                                    {
                                        Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Character history was not same! SQLCount(" + sqlHistory.CharacterHistory.Count + ") > BINCount(" + binHistory.CharacterHistory.Count + ")\n");
                                    }
                                    if (sqlHistory.CharacterHistory.Count > 0)
                                    {
                                        string charHistoryDebugInfo = "";
                                        foreach (var binItem in binHistory.CharacterHistory)
                                        {
                                            var sqlItem = sqlHistory.GetCharacterItemAtTime(binItem.Uploader.GetTime());
                                            if (sqlItem.Data.IsSame(binItem.Data) == false)
                                            {
                                                charHistoryDebugInfo += "\tSQL" + sqlItem.GetAsString() + " != BIN" + binItem.GetAsString() + "\n";
                                            }
                                        }
                                        if (charHistoryDebugInfo != "")
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Character history was not same!\n" + charHistoryDebugInfo);
                                        }
                                    }

                                    if (sqlHistory.HonorHistory.Count > binHistory.HonorHistory.Count || sqlHistory.HonorHistory.Count == 0)
                                    {
                                        Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Honor history was not same! SQLCount(" + sqlHistory.HonorHistory.Count + ") > BINCount(" + binHistory.HonorHistory.Count + ")");
                                    }
                                    if (sqlHistory.HonorHistory.Count > 0)
                                    {
                                        string honorHistoryDebugInfo = "";
                                        foreach (var binItem in binHistory.HonorHistory)
                                        {
                                            var sqlItem = sqlHistory.GetHonorItemAtTime(binItem.Uploader.GetTime());
                                            if (sqlItem.Data.IsSame(binItem.Data, wowVersion) == false)
                                            {
                                                honorHistoryDebugInfo += "\tSQL" + sqlItem.GetAsString() + " != BIN" + binItem.GetAsString() + "\n";
                                            }
                                        }
                                        if (honorHistoryDebugInfo != "")
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Honor history was not same!\n" + honorHistoryDebugInfo);
                                        }
                                    }
                                    if (sqlHistory.GearHistory.Count > binHistory.GearHistory.Count || sqlHistory.GearHistory.Count == 0)
                                    {
                                        if(binHistory.GearHistory.Count != 0)
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Gear history was not same! SQLCount(" + sqlHistory.GearHistory.Count + ") > BINCount(" + binHistory.GearHistory.Count + ")");
                                        }
                                    }
                                    if (sqlHistory.GearHistory.Count > 0)
                                    {
                                        string gearHistoryDebugInfo = "";
                                        foreach (var binItem in binHistory.GearHistory)
                                        {
                                            var sqlItem = sqlHistory.GetGearItemAtTime(binItem.Uploader.GetTime());
                                            if (sqlItem.Data.IsSame(binItem.Data) == false)
                                            {
                                                TimeSpan closestTimeFound = TimeSpan.MaxValue;
                                                foreach (var sqlItem2 in sqlHistory.GearHistory)
                                                {
                                                    if (sqlItem2.Data.IsSame(binItem.Data) == true)
                                                    {
                                                        TimeSpan thisTimeSpan = (sqlItem2.Uploader.GetTime() - binItem.Uploader.GetTime());
                                                        if (thisTimeSpan.TotalSeconds < 0)
                                                            thisTimeSpan = thisTimeSpan.Negate();

                                                        if (thisTimeSpan < closestTimeFound)
                                                            closestTimeFound = thisTimeSpan;
                                                    }
                                                }
                                                if (closestTimeFound == TimeSpan.MaxValue)
                                                {
                                                    gearHistoryDebugInfo += "\n\tSQL != BIN DiffString: " + sqlItem.GetDiffString(binItem);
                                                }
                                                else
                                                {
                                                    if (closestTimeFound.TotalSeconds > 5)
                                                    {
                                                        if (closestTimeFound.TotalHours <= 1)
                                                        {
                                                            gearHistoryDebugInfo += "\n\tSQL != BIN TIME SECDIFF: " + (int)closestTimeFound.TotalSeconds + " seconds";
                                                        }
                                                        else if (closestTimeFound.TotalDays <= 1)
                                                        {
                                                            gearHistoryDebugInfo += "\n\tSQL != BIN TIME MINDIFF: " + (int)closestTimeFound.TotalMinutes + " minutes";
                                                        }
                                                        else
                                                        {
                                                            gearHistoryDebugInfo += "\n\tSQL != BIN TIME HOURDIFF: " + (int)closestTimeFound.TotalHours + " hours";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (gearHistoryDebugInfo != "")
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Gear history was not same!" + (gearHistoryDebugInfo.Count((_V) => _V == '\n') == 1 ? gearHistoryDebugInfo.Substring(2) : gearHistoryDebugInfo));
                                        }
                                    }
                                    if (sqlHistory.GuildHistory.Count > binHistory.GuildHistory.Count || sqlHistory.GuildHistory.Count == 0)
                                    {
                                        Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Guild history was not same! SQLCount(" + sqlHistory.GuildHistory.Count + ") > BINCount(" + binHistory.GuildHistory.Count + ")");
                                    }
                                    if (sqlHistory.GuildHistory.Count > 0)
                                    {
                                        string guildHistoryDebugInfo = "";
                                        foreach (var binItem in binHistory.GuildHistory)
                                        {
                                            var sqlItem = sqlHistory.GetGuildItemAtTime(binItem.Uploader.GetTime());
                                            if (sqlItem.Data.IsSame(binItem.Data) == false)
                                            {
                                                guildHistoryDebugInfo += "\tSQL" + sqlItem.GetAsString() + " != BIN" + binItem.GetAsString() + "\n";
                                            }
                                        }
                                        if (guildHistoryDebugInfo != "")
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Guild history was not same!\n" + guildHistoryDebugInfo);
                                        }
                                    }
                                    if (sqlHistory.ArenaHistory != null && binHistory.ArenaHistory != null)
                                    {
                                        if (sqlHistory.ArenaHistory.Count > binHistory.ArenaHistory.Count || sqlHistory.ArenaHistory.Count == 0 && binHistory.ArenaHistory.Count != 0)
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Arena history was not same! SQLCount(" + sqlHistory.ArenaHistory.Count + ") > BINCount(" + binHistory.ArenaHistory.Count + ")");
                                        }
                                        if (sqlHistory.ArenaHistory.Count > 0)
                                        {
                                            string arenaHistoryDebugInfo = "";
                                            foreach (var binItem in binHistory.ArenaHistory)
                                            {
                                                var sqlItem = sqlHistory.GetArenaItemAtTime(binItem.Uploader.GetTime());
                                                if (sqlItem.Data.IsSame(binItem.Data) == false)
                                                {
                                                    arenaHistoryDebugInfo += "\tSQL" + sqlItem.GetAsString() + " != BIN" + binItem.GetAsString() + "\n";
                                                }
                                            }
                                            if (arenaHistoryDebugInfo != "")
                                            {
                                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Arena history was not same!\n" + arenaHistoryDebugInfo);
                                            }
                                        }
                                    }
                                    else if (sqlHistory.ArenaHistory != null && binHistory.ArenaHistory == null)
                                    {
                                        Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Arena history was not same! SQLCount(" + sqlHistory.ArenaHistory.Count + ") != BIN(null)");
                                    }
                                    else if (sqlHistory.ArenaHistory == null && binHistory.ArenaHistory != null)
                                    {
                                        foreach(var binItem in binHistory.ArenaHistory)
                                        {
                                            if(binItem.Data != null && (binItem.Data.Team2v2 != null || binItem.Data.Team3v3 != null || binItem.Data.Team5v5 != null))
                                            {
                                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Arena history was not same! SQL(null) != BINCount(" + binHistory.ArenaHistory.Count + ")");
                                                break;
                                            }
                                        }
                                    }

                                    if (sqlHistory.TalentsHistory != null && binHistory.TalentsHistory != null)
                                    {
                                        if (sqlHistory.TalentsHistory.Count > binHistory.TalentsHistory.Count || sqlHistory.TalentsHistory.Count == 0 && binHistory.TalentsHistory.Count != 0)
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Talents history was not same! SQLCount(" + sqlHistory.TalentsHistory.Count + ") > BINCount(" + binHistory.TalentsHistory.Count + ")");
                                        }
                                        if (sqlHistory.TalentsHistory.Count > 0)
                                        {
                                            string talentsHistoryDebugInfo = "";
                                            foreach (var binItem in binHistory.TalentsHistory)
                                            {
                                                var sqlItem = sqlHistory.GetTalentsItemAtTime(binItem.Uploader.GetTime());
                                                if (sqlItem.Data != binItem.Data)
                                                {
                                                    if (binItem.Data == "")
                                                    {
                                                        //Ignore this case...
                                                    }
                                                    else
                                                    {
                                                        talentsHistoryDebugInfo += "\n\tSQL" + sqlItem.GetAsString() + " != BIN" + binItem.GetAsString();
                                                    }
                                                }
                                            }
                                            if (talentsHistoryDebugInfo != "")
                                            {
                                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Talents history was not same! " + talentsHistoryDebugInfo);
                                            }
                                        }
                                    }
                                    else if ((sqlHistory.TalentsHistory != null && sqlHistory.TalentsHistory.Count > 0) && binHistory.TalentsHistory == null)
                                    {
                                        Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Talents history was not same! SQLCount(" + sqlHistory.TalentsHistory.Count + ") != BIN(null)");
                                    }
                                    else if (sqlHistory.TalentsHistory == null && (binHistory.TalentsHistory != null && binHistory.TalentsHistory.Count > 0))
                                    {
                                        if (binHistory.TalentsHistory.Count == 1 && binHistory.TalentsHistory[0].Data == "")
                                        {
                                            //Skip this case. It basically means there was no talentshistory!
                                        }
                                        else
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Talents history was not same! SQL(null) != BINCount(" + binHistory.TalentsHistory.Count + ")");
                                        }
                                    }
                                }
                                if (sqlExtraData != null && binExtraData != null)
                                {
                                    foreach (var binMount in binExtraData.Mounts)
                                    {
                                        var sqlMountIndex = sqlExtraData.Mounts.FindIndex((_V) => _V.Mount == binMount.Mount);
                                        if (sqlMountIndex != -1)
                                        {
                                            var sqlMount = sqlExtraData.Mounts[sqlMountIndex];
                                            foreach (var binUploader in binMount.Uploaders)
                                            {
                                                int foundIndex = sqlMount.Uploaders.FindIndex((_V) => _V.Equals(binUploader));
                                                if (foundIndex == -1)
                                                {
                                                    Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" mount \"" + binMount.Mount + "\" did not have same uploaders in SQL!");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Did not have mount \"" + binMount.Mount + "\" in SQL!");
                                        }
                                    }

                                    foreach (var binPet in binExtraData.Pets)
                                    {
                                        var sqlPetIndex = sqlExtraData.Pets.FindIndex((_V) => _V.Name == binPet.Name && _V.Level == binPet.Level && _V.CreatureFamily == binPet.CreatureFamily && _V.CreatureType == binPet.CreatureType);
                                        if (sqlPetIndex != -1)
                                        {
                                            var sqlPet = sqlExtraData.Pets[sqlPetIndex];
                                            foreach (var binUploader in binPet.Uploaders)
                                            {
                                                int foundIndex = sqlPet.Uploaders.FindIndex((_V) => _V.Equals(binUploader));
                                                if (foundIndex == -1)
                                                {
                                                    Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" pet \"" + binPet.Name + "\" did not have same uploaders in SQL!");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Did not have pet \"" + binPet.Name + "\" in SQL!");
                                        }
                                    }

                                    foreach (var binCompanion in binExtraData.Companions)
                                    {
                                        var sqlCompanionIndex = sqlExtraData.Companions.FindIndex((_V) => _V.Name == binCompanion.Name && _V.Level == binCompanion.Level);
                                        if (sqlCompanionIndex != -1)
                                        {
                                            var sqlCompanion = sqlExtraData.Companions[sqlCompanionIndex];
                                            foreach (var binUploader in binCompanion.Uploaders)
                                            {
                                                int foundIndex = sqlCompanion.Uploaders.FindIndex((_V) => _V.Equals(binUploader));
                                                if (foundIndex == -1)
                                                {
                                                    Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" companion \"" + binCompanion.Name + "\" did not have same uploaders in SQL!");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Did not have companion \"" + binCompanion.Name + "\" in SQL!");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.ConsoleWriteLine("EXCEPTION OCCURED for \"" + binPlayer.Key + "\"\n----------------------\n" + ex.ToString());
                            }
                        }
                        Logger.ConsoleWriteLine("Done comparing realm datas for " + realmToTry.ToString() + "!!!");
                    }
                    Logger.ConsoleWriteLine("Everything Done for " + realmToTry.ToString() + "!!!");
                }
                catch (Exception ex)
                {
                    Logger.ConsoleWriteLine("EXCEPTION OCCURED\n----------------------\n" + ex.ToString());
                }
            }

            Logger.ConsoleWriteLine("Everything Done!!!");
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
            return;
            /*Some Testing*/

            //if (debugMode == false)
            {
                Console.WriteLine("Waiting for ContributorDB to load");
                ContributorDB.Initialize(debugMode);
                while (ContributorDB.GetMongoDB() == null)
                {
                    ContributorDB.Initialize(debugMode);
                    System.Threading.Thread.Sleep(100);
                    Console.Write(".");
                }
                Console.WriteLine("ContributorDB loaded!");
            }

            //VF_RealmPlayersDatabase.Deprecated.ContributorHandler.Initialize(g_RPPDBFolder + "Database\\");
            g_RPPDatabaseHandler = new RPPDatabaseHandler(g_RPPDBFolder);
            g_RDDatabaseHandler = new RDDatabaseHandler(g_RDDBFolder, g_RPPDatabaseHandler);
            AddonDatabaseService.HandleUnhandledFiles("VF_RealmPlayers");
            AddonDatabaseService.HandleUnhandledFiles("VF_RaidDamage");
            AddonDatabaseService.HandleUnhandledFiles("VF_RealmPlayersTBC");
            AddonDatabaseService.HandleUnhandledFiles("VF_RaidStatsTBC");
            try
            {
                Application.Run(new MainWindow());
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                g_RDDatabaseHandler.Shutdown();
                g_RPPDatabaseHandler.Shutdown();
            }
        }
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            g_RPPDatabaseHandler.Shutdown();
        }
    }
}
