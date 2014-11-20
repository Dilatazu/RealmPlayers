namespace VF_WowLauncherInstaller
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.c_txtInstallDirectory = new System.Windows.Forms.TextBox();
            this.c_btnBrowse = new System.Windows.Forms.Button();
            this.c_lblInstallDirectory = new System.Windows.Forms.Label();
            this.c_btnInstall = new System.Windows.Forms.Button();
            this.c_lblWelcome = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // c_txtInstallDirectory
            // 
            this.c_txtInstallDirectory.Location = new System.Drawing.Point(12, 117);
            this.c_txtInstallDirectory.Name = "c_txtInstallDirectory";
            this.c_txtInstallDirectory.Size = new System.Drawing.Size(433, 20);
            this.c_txtInstallDirectory.TabIndex = 0;
            this.c_txtInstallDirectory.Text = "C:\\Program Files\\VF_WowLauncher\\";
            this.c_txtInstallDirectory.TextChanged += new System.EventHandler(this.c_txtInstallDirectory_TextChanged);
            // 
            // c_btnBrowse
            // 
            this.c_btnBrowse.Location = new System.Drawing.Point(451, 116);
            this.c_btnBrowse.Name = "c_btnBrowse";
            this.c_btnBrowse.Size = new System.Drawing.Size(54, 21);
            this.c_btnBrowse.TabIndex = 1;
            this.c_btnBrowse.Text = "Browse";
            this.c_btnBrowse.UseVisualStyleBackColor = true;
            this.c_btnBrowse.Click += new System.EventHandler(this.c_btnBrowse_Click);
            // 
            // c_lblInstallDirectory
            // 
            this.c_lblInstallDirectory.Location = new System.Drawing.Point(12, 187);
            this.c_lblInstallDirectory.Name = "c_lblInstallDirectory";
            this.c_lblInstallDirectory.Size = new System.Drawing.Size(417, 40);
            this.c_lblInstallDirectory.TabIndex = 2;
            this.c_lblInstallDirectory.Text = "VF_WowLauncher will be installed in the folder: \r\nC:\\Program Files\\VF_WowLauncher" +
    "\\\r\nt";
            // 
            // c_btnInstall
            // 
            this.c_btnInstall.Location = new System.Drawing.Point(435, 200);
            this.c_btnInstall.Name = "c_btnInstall";
            this.c_btnInstall.Size = new System.Drawing.Size(70, 23);
            this.c_btnInstall.TabIndex = 3;
            this.c_btnInstall.Text = "Install";
            this.c_btnInstall.UseVisualStyleBackColor = true;
            this.c_btnInstall.Click += new System.EventHandler(this.c_btnInstall_Click);
            // 
            // c_lblWelcome
            // 
            this.c_lblWelcome.Location = new System.Drawing.Point(12, 13);
            this.c_lblWelcome.Name = "c_lblWelcome";
            this.c_lblWelcome.Size = new System.Drawing.Size(493, 100);
            this.c_lblWelcome.TabIndex = 4;
            this.c_lblWelcome.Text = resources.GetString("c_lblWelcome.Text");
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.Location = new System.Drawing.Point(276, 52);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(190, 13);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://RealmPlayers.com/Donate.aspx";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 232);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.c_lblWelcome);
            this.Controls.Add(this.c_btnInstall);
            this.Controls.Add(this.c_lblInstallDirectory);
            this.Controls.Add(this.c_btnBrowse);
            this.Controls.Add(this.c_txtInstallDirectory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainWindow";
            this.Text = "Installation of VF_WowLauncher";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox c_txtInstallDirectory;
        private System.Windows.Forms.Button c_btnBrowse;
        private System.Windows.Forms.Label c_lblInstallDirectory;
        private System.Windows.Forms.Button c_btnInstall;
        private System.Windows.Forms.Label c_lblWelcome;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

