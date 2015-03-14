using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace WowLauncherNetwork
{
    public enum WLN_PacketType
    {
        Request_AddonUpdateInfo = 0,
        Response_AddonUpdateInfo = 1,
        Upload_AddonData = 2,
        Upload_SuccessResponse = 3,
        Request_AddonUpdateInfoNew = 4,
        Request_AddonUpdateInfoPeek = 5,
        Response_AddonUpdateInfoPeek = 6,

        Request_SearchAddons = 102,
        Response_SearchAddons = 103,
        Request_RegisterNewUserID = 201,
        Response_RegisterNewUserID = 202,
    }
    [ProtoContract]
    public class WLN_RequestPacket_AddonUpdateInfo
    {
        [ProtoMember(1)]
        public string AddonName = "null";
        [ProtoMember(2)]
        public string CurrentVersion = "null";
    }
    [ProtoContract]
    public class WLN_RequestPacket_AddonUpdateInfoNew
    {
        [ProtoMember(1)]
        public string UserID = "Unknown.123456";
        [ProtoMember(2)]
        public string LauncherVersion = "1.0";
        [ProtoMember(3)]
        public VF_WoWLauncher.WowVersionEnum WowVersion = VF_WoWLauncher.WowVersionEnum.Vanilla;
        [ProtoMember(4)]
        public List<WLN_RequestPacket_AddonUpdateInfo> Addons = new List<WLN_RequestPacket_AddonUpdateInfo>();
    }
    [ProtoContract]
    public class WLN_RequestPacket_AddonUpdateInfoPeek
    {
        [ProtoMember(1)]
        public string UserID = "Unknown.123456";
        [ProtoMember(2)]
        public string LauncherVersion = "1.0";
        [ProtoMember(3)]
        public int MinutesSinceLastPeek;
    }
    [ProtoContract]
    public class WLN_ResponsePacket_AddonUpdateInfoPeek
    {
        [ProtoMember(1)]
        public List<string> AddonUpdatesAvailable = new List<string>();
    }
    [ProtoContract]
    public class WLN_ResponsePacket_AddonUpdateInfo
    {
        [ProtoMember(1)]
        public string AddonName = "null";
        [ProtoMember(2)]
        public string CurrentVersion = "null";
        [ProtoMember(3)]
        public string UpdateVersion = "null";
        [ProtoMember(4)]
        public string UpdateDescription = "null";
        [ProtoMember(5)]
        public string AddonPackageDownloadFTP = "null";
        [ProtoMember(6)]
        public int AddonPackageFileSize = 0;
        [ProtoMember(7)]
        public bool ClearAccountSavedVariablesRequired = false;
        [ProtoMember(8)]
        public bool ClearCharacterSavedVariablesRequired = false;
        [ProtoMember(9)]
        public string UpdateSubmitter = "null";
        [ProtoMember(10)]
        public VF_WoWLauncher.ServerComm.UpdateImportance UpdateImportance = VF_WoWLauncher.ServerComm.UpdateImportance.Minor;
        [ProtoMember(11)]
        public string MoreInfoSite = "";
    }
    [ProtoContract]
    public class WLN_RequestPacket_SearchAddons
    {
        [ProtoMember(1)]
        public string AddonSearchString = "null";
        [ProtoMember(2)]
        public string[] AddonCategories = null;
    }
    [ProtoContract]
    public class WLN_UploadPacket_AddonData
    {
        [ProtoMember(1)]
        public string AddonName = "null";
        [ProtoMember(2)]
        public string UserID = "null";
        [ProtoMember(3)]
        public short[] FileStatus;
        [ProtoMember(4)]
        public string Data = "null";
    }
    [ProtoContract]
    public class WLN_UploadPacket_SuccessResponse
    {
        [ProtoMember(1)]
        public string MessageToUser = "";
    }
    [ProtoContract]
    public class WLN_Request_RegisterNewUserID
    {
        [ProtoMember(1)]
        public string UserID = "";
        [ProtoMember(2)]
        public string RegisterPassword = "";
        [ProtoMember(3)]
        public string NewUsername = "";
    }
    [ProtoContract]
    public class WLN_Response_RegisterNewUserID
    {
        [ProtoMember(1)]
        public string UserID = "";
    }

    public static class NetworkSender
    {
        //public static void SendPacket_VF<T_Data>(this NetClient _Client, WLN_PacketType _PacketType, T_Data _Data, bool _Flush = true)
        //{
        //    NetOutgoingMessage newMessage = _Client.CreateMessage();
        //    newMessage.Write((byte)_PacketType);
        //    var memoryReturnBuffer = new System.IO.MemoryStream();
        //    Serializer.Serialize(memoryReturnBuffer, _Data);
        //    memoryReturnBuffer.Position = 0;
        //    byte[] returnBytes = new byte[memoryReturnBuffer.Length];
        //    memoryReturnBuffer.Read(returnBytes, 0, (int)memoryReturnBuffer.Length);
        //    newMessage.Write((int)returnBytes.Length);
        //    newMessage.Write(returnBytes);
        //    _Client.SendMessage(newMessage, NetDeliveryMethod.ReliableOrdered, 0);
        //    if (_Flush == true)
        //        _Client.FlushSendQueue();
        //}
        //public static void SendPacket_VF<T_Data>(this NetConnection _Connection, WLN_PacketType _PacketType, T_Data _Data)
        //{
        //    NetOutgoingMessage newMessage = _Connection.Peer.CreateMessage();
        //    newMessage.Write((byte)_PacketType);
        //    var memoryReturnBuffer = new System.IO.MemoryStream();
        //    Serializer.Serialize(memoryReturnBuffer, _Data);
        //    memoryReturnBuffer.Position = 0;
        //    byte[] returnBytes = new byte[memoryReturnBuffer.Length];
        //    memoryReturnBuffer.Read(returnBytes, 0, (int)memoryReturnBuffer.Length);
        //    newMessage.Write((int)returnBytes.Length);
        //    newMessage.Write(returnBytes);
        //    _Connection.SendMessage(newMessage, NetDeliveryMethod.ReliableOrdered, 0);
        //}
        //public static T_Data RecvPacketData_VF<T_Data>(this NetIncomingMessage _Message)
        //{
        //    int packetLength = _Message.ReadInt32();
        //    byte[] rawData = _Message.ReadBytes(packetLength);
        //    var memoryBuffer = new System.IO.MemoryStream(rawData);
        //    T_Data dataReceived = Serializer.Deserialize<T_Data>(memoryBuffer);
        //    return dataReceived;           
        //}
        public static bool RecvPacket_VF<T_Data>(this VF.NetworkClient _Client, WLN_PacketType _PacketType, out T_Data _RetData, TimeSpan _Timeout)
        {
            DateTime startRecv = DateTime.Now;
            if (_Client.WaitForConnect(_Timeout) == false)
                throw new Exception("Not Connected");
            VF.NetworkIncommingMessage msg;
            while (_Client.IsConnected())
            {
                msg = _Client.WaitMessage(_Timeout);
                if (msg != null)
                {
                    if (msg.MessageType == VF.NetworkMessageType.RawData)
                    {
                        WLN_PacketType packetType = (WLN_PacketType)msg.ReadByte();
                        if (packetType == _PacketType)
                        {
                            _RetData = msg.ReadClass<T_Data>();
                            return true;
                        }
                        else
                        {
                            _RetData = default(T_Data);
                            return false;// throw new Exception("Unexpected Packet");
                        }
                    }
                }
                if ((DateTime.Now - startRecv) > _Timeout)
                    throw new TimeoutException("Could not receive packet within the time period");
            }
            _RetData = default(T_Data);
            throw new Exception("Not Connected");
        }
        public static bool RecvPacket_VF<T_Data>(this VF.NetworkClient _Client, WLN_PacketType _PacketType, out T_Data _RetData)
        {
            return RecvPacket_VF(_Client, _PacketType, out _RetData, TimeSpan.FromSeconds(30));
        }
    }
    public class WLN_Network
    {
        //public static NetClient Connect(string _Host, int _Port)
        //{
        //    return Connect(_Host, _Port, TimeSpan.FromSeconds(20));
        //}
        //public static NetClient Connect(string _Host, int _Port, TimeSpan _Timeout)
        //{
        //    NetPeerConfiguration config = new NetPeerConfiguration("chat");
        //    config.AutoFlushSendQueue = false;
        //    config.AutoExpandMTU = true;
        //    config.ReceiveBufferSize = 1024 * 1024 * 64;
        //    config.SendBufferSize = 1024 * 1024 * 64;
        //    NetClient netClient = new NetClient(config);
        //    netClient.Start();
        //    netClient.Connect(_Host, _Port);

        //    DateTime startConnect = DateTime.Now;

        //    NetIncomingMessage im;
        //    while (netClient.ConnectionStatus != NetConnectionStatus.Connected)
        //    {
        //        im = netClient.WaitMessage(_Timeout.Milliseconds / 10);
        //        if ((DateTime.Now - startConnect) > _Timeout)
        //            throw new TimeoutException("Could not start connection");
        //    }
        //    return netClient;
        //}
        //public static void Disconnect(NetClient _Client)
        //{
        //    _Client.Disconnect("Byebye");
        //}
    }
}