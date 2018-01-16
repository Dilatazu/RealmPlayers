using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VF_RealmPlayersDatabase;
using UploaderCommunication = VF_RealmPlayersDatabase.UploaderCommunication;
using ICSharpCode.SharpZipLib.Zip;

using System.Collections.Concurrent;

namespace VF_WoWLauncherServer
{
    class RPPDatabaseHandler
    {
        public string m_RPPDBFolder;

        Database m_Database = null;
        UploaderCommunication.RPPCommunicator m_Communicator = null;
        ConcurrentQueue<RPPContribution> m_NewContributions = new ConcurrentQueue<RPPContribution>();
        List<RPPContribution> m_AddedContributions = new List<RPPContribution>();
        List<RPPContribution> m_ProblemContributions = new List<RPPContribution>();

        WowRealm[] m_RealmsInUse;

        object m_LockObject = new object();
        System.Threading.Thread m_MainThread = null;
        public RPPDatabaseHandler(string _RPPDBFolder)
        {
            List<WowRealm> realmsInUse = new List<WowRealm>();
            foreach(var realm in Database.ALL_REALMS)
            {
                if (Database.SKIP_REALM_TAGS.Contains(realm))
                    continue;
                if (StaticValues.DeadRealms.Contains(realm) == false)
                {
                    realmsInUse.Add(realm);
                }
            }
            m_RealmsInUse = realmsInUse.ToArray();
            m_RPPDBFolder = _RPPDBFolder;
            if (ItemDropDatabase.DatabaseExists(m_RPPDBFolder + "Database\\") == false)
            {
                ItemDropDatabase itemDropDatabase = new ItemDropDatabase(m_RPPDBFolder + "Database\\");
            }
            m_Communicator = new UploaderCommunication.RPPCommunicator(18374);
            m_Database = new Database(m_RPPDBFolder + "Database\\", null, m_RealmsInUse);
            m_MainThread = new System.Threading.Thread(MainThread);
            m_MainThread.Start();
        }
        public void AddContribution(RPPContribution _Contribution)
        {
            m_NewContributions.Enqueue(_Contribution);
        }
        public object GetLockObject()
        {
            return m_LockObject;
        }
        public RealmDatabase GetRealmDB(WowRealm _Realm)
        {
            return m_Database.GetRealm(_Realm);
        }
        public void Pause()
        {
            System.Threading.Monitor.Enter(m_LockObject);
        }
        public void Continue()
        {
            System.Threading.Monitor.Exit(m_LockObject);
        }
        public void Shutdown()
        {
            Logger.ConsoleWriteLine("Armory: Shutdown for RPPDatabaseHandler is triggered!", ConsoleColor.White);
            m_MainThread = null;
            if (m_Communicator != null)
            {
                m_Communicator.Shutdown();
                try
                {
                    ProcessData();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                m_Communicator.WaitForClosed();
                try
                {
                    ProcessData();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                m_Communicator = null;
            }
        }
        private void MainThread()
        {
            Logger.ConsoleWriteLine("Armory: MainThread for RPPDatabaseHandler is started!", ConsoleColor.Green);
            while (m_Communicator != null && m_MainThread != null)
            {
                bool processedData = false;
                try
                {
                    processedData = ProcessData();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                if(m_MainThread != null)
                {
                    if(processedData == true)
                    {
                        Logger.ConsoleWriteLine("Armory: Nothing to do, sleeping 30 seconds...", ConsoleColor.DarkYellow, false);
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        ConsoleColor prevColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(".");
                        Console.ForegroundColor = prevColor;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    System.Threading.Thread.Sleep(30000);
                }
            }
            while (true)
            {
                Logger.ConsoleWriteLine("Armory: MainThread for RPPDatabaseHandler is exited!", ConsoleColor.Green);
                System.Threading.Thread.Sleep(30000);
            }
        }
        bool ProcessData()
        {
            bool processedData = false;
            if (m_Communicator == null)
                return processedData;
            lock (m_LockObject)
            {
                m_AddedContributions.Clear();
                m_ProblemContributions.Clear();
                RPPContribution data;
                while (m_NewContributions.TryDequeue(out data))
                {
                    processedData = true;
                    if (m_Database.AddContribution(data) == true)
                    {
                        m_AddedContributions.Add(data);
                    }
                    else
                    {
                        m_ProblemContributions.Add(data);
                    }
                }
                while (m_Communicator.GetNextRPPContribution(out data))
                {
                    processedData = true;
                    if (m_Database.AddContribution(data) == true)
                    {
                        m_AddedContributions.Add(data);
                    }
                    else
                    {
                        m_ProblemContributions.Add(data);
                    }
                }
                if (m_AddedContributions.Count > 0)
                {
                    m_Database.SaveRealmDatabases(m_RPPDBFolder + "Database\\");
                }
                foreach (RPPContribution contribution in m_AddedContributions)
                {
                    BackupRPPContribution(contribution.GetFilename(), _Problematic: false);
                }
                foreach (RPPContribution contribution in m_ProblemContributions)
                {
                    BackupRPPContribution(contribution.GetFilename(), _Problematic: true);
                }
                m_Database.Cleanup();
            }
            if (m_Communicator == null || m_MainThread == null)
                return processedData;
            UpdateSummaryDatabases();
            UpdatePlayerSummaryDatabase();
            return processedData;
        }
        public void TriggerSaveDatabases()
        {
            m_Database.SaveRealmDatabases(m_RPPDBFolder + "Database\\");
        }
        public static string g_AddonContributionsBackupFolder = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_DataServer\\AddonContributionsBackup\\";
        void BackupRPPContribution(string _Filename, bool _Problematic = false)
        {
            try
            {
                if (System.IO.File.Exists(_Filename) == false)
                {
                    Logger.ConsoleWriteLine("Armory: Could not backup file: " + _Filename + ", it does not exist!", ConsoleColor.Red);
                    return;
                }
                string zipFileName = "";
                string zipFullFilePath = "";

                if (_Filename.Contains("VF_RealmPlayersTBC") == true)
                {
                    if(_Problematic == true)
                    {
                        zipFileName = "VF_RealmPlayersTBC_ProblemContributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    }
                    else
                    {
                        zipFileName = "VF_RealmPlayersTBC_Contributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    }
                    zipFullFilePath = g_AddonContributionsBackupFolder + "VF_RealmPlayersTBC\\" + zipFileName;
                }
                else
                {
                    if (_Problematic == true)
                    {
                        zipFileName = "VF_RealmPlayers_ProblemContributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    }
                    else
                    {
                        zipFileName = "VF_RealmPlayers_Contributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    }
                    zipFullFilePath = g_AddonContributionsBackupFolder + "VF_RealmPlayers\\" + zipFileName;
                }

                Utility.AssertFilePath(zipFullFilePath);
                ZipFile zipFile;
                if (System.IO.File.Exists(zipFullFilePath) == true)
                    zipFile = new ZipFile(zipFullFilePath);
                else
                    zipFile = ZipFile.Create(zipFullFilePath);

                zipFile.BeginUpdate();

                zipFile.Add(_Filename, _Filename.Split('\\', '/').Last());

                zipFile.CommitUpdate();
                zipFile.Close();
                System.IO.File.Delete(_Filename);
                Logger.ConsoleWriteLine("Armory: Successfull backup of file: " + _Filename + " into " + zipFileName);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Logger.ConsoleWriteLine("Armory: Well(RP-ZIP), if this happens give up...", ConsoleColor.Red);
                while (true)
                {
                    System.Threading.Thread.Sleep(5000);
                    Console.Write("ASSESS DAMAGE(RP-ZIP)! ", ConsoleColor.Red);
                }
            }
        }

        private DateTime m_LastSummaryDatabaseUpdateTime = DateTime.UtcNow.AddMinutes(-15);
        public void UpdateSummaryDatabases(bool _Force = false)
        {
            lock (m_LockObject)
            {
                if (_Force == true || ((DateTime.UtcNow - m_LastSummaryDatabaseUpdateTime).TotalHours > 4
                    && DateTime.UtcNow.Hour > 2 && DateTime.UtcNow.Hour < 16))
                {
                    Logger.ConsoleWriteLine("Armory: Started Updating Summary Databases", ConsoleColor.Green);
                    GC.Collect();
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    Database tempDatabase = new Database(m_RPPDBFolder + "Database\\", null, m_RealmsInUse);
                    tempDatabase.PurgeRealmDBs(true, true);

                    _UpdateGuildSummaryDatabase(tempDatabase);
                    Logger.ConsoleWriteLine("Armory: Done Updating Guild Summary Database", ConsoleColor.Green);
                    _UpdateItemSummaryDatabase(tempDatabase);
                    Logger.ConsoleWriteLine("Armory: Done Updating Summary Databases, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
                }
            }
            GC.Collect();
        }
        public void MigrateItemSummaryDatabaseToSQL()
        {
            lock (m_LockObject)
            {
                var itemSummaryDB = VF_RPDatabase.ItemSummaryDatabase.LoadSummaryDatabase(m_RPPDBFolder);
                itemSummaryDB.MigrateToSQL();
            }
            GC.Collect();
        }
        private DateTime m_LastPlayerSummaryDatabaseUpdateTime = DateTime.UtcNow.AddHours(-5);
        public void UpdatePlayerSummaryDatabase(bool _Force = false)
        {
            lock (m_LockObject)
            {
                if (_Force == true || ((DateTime.UtcNow - m_LastPlayerSummaryDatabaseUpdateTime).TotalHours > 8 
                    && DateTime.UtcNow.Hour > 10 && DateTime.UtcNow.Hour < 14
                    && DateTime.UtcNow.DayOfWeek != DayOfWeek.Sunday
                    && DateTime.UtcNow.DayOfWeek != DayOfWeek.Monday
                    && DateTime.UtcNow.DayOfWeek != DayOfWeek.Wednesday
                    && DateTime.UtcNow.DayOfWeek != DayOfWeek.Thursday))
                {
                    Logger.ConsoleWriteLine("Armory: Started Creating Player Summary Databases", ConsoleColor.Green);
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    VF_RPDatabase.PlayerSummaryDatabase playerSummaryDB = new VF_RPDatabase.PlayerSummaryDatabase();
                    foreach (var realm in Database.ALL_REALMS)
                    {
                        if (Database.SKIP_REALM_TAGS.Contains(realm))
                            continue;
                        GC.Collect();
                        Database fullDatabase = new Database(m_RPPDBFolder + "Database\\", new DateTime(2012, 5, 1, 0, 0, 0), new WowRealm[] { realm });
                        fullDatabase.PurgeRealmDBs(true, true, true);
                        playerSummaryDB.UpdateRealm(fullDatabase.GetRealm(realm));
                        Logger.ConsoleWriteLine("Armory: Updated Player Summary Database for realm " + realm, ConsoleColor.Green);
                    }
                    playerSummaryDB.SaveSummaryDatabase(m_RPPDBFolder);
                    Logger.ConsoleWriteLine("Armory: Done Creating Player Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
                    m_LastPlayerSummaryDatabaseUpdateTime = DateTime.UtcNow;
                }
            }
            GC.Collect();
        }

        private void _UpdateGuildSummaryDatabase(Database _Database, bool _UpdateAllHistory = false)
        {
            VF_RPDatabase.GuildSummaryDatabase.UpdateSummaryDatabase(m_RPPDBFolder, _Database, _UpdateAllHistory);
            m_LastSummaryDatabaseUpdateTime = DateTime.UtcNow;
        }
        public void CreateGuildSummaryDatabase()
        {
            lock (m_LockObject)
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();
                int realmIndex = 1;
                foreach(var realm in Database.ALL_REALMS)
                {
                    if (Database.SKIP_REALM_TAGS.Contains(realm))
                        continue;
                    var realmTimer = System.Diagnostics.Stopwatch.StartNew();
                    Database fullDatabase = new Database(m_RPPDBFolder + "Database\\", new DateTime(2012, 5, 1, 0, 0, 0), new WowRealm[] { realm });
                    fullDatabase.PurgeRealmDBs(true, true, true);

                    _UpdateGuildSummaryDatabase(fullDatabase, true);
                    Logger.ConsoleWriteLine("Armory: Guild Summary Database Generation " + (realmIndex++) + " / " + Database.ALL_REALMS.Length + ", Done with " + realm.ToString() + " it took " + (realmTimer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Cyan);
                }
                Logger.ConsoleWriteLine("Armory: Done Creating Guild Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
            }
            GC.Collect();
        }

        private void _UpdateItemSummaryDatabase(Database _Database, bool _UpdateAllHistory = false) //false means it only updates last 8 days
        {
            VF_RPDatabase.ItemSummaryDatabase.UpdateSummaryDatabase(m_RPPDBFolder, _Database, _UpdateAllHistory);
            m_LastSummaryDatabaseUpdateTime = DateTime.UtcNow;
        }
        public void CreateItemSummaryDatabase()
        {
            lock (m_LockObject)
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();
                int realmIndex = 1;
                foreach(var realm in Database.ALL_REALMS)
                {
                    if (Database.SKIP_REALM_TAGS.Contains(realm))
                        continue;
                    var realmTimer = System.Diagnostics.Stopwatch.StartNew();
                    Database fullDatabase = new Database(m_RPPDBFolder + "Database\\", new DateTime(2012, 5, 1, 0, 0, 0), new WowRealm[]{ realm });
                    fullDatabase.PurgeRealmDBs(true, true, true);
                    _UpdateItemSummaryDatabase(fullDatabase, true);
                    Logger.ConsoleWriteLine("Armory: Item Summary Database Generation " + (realmIndex++) + " / " + Database.ALL_REALMS.Length + ", Done with " + realm.ToString() + " it took " + (realmTimer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Cyan);
                }
                Logger.ConsoleWriteLine("Armory: Done Creating Item Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
            }
            GC.Collect();
        }
        public void ResetArchangel()
        {
            lock (m_LockObject)
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();
                Database fullDatabase = new Database(m_RPPDBFolder + "Database\\", new DateTime(2012, 5, 1, 0, 0, 0), new WowRealm[] { WowRealm.Archangel });
                Console.Write(".");
                fullDatabase.PurgeRealmDBs(true, true);
                Console.Write(".");
                _UpdateGuildSummaryDatabase(fullDatabase);
                Console.Write(".");
                VF_RPDatabase.PlayerSummaryDatabase.GenerateSummaryDatabase(m_RPPDBFolder, fullDatabase);
                Console.Write(".");
                _UpdateItemSummaryDatabase(fullDatabase);

                Logger.ConsoleWriteLine("Armory: Done Resetting Archangel Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
            }
            GC.Collect();
        }

        class ContributorDBElement
        {
            public MongoDB.Bson.ObjectId Id { get; set; }
            public string Key { get; set; }
            public string Name { get; set; }
            public string UserID { get; set; }
            public string IP { get; set; }
            public int ContributorID { get; set; }
            public bool TrustWorthy { get; set; }

            public ContributorDBElement(string _Key, Contributor _Contributor)
            {
                Key = _Key;
                Name = _Contributor.Name;
                UserID = _Contributor.UserID;
                IP = _Contributor.IP;
                ContributorID = _Contributor.ContributorID;
                TrustWorthy = _Contributor.TrustWorthy;
            }
            public ContributorDBElement()
            { }
        }

        //using MongoDB.Bson;
        //using MongoDB.Driver;
        //using MongoDB.Driver.Builders;
        //using MongoDB.Driver.GridFS;
        //using MongoDB.Driver.Linq;
        internal void PortToMongoDB()
        {
            /*var client = new MongoDB.Driver.MongoClient("mongodb://192.168.1.198");
            var server = client.GetServer();
            
            var database = server.GetDatabase("RealmPlayersDB");
            if (database.CollectionExists("Contributors") == false)
            {
                database.CreateCollection("Contributors");
            }
            var contributorDB = database.GetCollection<ContributorDBElement>("Contributors");

            var contributors = VF_RealmPlayersDatabase.Deprecated.ContributorHandler.GetContributorsCopy();
            foreach (var contributor in contributors)
            {
                var query = MongoDB.Driver.Builders.Query<ContributorDBElement>.EQ(e => e.Key, contributor.Key);
                if (contributorDB.FindOne(query) == null)
                {
                    var newContributor = new ContributorDBElement(contributor.Key, contributor.Value);
                    contributorDB.Insert(newContributor);
                    Console.WriteLine("Added new Contributor");
                }
                else
                {
                    Console.WriteLine("Contributor already exists");
                }
            }*/
        }
    }
}
