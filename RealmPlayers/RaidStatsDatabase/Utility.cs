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

        public static string GetRealmPreString(VF_RealmPlayersDatabase.WowRealm _Realm)
        {
            string realm = "" + (int)_Realm;
            if (realm.Length > 1)
            {
                if ((int)_Realm > 99)
                    throw new Exception("ERROR, REALM WAS INTENDED TO NEVER BE BIGGER THAN VALUE 99");
                realm = "R" + (int)_Realm;
            }
            return realm;
        }
    }
}
