using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;

namespace VF
{
    public struct UniqueRaidID
    {
        public int ID;
        public UniqueRaidID(int _ID)
        {
            ID = _ID;
        }
        public bool IsValid()
        {
            return ID > 0;
        }
        public static UniqueRaidID Invalid()
        {
            return new UniqueRaidID(-1);
        }
        static public explicit operator int (UniqueRaidID _UniqueRaidID)
        {
            return _UniqueRaidID.ID;
        }
    }
    public struct UniqueFightID
    {
        public int ID;
        public UniqueFightID(int _ID)
        {
            ID = _ID;
        }
        public bool IsValid()
        {
            return ID > 0;
        }
        public static UniqueFightID Invalid()
        {
            return new UniqueFightID(-1);
        }
        static public explicit operator int (UniqueFightID _UniqueFightID)
        {
            return _UniqueFightID.ID;
        }
    }
    public enum SQLAttemptType
    {
        UnknownAttempt = 0,
        WipeAttempt = 1,
        KillAttempt = 2,
        TrashAttempt = 3,
    }
    public class RaidEntry
    {
        public UniqueRaidID m_UniqueRaidID;
        public int m_IngameRaidID;
        public DateTime m_RaidResetDate;
        public string m_RaidInstance;
        public DateTime m_RaidStartDate;
        public DateTime m_RaidEndDate;
        public string m_RaidOwnerName;
        public WowRealm m_Realm;
    }
    public class FightEntry
    {
        public UniqueFightID m_UniqueFightID;
        public UniqueRaidID m_UniqueRaidID;
        public string m_FightName;
        public DateTime m_FightStartDate;
        public DateTime m_FightEndDate;
        public SQLAttemptType m_AttemptType;
    }
    public class FightDataFile
    {
        public string m_RecordedBy;
        public string m_DataFile;
    }
    public partial class SQLComm
    {
        public bool GetRaidEntry(UniqueRaidID _UniqueRaidID, out RaidEntry _ResultRaid)
        {
            var conn = OpenConnection();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT uniqueraidid, ingameraidid, raidresetdate, raidinstance, raidstartdate, raidenddate, raidownername, realm FROM raidentrytable WHERE uniqueraidid=:UniqueRaidID", conn))
                {
                    {
                        var URIDParam = new NpgsqlParameter("UniqueRaidID", NpgsqlDbType.Integer);
                        URIDParam.Value = (int)_UniqueRaidID.ID;
                        cmd.Parameters.Add(URIDParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultRaid = new RaidEntry();
                            _ResultRaid.m_UniqueRaidID = new UniqueRaidID(reader.GetInt32(0));
                            _ResultRaid.m_IngameRaidID = reader.GetInt32(1);
                            _ResultRaid.m_RaidResetDate = reader.GetTimeStamp(2).DateTime;
                            _ResultRaid.m_RaidInstance = reader.GetString(3);
                            _ResultRaid.m_RaidStartDate = reader.GetTimeStamp(4).DateTime;
                            _ResultRaid.m_RaidEndDate = reader.GetTimeStamp(5).DateTime;
                            _ResultRaid.m_RaidOwnerName = reader.GetString(6);
                            _ResultRaid.m_Realm = (WowRealm)reader.GetInt32(7);
                            return true;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            _ResultRaid = null;
            return false;
        }
        public bool GetRaidMembers(UniqueRaidID _UniqueRaidID, out List<string> _ResultRaidMembers)
        {
            var conn = OpenConnection();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT uniqueraidid, playername FROM raidmemberstable WHERE uniqueraidid=:UniqueRaidID", conn))
                {
                    {
                        var URIDParam = new NpgsqlParameter("UniqueRaidID", NpgsqlDbType.Integer);
                        URIDParam.Value = (int)_UniqueRaidID.ID;
                        cmd.Parameters.Add(URIDParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if(reader.Read() == true)
                        {
                            _ResultRaidMembers = new List<string>();
                            do
                            {
                                _ResultRaidMembers.Add(reader.GetString(1));
                            } while (reader.Read() == true);
                            return true;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            _ResultRaidMembers = null;
            return false;
        }
        public bool GetFightEntries(UniqueRaidID _UniqueRaidID, out List<FightEntry> _ResultFightEntries)
        {
            var conn = OpenConnection();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT id, uniqueraidid, fightname, fightstartdate, fightenddate, attempttype FROM fightentrytable WHERE uniqueraidid=:UniqueRaidID", conn))
                {
                    {
                        var URIDParam = new NpgsqlParameter("UniqueRaidID", NpgsqlDbType.Integer);
                        URIDParam.Value = (int)_UniqueRaidID.ID;
                        cmd.Parameters.Add(URIDParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultFightEntries = new List<FightEntry>();
                            do
                            {
                                FightEntry fightEntry = new FightEntry();
                                fightEntry.m_UniqueFightID = new UniqueFightID(reader.GetInt32(0));
                                fightEntry.m_UniqueRaidID = new UniqueRaidID(reader.GetInt32(1));
                                fightEntry.m_FightName = reader.GetString(2);
                                fightEntry.m_FightStartDate = reader.GetTimeStamp(3).DateTime;
                                fightEntry.m_FightEndDate = reader.GetTimeStamp(4).DateTime;
                                fightEntry.m_AttemptType = (SQLAttemptType)reader.GetInt16(5);
                                _ResultFightEntries.Add(fightEntry);
                            } while (reader.Read() == true);
                            return true;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            _ResultFightEntries = null;
            return false;
        }
        public bool GetFightDataFiles(UniqueFightID _UniqueFightID, out List<FightDataFile> _ResultFightDataFiles)
        {
            var conn = OpenConnection();
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT fightentryid, recordedby, datafile FROM fightdatafilestable WHERE fightentryid=:UniqueFightID", conn))
                {
                    {
                        var URIDParam = new NpgsqlParameter("UniqueFightID", NpgsqlDbType.Integer);
                        URIDParam.Value = (int)_UniqueFightID.ID;
                        cmd.Parameters.Add(URIDParam);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() == true)
                        {
                            _ResultFightDataFiles = new List<FightDataFile>();
                            do
                            {
                                FightDataFile fightEntryDataFile = new FightDataFile();
                                fightEntryDataFile.m_RecordedBy = reader.GetString(1);
                                fightEntryDataFile.m_DataFile = reader.GetString(2);
                                _ResultFightDataFiles.Add(fightEntryDataFile);
                            } while (reader.Read() == true);
                            return true;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            _ResultFightDataFiles = null;
            return false;
        }
    }
}
