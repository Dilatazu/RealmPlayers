using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VF_RealmPlayersDatabase;
using VF_RealmPlayersDatabase.PlayerData;
using UploaderCommunication = VF_RealmPlayersDatabase.UploaderCommunication;

using System.Collections.Concurrent;

namespace MongoDBPorting
{
    public partial class Form1 : Form
    {
        VF.MongoDatabase m_Database;
        public Form1()
        {
            InitializeComponent();

            VF_WoWLauncher.ConsoleUtility.CreateConsole();

        }

        private void MigrateRealmDBs_Click(object sender, EventArgs e)
        {
            throw new Exception("Not implemented fully yet!");

            var timer = System.Diagnostics.Stopwatch.StartNew();
            Database tempDatabase = new Database("R:\\VF_RealmPlayersData\\RPPDatabase\\Database\\");
            //tempDatabase.PurgeRealmDBs(true, true);
            foreach (var realm in tempDatabase.GetRealms())
            {
                while (realm.Value.IsLoadComplete() == false)
                {
                    System.Threading.Thread.Sleep(100);
                    Console.Write(".");
                }
            }
            Logger.ConsoleWriteLine("Done Loading Databases, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
            
            byte[] buffer = new byte[10000];
            foreach (var realm in tempDatabase.GetRealms())
            {
                Logger.ConsoleWriteLine("Uploading data for Realm: " + realm.Key.ToString());
                var playerDatabase = m_Database.GetCollection<PlayerDBElement>("PlayerData_" + realm.Key.ToString());
                if(playerDatabase.MongoDBCollection.Count() > 0)
                {
                    Logger.ConsoleWriteLine("Error, data is already uploaded for this Realm! Not uploading any data!", ConsoleColor.Red);
                    continue;
                }
                List<PlayerDBElement> realmElements = new List<PlayerDBElement>();
                System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);
                foreach(var player in realm.Value.m_Players)
                {
                    realmElements.Add(new PlayerDBElement(stream, player.Value));
                }
                Logger.ConsoleWriteLine("Done generating data, Now uploading to MongoDB");
                playerDatabase.Add(realmElements);
            }

            Logger.ConsoleWriteLine("Done Uploading to MongoDB, it took " + (timer.ElapsedMilliseconds / 1000) + " seconds", ConsoleColor.Green);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //test
        }

        private void MigrateContributors_Click(object sender, EventArgs e)
        {
            try
            {
                VF_RealmPlayersDatabase.Deprecated.ContributorHandler.Initialize("R:\\VF_RealmPlayersData\\RPPDatabase\\Database\\");
                var contributors = VF_RealmPlayersDatabase.Deprecated.ContributorHandler.GetContributorsCopy();
                ContributorDB.MigrateFromProtobufDB(VF_RealmPlayersDatabase.Deprecated.ContributorHandler.Getsm_CH(), contributors);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(MessageBox.Show("Are you sure you want to reset the database? This will delete everything!", "Are you really sure?", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (MessageBox.Show("I mean for real, this is not possible to revert.\n\n\nDO NOT DO THIS UNLESS YOU ARE SURE, MAKE SURE YOU THINK THIS THROUGH!!!\n\n\n\n\n\nARE YOU ABSOLUTELY 100% SURE YOU WANT TO DELETE THE DATABASE?", "WARNING DO NOT DO THIS!", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                    {
                        RealmPlayersDB.GetInstance().DropCollection("Contributors");
                        RealmPlayersDB.GetInstance().DropCollection("ContributorsMeta");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
    class PlayerDBElement : VF.MongoDBItem
    {
        public string Name { get; set; }
        public byte[] SerializedPlayerData { get; set; }
        public PlayerDBElement(System.IO.MemoryStream _BufferStream, Player _Player)
        {
            Name = _Player.Name;
            _BufferStream.SetLength(0);
            VF.Utility.PBSerialize(_Player, _BufferStream);
            SerializedPlayerData = new byte[_BufferStream.Length];
            _BufferStream.Position = 0;
            _BufferStream.Read(SerializedPlayerData, 0, (int)_BufferStream.Length);
        }
        public PlayerDBElement() { }

    }
}
