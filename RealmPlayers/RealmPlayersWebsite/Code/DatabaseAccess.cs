using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using RPPDatabase = VF_RealmPlayersDatabase.Database;
using ItemDropDatabase = VF_RealmPlayersDatabase.ItemDropDatabase;
using RealmDatabase = VF_RealmPlayersDatabase.RealmDatabase;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using Guild = VF_RealmPlayersDatabase.GeneratedData.Guild;
using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

using PlayerData = VF_RealmPlayersDatabase.PlayerData;
using GeneratedData = VF_RealmPlayersDatabase.GeneratedData;

namespace RealmPlayersServer
{
    public enum NotLoadedDecision
    {
        RedirectAndWait,
        SpinWait,
        ReturnNull,
    }
    public class DatabaseAccess
    {
        //public static RealmDatabase GetRealmDB(System.Web.UI.Page _Page, WowRealm _Realm, string _RedirectSubRealmSection = "")
        //{
        //    var rppDatabase = ApplicationInstance.Instance.GetRPPDatabase(false);
        //    if (rppDatabase == null)
        //    {
        //        PageUtility.RedirectErrorLoading(_Page, StaticValues.ConvertRealmParam(_Realm) + _RedirectSubRealmSection);
        //        return null;
        //    } 
        //    return rppDatabase.GetRealm(_Realm);
        //}
        //public static PlayerData.PlayerHistory GetRealmPlayerHistory(System.Web.UI.Page _Page, WowRealm _Realm, string _Player)
        //{
        //    var realm = GetRealmDB(_Page, _Realm, "-history");
        //    if (realm.IsPlayersHistoryLoadComplete() == false)
        //    {
        //        PageUtility.RedirectErrorLoading(_Page, StaticValues.ConvertRealmParam(_Realm) + "-history");
        //        return null;
        //    }
        //    PlayerData.PlayerHistory playerHistory;
        //    if (realm.PlayersHistory.TryGetValue(_Player, out playerHistory) == false)
        //        return null;
        //    return playerHistory;
        //}
        //public static PlayerData.Player GetRealmPlayer(System.Web.UI.Page _Page, WowRealm _Realm, string _Player)
        //{
        //    var realm = GetRealmDB(_Page, _Realm, "-latest");
        //    if (realm.IsPlayersLoadComplete() == false)
        //    {
        //        PageUtility.RedirectErrorLoading(_Page, StaticValues.ConvertRealmParam(_Realm) + "-latest");
        //        return null;
        //    }
        //    PlayerData.Player player;
        //    if (realm.Players.TryGetValue(_Player, out player) == false)
        //        return null;
        //    return player;
        //}
        //public static Dictionary<int, List<Tuple<DateTime, string>>> GetRealmCacheDB_ItemsUsed(System.Web.UI.Page _Page, WowRealm _Realm)
        //{
        //    var realm = GetRealmDB(_Page, _Realm, "-itemsused");
        //    if (realm.IsLoadComplete() == false)
        //    {
        //        PageUtility.RedirectErrorLoading(_Page, StaticValues.ConvertRealmParam(_Realm) + "-itemsused");
        //        return null;
        //    }
        //}
        public static RealmDatabase FindRealmDB(System.Web.UI.Page _Page, WowRealm _Realm, NotLoadedDecision _Decision = NotLoadedDecision.SpinWait)
        {
            var rppDatabase = Hidden.ApplicationInstance.Instance.GetRPPDatabase(_Decision == NotLoadedDecision.SpinWait);
            if (rppDatabase == null)
            {
                if (_Decision == NotLoadedDecision.RedirectAndWait)
                    PageUtility.RedirectErrorLoading(_Page, StaticValues.ConvertRealmParam(_Realm));
                return null;
            }
            return rppDatabase.GetRealm(_Realm);
        }
        public static Dictionary<WowRealm, RealmDatabase> GetRealmDBs(System.Web.UI.Page _Page)
        {
            return Hidden.ApplicationInstance.Instance.GetRPPDatabase(true).GetRealms();
        }
        public static PlayerData.PlayerHistory FindRealmPlayerHistory(System.Web.UI.Page _Page, WowRealm _Realm, string _Player)
        {
            var realm = FindRealmDB(_Page, _Realm);
            if (realm == null/* || realm.IsPlayersHistoryLoadComplete() == false*/)
                return null;
            PlayerData.PlayerHistory playerHistory;
            if (realm.PlayersHistory.TryGetValue(_Player, out playerHistory) == false)
                return null;
            return playerHistory;
        }
        public static PlayerData.ExtraData FindRealmPlayerExtraData(System.Web.UI.Page _Page, WowRealm _Realm, string _Player, NotLoadedDecision _Decision = NotLoadedDecision.ReturnNull)
        {
            var realm = FindRealmDB(_Page, _Realm);
            if (realm == null || (realm.IsPlayersExtraDataLoadComplete() == false && _Decision != NotLoadedDecision.SpinWait))
                return null;
            PlayerData.ExtraData playerExtraData;
            if (realm.PlayersExtraData.TryGetValue(_Player, out playerExtraData) == false)
                return null;
            return playerExtraData;
        }
        public static Dictionary<string, PlayerData.PlayerHistory> TryGetRealmPlayersHistory(System.Web.UI.Page _Page, WowRealm _Realm)
        {
            var realm = FindRealmDB(_Page, _Realm);
            if (realm == null || realm.IsPlayersHistoryLoadComplete() == false)
                return null;
            return realm.PlayersHistory;
        }
        public static Dictionary<string, PlayerData.PlayerHistory> GetRealmPlayersHistory(System.Web.UI.Page _Page, WowRealm _Realm, NotLoadedDecision _Decision = NotLoadedDecision.RedirectAndWait)
        {
            var realm = FindRealmDB(_Page, _Realm);
            if (realm == null || (realm.IsPlayersHistoryLoadComplete() == false && _Decision != NotLoadedDecision.SpinWait))
            {
                if(_Decision == NotLoadedDecision.RedirectAndWait)
                    PageUtility.RedirectErrorLoading(_Page, StaticValues.ConvertRealmParam(_Realm) + "-history");
                return null;
            }
            return realm.PlayersHistory;
        }
        public static Dictionary<string, PlayerData.Player> GetRealmPlayers(System.Web.UI.Page _Page, WowRealm _Realm, NotLoadedDecision _Decision = NotLoadedDecision.SpinWait)
        {
            var realm = FindRealmDB(_Page, _Realm, _Decision);
            if (realm == null || (realm.IsPlayersLoadComplete() == false && _Decision != NotLoadedDecision.SpinWait))
            {
                if (_Decision == NotLoadedDecision.RedirectAndWait)
                    PageUtility.RedirectErrorLoading(_Page, StaticValues.ConvertRealmParam(_Realm));
                return null;
            }
            return realm.Players;
        }
        public static PlayerData.Player FindRealmPlayer(System.Web.UI.Page _Page, WowRealm _Realm, string _Player, NotLoadedDecision _Decision = NotLoadedDecision.SpinWait)
        {
            var realmPlayers = GetRealmPlayers(_Page, _Realm, _Decision);
            if (realmPlayers == null)
                return null;
            PlayerData.Player player;
            if (realmPlayers.TryGetValue(_Player, out player) == false)
                return null;
            return player;
        }
        //public static Dictionary<int, List<Tuple<DateTime, string>>> GetRealmItemsUsed(System.Web.UI.Page _Page, WowRealm _Realm)
        //{
        //    var realm = FindRealmDB(_Page, _Realm);
        //    if (realm == null || realm.IsLoadComplete() == false)
        //        return new Dictionary<int, List<Tuple<DateTime, string>>>();
        //    var cacheDatabase = realm.GetCacheDatabase(false, true);
        //    if (cacheDatabase != null)
        //    {
        //        if (cacheDatabase.GetItemsUsed().Count == 0)
        //            cacheDatabase.GenerateItemsUsed();
        //        return cacheDatabase.GetItemsUsed();
        //    }
        //    else
        //        return new Dictionary<int, List<Tuple<DateTime, string>>>();
        //}
        //public static IEnumerable<KeyValuePair<string, Guild>> GetRealmGuilds(System.Web.UI.Page _Page, WowRealm _Realm, NotLoadedDecision _Decision = NotLoadedDecision.SpinWait)
        //{
        //    var realm = FindRealmDB(_Page, _Realm, _Decision);
        //    if (realm == null || (realm.IsLoadComplete() == false && _Decision != NotLoadedDecision.SpinWait))
        //    {
        //        if (_Decision == NotLoadedDecision.RedirectAndWait)
        //            PageUtility.RedirectErrorLoading(_Page, StaticValues.ConvertRealmParam(_Realm) + "-guilds");
        //        return null;
        //    }
        //    var cacheDatabase = realm.GetCacheDatabase(false, _Decision != NotLoadedDecision.SpinWait);
        //    if (cacheDatabase != null)
        //    {
        //        realm.GetCacheDatabase().GenerateGuilds();
        //        return realm.GetCacheDatabase().GetGuilds();
        //    }
        //    else
        //        return new List<KeyValuePair<string, Guild>>();
        //}
        //public static Guild GetRealmGuild(System.Web.UI.Page _Page, WowRealm _Realm, string _Guild, NotLoadedDecision _Decision = NotLoadedDecision.SpinWait)
        //{
        //    var realm = FindRealmDB(_Page, _Realm, _Decision);
        //    if (realm == null || (realm.IsLoadComplete() == false && _Decision != NotLoadedDecision.SpinWait))
        //    {
        //        if (_Decision == NotLoadedDecision.RedirectAndWait)
        //            PageUtility.RedirectErrorLoading(_Page, StaticValues.ConvertRealmParam(_Realm) + "-guilds");
        //        return null;
        //    }
        //    var cacheDatabase = realm.GetCacheDatabase();
        //    if (cacheDatabase != null)
        //    {
        //        return realm.GetCacheDatabase().GetGuild(_Guild);
        //    }
        //    else
        //        return new Guild(_Guild, realm);
        //}
        public static string GetCurrentItemDatabaseAddress()
        {
            return Hidden.ApplicationInstance.Instance.GetCurrentItemDatabaseAddress();
        }
        public static ItemInfo GetItemInfo(int _ItemID, WowVersionEnum _WowVersion)
        {
            return Hidden.ApplicationInstance.Instance.GetItemInfo(_ItemID, _WowVersion);
        }
        public static Code.ContributorStatistics GetContributorStatistics()
        {
            Hidden.ApplicationInstance.Instance.GetRPPDatabase(true);
            return Hidden.ApplicationInstance.Instance.GetContributorStatistics();
        }
        public static VF_RealmPlayersDatabase.ItemDropDatabase GetItemDropDatabase(System.Web.UI.Page _Page, WowVersionEnum _WowVersion, NotLoadedDecision _Decision = NotLoadedDecision.SpinWait)
        {
            Hidden.ApplicationInstance.Instance.GetItemInfoCache(_WowVersion, _Decision == NotLoadedDecision.SpinWait);
            var itemDropDatabase = Hidden.ApplicationInstance.Instance.GetItemDropDatabase();
            if (itemDropDatabase == null && _Decision == NotLoadedDecision.RedirectAndWait)
                PageUtility.RedirectErrorLoading(_Page, "itemdropdatabase");
            return itemDropDatabase;
        }
    }
}