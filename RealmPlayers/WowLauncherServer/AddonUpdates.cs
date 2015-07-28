using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using ICSharpCode.SharpZipLib.Zip;

using WowLauncherNetwork;
using VF_WoWLauncher;

namespace VF_WoWLauncherServer
{
    class AddonUpdates
    {
        public static string AddonUpdateFolder = "D:\\FTP\\WowLauncher\\AddonUpdates\\";
        public static string AddonUpdateDownloadFTPAddress = "ftp://realmplayers.com:5511/AddonUpdates/";
        public static DateTime g_LastAddonUpdateTimeUTC = DateTime.UtcNow;

        private static string GetLatestAddonPackageFilename(string _AddonName)
        {
            if (System.IO.Directory.Exists(AddonUpdateFolder + _AddonName) == false)
                return "";
            string[] addonPackageFiles = System.IO.Directory.GetFiles(AddonUpdateFolder + _AddonName + "\\");

            int highestVersioniteratedNumber = -1;
            string highestVersioniteratedNumber_File = "";
            foreach (var addonPackageFile in addonPackageFiles)
            {
                string versionIteratedNumberStr = addonPackageFile.Split('\\', '/').Last().Split('_').First();
                int versionIteratedNumber = int.Parse(versionIteratedNumberStr);
                if (versionIteratedNumber > highestVersioniteratedNumber)
                {
                    highestVersioniteratedNumber = versionIteratedNumber;
                    highestVersioniteratedNumber_File = addonPackageFile;
                }
            }
            return highestVersioniteratedNumber_File;
        }

        static Dictionary<string, List<string>> DefaultBetaAddonInfo()
        {
            return new Dictionary<string, List<string>>
            {
                {"VF_RaidStatsTBC", new List<string>{}},
                {"VF_BGStats", new List<string>{}},
                {"VF_BGStatsTBC", new List<string>{}}
            };
        }
        static Dictionary<string, List<string>> m_BetaAddonInfo = DefaultBetaAddonInfo();
        public static Dictionary<string, List<string>> GetBetaAddonInfo()
        {
            return m_BetaAddonInfo;
        }
        public static void LoadAddonBetaInfo(string _BetaInfoFilename = "BetaAddonsInfo.dat")
        {
            if (VF.Utility.LoadSerialize(_BetaInfoFilename, out m_BetaAddonInfo) == false)
            {
                Logger.ConsoleWriteLine("FAILED TO LOAD AddonBetaData...", ConsoleColor.Red);
                m_BetaAddonInfo = DefaultBetaAddonInfo();
            }
        }
        public static void SaveAddonBetaInfo(string _BetaInfoFilename = "BetaAddonsInfo.dat")
        {
            VF.Utility.SaveSerialize(_BetaInfoFilename, m_BetaAddonInfo, true);
        }
        public static bool AddBetaParticipant(string _AddonName, string _UserID)
        {
            if(IsBetaParticipant(_AddonName, _UserID) == true)
            {
                Logger.ConsoleWriteLine("Ensured beta UserID(" + _UserID + ") for Addon(" + _AddonName + ")", ConsoleColor.Green);
                return false;
            }
            m_BetaAddonInfo.AddToList(_AddonName, _UserID);
            Logger.ConsoleWriteLine("Added beta UserID(" + _UserID + ") for Addon(" + _AddonName + ")", ConsoleColor.Green);
            SaveAddonBetaInfo();
            return true;
        }
        public static bool RemoveBetaParticipant(string _AddonName, string _UserID)
        {
            List<string> participants;
            if(m_BetaAddonInfo.TryGetValue(_AddonName, out participants) == true)
            {
                if(participants.Remove(_UserID) == true)
                {
                    Logger.ConsoleWriteLine("Removed beta UserID(" + _UserID + ") for Addon(" + _AddonName + ")", ConsoleColor.Green);
                    SaveAddonBetaInfo();
                    return true;
                }
            }
            return false;
        }
        public static bool IsAddonBeta(string _AddonName)
        {
            if (m_BetaAddonInfo.ContainsKey(_AddonName) == true)
            {
                if (m_BetaAddonInfo[_AddonName].Contains("Everyone"))
                    return false;

                return true;
            }
            return false;
        }
        public static bool IsBetaParticipant(string _AddonName, string _UserID)
        {
            if (IsAddonBeta(_AddonName) == false)
                return false;
            return m_BetaAddonInfo[_AddonName].Contains(_UserID);
        }

