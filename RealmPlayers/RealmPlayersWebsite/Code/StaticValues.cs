using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RealmPlayersServer.Code.Resources;

using PlayerItem = VF_RealmPlayersDatabase.ItemSlot;
using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;
using PlayerSex = VF_RealmPlayersDatabase.PlayerSex;

using Player = VF_RealmPlayersDatabase.PlayerData.Player;
using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;

namespace RealmPlayersServer
{
    public class StaticValues : VF_RealmPlayersDatabase.StaticValues
    {
        public static string GetTimeSinceLastSeenUTC(DateTime _LastSeenUTC)
        {
            int value = 1;
            string extension = " min";

            var timeDiff = System.DateTime.UtcNow - _LastSeenUTC;

            if ((value = (int)timeDiff.TotalMinutes) < 90)
                extension = " min";
            else if ((value = (int)timeDiff.TotalHours) < 36)
                extension = " hr";
            else if ((value = (int)timeDiff.TotalDays) < 45)
                extension = " day";
            else
            {
                value = (int)(timeDiff.TotalDays / 30);
                extension = " mth";
            }

            return value + extension + (value > 1 ? "s" : "");
        }

        public static string[] ItemDatabaseAddresses = { "http://database.wow-one.com/", "http://db.vanillagaming.org/", "http://db.valkyrie-wow.com/" };

        public static string GetFactionIMG(PlayerFaction _Faction)
        {
            return VisualResources._FactionImgUrl[_Faction];
        }
        public static string GetFactionCSSName(PlayerFaction _Faction)
        {
            return VisualResources._FactionCSSName[_Faction];
        }
        public static string GetRaceCrestImageUrl(PlayerRace _Race)
        {
            if (VisualResources._RaceCrestImage.ContainsKey(_Race))
                return VisualResources._RaceCrestImage[_Race];
            return "error.png";
        }
        //public static string GetRaceIconImageUrl(PlayerRace _Race, PlayerSex _Sex)
        //{
        //    if (_Sex == PlayerSex.Female)
        //        return VisualResources._FemaleRaceImgUrl[_Race];
        //    else
        //        return VisualResources._MaleRaceImgUrl[_Race];
        //}
        public static string ConvertRealmViewing(VF_RealmPlayersDatabase.WowRealm _Realm)
        {
            return VisualResources._RealmVisualString[_Realm];
        }
        public static string ConvertRealmParam(VF_RealmPlayersDatabase.WowRealm _Realm)
        {
            return VisualResources._RealmParamString[_Realm];
        }
        public static string ConvertRankVisual(int _Rank, PlayerFaction _Faction)
        {
            if (_Rank < 0 || _Rank > 14)
                return "Unknown";
            if (_Faction == PlayerFaction.Horde)
                return VisualResources._HordeRankVisualName[_Rank];
            else if (_Faction == PlayerFaction.Alliance)
                return VisualResources._AllianceRankVisualName[_Rank];
            else
                return "Unknown";
        }
        public static string ConvertRankVisualWithNr(int _Rank, PlayerFaction _Faction)
        {
            return ConvertRankVisual(_Rank, _Faction) + "(" + _Rank + ")";
        }
        public static float CalculateRankChange(Player _Player, PlayerHistory _PlayerHistory)
        {
            DateTime beforeLastRankUpdate = CalculateLastRankUpdadeDateUTC(_Player.LastSeen).AddHours(-1.0);
            var lastweekHonorData = _PlayerHistory.GetHonorItemAtTime(beforeLastRankUpdate).Data;
            if (lastweekHonorData != null)
            {
                return _Player.GetRankTotal() - lastweekHonorData.GetRankTotal();
            }
            else
                return _Player.GetRankTotal();
        }

        internal static int CalculateHKChange(Player _Player, PlayerHistory _PlayerHistory)
        {
            DateTime beforeLastRankUpdate = CalculateLastRankUpdadeDateUTC(_Player.LastSeen).AddHours(-1.0);
            var lastweekHonorEntry = _PlayerHistory.GetHonorItemAtTime(beforeLastRankUpdate);

            var lastweekHonorData = lastweekHonorEntry.Data;
            if (lastweekHonorEntry.Uploader.GetTime() < CalculateLastRankUpdadeDateUTC(_Player.LastSeen).AddDays(-6.5))
                return -1;

            return (_Player.Honor.LifetimeHK - _Player.Honor.ThisWeekHK) - (lastweekHonorData.LifetimeHK - lastweekHonorData.ThisWeekHK);
        }
    }
}