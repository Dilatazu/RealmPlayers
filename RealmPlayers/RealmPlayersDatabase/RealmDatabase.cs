using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Npgsql;
using NpgsqlTypes;

namespace VF_RealmPlayersDatabase
{
    public class RealmDatabase
    {
        public enum LoadStatus
        {
            CurrentlyLoading = 1,
            PlayersLoaded = 2,
            PlayersHistoryLoaded = 3,
            PlayersExtraDataLoaded = 4,
            EverythingLoaded = 5,
            Load_Failed = 6,
        }
        public WowRealm Realm = WowRealm.Unknown;
        public WowVersionEnum WowVersion = WowVersionEnum.Unknown;
        public Dictionary<string, PlayerData.Player> m_Players = new Dictionary<string, PlayerData.Player>();
        RealmDatabaseHistory m_History = new RealmDatabaseHistory();
        //public Dictionary<string, PlayerData.AchievementData> m_PlayersAchievements = new Dictionary<string, PlayerData.AchievementData>();
        public Dictionary<string, PlayerData.ExtraData> m_PlayersExtraData = new Dictionary<string, PlayerData.ExtraData>();

        public bool Updated = false;
        //private GeneratedData.RealmCacheDatabase m_CacheDatabase = null;
        private System.Threading.Thread m_LoaderThread = null;
        private volatile LoadStatus m_LoadStatus = LoadStatus.EverythingLoaded;
        private object m_LockObj = new object();

        public bool WaitForLoad(LoadStatus _LoadStatus)
        {
            var loadStatus = m_LoadStatus;
            if (loadStatus < _LoadStatus)
            {
                lock(m_LockObj)
                {
                    loadStatus = m_LoadStatus;
                    while (loadStatus < _LoadStatus)
                    {
                        Monitor.Exit(m_LockObj);
                        System.Threading.Thread.Sleep(1000);
                        Monitor.Enter(m_LockObj);
                        loadStatus = m_LoadStatus;
                    }
                }
            }
            if (_LoadStatus == LoadStatus.Load_Failed)
                return false;
            return true;
        }
        public Dictionary<string, PlayerData.Player> Players
        {
            get 
            {
                WaitForLoad(LoadStatus.PlayersLoaded);
                return m_Players;
            }
        }
        public Dictionary<string, PlayerData.PlayerHistory> PlayersHistory
        {
            get
            {
                WaitForLoad(LoadStatus.PlayersHistoryLoaded);
                return m_History.m_PlayersHistory;
            }
        }
        public Dictionary<string, PlayerData.ExtraData> PlayersExtraData
        {
            get
            {
                WaitForLoad(LoadStatus.PlayersExtraDataLoaded);
                return m_PlayersExtraData;
            }
        }

