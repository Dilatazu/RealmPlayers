using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RealmPlayersDB
{
    private static VF.MongoDatabase sm_GlobalInstance = null;
    private static object sm_ThreadObject = new object();
    public static VF.MongoDatabase GetInstance(bool _DebugMode = false)
    {
        if (sm_GlobalInstance == null)
        {
            lock (sm_ThreadObject)
            {
                if (sm_GlobalInstance == null)
                {
                    if (_DebugMode == true)
                    {
                        sm_GlobalInstance = new VF.MongoDatabase("192.168.1.115", "RealmPlayersDB");
                    }
                    else
                    {
                        sm_GlobalInstance = new VF.MongoDatabase("127.0.0.1", "RealmPlayersDB");
                    }
                }
            }
        }
        return sm_GlobalInstance;
    }
}