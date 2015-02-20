using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

using MongoDBClient = MongoDB.Driver.MongoClient;
using MongoDBServer = MongoDB.Driver.MongoServer;
using MongoDBDatabase = MongoDB.Driver.MongoDatabase;

#if VF_LibrarySlot1
namespace VF_Library1
#elif VF_LibrarySlot2
namespace VF_Library2
#else
namespace VF
#endif
{
    public class MongoDatabase
    {
        private MongoDB.Driver.MongoClient m_Client;
        private MongoDB.Driver.MongoServer m_Server;
        private MongoDB.Driver.MongoDatabase m_Database;
        private bool m_TryingToReconnect = false;
        private bool m_Connected = false;
        public MongoDatabase(string _IPAddress, string _DatabaseName)
        {
            m_Client = new MongoDB.Driver.MongoClient("mongodb://" + _IPAddress);
            m_Server = m_Client.GetServer();
            m_Database = m_Server.GetDatabase(_DatabaseName);
        }
        public MongoCollection<T_CollectionType> GetCollection<T_CollectionType>(string _CollectionName) where T_CollectionType : MongoDBItem
        {
            var collection = m_Database.GetCollection<T_CollectionType>(_CollectionName);
            if (collection == null)
                return null;
            return new MongoCollection<T_CollectionType>(collection, this);
        }
        public bool IsConnected()
        {
            if (m_Connected == false || m_Server.State != MongoServerState.Connected)
            {
                if(m_TryingToReconnect == false)
                {
                    lock (m_Client)
                    {
                        if (m_TryingToReconnect == true)
                        {
                            return false;
                        }
                        else
                        {
                            m_TryingToReconnect = true;
                        }
                    }
                    m_Connected = false;
                    (new System.Threading.Tasks.Task(() =>
                    {
                        try
                        {
                            m_Server.Connect();
                            if (m_Server.State == MongoServerState.Connected)
                                m_Connected = true;
                        }
                        catch (Exception)
                        { }
                        m_TryingToReconnect = false;
                    })).Start();
                }
            }
            return m_Connected == true && m_Server.State == MongoServerState.Connected;
        }
        public void DropDatabase()
        {
            m_Database.Drop();
        }
        public void DropCollection(string _CollectionName)
        {
            m_Database.DropCollection(_CollectionName);
        }
    }
    public class MongoDBItem
    {
        public MongoDB.Bson.ObjectId Id { get; set; }
    }
    public class MongoCollection<T_CollectionType> where T_CollectionType : MongoDBItem
    {
        private MongoDB.Driver.MongoCollection<T_CollectionType> m_Collection;
        private MongoDatabase m_Parent;

        public MongoDB.Driver.MongoCollection<T_CollectionType> MongoDBCollection
        { 
            get { return m_Collection; } 
        }
        public MongoCollection(MongoDB.Driver.MongoCollection<T_CollectionType> _Collection, MongoDatabase _Parent)
        {
            m_Collection = _Collection;
            m_Parent = _Parent;
        }
        public bool IsConnected()
        {
            return m_Parent.IsConnected();
        }

        public T_CollectionType Find<T_CompareType>(System.Linq.Expressions.Expression<Func<T_CollectionType, T_CompareType>> _Lambda, T_CompareType _CompareValue)
        {
            return m_Collection.FindOne(MongoDB.Driver.Builders.Query<T_CollectionType>.EQ<T_CompareType>(_Lambda, _CompareValue));
        }
        public T_CollectionType Find(IMongoQuery _Query)
        {
            return m_Collection.FindOne(_Query);
        }
        //MongoDB.Driver.MongoCursor<T_CollectionType> FindAll<T_CompareType>(System.Linq.Expressions.Expression<Func<T_CollectionType, T_CompareType>> _Lambda, T_CompareType _CompareValue)
        //{
        //    return m_Collection.FindAll(MongoDB.Driver.Builders.Query<T_CollectionType>.EQ<T_CompareType>(_Lambda, _CompareValue));
        //}

        public void Add(T_CollectionType _Item)
        {
            var result = m_Collection.Insert(_Item);
        }
        public void Add(IEnumerable<T_CollectionType> _Items)
        {
            m_Collection.InsertBatch(_Items);
        }
        public void Save(T_CollectionType _Item, MongoDB.Bson.ObjectId _ID)
        {
            _Item.Id = _ID;
        }
        //public void AddOrUpdate<T_CompareType>(T_CollectionType _Item, System.Linq.Expressions.Expression<Func<T_CollectionType, T_CompareType>> _Lambda, T_CompareType _CompareValue)
        //{
        //    m_Collection.find
        //    m_Collection.Update(MongoDB.Driver.Builders.Query<T_CollectionType>.EQ<T_CompareType>(_Lambda, _CompareValue), Update<T_CollectionType>.Replace(_Item));
        //}
    }
}
