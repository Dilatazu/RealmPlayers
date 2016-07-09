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

        object m_LockObject = new object();
        System.Threading.Thread m_MainThread = null;
        public RPPDatabaseHandler(string _RPPDBFolder)
        {
            m_RPPDBFolder = _RPPDBFolder;
            if (ItemDropDatabase.DatabaseExists(m_RPPDBFolder + "Database\\") == false)
            {
                ItemDropDatabase itemDropDatabase = new ItemDropDatabase(m_RPPDBFolder + "Database\\");
            }
            m_Communicator = new UploaderCommunication.RPPCommunicator(18374);
            m_Database = new Database(m_RPPDBFolder + "Database\\");
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
            Logger.ConsoleWriteLine("Shutdown for RPPDatabaseHandler is triggered!", ConsoleColor.White);
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
            Logger.ConsoleWriteLine("MainThread for RPPDatabaseHandler is started!", ConsoleColor.Green);
            while (m_Communicator != null && m_MainThread != null)
            {
                try
                {
                    ProcessData();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                if(m_MainThread != null)
                    System.Threading.Thread.Sleep(30000);
            }
            Logger.ConsoleWriteLine("MainThread for RPPDatabaseHandler is exited!", ConsoleColor.Green);
        }
        void ProcessData()
        {
            if (m_Communicator == null)
                return;
            lock (m_LockObject)
            {
                m_AddedContributions.Clear();
                RPPContribution data;
                while (m_NewContributions.TryDequeue(out data))
                {
                    m_Database.AddContribution(data);
                    m_AddedContributions.Add(data);
                }
                while (m_Communicator.GetNextRPPContribution(out data))
                {
                    m_Database.AddContribution(data);
                    m_AddedContributions.Add(data);
                }
                if (m_AddedContributions.Count > 0)
                {
                    m_Database.SaveRealmDatabases(m_RPPDBFolder + "Database\\");
                }
                foreach (RPPContribution contribution in m_AddedContributions)
                {
                    BackupRPPContribution(contribution.GetFilename());
                }
                m_Database.Cleanup();
            }
            if (m_Communicator == null || m_MainThread == null)
                return;
            UpdateSummaryDatabases();
            UpdatePlayerSummaryDatabase();
        }
        public void TriggerSaveDatabases()
        {
            m_Database.SaveRealmDatabases(m_RPPDBFolder + "Database\\");
        }
        public static string g_AddonContributionsBackupFolder = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_DataServer\\AddonContributionsBackup\\";
        void BackupRPPContribution(string _Filename)
        {
            if (System.IO.File.Exists(_Filename) == false)
            {
                Logger.ConsoleWriteLine("Could not backup file: " + _Filename + ", it does not exist!", ConsoleColor.Red);
                return;
            }
            string zipFileName = "";
            string zipFullFilePath = "";

            if (_Filename.Contains("VF_RealmPlayersTBC") == true)
            {
                zipFileName = "VF_RealmPlayersTBC_Contributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                zipFullFilePath = g_AddonContributionsBackupFolder + "VF_RealmPlayersTBC\\" + zipFileName;
            }
            else
            {
                zipFileName = "VF_RealmPlayers_Contributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
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
            Logger.ConsoleWriteLine("Successfull backup of file: " + _Filename + " into " + zipFileName);
        }

        private DateTime m_LastSummaryDatabaseUpdateTime = DateTime.UtcNow.AddMinutes(-15);
        public void UpdateSummaryDatabases(bool _Force = false)
        {
            lock (m_LockObject)
            {
                if (_Force == true || (DateTime.UtcNow - m_LastSummaryDatabaseUpdateTime).TotalMinutes > 113)
                {
                    Logger.ConsoleWriteLine("Started Updating Summary Databases", ConsoleColor.Green);
                    GC.Collect();
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    Database tempDatabase = new Database(m_RPPDBFolder + "Database\\");
                    tempDatabase.PurgeRealmDBs(true, true);

                    _UpdateGuildSummaryDatabase(tempDatabase);
                    _UpdateItemSummaryDatabase(tempDatabase);
                    Logger.ConsoleWriteLine("Done Updating Summary Databases, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
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
                if (_Force == true || (DateTime.UtcNow - m_LastPlayerSummaryDatabaseUpdateTime).TotalHours > 6)
                {
                    Logger.ConsoleWriteLine("Started Creating Player Summary Databases", ConsoleColor.Green);
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    VF_RPDatabase.PlayerSummaryDatabase playerSummaryDB = new VF_RPDatabase.PlayerSummaryDatabase();
                    foreach (var realm in Database.ALL_REALMS)
                    {
                        Database fullDatabase = new Database(m_RPPDBFolder + "Database\\", new DateTime(2012, 5, 1, 0, 0, 0), new WowRealm[] { realm });
                        fullDatabase.PurgeRealmDBs(true, true, true);
                        playerSummaryDB.UpdateRealm(fullDatabase.GetRealm(realm));
                        Logger.ConsoleWriteLine("Updated Player Summary Database for realm " + realm, ConsoleColor.Green);
                    }
                    playerSummaryDB.SaveSummaryDatabase(m_RPPDBFolder);
                    Logger.ConsoleWriteLine("Done Creating Player Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
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
                    var realmTimer = System.Diagnostics.Stopwatch.StartNew();
                    Database fullDatabase = new Database(m_RPPDBFolder + "Database\\", new DateTime(2012, 5, 1, 0, 0, 0), new WowRealm[] { realm });
                    fullDatabase.PurgeRealmDBs(true, true, true);

                    _UpdateGuildSummaryDatabase(fullDatabase, true);
                    Logger.ConsoleWriteLine("Guild Summary Database Generation " + (realmIndex++) + " / " + Database.ALL_REALMS.Length + ", Done with " + realm.ToString() + " it took " + (realmTimer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Cyan);
                }
                Logger.ConsoleWriteLine("Done Creating Guild Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
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
                    var realmTimer = System.Diagnostics.Stopwatch.StartNew();
                    Database fullDatabase = new Database(m_RPPDBFolder + "Database\\", new DateTime(2012, 5, 1, 0, 0, 0), new WowRealm[]{ realm });
                    fullDatabase.PurgeRealmDBs(true, true, true);
                    _UpdateItemSummaryDatabase(fullDatabase, true);
                    Logger.ConsoleWriteLine("Item Summary Database Generation " + (realmIndex++) + " / " + Database.ALL_REALMS.Length + ", Done with " + realm.ToString() + " it took " + (realmTimer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Cyan);
                }
                Logger.ConsoleWriteLine("Done Creating Item Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
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

                Logger.ConsoleWriteLine("Done Resetting Archangel Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
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
