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
    public partial class AddonsSettingsForm : Form
    {
        public AddonsSettingsForm()
        {
            InitializeComponent();
            c_clbAddonStatus.MouseMove += c_clbAddonStatus_MouseMove;
            c_btnDisableAllAddons.Click += c_btnDisableAllAddons_Click;
            c_btnEnableAllAddons.Click += c_btnEnableAllAddons_Click;
            c_btnReset.Click += c_btnReset_Click;
            c_lbAddons.SelectedIndexChanged += c_lbAddons_SelectedIndexChanged;
        }

        AddonsWTF m_AddonsWTF = null;
        List<Tuple<string, AddonStatus>> m_AddonStatusChanges = new List<Tuple<string, AddonStatus>>();
        Dictionary<string, InstalledAddons.AddonInfo> m_AddonInfos = new Dictionary<string, InstalledAddons.AddonInfo>();
        private void AddonsSettingsForm_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);
            //this.TopMost = true;
            (new System.Threading.Tasks.Task(() => {
                m_AddonsWTF = AddonsWTF.LoadAllAccountAddons(WowVersionEnum.Vanilla);
                var installedAddons = InstalledAddons.GetInstalledAddons(WowVersionEnum.Vanilla);
                foreach (var installedAddon in installedAddons)
                {
                    m_AddonInfos[installedAddon] = InstalledAddons.GetAddonInfo(installedAddon, WowVersionEnum.Vanilla);
                    c_lbAddons.BeginInvoke(new Action(() => {
                        c_lbAddons.Items.Add(installedAddon);
                        if(c_lbAddons.Items.Count == 1) 
                            c_lbAddons.SelectedIndex = 0;
                    }));
                }
            })).Start();
            c_btnSaveAllChanges.Enabled = false;
        }

        class clbAddonStatusItem
        {
            AddonStatus m_OriginalStatus;

            public override string ToString()
            {
                return m_OriginalStatus.Account + " - " + m_OriginalStatus.Character;
            }
            public clbAddonStatusItem(AddonStatus _Status)
            {
                m_OriginalStatus = _Status;
            }
            public AddonStatus OriginalStatus
            {
                get { return m_OriginalStatus; }
            }
        }

        private void c_lbAddons_SelectedIndexChanged(object sender, EventArgs e)
        {
            string addonName = (string)c_lbAddons.SelectedItem;
            if (m_AddonInfos.ContainsKey(addonName))
            {
                var addonInfo = m_AddonInfos[addonName];
                c_gbAddonInfo.Text = "Information for Addon \"" + addonName + "\"";
                c_txtAddonTitle.Text = WowUtility.ConvertNoColor(addonInfo.m_AddonTitle);
                c_txtAddonVersion.Text = addonInfo.m_VersionString;
                c_txtAuthor.Text = addonInfo.m_Author;
                c_txtNotes.Text = addonInfo.m_Notes;

                c_clbAddonStatus.ItemCheck -= c_clbAddonStatus_ItemCheck;
                c_clbAddonStatus.Items.Clear();
                var addonStatuses = m_AddonsWTF.GetAddonStatus(addonName);
                foreach (var addonStatusEValue in addonStatuses)
                {
                    var addonStatus = addonStatusEValue;
                    int lastIndex = m_AddonStatusChanges.FindLastIndex((_Value) => { return (_Value.Item1 == addonName && _Value.Item2.IsSameChar(addonStatus)); });
                    if (lastIndex != -1)
                        addonStatus = m_AddonStatusChanges[lastIndex].Item2;
                    if (addonStatus.Enabled == AddonStatus.Enum.Enabled)
                        c_clbAddonStatus.Items.Add(new clbAddonStatusItem(addonStatusEValue), true);
                    else if (addonStatus.Enabled == AddonStatus.Enum.Disabled)
                        c_clbAddonStatus.Items.Add(new clbAddonStatusItem(addonStatusEValue), false);
                    else if (addonStatus.Enabled == AddonStatus.Enum.Default)
                        c_clbAddonStatus.Items.Add(new clbAddonStatusItem(addonStatusEValue), CheckState.Indeterminate);
                }
                c_clbAddonStatus.ItemCheck += c_clbAddonStatus_ItemCheck;
                c_lblEnabledFor.Text = "\"" + addonName + "\" is enabled for:";

                var neededForAddons = m_AddonInfos.Where((_Value) =>
                {
                    if (_Value.Value != null)
                    {
                        return _Value.Value.m_Dependencies.Contains(addonName);
                    }
                    else
                    {
                        return false;
                    }
                });
                c_lbNeededFor.Items.Clear();
                foreach (var neededForAddon in neededForAddons)
                {
                    c_lbNeededFor.Items.Add(neededForAddon.Key);
                }
                c_lblNeededFor.Text = "\"" + addonName + "\" is needed for addons:";
                /*c_lbDependencies.Items.Clear();
                c_lbDependencies.Items.AddRange(addonInfo.m_Dependencies.ToArray());

                c_lbSavedVariables.Items.Clear();
                c_lbSavedVariables.Items.AddRange(addonInfo.m_SavedVariables.ToArray());

                c_lbSavedVariablesPerCharacter.Items.Clear();
                c_lbSavedVariablesPerCharacter.Items.AddRange(addonInfo.m_SavedVariablesPerCharacter.ToArray());*/
            }
            else
            {
                c_gbAddonInfo.Text = "null";
                c_txtAddonTitle.Text = "null";
                c_txtAddonVersion.Text = "null";
                c_txtAuthor.Text = "null";
                c_txtNotes.Text = "null";


                c_clbAddonStatus.ItemCheck -= c_clbAddonStatus_ItemCheck;
                c_clbAddonStatus.Items.Clear();
                c_lblEnabledFor.Text = "null";
                c_lbNeededFor.Items.Clear();
                c_lblNeededFor.Text = "null";
            }
        }

        void c_clbAddonStatus_MouseMove(object sender, MouseEventArgs e)
        {
            string tooltipText = "";
            int index = c_clbAddonStatus.IndexFromPoint(e.Location);
            if (index >= 0 && index < c_clbAddonStatus.Items.Count)
            {
                var checkedStatus = c_clbAddonStatus.GetItemChecked(index);
                AddonStatus addonStatus = ((clbAddonStatusItem)c_clbAddonStatus.Items[index]).OriginalStatus;
                tooltipText = "Account: " + addonStatus.Account
                    + "\r\nRealm: " + addonStatus.Realm
                    + "\r\nCharacter: " + addonStatus.Character
                    + "\r\nAddon Setting: " + (checkedStatus ? "Enabled" : "Disabled");
                
                //if (checkedStatus != (addonStatus.Enabled == AddonStatus.Enum.Enabled || addonStatus.Enabled == AddonStatus.Enum.Default))
                //    tooltipText += "\r\nPrevious Setting: " + addonStatus.Enabled.ToString();
            }
            //c_tlToolTip.ShowAlways = true;
            if (c_tlToolTip.GetToolTip(c_clbAddonStatus) != tooltipText)
                c_tlToolTip.SetToolTip(c_clbAddonStatus, tooltipText);
        }
        
        void c_clbAddonStatus_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string addonName = (string)c_lbAddons.SelectedItem;
            AddonStatus addonStatus = ((clbAddonStatusItem)c_clbAddonStatus.Items[e.Index]).OriginalStatus;
            addonStatus.Enabled = (e.NewValue == CheckState.Checked ? AddonStatus.Enum.Enabled : AddonStatus.Enum.Disabled);
            m_AddonStatusChanges.Add(Tuple.Create(addonName, addonStatus));
            c_btnSaveAllChanges.Enabled = true;
        }

        private void c_btnDisableAllAddons_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < c_clbAddonStatus.Items.Count; ++i)
            {
                c_clbAddonStatus.SetItemChecked(i, false);
            }
        }

        private void c_btnEnableAllAddons_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < c_clbAddonStatus.Items.Count; ++i)
            {
                c_clbAddonStatus.SetItemChecked(i, true);
            }
        }

        private void c_btnReset_Click(object sender, EventArgs e)
        {
            string addonName = (string)c_lbAddons.SelectedItem;
            m_AddonStatusChanges.RemoveAll((_Value) => { return (_Value.Item1 == addonName); });
            c_lbAddons_SelectedIndexChanged(null, null);
            if (m_AddonStatusChanges.Count == 0)
                c_btnSaveAllChanges.Enabled = false;
        }

        private void c_btnUninstallAddon_Click(object sender, EventArgs e)
        {
            string addonName = (string)c_lbAddons.SelectedItem;
            c_btnUninstallAddon.Enabled = false;
            if (c_lbNeededFor.Items.Count == 0)
            {
                if (Utility.MessageBoxShow("Are you sure you want to uninstall the addon \"" + addonName + "\"?", "Are you sure?", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                {
                    string backupFile = InstalledAddons.BackupAddon(addonName, WowVersionEnum.Vanilla);
                    if (InstalledAddons.UninstallAddon(addonName, WowVersionEnum.Vanilla) == true)
                    {
                        Utility.MessageBoxShow("Addon \"" + addonName + "\" was successfully uninstalled!\r\n\r\nA backup zip file of the addon and WTF files was saved incase you regret the decision here:\r\n" + StaticValues.LauncherWorkDirectory + "\\" + backupFile);
                        c_lbAddons.SelectedIndex = 0;
                        c_lbAddons.Items.Remove(addonName);
                    }
                    else
                    {
                        Utility.MessageBoxShow("Addon \"" + addonName + "\" could not be uninstalled. Try again later. If you get this error all the times contact Dilatazu for more info.");
                    }
                }
            }
            else
            {
                string neededForStr = "";
                foreach (string item in c_lbNeededFor.Items)
                {
                    neededForStr = neededForStr + item + "\r\n";
                }
                if (Utility.MessageBoxShow("\"" + addonName + "\" is needed for multiple addons. Removing this addon means that the following addons will stop work:\r\n" + neededForStr + "\r\nAre you sure you want to uninstall the addon?", "Are you sure?", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                {
                    string backupFile = InstalledAddons.BackupAddon(addonName, WowVersionEnum.Vanilla);
                    if (InstalledAddons.UninstallAddon(addonName, WowVersionEnum.Vanilla) == true)
                    {
                        Utility.MessageBoxShow("Addon \"" + addonName + "\" was successfully uninstalled!\r\n\r\nA backup zip file of the addon and WTF files was saved incase you regret the decision:\r\n" + StaticValues.LauncherWorkDirectory + "\\" + backupFile);
                        c_lbAddons.SelectedIndex = 0;
                        c_lbAddons.Items.Remove(addonName);
                    }
                    else
                    {
                        Utility.MessageBoxShow("Addon \"" + addonName + "\" could not be uninstalled. Try again later. If you get this error all the times contact Dilatazu for more info.");
                    }
                }
            }
            c_btnUninstallAddon.Enabled = true;
        }

        private void c_lblEnabledFor_Click(object sender, EventArgs e)
        {

        }

        private void c_gbAddonInfo_Enter(object sender, EventArgs e)
        {

        }

        private void c_btnReinstallAddon_Click(object sender, EventArgs e)
        {
            string addonName = (string)c_lbAddons.SelectedItem;
            if (Utility.MessageBoxShow("Reinstalling an addon means all the previous WTF/savedvariables data will be removed. Are you sure you want to reinstall the addon \"" + addonName + "\"?", "Are you sure?", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
            {
                string backupFile = InstalledAddons.BackupAddon(addonName, WowVersionEnum.Vanilla, AddonBackupMode.BackupWTF);
                InstalledAddons.ReinstallAddon(addonName, WowVersionEnum.Vanilla);
                Utility.MessageBoxShow("Addon \"" + addonName + "\" was successfully reinstalled!\r\n\r\nA backup zip file of the old WTF files was saved incase you regret the decision:\r\n" + StaticValues.LauncherWorkDirectory + "\\" + backupFile);
            }
        }

        private void c_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void c_btnSaveAllChanges_Click(object sender, EventArgs e)
        {
            if (m_AddonStatusChanges.Count > 0)
            {
                foreach (var addonChanges in m_AddonStatusChanges)
                {
                    m_AddonsWTF.SetAddonStatus(addonChanges.Item1, addonChanges.Item2);
                }
                m_AddonsWTF.SaveAll();
                Utility.MessageBoxShow("Successfully saved " + m_AddonStatusChanges.Count + " changes to the Enabled/Disabled settings for various addons");
            }
            Close();
        }

        private void c_btnCleanWTF_Click(object sender, EventArgs e)
        {
            if (Utility.MessageBoxShow("This command will clean the entire WTF folder of old unused data etc. Are you sure you want to continue?", "Are you sure you want to clean the entire WTF folder?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {

            }
        }

        private void c_clbAddonStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    public class AddonsSettings
    {
        public static void ShowAddonsSettings()
        {
            AddonsSettingsForm addonsSettingsForm = new AddonsSettingsForm();
            addonsSettingsForm.ShowDialog();
        }
    }
}
