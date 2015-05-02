using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace VF
{
    public partial class NetworkServer
    {
        Socket m_ListenerSocket = null;
        System.Threading.Thread m_ServerThread;

        ConcurrentQueue<NetworkIncommingMessage> m_InternalMessages = new ConcurrentQueue<NetworkIncommingMessage>();
        ConcurrentQueue<NetworkIncommingMessage> m_Messages = new ConcurrentQueue<NetworkIncommingMessage>();
        System.Threading.AutoResetEvent m_NewMessageEvent = new System.Threading.AutoResetEvent(false);

        List<NetworkServerClient> m_Clients = new List<NetworkServerClient>();

        public NetworkServer(int _Port)
        {
            m_ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ListenerSocket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, _Port));
            m_ListenerSocket.NoDelay = true;
            m_ServerThread = new System.Threading.Thread(ServerThread);
            m_ServerThread.Start();
        }

        private void ServerThread()
        {
            m_ListenerSocket.Listen(10);
            m_ListenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), this);
            while (m_ListenerSocket != null)
            {
                try
                {
                    Heartbeat();
                    System.Threading.Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    VF.Logger.LogException(ex);
                }
            }
        }

        private void Heartbeat()
        {
            List<NetworkServerClient> clients = null;
            lock (m_Clients)
            {
                clients = new List<NetworkServerClient>(m_Clients);
            }
            foreach(var client in clients)
            {
                if (client._GetExceptions().Count > 0 || (client.Socket != null && client.Socket.Connected == false)
                    || client.IsReceiving() == false)
                {
                    try
                    {
                        Exception clientException;
                        while (client._GetExceptions().TryDequeue(out clientException) == true)
                        {
                            VF.Logger.LogException(clientException);
                        }
                        client.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        VF.Logger.LogException(ex);
                    }
                    lock (m_Clients)
                    {
                        m_Clients.Remove(client);
                    }
                }
            }
        }
        private void AcceptCallback(IAsyncResult _AsyncResult)
        {
            try
            {
                Socket newConnection = m_ListenerSocket.EndAccept(_AsyncResult);
                NetworkServerClient serverClient = new NetworkServerClient(this, newConnection);
                lock (m_Clients)
                {
                    m_Clients.Add(serverClient);
                }
                try
                {
                    serverClient._StartReceive();
                }
                catch (Exception ex)
                {
                    try
                    {
                        serverClient.Disconnect();
                    }
                    catch (Exception)
                    {
                    }
                    lock (m_Clients)
                    {
                        m_Clients.Remove(serverClient);
                    }
                }
            }
            catch (Exception ex)
            {
                VF.Logger.LogException(ex);
            }
            try
            {
                m_ListenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), this);
            }
            catch (Exception ex)
            {
                VF.Logger.LogException(ex);
            }
        }

        internal NetworkIncommingMessage _CreateMessage(NetworkServerClient _Client)
        {
            NetworkIncommingMessage newMessage = new NetworkIncommingMessage(_Client.Socket);
            return newMessage;
        }

        internal NetworkOutgoingMessage _CreateOutgoingMessage(NetworkServerClient _ServerClient, int _InitialSize = -1)
        {
            NetworkOutgoingMessage newMessage = new NetworkOutgoingMessage(_ServerClient.Socket, NetworkMessageType.RawData, _InitialSize);
            return newMessage;
        }

        internal void _AddReceivedMessage(NetworkIncommingMessage _Message)
        {
            if (_Message.MessageType == NetworkMessageType.RawData)
            {
                m_Messages.Enqueue(_Message);
                m_NewMessageEvent.Set();
            }
            else
                m_InternalMessages.Enqueue(_Message);
        }

        public NetworkIncommingMessage WaitMessage(TimeSpan _Timeout)
        {
            NetworkIncommingMessage msg = ReadMessage();
            if (msg != null)
                return msg;

            m_NewMessageEvent.WaitOne(_Timeout);
            return ReadMessage();
        }

        private NetworkIncommingMessage ReadMessage()
        {
            NetworkIncommingMessage msg = null;
            if (m_Messages.TryDequeue(out msg) == false)
                return null;

            return msg;
        }

        public void SendMessage(NetworkOutgoingMessage _Message)
        {
            var client = m_Clients.FirstOrDefault((_Value) => _Value.Socket == _Message._GetSocket());
            if (client == null)
                throw new Exception("Client was not Connected");

            client.SendMessage(_Message);
        }
        //public void SendPacket<T_PacketTypeEnum, T_PacketClass>(NetworkServerClient _Client, T_PacketTypeEnum _PacketType, T_PacketClass _Data) where T_PacketTypeEnum : struct, IConvertible
        //{
        //    var memoryReturnBuffer = new System.IO.MemoryStream();
        //    ProtoBuf.Serializer.Serialize(memoryReturnBuffer, _Data);
        //    memoryReturnBuffer.Position = 0;
        //    byte[] returnBytes = new byte[memoryReturnBuffer.Length];
        //    memoryReturnBuffer.Read(returnBytes, 0, (int)memoryReturnBuffer.Length);

        //    NetworkOutgoingMessage newMessage = new NetworkOutgoingMessage(_Client.Socket, NetworkMessageType.ClassPacket, 4 + 4 + returnBytes.Length);
        //    newMessage.WriteInt32((int)_PacketType);
        //    newMessage.Write((int)returnBytes.Length);
        //    newMessage.Write(returnBytes);

        //}
        public void Close()
        {
            Logger.LogMessage("Not Implemented Close Yet. Whatever", ConsoleColor.Red);
        }
    }
}
