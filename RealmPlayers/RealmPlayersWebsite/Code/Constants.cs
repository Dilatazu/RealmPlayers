using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealmPlayersServer
{
    public class Constants
    {
        public static bool DebugMode = false;
        public static string RPPDbDir = "R:\\VF_RealmPlayersData\\RPPDatabase\\";
        public static string RPPDbWriteDir = "R:\\VF_RealmPlayersData\\RPPDatabase\\";

        private static bool m_Initialized = false;
        public static void AssertInitialize()
        {
            if (m_Initialized == false)
            {
                if (DebugMode == true)
                {
                    RPPDbWriteDir = "R:\\VF_RealmPlayersData\\RPPDatabaseDebug\\";
                }
                if (System.IO.Directory.Exists(RPPDbDir) == false)
                {
                    RPPDbDir = RPPDbDir.Replace("R:", "M:");
                    RPPDbWriteDir = RPPDbWriteDir.Replace("R:", "M:");
                }
                m_Initialized = true;
            }
        }
    }
}