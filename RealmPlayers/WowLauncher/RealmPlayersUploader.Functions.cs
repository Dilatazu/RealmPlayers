using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_WoWLauncher
{
    partial class RealmPlayersUploader
    {
        public static bool ConvertBool(string _String)
        {
            string toLower = _String.ToLower();
            if (toLower == "false" || toLower == "nil" || toLower == "0")
                return false;
            return true;
        }
        public static void LogMessage(string _Message)
        {
            if (System.IO.File.Exists("ErrorLog.txt") == false)
                System.IO.File.WriteAllText("ErrorLog.txt", "Creating file");
            var logFile = System.IO.File.AppendText("ErrorLog.txt");
            logFile.WriteLine(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ": " + _Message + "\r\n\r\n");
            logFile.Close();
        }
        static bool CreateNewLog = true;
        public static void ConsoleWriteLine(string _Message)
        {
            string fullMessage = DateTime.Now.ToString("HH_mm_ss") + ": " + _Message;
            Console.WriteLine(fullMessage);
            if (System.IO.File.Exists("Log.txt") == false || CreateNewLog == true)
            {
                CreateNewLog = false;
                System.IO.File.WriteAllText("Log.txt", fullMessage + "\r\n");
            }
            else
            {
                var logFile = System.IO.File.AppendText("Log.txt");
                logFile.WriteLine(fullMessage);
                logFile.Close();
            }
        }
        public static string GetWowDirectory(string _ExampleWTFLuaPath)
        {
            if (_ExampleWTFLuaPath.Contains("WTF") == false || _ExampleWTFLuaPath.Contains("Account") == false || _ExampleWTFLuaPath.Contains("SavedVariables") == false || _ExampleWTFLuaPath.Contains("VF_RealmPlayers.lua") == false)
                return "";

            return _ExampleWTFLuaPath.Substring(0, _ExampleWTFLuaPath.IndexOf("WTF"));
        }
        public static string GetRealmPlayerAddonFolderPath(string _ExampleWTFLuaPath)
        {
            return GetWowDirectory(_ExampleWTFLuaPath) + "Interface\\AddOns\\VF_RealmPlayers\\";
        }
        public static void LogException(Exception _Ex)
        {
            LogMessage(_Ex.ToString());
        }
        public static bool IsValidUserID(string userID)
        {
            if (userID == "Unknown.123456" || userID == "")
                return false;

            bool foundDot = false;
            foreach (char c in userID)
            {
                if (c == '.')
                {
                    if (foundDot == true)
                        return false;
                    foundDot = true;
                }
                else if (foundDot == false)
                {
                    if ((c >= 'a' && c <= 'z')
                    || (c >= 'A' && c <= 'Z'))
                    {
                        //Valid
                    }
                    else
                    {
                        //Invalid
                        return false;
                    }
                }
                else
                {
                    if (c >= '0' && c <= '9')
                    {
                        //Valid
                    }
                    else
                    {
                        //Invalid
                        return false;
                    }
                }
            }
            if (foundDot == false)
                return false;

            return true;
        }
        public static bool IsTemperedWith(string _LuaFile, double _MinuteThreshold = 0.5)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(_LuaFile);
            DateTime lastWriteTime = System.IO.File.GetLastWriteTime(_LuaFile);
            string savedVarDir = System.IO.Path.GetDirectoryName(_LuaFile);
            string[] files = System.IO.Directory.GetFiles(savedVarDir, "*.lua");
            int vouchingFiles = 0;
            int extraVouchingFiles = 0;
            foreach (string file in files)
            {
                double totalMins = (System.IO.File.GetLastWriteTime(file) - lastWriteTime).TotalMinutes;
                totalMins = Math.Abs(totalMins);
                if (totalMins < _MinuteThreshold)
                    vouchingFiles++;
                if (totalMins < 0.25)
                    extraVouchingFiles++;
            }
            if (vouchingFiles > 5 || extraVouchingFiles > 3)
                return false;
            else
            {
                List<string> extraCheckFiles = new List<string>();
                string wowDir = GetWowDirectory(_LuaFile);
                extraCheckFiles.Add(wowDir + "WTF\\Config.wtf");
                extraCheckFiles.Add(wowDir + "WDB\\creaturecache.wdb");
                extraCheckFiles.Add(wowDir + "WDB\\itemnamecache.wdb");
                extraCheckFiles.Add(wowDir + "WDB\\gameobjectcache.wdb");
                extraCheckFiles.Add(wowDir + "WDB\\npccache.wdb");
                extraCheckFiles.Add(wowDir + "WDB\\wowcache.wdb");
                foreach (string file in extraCheckFiles)
                {
                    double totalMins = (lastWriteTime - System.IO.File.GetLastWriteTime(file)).TotalMinutes;
                    //Om lua filen skrevs till tidigare än WDB filen så innebär det att filen inte är tempered
                    //därför om totalMins är minus värde så måste värdet vara mindre än _MinuteThreshold för att få igenom ifsatsen
                    //därför gör vi ingen abs här.
                    if (totalMins < _MinuteThreshold)
                        vouchingFiles++;
                    if (totalMins < 0.25)
                        extraVouchingFiles++;
                }
                if (vouchingFiles > 5 || extraVouchingFiles > 3)
                    return false;
            }
            return true;
        }
    }
}
