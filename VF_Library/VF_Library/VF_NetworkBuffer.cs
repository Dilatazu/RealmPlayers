using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace VF
{
    public class NetworkBuffer
    {
        byte[] m_Buffer;
        int m_Pointer;
        int m_Size;
        public NetworkBuffer(byte[] _Buffer, int _Offset, int _Size)
        {
            m_Buffer = _Buffer;
            m_Pointer = _Offset;
            m_Size = _Size;
        }
        public int Offset
        {
            get { return m_Pointer; }
        }
        public int Size
        {
            get { return m_Size; }
        }
        public int BytesLeft
        {
            get { return m_Size - m_Pointer; }
        }

        public byte ReadByte()
        {
            _AssertNotOverflow(1);

            byte retValue = m_Buffer[m_Pointer];
            m_Pointer += 1;
            return retValue;
        }
        //public UInt32 ReadUInt32()
        //{
        //    _AssertNotOverflow(4);

        //    UInt32 retValue = BitConverter.ToUInt32(m_Buffer, m_Pointer);
        //    m_Pointer += 4;
        //    return retValue;
        //}
        public Int32 ReadInt32()
        {
            _AssertNotOverflow(4);

            Int32 retValue = BitConverter.ToInt32(m_Buffer, m_Pointer);
            m_Pointer += 4;
            return retValue;
        }
        public T_Data ReadSerializedData<T_Data>()
        {
            int packetLength = ReadInt32();
            _AssertNotOverflow(packetLength);
            var memoryBuffer = new System.IO.MemoryStream(m_Buffer, m_Pointer, packetLength);
            T_Data dataReceived = Serializer.Deserialize<T_Data>(memoryBuffer);
            m_Pointer += packetLength;
            return dataReceived;
        }

        public void WriteByte(byte _Byte)
        {
            _AssertNotOverflow(1);

            m_Buffer[m_Pointer] = _Byte;
            m_Pointer += 1;
        }
        //public void WriteUInt32(UInt32 _UInt)
        //{
        //    byte[] bytes = BitConverter.GetBytes(_UInt);
        //    WriteBytes(bytes, 0, bytes.Length);
        //}
        public void WriteInt32(Int32 _Int)
        {
            byte[] bytes = BitConverter.GetBytes(_Int);
            WriteBytes(bytes, 0, bytes.Length);
        }
        public void WriteBytes(byte[] _Bytes, int _Offset, int _Count)
        {
            _AssertNotOverflow(_Count);

            for(int i = 0; i < _Count; ++i)
                m_Buffer[m_Pointer + i] = _Bytes[_Offset + i];
            
            m_Pointer += _Count;
        }
        public void WriteSerializedData<T_Data>(T_Data _Data)
        {
            var memoryReturnBuffer = new System.IO.MemoryStream();
            Serializer.Serialize(memoryReturnBuffer, _Data);
            memoryReturnBuffer.Position = 0;
            int dataLength = (int)memoryReturnBuffer.Length;
            WriteInt32(dataLength);
            _AssertNotOverflow(dataLength);
            memoryReturnBuffer.Read(m_Buffer, m_Pointer, dataLength);
            m_Pointer += dataLength;
        }

        public void _AssertNotOverflow(int _Count)
        {
            if (_Count < 0 || m_Pointer + _Count > m_Buffer.Length)
                throw new OverflowException("NetworkBuffer overflow trying to Read/Write " + _Count + " bytes");
        }
    }
}
