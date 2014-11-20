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
    }
}
