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
        public void AddData(UploadID _Uploader, System.Xml.XmlNode _PlayerNode)
        {
            AddData(_Uploader, XMLUtility.GetChildValue(_PlayerNode, "ExtraData", ""));
        }
        private void AddMount(string _MountName, UploadID _Uploader)
        {
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
        private void AddPet(string _PetData, UploadID _Uploader)
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

            var petIndex = Pets.FindIndex((_Value) => (_Value.Name == petName && _Value.Level == petLevel && _Value.CreatureFamily == petFamily && _Value.CreatureType == petType));
            if (petIndex != -1)
            {
                Pets[petIndex].AddUploader(_Uploader);
            }
            else
            {
                Pets.Add(new PetData(petName, petLevel, petFamily, petType, _Uploader));
            }
        }
        private void AddCompanion(string _CompanionData, UploadID _Uploader)
        {
            var companionDatas = _CompanionData.Split(':');
            if (companionDatas.Length != 2)
                return;

            string companionName = companionDatas[0];
            int companionLevel = 0;
            if (int.TryParse(companionDatas[1], out companionLevel) == false)
                return;

            var companionIndex = Companions.FindIndex((_Value) => (_Value.Name == companionName && _Value.Level == companionLevel));
            if (companionIndex != -1)
            {
                Companions[companionIndex].AddUploader(_Uploader);
            }
            else
            {
                Companions.Add(new CompanionData(companionName, companionLevel, _Uploader));
            }
        }
        public void AddData(UploadID _Uploader, string _ExtraDataString)
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
                        AddMount(extraData.Substring(2), _Uploader);
                    }
                    else if (extraData.StartsWith("P:"))
                    {
                        //Pet
                        AddPet(extraData.Substring(2), _Uploader);
                    }
                    else if (extraData.StartsWith("C:"))
                    {
                        //Companion
                        AddCompanion(extraData.Substring(2), _Uploader);
                    }
                }
            }
            catch (Exception)
            { }
        }
    }
}
