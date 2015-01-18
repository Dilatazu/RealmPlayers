using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace VF_WoWLauncher
{
    public enum AddonBackupMode
    {
        BackupWTF = 1,
        BackupAddonFiles = 2,
        BackupWTF_And_AddonFiles = 3,
    }
    public class InstalledAddons
    {
        internal static List<string> GetInstalledAddons(WowVersionEnum _WowVersion)
        {
            var addons = Utility.GetDirectoriesInDirectory(Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\");
            for (int i = 0; i < addons.Count; ++i)
            {
                string addonDirectory = Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + addons[i] + "\\";
                if (System.IO.File.Exists(addonDirectory + addons[i] + ".toc") == false)
                {
                    addons.RemoveAt(i);
                    --i;
                }
                //if (addons[i].StartsWith("Blizzard_"))
                //{
                //    addons.RemoveAt(i);
                //    --i;
                //}
            }
            return addons;
        }
        internal static string BackupAddon(string _AddonName, WowVersionEnum _WowVersion, AddonBackupMode _BackupMode = AddonBackupMode.BackupWTF_And_AddonFiles)
        {
            int fileID = System.Threading.Interlocked.Increment(ref g_UniqueFileIDCounter);
            string backupFileName = StaticValues.LauncherBackupsDirectory + DateTime.Now.ToString("yyyy_MM_dd") + "/BackupAddon_" + _AddonName + "_" + DateTime.Now.ToString("HH_mm_ss") + "." + fileID + ".zip";
            Utility.AssertFilePath(backupFileName);

            ZipFile zipFile;
            if (System.IO.File.Exists(backupFileName) == true)
            {
                backupFileName = Utility.ConvertToUniqueFilename(backupFileName, '.');
            }
            if (System.IO.File.Exists(backupFileName) == true)
                throw new Exception("Backup file already exists");

            zipFile = ZipFile.Create(backupFileName);

            zipFile.BeginUpdate();
            
            if (_BackupMode.HasFlag(AddonBackupMode.BackupWTF))
            {
                string wtfAccStartPath = Settings.GetWowDirectory(_WowVersion);
                var savedVariableFiles = WowUtility.GetPerCharacterSavedVariableFilePaths(_AddonName, _WowVersion);
                savedVariableFiles.AddRange(WowUtility.GetSavedVariableFilePaths(_AddonName, _WowVersion));

                foreach (string savedVariableFile in savedVariableFiles)
                {
                    zipFile.Add(savedVariableFile, savedVariableFile.Substring(wtfAccStartPath.Length));
                }
            }
            if (_BackupMode.HasFlag(AddonBackupMode.BackupAddonFiles))
            {
                zipFile.AddDirectoryFilesRecursive(Settings.GetWowDirectory(_WowVersion), Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + _AddonName);
            }
            zipFile.CommitUpdate();
            zipFile.Close();
            return backupFileName;
        }
        public static int g_UniqueFileIDCounter = 0;
        internal static string BackupAddons(List<string> _AddonNames, WowVersionEnum _WowVersion, AddonBackupMode _BackupMode = AddonBackupMode.BackupWTF_And_AddonFiles)
        {
            int fileID = System.Threading.Interlocked.Increment(ref g_UniqueFileIDCounter);
            string backupFileName = StaticValues.LauncherBackupsDirectory + DateTime.Now.ToString("yyyy_MM_dd") + "/BackupAddons_" + DateTime.Now.ToString("HH_mm_ss") + "." + fileID + ".zip";
            Utility.AssertFilePath(backupFileName);

            ZipFile zipFile;
            
            if (System.IO.File.Exists(backupFileName) == true)
            {
                backupFileName = Utility.ConvertToUniqueFilename(backupFileName, '.');
            }
            if (System.IO.File.Exists(backupFileName) == true)
                throw new Exception("Backup file already exists");

            zipFile = ZipFile.Create(backupFileName);

            zipFile.BeginUpdate();

            foreach (var addonName in _AddonNames)
            {
                if (_BackupMode.HasFlag(AddonBackupMode.BackupWTF))
                {
                    string wtfAccStartPath = Settings.GetWowDirectory(_WowVersion);
                    var savedVariableFiles = WowUtility.GetPerCharacterSavedVariableFilePaths(addonName, _WowVersion);
                    savedVariableFiles.AddRange(WowUtility.GetSavedVariableFilePaths(addonName, _WowVersion));

                    foreach (string savedVariableFile in savedVariableFiles)
                    {
                        zipFile.Add(savedVariableFile, savedVariableFile.Substring(wtfAccStartPath.Length));
                    }
                }
                if (_BackupMode.HasFlag(AddonBackupMode.BackupAddonFiles))
                {
                    if (System.IO.Directory.Exists(Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + addonName))
                    {
                        zipFile.AddDirectoryFilesRecursive(Settings.GetWowDirectory(_WowVersion), Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + addonName);
                    }
                }
            }
            zipFile.CommitUpdate();
            zipFile.Close();
            return backupFileName;
        }

        public static List<string> GetAddonsInAddonPackage(string _AddonZipFilePackage)
        {
            List<string> addonsInPackage = new List<string>();
            using (var fileStream = new System.IO.FileStream(_AddonZipFilePackage, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (var zipFile = new ZipFile(fileStream))
                {
                    foreach (ZipEntry zipEntry in zipFile)
                    {
                        if (zipEntry.IsDirectory)
                        {
                            if(zipEntry.Name.Split(new char[]{'\\', '/'}, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                            {
                                //Root Entry Directory
                                addonsInPackage.Add(zipEntry.Name.Replace("\\", "").Replace("/",""));
                            }
                        }
                    }
                }
            }
            return addonsInPackage;
        }
        internal static List<string> InstallAddonPackage(string _AddonZipFilePackage, WowVersionEnum _WowVersion, Action<float> _InstallProgress = null, bool _ClearWTFSettings = false)
        {
            List<string> addonsInPackage = GetAddonsInAddonPackage(_AddonZipFilePackage);
            _InstallProgress(0.2f);
            string backupFile = BackupAddons(addonsInPackage, _WowVersion);
            _InstallProgress(0.4f);
            for(int i = 0; i < addonsInPackage.Count; ++i)
            {
                string addon = addonsInPackage[i];
                if (_ClearWTFSettings == true)
                {
                    var savedVariableFiles = WowUtility.GetPerCharacterSavedVariableFilePaths(addon, _WowVersion);
                    savedVariableFiles.AddRange(WowUtility.GetSavedVariableFilePaths(addon, _WowVersion));
                    foreach (string savedVariableFile in savedVariableFiles)
                    {
                        Utility.DeleteFile(savedVariableFile);
                    }
                }
                if (System.IO.Directory.Exists(Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + addon))
                {
                    Utility.DeleteDirectory(Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + addon);
                }
                _InstallProgress(0.4f + ((float)(i+1) / (float)addonsInPackage.Count) * 0.4f);
            }

            _InstallProgress(0.8f);
            try
            {
                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(_AddonZipFilePackage, Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\", null);
                _InstallProgress(1.0f);
                return addonsInPackage;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Utility.MessageBoxShow("There was an error extracting the AddonPackage, Backup of anything deleted/replaced exists in the \"" + backupFile + "\" unfortunately automatic restoration is not implemented yet");
            }
            return null;
        }
        internal static void ReinstallAddon(string _AddonName, WowVersionEnum _WowVersion)
        {
            //Delete WTF files
            var savedVariableFiles = WowUtility.GetPerCharacterSavedVariableFilePaths(_AddonName, _WowVersion);
            savedVariableFiles.AddRange(WowUtility.GetSavedVariableFilePaths(_AddonName, _WowVersion));
            foreach (string savedVariableFile in savedVariableFiles)
            {
                Utility.DeleteFile(savedVariableFile);
            }
            //Delete WTF files
        }
        internal static bool UninstallAddon(string _AddonName, WowVersionEnum _WowVersion)
        {
            if (System.IO.Directory.Exists(Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + _AddonName))
            {
                Utility.DeleteDirectory(Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + _AddonName);

                //Delete WTF files
                var savedVariableFiles = WowUtility.GetPerCharacterSavedVariableFilePaths(_AddonName, _WowVersion);
                savedVariableFiles.AddRange(WowUtility.GetSavedVariableFilePaths(_AddonName, _WowVersion));
                foreach (string savedVariableFile in savedVariableFiles)
                {
                    Utility.DeleteFile(savedVariableFile);
                }
                //Delete WTF files
            }
            return System.IO.Directory.Exists(Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + _AddonName) == false;
        }
        public class AddonInfo
        {
            public string m_AddonName = "Unknown";
            public string m_AddonTitle = "";
            public string m_Notes = "";
            public string m_Author = "";
            public string m_VersionString = "";
            public bool m_TOCVersionString = false;
            public DateTime m_NewestModificationDate = DateTime.MinValue;
            public List<string> m_Dependencies = new List<string>();
            public List<string> m_SavedVariables = new List<string>();
            public List<string> m_SavedVariablesPerCharacter = new List<string>();

            public AddonInfo(string _AddonName)
            {
                m_AddonName = _AddonName;
            }
        }
        public static AddonInfo GetAddonInfo(string _AddonName, string _AddonFolderPath)
        {
            AddonInfo addonInfo = new AddonInfo(_AddonName);
            if (System.IO.Directory.Exists(_AddonFolderPath) == false || System.IO.File.Exists(_AddonFolderPath + "\\" + _AddonName + ".toc") == false)
            {
                return null;
            }
            string[] tocLines = System.IO.File.ReadAllLines(_AddonFolderPath + "\\" + _AddonName + ".toc");
            List<string> startupFiles = new List<string>();
            foreach (var tocLine in tocLines)
            {
                try 
	            {
                    if (tocLine.StartsWith("##"))//SettingsLine
                    {
                        if (tocLine.StartsWith("## Version:"))
                        {
                            addonInfo.m_VersionString = tocLine.SplitVF("## Version:").Last().Trim();
                            addonInfo.m_TOCVersionString = true;
                        }
                        else if (tocLine.StartsWith("## Dependencies:"))
                        {
                            string[] dependencies = tocLine.SplitVF("## Dependencies:").Last().Split(',');
                            foreach (var dependency in dependencies)
                                addonInfo.m_Dependencies.Add(dependency.Trim());
                        }
                        else if (tocLine.StartsWith("## RequiredDeps:"))
                        {
                            string[] dependencies = tocLine.SplitVF("## RequiredDeps:").Last().Split(',');
                            foreach (var dependency in dependencies)
                                addonInfo.m_Dependencies.Add(dependency.Trim());
                        }
                        else if (tocLine.StartsWith("## Dep"))
                        {
                            string[] dependencies = tocLine.Substring(tocLine.IndexOf(':') + 1).Split(',');
                            foreach (var dependency in dependencies)
                                addonInfo.m_Dependencies.Add(dependency.Trim());
                        }
                        else if (tocLine.StartsWith("## SavedVariables:"))
                        {
                            string[] dependencies = tocLine.SplitVF("## SavedVariables:").Last().Split(',');
                            foreach (var dependency in dependencies)
                                addonInfo.m_SavedVariables.Add(dependency.Trim());
                        }
                        else if (tocLine.StartsWith("## SavedVariablesPerCharacter:"))
                        {
                            string[] dependencies = tocLine.SplitVF("## SavedVariablesPerCharacter:").Last().Split(',');
                            foreach (var dependency in dependencies)
                                addonInfo.m_SavedVariablesPerCharacter.Add(dependency.Trim());
                        }
                        else if (tocLine.StartsWith("## Title:"))
                        {
                            addonInfo.m_AddonTitle = tocLine.SplitVF("## Title:").Last().Trim();
                            if (addonInfo.m_AddonTitle.Contains(" v") && addonInfo.m_VersionString == "")
                            {
                                string versionSplit = addonInfo.m_AddonTitle.SplitVF(" v").Last();
                                versionSplit = versionSplit.Split(' ').First();
                                if (versionSplit.Contains("."))
                                {
                                    string[] versionNrs = versionSplit.Split('.');
                                    bool isVersionString = true;
                                    foreach (string versionNr in versionNrs)
                                    {
                                        int versionInt;
                                        if (int.TryParse(versionNr, out versionInt) == false)
                                        {
                                            isVersionString = false;
                                            break;
                                        }
                                    }
                                    if (isVersionString == true)
                                        addonInfo.m_VersionString = "#TocGuess# " + versionSplit;
                                }
                            }
                        }
                        else if (tocLine.StartsWith("## Notes:"))
                        {
                            addonInfo.m_Notes = tocLine.Substring(tocLine.IndexOf(':') + 1);
                        }
                        else if (tocLine.StartsWith("## Notes"))
                        {
                            if (addonInfo.m_Notes == "")
                            {
                                addonInfo.m_Notes = tocLine.Substring(tocLine.IndexOf(':') + 1);
                            }
                        }
                        else if (tocLine.StartsWith("## Author:"))
                        {
                            addonInfo.m_Author = tocLine.Substring(tocLine.IndexOf(':') + 1);
                        }
                    }
                    else
                    {
                        startupFiles.Add(tocLine);
                    }
	            }
	            catch (Exception)
	            {}
            }
            if (addonInfo.m_AddonName == "SW_Stats" && addonInfo.m_VersionString == "2.0 Beta")
            {
                if(System.IO.File.Exists(_AddonFolderPath + "\\neutral.lua") == true)
                {
                    var neutralFile = System.IO.File.ReadAllText(_AddonFolderPath + "\\neutral.lua");
                    if (neutralFile.Contains("SW_VERSION = \"2.0 Beta.7\"") == true)
                        addonInfo.m_VersionString = "2.0 Beta.7";
                }
            }
            if (addonInfo.m_VersionString == "")
            {
                string lowerCaseAddonName = _AddonName.ToLower();
                List<string> addonFiles = Utility.GetFilesInDirectory(_AddonFolderPath, "*.lua");
                var addonFilesOrdered = addonFiles.OrderBy((_Value) =>
                {
                    int findIndex = startupFiles.FindIndex((_File) => _File == _Value);
                    if (findIndex != -1)
                    {
                        return findIndex;
                    }
                    return startupFiles.Count + lowerCaseAddonName.LevenshteinDistance(_Value.ToLower());
                });
                foreach (var addonFile in addonFilesOrdered)
                {
                    DateTime currLastWriteTime = System.IO.File.GetLastWriteTimeUtc(addonFile);
                    if (currLastWriteTime > addonInfo.m_NewestModificationDate)
                        addonInfo.m_NewestModificationDate = currLastWriteTime;
                    if (addonInfo.m_VersionString == "")
                    {
                        string[] fileLines = System.IO.File.ReadAllLines(_AddonFolderPath + "\\" + addonFile);
                        foreach (var fileLine in fileLines)
                        {
                            try
                            {
                                string currLineLowered = fileLine.ToLower();
                                if (currLineLowered.Contains("version") && currLineLowered.Contains(lowerCaseAddonName))
                                {
                                    string variableVersion = fileLine.Split('=').Last().SplitVF("--").First().Trim();
                                    while (variableVersion.EndsWith(";"))
                                        variableVersion = variableVersion.Substring(0, variableVersion.Length - 1).Trim();
                                    addonInfo.m_VersionString = "#LuaGuess# " + variableVersion.Replace("\"", "");
                                    break;
                                }
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
            }
            else
            {
                List<string> addonFiles = Utility.GetFilesInDirectory(_AddonFolderPath, "*.lua");
                foreach (var addonFile in addonFiles)
                {
                    DateTime currLastWriteTime = System.IO.File.GetLastWriteTimeUtc(addonFile);
                    if (currLastWriteTime > addonInfo.m_NewestModificationDate)
                        addonInfo.m_NewestModificationDate = currLastWriteTime;
                }
            }
            return addonInfo;
        }
        internal static AddonInfo GetAddonInfo(string _AddonName, WowVersionEnum _WowVersion)
        {
            string addonDirectory = Settings.GetWowDirectory(_WowVersion) + "Interface\\AddOns\\" + _AddonName + "\\";
            return GetAddonInfo(_AddonName, addonDirectory);
        }
    }
}
