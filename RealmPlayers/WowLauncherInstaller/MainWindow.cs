using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace VF_WowLauncherInstaller
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static string GetProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
        static void CreateShortcut(string shortcutFile, string targetFile, string description, string iconPath)
        {
            string shortcutLocation = shortcutFile.Replace('\\', '/') + ".lnk";
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = description;   // The description of the shortcut
            shortcut.IconLocation = @"" + iconPath.Replace('\\', '/');           // The icon of the shortcut
            shortcut.TargetPath = "\"" + targetFile.Replace('\\', '/') + "\"";                 // The path of the file that will launch when the shortcut is run
            shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(targetFile);
            shortcut.Save();                                    // Save the shortcut
        }

        private string m_InstallDirectory = "";

        private void SetInstallDirectory(string _InstallDirectory)
        {
            _InstallDirectory = _InstallDirectory.Replace('/', '\\');
            if (_InstallDirectory.EndsWith("\\") == false)
                _InstallDirectory += "\\";
            if (_InstallDirectory.ToLower().EndsWith("\\vf_wowlauncher\\") == false)
                _InstallDirectory += "VF_WowLauncher\\";

            m_InstallDirectory = _InstallDirectory;
            c_lblInstallDirectory.Text = "VF_WowLauncher will be installed to the folder:\r\n" + _InstallDirectory;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            c_txtInstallDirectory.Text = GetProgramFilesx86();
        }

        private void c_btnBrowse_Click(object sender, EventArgs e)
        {
            VF.FolderSelectDialog folderSelectDialog = new VF.FolderSelectDialog();
            folderSelectDialog.Title = "Choose where to install VF_WowLauncher";
            string initDir = c_txtInstallDirectory.Text.Replace('/', '\\');
            while (System.IO.Directory.Exists(initDir) == false)
            {
                int lastIndexOfBackSlash = initDir.LastIndexOf('\\');
                if(lastIndexOfBackSlash == -1)
                {
                    initDir = GetProgramFilesx86();
                    break;
                }
                initDir = initDir.Substring(0, lastIndexOfBackSlash);
            }
            folderSelectDialog.InitialDirectory = initDir;
            if (folderSelectDialog.ShowDialog(this) == true)
            {
                c_txtInstallDirectory.Text = folderSelectDialog.FileName;
            }
        }

        private void c_txtInstallDirectory_TextChanged(object sender, EventArgs e)
        {
            SetInstallDirectory(c_txtInstallDirectory.Text);
        }

        private bool Install()
        {
            if (System.IO.Directory.Exists(m_InstallDirectory) == true)
            {
                var files = System.IO.Directory.GetFiles(m_InstallDirectory);

                bool launcherInDirectory = false;
                foreach (var file in files)
                {
                    if (file.ToLower().Contains("vf_wowlauncher.exe") == true || file.ToLower().Contains("wyupdate.exe") == true)
                    {
                        launcherInDirectory = true;
                    }
                }
                if (launcherInDirectory == true)
                {
                    var dialogResult = MessageBox.Show("The installer has detected that VF_WowLauncher was already installed. Do you want to delete all the old settings and replace the other installation?", "Delete all old settings?", MessageBoxButtons.YesNo);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        try
                        {
                            VF.Utility.DeleteDirectory(m_InstallDirectory);
                        }
                        catch (Exception ex)
                        { }
                        if (System.IO.Directory.Exists(m_InstallDirectory) == true)
                        {
                            VF.Utility.SoftThreadSleep(5000);
                            if (System.IO.Directory.Exists(m_InstallDirectory) == true)
                            {
                                MessageBox.Show("Could not delete the old installation, try restart the computer or install to another path");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (files.Length > 0 || System.IO.Directory.GetDirectories(m_InstallDirectory).Length > 0)
                {
                    MessageBox.Show("The installer has detected that there were files in the folder selected as install directory. Please chose another directory or delete the directory and try install again");
                    return false;
                }
                else
                {
                    VF.Utility.DeleteDirectory(m_InstallDirectory);
                }
            }

            //At this stage m_InstallDirectory should not exist!
            if (System.IO.Directory.Exists(m_InstallDirectory) == true)
                throw new Exception("Unexpected: Installation Directory should not exist at this point!");

            System.IO.Directory.CreateDirectory(m_InstallDirectory);

            System.IO.File.WriteAllBytes(m_InstallDirectory + "wyUpdate.exe", Properties.Resources.wyUpdate);
            System.IO.File.WriteAllBytes(m_InstallDirectory + "client.wyc", Properties.Resources.client);

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = m_InstallDirectory + "wyUpdate.exe";
            startInfo.Arguments = "/skipinfo";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            var wyUpdateProcess = System.Diagnostics.Process.Start(startInfo);

            string data = "";
            while (wyUpdateProcess.HasExited == false)
            {
                VF.Utility.SoftThreadSleep(100);
                data += wyUpdateProcess.StandardOutput.ReadToEnd();
            }

            data += wyUpdateProcess.StandardOutput.ReadToEnd();
            int exitCode = wyUpdateProcess.ExitCode;
            if (exitCode == 1)
                return false;

            if (System.IO.File.Exists(m_InstallDirectory + "VF_WowLauncher.exe") == false)
                return false;
            
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (System.IO.File.Exists(desktopPath + "\\VF_WoWLauncher.lnk") == true)
                VF.Utility.DeleteFile(desktopPath + "\\VF_WoWLauncher.lnk");
            VF.Utility.SoftThreadSleep(100);
            CreateShortcut(desktopPath + "\\VF_WoWLauncher"
                , m_InstallDirectory + "VF_WowLauncher.exe", "VF_WoWLauncher", m_InstallDirectory + "VF_WowLauncher.exe");

            return true;
        }

        private void c_btnInstall_Click(object sender, EventArgs e)
        {
            c_btnInstall.Enabled = false;
            try
            {
                if (Install() == true)
                {
                    MessageBox.Show("VF_WowLauncher was installed successfully! A shortcut to VF_WoWLauncher has been created on your desktop.\n\nInstallation program will now close and VF_WoWLauncher will start, there are a few installation steps that may need to be done within the VF_WoWLauncher application");
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.FileName = m_InstallDirectory + "VF_WowLauncher.exe";
                    startInfo.WorkingDirectory = m_InstallDirectory;
                    var launcherProcess = System.Diagnostics.Process.Start(startInfo);
                    Close();
                }
                else
                {
                    MessageBox.Show("VF_WowLauncher failed to install. Please try Install again");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error. Please screenshot this and send to Dilatazu@gmail.com\r\nERROR_MESSAGE: " + ex.ToString());
            }
            c_btnInstall.Enabled = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://realmplayers.com/Donate.aspx");
        }
    }
}
