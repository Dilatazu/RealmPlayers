using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace VF
{
    public class NetworkIncommingMessage
    {
        private Socket m_Socket;

        private PacketHeader m_PacketHeader = null;
        private byte[] m_Buffer = null;
        private int m_BufferDataPointer = 0;
        private NetworkBuffer m_NetworkBuffer = null;
        public NetworkIncommingMessage(Socket _Socket)
        {
            m_Socket = _Socket;
            m_PacketHeader = null;
        }

        public NetworkMessageType MessageType
        {
            get
            {
                return (NetworkMessageType)m_PacketHeader.PacketType;
            }
        }
        public int MessageSize
        {
            get
            {
                return m_PacketHeader.PacketByteLength;
            }
        }
        //public NetworkBuffer Data
        //{
        //    get
        //    {
        //        return m_NetworkBuffer;
        //    }
        //}
        //public NetworkServerClient SenderConnection
        //{
        //    get
        //    {
        //        return m_Client;
        //    }
        //}
        public System.Net.IPAddress SenderIP
        {
            get
            {
                return ((System.Net.IPEndPoint)m_Socket.RemoteEndPoint).Address;
            }
        }

        internal int _AddData(byte[] _Buffer, int _Offset, int _Count)
        {
            NetworkBuffer inputBuffer = new NetworkBuffer(_Buffer, _Offset, _Offset + _Count);
            if (m_PacketHeader == null)
            {
                //Headern är inte initierad än
                if (m_BufferDataPointer + _Count < 5)
                {
                    //Inte hela headern är nerladdat, spara undan det vi har fått hittils
                    if (m_Buffer == null)
                        m_Buffer = new byte[5];

                    for (int i = 0; i < _Count; ++i)
                        m_Buffer[m_BufferDataPointer + i] = inputBuffer.ReadByte();

                    m_BufferDataPointer += _Count;
                }
                else
                {
                    //Vi har all data vi behöver för Headern
                    m_PacketHeader = new PacketHeader();
                    if (m_Buffer == null)
                    {
                        //Ingen temporär buffer har skapats vilket betyder vi kan läsa direkt från input
                        m_PacketHeader.PacketType = inputBuffer.ReadByte();
                        m_PacketHeader.PacketByteLength = inputBuffer.ReadInt32();
                    }
                    else
                    {
                        //En temporär buffer har skapats redan, så vi läser först in till den
                        for (int i = m_BufferDataPointer; i < 5; ++i)
                            m_Buffer[i] = inputBuffer.ReadByte();

                        NetworkBuffer packetHeaderBuffer = new NetworkBuffer(m_Buffer, 0, 5);
                        m_PacketHeader.PacketType = packetHeaderBuffer.ReadByte();
                        m_PacketHeader.PacketByteLength = packetHeaderBuffer.ReadInt32();
                    }

                    //PacketHeader har skapats och initieras, skapa buffrarna utefter det som Headern säger
                    if (m_PacketHeader.PacketByteLength < 1024 * 1024 * 32)//32MB
                    {
                        m_Buffer = new byte[m_PacketHeader.PacketByteLength];
                        m_BufferDataPointer = 0;
                    }
                    else
                    {
                        throw new Exception("No support for packets bigger than 32MB yet");
                    }
                }
            }
            ////
            if (m_PacketHeader != null)
            {
                if (m_Buffer.Length != m_PacketHeader.PacketByteLength)//Sanity check
                    throw new Exception("This should never happen!");

                //PacketHeader är inläst och buffern är initierad osv
                NetworkBuffer packetBuffer = new NetworkBuffer(m_Buffer, m_BufferDataPointer, m_Buffer.Length);
                for (; inputBuffer.BytesLeft > 0 && m_BufferDataPointer < m_PacketHeader.PacketByteLength; ++m_BufferDataPointer)
                {
                    packetBuffer.WriteByte(inputBuffer.ReadByte());
                }
                if (m_BufferDataPointer == m_PacketHeader.PacketByteLength)
                {
                    //Vi har fått all data för detta paketet!
                    m_NetworkBuffer = new NetworkBuffer(m_Buffer, 0, m_PacketHeader.PacketByteLength);
                }
            }
            return inputBuffer.Offset - _Offset;
        }

        internal bool IsComplete()
        {
            if (m_PacketHeader == null)
                return false;

            return (m_BufferDataPointer == m_PacketHeader.PacketByteLength);
        }

        public byte ReadByte()
        {
            return m_NetworkBuffer.ReadByte();
        }
        public Int32 ReadInt32()
        {
            return m_NetworkBuffer.ReadInt32();
        }
        public T_Class ReadClass<T_Class>()
        {
            int classLength = m_NetworkBuffer.ReadInt32();
            m_NetworkBuffer._AssertNotOverflow(classLength);
            var memoryBuffer = new System.IO.MemoryStream(m_Buffer, m_NetworkBuffer.Offset, classLength);
            T_Class classInstance = ProtoBuf.Serializer.Deserialize<T_Class>(memoryBuffer);
            m_NetworkBuffer = new NetworkBuffer(m_Buffer, m_NetworkBuffer.Offset + classLength, m_PacketHeader.PacketByteLength);
            return classInstance;
        }

        public NetworkOutgoingMessage CreateResponseMessage(int _InitialSize = -1)
        {
            return new NetworkOutgoingMessage(m_Socket, NetworkMessageType.RawData, _InitialSize);
        }
    }
}
