namespace VF_WoWLauncher
{
    partial class AddonsSettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.c_tlToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.c_btnSaveAllChanges = new System.Windows.Forms.Button();
            this.c_lblEnabledFor = new System.Windows.Forms.Label();
            this.c_btnReset = new System.Windows.Forms.Button();
            this.c_clbAddonStatus = new System.Windows.Forms.CheckedListBox();
            this.c_btnEnableAllAddons = new System.Windows.Forms.Button();
            this.c_btnDisableAllAddons = new System.Windows.Forms.Button();
            this.c_btnUninstallAddon = new System.Windows.Forms.Button();
            this.c_txtAddonVersion = new System.Windows.Forms.TextBox();
            this.c_txtAddonTitle = new System.Windows.Forms.TextBox();
            this.c_lblAddonTitle = new System.Windows.Forms.Label();
            this.c_lblAddonVersion = new System.Windows.Forms.Label();
            this.c_lbAddons = new System.Windows.Forms.ListBox();
            this.c_lbNeededFor = new System.Windows.Forms.ListBox();
            this.c_lblNeededFor = new System.Windows.Forms.Label();
            this.c_lblNotes = new System.Windows.Forms.Label();
            this.c_txtNotes = new System.Windows.Forms.TextBox();
            this.c_txtAuthor = new System.Windows.Forms.TextBox();
            this.c_lblAuthor = new System.Windows.Forms.Label();
            this.c_gbAddonInfo = new System.Windows.Forms.GroupBox();
            this.c_btnReinstallAddon = new System.Windows.Forms.Button();
            this.c_btnCancel = new System.Windows.Forms.Button();
            this.c_btnCleanWTF = new System.Windows.Forms.Button();
            this.c_gbAddonInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // c_btnSaveAllChanges
            // 
            this.c_btnSaveAllChanges.Location = new System.Drawing.Point(584, 448);
            this.c_btnSaveAllChanges.Name = "c_btnSaveAllChanges";
            this.c_btnSaveAllChanges.Size = new System.Drawing.Size(129, 23);
            this.c_btnSaveAllChanges.TabIndex = 39;
            this.c_btnSaveAllChanges.Text = "Save Changes && Close";
            this.c_btnSaveAllChanges.UseVisualStyleBackColor = true;
            this.c_btnSaveAllChanges.Click += new System.EventHandler(this.c_btnSaveAllChanges_Click);
            // 
            // c_lblEnabledFor
            // 
            this.c_lblEnabledFor.AutoSize = true;
            this.c_lblEnabledFor.Location = new System.Drawing.Point(257, 22);
            this.c_lblEnabledFor.Name = "c_lblEnabledFor";
            this.c_lblEnabledFor.Size = new System.Drawing.Size(97, 13);
            this.c_lblEnabledFor.TabIndex = 38;
            this.c_lblEnabledFor.Text = "Enabled addon for:";
            this.c_lblEnabledFor.Click += new System.EventHandler(this.c_lblEnabledFor_Click);
            // 
            // c_btnReset
            // 
            this.c_btnReset.Location = new System.Drawing.Point(351, 38);
            this.c_btnReset.Name = "c_btnReset";
            this.c_btnReset.Size = new System.Drawing.Size(56, 23);
            this.c_btnReset.TabIndex = 37;
            this.c_btnReset.Text = "Reset";
            this.c_btnReset.UseVisualStyleBackColor = true;
            // 
            // c_clbAddonStatus
            // 
            this.c_clbAddonStatus.FormattingEnabled = true;
            this.c_clbAddonStatus.Location = new System.Drawing.Point(260, 67);
            this.c_clbAddonStatus.Name = "c_clbAddonStatus";
            this.c_clbAddonStatus.Size = new System.Drawing.Size(238, 349);
            this.c_clbAddonStatus.TabIndex = 36;
            this.c_clbAddonStatus.SelectedIndexChanged += new System.EventHandler(this.c_clbAddonStatus_SelectedIndexChanged);
            // 
            // c_btnEnableAllAddons
            // 
            this.c_btnEnableAllAddons.Location = new System.Drawing.Point(260, 38);
            this.c_btnEnableAllAddons.Name = "c_btnEnableAllAddons";
            this.c_btnEnableAllAddons.Size = new System.Drawing.Size(85, 23);
            this.c_btnEnableAllAddons.TabIndex = 35;
            this.c_btnEnableAllAddons.Text = "Enable for All";
            this.c_btnEnableAllAddons.UseVisualStyleBackColor = true;
            // 
            // c_btnDisableAllAddons
            // 
            this.c_btnDisableAllAddons.Location = new System.Drawing.Point(413, 38);
            this.c_btnDisableAllAddons.Name = "c_btnDisableAllAddons";
            this.c_btnDisableAllAddons.Size = new System.Drawing.Size(85, 23);
            this.c_btnDisableAllAddons.TabIndex = 34;
            this.c_btnDisableAllAddons.Text = "Disable for All";
            this.c_btnDisableAllAddons.UseVisualStyleBackColor = true;
            // 
            // c_btnUninstallAddon
            // 
            this.c_btnUninstallAddon.Location = new System.Drawing.Point(152, 393);
            this.c_btnUninstallAddon.Name = "c_btnUninstallAddon";
            this.c_btnUninstallAddon.Size = new System.Drawing.Size(99, 23);
            this.c_btnUninstallAddon.TabIndex = 33;
            this.c_btnUninstallAddon.Text = "Uninstall Addon";
            this.c_btnUninstallAddon.UseVisualStyleBackColor = true;
            this.c_btnUninstallAddon.Click += new System.EventHandler(this.c_btnUninstallAddon_Click);
            // 
            // c_txtAddonVersion
            // 
            this.c_txtAddonVersion.Location = new System.Drawing.Point(50, 44);
            this.c_txtAddonVersion.Name = "c_txtAddonVersion";
            this.c_txtAddonVersion.ReadOnly = true;
            this.c_txtAddonVersion.Size = new System.Drawing.Size(199, 20);
            this.c_txtAddonVersion.TabIndex = 32;
            // 
            // c_txtAddonTitle
            // 
            this.c_txtAddonTitle.Location = new System.Drawing.Point(50, 18);
            this.c_txtAddonTitle.Name = "c_txtAddonTitle";
            this.c_txtAddonTitle.ReadOnly = true;
            this.c_txtAddonTitle.Size = new System.Drawing.Size(200, 20);
            this.c_txtAddonTitle.TabIndex = 31;
            // 
            // c_lblAddonTitle
            // 
            this.c_lblAddonTitle.AutoSize = true;
            this.c_lblAddonTitle.Location = new System.Drawing.Point(8, 21);
            this.c_lblAddonTitle.Name = "c_lblAddonTitle";
            this.c_lblAddonTitle.Size = new System.Drawing.Size(30, 13);
            this.c_lblAddonTitle.TabIndex = 30;
            this.c_lblAddonTitle.Text = "Title:";
            // 
            // c_lblAddonVersion
            // 
            this.c_lblAddonVersion.AutoSize = true;
            this.c_lblAddonVersion.Location = new System.Drawing.Point(9, 47);
            this.c_lblAddonVersion.Name = "c_lblAddonVersion";
            this.c_lblAddonVersion.Size = new System.Drawing.Size(45, 13);
            this.c_lblAddonVersion.TabIndex = 29;
            this.c_lblAddonVersion.Text = "Version:";
            // 
            // c_lbAddons
            // 
            this.c_lbAddons.FormattingEnabled = true;
            this.c_lbAddons.Location = new System.Drawing.Point(12, 12);
            this.c_lbAddons.Name = "c_lbAddons";
            this.c_lbAddons.Size = new System.Drawing.Size(187, 459);
            this.c_lbAddons.TabIndex = 28;
            // 
            // c_lbNeededFor
            // 
            this.c_lbNeededFor.FormattingEnabled = true;
            this.c_lbNeededFor.Location = new System.Drawing.Point(11, 277);
            this.c_lbNeededFor.Name = "c_lbNeededFor";
            this.c_lbNeededFor.Size = new System.Drawing.Size(240, 108);
            this.c_lbNeededFor.TabIndex = 40;
            // 
            // c_lblNeededFor
            // 
            this.c_lblNeededFor.AutoSize = true;
            this.c_lblNeededFor.Location = new System.Drawing.Point(9, 261);
            this.c_lblNeededFor.Name = "c_lblNeededFor";
            this.c_lblNeededFor.Size = new System.Drawing.Size(95, 13);
            this.c_lblNeededFor.TabIndex = 41;
            this.c_lblNeededFor.Text = "Addon needed for:";
            // 
            // c_lblNotes
            // 
            this.c_lblNotes.AutoSize = true;
            this.c_lblNotes.Location = new System.Drawing.Point(9, 141);
            this.c_lblNotes.Name = "c_lblNotes";
            this.c_lblNotes.Size = new System.Drawing.Size(38, 13);
            this.c_lblNotes.TabIndex = 42;
            this.c_lblNotes.Text = "Notes:";
            // 
            // c_txtNotes
            // 
            this.c_txtNotes.Location = new System.Drawing.Point(11, 157);
            this.c_txtNotes.Multiline = true;
            this.c_txtNotes.Name = "c_txtNotes";
            this.c_txtNotes.ReadOnly = true;
            this.c_txtNotes.Size = new System.Drawing.Size(238, 101);
            this.c_txtNotes.TabIndex = 43;
            // 
            // c_txtAuthor
            // 
            this.c_txtAuthor.Location = new System.Drawing.Point(11, 89);
            this.c_txtAuthor.Multiline = true;
            this.c_txtAuthor.Name = "c_txtAuthor";
            this.c_txtAuthor.ReadOnly = true;
            this.c_txtAuthor.Size = new System.Drawing.Size(238, 49);
            this.c_txtAuthor.TabIndex = 45;
            // 
            // c_lblAuthor
            // 
            this.c_lblAuthor.AutoSize = true;
            this.c_lblAuthor.Location = new System.Drawing.Point(9, 73);
            this.c_lblAuthor.Name = "c_lblAuthor";
            this.c_lblAuthor.Size = new System.Drawing.Size(41, 13);
            this.c_lblAuthor.TabIndex = 44;
            this.c_lblAuthor.Text = "Author:";
            // 
            // c_gbAddonInfo
            // 
            this.c_gbAddonInfo.Controls.Add(this.c_btnReinstallAddon);
            this.c_gbAddonInfo.Controls.Add(this.c_txtAddonTitle);
            this.c_gbAddonInfo.Controls.Add(this.c_txtAuthor);
            this.c_gbAddonInfo.Controls.Add(this.c_lblAddonVersion);
            this.c_gbAddonInfo.Controls.Add(this.c_lblAuthor);
            this.c_gbAddonInfo.Controls.Add(this.c_lblAddonTitle);
            this.c_gbAddonInfo.Controls.Add(this.c_txtNotes);
            this.c_gbAddonInfo.Controls.Add(this.c_txtAddonVersion);
            this.c_gbAddonInfo.Controls.Add(this.c_lblNotes);
            this.c_gbAddonInfo.Controls.Add(this.c_btnUninstallAddon);
            this.c_gbAddonInfo.Controls.Add(this.c_lblNeededFor);
            this.c_gbAddonInfo.Controls.Add(this.c_btnDisableAllAddons);
            this.c_gbAddonInfo.Controls.Add(this.c_lbNeededFor);
            this.c_gbAddonInfo.Controls.Add(this.c_btnEnableAllAddons);
            this.c_gbAddonInfo.Controls.Add(this.c_clbAddonStatus);
            this.c_gbAddonInfo.Controls.Add(this.c_lblEnabledFor);
            this.c_gbAddonInfo.Controls.Add(this.c_btnReset);
            this.c_gbAddonInfo.Location = new System.Drawing.Point(205, 12);
            this.c_gbAddonInfo.Name = "c_gbAddonInfo";
            this.c_gbAddonInfo.Size = new System.Drawing.Size(508, 425);
            this.c_gbAddonInfo.TabIndex = 46;
            this.c_gbAddonInfo.TabStop = false;
            this.c_gbAddonInfo.Text = "Information for Addon";
            this.c_gbAddonInfo.Enter += new System.EventHandler(this.c_gbAddonInfo_Enter);
            // 
            // c_btnReinstallAddon
            // 
            this.c_btnReinstallAddon.Location = new System.Drawing.Point(11, 392);
            this.c_btnReinstallAddon.Name = "c_btnReinstallAddon";
            this.c_btnReinstallAddon.Size = new System.Drawing.Size(93, 23);
            this.c_btnReinstallAddon.TabIndex = 46;
            this.c_btnReinstallAddon.Text = "Reinstall Addon";
            this.c_btnReinstallAddon.UseVisualStyleBackColor = true;
            this.c_btnReinstallAddon.Click += new System.EventHandler(this.c_btnReinstallAddon_Click);
            // 
            // c_btnCancel
            // 
            this.c_btnCancel.Location = new System.Drawing.Point(518, 448);
            this.c_btnCancel.Name = "c_btnCancel";
            this.c_btnCancel.Size = new System.Drawing.Size(60, 23);
            this.c_btnCancel.TabIndex = 47;
            this.c_btnCancel.Text = "Cancel";
            this.c_btnCancel.UseVisualStyleBackColor = true;
            this.c_btnCancel.Click += new System.EventHandler(this.c_btnCancel_Click);
            // 
            // c_btnCleanWTF
            // 
            this.c_btnCleanWTF.Location = new System.Drawing.Point(205, 447);
            this.c_btnCleanWTF.Name = "c_btnCleanWTF";
            this.c_btnCleanWTF.Size = new System.Drawing.Size(117, 23);
            this.c_btnCleanWTF.TabIndex = 48;
            this.c_btnCleanWTF.Text = "Clean WTF folder";
            this.c_btnCleanWTF.UseVisualStyleBackColor = true;
            this.c_btnCleanWTF.Visible = false;
            this.c_btnCleanWTF.Click += new System.EventHandler(this.c_btnCleanWTF_Click);
            // 
            // AddonsSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 482);
            this.Controls.Add(this.c_btnCleanWTF);
            this.Controls.Add(this.c_btnCancel);
            this.Controls.Add(this.c_gbAddonInfo);
            this.Controls.Add(this.c_btnSaveAllChanges);
            this.Controls.Add(this.c_lbAddons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddonsSettingsForm";
            this.Text = "Addons Manager";
            this.Load += new System.EventHandler(this.AddonsSettingsForm_Load);
            this.c_gbAddonInfo.ResumeLayout(false);
            this.c_gbAddonInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip c_tlToolTip;
        private System.Windows.Forms.Label c_lblEnabledFor;
        private System.Windows.Forms.Button c_btnReset;
        private System.Windows.Forms.CheckedListBox c_clbAddonStatus;
        private System.Windows.Forms.Button c_btnEnableAllAddons;
        private System.Windows.Forms.Button c_btnDisableAllAddons;
        private System.Windows.Forms.Button c_btnUninstallAddon;
        private System.Windows.Forms.TextBox c_txtAddonVersion;
        private System.Windows.Forms.TextBox c_txtAddonTitle;
        private System.Windows.Forms.Label c_lblAddonTitle;
        private System.Windows.Forms.Label c_lblAddonVersion;
        private System.Windows.Forms.ListBox c_lbAddons;
        private System.Windows.Forms.Button c_btnSaveAllChanges;
        private System.Windows.Forms.ListBox c_lbNeededFor;
        private System.Windows.Forms.Label c_lblNeededFor;
        private System.Windows.Forms.Label c_lblNotes;
        private System.Windows.Forms.TextBox c_txtNotes;
        private System.Windows.Forms.TextBox c_txtAuthor;
        private System.Windows.Forms.Label c_lblAuthor;
        private System.Windows.Forms.GroupBox c_gbAddonInfo;
        private System.Windows.Forms.Button c_btnReinstallAddon;
        private System.Windows.Forms.Button c_btnCancel;
        private System.Windows.Forms.Button c_btnCleanWTF;
    }
}