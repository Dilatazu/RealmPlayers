namespace VF_WoWLauncher
{
    partial class AccountSettingsForm
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
            this.c_lbAccounts = new System.Windows.Forms.ListBox();
            this.c_lbCharacters = new System.Windows.Forms.ListBox();
            this.c_ddlRealm = new System.Windows.Forms.ComboBox();
            this.c_btnDisableAllAddons = new System.Windows.Forms.Button();
            this.c_btnEnableAllAddons = new System.Windows.Forms.Button();
            this.c_btnSaveAddonConfig = new System.Windows.Forms.Button();
            this.c_clbAddons = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // c_lbAccounts
            // 
            this.c_lbAccounts.FormattingEnabled = true;
            this.c_lbAccounts.Location = new System.Drawing.Point(12, 35);
            this.c_lbAccounts.Name = "c_lbAccounts";
            this.c_lbAccounts.Size = new System.Drawing.Size(151, 147);
            this.c_lbAccounts.TabIndex = 0;
            this.c_lbAccounts.SelectedIndexChanged += new System.EventHandler(this.c_lbAccounts_SelectedIndexChanged);
            // 
            // c_lbCharacters
            // 
            this.c_lbCharacters.FormattingEnabled = true;
            this.c_lbCharacters.Location = new System.Drawing.Point(169, 35);
            this.c_lbCharacters.Name = "c_lbCharacters";
            this.c_lbCharacters.Size = new System.Drawing.Size(172, 147);
            this.c_lbCharacters.TabIndex = 1;
            this.c_lbCharacters.SelectedIndexChanged += new System.EventHandler(this.c_lbCharacters_SelectedIndexChanged);
            // 
            // c_ddlRealm
            // 
            this.c_ddlRealm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_ddlRealm.FormattingEnabled = true;
            this.c_ddlRealm.Items.AddRange(new object[] {
            "Emerald Dream",
            "Warsong",
            "Al\'Akir"});
            this.c_ddlRealm.Location = new System.Drawing.Point(12, 8);
            this.c_ddlRealm.Name = "c_ddlRealm";
            this.c_ddlRealm.Size = new System.Drawing.Size(179, 21);
            this.c_ddlRealm.TabIndex = 2;
            this.c_ddlRealm.SelectedIndexChanged += new System.EventHandler(this.c_ddlRealm_SelectedIndexChanged);
            // 
            // c_btnDisableAllAddons
            // 
            this.c_btnDisableAllAddons.Location = new System.Drawing.Point(12, 573);
            this.c_btnDisableAllAddons.Name = "c_btnDisableAllAddons";
            this.c_btnDisableAllAddons.Size = new System.Drawing.Size(103, 23);
            this.c_btnDisableAllAddons.TabIndex = 4;
            this.c_btnDisableAllAddons.Text = "Disable all Addons";
            this.c_btnDisableAllAddons.UseVisualStyleBackColor = true;
            this.c_btnDisableAllAddons.Click += new System.EventHandler(this.c_btnDisableAllAddons_Click);
            // 
            // c_btnEnableAllAddons
            // 
            this.c_btnEnableAllAddons.Location = new System.Drawing.Point(130, 573);
            this.c_btnEnableAllAddons.Name = "c_btnEnableAllAddons";
            this.c_btnEnableAllAddons.Size = new System.Drawing.Size(106, 23);
            this.c_btnEnableAllAddons.TabIndex = 5;
            this.c_btnEnableAllAddons.Text = "Enable all Addons";
            this.c_btnEnableAllAddons.UseVisualStyleBackColor = true;
            this.c_btnEnableAllAddons.Click += new System.EventHandler(this.c_btnEnableAllAddons_Click);
            // 
            // c_btnSaveAddonConfig
            // 
            this.c_btnSaveAddonConfig.Location = new System.Drawing.Point(242, 304);
            this.c_btnSaveAddonConfig.Name = "c_btnSaveAddonConfig";
            this.c_btnSaveAddonConfig.Size = new System.Drawing.Size(123, 23);
            this.c_btnSaveAddonConfig.TabIndex = 6;
            this.c_btnSaveAddonConfig.Text = "Save Addon Config";
            this.c_btnSaveAddonConfig.UseVisualStyleBackColor = true;
            // 
            // c_clbAddons
            // 
            this.c_clbAddons.FormattingEnabled = true;
            this.c_clbAddons.Location = new System.Drawing.Point(12, 188);
            this.c_clbAddons.Name = "c_clbAddons";
            this.c_clbAddons.Size = new System.Drawing.Size(224, 379);
            this.c_clbAddons.TabIndex = 7;
            // 
            // AccountSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 604);
            this.Controls.Add(this.c_clbAddons);
            this.Controls.Add(this.c_btnSaveAddonConfig);
            this.Controls.Add(this.c_btnEnableAllAddons);
            this.Controls.Add(this.c_btnDisableAllAddons);
            this.Controls.Add(this.c_ddlRealm);
            this.Controls.Add(this.c_lbCharacters);
            this.Controls.Add(this.c_lbAccounts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AccountSettingsForm";
            this.Text = "Addons Settings";
            this.Load += new System.EventHandler(this.AccountSettingsForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox c_lbAccounts;
        private System.Windows.Forms.ListBox c_lbCharacters;
        private System.Windows.Forms.ComboBox c_ddlRealm;
        private System.Windows.Forms.Button c_btnDisableAllAddons;
        private System.Windows.Forms.Button c_btnEnableAllAddons;
        private System.Windows.Forms.Button c_btnSaveAddonConfig;
        private System.Windows.Forms.CheckedListBox c_clbAddons;
    }
}