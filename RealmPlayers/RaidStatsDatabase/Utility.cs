using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VF_RaidDamageDatabase
{
    public class Utility// : VF_RealmPlayersDatabase.Utility
    {
        public static string GetMethodAndLineNumber()
        {
            System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);

            string method = stackFrame.GetMethod().ToString();
            int line = stackFrame.GetFileLineNumber();

            return method + ":" + line;
        }
    }
}
