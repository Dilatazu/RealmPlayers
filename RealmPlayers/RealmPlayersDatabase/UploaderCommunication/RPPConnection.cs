using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace VF_RealmPlayersDatabase.UploaderCommunication
{
    enum FileUploadType
    {
        RealmPlayers, //Must stay as this exact name
        RaidDamage, //Must stay as this exact name
    }
    class RPPConnection
    {
        Socket TCPSocket;
        //TcpClient Client;
        //NetworkStream Stream;
        public class HeaderData
        {
            public string Command = "RPUpload";
            public Single UploaderVersion = 0.0f;
            public Single AddonVersion = 1.2f;
            public Contributor Contributor = null;
            public string UserIDStr = "";
            public FileUploadType FileType = FileUploadType.RealmPlayers;
            public int FileTotalSize = int.MaxValue;
            public string[] Flags = new string[]{"NOF"};
            public bool IsTemperedFlag()
            {
                if (Flags.Contains("TFD"))
                    return true;
                else
                    return false;
            }
        };
        HeaderData Header = null;
        string Data = "";

        public Contributor GetContributor()
        {
            return Header.Contributor;
        }
        public string GetData()
        {
            return Data;
        }

        public RPPConnection(Socket/*TcpClient*/ _Client)
        {
            TCPSocket = _Client;
            TCPSocket.LingerState = new LingerOption(true, 5);
            TCPSocket.SendTimeout = 2000;
            TCPSocket.ReceiveTimeout = 2000;
        }

        public void Run(Action<Contributor, string, HeaderData> _ProcessDataFunction)
        {
            try
            {
                RetrieveAllData();
                ProcessRetrievedData();
                SendResponse();
                CloseConnection();
                if (Header.Command == "RPUpload")
                {
                    Logger.ConsoleWriteLine("Successfully received new data from " + GetContributor().Name, ConsoleColor.Green);
                    _ProcessDataFunction(GetContributor(), GetData(), Header);
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                Logger.LogException(ex);
            }
        }
        private void RetrieveAllData()
        {
            int i = 0;
            Byte[] bytes = new Byte[1024];
            List<Byte> totalData = new List<Byte>(10000);
            DateTime startReceiveTime = DateTime.Now;
            int totalReceivedBytes = 0;
            while ((Header == null || totalReceivedBytes < Header.FileTotalSize) && ((i = TCPSocket.Receive(bytes, bytes.Length, SocketFlags.None)) != 0)/*((i = Stream.Read(bytes, 0, bytes.Length)) != 0)*/)
            {
                totalReceivedBytes += i;
                for (int bi = 0; bi < i; ++bi)
                    totalData.Add(bytes[bi]);
                Data += System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                if (Header == null && Data.IndexOf('%') != -1 && Data.Length > Data.IndexOf('%') + 10)
                {
                    string headerStr = Data.Substring(0, Data.IndexOf('%'));
                    Data = Data.Substring(Data.IndexOf('%') + 1);
                    ProcessHeaderData(headerStr);
                    totalReceivedBytes = System.Text.Encoding.UTF8.GetBytes(Data).Length;
                }
                if ((DateTime.Now - startReceiveTime).TotalSeconds > 10)
                {
                    throw new Exception("Transfer took longer than 10 seconds, should not be allowed, canceling");
                }
            }
            if(totalReceivedBytes != Header.FileTotalSize)
                Logger.ConsoleWriteLine("Packet was not the size it is supposed to be! (" + totalReceivedBytes + "/" + Header.FileTotalSize + ")");
            Data = System.Text.Encoding.UTF8.GetString(totalData.ToArray());
            Data = Data.Substring(Data.IndexOf('%') + 1);
        }
        private void ProcessHeaderData(string _Header)
        {
            string[] headerData = _Header.Split(new char[] { ';' });

            if (headerData[0].Contains('=') == true)
            {
                Header = new HeaderData();

                foreach (string currHeader in headerData)
                {
                    if (currHeader.Contains('='))
                    {
                        string[] currHeaderSplit = currHeader.Split('=');
                        if (currHeaderSplit[0] == "VF_RPP_Version")
                            Header.UploaderVersion = Utility.ParseSingle(currHeaderSplit[1]);
                        else if (currHeaderSplit[0] == "Command")
                            Header.Command = currHeaderSplit[1];
                        else if (currHeaderSplit[0] == "FileSize")
                            Header.FileTotalSize = int.Parse(currHeaderSplit[1]);
                        else if (currHeaderSplit[0] == "FileType")
                            Header.FileType = (FileUploadType)Enum.Parse(typeof(FileUploadType), currHeaderSplit[1], true);
                        else if (currHeaderSplit[0] == "UserID")
                        {
                            Header.UserIDStr = currHeaderSplit[1].ToLower();
                            Header.UserIDStr = Header.UserIDStr[0].ToString().ToUpper() + Header.UserIDStr.Substring(1);
                            if (Header.Command == "UserCheck")
                            {
                                Header.Contributor = new Contributor(int.MaxValue, Header.UserIDStr);
                                Header.Contributor.SetUserIP((System.Net.IPEndPoint)TCPSocket.RemoteEndPoint);
                                try
                                {
                                    Logger.ConsoleWriteLine("UserCheck for " + Header.Contributor.IP + " tried UserID(" + Header.UserIDStr + ") raw(" + currHeaderSplit[1] + ")");
                                }
                                catch (Exception)
                                {}
                            }
                            else
                            {
                                Header.Contributor = ContributorDB.GetContributor(Header.UserIDStr, (System.Net.IPEndPoint)TCPSocket.RemoteEndPoint/*Client.Client.RemoteEndPoint*/);
                                if (Header.Contributor.UserID != Header.UserIDStr)
                                {
                                    Logger.ConsoleWriteLine("User(" + Header.Contributor.IP + ") tried to access using UserID(" + Header.UserIDStr + ") raw(" + currHeaderSplit[1] + ")");
                                }
                            }
                        }
                        else if (currHeaderSplit[0] == "AddonVersion")
                            Header.AddonVersion = Utility.ParseSingle(currHeaderSplit[1]);
                        else if (currHeaderSplit[0] == "Flags")
                        {
                            Header.Flags = currHeaderSplit[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Old uploader version that is not supported anymore");
            }
        }
        private void ProcessRetrievedData()
        {
            if (Header.Command == "RPUpload")
            {
                if (Header.UploaderVersion >= 1.0f && Header.Contributor != null)
                {
                    //No processessing is needed
                }
                else
                    throw new Exception("Uploader version is faulty");
            }
        }
        private void SendResponse()
        {
            if (Header.Command == "RPUpload")
            {
                byte[] standardSuccessResponse = System.Text.Encoding.UTF8.GetBytes("VF_RPP_Success=true;");
                if (Header.UploaderVersion >= 1.3f)
                {
                    if (Header.UploaderVersion < 1.7f)
                        Header.FileType = FileUploadType.RealmPlayers;

                    if (Header.FileType == FileUploadType.RealmPlayers)
                    {
                        byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(
                            AddonHandler.CreateAddonUpdate(Header.Contributor, Header.AddonVersion) //"" om inget behövs uppdateras
                            + "VF_RPP_Success=true;");
                        TCPSocket.Send(responseBytes);
                    }
                    else if (Header.FileType == FileUploadType.RaidDamage)
                    {
                        TCPSocket.Send(standardSuccessResponse);
                    }
                }
                else if (Header.UploaderVersion >= 1.2f)
                {
                    byte[] extraResponse = System.Text.Encoding.UTF8.GetBytes("Message=There is a newer version of the uploader available;");
                    TCPSocket.Send(extraResponse);
                    TCPSocket.Send(standardSuccessResponse);
                }
                else
                {
                    throw new Exception("Uploader version is too old");
                }
            }
            else if (Header.Command == "UserCheck")
            {
                var result = ContributorDB.CheckContributor(Header.UserIDStr, (System.Net.IPEndPoint)TCPSocket.RemoteEndPoint);
                if (result == ContributorDB.CheckContributorResult.UserID_Success_Login
                || result == ContributorDB.CheckContributorResult.UserID_Success_Registered)
                {
                    TCPSocket.Send(System.Text.Encoding.UTF8.GetBytes("VF_RPP_Success=true;"));
                }
                else
                {
                    TCPSocket.Send(System.Text.Encoding.UTF8.GetBytes("VF_RPP_Success=false;"));
                }
            }
        }
        private void CloseConnection()
        {
            if (TCPSocket != null)
            {
                TCPSocket.Shutdown(SocketShutdown.Both);
                TCPSocket.Close();
            }
            TCPSocket = null;
        }
    }
}
