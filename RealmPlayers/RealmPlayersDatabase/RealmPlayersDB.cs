using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RealmPlayersDB
{
    private static VF.MongoDatabase sm_GlobalInstance = null;
    private static object sm_ThreadObject = new object();
    public static VF.MongoDatabase GetInstance()
    {
        if (sm_GlobalInstance == null)
        {
            lock (sm_ThreadObject)
            {
                if (sm_GlobalInstance == null)
                {
                    sm_GlobalInstance = new VF.MongoDatabase("127.0.0.1", "RealmPlayersDB");
                }
            }
        }
        return sm_GlobalInstance;
    }
}