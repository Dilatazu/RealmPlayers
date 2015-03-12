using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace VF
{
    public partial class NetworkServerClient
    {
        NetworkServer m_Server = null;
        Socket m_Socket = null;

        int m_RecvBufferSize = 1024;
        byte[] m_RecvBuffer;

        NetworkIncommingMessage m_CurrentIncommingMessage = null;
        ConcurrentQueue<NetworkOutgoingMessage> m_Messages = new ConcurrentQueue<NetworkOutgoingMessage>();
        NetworkOutgoingMessage m_CurrentOutgoingMessage = null;

        ConcurrentQueue<Exception> m_Exceptions = new ConcurrentQueue<Exception>();

        bool m_CurrentReceiving = false;
        bool m_CurrentlySending = false;
        public bool IsReceiving() { return m_CurrentReceiving; }
        public bool IsSending() { return m_CurrentlySending; }
        public NetworkServerClient(NetworkServer _Server, Socket _Connection)
        {
            m_Server = _Server;
            m_Socket = _Connection;
            m_Socket.NoDelay = true;
            m_RecvBuffer = new byte[m_RecvBufferSize];
        }

        public Socket Socket
        {
            get 
            {
                return m_Socket;
            }
        }
        public bool HaveOutgoingMessages()
        {
            if (m_Messages.Count > 0 && m_CurrentOutgoingMessage != null)
                return true;
            else
                return false;
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
                m_Socket = null;
            }
        }
        internal ConcurrentQueue<Exception> _GetExceptions()
        {
            return m_Exceptions;
        }
        internal void _StartReceive()
        {
            if (m_CurrentReceiving == false)
            {
                m_CurrentReceiving = true;
                m_Socket.BeginReceive(m_RecvBuffer, 0, m_RecvBufferSize, 0, new AsyncCallback(ReceiveCallback), this);
            }
        }
        internal void _StartSending()
        {
            try
            {
                if (m_CurrentlySending == false)
                {
                    if(m_Messages.TryDequeue(out m_CurrentOutgoingMessage) == true)
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
            catch (Exception ex)
            {
                m_Exceptions.Enqueue(ex);
                m_CurrentlySending = false;
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
                    if (m_Messages.TryDequeue(out m_CurrentOutgoingMessage) == true)
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
                    m_CurrentOutgoingMessage = null;
                    m_CurrentlySending = false;
                }
            }
            catch (Exception ex)
            {
                m_Exceptions.Enqueue(ex);
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
                            m_CurrentIncommingMessage = m_Server._CreateMessage(this);

                        int bytesAdded = m_CurrentIncommingMessage._AddData(m_RecvBuffer, bytesHandled, bytesRead - bytesHandled);
                        if (m_CurrentIncommingMessage.IsComplete() == true)
                        {
                            //Paketet är färdigt
                            m_Server._AddReceivedMessage(m_CurrentIncommingMessage);
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
            catch(System.Net.Sockets.SocketException ex)
            {
                m_CurrentReceiving = false;
                if (ex.NativeErrorCode.Equals(10054)) //WSAECONNRESET "An existing connection was forcibly closed by the remote host"
                {
                    //Do nothing. just shutdown connection
                }
                else
                {
                    m_Exceptions.Enqueue(ex);
                }
            }
            catch (Exception ex)
            {
                m_Exceptions.Enqueue(ex);
                m_CurrentReceiving = false;
            }
        }

        public NetworkOutgoingMessage CreateMessage(int _InitialSize = -1)
        {
            return m_Server._CreateOutgoingMessage(this, _InitialSize);
        }

        internal void SendMessage(NetworkOutgoingMessage _Message)
        {
            _Message._CompilePacket();
            m_Messages.Enqueue(_Message);
            _StartSending();
        }
    }
}
