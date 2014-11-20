using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase.UploaderCommunication
{
    class AddonHandler
    {
        static Dictionary<string, string> m_LatestAddonFilesEncoded = new Dictionary<string, string>();
        static Dictionary<int, DateTime> m_LastAddonUpdateAttemptForContributor = new Dictionary<int, DateTime>(); 
        public static string CreateAddonUpdate(Contributor _Contributor, Single _CurrentAddonVersion)
        {
            if (_CurrentAddonVersion >= GetLatestAddonVersion())
                return "";

            DateTime lastAddonUpdateAttempt = DateTime.Now.AddHours(-10);
            if (m_LastAddonUpdateAttemptForContributor.ContainsKey(_Contributor.GetContributorID()) == true)
                lastAddonUpdateAttempt = m_LastAddonUpdateAttemptForContributor[_Contributor.GetContributorID()];

            string addonUpdateResponse = "";
            if ((DateTime.Now - lastAddonUpdateAttempt).TotalHours > 1)
            {
                if (GetLatestAddonVersion() >= 1.3f)
                {
                    addonUpdateResponse += AddAddonFile("VF_RealmPlayers.lua");
                    addonUpdateResponse += AddAddonFile("VF_RealmPlayers.toc");
                }
                m_LastAddonUpdateAttemptForContributor[_Contributor.GetContributorID()] = DateTime.Now;
            }
            Logger.ConsoleWriteLine(_Contributor.Name + " is updating from AddonVersion " + _CurrentAddonVersion + " to " + GetLatestAddonVersion(), ConsoleColor.Cyan);
            return addonUpdateResponse;
        }
        private static string AddAddonFile(string _Filename)
        {
            if (m_LatestAddonFilesEncoded.ContainsKey(_Filename) == false)
            {
                if (System.IO.File.Exists("AddonFiles\\" + _Filename))
                {
                    m_LatestAddonFilesEncoded.Add(_Filename,
                        System.IO.File.ReadAllText("AddonFiles\\" + _Filename).Replace("å", "&AOU&").Replace("ä", "&AE&").Replace(';', 'å').Replace('=', 'ä'));
                }
            }
            if (m_LatestAddonFilesEncoded.ContainsKey(_Filename))
            {
                return "AddonUpdate-" + _Filename + "="
                    + m_LatestAddonFilesEncoded[_Filename] + ";";
            }
            return "";
        }
        public static Single GetLatestAddonVersion()
        {
            return 1.51f;
        }
    }
}
