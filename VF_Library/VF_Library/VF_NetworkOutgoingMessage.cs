using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace VF
{
    public class NetworkOutgoingMessage
    {
        private Socket m_Socket;

        private byte[] m_Buffer = null;
        private NetworkBuffer m_NetworkBuffer = null;
        public NetworkOutgoingMessage(Socket _Socket, NetworkMessageType _Type, int _InitialSize)
        {
            if (_InitialSize < 0)
                _InitialSize = 1024;

            m_Socket = _Socket;
            m_Buffer = new byte[_InitialSize + 5];
            m_Buffer[0] = (byte)_Type;
            m_NetworkBuffer = new NetworkBuffer(m_Buffer, 5, m_Buffer.Length);
        }

        public byte[] _GetBuffer()
        {
            return m_Buffer;
        }
        public int _GetBufferSize()
        {
            return m_NetworkBuffer.Offset;
        }
        public Socket _GetSocket()
        {
            return m_Socket;
        }

        private void AssureCapacity(int _ByteCount)
        {
            if (m_NetworkBuffer.BytesLeft < _ByteCount)
            {
                byte[] newBuffer = new byte[m_Buffer.Length + _ByteCount + 1024];
                for (int i = 0; i < m_Buffer.Length; ++i)
                    newBuffer[i] = m_Buffer[i];
                int currOffset = m_NetworkBuffer.Offset;
                m_Buffer = newBuffer;
                m_NetworkBuffer = new NetworkBuffer(m_Buffer, currOffset, m_Buffer.Length);
            }
        }
        public void WriteByte(byte _Byte)
        {
            AssureCapacity(1);
            m_NetworkBuffer.WriteByte(_Byte);
        }
        public void WriteInt32(Int32 _Int)
        {
            AssureCapacity(4);
            m_NetworkBuffer.WriteInt32(_Int);
        }
        public void WriteBytes(byte[] _Bytes)
        {
            AssureCapacity(_Bytes.Length);
            m_NetworkBuffer.WriteBytes(_Bytes, 0, _Bytes.Length);
        }
        public void WriteBytes(byte[] _Bytes, int _Offset, int _Count)
        {
            AssureCapacity(_Count);
            m_NetworkBuffer.WriteBytes(_Bytes, _Offset, _Count);
        }
        public void WriteClass<T_Class>(T_Class _Instance)
        {
            var memoryReturnBuffer = new System.IO.MemoryStream();
            ProtoBuf.Serializer.Serialize(memoryReturnBuffer, _Instance);

            int dataSize = (int)memoryReturnBuffer.Length;
            AssureCapacity(dataSize + 4);

            m_NetworkBuffer.WriteInt32(dataSize);
            memoryReturnBuffer.Position = 0;
            memoryReturnBuffer.Read(m_Buffer, m_NetworkBuffer.Offset, dataSize);

            int currOffset = m_NetworkBuffer.Offset;
            m_NetworkBuffer = new NetworkBuffer(m_Buffer, currOffset + dataSize, m_Buffer.Length);
        }

        public void _CompilePacket()
        {
            byte[] bytes = BitConverter.GetBytes(m_NetworkBuffer.Offset - 5);
            for (int i = 0; i < bytes.Length; ++i)
                m_Buffer[i + 1] = bytes[i];
        }
    }
}
