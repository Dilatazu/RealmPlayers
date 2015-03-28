namespace VF_WoWLauncher
{
    partial class ApplicationSettingsForm
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
            this.c_btnClose = new System.Windows.Forms.Button();
            this.c_lblWowDirectory = new System.Windows.Forms.Label();
            this.c_btnWowDirectoryBrowse = new System.Windows.Forms.Button();
            this.c_txtWowDirectory = new System.Windows.Forms.TextBox();
            this.c_gpbUploading = new System.Windows.Forms.GroupBox();
            this.c_lblUserID = new System.Windows.Forms.Label();
            this.c_btnChangeUserID = new System.Windows.Forms.Button();
            this.c_txtUserID = new System.Windows.Forms.TextBox();
            this.c_cbxWoWNoDelay = new System.Windows.Forms.CheckBox();
            this.c_lblTBCWowDirectory = new System.Windows.Forms.Label();
            this.c_btnTBCWowDirectoryBrowse = new System.Windows.Forms.Button();
            this.c_txtTBCWowDirectory = new System.Windows.Forms.TextBox();
            this.c_cbxEnableTBC = new System.Windows.Forms.CheckBox();
            this.c_cbAutoRefresh = new System.Windows.Forms.CheckBox();
            this.c_cbxRunNotAdmin = new System.Windows.Forms.CheckBox();
            this.c_cbxAutoHide = new System.Windows.Forms.CheckBox();
            this.c_cbAutoUpdateVF = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.c_cbxFeenixNews = new System.Windows.Forms.CheckBox();
            this.c_cbxNostalriusNews = new System.Windows.Forms.CheckBox();
            this.c_gpbUploading.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // c_btnClose
            // 
            this.c_btnClose.Location = new System.Drawing.Point(330, 320);
            this.c_btnClose.Name = "c_btnClose";
            this.c_btnClose.Size = new System.Drawing.Size(75, 23);
            this.c_btnClose.TabIndex = 3;
            this.c_btnClose.Text = "Close";
            this.c_btnClose.UseVisualStyleBackColor = true;
            this.c_btnClose.Click += new System.EventHandler(this.c_btnClose_Click);
            // 
            // c_lblWowDirectory
            // 
            this.c_lblWowDirectory.AutoSize = true;
            this.c_lblWowDirectory.Location = new System.Drawing.Point(7, 188);
            this.c_lblWowDirectory.Name = "c_lblWowDirectory";
            this.c_lblWowDirectory.Size = new System.Drawing.Size(122, 13);
            this.c_lblWowDirectory.TabIndex = 6;
            this.c_lblWowDirectory.Text = "WoW Classic Directory: ";
            // 
            // c_btnWowDirectoryBrowse
            // 
            this.c_btnWowDirectoryBrowse.Location = new System.Drawing.Point(342, 204);
            this.c_btnWowDirectoryBrowse.Name = "c_btnWowDirectoryBrowse";
            this.c_btnWowDirectoryBrowse.Size = new System.Drawing.Size(63, 36);
            this.c_btnWowDirectoryBrowse.TabIndex = 5;
            this.c_btnWowDirectoryBrowse.Text = "Browse";
            this.c_btnWowDirectoryBrowse.UseVisualStyleBackColor = true;
            this.c_btnWowDirectoryBrowse.Click += new System.EventHandler(this.c_btnWowDirectoryBrowse_Click);
            // 
            // c_txtWowDirectory
            // 
            this.c_txtWowDirectory.Location = new System.Drawing.Point(10, 204);
            this.c_txtWowDirectory.Multiline = true;
            this.c_txtWowDirectory.Name = "c_txtWowDirectory";
            this.c_txtWowDirectory.Size = new System.Drawing.Size(326, 34);
            this.c_txtWowDirectory.TabIndex = 4;
            this.c_txtWowDirectory.Text = "Test1TTest2\r\nTest3";
            this.c_txtWowDirectory.TextChanged += new System.EventHandler(this.c_txtWowDirectory_TextChanged);
            // 
            // c_gpbUploading
            // 
            this.c_gpbUploading.Controls.Add(this.c_lblUserID);
            this.c_gpbUploading.Controls.Add(this.c_btnChangeUserID);
            this.c_gpbUploading.Controls.Add(this.c_txtUserID);
            this.c_gpbUploading.Location = new System.Drawing.Point(10, 12);
            this.c_gpbUploading.Name = "c_gpbUploading";
            this.c_gpbUploading.Size = new System.Drawing.Size(394, 50);
            this.c_gpbUploading.TabIndex = 7;
            this.c_gpbUploading.TabStop = false;
            this.c_gpbUploading.Text = "Data Uploading";
            // 
            // c_lblUserID
            // 
            this.c_lblUserID.AutoSize = true;
            this.c_lblUserID.Location = new System.Drawing.Point(127, 25);
            this.c_lblUserID.Name = "c_lblUserID";
            this.c_lblUserID.Size = new System.Drawing.Size(46, 13);
            this.c_lblUserID.TabIndex = 5;
            this.c_lblUserID.Text = "UserID: ";
            // 
            // c_btnChangeUserID
            // 
            this.c_btnChangeUserID.Location = new System.Drawing.Point(324, 19);
            this.c_btnChangeUserID.Name = "c_btnChangeUserID";
            this.c_btnChangeUserID.Size = new System.Drawing.Size(64, 23);
            this.c_btnChangeUserID.TabIndex = 4;
            this.c_btnChangeUserID.Text = "Change";
            this.c_btnChangeUserID.UseVisualStyleBackColor = true;
            this.c_btnChangeUserID.Click += new System.EventHandler(this.c_btnChangeUserID_Click);
            // 
            // c_txtUserID
            // 
            this.c_txtUserID.Location = new System.Drawing.Point(179, 22);
            this.c_txtUserID.Name = "c_txtUserID";
            this.c_txtUserID.ReadOnly = true;
            this.c_txtUserID.Size = new System.Drawing.Size(139, 20);
            this.c_txtUserID.TabIndex = 3;
            // 
            // c_cbxWoWNoDelay
            // 
            this.c_cbxWoWNoDelay.AutoSize = true;
            this.c_cbxWoWNoDelay.Location = new System.Drawing.Point(176, 324);
            this.c_cbxWoWNoDelay.Name = "c_cbxWoWNoDelay";
            this.c_cbxWoWNoDelay.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.c_cbxWoWNoDelay.Size = new System.Drawing.Size(148, 17);
            this.c_cbxWoWNoDelay.TabIndex = 8;
            this.c_cbxWoWNoDelay.Text = "Remove WoW start delay";
            this.c_cbxWoWNoDelay.UseVisualStyleBackColor = true;
            this.c_cbxWoWNoDelay.Visible = false;
            this.c_cbxWoWNoDelay.CheckedChanged += new System.EventHandler(this.c_cbxWoWNoDelay_CheckedChanged);
            // 
            // c_lblTBCWowDirectory
            // 
            this.c_lblTBCWowDirectory.AutoSize = true;
            this.c_lblTBCWowDirectory.Location = new System.Drawing.Point(8, 249);
            this.c_lblTBCWowDirectory.Name = "c_lblTBCWowDirectory";
            this.c_lblTBCWowDirectory.Size = new System.Drawing.Size(110, 13);
            this.c_lblTBCWowDirectory.TabIndex = 11;
            this.c_lblTBCWowDirectory.Text = "TBC WoW Directory: ";
            // 
            // c_btnTBCWowDirectoryBrowse
            // 
            this.c_btnTBCWowDirectoryBrowse.Location = new System.Drawing.Point(343, 265);
            this.c_btnTBCWowDirectoryBrowse.Name = "c_btnTBCWowDirectoryBrowse";
            this.c_btnTBCWowDirectoryBrowse.Size = new System.Drawing.Size(63, 36);
            this.c_btnTBCWowDirectoryBrowse.TabIndex = 10;
            this.c_btnTBCWowDirectoryBrowse.Text = "Browse";
            this.c_btnTBCWowDirectoryBrowse.UseVisualStyleBackColor = true;
            this.c_btnTBCWowDirectoryBrowse.Click += new System.EventHandler(this.c_btnTBCWowDirectoryBrowse_Click);
            // 
            // c_txtTBCWowDirectory
            // 
            this.c_txtTBCWowDirectory.Location = new System.Drawing.Point(11, 265);
            this.c_txtTBCWowDirectory.Multiline = true;
            this.c_txtTBCWowDirectory.Name = "c_txtTBCWowDirectory";
            this.c_txtTBCWowDirectory.Size = new System.Drawing.Size(326, 34);
            this.c_txtTBCWowDirectory.TabIndex = 9;
            this.c_txtTBCWowDirectory.Text = "Test1TTest2\r\nTest3";
            this.c_txtTBCWowDirectory.TextChanged += new System.EventHandler(this.c_txtTBCWowDirectory_TextChanged);
            // 
            // c_cbxEnableTBC
            // 
            this.c_cbxEnableTBC.AutoSize = true;
            this.c_cbxEnableTBC.Location = new System.Drawing.Point(116, 248);
            this.c_cbxEnableTBC.Name = "c_cbxEnableTBC";
            this.c_cbxEnableTBC.Size = new System.Drawing.Size(59, 17);
            this.c_cbxEnableTBC.TabIndex = 12;
            this.c_cbxEnableTBC.Text = "Enable";
            this.c_cbxEnableTBC.UseVisualStyleBackColor = true;
            this.c_cbxEnableTBC.CheckedChanged += new System.EventHandler(this.c_cbxEnableTBC_CheckedChanged);
            // 
            // c_cbAutoRefresh
            // 
            this.c_cbAutoRefresh.AutoSize = true;
            this.c_cbAutoRefresh.Location = new System.Drawing.Point(10, 68);
            this.c_cbAutoRefresh.Name = "c_cbAutoRefresh";
            this.c_cbAutoRefresh.Size = new System.Drawing.Size(173, 17);
            this.c_cbAutoRefresh.TabIndex = 13;
            this.c_cbAutoRefresh.Text = "Poll forums for news every hour";
            this.c_cbAutoRefresh.UseVisualStyleBackColor = true;
            this.c_cbAutoRefresh.CheckedChanged += new System.EventHandler(this.c_cbAutoRefresh_CheckedChanged);
            // 
            // c_cbxRunNotAdmin
            // 
            this.c_cbxRunNotAdmin.AutoSize = true;
            this.c_cbxRunNotAdmin.Location = new System.Drawing.Point(11, 324);
            this.c_cbxRunNotAdmin.Name = "c_cbxRunNotAdmin";
            this.c_cbxRunNotAdmin.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.c_cbxRunNotAdmin.Size = new System.Drawing.Size(143, 17);
            this.c_cbxRunNotAdmin.TabIndex = 14;
            this.c_cbxRunNotAdmin.Text = "Dont run WoW as admin";
            this.c_cbxRunNotAdmin.UseVisualStyleBackColor = true;
            this.c_cbxRunNotAdmin.CheckedChanged += new System.EventHandler(this.c_cbxRunNotAdmin_CheckedChanged);
            // 
            // c_cbxAutoHide
            // 
            this.c_cbxAutoHide.AutoSize = true;
            this.c_cbxAutoHide.Enabled = false;
            this.c_cbxAutoHide.Location = new System.Drawing.Point(13, 335);
            this.c_cbxAutoHide.Name = "c_cbxAutoHide";
            this.c_cbxAutoHide.Size = new System.Drawing.Size(116, 17);
            this.c_cbxAutoHide.TabIndex = 15;
            this.c_cbxAutoHide.Text = "Auto hide old news";
            this.c_cbxAutoHide.UseVisualStyleBackColor = true;
            this.c_cbxAutoHide.Visible = false;
            this.c_cbxAutoHide.CheckedChanged += new System.EventHandler(this.c_cbxAutoHide_CheckedChanged);
            // 
            // c_cbAutoUpdateVF
            // 
            this.c_cbAutoUpdateVF.AutoSize = true;
            this.c_cbAutoUpdateVF.Location = new System.Drawing.Point(10, 156);
            this.c_cbAutoUpdateVF.Name = "c_cbAutoUpdateVF";
            this.c_cbAutoUpdateVF.Size = new System.Drawing.Size(229, 17);
            this.c_cbAutoUpdateVF.TabIndex = 16;
            this.c_cbAutoUpdateVF.Text = "Automatically update RealmPlayers addons";
            this.c_cbAutoUpdateVF.UseVisualStyleBackColor = true;
            this.c_cbAutoUpdateVF.Visible = false;
            this.c_cbAutoUpdateVF.CheckedChanged += new System.EventHandler(this.c_cbAutoUpdateVF_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.c_cbxNostalriusNews);
            this.groupBox1.Controls.Add(this.c_cbxFeenixNews);
            this.groupBox1.Location = new System.Drawing.Point(10, 91);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(394, 45);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Realm Forum news from sources";
            // 
            // c_cbxFeenixNews
            // 
            this.c_cbxFeenixNews.AutoSize = true;
            this.c_cbxFeenixNews.Location = new System.Drawing.Point(6, 19);
            this.c_cbxFeenixNews.Name = "c_cbxFeenixNews";
            this.c_cbxFeenixNews.Size = new System.Drawing.Size(106, 17);
            this.c_cbxFeenixNews.TabIndex = 14;
            this.c_cbxFeenixNews.Text = "Feenix(wow-one)";
            this.c_cbxFeenixNews.UseVisualStyleBackColor = true;
            this.c_cbxFeenixNews.CheckedChanged += new System.EventHandler(this.c_cbxGetFeenixNews_CheckedChanged);
            // 
            // c_cbxNostalriusNews
            // 
            this.c_cbxNostalriusNews.AutoSize = true;
            this.c_cbxNostalriusNews.Location = new System.Drawing.Point(118, 19);
            this.c_cbxNostalriusNews.Name = "c_cbxNostalriusNews";
            this.c_cbxNostalriusNews.Size = new System.Drawing.Size(72, 17);
            this.c_cbxNostalriusNews.TabIndex = 15;
            this.c_cbxNostalriusNews.Text = "Nostalrius";
            this.c_cbxNostalriusNews.UseVisualStyleBackColor = true;
            this.c_cbxNostalriusNews.CheckedChanged += new System.EventHandler(this.c_cbxNostalriusNews_CheckedChanged);
            // 
            // ApplicationSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 353);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.c_cbAutoRefresh);
            this.Controls.Add(this.c_cbAutoUpdateVF);
            this.Controls.Add(this.c_cbxAutoHide);
            this.Controls.Add(this.c_cbxRunNotAdmin);
            this.Controls.Add(this.c_cbxEnableTBC);
            this.Controls.Add(this.c_lblTBCWowDirectory);
            this.Controls.Add(this.c_btnTBCWowDirectoryBrowse);
            this.Controls.Add(this.c_txtTBCWowDirectory);
            this.Controls.Add(this.c_cbxWoWNoDelay);
            this.Controls.Add(this.c_gpbUploading);
            this.Controls.Add(this.c_lblWowDirectory);
            this.Controls.Add(this.c_btnWowDirectoryBrowse);
            this.Controls.Add(this.c_txtWowDirectory);
            this.Controls.Add(this.c_btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ApplicationSettingsForm";
            this.Text = "Application Settings";
            this.Load += new System.EventHandler(this.ApplicationSettingsForm_Load);
            this.c_gpbUploading.ResumeLayout(false);
            this.c_gpbUploading.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button c_btnClose;
        private System.Windows.Forms.Label c_lblWowDirectory;
        private System.Windows.Forms.Button c_btnWowDirectoryBrowse;
        private System.Windows.Forms.TextBox c_txtWowDirectory;
        private System.Windows.Forms.GroupBox c_gpbUploading;
        private System.Windows.Forms.Label c_lblUserID;
        private System.Windows.Forms.Button c_btnChangeUserID;
        private System.Windows.Forms.TextBox c_txtUserID;
        private System.Windows.Forms.CheckBox c_cbxWoWNoDelay;
        private System.Windows.Forms.Label c_lblTBCWowDirectory;
        private System.Windows.Forms.Button c_btnTBCWowDirectoryBrowse;
        private System.Windows.Forms.TextBox c_txtTBCWowDirectory;
        private System.Windows.Forms.CheckBox c_cbxEnableTBC;
        private System.Windows.Forms.CheckBox c_cbAutoRefresh;
        private System.Windows.Forms.CheckBox c_cbxRunNotAdmin;
        private System.Windows.Forms.CheckBox c_cbxAutoHide;
        private System.Windows.Forms.CheckBox c_cbAutoUpdateVF;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox c_cbxFeenixNews;
        private System.Windows.Forms.CheckBox c_cbxNostalriusNews;

    }
}