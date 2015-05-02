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
    public partial class ApplicationSettingsForm : Form
    {
        public ApplicationSettingsForm()
        {
            InitializeComponent();
        }

        private void ApplicationSettingsForm_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);
            //this.TopMost = true;
            c_txtUserID.Text = Settings.UserID;
            c_txtWowDirectory.Text = Settings.GetWowDirectory(WowVersionEnum.Vanilla);
            c_cbAutoRefresh.Checked = Settings.Instance.AutoRefreshNews;
            c_cbxRunNotAdmin.Checked = Settings.Instance.RunWoWNotAdmin;
            c_cbxAutoHide.Checked = Settings.Instance.AutoHideOldNews;
            c_cbAutoUpdateVF.Checked = Settings.Instance.AutoUpdateVFAddons;
            c_cbxFeenixNews.Checked = Settings.Instance.NewsSources_Feenix;
            c_cbxNostalriusNews.Checked = Settings.Instance.NewsSources_Nostalrius;
            c_cbxKronosNews.Checked = Settings.Instance.NewsSources_Kronos;
            //c_cbxWoWNoDelay.Checked = Settings.Instance.UseWoWNoDelay;
            if (Settings.HaveTBC == true)
            {
                c_cbxEnableTBC.Checked = true;
                c_txtTBCWowDirectory.Enabled = true;
                c_txtTBCWowDirectory.Text = Settings.GetWowDirectory(WowVersionEnum.TBC);
                c_btnTBCWowDirectoryBrowse.Enabled = true;
            }
            else
            {
                c_cbxEnableTBC.Checked = false;
                c_txtTBCWowDirectory.Text = "";
                c_txtTBCWowDirectory.Enabled = false;
                c_btnTBCWowDirectoryBrowse.Enabled = false;
            }
        }

        private void c_btnChangeUserID_Click(object sender, EventArgs e)
        {
            if (RealmPlayersUploader.IsValidUserID(Settings.UserID) == false)
            {
                SetupUserID.ShowSetupUserID();
                c_txtUserID.Text = Settings.UserID;
            }
            else
            {
                var result = Utility.MessageBoxShow("Are you sure you want to change your UserID? A UserID is unique and should only be used by 1 person. There are very few reasons to ever change this UserID. Do you have a valid reason and are sure you want to change the UserID?", "Are you sure?", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    SetupUserID.ShowSetupUserID();
                    c_txtUserID.Text = Settings.UserID;
                }
            }
        }

        private void c_btnClose_Click(object sender, EventArgs e)
        {
            if (WowUtility.IsValidWowDirectory(c_txtWowDirectory.Text) == false)
            {
                var dialogResult = Utility.MessageBoxShow("The specified WoW Classic Directory is not valid. Only valid WoW Classic Directory can be used. Use the old specified \"" + Settings.GetWowDirectory(WowVersionEnum.Vanilla) + "\"?", "Not valid Wow Directory", MessageBoxButtons.YesNo);
                if (dialogResult == System.Windows.Forms.DialogResult.No)
                {
                    c_txtWowDirectory.ForeColor = Color.Red;
                }
                else//if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    c_txtWowDirectory.Text = Settings.GetWowDirectory(WowVersionEnum.Vanilla);
                }
                return;
            }
            else
            {
                Settings.Instance._WowDirectory = c_txtWowDirectory.Text;
            }
            if (c_cbxEnableTBC.Checked == true)
            {
                if (WowUtility.IsValidWowDirectory(c_txtTBCWowDirectory.Text) == false)
                {
                    if (Settings.HaveTBC == true)
                    {
                        var dialogResult = Utility.MessageBoxShow("The specified WoW The Burning Crusade Directory is not valid. Only valid WoW TBC Directory can be used. Use the old specified \"" + Settings.GetWowDirectory(WowVersionEnum.TBC) + "\"?", "Not valid Wow Directory", MessageBoxButtons.YesNo);
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            c_txtTBCWowDirectory.ForeColor = Color.Red;
                        }
                        else//if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            c_txtTBCWowDirectory.Text = Settings.GetWowDirectory(WowVersionEnum.TBC);
                        }
                        return;
                    }
                    else
                    {
                        var dialogResult = Utility.MessageBoxShow("The specified WoW The Burning Crusade Directory is not valid. Only valid WoW TBC Directory can be used. Disable the use of WoW TBC Directory?", "Not valid Wow Directory", MessageBoxButtons.YesNo);
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            c_txtTBCWowDirectory.ForeColor = Color.Red;
                        }
                        else//if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            c_txtTBCWowDirectory.ForeColor = Color.Black;
                            c_cbxEnableTBC.Checked = false;
                            c_txtTBCWowDirectory.Text = "";
                        }
                        return;
                    }
                }
                else
                {
                    Settings.Instance._WowTBCDirectory = c_txtTBCWowDirectory.Text;
                }
            }
            else
            {
                Settings.Instance._WowTBCDirectory = "";
            }

            Settings.Instance.RunWoWNotAdmin = c_cbxRunNotAdmin.Checked;
            Settings.Instance.AutoHideOldNews = c_cbxAutoHide.Checked;
            Settings.Instance.AutoUpdateVFAddons = c_cbAutoUpdateVF.Checked;
            //Settings.Instance.UseWoWNoDelay = c_cbxWoWNoDelay.Checked;
            Settings.Instance.AutoRefreshNews = c_cbAutoRefresh.Checked;
            Settings.Instance.NewsSources_Feenix = c_cbxFeenixNews.Checked;
            Settings.Instance.NewsSources_Nostalrius = c_cbxNostalriusNews.Checked;
            Settings.Instance.NewsSources_Kronos = c_cbxKronosNews.Checked;
            Close();
        }

        private void c_btnWowDirectoryBrowse_Click(object sender, EventArgs e)
        {
            VF.FolderSelectDialog folderSelectDialog = new VF.FolderSelectDialog();
            folderSelectDialog.Title = "Please find the WoW Classic Directory for me";
            folderSelectDialog.InitialDirectory = Settings.GetWowDirectory(WowVersionEnum.Vanilla);
            if (folderSelectDialog.ShowDialog() == true)
            {
                c_txtWowDirectory.Text = folderSelectDialog.FileName;
                if (c_txtWowDirectory.Text.EndsWith("\\") == false && c_txtWowDirectory.Text.EndsWith("/") == false)
                    c_txtWowDirectory.Text += "\\";
                if (WowUtility.IsValidWowDirectory(c_txtWowDirectory.Text) == false)
                {
                    c_txtWowDirectory.ForeColor = Color.Red;
                    Utility.MessageBoxShow(c_txtWowDirectory.Text + " is not a valid WoW Classic Directory. \r\nPlease choose the correct directory where WoW is installed.");
                }
                else
                    c_txtWowDirectory.ForeColor = Color.Black;
            }
        }

        private void c_txtWowDirectory_TextChanged(object sender, EventArgs e)
        {
            if (c_txtWowDirectory.ForeColor != Color.Black)
                c_txtWowDirectory.ForeColor = Color.Black;
        }

        private void c_cbxWoWNoDelay_CheckedChanged(object sender, EventArgs e)
        {
            /*if (c_cbxWoWNoDelay.Checked == true)
            {
                if (System.IO.File.Exists(Settings.WowDirectory + "VF_WoWNoDelay.exe") == false)
                {
                    if (Utility.MessageBoxShow("This feature require you to download a patched version of WoW.exe made by Dilatazu in order to use. The new patched WoW.exe will be named \"VF_WoWNoDelay.exe\" it is hardcoded to connect to feenix realms(changing realmlist in \"realmlist.wtf\" wont work with this version). The old WoW.exe will still exist for you when you want/need to change back. \r\nDo you want to download this patched WoW.exe file and use it?", "", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                    {
                        c_cbxWoWNoDelay.Enabled = false;
                        if (System.IO.File.Exists(Settings.WowDirectory + "VF_WoWNoDelay.exe.downloading") == true)
                        {
                            Utility.DeleteFile(Settings.WowDirectory + "VF_WoWNoDelay.exe.downloading");
                        }
                        (new System.Threading.Tasks.Task(() =>
                        {
                            try
                            {
                                try
                                {
                                    FTPConnection ftpConnection = new FTPConnection("realmplayers.com:5511", "", "");
                                    ftpConnection.download("VF_WoWNoDelay.exe", Settings.WowDirectory + "VF_WoWNoDelay.exe.downloading");
                                    System.IO.File.Move(Settings.WowDirectory + "VF_WoWNoDelay.exe.downloading", Settings.WowDirectory + "VF_WoWNoDelay.exe");
                                }
                                catch (Exception ex)
                                {
                                    c_cbxWoWNoDelay.BeginInvoke(new Action(() =>
                                    {
                                        c_cbxWoWNoDelay.Checked = false;
                                        c_cbxWoWNoDelay.Enabled = true;
                                    }));
                                    Logger.LogException(ex);
                                }
                                if (System.IO.File.Exists(Settings.WowDirectory + "VF_WoWNoDelay.exe.downloading") == true)
                                {
                                    Utility.DeleteFile(Settings.WowDirectory + "VF_WoWNoDelay.exe.downloading");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogException(ex);
                            }
                            c_cbxWoWNoDelay.BeginInvoke(new Action(() =>
                            {
                                c_cbxWoWNoDelay.Enabled = true;
                            }));
                        })).Start();
                    }
                    else
                    {
                        c_cbxWoWNoDelay.Checked = false;
                        Utility.MessageBoxShow("Since \"VF_WoWNoDelay.exe\" does not exist this option can not be enabled");
                    }
                }
            }*/
        }

        private void c_cbxEnableTBC_CheckedChanged(object sender, EventArgs e)
        {
            c_txtTBCWowDirectory.Enabled = c_cbxEnableTBC.Checked;
            c_btnTBCWowDirectoryBrowse.Enabled = c_cbxEnableTBC.Checked;
            if (Settings.HaveTBC == false && c_cbxEnableTBC.Checked == true)
            {
                Utility.MessageBoxShow("Please note that this Launcher is originally made for WoW Classic and that some features may not work 100% for WoW TBC. \r\nPlease report any bugs and errors found on the forum at forum.realmplayers.com");
            }
        }

        private void c_btnTBCWowDirectoryBrowse_Click(object sender, EventArgs e)
        {
            VF.FolderSelectDialog folderSelectDialog = new VF.FolderSelectDialog();
            folderSelectDialog.Title = "Please find the WoW The Burning Crusade Directory for me";
            folderSelectDialog.InitialDirectory = Settings.GetWowDirectory(WowVersionEnum.TBC);
            if (folderSelectDialog.ShowDialog() == true)
            {
                c_txtTBCWowDirectory.Text = folderSelectDialog.FileName;
                if (c_txtTBCWowDirectory.Text.EndsWith("\\") == false && c_txtTBCWowDirectory.Text.EndsWith("/") == false)
                    c_txtTBCWowDirectory.Text += "\\";
                if (WowUtility.IsValidWowDirectory(c_txtTBCWowDirectory.Text) == false)
                {
                    c_txtTBCWowDirectory.ForeColor = Color.Red;
                    Utility.MessageBoxShow(c_txtTBCWowDirectory.Text + " is not a valid WoW The Burning Crusade Directory. \r\nPlease choose the correct directory where WoW TBC is installed.");
                }
                else
                {
                    if(WowUtility.IsWowDirectoryTBC(c_txtTBCWowDirectory.Text) == true)
                    {
                        c_txtTBCWowDirectory.ForeColor = Color.Black;
                    }
                    else
                    {
                        c_txtTBCWowDirectory.ForeColor = Color.Red;
                        Utility.MessageBoxShow(c_txtTBCWowDirectory.Text + " is not a valid WoW The Burning Crusade Directory. \r\nPlease choose the correct directory where WoW TBC is installed.");
                    }
                }
            }
        }

        private void c_txtTBCWowDirectory_TextChanged(object sender, EventArgs e)
        {
            if (c_txtTBCWowDirectory.ForeColor != Color.Black)
                c_txtTBCWowDirectory.ForeColor = Color.Black;
        }

        private void c_cbAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void c_cbxRunNotAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Instance.RunWoWNotAdmin == false && c_cbxRunNotAdmin.Checked == true)
            {
                if (Utility.MessageBoxShow("Please note that this feature is only recommended if you have previously experienced issues running wow as admin\r\nAre you sure you want to use this feature?", "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    c_cbxRunNotAdmin.Checked = false;
                }
            }
        }

        private void c_cbxAutoHide_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void c_cbAutoUpdateVF_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void c_cbxGetFeenixNews_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void c_cbxNostalriusNews_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
    public class ApplicationSettings
    {
        public static void ShowApplicationSettings()
        {
            ApplicationSettingsForm appSettingsForm = new ApplicationSettingsForm();
            appSettingsForm.ShowDialog();
        }
    }
}
