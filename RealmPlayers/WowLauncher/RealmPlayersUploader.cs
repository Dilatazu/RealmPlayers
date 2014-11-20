//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Net.Sockets;

//namespace VF_WoWLauncher
//{
//    partial class RealmPlayersUploader
//    {
//        static string PROGRAM_VERSION = "1.9";

//        enum FileUploadType
//        {
//            RealmPlayers, //Must stay as this exact name
//            RaidDamage, //Must stay as this exact name
//        }

//        public static string ServerIP = "qinarw.dyndns.org";
//        public static int ServerPort = 18374;
//        static bool ClearWTFfile = true;
//        static bool AutoUpdateAddon = false;
//        //static bool UploadRaidDamage = false;
//        public static bool IsDatabasesNotUploaded(WowVersion _WowVersion)
//        {
//            var savedVariableFiles = WowUtility.GetSavedVariableFilePaths("VF_RealmPlayers", _WowVersion);
//            foreach (var luaFilePath in savedVariableFiles)
//            {
//                var fileInfo = new System.IO.FileInfo(luaFilePath);
//                if (fileInfo.Length > 500)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//        public static void ClearAllSavedDatabases(WowVersion _WowVersion)
//        {
//            var savedVariableFiles = WowUtility.GetSavedVariableFilePaths("VF_RealmPlayers", _WowVersion);
//            foreach (var luaFilePath in savedVariableFiles)
//            {
//                string settingsValues = "\r\nVF_RealmPlayers_Settings = {\r\n[\"DebugMode\"] = false,[\"MaxDatabase\"] = 1000\r\n}\r\n";
//                var entireFile = System.IO.File.ReadAllText(luaFilePath);
//                if (entireFile.Length > 500)
//                {
//                    try
//                    {
//                        if (entireFile.Contains("VF_RealmPlayers_Settings = ") == true)
//                        {
//                            string resultFile = "";
//                            string resultSettingsVariable = "";
//                            WowUtility.ExtractLuaVariableFromFile(entireFile, "VF_RealmPlayers_Settings", out resultFile, out resultSettingsVariable);
//                            if (resultSettingsVariable.Length > 5 && resultSettingsVariable.Contains('}'))
//                                settingsValues = resultSettingsVariable;
//                        }

//                        System.IO.File.WriteAllText(luaFilePath, settingsValues);
//                    }
//                    catch (Exception)
//                    { }
//                }
//            }
//        }
//        public static void Upload(WowVersion _WowVersion)
//        {
//            if (IsValidUserID(Settings.UserID) == false)
//                return;
//            ConsoleWriteLine("Started VF_RealmPlayersUploader " + PROGRAM_VERSION);

//            var savedVariableFiles = WowUtility.GetSavedVariableFilePaths("VF_RealmPlayers", _WowVersion);

//            bool noFile = true;
//            Dictionary<string, string> fileUpdates = new Dictionary<string, string>();
//            int successfullUploads = 0;
//            int totalUploadTries = 0;
//            foreach (var luaFilePath in savedVariableFiles)
//            {
//                if (totalUploadTries >= 4) //2 completely failed upload attempts
//                {
//                    ConsoleWriteLine("There was too many failed upload attempts, shutting down, run the uploader at another time again!");
//                    break;
//                }
//                ConsoleWriteLine("Processing file \"" + luaFilePath + "\"");
//                if (System.IO.File.Exists(luaFilePath) == true)
//                {
//                    FileUploadType fileType = FileUploadType.RealmPlayers;
//                    if (luaFilePath.EndsWith("VF_RaidDamage.lua") == true)
//                        fileType = FileUploadType.RaidDamage;

//                    noFile = false;
//                    string entireFile = System.IO.File.ReadAllText(luaFilePath);
//                    if (entireFile.Length > 500)
//                    {
//                        if (entireFile.Contains("this:RegisterEvent") && entireFile.Contains("DEFAULT_CHAT_FRAME:AddMessage"))
//                        {
//                            Utility.MessageBoxShow("File looks like VF_RealmPlayers Addon file, are you sure the path is correct? Go read Readme file again");
//                            ConsoleWriteLine("File looks like Addon file, are you sure the path is correct? Moving on to next file path");
//                            continue;
//                        }
//                        double addonVersion = 1.2;//Default(innan uploadern blev uppdaterad)
//                        if (fileType == FileUploadType.RealmPlayers)
//                        {
//                            if (entireFile.Contains("VF_RealmPlayersVersion = ") == true)
//                            {
//                                addonVersion = double.Parse(entireFile.Split(new string[] { "VF_RealmPlayersVersion = " }
//                                    , StringSplitOptions.None)[1].Split('\r')[0].Replace("\"", "").Replace('.', ','));
//                            }
//                            ConsoleWriteLine("Successfully processed the file, VF_RealmPlayers addon version: " + addonVersion);
//                        }
//                        else if (fileType == FileUploadType.RaidDamage)
//                        {
//                            addonVersion = 2.0;
//                            ConsoleWriteLine("Successfully processed the file, VF_RaidDamage addon version: " + addonVersion);
//                        }
//                        bool isTemperedWith = IsTemperedWith(luaFilePath);

