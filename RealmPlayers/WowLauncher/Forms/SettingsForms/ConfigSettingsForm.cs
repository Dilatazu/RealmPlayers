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
    public partial class ConfigSettingsForm : Form
    {
        private ConfigWTF m_ConfigWTF = null;
        private string m_ProfileName = "DefaultProfile";
        public ConfigSettingsForm(ConfigWTF _ConfigWTF, string _ProfileName)
        {
            m_ConfigWTF = _ConfigWTF;
            m_ProfileName = _ProfileName;
            this.Activated += ConfigSettingsForm_Activated;
            InitializeComponent();
        }

        void ConfigSettingsForm_Activated(object sender, EventArgs e)
        {
            c_lbSettings.Focus();
        }

        private void ConfigSettingsForm_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);
            //this.TopMost = true;
            c_ddlConfigProfile.Items.Clear();
            c_ddlConfigProfile.Items.Add("Active Wow Config");
            c_ddlConfigProfile.Items.AddRange(ConfigProfiles.GetProfileNames().ToArray());
            if (m_ConfigWTF == null)
                m_ConfigWTF = ConfigWTF.LoadWTFConfigFile(WowVersionEnum.Vanilla);
            if (c_ddlConfigProfile.Items.Contains(m_ProfileName) == false)
                c_ddlConfigProfile.Items.Add(m_ProfileName);

            c_ddlConfigProfile.SelectedItem = m_ProfileName;
            ConfigureConfigWTF();
        }
        private void ConfigureConfigWTF()
        {
            m_ConfigWTF.InitMaximizedCB(c_cbMaximized);
            m_ConfigWTF.InitWindowModeCB(c_cbWindowMode);
            m_ConfigWTF.InitResolutionDDL(c_ddlResolution);
            m_ConfigWTF.InitScriptMemoryDDL(c_ddlScriptMemory);
            m_ConfigWTF.InitAllConfigsLB(c_lbSettings);

            c_cbMaximized.CheckedChanged += m_ConfigWTF.EventMaximizedCB_Changed;
            c_cbWindowMode.CheckedChanged += m_ConfigWTF.EventWindowModeCB_Changed;
            c_ddlResolution.SelectedIndexChanged += m_ConfigWTF.EventResolutionDDL_Changed;
            c_ddlScriptMemory.SelectedIndexChanged += m_ConfigWTF.EventScriptMemoryDDL_Changed;
            c_lbSettings.MouseDoubleClick += m_ConfigWTF.EventAllConfigsLB_MouseDoubleClick;
        }

        private void c_ddlConfigProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)c_ddlConfigProfile.SelectedItem != m_ProfileName)
            {
                m_ProfileName = (string)c_ddlConfigProfile.SelectedItem;
                c_cbMaximized.CheckedChanged -= m_ConfigWTF.EventMaximizedCB_Changed;
                c_cbWindowMode.CheckedChanged -= m_ConfigWTF.EventWindowModeCB_Changed;
                c_ddlResolution.SelectedIndexChanged -= m_ConfigWTF.EventResolutionDDL_Changed;
                c_ddlScriptMemory.SelectedIndexChanged -= m_ConfigWTF.EventScriptMemoryDDL_Changed;
                c_lbSettings.MouseDoubleClick -= m_ConfigWTF.EventAllConfigsLB_MouseDoubleClick;
                m_ConfigWTF.Dispose();
                m_ConfigWTF = ConfigProfiles.GetProfileConfigFile(m_ProfileName);
                ConfigureConfigWTF();
            }
            c_lbSettings.Focus();
        }

        //private void c_btnDone_Click(object sender, EventArgs e)
        //{
        //    m_ConfigWTF.SaveWTFConfigFile();
        //    Close();
        //}

        private void c_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void c_btnSave_Click(object sender, EventArgs e)
        {
            string profileName = m_ProfileName;
            if (profileName == "Active Wow Config")
                profileName = "DefaultProfile";
            if (SaveConfigProfile.SaveConfig(profileName, m_ConfigWTF) == true)
                Close();
            else
                c_lbSettings.Focus();
        }

        private void c_ddlResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            c_lbSettings.Focus();
        }

        private void c_ddlScriptMemory_SelectedIndexChanged(object sender, EventArgs e)
        {
            c_lbSettings.Focus();
        }

        private void c_cbWindowMode_CheckedChanged(object sender, EventArgs e)
        {
            c_lbSettings.Focus();
        }

        private void c_cbMaximized_CheckedChanged(object sender, EventArgs e)
        {
            c_lbSettings.Focus();
        }

    }
    public partial class ConfigSettings
    {
        internal static void EditWTFConfigSettings(WowVersionEnum _WowVersion)
        {
            ConfigSettingsForm configForm = new ConfigSettingsForm(ConfigWTF.LoadWTFConfigFile(_WowVersion), "Active Wow Config");
            configForm.ShowDialog();
        }
        public static void EditProfileConfig(string _Profile)
        {
            ConfigSettingsForm configForm = new ConfigSettingsForm(ConfigProfiles.GetProfileConfigFile(_Profile), _Profile);
            configForm.ShowDialog();
        }
    }
}
