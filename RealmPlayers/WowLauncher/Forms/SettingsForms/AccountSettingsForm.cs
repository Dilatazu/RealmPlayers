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
    public partial class AccountSettingsForm : Form
    {
        public AccountSettingsForm()
        {
            InitializeComponent();
        }

        private void AccountSettingsForm_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);
            //this.TopMost = true;
            c_ddlRealm.SelectedItem = Settings.Instance.DefaultRealm;
            UpdateAccounts();
        }
        private void UpdateAccounts()
        {
            var accounts = Utility.GetDirectoriesInDirectory(Settings.GetWowDirectory(WowVersion.Vanilla) + "WTF\\Account");
            for (int i = 0; i < accounts.Count; ++i)
            {
                string realmPaths = Settings.GetWowDirectory(WowVersion.Vanilla) + "WTF\\Account\\" + accounts[i];
                var realms = Utility.GetDirectoriesInDirectory(realmPaths);
                if (realms.Contains(StaticValues.RealmNameConverter.First((_Value) => _Value.Value == (string)c_ddlRealm.SelectedItem).Key) == false)
                {
                    accounts.Remove(accounts[i]);
                    --i;
                }
            }
            c_lbAccounts.Items.Clear();
            c_lbAccounts.Items.AddRange(accounts.ToArray());
            if (c_lbAccounts.Items.Count > 0)
                c_lbAccounts.SelectedIndex = 0;
        }
        private void UpdateCharacters()
        {
            string account = (string)c_lbAccounts.SelectedItem;
            string realmPaths = Settings.GetWowDirectory(WowVersion.Vanilla) + "WTF\\Account\\" + account;
            var realms = Utility.GetDirectoriesInDirectory(realmPaths);
            List<string> characters = new List<string>();
            foreach (var realm in realms)
            {
                string realmName = "";
                StaticValues.RealmNameConverter.TryGetValue(realm, out realmName);
                if ((string)c_ddlRealm.SelectedItem == realmName) //realm != "SavedVariables" && 
                {
                    var realmCharacters = Utility.GetDirectoriesInDirectory(realmPaths + "\\" + realm);
                    foreach (var character in realmCharacters)
                    {
                        characters.Add(character);
                        //characters.Add(realm + "/" + character);
                    }
                }
            }
            c_lbCharacters.Items.Clear();
            c_lbCharacters.Items.AddRange(characters.ToArray());
            if (c_lbCharacters.Items.Count > 0)
                c_lbCharacters.SelectedIndex = 0;
        }
        AddonsConfig m_AddonsConfig = null;
        private void UpdateAddons()
        {
            string characterDir = Settings.GetWowDirectory(WowVersion.Vanilla) + "WTF\\Account\\" + (string)c_lbAccounts.SelectedItem
                 + "\\" + StaticValues.RealmNameConverter.First((_Value) => _Value.Value == (string)c_ddlRealm.SelectedItem).Key
                 + "\\" + (string)c_lbCharacters.SelectedItem + "\\";

            //if (System.IO.File.Exists(characterDir + "AddOns.txt") == true)
            //{
                if (m_AddonsConfig != null)
                {
                    //m_AddonsConfig.ReleaseAllConfigsLB(c_lbAddons);
                    m_AddonsConfig.ReleaseAllConfigsCLB(c_clbAddons);
                }
                m_AddonsConfig = AddonsConfig.LoadConfigFile(characterDir + "AddOns.txt");
                //m_AddonsConfig.InitAllConfigsLB(c_lbAddons);
                m_AddonsConfig.InitAllConfigsCLB(c_clbAddons);

                if (c_clbAddons.Items.Count >= 1)
                {
                    c_btnDisableAllAddons.Enabled = true;
                    c_btnEnableAllAddons.Enabled = true;
                }
                else// if (c_lbAddons.Items.Count < 1)
                {
                    c_btnDisableAllAddons.Enabled = false;
                    c_btnEnableAllAddons.Enabled = false;
                }
            //}
            //else
            //{
            //    if (m_AddonsConfig != null)
            //    {
            //        c_lbAddons.MouseDoubleClick -= m_AddonsConfig.EventAllConfigsLB_MouseDoubleClick;
            //        m_AddonsConfig.Dispose();
            //    }
            //    c_lbAddons.Items.Clear();
            //}

        }
        private void c_lbAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCharacters();
        }

        private void c_ddlRealm_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAccounts();
        }

        private void c_lbCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAddons();
        }

        private void c_btnDisableAllAddons_Click(object sender, EventArgs e)
        {
            if (c_clbAddons.Items.Count >= 1)
            {
                if (Utility.MessageBoxShow("Are you sure you want to disable all addons for the character " + (string)c_lbCharacters.SelectedItem + "? This action can not be reversed. All previous settings will be forgotten.", "Disable all addons?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    string characterDir = Settings.GetWowDirectory(WowVersion.Vanilla) + "WTF\\Account\\" + (string)c_lbAccounts.SelectedItem
                         + "\\" + StaticValues.RealmNameConverter.First((_Value) => _Value.Value == (string)c_ddlRealm.SelectedItem).Key
                         + "\\" + (string)c_lbCharacters.SelectedItem + "\\";

                    m_AddonsConfig.DisableAllAddons();

                    m_AddonsConfig.SaveConfigFile(characterDir + "AddOns.txt");
                }
            }
        }

        private void c_btnEnableAllAddons_Click(object sender, EventArgs e)
        {
            if (c_clbAddons.Items.Count >= 1)
            {
                if (Utility.MessageBoxShow("Are you sure you want to enable all addons for the character " + (string)c_lbCharacters.SelectedItem + "? This action can not be reversed. All previous settings will be forgotten.", "Enable all addons?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    string characterDir = Settings.GetWowDirectory(WowVersion.Vanilla) + "WTF\\Account\\" + (string)c_lbAccounts.SelectedItem
                         + "\\" + StaticValues.RealmNameConverter.First((_Value) => _Value.Value == (string)c_ddlRealm.SelectedItem).Key
                         + "\\" + (string)c_lbCharacters.SelectedItem + "\\";

                    m_AddonsConfig.EnableAllAddons();

                    m_AddonsConfig.SaveConfigFile(characterDir + "AddOns.txt");
                }
            }
        }
    }
    public class AccountSettings
    {
        public static void ShowAccountSettings()
        {
            AccountSettingsForm accountSettingsForm = new AccountSettingsForm();
            accountSettingsForm.ShowDialog();
        }
    }
}
