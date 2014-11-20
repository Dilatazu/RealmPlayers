using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading;

namespace VF_RealmPlayersDatabase.Deprecated
{
    [XmlRoot("ContributorHandler")]
    public class ContributorHandler
    {
        SerializableDictionary<string, Contributor> m_Contributors = new SerializableDictionary<string, Contributor>();
        int m_VIPContributorIDCounter = 0;
        int m_ContributorIDCounter = Contributor.ContributorTrustworthyIDBound;
        object m_ThreadObject = new object();
        [XmlElement]
        public int VIPContributorIDCounter
        {
            get {return m_VIPContributorIDCounter;}
            set {m_VIPContributorIDCounter = value;}
        }
        [XmlElement]
        public int ContributorIDCounter
        {
            get { return m_ContributorIDCounter; }
            set { m_ContributorIDCounter = value; }
        }
        [XmlElement]
        public SerializableDictionary<string, Contributor> Contributors
        {
            get { return m_Contributors; }
            set { m_Contributors = value; }
        }
        static ContributorHandler sm_CH = null;
        public static ContributorHandler Getsm_CH()
        {
            return sm_CH;
        }
        public static int GetContributorCount()
        {
            return sm_CH.m_Contributors.Count;
        }
        public static int GetVIPContributorCount()
        {
            lock (sm_CH.m_ThreadObject)
            {
                int counter = 0;
                foreach (var contributor in sm_CH.m_Contributors)
                {
                    if (contributor.Value.IsVIP())
                        ++counter;
                }
                return counter;
            }
        }
        public static IDictionary<string, Contributor> GetContributors_Unsafe()
        {
            return sm_CH.m_Contributors;
        }
        public static Dictionary<string, Contributor> GetContributorsCopy()
        {
            Dictionary<string, Contributor> copy = null;
            lock (sm_CH.m_ThreadObject)
            {
                copy = new Dictionary<string, Contributor>(sm_CH.m_Contributors);
            }
            return copy;
        }
        public static Contributor _GetContributor(string _UserID)
        {
            lock (sm_CH.m_ThreadObject)
            {
                if (sm_CH.m_Contributors.ContainsKey(_UserID) == true)
                {
                    return sm_CH.m_Contributors[_UserID];
                }
            }
            return null;
        }
        public static Contributor GetContributor(string _UserID, System.Net.IPEndPoint _UserIP, bool _CreateIfNotExist = true)
        {
            return GetContributor(_UserID, _UserIP.Address, _CreateIfNotExist);
        }
        public static Contributor GetContributor(string _UserID, System.Net.IPAddress _UserIP, bool _CreateIfNotExist = true)
        {
            lock (sm_CH.m_ThreadObject)
            {
                string userIP = _UserIP.ToString();
                if (sm_CH.m_Contributors.ContainsKey(_UserID) == true)
                {
                    sm_CH.m_Contributors[_UserID].SetUserIP(_UserIP);
                    return sm_CH.m_Contributors[_UserID];
                }

                if (sm_CH.m_Contributors.ContainsKey(userIP) == false)
                {
                    if (_CreateIfNotExist == false)
                        return null;
                    sm_CH.m_Contributors.Add(userIP, new Contributor(++sm_CH.m_ContributorIDCounter, _UserIP));
                }

                return sm_CH.m_Contributors[userIP];
            }
        }
        public static void RemoveContributor(string _UserID)
        {
            lock (sm_CH.m_ThreadObject)
            {
                sm_CH.m_Contributors.Remove(_UserID);
            }
        }
        public enum CheckContributorResult
        {
            UserID_Failed_UserPass_Wrong,
            UserID_Failed_Check_Too_Many_Tries,
            UserID_Success_Login,
            UserID_Success_Registered,
        }
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
        Dictionary<string, ContributorCheckInfo> m_IPContributorChecks = new Dictionary<string, ContributorCheckInfo>();
        public static CheckContributorResult CheckContributor(string _UserID, System.Net.IPEndPoint _UserIP, bool _AutoRegister = false)
        {
            lock (sm_CH.m_ThreadObject)
            {
                string userIP = _UserIP.Address.ToString();
                if (sm_CH.m_IPContributorChecks.ContainsKey(userIP) == false)
                    sm_CH.m_IPContributorChecks.Add(userIP, new ContributorCheckInfo());

                if (sm_CH.m_IPContributorChecks[userIP].AllowedCheck(_UserID) == false)
                {
                    Logger.ConsoleWriteLine(userIP + " tried to check UserID(" + _UserID + "), too many tries", ConsoleColor.Red);
                    return CheckContributorResult.UserID_Failed_Check_Too_Many_Tries;
                }

                if (sm_CH.m_Contributors.ContainsKey(_UserID) == true)
                    return CheckContributorResult.UserID_Success_Login;

                string userName = _UserID.Split('.')[0];
                try
                {
                    var result = sm_CH.m_Contributors.First((_Value) => { return _Value.Key.StartsWith(userName + "."); }).Value;
                    //Om det inte throwar exception här så innebär det att UserName fanns, dvs UserID_Failed_UserPass_Wrong
                    return CheckContributorResult.UserID_Failed_UserPass_Wrong;
                }
                catch (Exception)
                {
                    //UserID existerar inte, registrera det
                    if (_AutoRegister == true)
                    {
                        sm_CH.m_Contributors.Add(_UserID, new Contributor(++sm_CH.m_ContributorIDCounter, _UserID));
                        return CheckContributorResult.UserID_Success_Registered;
                    }
                    else
                        return CheckContributorResult.UserID_Failed_UserPass_Wrong;
                }
            }
        }
        public static Contributor GetContributor(UploadID _Uploader)
        {
            lock (sm_CH.m_ThreadObject)
            {
                try
                {
                    if (_Uploader.IsNull())
                    {
                        return null;
                    }
                    else
                    {
                        return sm_CH.m_Contributors.Single((KeyValuePair<string, Contributor> _Args)
                            =>
                        {
                            return (_Args.Value.GetContributorID() == _Uploader.GetContributorID());
                        }).Value;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    return null;
                }
            }
        }
        public static bool AddVIPContributor(string _UserID)
        {
            lock (sm_CH.m_ThreadObject)
            {
                if (sm_CH.m_Contributors.ContainsKey(_UserID) == false)
                {
                    try
                    {
                        string userNameDot = _UserID.Split('.').First() + ".";
                        var result = sm_CH.m_Contributors.First((_Value) => { return _Value.Key.StartsWith(userNameDot); }).Value;
                        //Om det inte throwar exception här så innebär det att UserName fanns redan
                        return false;
                    }
                    catch (Exception)
                    {
                        //UserID existerar inte, registrera det
                        Contributor contributor = new Contributor(++sm_CH.m_VIPContributorIDCounter, _UserID);
                        sm_CH.m_Contributors.Add(_UserID, contributor);
                        return true;
                    }
                }
            }
            return true;
        }
        public static bool GenerateUserID(string _Username, out string _ReturnUserID)
        {
            _ReturnUserID = _Username;
            if (_Username.Contains('.'))
            {
                _ReturnUserID = _Username;
                return false;
            }
            _Username = _Username.Replace(" ", "");//Ta väck mellanslag
            if (_Username.Length < 3)
                return false;
            if (_Username[0] >= 'A' && _Username[0] <= 'Z')
            {
                //Första bokstaven måste vara stor bokstav
            }
            else
            {
                return false;
            }
            for (int i = 1; i < _Username.Length; ++i)
            {
                if (_Username[i] >= 'a' && _Username[i] <= 'z')
                {
                    //Enda tillåtna bokstäverna är från a-z i små bokstäver
                }
                else
                {
                    return false;
                }
            }
            string userID = _Username + ".";

            Random rand = new Random();
            for (int i = 0; i < 6; ++i)
                userID += rand.Next(0, 10); //Lägg till 6 random siffror

            if (userID.Length != _Username.Length + 7)
                return false;

            _ReturnUserID = userID;
            return true;
        }
        public static void LoadContributors()
        {
            //AddVIPContributor("***REMOVED***");
            //AddVIPContributor("***REMOVED***");
            //AddVIPContributor("***REMOVED***");
            //AddVIPContributor("***REMOVED***");
            //AddVIPContributor("***REMOVED***");
            //AddVIPContributor("***REMOVED***");
            //AddVIPContributor("***REMOVED***");
            //AddVIPContributor("***REMOVED***");
            //AddVIPContributor("***REMOVED***");
            //AddVIPContributor("***REMOVED***");
            try
            {
                if (System.IO.File.Exists("NewContributors.txt") == true)
                {
                    string[] newContributors = System.IO.File.ReadAllLines("NewContributors.txt");
                    foreach (string newContributor in newContributors)
                    {
                        AddVIPContributor(newContributor);
                    }
                }
            }
            catch (Exception)
            { }
        }
        public static void Initialize(string _RootPath)
        {
            if (System.IO.File.Exists(_RootPath + "Contributors.dat") == true)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContributorHandler));
                System.IO.TextReader textReader = new System.IO.StreamReader(_RootPath + "Contributors.dat");
                sm_CH = (ContributorHandler)xmlSerializer.Deserialize(textReader);
                textReader.Close();
                LoadContributors();
            }
            else
            {
                sm_CH = new ContributorHandler();
                LoadContributors();
                Save(_RootPath);
            }
        }
        public static void Save(string _RootPath)
        {
            lock (sm_CH.m_ThreadObject)
            {
                Utility.AssertDirectory(_RootPath);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContributorHandler));
                Utility.BackupFile(_RootPath + "Contributors.dat");
                System.IO.TextWriter textWriter = new System.IO.StreamWriter(_RootPath + "Contributors.dat");
                xmlSerializer.Serialize(textWriter, sm_CH);
                textWriter.Close();
            }
        }
    }
}
