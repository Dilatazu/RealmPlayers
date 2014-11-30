using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using ICSharpCode.SharpZipLib.Zip;

using VF_RaidDamageDatabase;

namespace VF_WoWLauncherServer
{
    class RDDatabaseHandler
    {
        private string m_RDDBFolder;

        object m_LockObject = new object();
        System.Threading.Thread m_MainThread = null;

        RaidCollection m_RaidCollection = null;
        RPPDatabaseHandler m_RPPDatabaseHandler = null;

        ConcurrentQueue<string> m_NewContributions = new ConcurrentQueue<string>();
        List<string> m_AddedContributionFiles = new List<string>();
        List<string> m_AddedEmptyFiles = new List<string>();

        public RDDatabaseHandler(string _RDDBFolder, RPPDatabaseHandler _RPPDatabaseHandler)
        {
            m_RDDBFolder = _RDDBFolder;
            m_RPPDatabaseHandler = _RPPDatabaseHandler;

            VF.Utility.LoadSerialize<RaidCollection>(m_RDDBFolder + "RaidCollection.dat", out m_RaidCollection);
            m_MainThread = new System.Threading.Thread(MainThread);
            m_MainThread.Start();
        }
        public void AddContribution(string _Contribution)
        {
            m_NewContributions.Enqueue(_Contribution);
        }
        public void Shutdown()
        {
            m_MainThread = null;
            ProcessData();
        }
        private void MainThread()
        {
            while (m_MainThread != null)
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
        }
        public void Pause()
        {
            System.Threading.Monitor.Enter(m_LockObject);
        }
        public void Continue()
        {
            System.Threading.Monitor.Exit(m_LockObject);
        }
        void ProcessData()
        {
            lock (m_LockObject)
            {
                if (System.IO.Directory.Exists(m_RDDBFolder + "\\ManuallyAdded\\") == true)
                {
                    string[] luaFiles = System.IO.Directory.GetFiles(m_RDDBFolder + "\\ManuallyAdded\\");
                    foreach (string file in luaFiles)
                    {
                        if (((file.EndsWith(".lua") == true && file.Contains("VF_RaidDamage") == false)) || file.EndsWith(".txt") == true)
                        {
                            Logger.ConsoleWriteLine("Added new manual contribution file: \"" + file + "\"", ConsoleColor.Cyan);
                            m_NewContributions.Enqueue(file);
                        }
                        else
                        {
                            Logger.ConsoleWriteLine("Not adding file: \"" + file + "\" please move it out of the ManuallyAdded folder!", ConsoleColor.Red);
                        }
                    }
                }
                m_AddedContributionFiles.Clear();
                m_AddedEmptyFiles.Clear();
                string raidDamageDataFile;
                while (m_NewContributions.TryDequeue(out raidDamageDataFile))
                {
                    if (AddFightsToDatabase(raidDamageDataFile) >= 1)
                    {
                        m_AddedContributionFiles.Add(raidDamageDataFile);
                        GC.Collect();
                    }
                    else
                    {
                        m_AddedEmptyFiles.Add(raidDamageDataFile);
                    }
                }
                
                foreach (string emptyData in m_AddedEmptyFiles)
                {
                    BackupRDContribution(emptyData, RDContributionType.Empty);
                }
                foreach (string contribution in m_AddedContributionFiles)
                {
                    BackupRDContribution(contribution, RDContributionType.Data);
                }
            }
        }

