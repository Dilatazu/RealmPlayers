using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

using Utility = VF_RealmPlayersDatabase.Utility;

namespace VF_RaidDamageWebsite
{
    public class Logger
    {
        private static DateTime sm_LogStartTime = DateTime.Now;
        private static List<string> sm_Log = new List<string>();
        public static void Initialize()
        {
            VF_RealmPlayersDatabase.Logger.SetupExternalLog(ConsoleWriteLine, LogException);
        }
        public static void LogException(Exception _Ex)
        {
            ConsoleWriteLine(_Ex.ToString(), ConsoleColor.Red);
        }
        public static void ConsoleWriteLine(string _Message, System.Drawing.Color _Color)
        {
            string dateTimeNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string fullMessage = dateTimeNow + " : " + _Message;
            int colorARGB = _Color.ToArgb() & 0xFFFFFF;
            fullMessage = "<font color='#" + colorARGB.ToString("X6") + "'>" + fullMessage + "</font><br/>";
            Monitor.Enter(sm_Log);
            bool addedDuplicate = false;
            if (sm_Log.Count > 0)
            {
                try
                {
                    if (sm_Log.Last().EndsWith(fullMessage))
                    {
                        if (sm_Log.Last().StartsWith("<") == false)
                        {
                            int indexOfStar = sm_Log.Last().IndexOf('*');
                            string num = sm_Log.Last().Substring(0, indexOfStar);
                            num = "" + (int.Parse(num) + 1);
                            sm_Log[sm_Log.Count - 1] = num + sm_Log[sm_Log.Count - 1].Substring(indexOfStar);
                            addedDuplicate = true;
                        }
                        else
                        {
                            sm_Log[sm_Log.Count - 1] = "2* " + sm_Log[sm_Log.Count - 1];
                            addedDuplicate = true;
                        }
                    }
                }
                catch (Exception)
                { }
            }
            if (addedDuplicate == false)
            {
                sm_Log.Add(fullMessage);
            }
            Monitor.Exit(sm_Log);
        }
        public static void ConsoleWriteLine(string _Message, ConsoleColor _Color = ConsoleColor.White)
        {
            ConsoleWriteLine(_Message, System.Drawing.Color.FromName(_Color.ToString()));
        }
        public static List<string> GetCopyOfLog(int _Count)
        {
            List<string> retLog = new List<string>();
            Monitor.Enter(sm_Log);
            if (_Count > sm_Log.Count)
                _Count = sm_Log.Count;
            for (int i = sm_Log.Count - _Count; i < sm_Log.Count; ++i)
            {
                retLog.Add(sm_Log[i]);
            }
            Monitor.Exit(sm_Log);
            return retLog;
        }
        public static List<string> GetCopyOfLog()
        {
            Monitor.Enter(sm_Log);
            List<string> retLog = new List<string>(sm_Log);
            Monitor.Exit(sm_Log);
            return retLog;
        }

        public static void SaveToDisk()
        {
            try
            {
                var logCopy = GetCopyOfLog(999999);
                string logFilename = ApplicationInstance.g_RDDBDir + "VF_WebsiteLogs\\Log_" + sm_LogStartTime.ToString("yyyy_MM_dd_HH_mm_ss") + ".html";
                Utility.AssertFilePath(logFilename);
                System.IO.File.WriteAllLines(logFilename, sm_Log);
                Logger.ConsoleWriteLine("SaveToDisk(): Saved Logfile to disk!", ConsoleColor.Yellow);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
    }
}