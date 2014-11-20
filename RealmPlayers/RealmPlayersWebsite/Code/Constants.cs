using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealmPlayersServer
{
    public class Constants
    {
        public static bool DebugMode = false;
        public static string RPPDbDir = "C:\\VF_RealmPlayersData\\RPPDatabase\\";
        public static string RPPDbWriteDir = "C:\\VF_RealmPlayersData\\RPPDatabase\\";

        private static bool m_Initialized = false;
        public static void AssertInitialize()
        {
            if (m_Initialized == false)
            {
                if (DebugMode == true)
                {
                    RPPDbWriteDir = "C:\\VF_RealmPlayersData\\RPPDatabaseDebug\\";
                }
                if (System.IO.Directory.Exists(RPPDbDir) == false)
                {
                    RPPDbDir = RPPDbDir.Replace("C:", "M:");
                    RPPDbWriteDir = RPPDbWriteDir.Replace("C:", "M:");
                }
                m_Initialized = true;
            }
        }
    }
}