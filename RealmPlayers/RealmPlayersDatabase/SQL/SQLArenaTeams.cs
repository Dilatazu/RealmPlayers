using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

using Contributor = VF_RealmPlayersDatabase.Contributor;
using PlayerData = VF_RealmPlayersDatabase.PlayerData;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using PlayerRace = VF_RealmPlayersDatabase.PlayerRace;
using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerSex = VF_RealmPlayersDatabase.PlayerSex;
using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;

namespace VF
{
    public struct SQLArenaInfo
    {
        public int Team2v2;
        public int Team3v3;
        public int Team5v5;
        public SQLArenaInfo(int _Team2v2, int _Team3v3, int _Team5v5)
        {
            Team2v2 = _Team2v2;
            Team3v3 = _Team3v3;
            Team5v5 = _Team5v5;
        }
        public bool IsNull()
        {
            return Team2v2 == 0 && Team3v3 == 0 && Team5v5 == 0;
        }
    }
    public partial class SQLComm
    {
        public bool GetPlayerArenaInfo(SQLPlayerData _PlayerData, out SQLArenaInfo _ResultArenaInfo)
        {
            _ResultArenaInfo = new SQLArenaInfo(0, 0, 0);
            if (_PlayerData.PlayerArenaID == 0) return false;

            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                const int TEAM2V2_COLUMN = 0;
                const int TEAM3V3_COLUMN = 1;
                const int TEAM5V5_COLUMN = 2;
                using (var cmd = new NpgsqlCommand("SELECT team_2v2, team_3v3, team_5v5 FROM PlayerArenaInfoTable WHERE id=:ID", conn))
                {
                    {
                        var idParam = new NpgsqlParameter("ID", NpgsqlDbType.Integer);
                        idParam.Value = _PlayerData.PlayerArenaID;
                        cmd.Parameters.Add(idParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultArenaInfo.Team2v2 = reader.GetInt32(TEAM2V2_COLUMN);
                            _ResultArenaInfo.Team3v3 = reader.GetInt32(TEAM3V3_COLUMN);
                            _ResultArenaInfo.Team5v5 = reader.GetInt32(TEAM5V5_COLUMN);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool GetPlayerArenaData(SQLArenaInfo _ArenaInfo, out PlayerData.ArenaData _ResultArenaData)
        {
            _ResultArenaData = null;
            if (_ArenaInfo.IsNull() == true) return false;

            int[] teamIDs = new int[3];
            int teamIDsCounter = 0;
            if (_ArenaInfo.Team2v2 != 0) teamIDs[teamIDsCounter++] = _ArenaInfo.Team2v2;
            if (_ArenaInfo.Team3v3 != 0) teamIDs[teamIDsCounter++] = _ArenaInfo.Team3v3;
            if (_ArenaInfo.Team5v5 != 0) teamIDs[teamIDsCounter++] = _ArenaInfo.Team5v5;
            if (teamIDs.Length > teamIDsCounter)
            {
                int[] oldTeamIDs = teamIDs;
                teamIDs = new int[teamIDsCounter];
                for(int i = 0; i < teamIDsCounter; ++i)
                {
                    teamIDs[i] = oldTeamIDs[i];
                }
            }

            using (var conn = new NpgsqlConnection(g_ConnectionString))
            {
                conn.Open();
                const int ID_COLUMN = 0;
                const int TEAMNAME_COLUMN = 1;
                const int TEAMRATING_COLUMN = 2;
                const int GAMESPLAYED_COLUMN = 3;
                const int GAMESWON_COLUMN = 4;
                const int PLAYERGAMESPLAYED_COLUMN = 5;
                const int PLAYERRATING_COLUMN = 6;

                using (var cmd = new NpgsqlCommand("SELECT id, teamname, teamrating, gamesplayed, gameswon, playergamesplayed, playerrating FROM PlayerArenaDataTable WHERE id IN (:IDs)", conn))
                {
                    {
                        var idsParam = new NpgsqlParameter("IDs", NpgsqlDbType.Array | NpgsqlDbType.Integer);
                        idsParam.Value = teamIDs;
                        cmd.Parameters.Add(idsParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows == true)
                        {
                            _ResultArenaData = new PlayerData.ArenaData();
                            while (reader.Read() == true)
                            {
                                var arenaTeamData = new PlayerData.ArenaPlayerData();
                                int id = reader.GetInt32(ID_COLUMN);

                                arenaTeamData.TeamName = reader.GetString(TEAMNAME_COLUMN);
                                arenaTeamData.TeamRating = reader.GetInt32(TEAMRATING_COLUMN);
                                arenaTeamData.GamesPlayed = reader.GetInt32(GAMESPLAYED_COLUMN);
                                arenaTeamData.GamesWon = reader.GetInt32(GAMESWON_COLUMN);
                                arenaTeamData.PlayerPlayed = reader.GetInt32(PLAYERGAMESPLAYED_COLUMN);
                                arenaTeamData.PlayerRating = reader.GetInt32(PLAYERRATING_COLUMN);

                                if (id == _ArenaInfo.Team2v2) _ResultArenaData.Team2v2 = arenaTeamData;
                                if (id == _ArenaInfo.Team3v3) _ResultArenaData.Team3v3 = arenaTeamData;
                                if (id == _ArenaInfo.Team5v5) _ResultArenaData.Team5v5 = arenaTeamData;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
