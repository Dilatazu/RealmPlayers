using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VF
{
    public class LootGenerator
    {
        public static string CreateLootDroppedData(List<Tuple<string, int>> _ItemDrops, VF_RaidDamageDatabase.RealmDB _RealmDB, VF_RPDatabase.ItemSummaryDatabase _ItemSummaryDatabase, Func<int, VF_RealmPlayersDatabase.WowVersionEnum, RealmPlayersServer.ItemInfo> _GetItemInfoFunc)
        {
            var wowVersion = VF_RealmPlayersDatabase.StaticValues.GetWowVersion(_RealmDB.Realm);
            string currentItemDatabase = RealmPlayersServer.DatabaseAccess.GetCurrentItemDatabaseAddress();
            string lootDropped = "";
            if (_ItemDrops.Count > 0)
            {
                int recvItemIndex = 0;
                int xMax = 58;
                int yMax = 58;

                string itemLinks = "";
                foreach (var itemDrop in _ItemDrops)
                {
                    VF_RealmPlayersDatabase.PlayerData.Player itemReceiver = _RealmDB.m_RealmDB.FindPlayer(itemDrop.Item1);
                    int itemID = itemDrop.Item2;
                    var itemInfo = _GetItemInfoFunc(itemID, wowVersion);
                    if (itemInfo != null && itemInfo.ItemQuality >= 4)
                    {
                        var usageCount = _ItemSummaryDatabase.GetItemUsageCount(_RealmDB.Realm, itemID, 0);

                        int xPos = (recvItemIndex % 4) * 58;
                        int yPos = (int)(recvItemIndex / 4) * 58;
                        string itemLink = currentItemDatabase + "?item=" + itemID + (wowVersion == VF_RealmPlayersDatabase.WowVersionEnum.TBC ? "-1" : "-0");

                        itemLinks += "<div style='background: none; width: 58px; height: 58px;margin: " + yPos + "px " + xPos + "px;'>"
                            + "<img class='itempic' src='" + "http://realmplayers.com/" + itemInfo.GetIconImageAddress() + "'/>"
                            + "<div class='quality' id='" + RealmPlayersServer.CharacterViewer.ItemQualityConversion[itemInfo.ItemQuality] + "'></div>"
                            + "<img class='itemframe' src='assets/img/icons/ItemNormalFrame.png'/>"
                            + "<a class='itemlink' href='" + itemLink + "'></a>"
                            + (usageCount > 0 ? "<a class='itemplayersframe' href='http://realmplayers.com/ItemUsageInfo.aspx?realm=" + StaticValues.ConvertRealmParam(_RealmDB.Realm) + "&item=" + itemID + "'>" + usageCount + "</a>" : "")
                            + (/*Reliable Received*/true ? "<a class='itemreceiveframe' style='" + (itemReceiver == null ? "color: #CCC !important' href='" + itemLink + "'>???" : "color: " + PageUtility.GetClassColor(itemReceiver) + " !important' href='"
                            //+ "http://realmplayers.com/CharacterViewer.aspx?realm=" + StaticValues.ConvertRealmParam(_RealmDB.Realm) + "&player=" + itemReceiver.Name 
                            + itemLink
                            + "'>" + itemReceiver.Name) + "</a>" : "")
                            + "</div>";

                        if (xPos + 58 > xMax)
                            xMax = xPos + 58;
                        if (yPos + 58 > yMax)
                            yMax = yPos + 58;
                        ++recvItemIndex;
                    }
                    else// if(itemInfo == null)
                    {
                        //Logger.ConsoleWriteLine("ItemInfo could not be found for ItemID: " + itemID, ConsoleColor.Red);
                    }
                }
                itemLinks = "<div class='inventory' style='background: none; width: " + xMax + "px; height: " + yMax + "px;'>" + itemLinks;
                itemLinks += "</div>";

                lootDropped += itemLinks;
            }
            return lootDropped;
        }
    }
}