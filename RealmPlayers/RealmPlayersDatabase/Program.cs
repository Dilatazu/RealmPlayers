using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace VF_RealmPlayersDatabase
{
    class Program
    {
        static Database m_Database = null;
        static UploaderCommunication.RPPCommunicator m_Communicator = null;
        static List<RPPContribution> m_AddedContributions = new List<RPPContribution>();
        static void Main(string[] args)
        {
            try
            {
                if (ItemDropDatabase.DatabaseExists("Database\\") == false)
                {
                    ItemDropDatabase itemDropDatabase = new ItemDropDatabase("Database\\");
                }
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                m_Communicator = new UploaderCommunication.RPPCommunicator(18374);
                m_Database = new Database("Database\\");
                while (m_Communicator != null)
                {
                    ProcessData();
                    m_Database.Cleanup();
                    System.Threading.Thread.Sleep(30000);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            m_Communicator.Shutdown();
            ProcessData();
            m_Communicator.WaitForClosed();
            ProcessData();
            m_Communicator = null;
        }
        static void ProcessData()
        {
            m_AddedContributions.Clear();
            RPPContribution data;
            while (m_Communicator.GetNextRPPContribution(out data))
            {
                m_Database.AddContribution(data);
                m_AddedContributions.Add(data);
            }
            if (m_AddedContributions.Count > 0)
            {
                m_Database.SaveRealmDatabases("Database\\");
            }
            foreach (RPPContribution contribution in m_AddedContributions)
            {
                BackupRPPContribution(contribution.GetFilename());
            }
        }
        static void BackupRPPContribution(string _Filename)
        {
            if (System.IO.File.Exists(_Filename) == false)
            {
                Logger.ConsoleWriteLine("Could not backup file: " + _Filename + ", it does not exist!");
                return;
            }
            if (System.IO.Directory.Exists("RPPContributionsBackup") == false)
                System.IO.Directory.CreateDirectory("RPPContributionsBackup");
            string zipFileName = "RPPContributionsBackup\\RPPContributions_" + DateTime.Now.ToString("yyyy_MM_dd") + ".zip";
            ZipFile zipFile;
            if (System.IO.File.Exists(zipFileName) == true)
                zipFile = new ZipFile(zipFileName);
            else
                zipFile = ZipFile.Create(zipFileName);

            zipFile.BeginUpdate();

            zipFile.Add(_Filename, _Filename.Split('\\', '/').Last());

            zipFile.CommitUpdate();
            zipFile.Close();
            System.IO.File.Delete(_Filename);
            Logger.ConsoleWriteLine("Successfull backup of file: " + _Filename + " into " + zipFileName);
        }
    }
}
