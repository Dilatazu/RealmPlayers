using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_WoWLauncherServer
{
    public class Logger
    {
        //public static void LogMessage(string _Message)
        //{
        //    var logFile = System.IO.File.AppendText("ErrorLog.txt");
        //    logFile.WriteLine(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ": " + _Message + "\r\n\r\n");
        //    logFile.Close();
        //}
        private static Action<string, ConsoleColor> sm_ExternalLog_ConsoleWriteLine = null;
        private static Action<Exception> sm_ExternalLog_LogException = null;
        public static void SetupExternalLog(Action<string, ConsoleColor> _ConsoleWriteLine, Action<Exception> _LogException = null)
        {
            sm_ExternalLog_ConsoleWriteLine = _ConsoleWriteLine;
            sm_ExternalLog_LogException = _LogException;
        }

        public static System.Collections.Concurrent.ConcurrentQueue<string> sm_QueuedExtraLogs = new System.Collections.Concurrent.ConcurrentQueue<string>();
        public static void LogException(Exception _Ex)
        {
            if (sm_ExternalLog_LogException != null)
            {
                try
                {
                    sm_ExternalLog_LogException(_Ex);
                }
                catch (Exception)
                { }
            }
            else
            {
                ConsoleWriteLine("\r\n-------------------------------\r\n" + _Ex.ToString() + "\"\r\n-------------------------------", ConsoleColor.Red);
            }
        }
        public static void ConsoleWriteLine(string _Message, ConsoleColor _Color = ConsoleColor.White, bool _LogToFile = true)
        {
            if (sm_ExternalLog_ConsoleWriteLine != null)
            {
                try
                {
                    sm_ExternalLog_ConsoleWriteLine(_Message, _Color);
                }
                catch (Exception)
                { }
            }
            else
            {
                Console.ForegroundColor = _Color;
                string fullMessage = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " : " + _Message;
                Console.WriteLine(fullMessage.Substring("yyyy/MM/dd ".Length));
                if(_LogToFile == true)
                {
                    try
                    {
                        var logFile = System.IO.File.Open("Log.txt", System.IO.FileMode.Append, System.IO.FileAccess.Write);
                        if (logFile.Length > 1 * 1024 * 1024)
                        {
                            //Om loggfilen är större än 1MB, skapa en ny och gör gamla till backup
                            logFile.Close();
                            System.IO.File.Copy("Log.txt", "Log_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt");
                            string queuedLog = "";
                            while (sm_QueuedExtraLogs.TryDequeue(out queuedLog) == true)
                            {
                                fullMessage = queuedLog + "\r\n" + fullMessage;
                            }
                            System.IO.File.WriteAllText("Log.txt", fullMessage + "\r\n");
                        }
                        else
                        {
                            System.IO.StreamWriter fw = new System.IO.StreamWriter(logFile);
                            string queuedLog = "";
                            while (sm_QueuedExtraLogs.TryDequeue(out queuedLog) == true)
                            {
                                fw.WriteLine(queuedLog);
                            }
                            fw.WriteLine(fullMessage);
                            fw.Flush();
                            logFile.Close();
                        }
                    }
                    catch (Exception)
                    {
                        sm_QueuedExtraLogs.Enqueue(fullMessage);
                    }
                }
                Console.ResetColor();
            }
        }

    }
}
