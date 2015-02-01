using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

using System.Windows.Shell;
namespace VF_WoWLauncher
{
    class WoWLauncherApp : System.Windows.Application
    {
        [DllImport("user32.dll")]
        private static extern int RegisterWindowMessage(string msgName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public void InitiateMessageIDs()
        {
            m_MESSAGEID_ResetWindowPosition = RegisterWindowMessage("WoWLauncherApp.ResetWindowPosition");
        }
        private JumpList m_JumpList = null;
        public void InitiateJumpList()
        {
            if (Environment.OSVersion.Version.Major > 6 //6 == Vista och Windows 7
            || (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1)) // 6.1 == Windows 7
            {
                m_JumpList = new JumpList();
                {
                    var jumpItem = new JumpTask();
                    jumpItem.Title = "Reset Position";
                    jumpItem.Description = "Resets the window to the center of the primary display.";
                    jumpItem.CustomCategory = "";
                    jumpItem.ApplicationPath = StaticValues.LauncherExecuteFile;
                    jumpItem.IconResourcePath = StaticValues.LauncherExecuteFile;
                    jumpItem.Arguments = "/ResetWindowPosition";
                    jumpItem.WorkingDirectory = StaticValues.LauncherWorkDirectory;
                    m_JumpList.JumpItems.Add(jumpItem);
                }

                foreach (var launchShortcut in Settings.Instance.ShortcutLaunches)
                {
                    AddJumpListLaunchCommand(launchShortcut);
                }
                System.Windows.Shell.JumpList.SetJumpList(Program.g_LauncherApp, m_JumpList);
                m_JumpList.Apply();
            }
        }
        public void AddJumpListLaunchCommand(Settings.LaunchShortcut _LaunchShortcut)
        {
            var jumpItem = new JumpTask();
            jumpItem.Title = _LaunchShortcut.ShortcutName;
            jumpItem.Description = "Launches WoW with the config profile \"" + _LaunchShortcut.Profile + "\" and the realm \"" + _LaunchShortcut.Realm + "\"";
            jumpItem.CustomCategory = "Launch";
            jumpItem.ApplicationPath = StaticValues.LauncherExecuteFile;
            jumpItem.IconResourcePath = StaticValues.LauncherExecuteFile;
            jumpItem.Arguments = "/LaunchWoW \"" + _LaunchShortcut.Realm + "\" /ConfigProfile \"" + _LaunchShortcut.Profile + "\"";
            jumpItem.WorkingDirectory = StaticValues.LauncherWorkDirectory;
            m_JumpList.JumpItems.Add(jumpItem);
            m_JumpList.Apply();
        }
        private int m_MESSAGEID_ResetWindowPosition = 0;
        
        public bool HandleJumpListMessages(LauncherWindow _LauncherWindow, ref System.Windows.Forms.Message _Message)
        {
            if (_LauncherWindow == null)
                return false;

            if (_Message.Msg == m_MESSAGEID_ResetWindowPosition)
            {
                _LauncherWindow.Top = Screen.PrimaryScreen.WorkingArea.Height / 2 - _LauncherWindow.Height / 2;
                _LauncherWindow.Left = Screen.PrimaryScreen.WorkingArea.Width / 2 - _LauncherWindow.Width / 2;
                return true;
            }
            return false;
        }
        public void HandleJumpListCommands(LauncherWindow _LauncherWindow, CMDArguments _Args)
        {
            IntPtr otherProcessPtr = IntPtr.Zero;
            if (_LauncherWindow == null)
            {
                otherProcessPtr = Program.FocusOtherProcess();
            }
            if (_Args["ResetWindowPosition"] != null)
            {
                if (_LauncherWindow != null)
                {
                    Settings.Instance.LauncherWindow_Left = -1;
                    Settings.Instance.LauncherWindow_Top = -1;
                }
                else
                {
                    if (otherProcessPtr != IntPtr.Zero)
                    {
                        SendMessage(otherProcessPtr, m_MESSAGEID_ResetWindowPosition, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
        }
    }
    static class Program
    {
        public static WoWLauncherApp g_LauncherApp = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            g_LauncherApp = new WoWLauncherApp();
            g_LauncherApp.InitiateMessageIDs();
            try
            {
                try
                {
                    StaticValues.StartupArguments = new CMDArguments(args);
                    if (StaticValues.StartupArguments["RealmPlayersUploader"] != null) // /RealmPlayersUploader
                    {
                        Settings.Initialize();
                        ConsoleUtility.CreateConsole();
                        if (RealmPlayersUploader.IsValidUserID(Settings.UserID) == true)
                        {
                            bool sentAll;

                            if (Settings.HaveClassic == true)
                            {
                                Logger.ConsoleWriteLine("Starting to send VF_RealmPlayers Files", ConsoleColor.White);
                                var sentRealmPlayersFiles = ServerComm.SendAddonData(Settings.UserID, "VF_RealmPlayers", WowVersionEnum.Vanilla, "VF_RealmPlayersData", 50, out sentAll);
                                foreach (var file in sentRealmPlayersFiles)
                                    Logger.ConsoleWriteLine("Sent VF_RealmPlayers File \"" + file + "\"", ConsoleColor.Green);

                                Logger.ConsoleWriteLine("Starting to send VF_RaidDamage Files", ConsoleColor.White);
                                var sentRaidDamageFiles = ServerComm.SendAddonData(Settings.UserID, "VF_RaidDamage", WowVersionEnum.Vanilla, "VF_RaidDamageData", 5000, out sentAll);
                                foreach (var file in sentRaidDamageFiles)
                                    Logger.ConsoleWriteLine("Sent VF_RaidDamage File \"" + file + "\"", ConsoleColor.Cyan);
                            }
                            if(Settings.HaveTBC == true)
                            {
                                Logger.ConsoleWriteLine("Starting to send VF_RealmPlayersTBC Files", ConsoleColor.White);
                                var sentRealmPlayersTBCFiles = ServerComm.SendAddonData(Settings.UserID, "VF_RealmPlayersTBC", WowVersionEnum.TBC, "VF_RealmPlayersData", 50, out sentAll);
                                foreach (var file in sentRealmPlayersTBCFiles)
                                    Logger.ConsoleWriteLine("Sent VF_RealmPlayersTBC File \"" + file + "\"", ConsoleColor.Cyan);

                                Logger.ConsoleWriteLine("Starting to send VF_RaidStatsTBC Files", ConsoleColor.White);
                                var sentRaidStatsTBCFiles = ServerComm.SendAddonData(Settings.UserID, "VF_RaidStatsTBC", WowVersionEnum.TBC, "VF_RaidStatsData", 5000, out sentAll);
                                foreach (var file in sentRaidStatsTBCFiles)
                                    Logger.ConsoleWriteLine("Sent VF_RaidStatsTBC File \"" + file + "\"", ConsoleColor.Cyan);
                            }
                        }
                        else
                        {
                            Logger.ConsoleWriteLine("UserID: " + Settings.UserID + " was not a valid UserID!");
                        }
                        Logger.ConsoleWriteLine("Closing in 5 seconds!", ConsoleColor.White);
                        System.Threading.Thread.Sleep(5000);
                        return;
                    }
                    else if (StaticValues.StartupArguments["LaunchWow"] != null)
                    {
                        Settings.Initialize();
                        ConsoleUtility.CreateConsole();
                        string useRealm = StaticValues.StartupArguments["LaunchWow"];
                        string useConfigProfile = "Active Wow Config";
                        if (StaticValues.StartupArguments["ConfigProfile"] != null)
                            useConfigProfile = StaticValues.StartupArguments["ConfigProfile"];

                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        
                        var wowVersion = WowVersionEnum.Vanilla;
                        if (useRealm == "Archangel(TBC)")
                        {
                            useConfigProfile = "Active Wow Config";
                            wowVersion = WowVersionEnum.TBC;
                            if (Settings.Instance.ClearWDB == true)
                            {
                                Utility.DeleteDirectory(Settings.GetWowDirectory(wowVersion) + "Cache");
                            }
                        }
                        else
                        {
                            if (Settings.Instance.ClearWDB == true)
                            {
                                Utility.DeleteDirectory(Settings.GetWowDirectory(wowVersion) + "WDB");
                            }
                        }

                        if (RealmPlayersUploader.IsValidUserID(Settings.UserID) == true)
                        {
                            if (System.IO.File.Exists(StaticValues.LauncherToolsDirectory + "RunWowAndUploader.bat") == false
                                || System.IO.File.ReadAllText(StaticValues.LauncherToolsDirectory + "RunWowAndUploader.bat") != StaticValues.RunWowAndUploaderBatFileData)
                            {
                                Utility.AssertDirectory(StaticValues.LauncherToolsDirectory);
                                System.IO.File.WriteAllText(StaticValues.LauncherToolsDirectory + "RunWowAndUploader.bat", StaticValues.RunWowAndUploaderBatFileData);
                            }

                            if (Settings.Instance.RunWoWNotAdmin == false)
                            {
                                //startInfo.FileName = Settings.WowDirectory + "WoW.exe";
                                //startInfo.WorkingDirectory = Settings.WowDirectory;

                                startInfo.FileName = StaticValues.LauncherToolsDirectory + "RunWowAndUploader.bat";
                                //startInfo.FileName = Settings.WowDirectory + "22VF_RealmPlayersUploader 1.5\\RunWoWAndUploaderNoCMDWindow.vbs";
                                //startInfo.WorkingDirectory = Settings.WowDirectory + "22VF_RealmPlayersUploader 1.5\\";
                                startInfo.Arguments = "\"" + Settings.GetWowDirectory(wowVersion) + "\"";
                            }
                            else
                            {
                                string slash = "\\\\";
                                string snuff = "\\\"";

                                startInfo.FileName = StaticValues.LauncherToolsDirectory + "NotAdmin.exe";
                                startInfo.Arguments = "\"cmd.exe\" \".\\\\\" "
                                    + "\"/c "
                                        + snuff
                                            + snuff + StaticValues.LauncherWorkDirectory.Replace("\\", slash) + "/" + StaticValues.LauncherToolsDirectory.Replace("\\", slash) + "RunWowAndUploader.bat" + snuff
                                            + " " + snuff + Settings.GetWowDirectory(wowVersion) + "\\" + snuff
                                        + snuff
                                    + "\" nowindow";
                            }
                            startInfo.UseShellExecute = false;
                            startInfo.RedirectStandardOutput = false;
                            startInfo.CreateNoWindow = true;
                        }
                        else
                        {
                            startInfo.FileName = Settings.GetWowDirectory(wowVersion) + "WoW.exe";
                            startInfo.WorkingDirectory = Settings.GetWowDirectory(wowVersion);
                        }
                        Logger.ConsoleWriteLine("Starting to Launch WoW for realm: \"" + useRealm + "\", with ConfigProfile: \"" + useConfigProfile + "\"", ConsoleColor.Green);
                        LaunchFunctions.LaunchWow(useConfigProfile, useRealm, startInfo);
                        Logger.ConsoleWriteLine("Done with Launching WoW! Closing console window in 5 seconds!", ConsoleColor.White);
                        System.Threading.Thread.Sleep(5000);
                        return;
                    }

                    g_AppMutex = new Mutex(true, "Local\\VF_WoWLauncher");
                    if (g_AppMutex == null || g_AppMutex.WaitOne(TimeSpan.Zero, true))
                    {
                        Settings.Initialize();

                        if (Settings.DebugMode)
                        {
                            //ConsoleUtility.CreateConsole();
                        }

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        var launcherWindow = new LauncherWindow();
                        g_LauncherApp.HandleJumpListCommands(launcherWindow, StaticValues.StartupArguments);
                        Application.Run(launcherWindow);
                        if (g_AppMutex != null)
                        {
                            ForumReader.SaveForumSections();
                            Settings.Save();
                            ReleaseAppMutex();
                        }
                    }
                    else
                    {
                        g_AppMutex = null;
                        g_LauncherApp.HandleJumpListCommands(null, StaticValues.StartupArguments);
                    }
                }
                catch (Exception ex)
                {
                    ReleaseAppMutex();
                    Utility.MessageBoxShow("Exception occured! Please printscreen this message and send to Dilatazu @ realmplayers forums:\r\n" + ex.ToString());
                    ConsoleUtility.CreateConsole();
                    Logger.LogException(ex);
                    Logger.ConsoleWriteLine("Closing in 10 seconds!", ConsoleColor.White);
                    System.Threading.Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                ReleaseAppMutex();
                Utility.MessageBoxShow("Very unexpected Exception occured! Please printscreen this message and send to Dilatazu @ realmplayers forums:\r\n" + ex.ToString());
            }
            ReleaseAppMutex();
        }

        private static Mutex g_AppMutex = null;
        private static void ReleaseAppMutex()
        {
            if (g_AppMutex != null)
            {
                g_AppMutex.ReleaseMutex();
                g_AppMutex = null;
            }
        }


        [DllImport("user32")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32")]
        static extern int ShowWindow(IntPtr hWnd, int swCommand);
        [DllImport("user32")]
        static extern bool IsIconic(IntPtr hWnd);

        public static IntPtr FocusOtherProcess()
        {
            Process proc = Process.GetCurrentProcess();

            // Using Process.ProcessName does not function properly when
            // the actual name exceeds 15 characters. Using the assembly 
            // name takes care of this quirk and is more accurate than 
            // other work arounds.

            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            foreach (Process otherProc in Process.GetProcessesByName(assemblyName))
            {
                //ignore "this" process, and ignore wyUpdate with a different filename

                if (proc.Id != otherProc.Id
                        && otherProc.MainModule != null && proc.MainModule != null
                        && proc.MainModule.FileName == otherProc.MainModule.FileName)
                {
                    // Found a "same named process".
                    // Assume it is the one we want brought to the foreground.
                    // Use the Win32 API to bring it to the foreground.

                    IntPtr hWnd = otherProc.MainWindowHandle;

                    if (IsIconic(hWnd))
                        ShowWindow(hWnd, 9); //SW_RESTORE

                    SetForegroundWindow(hWnd);
                    return hWnd;
                }
            }
            return IntPtr.Zero;
        }
    }
}