//                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(entireFile);
//                        int tries = 0;
//                        while (tries < 3)
//                        {
//                            ++tries;
//                            Socket tcpSocket = null;
//                            try
//                            {
//                                DateTime startConnectionTime = DateTime.Now;
//                                ConsoleWriteLine("Connecting to Server..." + ((tries > 1) ? "(try nr " + tries + "/3)" : ""));
//                                tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                                IAsyncResult ar = null;
//                                var addressList = System.Net.Dns.GetHostEntry(ServerIP).AddressList;
//                                for (int alCounter = 0; alCounter < addressList.Length; ++alCounter)
//                                {
//                                    try
//                                    {
//                                        ar = tcpSocket.BeginConnect(addressList[alCounter], ServerPort, null, null);
//                                        break;
//                                    }
//                                    catch (Exception)
//                                    { }
//                                }
//                                System.Threading.WaitHandle waitHandle = ar.AsyncWaitHandle;
//                                try
//                                {
//                                    for (int t = 0; t < 5; ++t)
//                                    {
//                                        if (waitHandle.WaitOne(TimeSpan.FromSeconds(1), false))
//                                            break;
//                                        Console.Write(".");
//                                    }
//                                    Console.Write("\r\n");
//                                    if (!waitHandle.WaitOne(TimeSpan.FromSeconds(1), false))
//                                    {
//                                        ConsoleWriteLine("Connection did not succeed, gave up after 5 seconds. Retrying...");
//                                        tcpSocket.Close();
//                                        tcpSocket = null;
//                                        System.Threading.Thread.Sleep(1000);
//                                        waitHandle.Close();
//                                        continue;
//                                    }

//                                    tcpSocket.EndConnect(ar);
//                                }
//                                finally
//                                {
//                                    waitHandle.Close();
//                                }
//                                tcpSocket.ReceiveTimeout = 5000;
//                                tcpSocket.SendTimeout = 5000;
//                                byte[] header = System.Text.Encoding.UTF8.GetBytes("VF_RPP_Version=" + PROGRAM_VERSION + ";AddonVersion=" + addonVersion + ";UserID=" + Settings.UserID + ";FileType=" + fileType.ToString() + ";Flags=" + (isTemperedWith ? "TFD" : "NOF") + ";FileSize=" + bytes.Length + "%");
//                                ConsoleWriteLine("Connected to the server, started sending file...");
//                                tcpSocket.Send(header);
//                                tcpSocket.Send(bytes);
//                                tcpSocket.Shutdown(SocketShutdown.Send);
//                                Byte[] readBuffer = new Byte[1024];
//                                List<Byte> totalData = new List<Byte>(10000);
//                                int i = 0;
//                                ConsoleWriteLine("Successfully sent file to the server, now receiving response...");
//                                while ((i = tcpSocket.Receive(readBuffer, readBuffer.Length, SocketFlags.None)) != 0)
//                                {
//                                    for (int bi = 0; bi < i; ++bi)
//                                        totalData.Add(readBuffer[bi]);
//                                    if ((DateTime.Now - startConnectionTime).TotalSeconds > 10)
//                                    {
//                                        throw new Exception("Transfer took longer than 10 seconds, should not be allowed, canceling");
//                                    }
//                                }
//                                byte[] byteArray = totalData.ToArray();
//                                string data = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
//                                ConsoleWriteLine("Received entire response, now processing");
//                                bool wasSuccess = false;
//                                if (data.Contains(";") && data.Contains("="))
//                                {
//                                    string[] dataSplit = data.Split(';');
//                                    foreach (string currDataSplit in dataSplit)
//                                    {
//                                        if (currDataSplit.Contains("="))
//                                        {
//                                            string[] currValue = currDataSplit.Split('=');
//                                            if (currValue[0] == "VF_RPP_Success")
//                                                wasSuccess = ConvertBool(currValue[1]);
//                                            else if (currValue[0] == "Message")
//                                                Utility.MessageBoxShow(currValue[1]);
//                                            else if (currValue[0].StartsWith("AddonUpdate-"))
//                                            {
//                                                string addonFolder = GetRealmPlayerAddonFolderPath(luaFilePath);
//                                                string fileName = currValue[0].Split('-')[1];
//                                                string addonLuaFile = currValue[1].Replace('å', ';').Replace('ä', '=');
//                                                addonLuaFile = addonLuaFile.Replace("&AOU&", "å").Replace("&AE&", "ä");
//                                                if (fileUpdates.ContainsKey(addonFolder + fileName) == false)
//                                                    fileUpdates.Add(addonFolder + fileName, addonLuaFile);
//                                                else
//                                                    fileUpdates[addonFolder + fileName] = addonLuaFile;
//                                            }
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    if (data.StartsWith("VF_RPP_Success"))
//                                        wasSuccess = true;
//                                }
//                                if (wasSuccess == true)
//                                {
//                                    ++successfullUploads;
//                                    ConsoleWriteLine("Received Information about successful upload from server");
//                                    try
//                                    {
//                                        tcpSocket.Shutdown(SocketShutdown.Both);
//                                        tcpSocket.Close(5);
//                                        tcpSocket = null;
//                                    }
//                                    catch (Exception)
//                                    { tcpSocket = null; }
//                                    //tcpClient.Client.Shutdown(SocketShutdown.Both);
//                                    //tcpStream.Close();
//                                    //tcpClient.Close();
//                                    //tcpStream = null;
//                                    //tcpClient = null;
//                                    if (ClearWTFfile == true)
//                                    {
//                                        if (fileType == FileUploadType.RealmPlayers)
//                                        {
//                                            string settingsValues = "{[\"DebugMode\"] = false,[\"MaxDatabase\"] = 1000};";
//                                            if (entireFile.Contains("VF_RealmPlayers_Settings = ") == true)
//                                            {
//                                                settingsValues = entireFile.Split(new string[] { "VF_RealmPlayers_Settings = " }, StringSplitOptions.None)
//                                                    [1].Split(new string[] { "VF_RealmPlayersData" }, StringSplitOptions.None)[0];
//                                            }

