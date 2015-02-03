using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace RealmPlayersServer
{
    public class DynamicFileData
    {
        static readonly DynamicFileData sm_Instance = new DynamicFileData();

        Dictionary<string, Tuple<string, DateTime>> m_TextFiles = new Dictionary<string, Tuple<string, DateTime>>();
        public static string GetTextFile(string _Path, TimeSpan? _ValidTime = null)
        {
            TimeSpan validTime = new TimeSpan(1, 0, 0);
            if (_ValidTime.HasValue == true)
                validTime = _ValidTime.Value;

            string retString = "";

            Monitor.Enter(sm_Instance);
            if (sm_Instance.m_TextFiles.ContainsKey(_Path) == true)
            {
                if (DateTime.UtcNow > sm_Instance.m_TextFiles[_Path].Item2.Add(validTime))
                {
                    Monitor.Exit(sm_Instance);
                    try
                    {
                        retString = System.IO.File.ReadAllText(_Path);
                    }
                    catch (Exception)
                    {
                        retString = "";
                    }
                    if (retString != "")
                    {
                        lock (sm_Instance)
                        {
                            sm_Instance.m_TextFiles[_Path] = new Tuple<string, DateTime>(retString, DateTime.UtcNow);
                        }
                    }
                }
                else
                {
                    retString = sm_Instance.m_TextFiles[_Path].Item1;
                    Monitor.Exit(sm_Instance);
                }
            }
            else
            {
                Monitor.Exit(sm_Instance);
                try
                {
                    retString = System.IO.File.ReadAllText(_Path);
                }
                catch (Exception)
                {
                    retString = "";
                }
                if (retString != "")
                {
                    lock(sm_Instance)
                    {
                        if (sm_Instance.m_TextFiles.ContainsKey(_Path) == false)
                            sm_Instance.m_TextFiles.Add(_Path, new Tuple<string, DateTime>(retString, DateTime.UtcNow));
                    }
                }
            }
            return retString;
        }
    }
}