using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF
{
    public class PacketHeader
    {
        public byte PacketType;
        public int PacketByteLength;
    }
    public enum NetworkMessageType
    {
        RawData = 255,
    }
    //public class NetworkMessage
    //{
    //    private enum MessageDirection
    //    {
    //        Incomming = 0,
    //        Outgoing = 1,
    //    }

        
    //}
}
