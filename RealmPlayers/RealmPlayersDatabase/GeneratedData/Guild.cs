using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase.GeneratedData
{
    public class Guild
    {
        public string Name = "Unknown";
        public string ProgressString = "";
        public PlayerFaction Faction = PlayerFaction.Unknown;
        public List<PlayerData.Player> Players = new List<PlayerData.Player>();
        private List<WowBoss> m_Progress = new List<WowBoss>();
        public Guild(string _Name, RealmDatabase _DatabasePointer)
        {
            Name = _Name;
            foreach (var player in _DatabasePointer.Players)
            {
                if (player.Value.Guild.GuildName == Name)
                {
                    if ((DateTime.UtcNow - player.Value.Uploader.GetTime()).Days < 31)
                    {
                        Players.Add(player.Value);
                        if (Faction == PlayerFaction.Unknown)
                            Faction = StaticValues.GetFaction(player.Value.Character.Race);
                    }
                }
            }
        }

        public long GetTotals(Func<PlayerData.Player, int> _VariableGetLambda)
        {
            long total = 0;
            foreach (var player in Players)
            {
                total += _VariableGetLambda(player);
            }
            return total;
        }
        public double GetTotals(Func<PlayerData.Player, double> _VariableGetLambda)
        {
            double total = 0;
            foreach (var player in Players)
            {
                total += _VariableGetLambda(player);
            }
            return total;
        }
        public long GetTotalHKs()
        {
            return GetTotals((PlayerData.Player _Player) => { return _Player.Honor.LifetimeHK; });
        }
        public int GetTotalPlayers()
        {
            return Players.Count;
        }
        public int GetTotalLevel60s()
        {
            return (int)GetTotals((PlayerData.Player _Player) => { return ((_Player.Character.Level == 60) ? 1 : 0); });
        }
        public List<WowBoss> GetProgress()
        {
            if (m_Progress.Count == 0)
            {
                //Gå igenom alla players och kom fram till vilka som har loot från vilken boss och lägg till isåfall
            }
            return m_Progress;
        }
    }
}
