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
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void AboutWindow_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);
            c_lblProgramVersion.Text = "Version: " + StaticValues.LauncherVersion;
        }

        private void c_btnAboutRealmplayers_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://realmplayers.com/About.aspx");
        }

        private void c_btnDonate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://realmplayers.com/Donate.aspx");
        }

        private void c_btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
