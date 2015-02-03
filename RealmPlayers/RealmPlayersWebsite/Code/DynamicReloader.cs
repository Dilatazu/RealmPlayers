using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

using MultithreadTask = System.Threading.Tasks.Task;

namespace RealmPlayersServer
{
    public class DynamicReloader
    {
        private class DataHolder
        {
            public DateTime m_LastOutdateCheckDateTime = DateTime.MinValue;
            public DateTime m_LastLoadTime = DateTime.MinValue;
            private object m_DataPriv = null;
            public object m_LockObject = new object();
            public MultithreadTask m_LoadTask = null;
            public bool m_DataLoaded = false;

            public object m_Data
            {
                get { return m_DataPriv; }
            }
            public bool IsDataLoaded()
            {
                return m_DataLoaded;
            }

            public DataHolder()
            { }
            public void CreateLoadTask<T>(Func<T> _LoadFunction)
            {
                if (m_LoadTask == null)
                {
                    m_LoadTask = new MultithreadTask(() =>
                    {
                        T result = default(T);
                        try
                        {
                            result = _LoadFunction();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                        lock (m_LockObject)
                        {
                            if (m_DataPriv == null || result != null)
                            {
                                m_DataPriv = result;
                                m_LastOutdateCheckDateTime = DateTime.UtcNow;
                                m_LastLoadTime = DateTime.UtcNow;
                            }
                            m_LoadTask = null;
                            m_DataLoaded = true;
                        }
                    });
                    m_LoadTask.Start();
                }
            }
        }

        private static Dictionary<Guid, DataHolder> m_Data = new Dictionary<Guid, DataHolder>();
        private static object m_LockObject = new object();
        public static T GetData<T>(Func<T> _LoadFunction, Func<T, DateTime, bool> _IsOutdated, TimeSpan? _CheckOutdatedEvery = null, bool _WaitUntilLoaded = true)
        {
            if (_CheckOutdatedEvery.HasValue == false)
                _CheckOutdatedEvery = new TimeSpan(0, 0, 5, 0);//5 min

            Guid guid = typeof(T).GUID;
            DataHolder dataHolder = null;
            lock(m_LockObject)
            {
                if (m_Data.TryGetValue(guid, out dataHolder) == false)
                {
                    dataHolder = new DataHolder();
                    m_Data.Add(guid, dataHolder);
                }
            }

            if (dataHolder.m_Data == null && _WaitUntilLoaded == false)
            {//Either Locking or returning, no other outcome

                if (Monitor.TryEnter(dataHolder.m_LockObject, 100) == false)
                    return default(T);//null
            }
            else
            {//Locking
                Monitor.Enter(dataHolder.m_LockObject);
            }
            /////////////////////////dataHolder.m_LockObject is LOCKED/////////////////////////
            object returnData = dataHolder.m_Data;
            if (returnData == null)
            {
                dataHolder.CreateLoadTask(_LoadFunction);
                if (_WaitUntilLoaded == false)
                {
                    Monitor.Exit(dataHolder.m_LockObject);
                    return default(T);//null
                }
                while (returnData == null)
                {
                    Monitor.Exit(dataHolder.m_LockObject);
                    System.Threading.Thread.Sleep(100);
                    Monitor.Enter(dataHolder.m_LockObject);
                    returnData = dataHolder.m_Data;
                    if (dataHolder.IsDataLoaded() == true && returnData == null)
                    {
                        Monitor.Exit(dataHolder.m_LockObject);
                        return default(T);//null
                    }
                }
            }
            else if ((DateTime.UtcNow - dataHolder.m_LastOutdateCheckDateTime) > _CheckOutdatedEvery.Value)
            {
                if (dataHolder.m_LoadTask == null)
                {
                    if (_IsOutdated((T)dataHolder.m_Data, dataHolder.m_LastLoadTime) == true)
                    {
                        dataHolder.CreateLoadTask(_LoadFunction);
                    }
                    else
                    {
                        dataHolder.m_LastOutdateCheckDateTime = dataHolder.m_LastOutdateCheckDateTime.AddMinutes(1);
                    }
                }
            }
            /////////////////////////dataHolder.m_LockObject is LOCKED/////////////////////////
            Monitor.Exit(dataHolder.m_LockObject);

            return (T)returnData;
        }
        public static DateTime GetLastLoadTime<T>()
        {
            Guid guid = typeof(T).GUID;
            DataHolder dataHolder = null;
            lock(m_LockObject)
            {
                if (m_Data.TryGetValue(guid, out dataHolder) == false)
                    dataHolder = null;
            }
            if (dataHolder != null)
                return dataHolder.m_LastLoadTime;
            else
                return DateTime.MinValue;
        }
    }
}