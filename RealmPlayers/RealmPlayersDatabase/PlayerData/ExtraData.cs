using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace VF_RealmPlayersDatabase.PlayerData
{
    [ProtoContract]
    public class MountData
    {
        [ProtoMember(1)]
        public string Mount = "";
        [ProtoMember(2)]
        public List<UploadID> Uploaders = new List<UploadID>();

        public MountData() { }
        public MountData(string _Mount, UploadID _UploadID) 
        {
            Mount = _Mount;
            Uploaders.Add(_UploadID);
        }
        public void AddUploader(UploadID _UploadID)
        {
            if (Uploaders.FindIndex((_Value) => _Value.GetContributorID() == _UploadID.GetContributorID()) == -1)
            {
                Uploaders.Add(_UploadID);
            }
        }
        public UploadID GetEarliestUpload()
        {
            if (Uploaders.Count == 0)
                return UploadID.Null();
            UploadID earliestUploader = Uploaders.First();
            foreach(var uploader in Uploaders)
            {
                if (uploader.GetTime() < earliestUploader.GetTime())
                {
                    earliestUploader = uploader;
                }
            }
            return earliestUploader;
        }
    }
    [ProtoContract]
    public class PetData
    {
        [ProtoMember(1)]
        public string Name = "";
        [ProtoMember(2)]
        public int Level = -1;
        [ProtoMember(3)]
        public string CreatureFamily = "";
        [ProtoMember(4)]
        public string CreatureType = "";
        [ProtoMember(5)]
        public List<UploadID> Uploaders = new List<UploadID>();

        public PetData() { }
        public PetData(string _Name, int _Level, string _Family, string _Type, UploadID _Uploader)
        {
            Name = _Name;
            Level = _Level;
            CreatureFamily = _Family;
            CreatureType = _Type;
            Uploaders.Add(_Uploader);
        }

        public void AddUploader(UploadID _UploadID)
        {
            if (Uploaders.FindIndex((_Value) => _Value.GetContributorID() == _UploadID.GetContributorID()) == -1)
            {
                Uploaders.Add(_UploadID);
            }
        }
        public UploadID GetEarliestUpload()
        {
            if (Uploaders.Count == 0)
                return UploadID.Null();
            UploadID earliestUploader = Uploaders.First();
            foreach (var uploader in Uploaders)
            {
                if (uploader.GetTime() < earliestUploader.GetTime())
                {
                    earliestUploader = uploader;
                }
            }
            return earliestUploader;
        }
    }
    [ProtoContract]
    public class CompanionData
    {
        [ProtoMember(1)]
        public string Name = "";
        [ProtoMember(2)]
        public int Level = -1;
        [ProtoMember(3)]
        public List<UploadID> Uploaders = new List<UploadID>();
        
        public CompanionData() { }
        public CompanionData(string _Name, int _Level, UploadID _Uploader)
        {
            Name = _Name;
            Level = _Level;
            Uploaders.Add(_Uploader);
        }

        public void AddUploader(UploadID _UploadID)
        {
            //Om vi vill spara multipla kopior, använd istället: if (Uploaders.FindIndex((_Value) => (_Value.GetContributorID() == _UploadID.GetContributorID() && (_UploadID.GetTime() - _Value.GetTime()).TotalDays < 30)) == -1)
            if (Uploaders.FindIndex((_Value) => _Value.GetContributorID() == _UploadID.GetContributorID()) == -1)
            {
                Uploaders.Add(_UploadID);
            }
        }
        public UploadID GetEarliestUpload()
        {
            if (Uploaders.Count == 0)
                return UploadID.Null();
            UploadID earliestUploader = Uploaders.First();
            foreach (var uploader in Uploaders)
            {
                if (uploader.GetTime() < earliestUploader.GetTime())
                {
                    earliestUploader = uploader;
                }
            }
            return earliestUploader;
        }
    }
    [ProtoContract]
    public class ExtraData
    {
        [ProtoMember(1)]
        public List<MountData> Mounts = new List<MountData>();
        [ProtoMember(2)]
        public List<PetData> Pets = new List<PetData>();
        [ProtoMember(3)]
        public List<CompanionData> Companions = new List<CompanionData>();

        public ExtraData()
        { }
        public void AddData(UploadID _Uploader, System.Xml.XmlNode _PlayerNode, VF.SQLPlayerID? _PlayerID = null, Func<int, VF.SQLUploadID> _GetSQLUploadIDFunc = null)
        {
            AddData(_Uploader, XMLUtility.GetChildValue(_PlayerNode, "ExtraData", ""), _PlayerID, _GetSQLUploadIDFunc);
        }
        public void _AddMount(string _MountName, UploadID _Uploader, VF.SQLPlayerID? _PlayerID = null, Func<int, VF.SQLUploadID> _GetSQLUploadIDFunc = null)
        {
            if (_PlayerID.HasValue && _PlayerID.Value.IsValid() && _GetSQLUploadIDFunc != null)
            {
                VF.SQLComm comm = new VF.SQLComm();
                int mountID = comm.GenerateMountID(_MountName);
                if (mountID > 0)
                {
                    comm.AddPlayerMount(_PlayerID.Value, _GetSQLUploadIDFunc(0), _Uploader.GetTime(), mountID);
                }
            }

            var mountIndex = Mounts.FindIndex((_Value) => _Value.Mount == _MountName);
            if (mountIndex != -1)
            {
                Mounts[mountIndex].AddUploader(_Uploader);
            }
            else
            {
                Mounts.Add(new MountData(_MountName, _Uploader));
            }
        }
        private void _AddPet(string _PetData, UploadID _Uploader, VF.SQLPlayerID? _PlayerID = null, Func<int, VF.SQLUploadID> _GetSQLUploadIDFunc = null)
        {
            var petDatas = _PetData.Split(':');
            if (petDatas.Length != 4)
                return;

            string petName = petDatas[0];
            int petLevel = 0;
            if (int.TryParse(petDatas[1], out petLevel) == false)
                return;
            string petFamily = petDatas[2];
            string petType = petDatas[3];

            if (_PlayerID.HasValue && _PlayerID.Value.IsValid() && _GetSQLUploadIDFunc != null)
            {
                VF.SQLComm comm = new VF.SQLComm();
                int petID = comm.GeneratePetID(petName, petLevel, petFamily, petType);
                if (petID > 0)
                {
                    comm.AddPlayerPet(_PlayerID.Value, _GetSQLUploadIDFunc(0), _Uploader.GetTime(), petID);
                }
            }
            _AddPet(petName, petLevel, petFamily, petType, _Uploader);
        }
        public void _AddPet(string _PetName, int _PetLevel, string _PetFamily, string _PetType, UploadID _Uploader)
        {
            var petIndex = Pets.FindIndex((_Value) => (_Value.Name == _PetName && _Value.Level == _PetLevel && _Value.CreatureFamily == _PetFamily && _Value.CreatureType == _PetType));
            if (petIndex != -1)
            {
                Pets[petIndex].AddUploader(_Uploader);
            }
            else
            {
                Pets.Add(new PetData(_PetName, _PetLevel, _PetFamily, _PetType, _Uploader));
            }
        }
        private void _AddCompanion(string _CompanionData, UploadID _Uploader, VF.SQLPlayerID? _PlayerID = null, Func<int, VF.SQLUploadID> _GetSQLUploadIDFunc = null)
        {
            var companionDatas = _CompanionData.Split(':');
            if (companionDatas.Length != 2)
                return;

            string companionName = companionDatas[0];
            int companionLevel = 0;
            if (int.TryParse(companionDatas[1], out companionLevel) == false)
                return;

            if(_PlayerID.HasValue && _PlayerID.Value.IsValid() && _GetSQLUploadIDFunc != null)
            {
                VF.SQLComm comm = new VF.SQLComm();
                int companionID = comm.GenerateCompanionID(companionName, companionLevel);
                if(companionID > 0)
                {
                    comm.AddPlayerCompanion(_PlayerID.Value, _GetSQLUploadIDFunc(0), _Uploader.GetTime(), companionID);
                }
            }
            _AddCompanion(companionName, companionLevel, _Uploader);
        }
        public void _AddCompanion(string _CompanionName, int _CompanionLevel, UploadID _Uploader)
        {
            var companionIndex = Companions.FindIndex((_Value) => (_Value.Name == _CompanionName && _Value.Level == _CompanionLevel));
            if (companionIndex != -1)
            {
                Companions[companionIndex].AddUploader(_Uploader);
            }
            else
            {
                Companions.Add(new CompanionData(_CompanionName, _CompanionLevel, _Uploader));
            }
        }
        public void AddData(UploadID _Uploader, string _ExtraDataString, VF.SQLPlayerID? _PlayerID = null, Func<int, VF.SQLUploadID> _GetSQLUploadIDFunc = null)
        {
            try
            {
                if (_ExtraDataString == "") return;
                string[] extraDatas = _ExtraDataString.Split(',');
                foreach (var extraData in extraDatas)
                {
                    if (extraData.StartsWith("M:"))
                    {
                        //Mount
                        _AddMount(extraData.Substring(2), _Uploader, _PlayerID, _GetSQLUploadIDFunc);

                    }
                    else if (extraData.StartsWith("P:"))
                    {
                        //Pet
                        _AddPet(extraData.Substring(2), _Uploader, _PlayerID, _GetSQLUploadIDFunc);
                    }
                    else if (extraData.StartsWith("C:"))
                    {
                        //Companion
                        _AddCompanion(extraData.Substring(2), _Uploader, _PlayerID, _GetSQLUploadIDFunc);
                    }
                }
            }
            catch (Exception)
            { }
        }
        public bool PurgeBefore(DateTime _PurgeDate)
        {
            bool purgeResult = false;
            for(int i = 0; i < Companions.Count; ++i)
            {
                for(int u = 0; u < Companions[i].Uploaders.Count; ++u)
                {
                    if(Companions[i].Uploaders[u].GetTime() < _PurgeDate)
                    {
                        Companions[i].Uploaders.RemoveAt(u);
                        --u;
                    }
                }
                if(Companions[i].Uploaders.Count <= 0)
                {
                    Logger.ConsoleWriteLine("Removed Companion: " + Companions[i].Name, ConsoleColor.Cyan);
                    purgeResult = true;
                    Companions.RemoveAt(i);
                    --i;
                }
            }

            for (int i = 0; i < Pets.Count; ++i)
            {
                for (int u = 0; u < Pets[i].Uploaders.Count; ++u)
                {
                    if (Pets[i].Uploaders[u].GetTime() < _PurgeDate)
                    {
                        Pets[i].Uploaders.RemoveAt(u);
                        --u;
                    }
                }
                if (Pets[i].Uploaders.Count <= 0)
                {
                    Logger.ConsoleWriteLine("Removed Pet: " + Pets[i].Name, ConsoleColor.Cyan);
                    purgeResult = true;
                    Pets.RemoveAt(i);
                    --i;
                }
            }

            for (int i = 0; i < Mounts.Count; ++i)
            {
                for (int u = 0; u < Mounts[i].Uploaders.Count; ++u)
                {
                    if (Mounts[i].Uploaders[u].GetTime() < _PurgeDate)
                    {
                        Mounts[i].Uploaders.RemoveAt(u);
                        --u;
                    }
                }
                if (Mounts[i].Uploaders.Count <= 0)
                {
                    Logger.ConsoleWriteLine("Removed Mount: " + Mounts[i].Mount, ConsoleColor.Cyan);
                    purgeResult = true;
                    Mounts.RemoveAt(i);
                    --i;
                }
            }
            return purgeResult;
        }
    }
}
