using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace VF_RaidDamageDatabase
{
    [ProtoContract]
    public class RaidCollection_FightDataFileInfo
    {
        [ProtoMember(1)]
        public DateTime FightStartDateTime; //Sort of unique identifier for particular fight
        [ProtoMember(2)]
        public string FightName; //For easy to find
        [ProtoMember(3)]
        public List<Tuple<string, string>> _RecordedByPlayers = new List<Tuple<string, string>>();//Players/(Key)/DataFile(Value) that is recorded
        [ProtoMember(4)]
        public DateTime _FightEndDateTime = DateTime.MinValue; //Sort of unique identifier for particular fight
        [ProtoMember(5)]
        public FightData.AttemptType AttemptType = FightData.AttemptType.UnknownAttempt;

        public string GetDataFileRecordedBy(string _Player)
        {
            int index = _RecordedByPlayers.FindIndex(_Value => _Value.Item1 == _Player);
            if (index == -1)
                return "";
            else
                return _RecordedByPlayers[index].Item2;
        }
        public string GetFirstDataFile()
        {
            return _RecordedByPlayers.First().Item2;
        }
        public List<string> GetAllDataFiles()
        {
            List<string> retList = new List<string>();
            foreach (var data in _RecordedByPlayers)
            {
                retList.Add(data.Item2);
            }
            return retList;
        }
        public List<string> GetAllRecordedBy()
        {
            List<string> retList = new List<string>();
            foreach (var data in _RecordedByPlayers)
            {
                retList.Add(data.Item1);
            }
            return retList;
        }
        public bool IsRecordedBy(string _Player)
        {
            return GetDataFileRecordedBy(_Player) != "";
        }
        public void AddRecordedBy(string _Player, string _DataFile)
        {
            _RecordedByPlayers.Add(Tuple.Create(_Player, _DataFile));
        }

        public DateTime FightEndDateTime
        {
            get
            {
                if (_FightEndDateTime == DateTime.MinValue)
                    return FightStartDateTime;
                else
                    return _FightEndDateTime;
            }
        }
    }

}
