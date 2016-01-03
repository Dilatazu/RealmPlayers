using System.Collections.Generic;
using System.Linq;
using RealmPlayersServer;
using VF_RaidDamageDatabase.Models;

namespace VF_RaidDamageDatabase.Repositories
{
    internal interface IPurgedPlayersRepository
    {
        IEnumerable<PurgedPlayer> GetPurgedPlayers();
    }

    internal class PurgedPlayersRepository : IPurgedPlayersRepository
    {
        public IEnumerable<PurgedPlayer> GetPurgedPlayers()
        {
            string purgedPlayers = DynamicFileData.GetTextFile(Constants.RDDbDir + "Variables\\PurgedPlayers.txt");

            var purgedPlayersCollection = (from purgedPlayer in purgedPlayers.Split('\n')
                where purgedPlayer.Contains('|')
                select purgedPlayer.Split('|')
                into splitted
                where splitted.Length.Equals(2)
                select new PurgedPlayer(splitted[0].Trim(), splitted[1].Trim()));

            return purgedPlayersCollection;
        }
    }
}
