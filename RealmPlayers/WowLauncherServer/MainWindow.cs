using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using VF_RealmPlayersDatabase;

namespace VF_WoWLauncherServer
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            this.FormClosed += MainWindow_FormClosed;
        }
        Communication m_Comm = null;
        void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_Comm.Close();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            m_Comm = new Communication();

            c_lsContributors.ContextMenu = new System.Windows.Forms.ContextMenu();
            var copyMenu = c_lsContributors.ContextMenu.MenuItems.Add("Copy");
            copyMenu.Click += c_lsContributors_CopyMenu_Click;
            c_lsContributors.MouseMove += c_lsContributors_MouseMove;
            UpdateContributorsList();

            ToggleLockedButtons();
            AddonUpdates.LoadAddonBetaInfo();
            foreach (var betaParticipant in AddonUpdates.GetBetaAddonInfo())
            {
                if (betaParticipant.Value == null)
                    continue;
                foreach (var betaUser in betaParticipant.Value)
                {
                    c_lsBetaUsers.Items.Add(betaParticipant.Key + " - " + betaUser);
                }
            }
        }

        void c_lsContributors_MouseMove(object sender, MouseEventArgs e)
        {
            c_lsContributors.Focus();
            if (c_lsContributors.Items.Count > 0)
            {
                int selItemIndex = c_lsContributors.IndexFromPoint(e.Location);
                if (selItemIndex >= 0)
                    c_lsContributors.SelectedIndex = selItemIndex;
            }
        }

        void c_lsContributors_CopyMenu_Click(object sender, EventArgs e)
        {
            if (c_lsContributors.SelectedItem != null)
            {
                try
                {
                    Clipboard.SetText((string)c_lsContributors.SelectedItem);
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }
        }

        private void UpdateContributorsList()
        {
            var contributors = ContributorDB.GetAllTrustWorthyContributors();

            var contributorsSorted = contributors.OrderByDescending((_Value) => _Value.ContributorID);
            c_lsContributors.Items.Clear();
            foreach (var contributor in contributorsSorted)
            {
                c_lsContributors.Items.Add(contributor.Key);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if(fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddonUpdates.AddAddonUpdate(fileDialog.FileName);
            }
        }

        private void c_btnAddContributor_Click(object sender, EventArgs e)
        {
            string userID = "";
            if (ContributorUtility.GenerateUserID(c_txtAddContributorName.Text, out userID) == true)
            {
                if (ContributorDB.AddVIPContributor(userID, "***REMOVED***") == true)
                {
                    UpdateContributorsList();
                }
                else
                    MessageBox.Show("UserID could not be generated from \"" + c_txtAddContributorName.Text + "\", Username allready exists!");
            }
            else
                MessageBox.Show("UserID could not be generated from \"" + c_txtAddContributorName.Text + "\"");
        }

        private void c_lsContributors_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                Program.g_RDDatabaseHandler.CreateSummaryDatabase();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Program.g_RPPDatabaseHandler.CreateGuildSummaryDatabase();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Program.g_RPPDatabaseHandler.CreateItemSummaryDatabase();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Program.g_RPPDatabaseHandler.UpdatePlayerSummaryDatabase(true);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                Program.g_RDDatabaseHandler.FixBuggedSummaryDatabase();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Program.g_RPPDatabaseHandler.ResetArchangel();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                Program.g_RPPDatabaseHandler.PortToMongoDB();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void ToggleLockedButtons()
        {
            foreach (Control button in this.Controls)
            {
                if (button.GetType() != typeof(Button))
                    continue;
                if (button == button8)
                    continue;
                if (button.Enabled == true)
                    button.Enabled = false;
                else
                    button.Enabled = true;
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            ToggleLockedButtons();
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string betaAddon = c_txtBetaAddon.Text;
            string betaUser = c_txtBetaUserID.Text;
            if(AddonUpdates.AddBetaParticipant(betaAddon, betaUser) == true)
            {
                c_lsBetaUsers.Items.Add(betaAddon + " - " + betaUser);
            }
        }

        private void c_txtBetaAddon_TextChanged(object sender, EventArgs e)
        {

        }

        private void c_txtBetaUserID_TextChanged(object sender, EventArgs e)
        {

        }

        private void c_lsBetaUsers_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void c_lsBetaUsers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (c_lsBetaUsers.SelectedItem != null && c_lsBetaUsers.SelectedItem.GetType() == typeof(string))
            {
                var splitValues = ((string)c_lsBetaUsers.SelectedItem).SplitVF(" - ");
                if(AddonUpdates.RemoveBetaParticipant(splitValues[0], splitValues[1]) == true)
                {
                    c_lsBetaUsers.Items.Remove(c_lsBetaUsers.SelectedItem);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            /*
             * //SULTANEN NOSTALRIUS PURGE
            {
                UploadID uploadID = new UploadID(245, new DateTime(635608414529980000L, DateTimeKind.Local));
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Nostalrius).PurgeGearContribution("Sultanen", uploadID);
            }
            {
                UploadID uploadID = new UploadID(245, new DateTime(635608414529990000L, DateTimeKind.Local));
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Nostalrius).PurgeGearContribution("Sultanen", uploadID);
            }
            {
                UploadID uploadID = new UploadID(245, new DateTime(635608414530000000L, DateTimeKind.Local));
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Nostalrius).PurgeGearContribution("Sultanen", uploadID);
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Nostalrius).PurgeGearContribution("Sultanen", uploadID);//Do this command twice since there are 2 entries
            }
            {
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Nostalrius).PurgeExtraDataBefore("Sultanen", new DateTime(2015, 3, 8));
            }*/

            //ROOBY, PREDICTABLE, SLEZMINATOR WARSONG PTR PURGE
            {
                UploadID uploadID = new UploadID(192, new DateTime(635630003200000000L, DateTimeKind.Local));
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Warsong).PurgeGearContribution("Rooby", uploadID);
            }
            {
                UploadID uploadID = new UploadID(192, new DateTime(635629925260000000L, DateTimeKind.Local));
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Warsong).PurgeGearContribution("Predictable", uploadID);
            }
            {
                UploadID uploadID = new UploadID(192, new DateTime(635630000780000000L, DateTimeKind.Local));
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Warsong).PurgeGearContribution("Predictable", uploadID);
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Warsong).PurgeGearContribution("Predictable", uploadID);//Do this command twice since there are 2 entries
            }
            {
                UploadID uploadID = new UploadID(192, new DateTime(635629901570000000L, DateTimeKind.Local));
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Warsong).PurgeGearContribution("Slezminator", uploadID);
            }
            {
                UploadID uploadID = new UploadID(192, new DateTime(635629923940000000L, DateTimeKind.Local));
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Warsong).PurgeGearContribution("Casiie", uploadID);
            }
            {
                UploadID uploadID = new UploadID(192, new DateTime(635629924730000000L, DateTimeKind.Local));
                Program.g_RPPDatabaseHandler.GetRealmDB(WowRealm.Warsong).PurgeGearContribution("Opilol", uploadID);
            }
            Program.g_RPPDatabaseHandler.TriggerSaveDatabases();
        }
    }
}
