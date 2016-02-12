using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace VF_RealmPlayersDatabase.UploaderCommunication
{
    public class RPPCommunicator
    {
        ConcurrentQueue<RPPContribution> m_DownloadedData = new ConcurrentQueue<RPPContribution>();
        ConcurrentQueue<RPPContribution> m_DownloadedDataRD = new ConcurrentQueue<RPPContribution>();
        int m_DownloadedFileCounter = 0;
        Socket m_ServerSocket = null;
        //TcpListener m_TCPListener = null;
        System.Threading.Thread m_ListenerThread;
        public RPPCommunicator(int _Port) //18374
        {
            if (System.IO.Directory.Exists("RPPContributions\\") == true)
            {
                string[] files = System.IO.Directory.GetFiles("RPPContributions\\");
                foreach (var file in files)
                {
                    try
                    {
                        if (file.StartsWith("RPPContributions\\RealmPlayersData") == true)
                        {
                            //Old version
                            string contributorUserID = file.Split('_').Last().Replace(".txt", "");
                            Logger.ConsoleWriteLine("RPP File: " + file + ", was uploaded by UserID: " + contributorUserID, ConsoleColor.Yellow);
                            Contributor contributor = ContributorDB.GetContributor(contributorUserID, new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 18374), false);
                            if (contributor != null)
                                m_DownloadedData.Enqueue(new RPPContribution(contributor, file));
                            else
                                Logger.ConsoleWriteLine("Contributor: " + contributorUserID + " was not found", ConsoleColor.Red);
                        }
                        else if (file.StartsWith("RPPContributions\\RPP_") == true)
                        {
                            string contributorUserID = file.Split('_')[1];
                            string contributorUserIP = file.Split('_')[2];
                            Logger.ConsoleWriteLine("RPP File: " + file + ", was uploaded by UserID: " + contributorUserID + ", on IP: " + contributorUserIP, ConsoleColor.Yellow);
                            Contributor contributor = ContributorDB.GetContributor(contributorUserID, new System.Net.IPEndPoint(System.Net.IPAddress.Parse(contributorUserIP), 18374), false);
                            if (contributor != null)
                                m_DownloadedData.Enqueue(new RPPContribution(contributor, file));
                            else
                                Logger.ConsoleWriteLine("Contributor: " + contributorUserID + " was not found", ConsoleColor.Red);
                        }
                        else
                            Logger.ConsoleWriteLine("Skipping file: " + file + ", does not seem to be a RPP file");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
            if (System.IO.Directory.Exists("RDContributions\\") == true)
            {
                string[] files = System.IO.Directory.GetFiles("RDContributions\\");
                foreach (var file in files)
                {
                    try
                    {
                        if (file.StartsWith("RDContributions\\RD_") == true)
                        {
                            string contributorUserID = file.Split('_')[1];
                            string contributorUserIP = file.Split('_')[2];
                            Logger.ConsoleWriteLine("RD File: " + file + ", was uploaded by UserID: " + contributorUserID + ", on IP: " + contributorUserIP, ConsoleColor.Yellow);
                            Contributor contributor = ContributorDB.GetContributor(contributorUserID, new System.Net.IPEndPoint(System.Net.IPAddress.Parse(contributorUserIP), 18374), false);
                            if (contributor != null)
                                m_DownloadedDataRD.Enqueue(new RPPContribution(contributor, file));
                            else
                                Logger.ConsoleWriteLine("Contributor: " + contributorUserID + " was not found", ConsoleColor.Red);
                        }
                        else
                            Logger.ConsoleWriteLine("Skipping file: " + file + ", does not seem to be a RD file");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }

            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ServerSocket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, _Port));
            //m_TCPListener = new TcpListener(System.Net.IPAddress.Any, _Port);
            m_ListenerThread = new System.Threading.Thread(new System.Threading.ThreadStart(ListenerThread));
            m_ListenerThread.Start();
        }
        private void ListenerThread()
        {
            m_ServerSocket.Listen(100);
            //m_TCPListener.Start();
            System.Threading.Tasks.Task[] taskSlots = new System.Threading.Tasks.Task[8];
            while (m_ServerSocket/*m_TCPListener*/ != null)
            {
                try
                {
                    Socket unhandledClient = m_ServerSocket.Accept();
                    while (unhandledClient != null)
                    {
                        for (int i = 0; i < 8; ++i)
                        {
                            if (taskSlots[i] == null || taskSlots[i].IsCompleted == true)
                            {
                                Socket handledClient = unhandledClient;
                                unhandledClient = null;
                                taskSlots[i] = new System.Threading.Tasks.Task(() =>
                                {
                                    try
                                    {
                                        var rppConn = new RPPConnection(handledClient);
                                        rppConn.Run(SaveReceivedFile);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogException(ex);
                                    }
                                });
                                taskSlots[i].Start();
                                break;
                            }
                        }
                        if (unhandledClient != null)
                            System.Threading.Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("A blocking operation was interrupted by a call to WSACancelBlockingCall") == false)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
            m_ListenerThread = null;
        }
        private void SaveReceivedFile(Contributor _Contributor, string _Data, RPPConnection.HeaderData _Header)
        {
            string baseType = "RPP";
            if (_Header.FileType == FileUploadType.RaidDamage)
                baseType = "RD";
            string rppContributionsDir = baseType + "Contributions\\";
            if (_Contributor.IsVIP() == false)
                rppContributionsDir = baseType + "UnknownContributions\\";
            if (_Header.IsTemperedFlag() == true)
            {
                rppContributionsDir = rppContributionsDir.Replace(baseType, baseType + "Tempered");
                Logger.ConsoleWriteLine("File received from " + _Contributor.UserID + "_" + _Contributor.IP + " was tempered with! not using file, saving it in " + rppContributionsDir, ConsoleColor.Red);
            }

            if (System.IO.Directory.Exists(rppContributionsDir) == false)
                System.IO.Directory.CreateDirectory(rppContributionsDir);
            int fileCounterValue = System.Threading.Interlocked.Increment(ref m_DownloadedFileCounter);
            string fileName = rppContributionsDir + baseType + "_" + _Contributor.UserID + "_" + _Contributor.IP + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_FID" + fileCounterValue + ".txt";
            System.IO.File.WriteAllText(fileName, _Data);
            if (_Contributor.IsVIP() == true && _Header.IsTemperedFlag() == false)
            {
                if(_Header.FileType == FileUploadType.RealmPlayers)
                    m_DownloadedData.Enqueue(new RPPContribution(_Contributor, fileName));
                else
                    m_DownloadedDataRD.Enqueue(new RPPContribution(_Contributor, fileName));
            }
        }
        public int GetRPPContributionCount()
        {
            return m_DownloadedData.Count;
        }
        public bool GetNextRPPContribution(out RPPContribution _ReturnData)
        {
            return m_DownloadedData.TryDequeue(out _ReturnData);
        }
        public bool GetNextRDContribution(out RPPContribution _ReturnData)
        {
            return m_DownloadedDataRD.TryDequeue(out _ReturnData);
        }
        public void Shutdown()
        {
            var serverSocket = m_ServerSocket;
            m_ServerSocket = null;
            serverSocket.Close(5);
            //m_TCPListener = null;
        }
        public void WaitForClosed()
        {
            Console.Write("Waiting for ListenerThread");
            while (m_ListenerThread != null)
            {
                System.Threading.Thread.Sleep(500);
                Console.Write(".");
            }
        }
    }
}
