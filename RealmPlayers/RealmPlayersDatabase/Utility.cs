using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace VF_RealmPlayersDatabase
{
    public class Utility : VF.Utility
    {
        public static string GetRealmPreString(WowRealm _Realm)
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

        public static string DefaultServerLocation = "R:\\";
        public static string DefaultDebugLocation = "D:\\";

        public static bool LoadSerialize<T_Data>(string _Filename, out T_Data _LoadedData)
        {
            using (var file = File.OpenRead(_Filename))// + ".ProtoBuf"
            {
                _LoadedData = Serializer.Deserialize<T_Data>(file);
            }
            return true;
        }
    }
}