//                                            System.IO.File.WriteAllText(luaFilePath, "\r\nVF_RealmPlayers_Settings = " + settingsValues + "\r\nVF_RealmPlayersData = {}");
//                                        }
//                                        else if (fileType == FileUploadType.RaidDamage)
//                                        {
//                                            System.IO.File.WriteAllText(luaFilePath, "\r\nVF_RaidDamageData = {}");
//                                        }
//                                    }

//                                    if (fileUpdates.Count > 0 && fileType == FileUploadType.RealmPlayers)
//                                    {
//                                        ConsoleWriteLine("Information about available addon update was received from the server");
//                                        var dialogResult = System.Windows.Forms.DialogResult.Yes;
//                                        if (AutoUpdateAddon == false)
//                                            dialogResult = Utility.MessageBoxShow("Do you want to automaticly install the new addon version?", "New addon version!", System.Windows.Forms.MessageBoxButtons.YesNo);
//                                        if (dialogResult == System.Windows.Forms.DialogResult.Yes)
//                                        {
//                                            foreach (var fileUpdate in fileUpdates)
//                                            {
//                                                try
//                                                {
//                                                    if (System.IO.File.Exists(fileUpdate.Key))
//                                                    {
//                                                        System.IO.File.Copy(fileUpdate.Key, fileUpdate.Key + ".LastVersion", true);
//                                                    }
//                                                    System.IO.File.WriteAllText(fileUpdate.Key, fileUpdate.Value);
//                                                }
//                                                catch (Exception)
//                                                {
//                                                    Utility.MessageBoxShow("There was an error updating addon version, may have to manually install the new version");
//                                                    if (System.IO.File.Exists(fileUpdate.Key + ".LastVersion"))
//                                                    {
//                                                        System.IO.File.Copy(fileUpdate.Key + ".LastVersion", fileUpdate.Key, true);
//                                                    }
//                                                }
//                                            }
//                                            ConsoleWriteLine("Addon VF_RealmPlayers was updated!");
//                                        }
//                                        fileUpdates.Clear();
//                                    }
//                                    break;
//                                }
//                                else
//                                    ConsoleWriteLine("Server never replied which means data was not uploaded correctly. Retrying");
//                            }
//                            catch (Exception ex)
//                            {
//                                LogException(ex);
//                                ConsoleWriteLine("Something went wrong, an error was thrown. check ErrorLog.txt for more info. Retrying");
//                            }
//                            finally
//                            {
//                                if (tcpSocket != null)
//                                {
//                                    tcpSocket.Shutdown(SocketShutdown.Both);
//                                    tcpSocket.Close(5);
//                                    tcpSocket = null;
//                                }
//                                //if(tcpStream != null)
//                                //    tcpStream.Close();
//                                //if (tcpClient != null)
//                                //    tcpClient.Close();
//                            }
//                        }
//                        if (tries >= 3)
//                        {
//                            ConsoleWriteLine("Error, could not successfully transfer the file after 3 tries. Server may be down. Try upload it again later");
//                            //Utility.MessageBoxShow("Error, could not transfer file\r\n" + luaFile + "\r\n after 3 tries. Server may be down. Try upload it again later");
//                            break;
//                        }
//                        totalUploadTries += (tries - 1);
//                    }
//                    else
//                        ConsoleWriteLine("File was below 500 bytes, which means not enough data to upload, moving on to next");
//                }
//                else
//                    ConsoleWriteLine("File did not exist, moving on to next");
//            }
//            if (noFile == true)
//            {
//                ConsoleWriteLine("Done with everything. no files were found");
//                //Utility.MessageBoxShow("No VF_RealmPlayers.lua was found in any of the paths given in Settings.cfg, make sure the paths are correct");
//                return;
//            }
//            ConsoleWriteLine("Done with everything. Uploaded " + successfullUploads + " lua database files");
//        }
//    }
//}
