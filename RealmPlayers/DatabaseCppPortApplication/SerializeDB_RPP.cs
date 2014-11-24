using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCppPortApplication
{
    partial class SerializeDB
    {
        internal static void SerializeCharacterData(VF_RealmPlayersDatabase.PlayerData.CharacterData _Character)
        {
            CPP.Serialize_Int32((int)_Character.Race);
            CPP.Serialize_Int32((int)_Character.Class);
            CPP.Serialize_Int32((int)_Character.Sex);
            CPP.Serialize_Int32(_Character.Level);
        }
        internal static void SerializeGuildData(VF_RealmPlayersDatabase.PlayerData.GuildData _Guild)
        {
            CPP.Serialize_String(_Guild.GuildName);
            CPP.Serialize_String(_Guild.GuildRank);
            CPP.Serialize_Int32(_Guild.GuildRankNr);
        }
        internal static void SerializeHonorData(VF_RealmPlayersDatabase.PlayerData.HonorData _Honor)
        {
            CPP.Serialize_Int32(_Honor.CurrentRank);
            CPP.Serialize_Float(_Honor.CurrentRankProgress);
            CPP.Serialize_Int32(_Honor.TodayHK);
            CPP.Serialize_Int32(_Honor.TodayDK);
            CPP.Serialize_Int32(_Honor.YesterdayHK);
            CPP.Serialize_Int32(_Honor.YesterdayHonor);
            CPP.Serialize_Int32(_Honor.ThisWeekHK);
            CPP.Serialize_Int32(_Honor.ThisWeekHonor);
            CPP.Serialize_Int32(_Honor.LastWeekHK);
            CPP.Serialize_Int32(_Honor.LastWeekHonor);
            CPP.Serialize_Int32(_Honor.LastWeekStanding);
            CPP.Serialize_Int32(_Honor.LifetimeHK);
            CPP.Serialize_Int32(_Honor.LifetimeDK);
            CPP.Serialize_Int32(_Honor.LifetimeHighestRank);
        }
        internal static void SerializeGearData(VF_RealmPlayersDatabase.PlayerData.GearData _Gear)
        {
            CPP.Serialize_Int32(_Gear.Items.Count);
            foreach (var item in _Gear.Items)
            {
                CPP.Serialize_Int32((int)item.Key);

                /*Serialize PlayerData.GearData.GearItem*/
                {
                    CPP.Serialize_Int32((int)item.Value.Slot);
                    CPP.Serialize_Int32(item.Value.ItemID);
                    CPP.Serialize_Int32(item.Value.EnchantID);
                    CPP.Serialize_Int32(item.Value.SuffixID);
                    CPP.Serialize_Int32(item.Value.UniqueID);
                    if (item.Value.GemIDs == null)
                    {
                        CPP.Serialize_Int32(0);
                    }
                    else
                    {
                        CPP.Serialize_Int32(item.Value.GemIDs.Length);
                        foreach (var gemID in item.Value.GemIDs)
                        {
                            CPP.Serialize_Int32(gemID);
                        }
                    }
                }
                /*Serialize PlayerData.GearData.GearItem*/
            }
        }
        internal static void SerializeArenaData(VF_RealmPlayersDatabase.PlayerData.ArenaData _Arena)
        {
            if (_Arena == null)
            {
                CPP.Serialize_Int32(0);
            }
            else
            {
                const int _ArenaData_FLAG_TEAM2V2 = 0x1;
                const int _ArenaData_FLAG_TEAM3V3 = 0x2;
                const int _ArenaData_FLAG_TEAM5V5 = 0x4;

                int teamFlags = 0;
                if (_Arena.Team2v2 != null) teamFlags |= _ArenaData_FLAG_TEAM2V2;
                if (_Arena.Team3v3 != null) teamFlags |= _ArenaData_FLAG_TEAM3V3;
                if (_Arena.Team5v5 != null) teamFlags |= _ArenaData_FLAG_TEAM5V5;
                CPP.Serialize_Int32(teamFlags);
                if (teamFlags != 0)
                {
                    var apds = new VF_RealmPlayersDatabase.PlayerData.ArenaPlayerData[3];
                    apds[0] = _Arena.Team2v2;
                    apds[1] = _Arena.Team3v3;
                    apds[2] = _Arena.Team5v5;
                    foreach (var apd in apds)
                    {
                        if (apd == null)
                            continue;

                        /*Serialize PlayerData.ArenaData.ArenaPlayerData*/
                        CPP.Serialize_String(apd.TeamName);
                        CPP.Serialize_Int32(apd.TeamRating);
                        CPP.Serialize_Int32(apd.GamesPlayed);
                        CPP.Serialize_Int32(apd.GamesWon);
                        CPP.Serialize_Int32(apd.PlayerPlayed);
                        CPP.Serialize_Int32(apd.PlayerRating);
                        /*Serialize PlayerData.ArenaData.ArenaPlayerData*/
                    }
                }
            }
        }
        internal static void SerializeUploadID(VF_RealmPlayersDatabase.UploadID _Uploader)
        {
            CPP.Serialize_Int32(_Uploader.GetContributorID());
            CPP.Serialize_UInt64((UInt64)_Uploader.GetTime().ToBinary());
        }
        internal static void SerializePlayerData(VF_RealmPlayersDatabase.PlayerData.Player _Player)
        {
            CPP.Serialize_String(_Player.Name);
            CPP.Serialize_Int32((int)_Player.Realm);

            /*Serialize PlayerData.CharacterData*/
            SerializeCharacterData(_Player.Character);
            /*Serialize PlayerData.CharacterData*/

            /*Serialize PlayerData.GuildData*/
            SerializeGuildData(_Player.Guild);
            /*Serialize PlayerData.GuildData*/

            /*Serialize PlayerData.HonorData*/
            SerializeHonorData(_Player.Honor);
            /*Serialize PlayerData.HonorData*/

            /*Serialize PlayerData.GearData*/
            SerializeGearData(_Player.Gear);
            /*Serialize PlayerData.GearData*/

            /*Serialize PlayerData.ArenaData*/
            SerializeArenaData(_Player.Arena);
            /*Serialize PlayerData.ArenaData*/

            CPP.Serialize_UInt64((UInt64)_Player.LastSeen.ToBinary());

            /*Serialize PlayerData.UploadID*/
            SerializeUploadID(_Player.Uploader);
            /*Serialize PlayerData.UploadID*/
        }
        public static void Cpp_RealmDatabase(VF_RealmPlayersDatabase.WowRealm _Realm, string _Filename)
        {
            Console.WriteLine("Started Loading RealmDatabase");
            VF_RealmPlayersDatabase.RealmDatabase db = new VF_RealmPlayersDatabase.RealmDatabase(_Realm);
            db.LoadDatabase("Database\\" + _Realm.ToString() + "\\");
            Console.WriteLine("Loading Players for RealmDatabase");
            db.WaitForLoad(VF_RealmPlayersDatabase.RealmDatabase.LoadStatus.PlayersLoaded);
            Console.WriteLine("Loading PlayersHistory for RealmDatabase");
            db.WaitForLoad(VF_RealmPlayersDatabase.RealmDatabase.LoadStatus.PlayersHistoryLoaded);
            Console.WriteLine("Loading Everything else for RealmDatabase");
            db.WaitForLoad(VF_RealmPlayersDatabase.RealmDatabase.LoadStatus.EverythingLoaded);
            Console.WriteLine("Done with Loading RealmDatabase");

            Console.WriteLine("Started Serializing");
            CPP.Begin_Serialize(_Filename);

            CPP.Serialize_Int32((int)db.Realm);
            CPP.Serialize_Int32(db.Players.Count);
            foreach(var player in db.Players)
            {
                CPP.Serialize_String(player.Key);

                /*Serialize PlayerData*/
                SerializePlayerData(player.Value);
                /*Serialize PlayerData*/
            }

            CPP.Serialize_Int32(db.PlayersHistory.Count);
            foreach(var playerHistory in db.PlayersHistory)
            {
                CPP.Serialize_String(playerHistory.Key);

                /*Serialize PlayerDataHistory*/
                {
                    CPP.Serialize_Int32(playerHistory.Value.CharacterHistory.Count);
                    foreach(var item in playerHistory.Value.CharacterHistory)
                    {
                        /*Serialize PlayerDataHistory.CharacterDataHistoryItem*/
                        SerializeCharacterData(item.Data);
                        SerializeUploadID(item.Uploader);
                        /*Serialize PlayerDataHistory.CharacterDataHistoryItem*/
                    }

                    CPP.Serialize_Int32(playerHistory.Value.GuildHistory.Count);
                    foreach (var item in playerHistory.Value.GuildHistory)
                    {
                        /*Serialize PlayerDataHistory.GuildDataHistoryItem*/
                        SerializeGuildData(item.Data);
                        SerializeUploadID(item.Uploader);
                        /*Serialize PlayerDataHistory.GuildDataHistoryItem*/
                    }

                    CPP.Serialize_Int32(playerHistory.Value.HonorHistory.Count);
                    foreach (var item in playerHistory.Value.HonorHistory)
                    {
                        /*Serialize PlayerDataHistory.HonorDataHistoryItem*/
                        SerializeHonorData(item.Data);
                        SerializeUploadID(item.Uploader);
                        /*Serialize PlayerDataHistory.HonorDataHistoryItem*/
                    }

                    CPP.Serialize_Int32(playerHistory.Value.GearHistory.Count);
                    foreach (var item in playerHistory.Value.GearHistory)
                    {
                        /*Serialize PlayerDataHistory.GearDataHistoryItem*/
                        SerializeGearData(item.Data);
                        SerializeUploadID(item.Uploader);
                        /*Serialize PlayerDataHistory.GearDataHistoryItem*/
                    }

                    if (playerHistory.Value.ArenaHistory == null)
                    {
                        CPP.Serialize_Int32(0);
                    }
                    else
                    {
                        CPP.Serialize_Int32(playerHistory.Value.ArenaHistory.Count);
                        foreach (var item in playerHistory.Value.ArenaHistory)
                        {
                            /*Serialize PlayerDataHistory.ArenaDataHistoryItem*/
                            SerializeArenaData(item.Data);
                            SerializeUploadID(item.Uploader);
                            /*Serialize PlayerDataHistory.ArenaDataHistoryItem*/
                        }
                    }
                }
                /*Serialize PlayerDataHistory*/
            }

            Console.WriteLine("Done with Serializing");
            CPP.End_Serialize();

            Console.WriteLine("Started Validating RealmDatabase");
            CPP.Validate_RealmDatabase(_Filename);
            Console.WriteLine("Done with Validating RealmDatabase");
        }
    }
}
