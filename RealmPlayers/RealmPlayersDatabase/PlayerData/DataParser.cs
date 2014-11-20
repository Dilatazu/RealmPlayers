using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase.PlayerData
{
    class DataParser
    {
        public static DateTime ParseLastSeenUTC(System.Xml.XmlNode _PlayerNode)
        {
            string dateTime = XMLUtility.GetChildValue(_PlayerNode, "DateTimeUTC", "2000-01-01 01:01:01");
            DateTime lastSeen;
            System.DateTime.TryParse(dateTime, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out lastSeen);
            return lastSeen;
        }
        public static string ParsePlayerName(System.Xml.XmlNode _PlayerNode)
        {
            return _PlayerNode.Attributes[0].Value;//Ful lösning, men fungerar, kanske borde ändras i framtiden
        }
        public static string ParseRealm(System.Xml.XmlNode _PlayerNode)
        {
            try
            {
                string[] playerData = XMLUtility.GetChildValue(_PlayerNode, "PlayerData", "").Split(new char[] { ':' });
                return playerData[7];
            }
            catch (Exception)
            { }
            return "";
        }
    }
}