        public RealmDatabase(WowRealm _Realm)
        {
            Realm = _Realm;
            WowVersion = StaticValues.GetWowVersion(_Realm);
            Updated = false;
        }
        public PlayerData.Player GetPlayer(string _Name)
        {
            if (Players.ContainsKey(_Name) == false)
                Players.Add(_Name, new PlayerData.Player(_Name, Realm));
            return Players[_Name];
        }
        public PlayerData.Player FindPlayer(string _Name)
        {
            if (Players.ContainsKey(_Name) == false)
                return null;
            return Players[_Name];
        }
        public bool PlayerExist(string _Name)
        {
            return FindPlayer(_Name) != null;
        }
        public bool FindPlayerAtTime(string _Name, DateTime _DateTime, out PlayerData.Player _RetPlayer)
        {
            _RetPlayer = null;
            try
            {
                PlayerData.PlayerHistory playerHistory;
                if (PlayersHistory.TryGetValue(_Name, out playerHistory))
                    return playerHistory.GetPlayerAtTime(_Name, Realm, _DateTime, out _RetPlayer);
                return false;
            }
            catch (Exception)
            {}
            return false;
        }
        public List<PlayerData.Player> SearchPlayer(string _PartOfName)
        {
            List<PlayerData.Player> playerList = new List<PlayerData.Player>();
            string searchStr = _PartOfName.ToLower();

            var values = Players.Where((KeyValuePair<string, PlayerData.Player> _NamePlayer) 
                => {
                    if (_NamePlayer.Key.ToLower().Contains(searchStr) == true)
                        return true;
                    return false;
                });
            foreach (var value in values)
            {
                playerList.Add(value.Value);
            }
            return playerList;
        }
        public PlayerData.PlayerHistory GetPlayerHistory(string _Name)
        {
            if (PlayersHistory.ContainsKey(_Name) == false)
                PlayersHistory.Add(_Name, new PlayerData.PlayerHistory());
            return PlayersHistory[_Name];
        }
        public PlayerData.PlayerHistory FindPlayerHistory(string _Name)
        {
            if (PlayersHistory.ContainsKey(_Name) == false)
                return null;
            return PlayersHistory[_Name];
        }
        public PlayerData.ExtraData GetPlayerExtraData(string _Name)
        {
            if (PlayersExtraData.ContainsKey(_Name) == false)
                PlayersExtraData.Add(_Name, new PlayerData.ExtraData());
            return PlayersExtraData[_Name];
        }
        public PlayerData.ExtraData FindPlayerExtraData(string _Name)
        {
            if (PlayersExtraData.ContainsKey(_Name) == false)
                return null;
            return PlayersExtraData[_Name];
        }
        static DateTime DATE_HONOR_CORRUPTION = new DateTime(2014, 1, 17, 10, 0, 0);
        public void UpdatePlayer(System.Xml.XmlNode _PlayerNode, Contributor _Contributor, Func<int, VF.SQLUploadID> _GetSQLUploadIDFunc)
        {
            string playerName = PlayerData.DataParser.ParsePlayerName(_PlayerNode);
            DateTime lastSeen = PlayerData.DataParser.ParseLastSeenUTC(_PlayerNode);
            if (lastSeen > DateTime.UtcNow)
            {
                if (lastSeen > DateTime.UtcNow.AddMinutes(2))
                {
                    //Tillåt inte data som är från framtiden(wtf) flagga Contributor som opålitlig
                    _Contributor.SetWarningFlag(Contributor.WarningFlag.DataFromFuture);
                    return;
                }
                lastSeen = DateTime.UtcNow; //Om det är OK så sätter vi LastSeen till UtcNow istället.
            }
            if ((DateTime.UtcNow - lastSeen).TotalDays > 5) //Tillåt inte data som är äldre än 5 dagar
                return;
            //if (lastSeen < DATE_HONOR_CORRUPTION)// Honor corruption occured the 16th January, uncomment this line after 5 days have passed!
            //{
            //    Logger.ConsoleWriteLine("Someone submitted data that was during the Honor Corruption. Good thing i implemented this code!!!", ConsoleColor.Red);
            //    return;
            //}
            var uploadID = _Contributor.GetUploadID(lastSeen);

            PlayerData.Player currPlayer = GetPlayer(playerName);
            PlayerData.PlayerHistory currPlayerHistory = GetPlayerHistory(playerName);
            currPlayer.Update(_PlayerNode, uploadID, lastSeen, currPlayerHistory, WowVersion, _GetSQLUploadIDFunc);
            try 
	        {
                //ANVÄND INTE = tecken innuti savedvariables data!!!!!!!!! då buggar det ur totalt
                string extraData = XMLUtility.GetChildValue(_PlayerNode, "ExtraData", "");
                if (extraData != "")
                {
                    VF.SQLPlayerID playerID;
                    using (VF.SQLComm comm = new VF.SQLComm())
                    {
                        if (comm.GetPlayerID(Realm, playerName, out playerID) == false)
                        {
                            Logger.ConsoleWriteLine("Could not find SQL PlayerID for Player \"" + playerName + "\"");
                        }
                    }
                    var currPlayerExtraData = GetPlayerExtraData(playerName);
                    currPlayerExtraData.AddData(uploadID, extraData, playerID, _GetSQLUploadIDFunc);
                }
	        }
            catch(NpgsqlException ex)
            {
                throw ex;
            }
	        catch (Exception ex)
	        {
                Logger.LogException(ex);
	        }
            Updated = true;
        }
        public void RemoveContributions(Contributor _Contributor)
        {
            foreach (var playerData in Players)
            {
                PlayersHistory[playerData.Key].RollbackPlayer(playerData.Value, _Contributor);
            }
        }
        public void PurgeGearContribution(string _Character, UploadID _UploadID)
        {
            try
            {
                var playerHistory = PlayersHistory[_Character];
                var removeGearIndex = playerHistory.GearHistory.FindIndex((_Value) => _Value.Uploader.GetContributorID() == _UploadID.GetContributorID() && _Value.Uploader.GetTime() == _UploadID.GetTime());

                if (removeGearIndex >= 0)
                {
                    playerHistory.GearHistory.RemoveAt(removeGearIndex);
                    Logger.ConsoleWriteLine("Successfully removed Gear for Character: " + _Character, ConsoleColor.Cyan);
                    Updated = true;
                }
            }
            catch (Exception)
            { }
        }
        public void PurgeExtraDataBefore(string _Character, DateTime _PurgeDate)
        {
            var extraData = m_PlayersExtraData[_Character];
            if (extraData.PurgeBefore(_PurgeDate) == true)
                Updated = true;
        }
        public void Rollback(DateTime _DateTime)
        {
            foreach (var playerData in Players)
            {
                if (playerData.Value.LastSeen > _DateTime)
                {
                    PlayersHistory[playerData.Key].RollbackPlayer(playerData.Value, _DateTime);
                }
            }
        }
        public void RemoveUnknowns()
        {
            Players.Remove("Unknown");
            PlayersHistory.Remove("Unknown");
        }
        public void RemoveGMs()
        {
            List<string> removePlayers = new List<string>();
            foreach (var player in Players)
            {
                var headItem = player.Value.Gear.GetItem(ItemSlot.Head);
                var chestItem = player.Value.Gear.GetItem(ItemSlot.Chest);
                var feetItem = player.Value.Gear.GetItem(ItemSlot.Feet);
                if ((headItem != null && headItem.ItemID == 12064) //GM Hood
                || (chestItem != null && chestItem.ItemID == 2586) //GM Chest
                || (feetItem != null && feetItem.ItemID == 11508)) //GM Feet
                    removePlayers.Add(player.Key);
            }
            foreach (var player in removePlayers)
            {
                Players.Remove(player);
                PlayersHistory.Remove(player);
            }
        }
        public static Dictionary<string, PlayerData.PlayerHistory> _LoadPlayersHistoryChunked(string _RealmPath, WowRealm _Realm, DateTime _HistoryEarliestDateTime)
        {
            Dictionary<string, PlayerData.PlayerHistory> loadedPlayersHistory = new Dictionary<string, PlayerData.PlayerHistory>();
            DateTime dateToLoad = DateTime.UtcNow;
            dateToLoad = dateToLoad.AddDays(15 - dateToLoad.Day); //Så att dagen ställs in på ungefär mitten av månaden för säkerhetsskull

            {////BORDE BRYTAS UT TILL EN FUNKTION???
                Utility.LoadSerialize<Dictionary<string, PlayerData.PlayerHistory>>
                    (_RealmPath + "\\PlayersHistoryData_Now.dat", out loadedPlayersHistory);
            }////BORDE BRYTAS UT TILL EN FUNKTION???
            while (dateToLoad >= _HistoryEarliestDateTime)
            {
                if (System.IO.File.Exists(_RealmPath + "\\PlayersHistoryData_" + dateToLoad.ToString("yyyy_MM") + ".dat") == true)
                {////BORDE BRYTAS UT TILL EN FUNKTION???
                    GC.Collect();
                    Dictionary<string, PlayerData.PlayerHistory> extraPlayerHistory = null;
                    Utility.LoadSerialize<Dictionary<string, PlayerData.PlayerHistory>>
                        (_RealmPath + "\\PlayersHistoryData_" + dateToLoad.ToString("yyyy_MM") + ".dat", out extraPlayerHistory);

                    foreach (var playerHistory in loadedPlayersHistory)
                    {
                        PlayerData.PlayerHistory oldHistory = null;
                        if (extraPlayerHistory.TryGetValue(playerHistory.Key, out oldHistory))
                        {
                            playerHistory.Value.AddOldHistory(oldHistory);
                            extraPlayerHistory.Remove(playerHistory.Key);
                        }
                    }
                    foreach (var playerHistory in extraPlayerHistory)
                    {
                        loadedPlayersHistory.Add(playerHistory.Key, playerHistory.Value);
                    }
                    Logger.ConsoleWriteLine("Loaded \"PlayersHistoryData_" + dateToLoad.ToString("yyyy_MM") + ".dat\" for Database " + _Realm.ToString(), ConsoleColor.White);
                }////BORDE BRYTAS UT TILL EN FUNKTION???
                dateToLoad = dateToLoad.AddMonths(-1);
            }

            if (_HistoryEarliestDateTime.Year <= 2012 && System.IO.File.Exists(_RealmPath + "\\PlayersHistoryData_ManuallyAdded.dat") == true)
            {////BORDE BRYTAS UT TILL EN FUNKTION???
                Dictionary<string, PlayerData.PlayerHistory> extraPlayerHistory = null;
                Utility.LoadSerialize<Dictionary<string, PlayerData.PlayerHistory>>
                    (_RealmPath + "\\PlayersHistoryData_ManuallyAdded.dat", out extraPlayerHistory);

                foreach (var playerHistory in loadedPlayersHistory)
                {
                    PlayerData.PlayerHistory oldHistory = null;
                    if (extraPlayerHistory.TryGetValue(playerHistory.Key, out oldHistory))
                    {
                        playerHistory.Value.AddOldHistory(oldHistory);
                        extraPlayerHistory.Remove(playerHistory.Key);
                    }
                }
                foreach (var playerHistory in extraPlayerHistory)
                {
                    loadedPlayersHistory.Add(playerHistory.Key, playerHistory.Value);
                }
                Logger.ConsoleWriteLine("Loaded \"PlayersHistoryData_ManuallyAdded.dat\" for Database " + _Realm.ToString(), ConsoleColor.White);
            }////BORDE BRYTAS UT TILL EN FUNKTION???

            return loadedPlayersHistory;
        }
        private DateTime m_LoadedDBFileDateTime = DateTime.MinValue;
        public bool IsDBFileUpdated(string _RealmPath)
        {
            if (System.IO.File.Exists(_RealmPath + "\\PlayersData.dat") == true)
            {
                if (System.IO.File.GetLastWriteTimeUtc(_RealmPath + "\\PlayersData.dat") > m_LoadedDBFileDateTime)
                    return true;
                else
                    return false;
            }
            return true;
        }
        private void Thread_LoadDatabase(string _RealmPath, DateTime? _HistoryEarliestDateTime = null)
        {
            try
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();
                Logger.ConsoleWriteLine("Started Loading Database " + Realm.ToString(), ConsoleColor.Green);
                if (System.IO.File.Exists(_RealmPath + "\\PlayersData.dat") == true)
                {
                    m_LoadedDBFileDateTime = System.IO.File.GetLastWriteTimeUtc(_RealmPath + "\\PlayersData.dat");
                    Dictionary<string, PlayerData.Player> loadedPlayers = null;
                    Utility.LoadSerialize<Dictionary<string, PlayerData.Player>>(_RealmPath + "\\PlayersData.dat", out loadedPlayers);

                    lock(m_LockObj)
                    {
                        m_Players = loadedPlayers;
                        m_LoadStatus = LoadStatus.PlayersLoaded;
                    }
                    Logger.ConsoleWriteLine("Loaded \"PlayersData.dat\" for Database " + Realm.ToString(), ConsoleColor.White);
                }
                if (System.IO.File.Exists(_RealmPath + "\\PlayersHistoryData_Now.dat") == true)
                {
                    DateTime loadDate = DateTime.UtcNow;// new DateTime(2013, 5, 5, 1, 1, 1);
                    if (_HistoryEarliestDateTime != null)
                        loadDate = _HistoryEarliestDateTime.Value;

                    Dictionary<string, PlayerData.PlayerHistory> loadedPlayersHistory = null;
                    loadedPlayersHistory = _LoadPlayersHistoryChunked(_RealmPath, Realm, loadDate);

                    lock (m_LockObj)
                    {
                        m_History.m_PlayersHistory = loadedPlayersHistory;
                        m_LoadStatus = LoadStatus.PlayersHistoryLoaded;
                    }
                    Logger.ConsoleWriteLine("Loaded \"PlayersHistoryData_Now.dat\" for Database " + Realm.ToString(), ConsoleColor.White);
                }
                if (System.IO.File.Exists(_RealmPath + "\\PlayersExtraData.dat") == true)
                {
                    Dictionary<string, PlayerData.ExtraData> loadedPlayersExtraData = null;
                    VF.Utility.LoadSerialize<Dictionary<string, PlayerData.ExtraData>>(_RealmPath + "\\PlayersExtraData.dat", out loadedPlayersExtraData);
                    if (loadedPlayersExtraData == null)
                    {
                        loadedPlayersExtraData = new Dictionary<string, PlayerData.ExtraData>();
                    }

                    lock (m_LockObj)
                    {
                        m_PlayersExtraData = loadedPlayersExtraData;
                        m_LoadStatus = LoadStatus.PlayersExtraDataLoaded;
                    }
                    Logger.ConsoleWriteLine("Loaded \"PlayersExtraData.dat\" for Database " + Realm.ToString(), ConsoleColor.White);
                }

                lock (m_LockObj)
                {
                    m_LoadStatus = LoadStatus.EverythingLoaded;
                }
                m_LoaderThread = null;

                Logger.ConsoleWriteLine("Done with loading Database " + Realm.ToString() + ", it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                m_LoadStatus = LoadStatus.Load_Failed;
                Logger.LogException(ex);
                Logger.ConsoleWriteLine("EXCEPTION CAUSED FAILURE TO LOAD FOR REALM " + Realm.ToString());
            }
        }
        public void LoadDatabase(string _RealmPath, DateTime? _HistoryEarliestDateTime = null)
        {
            lock (m_LockObj)
            {
                if (m_LoadStatus == LoadStatus.EverythingLoaded)
                    m_LoadStatus = LoadStatus.CurrentlyLoading;
                else
                {
                    return;
                }
            }
            m_LoaderThread = new System.Threading.Thread(() => Thread_LoadDatabase(_RealmPath, _HistoryEarliestDateTime));
            m_LoaderThread.Start();
        }
        public bool IsPlayersLoadComplete()
        {
            return m_LoadStatus >= LoadStatus.PlayersLoaded;
        }
        public bool IsPlayersHistoryLoadComplete()
        {
            return m_LoadStatus >= LoadStatus.PlayersHistoryLoaded;
        }
        public bool IsPlayersExtraDataLoadComplete()
        {
            return m_LoadStatus >= LoadStatus.PlayersExtraDataLoaded;
        }
        public bool IsLoadComplete()
        {
            return m_LoadStatus == LoadStatus.EverythingLoaded;
        }
        private void _SaveDatabaseFile<T_Database>(string _Filename, T_Database _Database)
        {
            int saveTry = 0;
            while (saveTry < 10)
            {
                try
                {
                    Utility.SaveSerialize(_Filename, _Database);
                    saveTry = 10;
                    break;
                }
                catch (Exception)
                {
                    ++saveTry;
                    if (saveTry < 10)
                    {
                        Logger.ConsoleWriteLine("Failed to save \"" + _Filename + "\" try nr" + saveTry + "/10", ConsoleColor.Yellow);
                        System.Threading.Thread.Sleep(5000);
                    }
                    else
                        throw;
                }
            }
        }
        private void _VFSaveDatabaseFile<T_Database>(string _Filename, T_Database _Database)
        {
            int saveTry = 0;
            while (saveTry < 10)
            {
                try
                {
                    VF.Utility.SaveSerialize(_Filename, _Database);
                    saveTry = 10;
                    break;
                }
                catch (Exception)
                {
                    ++saveTry;
                    if (saveTry < 10)
                    {
                        Logger.ConsoleWriteLine("Failed to save \"" + _Filename + "\" try nr" + saveTry + "/10", ConsoleColor.Yellow);
                        System.Threading.Thread.Sleep(5000);
                    }
                    else
                        throw;
                }
            }
        }
        public void SaveDatabase(string _RealmPath, bool _ForceSave = false)
        {
            if (Updated == true || _ForceSave == true)
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();
                Logger.ConsoleWriteLine("Started Saving Database " + Realm.ToString(), ConsoleColor.Green);
                Utility.BackupFile(_RealmPath + "\\PlayersData.dat");
                _SaveDatabaseFile(_RealmPath + "\\PlayersData.dat", Players);
                lock(m_LockObj)
                { 
                    DateTime saveDateTime = new DateTime(2013, 5, 1, 0, 0, 0);
                    try
                    {
                        while (saveDateTime.AddMonths(1) < DateTime.UtcNow)
                        {
                            if (m_History.ExistsEarlierThan(saveDateTime.AddMonths(1)) == true)
                            {
                                if (m_History.ExistsEarlierThan(saveDateTime) == false)
                                {
                                    bool removeOldData = (DateTime.UtcNow - saveDateTime.AddMonths(1)).TotalDays > 7;
                                    if (removeOldData == true)
                                    {
                                        Dictionary<string, PlayerData.PlayerHistory> oldHistory = new Dictionary<string, PlayerData.PlayerHistory>();
                                        foreach (var playerHistory in PlayersHistory)
                                        {
                                            var oldPlayerHistory = playerHistory.Value.ExtractOldHistory(saveDateTime.AddMonths(1), removeOldData);
                                            oldHistory.Add(playerHistory.Key, oldPlayerHistory);
                                        }
                                    
                                        try
                                        {
                                            Utility.BackupFile(_RealmPath + "\\PlayersHistoryData_" + saveDateTime.ToString("yyyy_MM") + ".dat");
                                        }
                                        catch (Exception)
                                        {
                                            Logger.ConsoleWriteLine("Backup of PlayersHistoryData_****_**.dat failed", ConsoleColor.Red);
                                        }
                                        _SaveDatabaseFile(_RealmPath + "\\PlayersHistoryData_" + saveDateTime.ToString("yyyy_MM") + ".dat", oldHistory);
                                    }
                                }
                                else
                                    Logger.ConsoleWriteLine("Error occurred in history data saving", ConsoleColor.Red);
                            }
                            saveDateTime = saveDateTime.AddMonths(1);
                        }
                        try
                        {
                            Utility.BackupFile(_RealmPath + "\\PlayersHistoryData_Now.dat");
                        }
                        catch (Exception)
                        {
                            Logger.ConsoleWriteLine("Backup of PlayersHistoryData_Now.dat failed", ConsoleColor.Red);
                        }
                        _SaveDatabaseFile(_RealmPath + "\\PlayersHistoryData_Now.dat", PlayersHistory);
                    }
                    catch (Exception ex)
                    {
                        Logger.ConsoleWriteLine("Exception occurred during history data saving: " + ex.ToString(), ConsoleColor.Red);
                    }
                }
                Utility.BackupFile(_RealmPath + "\\PlayersExtraData.dat");
                _VFSaveDatabaseFile(_RealmPath + "\\PlayersExtraData.dat", PlayersExtraData);
                Updated = false;
                Logger.ConsoleWriteLine("Done with saving Database " + Realm.ToString() + ", it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
            }
        }
    }
}
