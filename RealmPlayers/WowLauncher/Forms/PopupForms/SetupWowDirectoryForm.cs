using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VF_WoWLauncher
{
    public partial class SetupWowDirectoryForm : Form
    {
        public SetupWowDirectoryForm()
        {
            InitializeComponent();
            c_txtWowDirectory.TextChanged += c_txtWowDirectory_TextChanged;
        }

        void c_txtWowDirectory_TextChanged(object sender, EventArgs e)
        {
            if(c_txtWowDirectory.ForeColor != Color.Black)
                c_txtWowDirectory.ForeColor = Color.Black;
        }

        private void c_btnCancel_Click(object sender, EventArgs e)
        {
            if(Utility.MessageBoxShow("If Wow Directory is not known the Launcher will not work. Are you sure you want to Cancel and close the application?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                Close();
        }

        private void c_btnOK_Click(object sender, EventArgs e)
        {
            if (WowUtility.IsValidWowDirectory(c_txtWowDirectory.Text) == true)
            {
                if(WowUtility.IsWowDirectoryClassic(c_txtWowDirectory.Text) == true)
                {
                    Settings.Instance._WowDirectory = c_txtWowDirectory.Text;
                    if (Settings.Instance._WowDirectory.EndsWith("\\") == false && Settings.Instance._WowDirectory.EndsWith("/") == false)
                        Settings.Instance._WowDirectory += "\\";
                    Settings.Save();
                    Close();
                }
                else if(WowUtility.IsWowDirectoryTBC(c_txtWowDirectory.Text) == true)
                {
                    Utility.MessageBoxShow("Please note that this Launcher was originally made for WoW Classic and that some features may not work 100% for WoW TBC. \r\nPlease report any bugs and errors found on the forum at forum.realmplayers.com");
                    Settings.Instance._WowDirectory = c_txtWowDirectory.Text;
                    if (Settings.Instance._WowDirectory.EndsWith("\\") == false && Settings.Instance._WowDirectory.EndsWith("/") == false)
                        Settings.Instance._WowDirectory += "\\";
                    Settings.Instance._WowTBCDirectory = Settings.Instance._WowDirectory;
                    Settings.Save();
                    Close();
                }
                else
                {
                    Utility.MessageBoxShow(c_txtWowDirectory.Text + " is not a valid Wow Directory. \r\nPlease choose the correct directory where Wow is installed.");
                }
            }
            else
            {
                c_txtWowDirectory.ForeColor = Color.Red;
                Utility.MessageBoxShow(c_txtWowDirectory.Text + " is not a valid Wow Directory. \r\nPlease choose the correct directory where Wow is installed.");
            }
        }

        private void c_btnBrowse_Click(object sender, EventArgs e)
        {
            VF.FolderSelectDialog folderSelectDialog = new VF.FolderSelectDialog();
            folderSelectDialog.Title = "Please find the WoW Classic(or TBC) Directory for me";
            folderSelectDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (folderSelectDialog.ShowDialog() == true)
            {
                c_txtWowDirectory.Text = folderSelectDialog.FileName;
                if (WowUtility.IsValidWowDirectory(c_txtWowDirectory.Text) == false)
                {
                    c_txtWowDirectory.ForeColor = Color.Red;
                    Utility.MessageBoxShow(c_txtWowDirectory.Text + " is not a valid Wow Directory. \r\nPlease choose the correct directory where Wow is installed.");
                }
                else
                    c_txtWowDirectory.ForeColor = Color.Black;
            }
        }

        private void SetupWowDirectoryForm_Load(object sender, EventArgs e)
        {
            this.Top = Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
            this.Left = Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
        }
    }
    public class SetupWowDirectory
    {
        public static bool ShowSetupWowDirectory()
        {
            string previousWowDirectory = Settings.GetWowDirectory(WowVersion.Vanilla);
            SetupWowDirectoryForm setupWowDirectoryForm = new SetupWowDirectoryForm();
            setupWowDirectoryForm.ShowDialog();
            return (previousWowDirectory != Settings.GetWowDirectory(WowVersion.Vanilla)); //True om WowDirectory förändrats
        }
    }
}
