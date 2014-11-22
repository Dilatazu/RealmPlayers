using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCppPortApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            SerializeDB.Cpp_PlayerSummaryDatabase("PlayerSummaryDatabase.CRP");
            SerializeDB.Cpp_GuildSummaryDatabase("ItemSummaryDatabase.CRP");
            SerializeDB.Cpp_ItemSummaryDatabase("ItemSummaryDatabase.CRP");
            Console.WriteLine("Press any key(and enter) to continue...");
            Console.ReadLine();
        }
    }
}
