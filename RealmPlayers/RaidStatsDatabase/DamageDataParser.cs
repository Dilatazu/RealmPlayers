using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * This file is the initial parsing of lua phase for RaidStats data
 * ParseFile is called with savedvariables lua filename and this in turns reads all the data from the lua file
 * and starts to parse it in the _ParseData function.
 * The parsing generates a list of DamageDataSessions, one for each player session that was found within the lua database.
 * The DamageDataSession is time synchronized to the server clock to ensure a precision of 1-2 minutes.
 * Each DamageDataSession has timeslices with all the information and addition meta data such as conversion tables for ID to names.
 */

using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

namespace VF_RaidDamageDatabase
{
    public class DamageDataParser
    {
        public static List<DamageDataSession> ParseFile(string _Filename, ref List<string> _SessionsDebugData, WowVersionEnum _WowVersion = WowVersionEnum.Unknown)
        {
            if (_SessionsDebugData == null)
                _SessionsDebugData = new List<string>();
            else
                _SessionsDebugData.Add("------------\"" + _Filename + "\"------------");
            string fullFile = System.IO.File.ReadAllText(_Filename);
            string[] data = fullFile.Split(new string[] { "\r\nVF_" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < data.Length; ++i)
            {
                string fixedData = data[i];
                fixedData = fixedData.Substring(fixedData.IndexOf('{') + 1);
                fixedData = fixedData.Substring(0, fixedData.LastIndexOf("\r\n}"));
                if (data[i].StartsWith("RaidDamageData") && (_WowVersion == WowVersionEnum.Unknown || _WowVersion == WowVersionEnum.Vanilla))
                {
                    return _ParseData(fixedData, ref _SessionsDebugData, WowVersionEnum.Vanilla);
                }
                else if (data[i].StartsWith("RaidStatsData") && (_WowVersion == WowVersionEnum.Unknown || _WowVersion == WowVersionEnum.TBC))
                {
                    return _ParseData(fixedData, ref _SessionsDebugData, WowVersionEnum.TBC);
                }
            }
            return new List<DamageDataSession>();
        }
        public static List<DamageDataSession> _ParseData(string _Data, ref List<string> _SessionDebugData, WowVersionEnum _WowVersion)
        {
            List<DamageDataSession> damageDataSessions = new List<DamageDataSession>();
            if (_SessionDebugData == null)
                _SessionDebugData = new List<string>();
            else
                _SessionDebugData.Add("------------NEW SESSIONS PARSEDATA------------");
            /*if (_Data.Contains("Highlord Mograine"))
            {
                Console.WriteLine("Detected 4HM, replacing Highlord Mograine with The Four Horsemen!");
                _Data = _Data.Replace("Highlord Mograine", "The Four Horsemen");
                _Data = _Data.Replace("Dead_Y=The Four Horsemen", "");
                _Data = _Data.Replace("Dead_T=The Four Horsemen", "");
                _Data = _Data.Replace("Dead_C=The Four Horsemen", "");
                _Data = _Data.Replace("Dead_S=The Four Horsemen", "");
                //Assume there was no deaths
            }*/

            string sessionSplitStr = null;
            string dataSplitStr = null;
            if(_WowVersion == WowVersionEnum.TBC)
            {
                sessionSplitStr = "\r\n\t{";
                dataSplitStr = "\r\n\t\t\"";
            }
            else
            {
                sessionSplitStr = "= {";
                dataSplitStr = "= \"";
            }


            string[] dataSessions = _Data.Split(new string[] { sessionSplitStr }, StringSplitOptions.None);
            for (int i = dataSessions.Length - 1; i >= 0; --i)
            {
                if (dataSessions[i].Length > 100)
                {
                    bool TimeSynch_UseServerTime = false;
                    string[] currSessionData = dataSessions[i].Split(new string[] { dataSplitStr }, StringSplitOptions.None);
                    //Logger.ConsoleWriteLine("Started session with " + currSessionData.Length + " timeSlices", ConsoleColor.Green);

                    DamageDataSession newSession = new DamageDataSession();
                    List<int> raidMemberIDs = new List<int>();
                    string currZone = "Unknown";

                    TimeSlice lastTimeSlice = null;
                    if (currSessionData.Length > 100)
                    {
                        Logger.ConsoleWriteLine("Parsing " + currSessionData.Length + " timeSlices.", ConsoleColor.White);
                    }
                    DateTime startParseDateTime = DateTime.UtcNow;
                    for (int u = currSessionData.Length - 1; u >= 0; --u)
                    {
                        string currTimeSlice = currSessionData[u];
                        if (currTimeSlice.Contains('\"'))
                        {
                            string timeSlice = currTimeSlice.Substring(0, currTimeSlice.LastIndexOf("\","));

                            if (timeSlice.StartsWith("Session:Info:") == true)
                            {//Session info data
                                string[] sessionInfoSplit = timeSlice.Split(new string[] { "Session:Info:", "," }, StringSplitOptions.RemoveEmptyEntries);
                                for (int o = 0; o < sessionInfoSplit.Length; ++o)
                                {
                                    string[] nameValue = sessionInfoSplit[o].Split('=');
                                    if (nameValue.Length == 2)
                                    {
                                        if (nameValue[0] == "Date")
                                        {
                                            DateTime parsedSessionDate;
                                            if (System.DateTime.TryParse(nameValue[1], System.Globalization.CultureInfo.InvariantCulture
                                                , System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out parsedSessionDate))
                                            {
                                                newSession.StartDateTime = parsedSessionDate;
                                            }
                                        }
                                        else if (nameValue[0] == "Time")
                                        {
                                            int newStartTime = 0;
                                            if(int.TryParse(nameValue[1], out newStartTime))
                                                newSession.StartTime = newStartTime;
                                        }
                                        else if (nameValue[0] == "ServerTime")
                                        {
                                            var hourMinute = nameValue[1].Split(':');
                                            if (hourMinute.Length >= 2)
                                            {
                                                int serverHour = 0;
                                                int serverMinute = 0;
                                                if (int.TryParse(hourMinute[0], out serverHour) == true)
                                                {
                                                    if (int.TryParse(hourMinute[1], out serverMinute) == true)
                                                    {
                                                        newSession.StartServerTime = serverHour * 3600 + serverMinute * 60;
                                                    }
                                                }
                                            }
                                        }
                                        else if (nameValue[0] == "Realm")
                                        {
                                            newSession.Realm = nameValue[1];
                                        }
                                        else if (nameValue[0] == "Player")
                                        {
                                            newSession.Player = nameValue[1];
                                        }
                                        else if (nameValue[0] == "Zone")
                                        {
                                            currZone = nameValue[1];
                                        }
                                        else if (nameValue[0] == "AddonVersion")
                                        {
                                            newSession.AddonVersion = nameValue[1];
                                            Logger.ConsoleWriteLine(newSession.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + " VF_RaidDamage " + newSession.AddonVersion, ConsoleColor.Cyan);
                                        }
                                        else if (nameValue[0].StartsWith("RaidID"))
                                        {
                                            string[] raid_Name_ID_Remain = nameValue[1].Split('-');
                                            string raidName = RaidID_RaidNameConvert(raid_Name_ID_Remain[0]);
                                            int raidID = -1;
                                            int raidRemaining = -1;

                                            if (int.TryParse(raid_Name_ID_Remain[1], out raidID) == false) raidID = -1;
                                            if (int.TryParse(raid_Name_ID_Remain[2], out raidRemaining) == false) raidRemaining = -1;

                                            if (newSession.RaidIDData.ContainsKey(raidName) == false)
                                            {
                                                if (lastTimeSlice != null)
                                                {
                                                    DamageDataSession.RaidIDEntry raidIDEntry = new DamageDataSession.RaidIDEntry();
                                                    raidIDEntry.RaidID = raidID;
                                                    raidIDEntry.RaidResetDate = newSession.StartDateTime.AddSeconds((lastTimeSlice.Time - newSession.StartTime) + raidRemaining);
                                                    raidIDEntry.LastSeen = newSession.StartDateTime.AddSeconds((lastTimeSlice.Time - newSession.StartTime));
                                                    newSession.RaidIDData.AddToList(raidName, raidIDEntry);
                                                }
                                            }
                                            else
                                            {
                                                if (lastTimeSlice != null)
                                                {
                                                    var raidIDData = newSession.RaidIDData[raidName].Last();
                                                    if (raidIDData.RaidID == raidID)
                                                    {
                                                        //RaidID allready exists, update LastSeen
                                                        raidIDData.LastSeen = newSession.StartDateTime.AddSeconds((lastTimeSlice.Time - newSession.StartTime));
                                                    }
                                                    else
                                                    {
                                                        //New RaidID while in the same session!
                                                        DamageDataSession.RaidIDEntry raidIDEntry = new DamageDataSession.RaidIDEntry();
                                                        raidIDEntry.RaidID = raidID;
                                                        raidIDEntry.RaidResetDate = newSession.StartDateTime.AddSeconds((lastTimeSlice.Time - newSession.StartTime) + raidRemaining);
                                                        raidIDEntry.LastSeen = newSession.StartDateTime.AddSeconds((lastTimeSlice.Time - newSession.StartTime));
                                                        newSession.RaidIDData.AddToList(raidName, raidIDEntry);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (timeSlice.StartsWith("Session:TimeSynch:") == true)
                            {
                                //TimeSynch data
                                DateTime timeSynchLocalDate = DateTime.MinValue;
                                int timeSynchLocalTime = -1;
                                int timeSynchServerTime = -1;
                                string[] sessionTimeSynchSplit = timeSlice.Split(new string[] { "Session:TimeSynch:", "," }, StringSplitOptions.RemoveEmptyEntries);
                                for (int o = 0; o < sessionTimeSynchSplit.Length; ++o)
                                {
                                    string[] nameValue = sessionTimeSynchSplit[o].Split('=');
                                    if (nameValue.Length == 2)
                                    {
                                        if (nameValue[0] == "Date")
                                        {
                                            DateTime parsedSessionDate;
                                            if (System.DateTime.TryParse(nameValue[1], System.Globalization.CultureInfo.InvariantCulture
                                                , System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out parsedSessionDate))
                                            {
                                                timeSynchLocalDate = parsedSessionDate;
                                            }
                                        }
                                        else if (nameValue[0] == "Time")
                                        {
                                            int newStartTime = 0;
                                            if (int.TryParse(nameValue[1], out newStartTime))
                                                timeSynchLocalTime = newStartTime;
                                        }
                                        else if (nameValue[0] == "ServerTime")
                                        {
                                            var hourMinute = nameValue[1].Split(':');
                                            if (hourMinute.Length >= 2)
                                            {
                                                int serverHour = 0;
                                                int serverMinute = 0;
                                                if (int.TryParse(hourMinute[0], out serverHour) == true)
                                                {
                                                    if (int.TryParse(hourMinute[1], out serverMinute) == true)
                                                    {
                                                        timeSynchServerTime = serverHour * 3600 + serverMinute * 60;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (timeSynchLocalDate != DateTime.MinValue && timeSynchLocalTime != -1 && timeSynchServerTime != -1)
                                {
                                    int localTimeSecondsPassed = (timeSynchLocalTime - newSession.StartTime);
                                    int localDateSecondsPassed = (int)((timeSynchLocalDate - newSession.StartDateTime).TotalSeconds);
                                    if (Math.Abs(localTimeSecondsPassed - localDateSecondsPassed) > 4)
                                    {
                                        Logger.ConsoleWriteLine("Potentially not accurate time keeping, localTimeDiff=" + localTimeSecondsPassed + ", localDateDiff=" + localDateSecondsPassed, ConsoleColor.Red);
                                    }
                                    newSession.StartServerTime = timeSynchServerTime - localTimeSecondsPassed;
                                    TimeSynch_UseServerTime = true;
                                }
                            }
                            else if (timeSlice.StartsWith("Session:Loot:") == true)
                            {
                                //Loot data

                                //Grab time so we know
                                DateTime currSliceDateTime = newSession.StartDateTime;
                                if (lastTimeSlice != null)
                                {
                                    currSliceDateTime = currSliceDateTime.AddSeconds(lastTimeSlice.Time - newSession.StartTime);
                                }
                                //Grab time so we know

                                string[] sessionLootSplit = timeSlice.Split(new string[] { "Session:Loot:", ";" }, StringSplitOptions.RemoveEmptyEntries);
                                for (int o = 0; o < sessionLootSplit.Length; ++o)
                                {
                                    string[] nameValue = sessionLootSplit[o].Split('=');
                                    if (nameValue[0].StartsWith("BL-"))
                                    {
                                        //Boss Loot
                                        string bossName = nameValue[0].Substring(3);
                                        var itemDrops = nameValue[1].Split(',');
                                        List<int> itemIDs = new List<int>();
                                        foreach (var itemDrop in itemDrops)
                                        {
                                            int itemID = 0;
                                            if (int.TryParse(itemDrop.Split(':').First(), out itemID) == true)
                                            {
                                                itemIDs.Add(itemID);
                                            }
                                        }
                                        if (itemIDs.Count > 0)
                                        {
                                            var bossLootIndex = newSession.BossLoot.FindIndex((_Value) => _Value.Item2 == bossName);
                                            if (bossLootIndex != -1)
                                            {
                                                newSession.BossLoot[bossLootIndex].Item3.AddRangeUnique(itemIDs);
                                            }
                                            else
                                            {
                                                newSession.BossLoot.Add(Tuple.Create(currSliceDateTime, bossName, itemIDs));
                                            }
                                        }
                                    }
                                    else if (nameValue[0].StartsWith("PL-"))
                                    {
                                        //Player Loot
                                        string playerName = nameValue[0].Substring(3);
                                        int itemID = 0;
                                        if (int.TryParse(nameValue[1].Split(':').First(), out itemID) == true)
                                        {
                                            newSession.PlayerLoot.Add(Tuple.Create(currSliceDateTime, playerName, itemID));
                                        }
                                    }
                                }
                            }
                            else if (timeSlice.StartsWith("Session:Debug:") == true)
                            {
                                //Debug data, ignorera
                                _SessionDebugData.Add(timeSlice);
                            }
                            else if(timeSlice.StartsWith("Session:") == false)
                            {//Timeslice data
                                TimeSlice newTimeSlice = new TimeSlice(lastTimeSlice, timeSlice, newSession.UnitIDToNames, raidMemberIDs, currZone, _WowVersion, newSession.BuffIDToNames, newSession.AddonVersion);
                                newSession.TimeSlices.Add(newTimeSlice);
                                lastTimeSlice = newTimeSlice;
                            }
                            else
                            {
                                Logger.ConsoleWriteLine("Unknown Session/TimeSlice Data: \"" + timeSlice + "\"", ConsoleColor.Red);
                            }
                        }
                        if (u % 50 == 49)
                        {
                            if ((DateTime.UtcNow - startParseDateTime).Seconds > 1)
                                Logger.ConsoleWriteLine("Parsed " + (currSessionData.Length - u) + " timeSlices", ConsoleColor.White);
                            else
                                Console.Write(".");
                        }
                    }
                    foreach (var raidMemberID in raidMemberIDs)
                    {
                        if(newSession.UnitIDToNames.ContainsKey(raidMemberID))
                            newSession.RaidMembers.Add(newSession.UnitIDToNames[raidMemberID]);
                    }

                    if (TimeSynch_UseServerTime == true && newSession.StartServerTime >= 0)
                    {
                        int serverTimeHours = (int)(newSession.StartServerTime / 3600);
                        int serverTimeMinutes = (int)((newSession.StartServerTime - serverTimeHours * 3600) / 60);
                        int serverTimeSeconds = newSession.StartServerTime - serverTimeHours * 3600 - serverTimeMinutes * 60;

                        int hourDiff = serverTimeHours - newSession.StartDateTime.Hour;
                        int minuteDiff = serverTimeMinutes - newSession.StartDateTime.Minute;
                        int secondDiff = serverTimeSeconds - newSession.StartDateTime.Second;

                        int addHour = -1;
                        if (TimeZoneInfo.Local.IsDaylightSavingTime(newSession.StartDateTime) == true)
                            addHour = -2;

                        hourDiff = hourDiff + addHour;

                        while (hourDiff > 12)
                            hourDiff -= 24;
                        while (hourDiff < -12)
                            hourDiff += 24;

                        string oldDateTimeUTC = newSession.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss");

                        newSession.StartDateTime = newSession.StartDateTime.AddHours(hourDiff).AddMinutes(minuteDiff).AddSeconds(secondDiff);

                        if(newSession.StartDateTime > DateTime.UtcNow && newSession.Realm == "Nostralia")
                        {
                            hourDiff -= 24;
                            newSession.StartDateTime = newSession.StartDateTime.AddHours(-24);
                        }

                        for (int qq = 0; qq < newSession.BossLoot.Count; ++qq)
                        {
                            var bossLootValue = newSession.BossLoot[qq];
                            newSession.BossLoot[qq] = new Tuple<DateTime,string,List<int>>(bossLootValue.Item1.AddHours(hourDiff).AddMinutes(minuteDiff).AddSeconds(secondDiff),bossLootValue.Item2, bossLootValue.Item3);
                        }
                        for (int qq = 0; qq < newSession.PlayerLoot.Count; ++qq)
                        {
                            var playerLootValue = newSession.PlayerLoot[qq];
                            newSession.PlayerLoot[qq] = new Tuple<DateTime, string, int>(playerLootValue.Item1.AddHours(hourDiff).AddMinutes(minuteDiff).AddSeconds(secondDiff), playerLootValue.Item2, playerLootValue.Item3);
                        }
                        Logger.ConsoleWriteLine("TimeSynched to ServerTime. old StartDateTimeUTC(" + oldDateTimeUTC + "), new StartDateTimeUTC(" + newSession.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.White);
                    }

                    if (newSession.StartDateTime < DateTime.UtcNow || newSession.Player == "IAmFromTheFuture")
                    {
                        damageDataSessions.Add(newSession);
                    }
                    else
                    {
                        Logger.ConsoleWriteLine("Session was discarded because it had a StartDateTime that was from the future! StartDateTime(" + newSession.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", ConsoleColor.Red);
                    }
                    if (currSessionData.Length > 100)
                    {
                        Logger.ConsoleWriteLine("Done with session", ConsoleColor.Green);
                    }
                    else
                        Console.Write(".");
                }
            }
            return damageDataSessions;
        }

        private static string RaidID_RaidNameConvert(string _RaidName)
        {
            if (_RaidName == "The Battle for Mount Hyjal")
                return "Hyjal Summit";
            else if (_RaidName == "Coilfang: Serpentshrine Cavern")
                return "Serpentshrine Cavern";
            else if (_RaidName == "The Sunwell")
                return "Sunwell Plateau";
            return _RaidName;
        }
    }
}
