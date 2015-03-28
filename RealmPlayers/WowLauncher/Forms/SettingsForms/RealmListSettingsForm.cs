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
    public partial class RealmListSettingsForm : Form
    {
        public RealmListSettingsForm()
        {
            InitializeComponent();
        }

        private void RealmListSettingsForm_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);
            foreach(var realmList in Settings.Instance.RealmLists)
            {
                c_lstRealmConfigurations.Items.Add(realmList.Key);
            }
            c_cbxWowVersion.Items.Add(WowVersionEnum.Vanilla.ToString());
            c_cbxWowVersion.Items.Add(WowVersionEnum.TBC.ToString());

            ResetNewRealmConfigInputs();
        }

        private void ResetNewRealmConfigInputs()
        {
            c_txtNewConfigName.Text = "Configuration Name";
            c_txtNewRealmName.Text = "Exact Realm Name";
            c_txtNewRealmListURL.Text = "Realmlist URL";
            c_cbxWowVersion.SelectedIndex = c_cbxWowVersion.Items.Count - 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (c_lstRealmConfigurations.SelectedItem == null)
                return;
            var realmList = Settings.Instance.RealmLists[(string)c_lstRealmConfigurations.SelectedItem];
            c_txtSelectedRealmName.Text = realmList.RealmName;
            c_txtSelectedRealmListURL.Text = realmList.RealmList;
            c_txtSelectedWowVersion.Text = realmList.WowVersion.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Settings.Instance.RealmLists.ContainsKey(c_txtNewConfigName.Text) == true)
            {
                Utility.MessageBoxShow("Realm Config with this name already exist! Please change it to a unique name!");
            }
            else if (c_txtNewConfigName.Text == "Configuration Name" || c_txtNewConfigName.Text == "")
            {
                Utility.MessageBoxShow("Realm Config name was not valid. Please enter a name that will be used when choosing realm in the dropdown box in the Launcher!");
            }
            else if (c_txtNewRealmName.Text == "Exact Realm Name" || c_txtNewRealmName.Text == "")
            {
                Utility.MessageBoxShow("Realm Name was not valid. Please enter the correct name of the realm you want to connect to when using this realm configuration in the Launcher!");
            }
            else if (c_txtNewRealmListURL.Text == "Realmlist URL" || c_txtNewRealmListURL.Text == "")
            {
                Utility.MessageBoxShow("Realmlist URL was not valid. Please enter the correct realmlist URL of the realm you want to connect to when using this realm configuration in the Launcher!");
            }
            else
            {
                Settings.Instance.RealmLists.Add(c_txtNewRealmName.Text, new RealmInfo { RealmName = c_txtNewRealmName.Text, RealmList = c_txtNewRealmListURL.Text, WowVersion = (WowVersionEnum)Enum.Parse(typeof(WowVersionEnum), (string)c_cbxWowVersion.SelectedItem) });
                c_lstRealmConfigurations.Items.Add(c_txtNewRealmName.Text);
                c_lstRealmConfigurations.SelectedIndex = c_lstRealmConfigurations.Items.Count - 1;
                ResetNewRealmConfigInputs();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void c_txtNewConfigName_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void c_txtNewConfigName_Enter(object sender, EventArgs e)
        {
            if (c_txtNewConfigName.Text == "Configuration Name")
            {
                c_txtNewConfigName.Text = "";
            }
        }

        private void c_txtNewConfigName_Leave(object sender, EventArgs e)
        {
            if (c_txtNewConfigName.Text == "")
            {
                c_txtNewConfigName.Text = "Configuration Name";
            }
        }

        private void c_txtNewRealmName_Enter(object sender, EventArgs e)
        {
            if (c_txtNewRealmName.Text == "Exact Realm Name")
            {
                c_txtNewRealmName.Text = "";
            }
        }

        private void c_txtNewRealmName_Leave(object sender, EventArgs e)
        {
            if (c_txtNewRealmName.Text == "")
            {
                c_txtNewRealmName.Text = "Exact Realm Name";
            }
        }

        private void c_txtNewRealmListURL_Enter(object sender, EventArgs e)
        {
            if (c_txtNewRealmListURL.Text == "Realmlist URL")
            {
                c_txtNewRealmListURL.Text = "";
            }
        }

        private void c_txtNewRealmListURL_Leave(object sender, EventArgs e)
        {
            if (c_txtNewRealmListURL.Text == "")
            {
                c_txtNewRealmListURL.Text = "Realmlist URL";
            }
        }

    }
    public class RealmListSettings
    {
        public static void ShowRealmListSettings()
        {
            RealmListSettingsForm realmListSettingsForm = new RealmListSettingsForm();
            realmListSettingsForm.ShowDialog();
        }
    }
}
