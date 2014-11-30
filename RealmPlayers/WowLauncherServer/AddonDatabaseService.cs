using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

using WowLauncherNetwork;
using VF_WoWLauncher;

using ContributorDB = VF_RealmPlayersDatabase.ContributorDB;
using RPPContribution = VF_RealmPlayersDatabase.RPPContribution;

namespace VF_WoWLauncherServer
{
    class AddonDatabaseService
    {
        private static long g_FileCounter = 0;
        public static string g_AddonUploadDataFolder = "R:\\VF_DataServer\\AddonUploadedData\\";
        public static string g_AddonUploadStatsFolder = "R:\\VF_DataServer\\AddonUploadedDataStats\\";
        public static void UploadData(System.Net.IPAddress _UploaderIP, WLN_UploadPacket_AddonData _Data)
        {
            var currContributor = ContributorDB.GetContributor(_Data.UserID, _UploaderIP, false);
            if (currContributor == null)
            {
                Logger.ConsoleWriteLine("User(" + _UploaderIP.ToString() + ") tried to access using UserID(" + _Data.UserID + ")");
            }
            else if (currContributor.UserID != _Data.UserID)
            {
                Logger.ConsoleWriteLine("User(" + _UploaderIP.ToString() + ") tried to access UserID(" + currContributor.UserID + ") using UserID(" + _Data.UserID + ")");
                currContributor = null;
            }
            string userID = _Data.UserID;
            string userIP = _UploaderIP.ToString();
            string uploadDataFolder = g_AddonUploadDataFolder + _Data.AddonName + "\\";
            string uploadStatsFolder = g_AddonUploadStatsFolder + _Data.AddonName + "\\";

            if (currContributor == null)
                userID = "Unknown_" + userID;
            else if (currContributor.IsVIP() == false)
                userID = "NotVIP_" + userID;

            string contributionsFolder = "Contributions\\";
            if (currContributor == null)
            {
                contributionsFolder = "Unknown" + contributionsFolder;
                userID = "Unknown_" + userID;
            }
            else if (currContributor.IsVIP() == false)
            {
                contributionsFolder = "NotVIP" + contributionsFolder;
                userID = "NotVIP_" + userID;
            }

            long fileFID = System.Threading.Interlocked.Increment(ref g_FileCounter);
            string fileName = userID + "_" + userIP + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_FID" + fileFID.ToString("0000");

            string debugResult = "";
            var temperedResult = DetermineTempered(_Data, out debugResult);
            string uploadStats = fileName + " @ Result: " + temperedResult.ToString() + ", Details: " + debugResult;

            Utility.AssertDirectory(uploadStatsFolder);
            System.IO.File.AppendAllText(uploadStatsFolder + userID + ".txt", uploadStats + "\r\n");
            
            if (temperedResult == TemperedStatus.OK)
            {
                Logger.ConsoleWriteLine("Data Received \"" + fileName + "\"", ConsoleColor.Green);
            }
            else if (temperedResult == TemperedStatus.Possibly_Tempered)
            {
                Logger.ConsoleWriteLine("Possibly Tempered Data Received \"" + fileName + "\"", ConsoleColor.Yellow);
            }
            else//if (temperedResult == TemperedStatus.Tempered)
            {
                contributionsFolder = "Tempered" + contributionsFolder;
                Logger.ConsoleWriteLine("Tempered Data Received \"" + fileName + "\"", ConsoleColor.Red);
            }

            string fullFilename = uploadDataFolder + contributionsFolder + fileName + ".txt";
            Utility.AssertFilePath(fullFilename);
            System.IO.File.WriteAllText(fullFilename, _Data.Data);

            if (currContributor != null && currContributor.IsVIP() == true && temperedResult != TemperedStatus.Tempered)
            {
                if (_Data.AddonName == "VF_RealmPlayers")
                {
                    Program.g_RPPDatabaseHandler.AddContribution(new RPPContribution(currContributor, fullFilename));
                }
                else if (_Data.AddonName == "VF_RaidDamage")
                {
                    Program.g_RDDatabaseHandler.AddContribution(fullFilename);
                }
                else if (_Data.AddonName == "VF_RealmPlayersTBC")
                {
                    Program.g_RPPDatabaseHandler.AddContribution(new RPPContribution(currContributor, fullFilename));
                }
                else if (_Data.AddonName == "VF_RaidStatsTBC")
                {
                    Program.g_RDDatabaseHandler.AddContribution(fullFilename);
                }
            }
        }
        public static void HandleUnhandledFiles(string _AddonName)
        {
            string uploadDataFolder = g_AddonUploadDataFolder + _AddonName + "\\Contributions\\";
            if (System.IO.Directory.Exists(uploadDataFolder) == false)
                return;

            string[] files = System.IO.Directory.GetFiles(uploadDataFolder);
            foreach(var file in files)
            {
                Logger.ConsoleWriteLine("Added unhandled file: \"" + file + "\"", ConsoleColor.Cyan);
                if (_AddonName == "VF_RealmPlayers")
                {
                    string[] fileName = System.IO.Path.GetFileNameWithoutExtension(file).Split('_');
                    if (fileName.Length == 9)
                    {
                        string userID = fileName[0];
                        string userIP = fileName[1];
                        var currContributor = ContributorDB.GetContributor(userID, System.Net.IPAddress.Parse(userIP), false);
                        Program.g_RPPDatabaseHandler.AddContribution(new RPPContribution(currContributor, file));
                    }
                }
                else if (_AddonName == "VF_RealmPlayersTBC")
                {
                    string[] fileName = System.IO.Path.GetFileNameWithoutExtension(file).Split('_');
                    if (fileName.Length == 9)
                    {
                        string userID = fileName[0];
                        string userIP = fileName[1];
                        var currContributor = ContributorDB.GetContributor(userID, System.Net.IPAddress.Parse(userIP), false);
                        Program.g_RPPDatabaseHandler.AddContribution(new RPPContribution(currContributor, file));
                    }
                }
                else if (_AddonName == "VF_RaidDamage")
                {
                    Program.g_RDDatabaseHandler.AddContribution(file);
                }
                else if (_AddonName == "VF_RaidStatsTBC")
                {
                    Program.g_RDDatabaseHandler.AddContribution(file);
                }
            }
        }
        enum TemperedStatus
        {
            OK,
            Possibly_Tempered,
            Tempered,
        }
        private static TemperedStatus DetermineTempered(WLN_UploadPacket_AddonData _Data, out string _DebugResult)
        {
            //First x files is other lua files, Last 6 is config files, 
            //If this is ever changed, remember to change on client aswell

            //Negative values means file is OLDER than Addon Data File
            //Positive values means file is NEWER than Addon Data File

            string fileResultStr = "";
            int nonTemperedScore = 0;
            bool confusingResult = false;
            int wdbFiles = 0;
            for (int i = 0; i < _Data.FileStatus.Length; ++i)
            {
                short currValue = _Data.FileStatus[i];
                if (currValue == short.MaxValue)
                    fileResultStr += "MAX, ";
                else if (currValue == short.MinValue)
                    fileResultStr += "MIN, ";
                else
                    fileResultStr += currValue + ", ";

                int voucherValue = 0;
                if (currValue > -5 && currValue < 5)
                {
                    //Perfect balance, if the file is within this range the chance of file being tempered is extremely low
                    voucherValue = 5;
                }
                else if (currValue > -15 && currValue < 15)
                {
                    //The superb threshold used so far
                    voucherValue = 3;
                }
                else if (currValue > -30 && currValue < 30)
                {
                    //The normal threshold used so far
                    voucherValue = 1;
                }
                else if (currValue <= -30)
                {
                    //Big chance of file being tempered
                    //But this lua file addon might be disabled
                }
                else if (currValue >= 30 && i < _Data.FileStatus.Length - 6)
                {
                    //Confusing result. This means that something weird might be up
                    confusingResult = true;
                }
                if (i >= _Data.FileStatus.Length - 6)
                {
                    voucherValue *= 2;
                    if (currValue != short.MinValue)
                        ++wdbFiles;
                }

                nonTemperedScore += voucherValue;
            }

            _DebugResult = nonTemperedScore + (confusingResult ? "?confusing?" : "") + " = {" + fileResultStr + "}";

            if (wdbFiles == 6)
            {
                if (nonTemperedScore > 20)
                    return TemperedStatus.OK;
                else if (nonTemperedScore > 10)
                    return TemperedStatus.Possibly_Tempered;
                else
                    return TemperedStatus.Tempered;
            }
            else
            {
                if (nonTemperedScore > 10)
                    return TemperedStatus.OK;
                else if (nonTemperedScore > 5)
                    return TemperedStatus.Possibly_Tempered;
                else
                    return TemperedStatus.Tempered;
            }
        }
    }
}
