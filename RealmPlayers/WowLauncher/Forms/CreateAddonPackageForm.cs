using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace VF_WoWLauncher
{
    public partial class CreateAddonPackageForm : Form
    {
        public CreateAddonPackageForm()
        {
            InitializeComponent();
        }

        private void CreateAddonPackageForm_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);

            c_cbUpdateImportance.Items.AddRange(Enum.GetNames(typeof(ServerComm.UpdateImportance)));
            c_cbUpdateImportance.SelectedItem = ServerComm.UpdateImportance.Good.ToString();

            //this.TopMost = true;
            if (RealmPlayersUploader.IsValidUserID(Settings.UserID) == false)
            {
                Utility.MessageBoxShow("You must have a valid UserID to be able to create AddonPackages");
                Close();
            }
        }

        private void c_btnBrowse_Click(object sender, EventArgs e)
        {
            VF.FolderSelectDialog folderSelectDialog = new VF.FolderSelectDialog();
            folderSelectDialog.Title = "Select the folder of the Addon you want to create an AddonPackage for";
            folderSelectDialog.InitialDirectory = Settings.GetWowDirectory(WowVersion.Vanilla) + "Interface\\AddOns\\";
            if (folderSelectDialog.ShowDialog() == true)
            {
                c_txtAddonFolder.Text = folderSelectDialog.FileName;
            }
        }

        private void c_txtAddonFolder_TextChanged(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(c_txtAddonFolder.Text) == true)
            {
                c_txtAddonFolder.ForeColor = Color.Black;
                var addonName = c_txtAddonFolder.Text.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
                var addonInfo = InstalledAddons.GetAddonInfo(addonName, c_txtAddonFolder.Text);
                if (addonInfo != null)
                {
                    c_txtVersion.Text = addonInfo.m_VersionString;
                    if (Utility.ContainsInvalidFilenameChars(c_txtVersion.Text) == false)
                    {
                        c_btnCreate.Enabled = true;
                        return;
                    }
                }
            }
            c_txtAddonFolder.ForeColor = Color.Red;

            c_btnCreate.Enabled = false;
            c_txtVersion.Text = "Unknown Version!";
        }

        private void c_btnCreate_Click(object sender, EventArgs e)
        {
            if (c_txtDescription.Text.Length < 20)
            {
                Utility.MessageBoxShow("Description must be atleast 20 characters long");
                return;
            }
            var addonName = c_txtAddonFolder.Text.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
            int addonNameIndex = c_txtAddonFolder.Text.LastIndexOf(addonName);

            string addonRootFolder = c_txtAddonFolder.Text.Substring(0, addonNameIndex);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "zip";
            saveFileDialog.Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = addonRootFolder;
            saveFileDialog.AddExtension = true;
            saveFileDialog.FileName = addonName + "Package_" + c_txtVersion.Text + ".zip";
            var dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                string addonPackageFilename = saveFileDialog.FileName;
                string descFile = "\r\n#Version=" + c_txtVersion.Text;
                descFile += "\r\n#Description=" + c_txtDescription.Text.Replace("\r\n#", "\n#");
                descFile += "\r\n#UpdateSubmitter=" + Settings.UserID.Split('.').First();
                descFile += "\r\n#UpdateImportance=|DefaultImportance=" + (string)c_cbUpdateImportance.SelectedItem + "|";

                Utility.AssertFilePath(addonPackageFilename);

                ZipFile zipFile;
                if (System.IO.File.Exists(addonPackageFilename) == true)
                {
                    throw new Exception("The AddonPackage file already exists!");
                }

                zipFile = ZipFile.Create(addonPackageFilename);

                zipFile.BeginUpdate();

                zipFile.AddDirectoryFilesRecursive(addonRootFolder, c_txtAddonFolder.Text);

                //ZipEntry descEntry = new ZipEntry(addonName + "/VF_WowLauncher_AddonDescription.txt");
                //descEntry.DateTime = DateTime.Now;

                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream);
                streamWriter.Write(descFile);
                streamWriter.Flush();
                
                CustomStaticDataSource entryDataSource = new CustomStaticDataSource();
                entryDataSource.SetStream(memoryStream);

                zipFile.Add(entryDataSource, addonName + "/VF_WowLauncher_AddonDescription.txt");
                zipFile.CommitUpdate();
                zipFile.Close();
                Close();
            }
        }

        class CustomStaticDataSource : IStaticDataSource
        {
            private System.IO.Stream _stream;
            // Implement method from IStaticDataSource
            public System.IO.Stream GetSource()
            {
                return _stream;
            }

            // Call this to provide the memorystream
            public void SetStream(System.IO.Stream inputStream)
            {
                _stream = inputStream;
                _stream.Position = 0;
            }
        }
    }
}
