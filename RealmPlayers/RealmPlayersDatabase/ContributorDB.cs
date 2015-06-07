using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace VF_RealmPlayersDatabase
{
    public partial class ContributorDB
    {
        public class ContributorMetaDBElement : VF.MongoDBItem
        {
            public int VIPContributorIDCounter { get; set; }
            public int ContributorIDCounter { get; set; }
        }
        public class ContributorDBElement : VF.MongoDBItem
        {
            public string Key { get; set; }
            public string Name { get; set; }
            public string UserID { get; set; }
            public string IP { get; set; }
            public int ContributorID { get; set; }
            public bool TrustWorthy { get; set; }
            public string AddedBy { get; set; }

            public ContributorDBElement(string _Key, Contributor _Contributor, string _AddedBy)
            {
                Key = _Key;
                Name = _Contributor.Name;
                UserID = _Contributor.UserID;
                IP = _Contributor.IP;
                ContributorID = _Contributor.ContributorID;
                TrustWorthy = _Contributor.TrustWorthy;
                AddedBy = _AddedBy;
            }
            public ContributorDBElement()
            { }
            public Contributor GetAsContributor()
            {
                return new Contributor { Name = Name, UserID = UserID, ContributorID = ContributorID, IP = IP, TrustWorthy = TrustWorthy };
            }

            internal void Update(Contributor _Contributor)
            {
                Name = _Contributor.Name;
                UserID = _Contributor.UserID;
                IP = _Contributor.IP;
                ContributorID = _Contributor.ContributorID;
                TrustWorthy = _Contributor.TrustWorthy;
            }
        }

        private static VF.MongoCollection<ContributorMetaDBElement> _sm_ContributorMetaDB_Internal = null;
        private static VF.MongoCollection<ContributorDBElement> _sm_ContributorDB_Internal = null;
        private static object sm_ThreadObject = new object();

        private static VF.MongoCollection<ContributorMetaDBElement> sm_ContributorMetaDB
        { 
            get
            {
                Initialize();
                if (_sm_ContributorMetaDB_Internal.IsConnected() == false)
                    return null;
                return _sm_ContributorMetaDB_Internal;
            }
        }
        private static VF.MongoCollection<ContributorDBElement> sm_ContributorDB
        {
            get
            {
                Initialize();
                if (_sm_ContributorDB_Internal.IsConnected() == false)
                    return null;
                return _sm_ContributorDB_Internal;
            }
        }
        public static VF.MongoCollection<ContributorDBElement> GetMongoDB()
        {
            return sm_ContributorDB;
        }
        public static void Initialize()
        {
            if (_sm_ContributorMetaDB_Internal == null)
            {
                lock (sm_ThreadObject)
                {
                    if (_sm_ContributorMetaDB_Internal == null)
                    {
                        VF.MongoDatabase databaseClient = RealmPlayersDB.GetInstance();
                        _sm_ContributorMetaDB_Internal = databaseClient.GetCollection<ContributorMetaDBElement>("ContributorsMeta");
                        _sm_ContributorDB_Internal = databaseClient.GetCollection<ContributorDBElement>("Contributors");
                    }
                }
            }
        }
        public static void MigrateFromProtobufDB(Deprecated.ContributorHandler _ContributorHandler, Dictionary<string, Contributor> _Contributors)
        {
            if (sm_ContributorMetaDB.MongoDBCollection.Count() != 0)
            {
                Logger.ConsoleWriteLine("Contributor meta data already exists");
                var metaDatas = sm_ContributorMetaDB.MongoDBCollection.FindAll();
                if (metaDatas.Count() != 1)
                {
                    Logger.ConsoleWriteLine("Error, there was " + metaDatas.Count() + " meta datas, this is invalid!");
                    throw new Exception("MigrateFromProtobufDB() failed!");
                }
                ContributorDB.ContributorMetaDBElement metaData = metaDatas.First();
                if (metaData.ContributorIDCounter != _ContributorHandler.ContributorIDCounter
                || metaData.VIPContributorIDCounter != _ContributorHandler.VIPContributorIDCounter)
                {
                    Logger.ConsoleWriteLine("Error, meta data did not match! "
                        + "(metaData.ContributorIDCounter(" + metaData.ContributorIDCounter
                        + ") != _ContributorHandler.ContributorIDCounter(" + _ContributorHandler.ContributorIDCounter
                        + ") || metaData.VIPContributorIDCounter(" + metaData.VIPContributorIDCounter
                        + ") != _ContributorHandler.VIPContributorIDCounter(" + _ContributorHandler.VIPContributorIDCounter + ")");
                    throw new Exception("MigrateFromProtobufDB() failed!");
                }
            }
            foreach (var contributor in _Contributors)
            {
                if (sm_ContributorDB.Find(e => e.Key, contributor.Key) == null)
                {
                    var newContributor = new ContributorDB.ContributorDBElement(contributor.Key, contributor.Value, "***REMOVED***");
                    sm_ContributorDB.Add(newContributor);
                    Logger.ConsoleWriteLine("Added new Contributor");
                }
                else
                {
                    Logger.ConsoleWriteLine("Contributor already exists");
                }
            }
            if (sm_ContributorMetaDB.MongoDBCollection.Count() == 0)
            {
                ContributorDB.ContributorMetaDBElement metaData = new ContributorDB.ContributorMetaDBElement();
                metaData.ContributorIDCounter = _ContributorHandler.ContributorIDCounter;
                metaData.VIPContributorIDCounter = _ContributorHandler.VIPContributorIDCounter;
                sm_ContributorMetaDB.Add(metaData);
                Logger.ConsoleWriteLine("Added Contributor meta data");
            }
        }
    }

    public partial class ContributorDB
    {
        class ContributorCheckInfo
        {
            public List<Tuple<string, DateTime>> m_Checks = new List<Tuple<string, DateTime>>();

            public bool AllowedCheck(string _UserID)
            {
                if (m_Checks.Count > 1 && m_Checks.Last().Item1 == _UserID)
                    return true;

                if (m_Checks.Count > 5)
                {
                    if (m_Checks.Last().Item1 != _UserID && (DateTime.UtcNow - m_Checks.Last().Item2).Hours > 2)
                    {
                        m_Checks = new List<Tuple<string, DateTime>>();
                        m_Checks.Add(new Tuple<string, DateTime>(_UserID, DateTime.UtcNow));
                        return true;
                    }
                    else
                    {
                        m_Checks.Add(new Tuple<string, DateTime>(_UserID, DateTime.UtcNow));
                        return false;
                    }
                }
                else
                {
                    m_Checks.Add(new Tuple<string, DateTime>(_UserID, DateTime.UtcNow));
                }
                return true;
            }
        }
        public enum CheckContributorResult
        {
            UserID_Failed_UserPass_Wrong,
            UserID_Failed_Check_Too_Many_Tries,
            UserID_Success_Login,
            UserID_Success_Registered,
        }
        private static Dictionary<string, ContributorCheckInfo> sm_IPContributorChecks = new Dictionary<string, ContributorCheckInfo>();
        public static CheckContributorResult CheckContributor(string _UserID, System.Net.IPEndPoint _UserIP)
        {
            return CheckContributor(_UserID, _UserIP.Address.ToString());
        }
        public static CheckContributorResult CheckContributor(string _UserID, string _UserIP)
        {
            if (sm_ContributorDB == null)
                return CheckContributorResult.UserID_Failed_UserPass_Wrong;
            try
            {
                lock (sm_ThreadObject)
                {
                    if (sm_IPContributorChecks.ContainsKey(_UserIP) == false)
                        sm_IPContributorChecks.Add(_UserIP, new ContributorCheckInfo());

                    if (sm_IPContributorChecks[_UserIP].AllowedCheck(_UserID) == false)
                    {
                        Logger.ConsoleWriteLine(_UserIP + " tried to check UserID(" + _UserID + "), too many tries", ConsoleColor.Red);
                        return CheckContributorResult.UserID_Failed_Check_Too_Many_Tries;
                    }

                    if (sm_ContributorDB.Find((_Value) => _Value.UserID, _UserID) != null)
                        return CheckContributorResult.UserID_Success_Login;

                    //string userName = _UserID.Split('.')[0];
                
                    //var result = sm_ContributorDB.Find(Query.Matches("Key", new BsonRegularExpression("^" + userName + "\\.")));
                    //Om det inte throwar exception här så innebär det att UserName fanns, dvs UserID_Failed_UserPass_Wrong
                    //if (result == null && _AutoRegister == true)
                    //{
                    //    var newMetaObject = sm_ContributorMetaDB.MongoDBCollection
                    //        .FindAndModify(Query.Null, SortBy.Null, Update.Inc("VIPContributorIDCounter", 1), true)
                    //            .GetModifiedDocumentAs<ContributorMetaDBElement>();
                    //    Contributor contributor = new Contributor(newMetaObject.VIPContributorIDCounter, _UserID);
                    //    sm_ContributorDB.Add(new ContributorDBElement(_UserID, contributor));
                    //    return CheckContributorResult.UserID_Success_Registered;
                    //}
                    //else
                    return CheckContributorResult.UserID_Failed_UserPass_Wrong;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return CheckContributorResult.UserID_Failed_UserPass_Wrong;
        }
        public static ContributorDBElement GetContributor(string _UserID)
        {
            if (sm_ContributorDB == null)
                return null;
            try
            {
                lock (sm_ThreadObject)
                {
                    return sm_ContributorDB.Find((_Value) => _Value.Key, _UserID);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return null;
        }
        public static Contributor GetContributor(UploadID _Uploader)
        {
            if (sm_ContributorDB == null)
                return null;
            try
            {
                lock (sm_ThreadObject)
                {
                    if (_Uploader.IsNull() == false)
                    {
                        //make sure uploader is not null before we start looking!
                        var contributorDBElement = sm_ContributorDB.Find((_Value) => _Value.ContributorID, _Uploader.GetContributorID());
                        if (contributorDBElement == null)
                            return null;

                        return contributorDBElement.GetAsContributor();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return null;
        }
        public static Contributor GetContributor(string _UserID, System.Net.IPEndPoint _UserIP, bool _CreateIfNotExist = true)
        {
            if (sm_ContributorDB == null)
                return null;
            return GetContributor(_UserID, _UserIP.Address, _CreateIfNotExist);
        }
        public static Contributor GetContributor(string _UserID, System.Net.IPAddress _UserIP, bool _CreateIfNotExist = true)
        {
            if (sm_ContributorDB == null)
                return null;
            try
            {
                lock (sm_ThreadObject)
                {
                    string userIP = _UserIP.ToString();
                    var contributorDBElement = sm_ContributorDB.Find((_Value) => _Value.UserID, _UserID);

                    if (contributorDBElement != null)
                    {
                        var contributor = contributorDBElement.GetAsContributor();
                        contributor.SetUserIP(_UserIP);
                        contributorDBElement.Update(contributor);
                        sm_ContributorDB.MongoDBCollection.Update(Query.EQ("UserID", _UserID), Update.Replace(contributorDBElement));
                        return contributor;
                    }

                    contributorDBElement = sm_ContributorDB.Find((_Value) => _Value.Key, userIP);
                    if (contributorDBElement == null)
                    {
                        if (_CreateIfNotExist == false)
                            return null;
                        var newMetaObject = sm_ContributorMetaDB.MongoDBCollection
                            .FindAndModify(Query.Null, SortBy.Null, Update.Inc("VIPContributorIDCounter", 1), true)
                                .GetModifiedDocumentAs<ContributorMetaDBElement>();
                        Contributor contributor = new Contributor(newMetaObject.VIPContributorIDCounter, _UserIP);
                        sm_ContributorDB.Add(new ContributorDBElement(userIP, contributor, "***REMOVED***"));
                        return contributor;
                    }

                    return contributorDBElement.GetAsContributor();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return null;
        }
        public static IEnumerable<ContributorDBElement> GetAllTrustWorthyContributors()
        {
            List<ContributorDBElement> retList = new List<ContributorDBElement>();
            if (sm_ContributorDB == null)
                return retList;
            try
            {
                lock (sm_ThreadObject)
                {
                    var contributorsList = sm_ContributorDB.MongoDBCollection.FindAs<ContributorDBElement>(Query.LT("ContributorID", Contributor.ContributorTrustworthyIDBound));
                    return contributorsList;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return retList;
        }
        public static List<string> GetAllTrustWorthyContributorUserIDs()
        {
            List<string> retList = new List<string>();
            if (sm_ContributorDB == null)
                return retList;
            try
            {
                lock (sm_ThreadObject)
                {
                    var contributorsList = sm_ContributorDB.MongoDBCollection.FindAs<ContributorDBElement>(Query.LT("ContributorID", Contributor.ContributorTrustworthyIDBound)).SetFields(Fields<ContributorDBElement>.Include(_Value => _Value.Key));
                    foreach(ContributorDBElement contributor in contributorsList)
                    {
                        retList.Add(contributor.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return retList;
        }
        public static bool AddVIPContributor(string _UserID, string _AddedByUserID)
        {
            if (sm_ContributorDB == null)
                return false;
            try
            {
                lock (sm_ThreadObject)
                {
                    if(sm_ContributorDB.Find((_Value) => _Value.Key, _UserID) == null)
                    {
                        string userName = _UserID.Split('.').First();
                        var result = sm_ContributorDB.Find(Query.Matches("Key", new BsonRegularExpression("^" + userName + "\\.")));
                        if (result == null)
                        {
                            var newMetaObject = sm_ContributorMetaDB.MongoDBCollection
                                .FindAndModify(Query.Null, SortBy.Null, Update.Inc("VIPContributorIDCounter", 1), true)
                                    .GetModifiedDocumentAs<ContributorMetaDBElement>();
                            Contributor contributor = new Contributor(newMetaObject.VIPContributorIDCounter, _UserID);
                            sm_ContributorDB.Add(new ContributorDBElement(_UserID, contributor, _AddedByUserID));
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return false;
        }
    }
}
