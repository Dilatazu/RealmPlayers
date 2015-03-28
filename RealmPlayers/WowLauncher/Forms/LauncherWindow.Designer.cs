namespace VF_WoWLauncher
{
    partial class LauncherWindow
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
            this.c_btnLaunch = new System.Windows.Forms.Button();
            this.c_ddlRealm = new System.Windows.Forms.ComboBox();
            this.c_btnConfig = new System.Windows.Forms.Button();
            this.c_ddlConfigProfile = new System.Windows.Forms.ComboBox();
            this.c_cbClearWDB = new System.Windows.Forms.CheckBox();
            this.c_btnManageAddons = new System.Windows.Forms.Button();
            this.c_dlAddons = new DetailedList.DetailedList();
            this.c_tRefreshNews = new System.Windows.Forms.Timer(this.components);
            this.c_tRefreshAddonUpdates = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // c_btnLaunch
            // 
            this.c_btnLaunch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.c_btnLaunch.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_btnLaunch.Location = new System.Drawing.Point(561, 398);
            this.c_btnLaunch.Name = "c_btnLaunch";
            this.c_btnLaunch.Size = new System.Drawing.Size(75, 51);
            this.c_btnLaunch.TabIndex = 0;
            this.c_btnLaunch.Text = "Play";
            this.c_btnLaunch.UseVisualStyleBackColor = true;
            this.c_btnLaunch.Click += new System.EventHandler(this.c_btnLaunch_Click);
            // 
            // c_ddlRealm
            // 
            this.c_ddlRealm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.c_ddlRealm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_ddlRealm.FormattingEnabled = true;
            this.c_ddlRealm.Location = new System.Drawing.Point(378, 427);
            this.c_ddlRealm.Name = "c_ddlRealm";
            this.c_ddlRealm.Size = new System.Drawing.Size(179, 21);
            this.c_ddlRealm.TabIndex = 1;
            this.c_ddlRealm.SelectedIndexChanged += new System.EventHandler(this.c_ddlRealm_SelectedIndexChanged);
            // 
            // c_btnConfig
            // 
            this.c_btnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.c_btnConfig.Location = new System.Drawing.Point(297, 400);
            this.c_btnConfig.Name = "c_btnConfig";
            this.c_btnConfig.Size = new System.Drawing.Size(75, 21);
            this.c_btnConfig.TabIndex = 3;
            this.c_btnConfig.Text = "Config";
            this.c_btnConfig.UseVisualStyleBackColor = true;
            this.c_btnConfig.Click += new System.EventHandler(this.c_btnConfig_Click);
            // 
            // c_ddlConfigProfile
            // 
            this.c_ddlConfigProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.c_ddlConfigProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_ddlConfigProfile.FormattingEnabled = true;
            this.c_ddlConfigProfile.Items.AddRange(new object[] {
            "Emerald Dream",
            "Warsong",
            "Al\'Akir"});
            this.c_ddlConfigProfile.Location = new System.Drawing.Point(378, 400);
            this.c_ddlConfigProfile.Name = "c_ddlConfigProfile";
            this.c_ddlConfigProfile.Size = new System.Drawing.Size(179, 21);
            this.c_ddlConfigProfile.TabIndex = 4;
            this.c_ddlConfigProfile.SelectedIndexChanged += new System.EventHandler(this.c_ddlConfigProfile_SelectedIndexChanged);
            // 
            // c_cbClearWDB
            // 
            this.c_cbClearWDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.c_cbClearWDB.AutoSize = true;
            this.c_cbClearWDB.Location = new System.Drawing.Point(293, 431);
            this.c_cbClearWDB.Name = "c_cbClearWDB";
            this.c_cbClearWDB.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.c_cbClearWDB.Size = new System.Drawing.Size(79, 17);
            this.c_cbClearWDB.TabIndex = 5;
            this.c_cbClearWDB.Text = "Clear WDB";
            this.c_cbClearWDB.UseVisualStyleBackColor = true;
            this.c_cbClearWDB.CheckedChanged += new System.EventHandler(this.c_cbClearWDB_CheckedChanged);
            // 
            // c_btnManageAddons
            // 
            this.c_btnManageAddons.Location = new System.Drawing.Point(9, 398);
            this.c_btnManageAddons.Name = "c_btnManageAddons";
            this.c_btnManageAddons.Size = new System.Drawing.Size(105, 23);
            this.c_btnManageAddons.TabIndex = 10;
            this.c_btnManageAddons.Text = "Manage Addons";
            this.c_btnManageAddons.UseVisualStyleBackColor = true;
            this.c_btnManageAddons.Click += new System.EventHandler(this.c_btnManageAddons_Click);
            // 
            // c_dlAddons
            // 
            this.c_dlAddons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.c_dlAddons.BackColor = System.Drawing.SystemColors.Window;
            this.c_dlAddons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c_dlAddons.Location = new System.Drawing.Point(9, 9);
            this.c_dlAddons.Margin = new System.Windows.Forms.Padding(0);
            this.c_dlAddons.Name = "c_dlAddons";
            this.c_dlAddons.Size = new System.Drawing.Size(627, 374);
            this.c_dlAddons.TabIndex = 9;
            this.c_dlAddons.Load += new System.EventHandler(this.c_dlAddons_Load);
            // 
            // c_tRefreshNews
            // 
            this.c_tRefreshNews.Tick += new System.EventHandler(this.c_tRefreshNews_Tick);
            // 
            // c_tRefreshAddonUpdates
            // 
            this.c_tRefreshAddonUpdates.Tick += new System.EventHandler(this.c_tRefreshAddonUpdates_Tick);
            // 
            // LauncherWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 457);
            this.Controls.Add(this.c_btnManageAddons);
            this.Controls.Add(this.c_dlAddons);
            this.Controls.Add(this.c_cbClearWDB);
            this.Controls.Add(this.c_ddlConfigProfile);
            this.Controls.Add(this.c_btnConfig);
            this.Controls.Add(this.c_ddlRealm);
            this.Controls.Add(this.c_btnLaunch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LauncherWindow";
            this.Text = "LauncherWindow";
            this.Load += new System.EventHandler(this.LauncherWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button c_btnLaunch;
        private System.Windows.Forms.ComboBox c_ddlRealm;
        private System.Windows.Forms.Button c_btnConfig;
        private System.Windows.Forms.ComboBox c_ddlConfigProfile;
        private System.Windows.Forms.CheckBox c_cbClearWDB;
        private DetailedList.DetailedList c_dlAddons;
        private System.Windows.Forms.Button c_btnManageAddons;
        private System.Windows.Forms.Timer c_tRefreshNews;
        private System.Windows.Forms.Timer c_tRefreshAddonUpdates;
    }
}

