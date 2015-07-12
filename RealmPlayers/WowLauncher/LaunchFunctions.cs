using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VF_WoWLauncher
{
    class LaunchFunctions
    {
        //private static System.Collections.Concurrent.ConcurrentQueue<Tuple<string, string, ProcessStartInfo>> g_LaunchQueue = new System.Collections.Concurrent.ConcurrentQueue<Tuple<string,string,ProcessStartInfo>>();
        public static void LaunchWow(string _Config, string _RealmName, ProcessStartInfo _WowProcessStartInfo)
        {
            //g_LaunchQueue.Enqueue(Tuple.Create(_Config, _RealmName, _WowProcessStartInfo));
            _LaunchWow(_Config, _RealmName, _WowProcessStartInfo);
        }
        private static void _LaunchWow(string _Config, string _RealmName, ProcessStartInfo _WowProcessStartInfo)
        {
            if (System.IO.File.Exists("VF_WowLauncherTools\\NotAdmin.exe") == false)
            {
                VF_Utility.AssertDirectory("VF_WowLauncherTools");
                System.IO.File.WriteAllBytes("VF_WowLauncherTools\\NotAdmin.exe", Properties.Resources.NotAdmin);
            }

            Logger.LogText("Started Launching");

            WowVersionEnum wowVersion = WowVersionEnum.Vanilla;
            string realmListFile = "";
            if(Settings.Instance.RealmLists.ContainsKey(_RealmName) == true)
            {
                var realmList = Settings.Instance.RealmLists[_RealmName];
                realmListFile = realmList.GetRealmListWTF();
                wowVersion = realmList.WowVersion;
            }
            else
            {
                Logger.LogText("Unexpected Error: No such Realm exists!");
                return;
            }

            if (_Config != "Active Wow Config")
            {
                ConfigWTF config = ConfigProfiles.GetProfileConfigFile(_Config);
                config.SaveWTFConfigFile(wowVersion);
                Settings.Instance.AddLaunchShortcut(_Config, _RealmName);
            }

            DateTime startCheck = DateTime.UtcNow;
            if (System.IO.File.Exists(Settings.GetWowDirectory(wowVersion) + "realmlist.wtf") == false || System.IO.File.ReadAllText(Settings.GetWowDirectory(wowVersion) + "realmlist.wtf") != realmListFile)
            {
                System.IO.File.WriteAllText(Settings.GetWowDirectory(wowVersion) + "realmlist.wtf", realmListFile);
                if (System.IO.File.Exists(Settings.GetWowDirectory(wowVersion) + "realmlist.wtf") == false)
                {
                    Logger.LogText("Unexpected Error: There is no realmlist.wtf!");
                    return;
                }
                Logger.LogText("Waiting for realmlist.wtf to get saved correctly");
                startCheck = DateTime.UtcNow;
                while ((startCheck - System.IO.File.GetLastWriteTimeUtc(Settings.GetWowDirectory(wowVersion) + "realmlist.wtf")).Seconds > 10)
                {
                    Logger.LogText(".", false);
                    System.Threading.Thread.Sleep(20);
                    if ((DateTime.UtcNow - startCheck).Seconds > 10)
                    {
                        Logger.LogText("Unexpected Error: Took too long trying to create the new realmlist.wtf!");
                        return;
                    }
                }
            }
            Logger.LogText("realmlist.wtf is saved, Launching World of Warcraft!");
            Process[] processesBeforeStart = Process.GetProcessesByName("Wow");
            var wowProcess = Process.Start(_WowProcessStartInfo);
            if (_WowProcessStartInfo.FileName.ToLower().EndsWith("wow.exe") == false)
            {
                wowProcess = null;
                while (wowProcess == null && (DateTime.UtcNow - startCheck).TotalSeconds < 30)
                {
                    Utility.SoftThreadSleep(500);
                    Process[] currentProcesses = Process.GetProcessesByName("Wow");
                    foreach (var currProcess in currentProcesses)
                    {
                        if (processesBeforeStart.Length == 0 || processesBeforeStart.FirstOrDefault((_Value) => _Value.Id == currProcess.Id) == null)
                        {
                            //Logger.LogText("found new Wow.exe Process!");
                            wowProcess = currProcess;
                            break;
                        }
                    }
                }
            }
            if (wowProcess == null)
            {
                Utility.MessageBoxShow("There was an error, WoW could not be launched");
                return;
            }
            startCheck = DateTime.UtcNow;
            try
            {
                while (wowProcess.WaitForInputIdle(50) == false && (DateTime.UtcNow - startCheck).TotalSeconds < 20)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            //wait 1 extra second
            Utility.SoftThreadSleep(1000);

            Logger.LogText("Done Launching");
        }
        //public void test()
        //{
        //    var directories = System.IO.Directory.GetDirectories("VersionHistory\\");

        //    int highestMajor = 0;
        //    int highestMinor = 0;
        //    int highestBuild = 0;
        //    string highestDir = "";

        //    foreach (var directory in directories)
        //    {
        //        try
        //        {
        //            string versionNumber = directory.Split('\\').Last();
        //            string[] versionsStr = versionNumber.Split('.');
        //            if (versionsStr.Length == 3)
        //            {
        //                int majorVersion = int.Parse(versionsStr[0]);
        //                int minorVersion = int.Parse(versionsStr[1]);
        //                int buildVersion = int.Parse(versionsStr[2]);
        //                if (majorVersion > highestMajor)
        //                {
        //                    highestMajor = majorVersion;
        //                    highestMinor = minorVersion;
        //                    highestBuild = buildVersion;
        //                    highestDir = directory;
        //                }
        //                else if (majorVersion == highestMajor)
        //                {
        //                    if (minorVersion > highestMinor)
        //                    {
        //                        highestMajor = majorVersion;
        //                        highestMinor = minorVersion;
        //                        highestBuild = buildVersion;
        //                        highestDir = directory;
        //                    }
        //                    else if (minorVersion == highestMinor)
        //                    {
        //                        if (buildVersion > highestBuild)
        //                        {
        //                            highestMajor = majorVersion;
        //                            highestMinor = minorVersion;
        //                            highestBuild = buildVersion;
        //                            highestDir = directory;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {}
        //    }
        //}
    }
}
