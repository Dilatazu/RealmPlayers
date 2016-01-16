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
            //try
            {
                RealmDatabase binRealm = new RealmDatabase(WowRealm.Al_Akir);
                Logger.ConsoleWriteLine("Started Loading!!!");
                binRealm.LoadDatabase("D:\\VF_RealmPlayersData\\RPPDatabase\\Database\\Al_Akir", new DateTime(2012, 5, 1, 0, 0, 0));//new DateTime(2015, 9, 1, 0, 0, 0));//, 
                Logger.ConsoleWriteLine("Loading...");
                binRealm.WaitForLoad(RealmDatabase.LoadStatus.EverythingLoaded);
                Logger.ConsoleWriteLine("Everything Loaded!!!");
                if(false)
                {
                    Logger.ConsoleWriteLine("Starting Saving to SQL!!!");
                    SQLMigration.SaveFakeContributorData();
                    SQLMigration.SaveRealmDatabase(binRealm);
                    Logger.ConsoleWriteLine("Done Saving to SQL!!!");
                }
                else
                {
                    Logger.ConsoleWriteLine("Starting Load from SQL!!!");
                    RealmDatabase sqlRealm = SQLMigration.LoadRealmDatabase(WowRealm.Al_Akir);
                    Logger.ConsoleWriteLine("Done Loading from SQL!!!");

                    Logger.ConsoleWriteLine("Starting Comparing realm datas!!!");
                    foreach (var binPlayer in binRealm.Players)
                    {
                        try
                        {
                            PlayerHistory binHistory = null;
                            if(binRealm.PlayersHistory.TryGetValue(binPlayer.Key, out binHistory) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" did not have history in BINRealm!");
                                continue;
                            }
                            Player sqlPlayer = null;
                            if(sqlRealm.Players.TryGetValue(binPlayer.Key, out sqlPlayer) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" did not exist for SQLRealm!");
                                continue;
                            }
                            PlayerHistory sqlHistory = null;
                            if (sqlRealm.PlayersHistory.TryGetValue(binPlayer.Key, out sqlHistory) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" did not have history in SQLRealm!");
                                continue;
                            }
                            if (binPlayer.Value.Guild.IsSame(sqlPlayer.Guild) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Guild data was not same!");
                            }
                            if (binPlayer.Value.Character.IsSame(sqlPlayer.Character) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Character data was not same!");
                            }
                            if (binPlayer.Value.Honor.IsSame(sqlPlayer.Honor) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Honor data was not same!");
                            }
                            if (binPlayer.Value.Gear.IsSame(sqlPlayer.Gear) == false)
                            {
                                string gearItemsDebugInfo = "GearDifferences:\n";
                                List<ItemSlot> diffCheckedSlots = new List<ItemSlot>();
                                foreach(ItemSlot slot in Enum.GetValues(typeof(ItemSlot)))
                                {
                                    ItemInfo binItem = null;
                                    ItemInfo sqlItem = null;
                                    if (binPlayer.Value.Gear.Items.TryGetValue(slot, out binItem) == false) binItem = null;
                                    if (sqlPlayer.Gear.Items.TryGetValue(slot, out sqlItem) == false) sqlItem = null;

                                    if (binItem == null && sqlItem != null)
                                    {
                                        gearItemsDebugInfo += "\tSQL{" + sqlItem.Slot.ToString() + ", " + sqlItem.ItemID + ", " + sqlItem.EnchantID + ", " + sqlItem.SuffixID + ", " + sqlItem.UniqueID + "}!=" +
                                            "BIN{null}\n";
                                    }
                                    else if(binItem != null && sqlItem == null)
                                    {
                                        gearItemsDebugInfo += "\tSQL{null}!=" +
                                            "BIN{" + binItem.Slot.ToString() + ", " + binItem.ItemID + ", " + binItem.EnchantID + ", " + binItem.SuffixID + ", " + binItem.UniqueID + "}\n";
                                    }
                                    else if(binItem != null && sqlItem != null)
                                    {
                                        if(sqlItem.IsSame(binItem) == false)
                                        {
                                            gearItemsDebugInfo += "\tSQL{" + sqlItem.Slot.ToString() + ", " + sqlItem.ItemID + ", " + sqlItem.EnchantID + ", " + sqlItem.SuffixID + ", " + sqlItem.UniqueID + "}!=" +
                                                "BIN{" + binItem.Slot.ToString() + ", " + binItem.ItemID + ", " + binItem.EnchantID + ", " + binItem.SuffixID + ", " + binItem.UniqueID + "}\n";
                                        }
                                    }
                                    else
                                    {
                                        if(binItem != null || sqlItem != null)
                                        {
                                            Logger.ConsoleWriteLine("ERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\n, this is unexpected and should never happen!\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR");
                                        }
                                    }
                                }
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Gear data was not same!\n" + gearItemsDebugInfo);
                            }

                            if (sqlHistory.CharacterHistory.Count != binHistory.CharacterHistory.Count)
                            {
                                string charHistoryDebugInfo = "";
                                for (int i = 0; i < sqlHistory.CharacterHistory.Count; ++i)
                                {
                                    charHistoryDebugInfo += "\tSQL[" + i + "]=" + sqlHistory.CharacterHistory[i].GetAsString() + "\n";
                                }
                                for (int i = 0; i < binHistory.CharacterHistory.Count; ++i)
                                {
                                    charHistoryDebugInfo += "\tBIN[" + i + "]=" + binHistory.CharacterHistory[i].GetAsString() + "\n";
                                }
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Character history was not same! SQLCount(" + sqlHistory.CharacterHistory.Count + ") != BINCount(" + binHistory.CharacterHistory.Count + ")\n" + charHistoryDebugInfo);
                            }
                            for(int i = 0; i < sqlHistory.CharacterHistory.Count; ++i)
                            {
                                if (i >= binHistory.CharacterHistory.Count)
                                    break;

                                var sqlChar = sqlHistory.CharacterHistory[i];
                                var binChar = binHistory.CharacterHistory[i];

                                if(sqlChar.Uploader.GetTime() != binChar.Uploader.GetTime())
                                {
                                    Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Character history item[" + i + "] was not same update datetime! SQL(" + sqlChar.Uploader.GetTime() + ") != BIN(" + binChar.Uploader.GetTime() + ")");
                                }
                                else if (sqlChar.Data.IsSame(binChar.Data) == false)
                                {
                                    Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Character history item[" + i + "] was not same update datetime! SQL" + sqlChar.Data.GetAsString() + " != BIN" + binChar.Data.GetAsString() + "");
                                }
                            }
                            if (sqlHistory.HonorHistory.Count != binHistory.HonorHistory.Count)
                            {
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Honor history was not same! SQLCount(" + sqlHistory.HonorHistory.Count + ") != BINCount(" + binHistory.HonorHistory.Count + ")");
                            }
                            if (sqlHistory.GearHistory.Count != binHistory.GearHistory.Count)
                            {
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Gear history was not same! SQLCount(" + sqlHistory.GearHistory.Count + ") != BINCount(" + binHistory.GearHistory.Count + ")");
                            }
                            if (sqlHistory.GuildHistory.Count != binHistory.GuildHistory.Count)
                            {
                                Logger.ConsoleWriteLine("\"" + binPlayer.Key + "\" Guild history was not same! SQLCount(" + sqlHistory.GuildHistory.Count + ") != BINCount(" + binHistory.GuildHistory.Count + ")");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.ConsoleWriteLine("EXCEPTION OCCURED for \"" + binPlayer.Key + "\"\n----------------------\n" + ex.ToString());
                        }
                    }
                    Logger.ConsoleWriteLine("Done comparing realm datas!!!");
                }
                Logger.ConsoleWriteLine("Everything Done!!!");
            }
            //catch (Exception ex)
            {
                //Console.WriteLine("EXCEPTION OCCURED\n----------------------\n" + ex.ToString());
            }
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
            }
            return;
            /*Some Testing*/

            if (debugMode == false)
            {
                Console.WriteLine("Waiting for ContributorDB to load");
                while (ContributorDB.GetMongoDB() == null)
                {
                    ContributorDB.Initialize();
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
