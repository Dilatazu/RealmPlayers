using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;

namespace DatabaseHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();
                //var history1 = new Dictionary<string, PlayerHistory>();

                Console.WriteLine("Start loading history1");
                var history1 = VF_RealmPlayersDatabase.RealmDatabase._LoadPlayersHistoryChunked(Environment.CurrentDirectory + "\\", VF_RealmPlayersDatabase.WowRealm.Unknown, DateTime.UtcNow.AddMonths(-12));
                Console.WriteLine("Done loading history1, It took " + (timer.ElapsedMilliseconds / 1000) + " seconds to load history1!");
                Dictionary<string, PlayerHistory> history2 = null;
                VF_RealmPlayersDatabase.Utility.LoadSerialize<Dictionary<string, PlayerHistory>>(Environment.CurrentDirectory + "\\PlayersHistoryData.dat", out history2);
                Console.WriteLine("Done loading history2, It took " + (timer.ElapsedMilliseconds / 1000) + " seconds to load history1 and history2!");

                if (history1.Count == history2.Count)
                {
                    foreach (var currObj in history1)
                    {
                        var historyKey = currObj.Key;
                        var historyObj1 = currObj.Value;
                        var historyObj2 = history2[historyKey];
                        if (historyObj1.IsEqual(historyObj2) == false)
                            throw new Exception("Not Equal1!");
                    }
                }
                else
                    throw new Exception("Not Equal2!");


                Console.WriteLine("Looks like everything is equal!");
                Console.WriteLine("It took " + (timer.ElapsedMilliseconds / 1000) + " seconds to check everything!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                while (true)
                    Console.ReadKey();
            }
            
            
            while (Console.KeyAvailable)
                Console.ReadKey();
            Console.WriteLine("Press any key to shutdown!");
            Console.ReadKey();
        }
    }
}
