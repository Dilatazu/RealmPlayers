using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using WowLauncherNetwork;
using ProtoBuf;

namespace VF_WoWLauncher
{
    public class ServerComm
    {
        public static string g_Host = "realmplayers.com";
        public static int g_Port = 19374;

        public class AddonInfo
        {
            public string Name;
            public List<string> Authors = new List<string>();
            public string Version;
            public List<string> VersionChangeLog = new List<string>();

            public string AuthorsStr
            {
                get
                {
                    string retValue = "";
                    foreach (var a in Authors)
                    {
                        retValue += a + ", ";
                    }
                    return retValue;
                }
            }
            public string VersionChangeLogStr
            {
                get 
                { 
                    string retValue = "";
                    foreach (var v in VersionChangeLog)
                    {
                        retValue += v + "\r\n";
                    }
                    return retValue;
                }
            }
        }
        public enum UpdateImportance
        {
            Very_Minor = 10,
            Minor = 20,
            Good = 50,
            Important = 80,
            Very_Important = 90,
            Critical = 100,
        }
        public class AddonUpdateInfo
        {
            public string AddonName = "null";
            public string CurrentVersion = "null";
            public string UpdateVersion = "null";
            public string UpdateDescription = "null";
            public string AddonPackageDownloadFTP = "null";
            public int AddonPackageFileSize = 0;
            public bool ClearAccountSavedVariablesRequired = false;
            public bool ClearCharacterSavedVariablesRequired = false;
            public string UpdateSubmitter = "null";
            public UpdateImportance UpdateImportance = UpdateImportance.Minor;
            public InstalledAddons.AddonInfo InstalledAddonInfo;
            public string MoreInfoSite = "";
        }
        internal static bool PeekAddonUpdates(int _MinutesSinceLastPeek)
        {
            VF.NetworkClient netClient = new VF.NetworkClient(g_Host, g_Port);
            netClient.WaitForConnect(TimeSpan.FromSeconds(60));
            try
            {
                {
                    VF.NetworkOutgoingMessage newMessage = netClient.CreateMessage();
                    WLN_RequestPacket_AddonUpdateInfoPeek request = new WLN_RequestPacket_AddonUpdateInfoPeek();
                    request.UserID = Settings.UserID;
                    request.LauncherVersion = StaticValues.LauncherVersion;
                    request.MinutesSinceLastPeek = _MinutesSinceLastPeek;

                    newMessage.WriteByte((byte)WLN_PacketType.Request_AddonUpdateInfoPeek);
                    newMessage.WriteClass(request);
                    netClient.SendMessage(newMessage);
                }
                WLN_ResponsePacket_AddonUpdateInfoPeek response = new WLN_ResponsePacket_AddonUpdateInfoPeek();
                if (netClient.RecvPacket_VF(WLN_PacketType.Response_AddonUpdateInfoPeek, out response) == true)
                {
                    if (response.AddonUpdatesAvailable != null)
                    {
                        return response.AddonUpdatesAvailable.Count > 0;
                    }
                }
            }
            finally
            {
                netClient.Disconnect();
                netClient = null;
            }
            return false;
        }
        internal static List<AddonUpdateInfo> GetAddonUpdateInfos(List<string> _AddonNames, WowVersionEnum _WowVersion)
        {
            VF.NetworkClient netClient = new VF.NetworkClient(g_Host, g_Port);
            List<AddonUpdateInfo> retList = new List<AddonUpdateInfo>();
            try
            {
                //VF.NetworkIncommingMessage msg;
                {
                    VF.NetworkOutgoingMessage newMessage = netClient.CreateMessage();
                    WLN_RequestPacket_AddonUpdateInfoNew request = new WLN_RequestPacket_AddonUpdateInfoNew();
                    request.UserID = Settings.UserID;
                    request.LauncherVersion = StaticValues.LauncherVersion;
                    request.WowVersion = _WowVersion;
                    for (int i = 0; i < _AddonNames.Count; ++i)
                    {
                        var addonUpdateInfo = new WLN_RequestPacket_AddonUpdateInfo();
                        addonUpdateInfo.AddonName = _AddonNames[i];
                        var addonInfo = InstalledAddons.GetAddonInfo(_AddonNames[i], _WowVersion);
                        if (addonInfo != null)
                            addonUpdateInfo.CurrentVersion = addonInfo.m_VersionString;
                        else
                            addonUpdateInfo.CurrentVersion = "0.0";
                        request.Addons.Add(addonUpdateInfo);
                    }
                    newMessage.WriteByte((byte)WLN_PacketType.Request_AddonUpdateInfoNew);
                    newMessage.WriteClass(request);
                    netClient.SendMessage(newMessage);
                }
                List<WLN_ResponsePacket_AddonUpdateInfo> recvAddonUpdateInfos = null;

                if (netClient.RecvPacket_VF(WLN_PacketType.Response_AddonUpdateInfo, out recvAddonUpdateInfos) == true)
                {
                    foreach (var recvAddonUpdateInfo in recvAddonUpdateInfos)
                    {
                        if (recvAddonUpdateInfo.AddonName == "null")
                            continue;
                        AddonUpdateInfo addonUpdateInfo = new AddonUpdateInfo();
                        addonUpdateInfo.AddonName = recvAddonUpdateInfo.AddonName;
                        addonUpdateInfo.InstalledAddonInfo = InstalledAddons.GetAddonInfo(recvAddonUpdateInfo.AddonName, _WowVersion);
                        addonUpdateInfo.CurrentVersion = recvAddonUpdateInfo.CurrentVersion;
                        addonUpdateInfo.UpdateVersion = recvAddonUpdateInfo.UpdateVersion;
                        addonUpdateInfo.UpdateDescription = recvAddonUpdateInfo.UpdateDescription;
                        addonUpdateInfo.AddonPackageDownloadFTP = recvAddonUpdateInfo.AddonPackageDownloadFTP;
                        addonUpdateInfo.ClearAccountSavedVariablesRequired = recvAddonUpdateInfo.ClearAccountSavedVariablesRequired;
                        addonUpdateInfo.UpdateSubmitter = recvAddonUpdateInfo.UpdateSubmitter;
                        addonUpdateInfo.UpdateImportance = recvAddonUpdateInfo.UpdateImportance;
                        addonUpdateInfo.MoreInfoSite = recvAddonUpdateInfo.MoreInfoSite;
                        retList.Add(addonUpdateInfo);
                    }
                }
            }
            finally
            {
                netClient.Disconnect();
            }
            return retList;
        }
        public static string DownloadAddonPackage(string _FTPDownloadAddress, int _AddonPackageFileSize, Action<float> _DownloadProgress = null)
        {
            try
            {
                int fileNameStartIndex = _FTPDownloadAddress.LastIndexOf('\\');
                if(fileNameStartIndex == -1 || fileNameStartIndex < _FTPDownloadAddress.LastIndexOf('/'))
                    fileNameStartIndex = _FTPDownloadAddress.LastIndexOf('/');
                string fileName = StaticValues.LauncherDownloadsDirectory + _FTPDownloadAddress.Substring(fileNameStartIndex + 1);

                FtpWebRequest ftpRequest = null;
                FtpWebResponse ftpResponse = null;
                System.IO.Stream ftpStream = null;
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(_FTPDownloadAddress);
                _DownloadProgress(0.1f);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential("WowLauncherUpdater", "");
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                _DownloadProgress(0.2f);
                /* Get the FTP Server's Response Stream */
                ftpStream = ftpResponse.GetResponseStream();
                /* Open a File Stream to Write the Downloaded File */
                Utility.AssertFilePath(fileName);
                System.IO.FileStream localFileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
                _DownloadProgress(0.3f);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[2048];
                int bytesRead = ftpStream.Read(byteBuffer, 0, 2048);
                /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                int totalDownloaded = 0;
                try
                {
                    while (bytesRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        totalDownloaded += bytesRead;
                        if (_AddonPackageFileSize != 0)
                        {
                            float progressValue = 0.3f + ((float)totalDownloaded / (float)_AddonPackageFileSize) * 0.65f;
                            if (progressValue <= 0.95f)
                            {
                                _DownloadProgress(progressValue);
                            }
                        }
                        bytesRead = ftpStream.Read(byteBuffer, 0, 2048);
                    }
                }
                catch (Exception ex) { Logger.LogException(ex); }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                _DownloadProgress(1.0f);
                return fileName;
            }
            catch (Exception ex) { Logger.LogException(ex); }
            return "";
        }
        //public static List<string> GetAddonsChangedSince(DateTime _LastCheckTime)
        //{
        //    List<string> retValue = new List<string>();

