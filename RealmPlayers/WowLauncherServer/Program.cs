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
            g_RDDatabaseHandler = new RDDatabaseHandler(g_RDDBFolder, g_RPPDatabaseHandler, DatabaseHandlerMode.Disabled);
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
            g_RDDatabaseHandler.Shutdown();
        }
    }
}
