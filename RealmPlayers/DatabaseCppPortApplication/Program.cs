using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;

namespace DatabaseCppPortApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //SerializeDB.Cpp_PlayerSummaryDatabase("PlayerSummaryDatabase.CRP");
            //SerializeDB.Cpp_GuildSummaryDatabase("ItemSummaryDatabase.CRP");
            //SerializeDB.Cpp_ItemSummaryDatabase("ItemSummaryDatabase.CRP");
            SerializeDB.Cpp_RealmDatabase(WowRealm.Archangel, "RealmDatabase_Archangel.CRP");
            SerializeDB.Cpp_RealmDatabase(WowRealm.Emerald_Dream, "RealmDatabase_Emerald_Dream.CRP");
            SerializeDB.Cpp_RealmDatabase(WowRealm.Warsong, "RealmDatabase_Warsong.CRP");
            SerializeDB.Cpp_RealmDatabase(WowRealm.Al_Akir, "RealmDatabase_Al_Akir.CRP");
            Console.WriteLine("Press any key(and enter) to continue...");
            Console.ReadLine();
        }
    }
}
