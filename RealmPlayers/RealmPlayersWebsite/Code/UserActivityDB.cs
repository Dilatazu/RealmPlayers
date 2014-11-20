using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

public partial class UserActivityDB
{
    public class Element : VF.MongoDBItem
    {
        public class URLVisit
        {
            public DateTime VisitTime {get;set;}
            public string URL {get;set;}
            public string FromURL {get;set;}
            public URLVisit()
            { }
            public URLVisit(string _URL, string _FromURL, DateTime _VisitTime)
            {
                URL = _URL;
                FromURL = _FromURL;
                VisitTime = _VisitTime;
            }
        }
        public string IP {get; set;}
        public List<URLVisit> URLVisits { get; set; }

        public Element() { }
        public Element(string _IP, string _URL, string _FromURL, DateTime _VisitTime)
        {
            IP = _IP;
            URLVisits = new List<URLVisit>();
            URLVisits.Add(new URLVisit(_URL, _FromURL, _VisitTime));
        }
    }

    private static VF.MongoCollection<Element> _sm_UserActivityDB_Internal = null;
    private static object sm_ThreadObject = new object();

    private static VF.MongoCollection<Element> sm_UserActivityDB
    {
        get
        {
            Initialize();
            if (_sm_UserActivityDB_Internal.IsConnected() == false)
                return null;
            return _sm_UserActivityDB_Internal;
        }
    }
    public static void Initialize()
    {
        if (_sm_UserActivityDB_Internal == null)
        {
            lock (sm_ThreadObject)
            {
                if (_sm_UserActivityDB_Internal == null)
                {
                    VF.MongoDatabase databaseClient = RealmPlayersDB.GetInstance();
                    _sm_UserActivityDB_Internal = databaseClient.GetCollection<Element>("UserActivity");
                }
            }
        }
    }
}
public partial class UserActivityDB
{
    private static DateTime sm_LastCleanupTime = DateTime.UtcNow.AddMinutes(-25);
    public static void AddUserActivity(string _IP, string _URL, string _FromURL)
    {
        if (sm_UserActivityDB == null)
            return;
        try
        {
            if((DateTime.UtcNow - sm_LastCleanupTime).Minutes > 30)
            {
                lock(sm_ThreadObject)
                {
                    if ((DateTime.UtcNow - sm_LastCleanupTime).Minutes > 30)
                    {
                        sm_LastCleanupTime = DateTime.UtcNow;
                        (new System.Threading.Tasks.Task(new Action(CleanupUserActivity))).Start();
                    }
                }
            }
    #if OPTIMIZED_ADDUSERACTIVITY
            Element element = sm_UserActivityDB.Find(Query.EQ("IP", _IP));
            if(element == null)
            {
                sm_UserActivityDB.Add(new Element(_IP, _URL, _FromURL, DateTime.UtcNow));
                return;
            }
            element.URLVisits.Add(new Element.URLVisit(_URL, _FromURL, DateTime.UtcNow));
            sm_UserActivityDB.MongoDBCollection.Update(Query.EQ("IP", _IP), Update.Replace(element));
    #else
            var elements = sm_UserActivityDB.MongoDBCollection.Find(Query.EQ("IP", _IP)).SetFields(Fields.Include("IP", "Id"));
            if(elements.Count() == 0)
            {
                sm_UserActivityDB.Add(new Element(_IP, _URL, _FromURL, DateTime.UtcNow));
                return;
            }
            else if(elements.Count() == 1)
            {
                Element refElement = new Element();
                foreach(var element in elements)
                {
                    refElement.Id = element.Id;
                    refElement.IP = element.IP;
                }
                sm_UserActivityDB.MongoDBCollection.Update(Query.EQ("IP", _IP), Update.Push("URLVisits", BsonValue.Create(new Element.URLVisit(_URL, _FromURL, DateTime.UtcNow).ToBsonDocument())));
            }
            else
            {
                VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("AddUserActivity() ERROR, there was more than 1 matching IP elements!", ConsoleColor.Red);
            }
    #endif
        }
        catch (Exception ex)
        {
            RealmPlayersServer.Logger.LogException(ex);
        }
    }
    public static void CleanupUserActivity()
    {
        if (sm_UserActivityDB == null)
            return;
        var startTime = DateTime.Now;
        var elements = sm_UserActivityDB.MongoDBCollection.Find(Query.SizeGreaterThanOrEqual("URLVisits", 100));
        int totalRemovedURLVisits = 0;
        foreach(var element in elements)
        {
            List<Element.URLVisit> removedURLVisits = new List<Element.URLVisit>();
            for (int i = 0; i < element.URLVisits.Count - 50; ++i)
            {
                removedURLVisits.Add(element.URLVisits[i]);
            }
            if(removedURLVisits.Count > 0)
            {
                DateTime removeLatestVisitTime = removedURLVisits.Last().VisitTime;
                totalRemovedURLVisits += removedURLVisits.Count;
                sm_UserActivityDB.MongoDBCollection.Update(Query.EQ("IP", element.IP), Update.Pull("URLVisits", Query.LTE("VisitTime", removeLatestVisitTime)));
            }
        }
        VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("CleanupUserActivity() removed " + totalRemovedURLVisits + " outdated URLVisits! It took " + (DateTime.Now - startTime).TotalSeconds.ToStringDot("0.0") + " seconds");
    }
    public static long GetUniqueVisitCountsSince(DateTime _StartDate)
    {
        if (sm_UserActivityDB == null)
            return 0;
        return sm_UserActivityDB.MongoDBCollection.Count(Query.ElemMatch("URLVisits", Query.GTE("VisitTime", _StartDate)));
    }
    public static long GetTotalUniqueVisitors()
    {
        if (sm_UserActivityDB == null)
            return 0;
        return sm_UserActivityDB.MongoDBCollection.Count();
    }

    public static Dictionary<string, Element.URLVisit> GetLatestVisits(DateTime _StartDate)
    {
        Dictionary<string, Element.URLVisit> retData = new Dictionary<string, Element.URLVisit>();
        if (sm_UserActivityDB == null)
            return retData;

        var elements = sm_UserActivityDB.MongoDBCollection.Find(Query.ElemMatch("URLVisits", Query.GTE("VisitTime", _StartDate))).SetFields(Fields.Include("IP").Slice("URLVisits", -1));
        foreach(var element in elements)
        {
            string key = element.IP;
            while (retData.ContainsKey(key))
                key = key + "?";
            retData.Add(key, element.URLVisits.Last());
        }
        return retData;
    }
    public static Element GetActivity(string _IP)
    {
        if (sm_UserActivityDB == null)
            return null;
        return sm_UserActivityDB.Find(Query.EQ("IP", _IP));
    }
}