using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace VF
{
    public class NetworkClient
    {
        Socket m_Socket = null;
        System.Threading.ManualResetEvent m_Connected = new System.Threading.ManualResetEvent(false);

        ConcurrentQueue<NetworkIncommingMessage> m_InternalMessages = new ConcurrentQueue<NetworkIncommingMessage>();
        ConcurrentQueue<NetworkOutgoingMessage> m_OutgoingMessages = new ConcurrentQueue<NetworkOutgoingMessage>();
        ConcurrentQueue<NetworkIncommingMessage> m_IncommingMessages = new ConcurrentQueue<NetworkIncommingMessage>();
        NetworkIncommingMessage m_CurrentIncommingMessage = null;
        NetworkOutgoingMessage m_CurrentOutgoingMessage = null;

        System.Threading.AutoResetEvent m_NewMessageEvent = new System.Threading.AutoResetEvent(false);

        bool m_CurrentReceiving = false;
        bool m_CurrentlySending = false;

        int m_RecvBufferSize = 1024;
        byte[] m_RecvBuffer;

        ConcurrentQueue<Exception> m_Exceptions = new ConcurrentQueue<Exception>();

        public NetworkClient(string _Host, int _Port)
        {
            m_RecvBuffer = new byte[m_RecvBufferSize];

            DateTime startConnectionTime = DateTime.Now;
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.NoDelay = true;
            IAsyncResult asyncResult = null;
            var addressList = System.Net.Dns.GetHostEntry(_Host).AddressList;
            for (int alCounter = 0; alCounter < addressList.Length; ++alCounter)
            {
                try
                {
                    asyncResult = m_Socket.BeginConnect(addressList[alCounter], _Port, new AsyncCallback(ConnectCallback), this);
                    break;
                }
                catch (Exception ex)
                {
                }
            }
            if (asyncResult == null)
                throw new Exception("Unexpected error, could not find host at all!");
        }
        private NetworkIncommingMessage _CreateMessage()
        {
            NetworkIncommingMessage newMessage = new NetworkIncommingMessage(m_Socket);
            return newMessage;
        }
        private void _AddReceivedMessage(NetworkIncommingMessage _Message)
        {
            if (_Message.MessageType == NetworkMessageType.RawData)
            {
                m_IncommingMessages.Enqueue(_Message);
                m_NewMessageEvent.Set();
            }
            else
                m_InternalMessages.Enqueue(_Message);
        }
        private void _StartReceive()
        {
            if (IsConnected())
            {
                if (m_CurrentReceiving == false)
                {
                    m_CurrentReceiving = true;
                    m_Socket.BeginReceive(m_RecvBuffer, 0, m_RecvBufferSize, 0, new AsyncCallback(ReceiveCallback), this);
                }
            }
        }
        private void _StartSending()
        {
            if (IsConnected())
            {
                if (m_CurrentlySending == false)
                {
                    if (m_OutgoingMessages.TryDequeue(out m_CurrentOutgoingMessage) == true)
                    {
                        m_CurrentlySending = true;
                        m_Socket.BeginSend(m_CurrentOutgoingMessage._GetBuffer(), 0, m_CurrentOutgoingMessage._GetBufferSize()
                            , SocketFlags.None, new AsyncCallback(SendCallback), this);
                    }
                    else
                    {
                        m_CurrentOutgoingMessage = null;
                    }
                }
            }
        }
        private void SendCallback(IAsyncResult _AsyncResult)
        {
            try
            {
                int bytesSent = m_Socket.EndSend(_AsyncResult);
                if (bytesSent == m_CurrentOutgoingMessage._GetBufferSize())
                {
                    //Packet sent successfully
                    if (m_OutgoingMessages.TryDequeue(out m_CurrentOutgoingMessage) == true)
                    {
                        m_Socket.BeginSend(m_CurrentOutgoingMessage._GetBuffer(), 0, m_CurrentOutgoingMessage._GetBufferSize()
                            , SocketFlags.None, new AsyncCallback(SendCallback), this);
                    }
                    else
                    {
                        m_CurrentOutgoingMessage = null;
                        m_CurrentlySending = false;
                    }
                }
                else
                {
                    m_Exceptions.Enqueue(new Exception("Not all bytes were sent! only " + bytesSent + " out of " + m_CurrentOutgoingMessage._GetBufferSize()));
                }
            }
            catch (Exception ex)
            {
                m_Connected.Reset();
                m_Exceptions.Enqueue(ex);
                if (m_Socket != null)
                    Logger.LogException(ex);
                m_CurrentlySending = false;
            }
        }
        private void ReceiveCallback(IAsyncResult _AsyncResult)
        {
            try
            {
                int bytesRead = m_Socket.EndReceive(_AsyncResult);
                if (bytesRead > 0)
                {
                    int bytesHandled = 0;
                    while (bytesHandled != bytesRead)
                    {
                        if (m_CurrentIncommingMessage == null)
                            m_CurrentIncommingMessage = _CreateMessage();

                        int bytesAdded = m_CurrentIncommingMessage._AddData(m_RecvBuffer, bytesHandled, bytesRead - bytesHandled);
                        if (m_CurrentIncommingMessage.IsComplete() == true)
                        {
                            //Paketet är färdigt
                            _AddReceivedMessage(m_CurrentIncommingMessage);
                            m_CurrentIncommingMessage = null;
                        }
                        bytesHandled += bytesAdded;
                    }

                    //Fortsätt receive
                    m_Socket.BeginReceive(m_RecvBuffer, 0, m_RecvBufferSize, 0, new AsyncCallback(ReceiveCallback), this);
                }
                else
                {
                    m_CurrentReceiving = false;
                }
            }
            catch (Exception ex)
            {
                m_Connected.Reset();
                m_Exceptions.Enqueue(ex);
                if(m_Socket != null)
                    Logger.LogException(ex);
                m_CurrentReceiving = false;
            }
        }

        private void ConnectCallback(IAsyncResult _AsyncResult)
        {
            try
            {
                m_Socket.EndConnect(_AsyncResult);
                m_Connected.Set();
                _StartReceive();
                _StartSending();
            }
            catch (Exception)
            {
                m_Connected.Reset();
            }
        }
        public bool IsConnected()
        {
            return m_Connected.WaitOne(0);
        }
        public bool WaitForConnect(TimeSpan _Timeout)
        {
            return m_Connected.WaitOne(_Timeout);
        }
        public void Disconnect()
        {
            if (m_Socket != null)
            {
                try
                {
                    try
                    {
                        m_Socket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception)
                    { }
                    m_Socket.Close();
                }
                catch (Exception)
                { }
                m_Connected.Reset();
                m_Socket = null;
            }
        }

        public NetworkOutgoingMessage CreateMessage(int _InitialSize = -1)
        {
            NetworkOutgoingMessage newMessage = new NetworkOutgoingMessage(m_Socket, NetworkMessageType.RawData, _InitialSize);
            return newMessage;
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
            if (m_IncommingMessages.TryDequeue(out msg) == false)
                return null;

            return msg;
        }

        public void SendMessage(NetworkOutgoingMessage _Message)
        {
            _Message._CompilePacket();
            m_OutgoingMessages.Enqueue(_Message);
            _StartSending();
        }
    }
}
