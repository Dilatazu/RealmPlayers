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
    public partial class SaveConfigProfileForm : Form
    {
        string m_DefaultProfile = "";
        ConfigWTF m_ConfigWTF = null;
        public bool m_SavedProfile = false;
        public SaveConfigProfileForm(string _DefaultProfile, ConfigWTF _ConfigWTF)
        {
            m_DefaultProfile = _DefaultProfile;
            m_ConfigWTF = _ConfigWTF;
            InitializeComponent();
        }

        private void SaveConfigProfileForm_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);
            //this.TopMost = true;
            c_ddlConfigProfiles.Items.AddRange(ConfigProfiles.GetProfileNames().ToArray());
            c_ddlConfigProfiles.Text = m_DefaultProfile;
        }

        private void c_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void c_btnSave_Click(object sender, EventArgs e)
        {
            if (c_ddlConfigProfiles.Text.Contains(' ') || c_ddlConfigProfiles.Text == "" || Utility.IsValidFilename(c_ddlConfigProfiles.Text) == false)
            {
                Utility.MessageBoxShow("\"" + c_ddlConfigProfiles.Text + "\" is not a valid profile name!");
                return;
            }
            ConfigProfiles.SaveConfigWTF(m_ConfigWTF, c_ddlConfigProfiles.Text);
            m_SavedProfile = true;
            Close();
        }
    }
    public class SaveConfigProfile
    {
        public static bool SaveConfig(string _DefaultProfile, ConfigWTF _ConfigWTF)
        {
            SaveConfigProfileForm saveConfigProfileForm = new SaveConfigProfileForm(_DefaultProfile, _ConfigWTF);
            saveConfigProfileForm.ShowDialog();
            return saveConfigProfileForm.m_SavedProfile;
        }
    }
}
