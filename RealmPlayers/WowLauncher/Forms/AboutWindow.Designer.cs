namespace VF_WoWLauncher
{
    partial class AboutWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
            this.c_lblProgramName = new System.Windows.Forms.Label();
            this.c_lblProgramVersion = new System.Windows.Forms.Label();
            this.c_lblProgramDescription = new System.Windows.Forms.Label();
            this.c_btnDonate = new System.Windows.Forms.Button();
            this.c_btnClose = new System.Windows.Forms.Button();
            this.c_btnAboutRealmplayers = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // c_lblProgramName
            // 
            this.c_lblProgramName.AutoSize = true;
            this.c_lblProgramName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_lblProgramName.Location = new System.Drawing.Point(12, 9);
            this.c_lblProgramName.Name = "c_lblProgramName";
            this.c_lblProgramName.Size = new System.Drawing.Size(141, 20);
            this.c_lblProgramName.TabIndex = 0;
            this.c_lblProgramName.Text = "VF_WowLauncher";
            // 
            // c_lblProgramVersion
            // 
            this.c_lblProgramVersion.AutoSize = true;
            this.c_lblProgramVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_lblProgramVersion.Location = new System.Drawing.Point(12, 29);
            this.c_lblProgramVersion.Name = "c_lblProgramVersion";
            this.c_lblProgramVersion.Size = new System.Drawing.Size(87, 16);
            this.c_lblProgramVersion.TabIndex = 1;
            this.c_lblProgramVersion.Text = "Version: 1.1.2";
            // 
            // c_lblProgramDescription
            // 
            this.c_lblProgramDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_lblProgramDescription.Location = new System.Drawing.Point(12, 61);
            this.c_lblProgramDescription.Name = "c_lblProgramDescription";
            this.c_lblProgramDescription.Size = new System.Drawing.Size(367, 115);
            this.c_lblProgramDescription.TabIndex = 2;
            this.c_lblProgramDescription.Text = resources.GetString("c_lblProgramDescription.Text");
            this.c_lblProgramDescription.Click += new System.EventHandler(this.c_lblProgramDescription_Click);
            // 
            // c_btnDonate
            // 
            this.c_btnDonate.Location = new System.Drawing.Point(223, 182);
            this.c_btnDonate.Name = "c_btnDonate";
            this.c_btnDonate.Size = new System.Drawing.Size(75, 23);
            this.c_btnDonate.TabIndex = 3;
            this.c_btnDonate.Text = "Donate";
            this.c_btnDonate.UseVisualStyleBackColor = true;
            this.c_btnDonate.Click += new System.EventHandler(this.c_btnDonate_Click);
            // 
            // c_btnClose
            // 
            this.c_btnClose.Location = new System.Drawing.Point(304, 182);
            this.c_btnClose.Name = "c_btnClose";
            this.c_btnClose.Size = new System.Drawing.Size(75, 23);
            this.c_btnClose.TabIndex = 4;
            this.c_btnClose.Text = "Close";
            this.c_btnClose.UseVisualStyleBackColor = true;
            this.c_btnClose.Click += new System.EventHandler(this.c_btnClose_Click);
            // 
            // c_btnAboutRealmplayers
            // 
            this.c_btnAboutRealmplayers.Location = new System.Drawing.Point(12, 182);
            this.c_btnAboutRealmplayers.Name = "c_btnAboutRealmplayers";
            this.c_btnAboutRealmplayers.Size = new System.Drawing.Size(141, 23);
            this.c_btnAboutRealmplayers.TabIndex = 5;
            this.c_btnAboutRealmplayers.Text = "About realmplayers.com";
            this.c_btnAboutRealmplayers.UseVisualStyleBackColor = true;
            this.c_btnAboutRealmplayers.Click += new System.EventHandler(this.c_btnAboutRealmplayers_Click);
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 217);
            this.Controls.Add(this.c_btnAboutRealmplayers);
            this.Controls.Add(this.c_btnClose);
            this.Controls.Add(this.c_btnDonate);
            this.Controls.Add(this.c_lblProgramDescription);
            this.Controls.Add(this.c_lblProgramVersion);
            this.Controls.Add(this.c_lblProgramName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AboutWindow";
            this.Text = "About";
            this.Load += new System.EventHandler(this.AboutWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label c_lblProgramName;
        private System.Windows.Forms.Label c_lblProgramVersion;
        private System.Windows.Forms.Label c_lblProgramDescription;
        private System.Windows.Forms.Button c_btnDonate;
        private System.Windows.Forms.Button c_btnClose;
        private System.Windows.Forms.Button c_btnAboutRealmplayers;
    }
}