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
        List<string> m_ProblemFiles = new List<string>();

        Dictionary<string, FightDataCollection> m_GetFightDataCollectionCache = new Dictionary<string, FightDataCollection>();
        List<RaidCollection_Raid> m_RaidsModifiedSinceLastSummaryUpdate = new List<RaidCollection_Raid>();
        DateTime m_DateTimeLastRaidStatsDBSave = DateTime.UtcNow;

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
            Logger.ConsoleWriteLine("Shutdown for RDDatabaseHandler is triggered!", ConsoleColor.White);
            m_MainThread = null;
            try
            {
                ProcessData();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        private bool SaveRaidStatsDBs()
        {
            if (m_RaidCollection != null)
            {
                try
                {
                    Logger.ConsoleWriteLine("Started saving all the accumulated RaidCollection.dat changes!", ConsoleColor.Green);
                    VF.Utility.BackupFile(m_RDDBFolder + "RaidCollection.dat", VF.Utility.BackupMode.Backup_Daily);
                    int tryCount = 1;
                    while(true)
                    {
                        try
                        {
                            VF.Utility.SaveSerialize(m_RDDBFolder + "RaidCollection.dat", m_RaidCollection, false);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to save RaidCollection!!! Trying again!(Try Nr " + tryCount + "/10", ConsoleColor.Red);
                            if (tryCount > 10)
                                throw ex;
                            System.Threading.Thread.Sleep(10000 * tryCount);
                        }
                        ++tryCount;
                    }

                    UpdateSummaryDatabase(m_GetFightDataCollectionCache, m_RaidsModifiedSinceLastSummaryUpdate);
                    m_GetFightDataCollectionCache = new Dictionary<string, FightDataCollection>();
                    m_RaidsModifiedSinceLastSummaryUpdate = new List<RaidCollection_Raid>();
                    Logger.ConsoleWriteLine("Done saving all the accumulated RaidCollection.dat changes!", ConsoleColor.Green);

                    foreach (string problemData in m_ProblemFiles)
                    {
                        BackupRDContribution(problemData, RDContributionType.Problem);
                    }
                    foreach (string emptyData in m_AddedEmptyFiles)
                    {
                        BackupRDContribution(emptyData, RDContributionType.Empty);
                    }
                    foreach (string contribution in m_AddedContributionFiles)
                    {
                        BackupRDContribution(contribution, RDContributionType.Data);
                    }
                    m_ProblemFiles.Clear();
                    m_AddedContributionFiles.Clear();
                    m_AddedEmptyFiles.Clear();
                    Logger.ConsoleWriteLine("Done saving all the file backups!", ConsoleColor.Green);

                    m_DateTimeLastRaidStatsDBSave = System.DateTime.UtcNow;
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    Logger.ConsoleWriteLine("Well, if this happens give up...", ConsoleColor.Red);
                    while (true)
                    {
                        System.Threading.Thread.Sleep(5000);
                        Console.Write("ASSESS DAMAGE(RS)! ", ConsoleColor.Red);
                    }
                }
            }
            return false;
        }
        private void MainThread()
        {
            Logger.ConsoleWriteLine("MainThread for RDDatabaseHandler is started!", ConsoleColor.Green);
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
            lock (m_LockObject)
            {
                SaveRaidStatsDBs();
            }
            while(true)
            {
                Logger.ConsoleWriteLine("MainThread for RDDatabaseHandler is exited!", ConsoleColor.Green);
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
                try
                {
                    string raidDamageDataFile;
                    while (m_NewContributions.TryDequeue(out raidDamageDataFile))
                    {
                        try
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
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                            m_ProblemFiles.Add(raidDamageDataFile);
                            Logger.ConsoleWriteLine("DUE TO ERRORS WE ARE RELOADING RAIDCOLLECTION!!!", ConsoleColor.Red);
                            //RESET m_RaidCollection!!!
                            VF.Utility.LoadSerialize<RaidCollection>(m_RDDBFolder + "RaidCollection.dat", out m_RaidCollection);
                            m_GetFightDataCollectionCache = new Dictionary<string, FightDataCollection>();
                            m_RaidsModifiedSinceLastSummaryUpdate = new List<RaidCollection_Raid>();
                            Logger.ConsoleWriteLine("RELOAD OF RAIDCOLLECTION WAS SUCCESSFULL!!!", ConsoleColor.Green);
                            foreach (string contribution in m_AddedContributionFiles)
                            {
                                m_NewContributions.Enqueue(contribution);
                            }
                            m_AddedContributionFiles.Clear();
                            break;
                        }
                    }
                    if (m_GetFightDataCollectionCache.Count > 20 ||
                        (m_GetFightDataCollectionCache.Count > 1 && DateTime.UtcNow > m_DateTimeLastRaidStatsDBSave.AddMinutes(30)))
                    {
                        if(SaveRaidStatsDBs() == true)
                        {
                            //yay...
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        private FightDataCollection _LoadRaidFightCollectionFile(string _FightFile)
        {
            if (_FightFile.StartsWith(VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\"))
            {
                _FightFile = _FightFile.Substring((VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\").Length);
            }
            VF_RaidDamageDatabase.FightDataCollection fightDataCollection = null;
            if (VF.Utility.LoadSerialize(m_RDDBFolder + _FightFile, out fightDataCollection) == false)
                return null;
            return fightDataCollection;
        }
        private int AddFightsToDatabase(string _fightCollectionFile)
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
                List<RaidCollection_Raid> raidsModified = new List<RaidCollection_Raid>();
                List<RaidCollection_Dungeon> dungeonsModified = new List<RaidCollection_Dungeon>();
                m_RaidCollection.AddFightCollection(fights, fightCollectionDatName, raidsModified, dungeonsModified);
                //TESTING
                if (raidsModified.Count > 0 || dungeonsModified.Count > 0)
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
                            try
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
                            catch (Exception ex)
                            {
                                raid.RaidOwnerName = "PUG";
                                Logger.LogException(ex);
                            }
                        }
                    }
                    foreach (var dungeon in dungeonsModified)
                    {
                        var bossFights = dungeon.GetBossFights(fights);
                        Logger.ConsoleWriteLine("Dungeon: " + dungeon.m_Dungeon + "(" + dungeon.m_UniqueDungeonID + ") by \"" + dungeon.m_GroupMembers.MergeToStringVF("\", \"") + "\"", ConsoleColor.White);
                        foreach (var bossFight in bossFights)
                        {
                            ++totalFightCount;
                            bossFight.GetFightDetails(); //Trigger FightDetail request, so we get error here instead of later on the website.
                            //Logger.ConsoleWriteLine("Fight: " + bossFight.GetBossName() + " added to RaidCollection", ConsoleColor.Green);
                        }
                    }
                    Logger.ConsoleWriteLine("--------------------", ConsoleColor.White);
                    VF.Utility.SaveSerialize(m_RDDBFolder + fightCollectionDatName, fights);

                    Logger.ConsoleWriteLine(_fightCollectionFile + " added " + totalFightCount + " fights to RaidCollection", ConsoleColor.Green);

                    m_GetFightDataCollectionCache.Add(fightCollectionDatName, fights);
                    m_RaidsModifiedSinceLastSummaryUpdate.AddRange(raidsModified);

                    /*
                    //DISABLED FOR NOW
                    string debugFilePath = m_RDDBFolder + "\\DebugData\\SessionDebug\\" + DateTime.UtcNow.ToString("yyyy_MM_dd") + ".txt";
                    VF.Utility.AssertFilePath(debugFilePath);
                    if (System.IO.File.Exists(debugFilePath) == true)
                    {
                        System.IO.File.AppendAllLines(debugFilePath, sessionDebugData);
                    }
                    else
                    {
                        System.IO.File.WriteAllLines(debugFilePath, sessionDebugData);
                    }*/
                }
                else
                {
                    Logger.ConsoleWriteLine(_fightCollectionFile + " already exists in RaidCollection, skipping", ConsoleColor.Green);
                }
            }

            return totalFightCount;
        }

        private void UpdateSummaryDatabase(Dictionary<string, FightDataCollection> _CachedFightDataCollections = null, List<RaidCollection_Raid> _RaidsModified = null, bool _ReplaceRaidsModified = false)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            //Caching
            Dictionary<string, FightDataCollection> getFightDataCollectionCache = _CachedFightDataCollections;
            if (getFightDataCollectionCache == null)
                getFightDataCollectionCache = new Dictionary<string, FightDataCollection>();
            List<RaidCollection_Raid> raidsModified = _RaidsModified;
            if (raidsModified == null)
                raidsModified = new List<RaidCollection_Raid>();
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
                var summaryDB = VF_RDDatabase.SummaryDatabase.UpdateSummaryDatabase(m_RDDBFolder, m_RaidCollection, raidsModified
                    , cachedGetFightDataCollectionFunc
                    , (_WowRealm) => { return new RealmDB(m_RPPDatabaseHandler.GetRealmDB(_WowRealm)); });
                Logger.ConsoleWriteLine("Done Updating Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
                timer = System.Diagnostics.Stopwatch.StartNew();
                VF_RDDatabase.GroupSummaryDatabase.UpdateSummaryDatabase(m_RDDBFolder, summaryDB);
                Logger.ConsoleWriteLine("Done Updating Group Summary Database, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
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
                int[] buggedRaidIDs = { 31617 };
                List<RaidCollection_Raid> buggedRaids = new List<RaidCollection_Raid>();
                foreach (var raid in m_RaidCollection.m_Raids)
                {
                    if(buggedRaidIDs.Contains(raid.Value.UniqueRaidID))
                    {
                        buggedRaids.Add(raid.Value);
                    }
                }
                //VF.Utility.SaveSerialize(m_RDDBFolder + "RaidCollection.dat", m_RaidCollection, false);
                UpdateSummaryDatabase(null, buggedRaids, true);
            }
            GC.Collect();
        }

        public static string g_AddonContributionsBackupFolder = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_DataServer\\AddonContributionsBackup\\";
        enum RDContributionType
        {
            Empty,
            Data,
            Problem,
        }
        void BackupRDContribution(string _Filename, RDContributionType _ContributionType)
        {
            try
            {
                //while(true)
                //{
                //    Console.WriteLine("Disabled Backup and deletion for now");
                //    for (int i = 0; i < 20; ++i)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        Console.Write(".");
                //    }
                //}
                if (System.IO.File.Exists(_Filename) == false)
                {
                    Logger.ConsoleWriteLine("Could not backup file: " + _Filename + ", it does not exist!", ConsoleColor.Red);
                    return;
                }
                string zipFileName = "";
                string zipFullFilePath = "";

                if (_Filename.Contains("VF_RaidStatsTBC") == true)
                {
                    if (_ContributionType == RDContributionType.Data)
                        zipFileName = "VF_RaidStatsTBC_Contributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    else if (_ContributionType == RDContributionType.Empty)
                        zipFileName = "VF_RaidStatsTBC_Empty_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    else//if (_ContributionType == RDContributionType.Problem)
                        zipFileName = "VF_RaidStatsTBC_Problem_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    zipFullFilePath = g_AddonContributionsBackupFolder + "VF_RaidStatsTBC\\" + zipFileName;
                }
                else
                {
                    if (_ContributionType == RDContributionType.Data)
                        zipFileName = "VF_RaidDamage_Contributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    else if (_ContributionType == RDContributionType.Empty)
                        zipFileName = "VF_RaidDamage_Empty_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    else//if (_ContributionType == RDContributionType.Problem)
                        zipFileName = "VF_RaidDamage_Problem_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
                    zipFullFilePath = g_AddonContributionsBackupFolder + "VF_RaidDamage\\" + zipFileName;
                }

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
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Logger.ConsoleWriteLine("Well(RP-ZIP), if this happens give up...", ConsoleColor.Red);
                while (true)
                {
                    System.Threading.Thread.Sleep(5000);
                    Console.Write("ASSESS DAMAGE(RS-ZIP)! ", ConsoleColor.Red);
                }
            }
        }
    }
}