        private FightDataCollection _LoadRaidFightCollectionFile(string _FightFile)
        {
            if (_FightFile.StartsWith("R:\\VF_RealmPlayersData\\RDDatabase\\"))
            {
                _FightFile = _FightFile.Substring("R:\\VF_RealmPlayersData\\RDDatabase\\".Length);
            }
            VF_RaidDamageDatabase.FightDataCollection fightDataCollection = null;
            if (VF.Utility.LoadSerialize(m_RDDBFolder + _FightFile, out fightDataCollection) == false)
                return null;
            return fightDataCollection;
        }
        private int AddFightsToDatabase(string _fightCollectionFile)
        {
            try
            {
                List<string> sessionDebugData = new List<string>();
                int totalFightCount = 0;
                string filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(_fightCollectionFile);
                string fightCollectionDatName = "DataFiles\\" + DateTime.UtcNow.ToString("yyyy_MM") + "\\" + filenameWithoutExtension + ".dat";
                fightCollectionDatName = VF.Utility.ConvertToUniqueFilename(fightCollectionDatName);
                List<DamageDataSession> dataSessions = new List<DamageDataSession>();
                dataSessions = DamageDataParser.ParseFile(_fightCollectionFile, ref sessionDebugData);
                Console.Write("Generating Fights...");
                var fights = FightDataCollection.GenerateFights(dataSessions);//, BossInformation.BossFights);
                Console.WriteLine("DONE");

                if (fights.Fights.Count >= 1)
                {
                    Logger.ConsoleWriteLine(_fightCollectionFile + " contained " + fights.Fights.Count + " fights", ConsoleColor.Yellow);
                    List<RaidCollection.Raid> raidsModified = new List<RaidCollection.Raid>();
                    m_RaidCollection.AddFightCollection(fights, fightCollectionDatName, raidsModified);
                    //TESTING
                    if (raidsModified.Count > 0)
                    {
                        Logger.ConsoleWriteLine("--------------------", ConsoleColor.White);
                        foreach (var raid in raidsModified)
                        {
                            var bossFights = raid.GetBossFights(fights, null);
                            Logger.ConsoleWriteLine("Raid: " + raid.RaidInstance + "(" + raid.RaidID + ") by " + raid.RaidOwnerName, ConsoleColor.White);
                            foreach (var bossFight in bossFights)
                            {
                                ++totalFightCount;
                                bossFight.GetFightDetails(); //Trigger FightDetail request, so we get error here instead of later on the website.
                                //Logger.ConsoleWriteLine("Fight: " + bossFight.GetBossName() + " added to RaidCollection", ConsoleColor.Green);
                            }
                            if (raid.RaidOwnerName.ToLower() == "unknown" || raid.RaidOwnerName == "")
                            {
                                lock (m_RPPDatabaseHandler.GetLockObject())
                                {
                                    var realmDB = new RealmDB(m_RPPDatabaseHandler.GetRealmDB(raid.Realm));

                                    List<string> attendingPlayers = new List<string>();
                                    foreach (var bossFight in bossFights)
                                    {
                                        attendingPlayers.AddRange(bossFight.GetAttendingUnits(realmDB.RD_IsPlayerFunc(bossFight)));
                                    }
                                    if (attendingPlayers.Distinct().Count() > 2)
                                    {
                                        Dictionary<string, int> guildCount = new Dictionary<string, int>();
                                        foreach (var attendingPlayer in attendingPlayers)
                                        {
                                            string guildName = realmDB.GetPlayer(attendingPlayer).Guild.GuildName;
                                            if (guildCount.ContainsKey(guildName))
                                                guildCount[guildName] = guildCount[guildName] + 1;
                                            else
                                                guildCount.Add(guildName, 1);
                                        }
                                        var biggestGuildCount = guildCount.OrderByDescending((_Value) => _Value.Value).First();
                                        if (biggestGuildCount.Value >= (int)(0.7f * (float)attendingPlayers.Count))
                                            raid.RaidOwnerName = biggestGuildCount.Key;
                                        else
                                            raid.RaidOwnerName = "PUG";

                                        Logger.ConsoleWriteLine("Raid: Changed RaidOwnerName for " + raid.RaidInstance + "(" + raid.RaidID + ") to " + raid.RaidOwnerName, ConsoleColor.White);
                                    }
                                    else
                                    {
                                        raid.RaidOwnerName = "PUG";
                                    }
                                }
                            }
                        }
                        Logger.ConsoleWriteLine("--------------------", ConsoleColor.White);
                        VF.Utility.SaveSerialize(m_RDDBFolder + fightCollectionDatName, fights);

                        VF.Utility.BackupFile(m_RDDBFolder + "RaidCollection.dat", VF.Utility.BackupMode.Backup_Daily);

                        VF.Utility.SaveSerialize(m_RDDBFolder + "RaidCollection.dat", m_RaidCollection, false);
                        Logger.ConsoleWriteLine(_fightCollectionFile + " added " + totalFightCount + " fights to RaidCollection", ConsoleColor.Green);

                        Dictionary<string, FightDataCollection> getFightDataCollectionCache = new Dictionary<string, FightDataCollection>();
                        getFightDataCollectionCache.Add(fightCollectionDatName, fights);
                        UpdateSummaryDatabase(getFightDataCollectionCache, raidsModified);


                        string debugFilePath = m_RDDBFolder + "\\DebugData\\SessionDebug\\" + DateTime.UtcNow.ToString("yyyy_MM_dd") + ".txt";
                        VF.Utility.AssertFilePath(debugFilePath);
                        if (System.IO.File.Exists(debugFilePath) == true)
                        {
                            System.IO.File.AppendAllLines(debugFilePath, sessionDebugData);
                        }
                        else
                        {
                            System.IO.File.WriteAllLines(debugFilePath, sessionDebugData);
                        }
                    }
                    else
                    {
                        Logger.ConsoleWriteLine(_fightCollectionFile + " already exists in RaidCollection, skipping", ConsoleColor.Green);
                    }
                }

                return totalFightCount;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return 0;
        }

        private void UpdateSummaryDatabase(Dictionary<string, FightDataCollection> _CachedFightDataCollections = null, List<RaidCollection.Raid> _RaidsModified = null, bool _ReplaceRaidsModified = false)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            //Caching
            Dictionary<string, FightDataCollection> getFightDataCollectionCache = _CachedFightDataCollections;
            if (getFightDataCollectionCache == null)
                getFightDataCollectionCache = new Dictionary<string, FightDataCollection>();
            List<RaidCollection.Raid> raidsModified = _RaidsModified;
            if (raidsModified == null)
                raidsModified = new List<RaidCollection.Raid>();
            Func<string, FightDataCollection> cachedGetFightDataCollectionFunc = (string _File) =>
            {
                FightDataCollection db = null;
                if (getFightDataCollectionCache.Count > 50 && GC.GetTotalMemory(false) >= 3L * 1024L * 1024L * 1024L)
                {
                    getFightDataCollectionCache = new Dictionary<string, FightDataCollection>();
                    GC.Collect();
                }
                if (getFightDataCollectionCache.TryGetValue(_File, out db) == false)
                {
                    db = _LoadRaidFightCollectionFile(_File);
                    getFightDataCollectionCache.Add(_File, db);
                }
                return db;
            };
            //Caching

            if (_ReplaceRaidsModified == false)
            {
                //Default
                VF_RDDatabase.SummaryDatabase.UpdateSummaryDatabase(m_RDDBFolder, m_RaidCollection, raidsModified
                    , cachedGetFightDataCollectionFunc
                    , (_WowRealm) => { return new RealmDB(m_RPPDatabaseHandler.GetRealmDB(_WowRealm)); });
                Logger.ConsoleWriteLine("Done Updating Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
            }
            else
            {
                //FixBuggedSummaryDatabase
                VF_RDDatabase.SummaryDatabase.FixBuggedSummaryDatabase(m_RDDBFolder, m_RaidCollection, raidsModified
                    , cachedGetFightDataCollectionFunc
                    , (_WowRealm) => { return new RealmDB(m_RPPDatabaseHandler.GetRealmDB(_WowRealm)); });
                Logger.ConsoleWriteLine("Done Fixing " + _RaidsModified.Count + "bugged raids in Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
            }
        }
        public void CreateSummaryDatabase()
        {
            lock (m_LockObject)
            {
                UpdateSummaryDatabase();
            }
            GC.Collect();
        }
        public void FixBuggedSummaryDatabase()
        {
            lock (m_LockObject)
            {
                int[] buggedRaidIDs = { 3021, 2886 };//2353, 2168, 1840, 2234, 1489, 2106 };
                List<RaidCollection.Raid> buggedRaids = new List<RaidCollection.Raid>();
                foreach (var raid in m_RaidCollection.m_Raids)
                {
                    if (buggedRaidIDs.Contains(raid.Value.UniqueRaidID))
                    {
                        buggedRaids.Add(raid.Value);
                    }
                }
                UpdateSummaryDatabase(null, buggedRaids, true);
            }
            GC.Collect();
        }

        static string g_AddonContributionsBackupFolder = "R:\\VF_DataServer\\AddonContributionsBackup\\";
        enum RDContributionType
        {
            Empty,
            Data,
        }
        void BackupRDContribution(string _Filename, RDContributionType _ContributionType)
        {
            if (System.IO.File.Exists(_Filename) == false)
            {
                Logger.ConsoleWriteLine("Could not backup file: " + _Filename + ", it does not exist!", ConsoleColor.Red);
                return;
            }
            string zipFileName = "";
            if(_ContributionType == RDContributionType.Data)
                zipFileName = "VF_RaidDamage_Contributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
            else
                zipFileName = "VF_RaidDamage_Empty_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
            string zipFullFilePath = g_AddonContributionsBackupFolder + "VF_RaidDamage\\" + zipFileName;
            VF_WoWLauncher.Utility.AssertFilePath(zipFullFilePath);
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
    }
}