        //    retValue.Add("VF_RealmPlayers");
        //    retValue.Add("VF_RaidDamage");
        //    retValue.Add("VF_HealingInformation");
        //    return null;
        //}
        internal static VF.NetworkClient sm_Client = null;
        internal static List<string> SendAddonData(string _UserID, string _AddonName, WowVersionEnum _WowVersion, string _ClearLuaVariableName, int _LuaVariableDataLengthThreshold, out bool _SentAll)
        {
            var savedVariableFiles = WowUtility.GetSavedVariableFilePaths(_AddonName, _WowVersion);//For Accounts only

            _LuaVariableDataLengthThreshold = _LuaVariableDataLengthThreshold + 12/*newlines = {} osv osv*/ + _ClearLuaVariableName.Length;
            VF.NetworkClient netClient = sm_Client;
            if (netClient != null && netClient.IsConnected() == false)
            {
                netClient.Disconnect();
                netClient = null;
            }
            List<string> sentAddonFiles = new List<string>();
            _SentAll = true;//Default läge
            try
            {
                foreach (var luaFilePath in savedVariableFiles)
                {
                    string luaFileData = System.IO.File.ReadAllText(luaFilePath);
                    string resultFileData = "";
                    string variableData = "";
                    if (WowUtility.ExtractLuaVariableFromFile(luaFileData, _ClearLuaVariableName, out resultFileData, out variableData) == true)
                    {
                        if (variableData.Length < _LuaVariableDataLengthThreshold)
                            continue;
                        if (netClient == null)
                        {
                            Logger.ConsoleWriteLine("Trying to connect to server...", ConsoleColor.Gray);
                            netClient = new VF.NetworkClient(g_Host, g_Port); //realmplayers.com
                            Logger.ConsoleWriteLine("Connected to server!", ConsoleColor.Green);
                        }

                        WLN_UploadPacket_AddonData addonData = new WLN_UploadPacket_AddonData();
                        addonData.AddonName = _AddonName;
                        addonData.UserID = _UserID;
                        addonData.FileStatus = WowUtility.GetFileStatus(luaFilePath);
                        addonData.Data = luaFileData;
                        var newMessage = netClient.CreateMessage();
                        newMessage.WriteByte((byte)WLN_PacketType.Upload_AddonData);
                        newMessage.WriteClass(addonData);
                        netClient.WaitForConnect(new TimeSpan(10000));
                        netClient.SendMessage(newMessage);
                        Logger.ConsoleWriteLine("Sent SavedVariables file \"" + luaFilePath + "\". Waiting for Response...", ConsoleColor.Gray);
                        WLN_UploadPacket_SuccessResponse response;
                        if (netClient.RecvPacket_VF(WLN_PacketType.Upload_SuccessResponse, out response, TimeSpan.FromSeconds(60)) == true)
                        {
                            Logger.ConsoleWriteLine("Received Response. Data was sent successfully!. Deleting old data", ConsoleColor.Gray);
                            sentAddonFiles.Add(luaFilePath);
                            if (response.MessageToUser != "")
                                Utility.MessageBoxShow(response.MessageToUser, "Message from Server");
                            //TODO: Do something with "response.MessageToUser"
                            System.IO.File.WriteAllText(luaFilePath, resultFileData); //Save the lua file with the variable cleared

                            Logger.ConsoleWriteLine("Operation Successfull! Preparing for next file.", ConsoleColor.Green);
                        }
                        else
                        {
                            Logger.ConsoleWriteLine("Operation Failed! Preparing for next file.", ConsoleColor.Red);
                            _SentAll = false;
                        }
                    }
                    else
                    { }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                _SentAll = false;
            }
            finally
            {
                if (_SentAll == false)
                {
                    if (netClient != null)
                    {
                        netClient.Disconnect();
                        netClient = null;
                    }
                }
                sm_Client = netClient;
            }
            return sentAddonFiles;
        }
        internal static void SendAddonData_Dispose()
        {
            if (sm_Client != null)
            {
                sm_Client.Disconnect();
                sm_Client = null;
            }
        }
    }
}