        public static WLN_ResponsePacket_AddonUpdateInfo GetAddonUpdate(string _UserID, WLN_RequestPacket_AddonUpdateInfo _Request)
        {
            string addonName = _Request.AddonName;
            string latestAddonPackageFilename = GetLatestAddonPackageFilename(addonName);
            if (latestAddonPackageFilename == "")
                return null;

            if (IsAddonBeta(addonName) && _Request.CurrentVersion == "0.0")
            {
                if (IsBetaParticipant(addonName, _UserID) == false)
                    return null;
            }
            try
            {
                DescriptionFileData addonDescription = GetDescriptionFileData(addonName, latestAddonPackageFilename);
                if (addonDescription == null)
                    return null;

                WLN_ResponsePacket_AddonUpdateInfo addonUpdateInfo = new WLN_ResponsePacket_AddonUpdateInfo();
                addonUpdateInfo.AddonName = addonName;
                addonUpdateInfo.CurrentVersion = _Request.CurrentVersion;

                addonUpdateInfo.UpdateVersion = "";
                addonUpdateInfo.UpdateDescription = "";
                addonUpdateInfo.ClearAccountSavedVariablesRequired = false;
                addonUpdateInfo.ClearCharacterSavedVariablesRequired = false;
                addonUpdateInfo.UpdateSubmitter = "Unknown";
                addonUpdateInfo.UpdateImportance = ServerComm.UpdateImportance.Minor;
 

                addonUpdateInfo.UpdateVersion = addonDescription.UpdateVersion;
                addonUpdateInfo.UpdateDescription = addonDescription.UpdateDescription;
                addonUpdateInfo.UpdateSubmitter = addonDescription.UpdateSubmitter;
                if (addonUpdateInfo.UpdateSubmitter == "")
                    addonUpdateInfo.UpdateSubmitter = "Unknown";
                addonUpdateInfo.ClearAccountSavedVariablesRequired = addonDescription.GetClearAccountSavedVariablesRequired(addonUpdateInfo.CurrentVersion);
                addonUpdateInfo.ClearCharacterSavedVariablesRequired = addonDescription.GetClearCharacterSavedVariablesRequired(addonUpdateInfo.CurrentVersion);
                addonUpdateInfo.UpdateImportance = addonDescription.GetUpdateImportance(addonUpdateInfo.CurrentVersion);
                if(_Request.CurrentVersion == "0.0")
                {
                    if(addonName == "VF_RealmPlayers")
                    {
                        addonUpdateInfo.UpdateDescription = "Latest addon version for gathering data and contribute to the armory at realmplayers.com";
                        addonUpdateInfo.UpdateSubmitter = "Dilatazu";
                    }
                    else if(addonName == "VF_RaidDamage" || addonName == "VF_RaidStats")
                    {
                        addonUpdateInfo.UpdateDescription = "Latest addon version for automatically logging data in raids. Logged raids will automatically be uploaded to RaidStats";
                        addonUpdateInfo.UpdateSubmitter = "Dilatazu";
                    }
                    else if(addonName == "VF_BGStats")
                    {
                        addonUpdateInfo.UpdateDescription = "Latest addon version for automatically logging data in battlegrounds. Logged bgs will automatically be uploaded to BGStats";
                        addonUpdateInfo.UpdateSubmitter = "Dilatazu";
                    }
                    else if(addonName == "VF_RealmPlayersTBC")
                    {
                        addonUpdateInfo.UpdateDescription = "Latest addon version for gathering data and contribute to the armory at realmplayers.com";
                        addonUpdateInfo.UpdateSubmitter = "Dilatazu";
                    }
                    else if (addonName == "VF_RaidStatsTBC")
                    {
                        addonUpdateInfo.UpdateDescription = "Latest addon version for automatically logging data in raids. Logged raids will automatically be uploaded to RaidStats";
                        addonUpdateInfo.UpdateSubmitter = "Dilatazu";
                    }
                    else if (addonName == "VF_BGStatsTBC")
                    {
                        addonUpdateInfo.UpdateDescription = "Latest addon version for automatically logging data in battlegrounds. Logged bgs will automatically be uploaded to BGStats";
                        addonUpdateInfo.UpdateSubmitter = "Dilatazu";
                    }
                    addonUpdateInfo.UpdateImportance = ServerComm.UpdateImportance.Good;
                }
                if (addonUpdateInfo.UpdateVersion == "")
                {
                    Logger.ConsoleWriteLine("AddonPackage \"" + latestAddonPackageFilename + "\" has invalid UpdateVersion, AddonPackage is not valid!");
                    return null; //Invalid UpdateVersion, AddonPackage is not valid!
                }
                if (wyVersionChecker.Compare(addonDescription.UpdateVersion, addonUpdateInfo.CurrentVersion) > 0)
                {
                    addonUpdateInfo.AddonPackageDownloadFTP = AddonUpdateDownloadFTPAddress + latestAddonPackageFilename.Substring(AddonUpdateFolder.Length);
                    return addonUpdateInfo;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return null;
        }

        public static void AddAddonUpdate(string _AddonPackageFile)
        {
            List<string> addonsInPackage = InstalledAddons.GetAddonsInAddonPackage(_AddonPackageFile);
            if (addonsInPackage.Count != 1)
            {
                System.Windows.Forms.MessageBox.Show("No support for multiple addons in AddonPackage yet!");
                return;
            }

            string addonName = addonsInPackage[0];
            var addonDescFile = GetDescriptionFileData(addonName, _AddonPackageFile);

            if (addonDescFile == null)
            {
                System.Windows.Forms.MessageBox.Show("AddonPackage did not have the necessary description file \"VF_WowLauncher_AddonDescription.txt\" or it was invalid format!");
                return;
            }
            string newAddonPackageFile = "";
            {
                string latestAddonPackageFilename = GetLatestAddonPackageFilename(addonName);
                if (latestAddonPackageFilename != "")
                {
                    var prevVersionAddonDescFile = GetDescriptionFileData(addonName, latestAddonPackageFilename);

                    if (wyVersionChecker.Compare(addonDescFile.UpdateVersion, prevVersionAddonDescFile.UpdateVersion) > 0)
                    {
                        //if new version is higher
                        string versionIteratedNumberStr = latestAddonPackageFilename.Split('\\', '/').Last().Split('_').First();
                        int versionIteratedNumber = int.Parse(versionIteratedNumberStr);

                        string newFileName = "" + (versionIteratedNumber + 1) + "_" + addonName + "_" + addonDescFile.UpdateVersion + ".zip";
                        newAddonPackageFile = AddonUpdateFolder + addonName + "\\" + newFileName;
                        if (System.Windows.Forms.MessageBox.Show("A new AddonPackage file will be created: \"" + newAddonPackageFile
                            + "\". \r\n#Addon: " + addonDescFile.AddonName
                            + "\r\n#Version: " + addonDescFile.UpdateVersion
                            + "\r\n#Description: \r\n" + addonDescFile.UpdateDescription
                            + "\r\n#UpdateImportance: " + addonDescFile.UpdateImportanceStr
                            + "\r\n#UpdateSubmitter: " + addonDescFile.UpdateSubmitter
                            + "\r\n#ClearWTFRequired: " + addonDescFile.ClearWTFRequiredStr
                            + "\r\n------------------------------------\r\nPrevious: \"" + latestAddonPackageFilename
                            + "\". \r\n#OldVersion: " + prevVersionAddonDescFile.UpdateVersion
                            + "\". \r\n#OldDescription: \r\n" + prevVersionAddonDescFile.UpdateDescription
                            + "\r\n#OldUpdateImportance: " + addonDescFile.UpdateImportanceStr
                            + "\r\n#OldUpdateSubmitter: " + addonDescFile.UpdateSubmitter
                            + "\r\n#OldClearWTFRequired: " + addonDescFile.ClearWTFRequiredStr
                            , "Are you sure?", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        {
                            return;
                        }
                    }
                    else
                    {
                        //Version was not higher than currently exists.
                        System.Windows.Forms.MessageBox.Show("AddonPackage with higher/same version allready exists! new version=\"" + addonDescFile.UpdateVersion + "\", old version=\"" + prevVersionAddonDescFile.UpdateVersion + "\"");
                        return;
                    }

                }
                else
                {
                    //Done add AddonUpdate
                    string newFileName = "1_" + addonName + "_" + addonDescFile.UpdateVersion + ".zip";
                    newAddonPackageFile = AddonUpdateFolder + addonName + "\\" + newFileName;
                    if (System.Windows.Forms.MessageBox.Show("A new AddonPackage file will be created: \"" + newAddonPackageFile
                        + "\". \r\n#Addon: " + addonDescFile.AddonName
                        + "\r\n#Version: " + addonDescFile.UpdateVersion 
                        + "\r\n#Description: \r\n" + addonDescFile.UpdateDescription 
                        + "\r\n#UpdateImportance: " + addonDescFile.UpdateImportanceStr
                        + "\r\n#UpdateSubmitter: " + addonDescFile.UpdateSubmitter
                        + "\r\n#ClearWTFRequired: " + addonDescFile.ClearWTFRequiredStr
                        , "Are you sure?", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }
            }
            if (System.IO.File.Exists(newAddonPackageFile) == false)
            {
                g_LastAddonUpdateTimeUTC = DateTime.UtcNow;
                VF_WoWLauncher.Utility.AssertFilePath(newAddonPackageFile);
                System.IO.File.Copy(_AddonPackageFile, newAddonPackageFile);
            }
            else
            {
                throw new Exception("This is not supposed to happen!");
            }
        }

        class DescriptionFileData
        {
            public string AddonName = "";
            public string UpdateVersion = "";
            public string UpdateDescription = "";
            public string ClearWTFRequiredStr = "";
            public string UpdateSubmitter = "";
            public string UpdateImportanceStr = "";

            public bool GetClearAccountSavedVariablesRequired(string _CurrentVersion)
            {
                //Example1: "clearwtfrequired=|1.8=false|1.7=true|1.6=true|1.5=false|" --- Assume true for 1.4, 1.3, 1.2, 1.1, 1.0 etc etc etc
                //Example2: "" --- Assume false for 1.8, 1.7, 1.6, 1.5, 1.4, 1.3, 1.2, 1.1, 1.0 etc etc etc
                //Example3: "clearwtfrequired=|1.8=true|" --- Assume true for 1.7, 1.6, 1.5, 1.4, 1.3, 1.2, 1.1, 1.0 etc etc etc
                //Example4: "clearwtfrequired=|1.8=false|" --- Assume true for 1.7, 1.6, 1.5, 1.4, 1.3, 1.2, 1.1, 1.0 etc etc etc

                if (ClearWTFRequiredStr == "")
                    return false;

                if (ClearWTFRequiredStr.Contains("|" + _CurrentVersion + "=false|"))
                    return false;
                else if (ClearWTFRequiredStr.Contains("|" + _CurrentVersion + "=asv|"))
                    return true;
                else if (ClearWTFRequiredStr.Contains("|" + _CurrentVersion + "=csv|"))
                    return false;
                else
                    return true;
            }
            public bool GetClearCharacterSavedVariablesRequired(string _CurrentVersion)
            {
                //Example1: "clearwtfrequired=|1.8=false|1.7=true|1.6=true|1.5=false|" --- Assume true for 1.4, 1.3, 1.2, 1.1, 1.0 etc etc etc
                //Example2: "" --- Assume false for 1.8, 1.7, 1.6, 1.5, 1.4, 1.3, 1.2, 1.1, 1.0 etc etc etc
                //Example3: "clearwtfrequired=|1.8=true|" --- Assume true for 1.7, 1.6, 1.5, 1.4, 1.3, 1.2, 1.1, 1.0 etc etc etc
                //Example4: "clearwtfrequired=|1.8=false|" --- Assume true for 1.7, 1.6, 1.5, 1.4, 1.3, 1.2, 1.1, 1.0 etc etc etc

                if (ClearWTFRequiredStr == "")
                    return false;

                if (ClearWTFRequiredStr.Contains("|" + _CurrentVersion + "=false|"))
                    return false;
                else if (ClearWTFRequiredStr.Contains("|" + _CurrentVersion + "=asv|"))
                    return false;
                else if (ClearWTFRequiredStr.Contains("|" + _CurrentVersion + "=csv|"))
                    return true;
                else
                    return true;
            }

            public ServerComm.UpdateImportance GetUpdateImportance(string _CurrentVersion)
            {
                if (UpdateImportanceStr == "")
                    return ServerComm.UpdateImportance.Minor;

                ServerComm.UpdateImportance updateImportance = ServerComm.UpdateImportance.Minor;
                string findStr = "|" + _CurrentVersion + "=";
                int findStrIndex = UpdateImportanceStr.IndexOf(findStr);
                if (findStrIndex != -1)
                {
                    string enumStr = UpdateImportanceStr.Substring(findStrIndex + findStr.Length).Split('|').First();
                    if (Enum.TryParse(enumStr, true, out updateImportance) == false)
                    {
                        updateImportance = ServerComm.UpdateImportance.Minor;
                        Logger.ConsoleWriteLine("UpdateImportance(" + enumStr + ") for addon \"" + AddonName + "\"(" + _CurrentVersion + ") was not valid Enum", ConsoleColor.Red);
                    }
                }
                else
                {
                    findStr = "|DefaultImportance=";
                    findStrIndex = UpdateImportanceStr.IndexOf(findStr);
                    if (findStrIndex != -1)
                    {
                        string enumStr = UpdateImportanceStr.Substring(findStrIndex + findStr.Length).Split('|').First();
                        if (Enum.TryParse(enumStr, true, out updateImportance) == false)
                        {
                            updateImportance = ServerComm.UpdateImportance.Minor;
                            Logger.ConsoleWriteLine("UpdateImportance(" + enumStr + ") for addon \"" + AddonName + "\"(DefaultImportance) was not valid Enum", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        updateImportance = ServerComm.UpdateImportance.Minor;
                    }
                }
                return updateImportance;
            }
        }
        private static DescriptionFileData GetDescriptionFileData(string _AddonName, string _AddonPackageFile)
        {
            string addonDescription = "";
            using (var fileStream = new System.IO.FileStream(_AddonPackageFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (var zipFile = new ZipFile(fileStream))
                {
                    var zipEntry = zipFile.GetEntry(_AddonName + "/VF_WowLauncher_AddonDescription.txt");
                    if (zipEntry == null)
                    {
                        zipEntry = zipFile.GetEntry(_AddonName + "\\VF_WowLauncher_AddonDescription.txt");
                        if(zipEntry != null)
                            Logger.ConsoleWriteLine("This actually does happen!");
                    }
                    if (zipEntry != null)
                    {
                        using (var inputStream = zipFile.GetInputStream(zipEntry))
                        {
                            using (System.IO.TextReader textReader = new System.IO.StreamReader(inputStream, Encoding.UTF8))
                            {
                                addonDescription = textReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            if (addonDescription == "")
                return null;

            DescriptionFileData descFileData = new DescriptionFileData();
            string[] DescEntries = addonDescription.SplitVF("\r\n#");
            foreach (var descEntry in DescEntries)
            {
                var descEntryType = descEntry.Substring(0, descEntry.IndexOf('=') + 1).ToLower();
                if (descEntryType.StartsWith("description=")) //Description=
                {
                    descFileData.UpdateDescription = descEntry.Substring("Description=".Length);
                }
                else if (descEntryType.StartsWith("version=")) //Version=
                {
                    descFileData.UpdateVersion = descEntry.Substring("Version=".Length);
                }
                else if (descEntryType.StartsWith("clearwtfrequired=")) //ClearWTFRequired=
                {
                    descFileData.ClearWTFRequiredStr = descEntry.Substring("ClearWTFRequired=".Length);
                }
                else if (descEntryType.ToLower().StartsWith("updatesubmitter=")) //UpdateSubmitter=
                {
                    descFileData.UpdateSubmitter = descEntry.Substring("UpdateSubmitter=".Length);
                }
                else if (descEntryType.ToLower().StartsWith("updateimportance=")) //UpdateImportance=
                {
                    descFileData.UpdateImportanceStr = descEntry.Substring("UpdateImportance=".Length);
                }
            }
            if (descFileData.UpdateVersion == "" || descFileData.UpdateDescription == "")
                return null;
            descFileData.AddonName = _AddonName;
            return descFileData;
        }
    }
}
