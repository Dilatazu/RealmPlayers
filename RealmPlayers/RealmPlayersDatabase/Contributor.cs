using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VF_RealmPlayersDatabase
{
    [Serializable]
    public class Contributor : ISerializable
    {
        public static int ContributorTrustworthyIDBound = 1000000; //Första 1000000 är till för VIPs
        public int ContributorID;
        public string UserID;
        public string Name;
        public string IP;
        public bool TrustWorthy;
        List<Tuple<DateTime, WarningFlag>> Warnings = new List<Tuple<DateTime, WarningFlag>>();
        public UploadID GetUploadID(DateTime _DateTime)
        {
            return new UploadID(ContributorID, _DateTime);
        }
        public void SetUserIP(System.Net.IPEndPoint _IP)
        {
            IP = _IP.Address.ToString();
        }
        public void SetUserIP(System.Net.IPAddress _IP)
        {
            IP = _IP.ToString();
        }
        public int GetContributorID()
        {
            return ContributorID;
        }
        public Contributor()
        { }
        public Contributor(int _ContributorID, System.Net.IPEndPoint _IP)
        {
            ContributorID = _ContributorID;
            IP = _IP.Address.ToString();
            UserID = "Unknown.123456";
            Name = "Unknown";
        }
        public Contributor(int _ContributorID, System.Net.IPAddress _IP)
        {
            ContributorID = _ContributorID;
            IP = _IP.ToString();
            UserID = "Unknown.123456";
            Name = "Unknown";
        }
        public Contributor(int _ContributorID, string _UserID)
        {
            ContributorID = _ContributorID;
            IP = "";
            UserID = _UserID;
            Name = _UserID.Split('.').First();
        }
        public string GetFilename()
        {
            return Name + "_" + IP;
        }
        public bool IsVIP()
        {
            return ContributorID < ContributorTrustworthyIDBound;
        }
        public bool IsTrustWorthy()
        {
            return ContributorID < ContributorTrustworthyIDBound && Warnings.Count == 0;
        }
        public enum WarningFlag
        {
            NoFlag,
            DataFromFuture,
            TemperedData,
        }
        public void SetWarningFlag(WarningFlag _WarningFlag)
        {
            if (Warnings.Count > 0)
            {
                if (Warnings.Last().Item2 == _WarningFlag)
                {
                    if ((DateTime.UtcNow - Warnings.Last().Item1).TotalMinutes < 5)
                    {
                        return;//Kan inte få en likadan varning inom loppet av 5 minuter(då det kan vara samma dataset)
                    }
                }
            }
            Warnings.Add(new Tuple<DateTime, WarningFlag>(DateTime.UtcNow, _WarningFlag));
            Logger.ConsoleWriteLine(GetFilename() + " just got warning flagged \"" + _WarningFlag.ToString() + "\" Warning Count = " + Warnings.Count);
        }

        #region Serializing
        public Contributor(SerializationInfo _Info, StreamingContext _Context)
        {
            ContributorID = _Info.GetInt32("ContributorID");
            UserID = _Info.GetString("UserID");
            Name = UserID.Split('.')[0];
            IP = _Info.GetString("IP");
            Warnings = (List<Tuple<DateTime, WarningFlag>>)_Info.GetValue("Warnings", typeof(List<Tuple<DateTime, WarningFlag>>));
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("ContributorID", ContributorID);
            _Info.AddValue("UserID", UserID);
            _Info.AddValue("IP", IP);
            _Info.AddValue("Warnings", Warnings);
        }
        #endregion
    }
}
