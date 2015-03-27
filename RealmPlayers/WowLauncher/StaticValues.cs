using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VF_WoWLauncher
{
    class StaticValues
    {
        //public static string LauncherVersion = "1.0";
        public static string LauncherVersion = System.Windows.Forms.Application.ProductVersion;
        public static CMDArguments StartupArguments = null;

        public static string ProfileDirectory = "VF_WowLauncherSettings/Profiles/";
        public static string LauncherSettingsDirectory = "VF_WowLauncherSettings/";
        public static string LauncherToolsDirectory = "VF_WowLauncherTools/";
        public static string LauncherBackupsDirectory = "VF_WowLauncherBackups/";
        public static string LauncherDownloadsDirectory = "VF_WowLauncherDownloads/";

        public static string LauncherExecuteFile = System.Windows.Forms.Application.ExecutablePath;
        public static string LauncherWorkDirectory = System.Windows.Forms.Application.StartupPath;

        public static string RealmListWTF_EmeraldDream = "set realmlist vanillafeenix.servegame.org\r\nset realmname \"Emerald Dream [1x] Blizzlike\"";
        public static string RealmListWTF_Warsong = "set realmlist vanillafeenix.servegame.org\r\nset realmname \"Warsong [12x] Blizzlike\"";
        public static string RealmListWTF_AlAkir = "set realmlist vanillafeenix.servegame.org\r\nset realmname \"Al'Akir [instant 60] Blizzlike\"";
        public static string RealmListWTF_Archangel = "set realmlist vanillafeenix.servegame.org\r\nset realmname \"Archangel [14x] Blizzlike\"";
        public static string RealmListWTF_VanillaGaming = "set realmlist logon.vanillagaming.org\r\nset realmname \"VanillaGaming\"";
        public static string RealmListWTF_Valkyrie = "set realmlist logon.valkyrie-wow.com\r\nset realmname \"Valkyrie\"";
        public static string RealmListWTF_Rebirth = "set realmlist wow.therebirth.net\r\nset realmname \"Rebirth\"";
        public static string RealmListWTF_Nostalrius = "set realmlist login.nostalrius.org\r\nset realmname \"Nostalrius Begins\"";
        public static string RealmListWTF_Kronos = "set realmlist wow.twinstar.cz\r\nset realmname \"Kronos\"";
        //public static string RealmListWTF_NostalGeek = "set realmlist login.nostalrius.org\r\nset realmname \"NostalGeek 1.12\"";

        public static string RunWowAndUploaderBatFileData = "start /b /WAIT /d %1 Wow.exe\r\nstart \"\" /d \"" + StaticValues.LauncherWorkDirectory + "\" \"" + StaticValues.LauncherExecuteFile + "\" /RealmPlayersUploader";
        public static string RunWowNotAdminAndUploaderBatFileData = "";//NotAdmin.exe "cmd.exe" ".\\" "/c \"\"D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Release\\VF_WowLauncherTools\\RunWowAndUploader.bat\" \"D:\Program\World of Warcraft Classic\\\" nowindow\"";
        public static string RunwyUpdateAndLauncherFileData = "start /b /WAIT /d %1 wyUpdate.exe /skipinfo\r\nstart \"\" /d \"" + StaticValues.LauncherWorkDirectory + "\" \"" + StaticValues.LauncherExecuteFile + "\"";

        public static string RunStuffs = "start \"\" /d \"D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Release\\VF_WowLauncherTools\" \"RunWowAndUploader.bat\" \"D:\\Program\\World of Warcraft Classic\"";

        public static List<string> Resolutions = new List<string>
        {                             
            "2560x1600",
            "2560x1440",
            "1920x1200",
            "1920x1080",
            "1600x1200",
            "1600x1050",
            "1600x900",
            "1440x900",
            "1280x800",
            "1280x768",
            "1280x720",
            "1024x768",
            "800x600",
        };
        public static Dictionary<string, int> ScriptMemorys = new Dictionary<string,int>
        {
            {"16MB", 16*1024},
            {"32MB", 32*1024},
            {"64MB", 64*1024},
            {"96MB", 96*1024},
            {"128MB", 128*1024},
            {"256MB", 256*1024},
            {"384MB", 384*1024},
            {"512MB", 512*1024},
        };
        private static Func<string, bool> _WowBoolValidator = (string _Value) => { return _Value == "1" || _Value == "0"; };
        private static Func<string, bool> _WowUIntValidator = (string _Value) => 
        { 
            int temp;
            if (int.TryParse(_Value, out temp) == true)
                return temp >= 0;
            return false;
        };
        public static Dictionary<string, Func<string, bool>> SettingsValidateFunctions = new Dictionary<string, Func<string, bool>>{
            //BOOLS
            {"gxMaximize", _WowBoolValidator},
            {"gxWindow", _WowBoolValidator},
            {"gxFixLag", _WowBoolValidator},
            {"hwDetect", _WowBoolValidator},
            {"DesktopGamma", _WowBoolValidator},
            {"profanityFilter", _WowBoolValidator},
            {"useUiScale", _WowBoolValidator},
            {"UnitNameOwn", _WowBoolValidator},
            //BOOLS
            
            //UINT
            {"gxRefresh", _WowUIntValidator},
            {"scriptMemory", _WowUIntValidator},
            {"CombatLogRangePartyPet", _WowUIntValidator},
            {"CombatLogRangeFriendlyPlayers", _WowUIntValidator},
            {"CombatLogRangeFriendlyPlayersPets", _WowUIntValidator},
            {"CombatLogRangeHostilePlayers", _WowUIntValidator},
            {"CombatLogRangeHostilePlayersPets", _WowUIntValidator},
            {"CombatLogRangeCreature", _WowUIntValidator},
            //UINT

            //CUSTOM
            {"gxResolution", 
                (string _Value) => 
                {
                    string[] xyValues = _Value.Split('x');
                    if (xyValues.Length != 2)
                        return false;
                    return _WowUIntValidator(xyValues[0]) && _WowUIntValidator(xyValues[1]);
                }
            },
            //CUSTOM
        };


        public static Dictionary<string, string> RealmNameConverter = new Dictionary<string, string>
        {
            {"Warsong [12x] Blizzlike", "Warsong"},
            {"Al'Akir [instant 60] Blizzlike", "Al'Akir"},
            {"Emerald Dream [1x] Blizzlike", "Emerald Dream"},
            {"Archangel [14x] Blizzlike", "Archangel(TBC)"},
            {"NostalGeek 1.12", "NostalGeek"},
            {"Nostalrius Begins", "Nostalrius"},
        };
    }
}
