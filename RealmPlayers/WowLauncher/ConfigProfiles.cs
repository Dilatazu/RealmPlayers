using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_WoWLauncher
{
    public class ConfigProfiles
    {
        public class ConfigProfile
        {
            
        }
        public Dictionary<string, ConfigProfile> m_ConfigProfiles = new Dictionary<string, ConfigProfile>();

        public static List<string> GetProfileNames()
        {
            Utility.AssertDirectory("VF_WowLauncherSettings/Profiles/");
            string[] directories = System.IO.Directory.GetDirectories("VF_WowLauncherSettings/Profiles");
            List<string> profileNames = new List<string>();
            foreach (string directory in directories)
            {
                if(System.IO.File.Exists(directory + "\\Config.wtf"))
                    profileNames.Add(directory.Split('\\','/').Last());
            }
            return profileNames;
        }
        public static void CreateProfile(string _ProfileName)
        {

        }
        public static void SaveConfigWTF(ConfigWTF _ConfigWTF, string _ProfileName)
        {
            Utility.AssertDirectory("VF_WowLauncherSettings/Profiles/" + _ProfileName);
            _ConfigWTF.SaveConfigFile("VF_WowLauncherSettings/Profiles/" + _ProfileName + "/Config.wtf");
        }
        public static ConfigWTF GetProfileConfigFile(string _ProfileName)
        {
            if (_ProfileName == "Active Wow Config")
                return ConfigWTF.LoadWTFConfigFile(WowVersion.Vanilla);

            string configWTFFile = "VF_WowLauncherSettings/Profiles/" + _ProfileName + "/Config.wtf";
            if (System.IO.File.Exists(configWTFFile) == true)
                return ConfigWTF.LoadConfigFile(configWTFFile);

            return null;
        }
    }
}
