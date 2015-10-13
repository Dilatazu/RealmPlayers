using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * This is the old Program which manually parsed lua databases that was put in a specific folder.
 * This step is done automatically in the VF_WowLauncherServer application instead, there has been modifications and additions since.
 * This file and information is basically just kept here for history and its useful information for someone that does not have access to the VF_WowLauncherServer project. 
 */

namespace VF_RaidDamageDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DamageDataSession> dataSessions = new List<DamageDataSession>();
            List<string> sessionDebugData = new List<string>();
            dataSessions = DamageDataParser.ParseFile("2014_05_03_20_59_00_FID0506.txt", ref sessionDebugData);

            var fights = FightDataCollection.GenerateFights(dataSessions);//, BossInformation.BossFights);
#if Use_Main
            RaidCollection raidCollection = null;
            if (System.IO.File.Exists("RaidCollection.dat") == true)
                VF.Utility.LoadSerialize<RaidCollection>("RaidCollection.dat", out raidCollection);
            else
            {
                raidCollection = new RaidCollection();
                if (System.IO.Directory.Exists("OldDataFiles\\"))
                {
                    string[] datFiles = System.IO.Directory.GetFiles("OldDataFiles\\");
                    foreach (string file in datFiles)
                    {
                        if (file.EndsWith(".dat") == true)
                        {
                            Logger.ConsoleWriteLine("Started Loading \"" + file + "\"", ConsoleColor.Green);
                            FightDataCollection loadedData = null;
                            VF.Utility.LoadSerialize(file, out loadedData);

                            string[] fileSplit = file.Split('_');
                            //string month = fileSplit[fileSplit.Count()-2];
                            //string year = fileSplit[fileSplit.Count()-3];
                            //string raidOwner = fileSplit[0].Split('\\')[1];
                            //for (int i = 1; i < fileSplit.Count() - 3; ++i)
                            //{
                            //    raidCollection.AddFightCollection(
                            //}
                            raidCollection.AddFightCollection(loadedData, file);
                            //foreach (var raid in raidCollection.m_Raids)
                            //{
                            //    if (raid.Value.m_DataFiles.Contains(file))
                            //    {
                            //        raid.Value.RaidOwnerName = fileSplit[0].Split('\\')[1];
                            //    }
                            //}
                        }
                    }
                }
            }
            bool backupedUpRaidCollection = false;
            RPPDatabase rppDatabase = null;
            string[] luaFiles = System.IO.Directory.GetFiles("./");
            foreach (string file in luaFiles)
            {
                if(file.EndsWith(".lua") == true)
                {
                    string fightCollectionDatName = "DataFiles\\" + DateTime.UtcNow.ToString("yyyy_MM") + "\\" + file.Replace(".lua", ".dat");
                    fightCollectionDatName = VF.Utility.ConvertToUniqueFilename(fightCollectionDatName);
                    List<DamageDataSession> dataSessions = new List<DamageDataSession>();
                    dataSessions = DamageDataParser.ParseFile(file);
                    
                    var fights = FightDataCollection.GenerateFights(dataSessions);//, BossInformation.BossFights);

                    Logger.ConsoleWriteLine(file + " contained " + fights.Fights.Count + " fights", ConsoleColor.Yellow);
                    if (fights.Fights.Count >= 1)
                    {
                        List<RaidCollection_Raid> raidsModified = new List<RaidCollection_Raid>();
                        raidCollection.AddFightCollection(fights, fightCollectionDatName, raidsModified);
                        //TESTING
                        if (raidsModified.Count > 0)
                        {
                            Logger.ConsoleWriteLine("--------------------", ConsoleColor.White);
                            int totalFightCount = 0;
                            foreach (var raid in raidsModified)
                            {
                                var bossFights = raid.GetBossFights(fights);
                                Logger.ConsoleWriteLine("Raid: " + raid.RaidInstance + "(" + raid.RaidID + ") by " + raid.RaidOwnerName, ConsoleColor.White);
                                foreach (var bossFight in bossFights)
                                {
                                    ++totalFightCount;
                                    bossFight.GetFightDetails(); //Trigger FightDetail request, so we get error here instead of later on the website.
                                    //Logger.ConsoleWriteLine("Fight: " + bossFight.GetBossName() + " added to RaidCollection", ConsoleColor.Green);
                                }
                                if (raid.RaidOwnerName.ToLower() == "unknown" || raid.RaidOwnerName == "")
                                {
                                    if (rppDatabase == null)
                                    {
                                        string rppDBDir = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RPPDatabase\\Database\\";
                                        var timeSinceWrite = DateTime.UtcNow - System.IO.File.GetLastWriteTime(rppDBDir + "Emerald_Dream\\PlayersData.dat");
                                        if (timeSinceWrite.TotalHours > 5)
                                        {
                                            if (timeSinceWrite.TotalDays > 20 ||
                                                System.Windows.Forms.MessageBox.Show("The file: \"" + rppDBDir + "Emerald_Dream\\PlayersData.dat\" is older than 5 hours. Do you want to use the database from the server instead?", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                            {
                                                rppDBDir = "\\\\" + HiddenStrings.ServerComputerName + "\\VF_RealmPlayersData\\RPPDatabase\\Database\\";
                                            }
                                        }
                                        Logger.ConsoleWriteLine("Loaded RPPDatabase(\"" + rppDBDir + "\")", ConsoleColor.Magenta);
                                        rppDatabase = new RPPDatabase(rppDBDir);
                                    }
                                    var realmDB = rppDatabase.GetRealmDB(raid.Realm);
                                    List<string> attendingPlayers = new List<string>();
                                    foreach (var bossFight in bossFights)
                                    {
                                        attendingPlayers.AddRange(bossFight.GetAttendingUnits(realmDB.RD_IsPlayer));
                                    }
                                    if (attendingPlayers.Count() > 10)
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
                                        if (biggestGuildCount.Value > attendingPlayers.Count / 2)
                                            raid.RaidOwnerName = biggestGuildCount.Key;
                                        else
                                            raid.RaidOwnerName = "PUG";

                                        Logger.ConsoleWriteLine("Raid: Changed RaidOwnerName for " + raid.RaidInstance + "(" + raid.RaidID + ") to " + raid.RaidOwnerName, ConsoleColor.White);
                                    }
                                }
                            }
                            Logger.ConsoleWriteLine("--------------------", ConsoleColor.White);
                            VF.Utility.BackupFile(/*VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\" + */fightCollectionDatName);
                            VF.Utility.SaveSerialize(/*VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\" + */fightCollectionDatName, fights);
                            string luaCopyFile = VF.Utility.ConvertToUniqueFilename("Lua\\" + file);
                            VF.Utility.AssertFilePath(luaCopyFile);
                            System.IO.File.Move(file, luaCopyFile);
                            if (backupedUpRaidCollection == false)
                            {
                                VF.Utility.BackupFile("RaidCollection.dat", VF.Utility.BackupMode.Backup_Always_TimeInFilename);
                                VF.Utility.BackupFile("RaidCollection.dat", VF.Utility.BackupMode.Backup_Daily);
                                backupedUpRaidCollection = true;
                            }
                            VF.Utility.SaveSerialize("RaidCollection.dat", raidCollection, false);
                            Logger.ConsoleWriteLine(file + " added " + totalFightCount + " fights to RaidCollection", ConsoleColor.Green);
                        }
                        else
                        {
                            Logger.ConsoleWriteLine(file + " already exists in RaidCollection, skipping", ConsoleColor.Green);
                        }
                    }
                    //Utility.BackupFile(VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\DamageDataSessions\\" + file.Replace(".lua", ".DamageDataSessions.dat"));
                    //Utility.SaveSerialize(VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\DamageDataSessions\\" + file.Replace(".lua", ".DamageDataSessions.dat"), dataSessions);
                }
            }
            Logger.ConsoleWriteLine("Done with all lua parsing", ConsoleColor.Green);
            Logger.ConsoleWriteLine("Press any key to exit", ConsoleColor.Green);
            if (System.Threading.Tasks.Task.Factory.StartNew(() => Console.ReadKey()).Wait(TimeSpan.FromSeconds(30.0)) == false)
            {
                Logger.ConsoleWriteLine("Aautomicly closing the program in 5 seconds", ConsoleColor.White);
                System.Threading.Tasks.Task.Factory.StartNew(() => Console.ReadKey()).Wait(TimeSpan.FromSeconds(5.0));
            }
#endif
        }
    }
}
