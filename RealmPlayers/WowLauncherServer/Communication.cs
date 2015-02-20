using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using WowLauncherNetwork;
using VF_RealmPlayersDatabase;

namespace VF_WoWLauncherServer
{
    class Communication
    {
        VF.NetworkServer m_Server;
        //NetServer m_Server;
        System.Threading.Thread m_ListenerThread;

        public Communication()
        {
            m_Server = new VF.NetworkServer(19374);
            m_ListenerThread = new System.Threading.Thread(ListenerThread);
            m_ListenerThread.Start();
        }
        public void Close()
        {
            m_Server.Close();
        }
        private void ListenerThread()
        {
			VF.NetworkIncommingMessage msg;
            while (m_ListenerThread != null)
            {
                try
                {
                    while ((msg = m_Server.WaitMessage(TimeSpan.FromMilliseconds(1000))) != null)
                    {
                        switch (msg.MessageType)
                        {
                            case VF.NetworkMessageType.RawData:
                                {
                                    // incoming message from a client
                                    WLN_PacketType packetType = (WLN_PacketType)msg.ReadByte();
                                    if (packetType == WLN_PacketType.Request_AddonUpdateInfo)
                                    {
#region BACKWARDS_COMPATIBILITY_ONLY
                                        BackwardsCompatible_Request_AddonUpdateInfo(msg);
#endregion BACKWARDS_COMPATIBILITY_ONLY
                                    }
                                    else if (packetType == WLN_PacketType.Request_AddonUpdateInfoNew)
                                    {
                                        WLN_RequestPacket_AddonUpdateInfoNew addonUpdateInfoRequest = msg.ReadClass<WLN_RequestPacket_AddonUpdateInfoNew>();
                                        List<WLN_ResponsePacket_AddonUpdateInfo> result = new List<WLN_ResponsePacket_AddonUpdateInfo>();
                                        Logger.ConsoleWriteLine("Received Request_AddonUpdateInfoNew from IP=" + msg.SenderIP.ToString() + ", UserID=" + addonUpdateInfoRequest.UserID + ", LauncherVersion=" + addonUpdateInfoRequest.LauncherVersion);

                                        Random rand = new Random((int)DateTime.UtcNow.Ticks);
                                        foreach (var addon in addonUpdateInfoRequest.Addons)
                                        {
                                            WLN_ResponsePacket_AddonUpdateInfo addonUpdateInfo = AddonUpdates.GetAddonUpdate(addonUpdateInfoRequest.UserID, addon);
                                            if (addonUpdateInfo != null)
                                            {
                                                result.Add(addonUpdateInfo);
                                            }
                                        }
                                        var response = msg.CreateResponseMessage(-1);//.SenderConnection.SendPacket_VF(WLN_PacketType.Response_AddonUpdateInfo, result);
                                        response.WriteByte((byte)WLN_PacketType.Response_AddonUpdateInfo);
                                        response.WriteClass(result);
                                        m_Server.SendMessage(response);
                                    }
                                    else if(packetType == WLN_PacketType.Request_AddonUpdateInfoPeek)
                                    {
                                        WLN_RequestPacket_AddonUpdateInfoPeek addonUpdateInfoRequest = msg.ReadClass<WLN_RequestPacket_AddonUpdateInfoPeek>();
                                        WLN_ResponsePacket_AddonUpdateInfoPeek result = new WLN_ResponsePacket_AddonUpdateInfoPeek();
                                        if(AddonUpdates.g_LastAddonUpdateTimeUTC > DateTime.UtcNow.AddMinutes(-addonUpdateInfoRequest.MinutesSinceLastPeek))
                                        {
                                            result.AddonUpdatesAvailable.Add("null");
                                        }
                                        var response = msg.CreateResponseMessage(-1);//.SenderConnection.SendPacket_VF(WLN_PacketType.Response_AddonUpdateInfo, result);
                                        response.WriteByte((byte)WLN_PacketType.Response_AddonUpdateInfoPeek);
                                        response.WriteClass(result);
                                        m_Server.SendMessage(response);
                                    }
                                    else if (packetType == WLN_PacketType.Upload_AddonData)
                                    {
                                        WLN_UploadPacket_AddonData uploadedAddonData = msg.ReadClass<WLN_UploadPacket_AddonData>();
                                        Logger.ConsoleWriteLine("Received Upload_AddonData(" + uploadedAddonData.AddonName + ") from " + msg.SenderIP.ToString());
                                        AddonDatabaseService.UploadData(msg.SenderIP, uploadedAddonData);
                                        var successResponse = new WLN_UploadPacket_SuccessResponse();

                                        var response = msg.CreateResponseMessage(-1);//.SenderConnection.SendPacket_VF(WLN_PacketType.Response_AddonUpdateInfo, result);
                                        response.WriteByte((byte)WLN_PacketType.Upload_SuccessResponse);
                                        response.WriteClass(successResponse);
                                        m_Server.SendMessage(response);
                                    }
                                    else if (packetType == WLN_PacketType.Request_RegisterNewUserID)
                                    {
                                        //WLN_Request_RegisterNewUserID registerPacket = msg.ReadClass<WLN_Request_RegisterNewUserID>();
                                        //Logger.ConsoleWriteLine("Received Register New UserID Request from: \"" + registerPacket.UserID + "\" using password: \"" + registerPacket.RegisterPassword + "\" registering name: \"" + registerPacket.NewUsername + "\"", ConsoleColor.Yellow);

                                        //var responseData = new WLN_Response_RegisterNewUserID();
                                        //responseData.UserID = "";

                                        //string userID = "";
                                        //if (ContributorHandler.GenerateUserID(registerPacket.NewUsername, out userID) == true)
                                        //{
                                        //    ContributorHandler.AddVIPContributor(userID);
                                        //    ContributorHandler.Save(Program.g_RPPDBFolder + "Database\\");
                                        //    UpdateContributorsList();
                                        //}

                                        //var response = msg.CreateResponseMessage(-1);
                                        //response.WriteByte((byte)WLN_PacketType.Response_RegisterNewUserID);
                                        //response.WriteClass(responseData);
                                        //m_Server.SendMessage(response);
                                    }
                                    else
                                    {
                                        Logger.ConsoleWriteLine("Some unknown data: type=" + (int)packetType + ", dataSize=" + msg.MessageSize, ConsoleColor.Red);
                                    }
                                }
                                break;
                            default:
                                Logger.ConsoleWriteLine("Unknown Message Received" + "MessageType: " + msg.MessageType + " MessageSize: " + msg.MessageSize + " bytes", ConsoleColor.Red);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        private void BackwardsCompatible_Request_AddonUpdateInfo(VF.NetworkIncommingMessage msg)
        {
            Logger.ConsoleWriteLine("Received Request_AddonUpdateInfo from " + msg.SenderIP.ToString());
            WLN_RequestPacket_AddonUpdateInfo[] addonUpdateInfoRequests = msg.ReadClass<WLN_RequestPacket_AddonUpdateInfo[]>();
            List<WLN_ResponsePacket_AddonUpdateInfo> result = new List<WLN_ResponsePacket_AddonUpdateInfo>();

            Random rand = new Random((int)DateTime.UtcNow.Ticks);
            foreach (var addon in addonUpdateInfoRequests)
            {
                WLN_ResponsePacket_AddonUpdateInfo addonUpdateInfo = AddonUpdates.GetAddonUpdate("", addon);
                if (addonUpdateInfo != null)
                {
                    result.Add(addonUpdateInfo);
                }
            }

            var response = msg.CreateResponseMessage(-1);//.SenderConnection.SendPacket_VF(WLN_PacketType.Response_AddonUpdateInfo, result);
            response.WriteByte((byte)WLN_PacketType.Response_AddonUpdateInfo);
            response.WriteClass(result);
            m_Server.SendMessage(response);
        }
    }
}
