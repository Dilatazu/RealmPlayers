using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace VF_WoWLauncher
{
    [ProtoContract]
    public class Settings
    {
        private Settings() { }
        [ProtoMember(1)]
        public string DefaultConfig = "Active Wow Config";
        [ProtoMember(2)]
        public string DefaultRealm = "Emerald Dream";
        [ProtoMember(3)]
        public bool ClearWDB = false;
        [ProtoMember(4)]
        public TimeSpan BackupWTFTimeSpan = TimeSpan.FromDays(30);
        [ProtoMember(5)]
        public DateTime BackupWTFNextTime = DateTime.UtcNow.AddDays(30);
        [ProtoMember(6)]
        public string _UserID = "";
        [ProtoMember(7)]
        public string _WowDirectory = ""; //D:\\Program\\World Of Warcraft Classic\\
        [ProtoMember(8)]
        public int LauncherWindow_Left = -1;
        [ProtoMember(9)]
        public int LauncherWindow_Top = -1;
        //[ProtoMember(10)]
        //public bool UseWoWNoDelay = false;
        [ProtoMember(11)]
        public string _WowTBCDirectory = ""; //D:\\Program\\World Of Warcraft Classic\\
        [ProtoMember(12)]//, IsRequired = true
        public bool AutoRefreshNews = true;
        [ProtoMember(13)]
        public List<LaunchShortcut> ShortcutLaunches = new List<LaunchShortcut>();
        [ProtoMember(14)]
        public bool UpdateToBeta = false;
        [ProtoMember(15)]
        public bool RunWoWNotAdmin = false;
        [ProtoMember(16)]
        public bool AutoHideOldNews = true;

        [ProtoContract]
        public class LaunchShortcut
        {
            [ProtoMember(1)]
            public string ShortcutName;
            [ProtoMember(2)]
            public string Profile;
            [ProtoMember(3)]
            public string Realm;
        }
        public void AddLaunchShortcut(string _Profile, string _Realm)
        {
            var existingShortcutIndex = ShortcutLaunches.FindIndex((_Value) => _Value.Profile == _Profile && _Value.Realm == _Realm);
            if (existingShortcutIndex == -1)
            {
                ShortcutLaunches.Add(new LaunchShortcut { ShortcutName = _Profile + " @ " + _Realm, Profile = _Profile, Realm = _Realm });
            }
            else
            {
                var newItem = ShortcutLaunches[existingShortcutIndex];
                ShortcutLaunches.Remove(newItem);
                ShortcutLaunches.Add(newItem);
            }
            if (ShortcutLaunches.Count > 5)
                ShortcutLaunches.RemoveAt(0);
        }
        public static string UserID
        {
            get
            {
                return Instance._UserID;
            }
        }
        private static string WowDirectory
        {
            get
            {
                return Instance._WowDirectory;
            }
        }
        private static string WowTBCDirectory
        {
            get
            {
                return Instance._WowTBCDirectory;
            }
        }
        public static bool HaveClassic
        {
            get
            {
                return Instance._WowTBCDirectory != Instance._WowDirectory;
            }
        }
        public static bool HaveTBC
        {
            get
            {
                return Instance._WowTBCDirectory != "";
            }
        }
        public static bool DebugMode
        {
            get
            {
                return Instance._UserID.StartsWith("Dilatazu") && (StaticValues.LauncherWorkDirectory.ToLower().Contains("release") || StaticValues.LauncherWorkDirectory.ToLower().Contains("debug"));
            }
        }
        public static bool FirstTimeRunning = false;
        private static Settings sm_Settings = Instance;
        public static Settings Instance
        {
            get
            {
                if (sm_Settings == null)
                {
                    if (System.IO.File.Exists(StaticValues.LauncherSettingsDirectory + "Settings.cfg") == true)
                    {
                        FirstTimeRunning = false;
                        Settings settingsData = null;
                        Utility.LoadSerialize(StaticValues.LauncherSettingsDirectory + "Settings.cfg", out settingsData);
                        sm_Settings = settingsData;
                    }
                    else
                    {
                        FirstTimeRunning = true;
                        sm_Settings = new Settings();
                    }
                }
                return sm_Settings;
            }
        }
        public static void Initialize()
        {
            var triggerLoad = Instance;
        }
        public static void Uninitialize()
        {
            sm_Settings = null;
        }
        public static void Save()
        {
            Utility.AssertDirectory(StaticValues.LauncherSettingsDirectory);
            Utility.SaveSerialize(StaticValues.LauncherSettingsDirectory + "Settings.cfg", sm_Settings, false);
        }

        internal static string GetWowDirectory(WowVersionEnum _WowVersion)
        {
            if (_WowVersion == WowVersionEnum.TBC && HaveTBC == true)
                return WowTBCDirectory;
            else
                return WowDirectory;
        }
    }
}
