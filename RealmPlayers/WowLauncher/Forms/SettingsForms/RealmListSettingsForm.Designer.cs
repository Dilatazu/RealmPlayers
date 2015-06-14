namespace VF_WoWLauncher
{
    partial class RealmListSettingsForm
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
            this.c_lstRealmConfigurations = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.c_txtNewConfigName = new System.Windows.Forms.TextBox();
            this.c_btnAddNewRealmConfig = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.c_cbxWowVersion = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.c_txtNewRealmName = new System.Windows.Forms.TextBox();
            this.c_txtNewRealmListURL = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.c_txtSelectedWowVersion = new System.Windows.Forms.TextBox();
            this.c_txtSelectedRealmListURL = new System.Windows.Forms.TextBox();
            this.c_txtSelectedRealmName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.c_btnClose = new System.Windows.Forms.Button();
            this.c_btnResetRealmLists = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // c_lstRealmConfigurations
            // 
            this.c_lstRealmConfigurations.FormattingEnabled = true;
            this.c_lstRealmConfigurations.Location = new System.Drawing.Point(12, 12);
            this.c_lstRealmConfigurations.Name = "c_lstRealmConfigurations";
            this.c_lstRealmConfigurations.Size = new System.Drawing.Size(168, 225);
            this.c_lstRealmConfigurations.TabIndex = 0;
            this.c_lstRealmConfigurations.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.c_txtNewConfigName);
            this.groupBox1.Controls.Add(this.c_btnAddNewRealmConfig);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.c_cbxWowVersion);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.c_txtNewRealmName);
            this.groupBox1.Controls.Add(this.c_txtNewRealmListURL);
            this.groupBox1.Location = new System.Drawing.Point(12, 245);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 129);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "New Realm Config";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Config Name:";
            // 
            // c_txtNewConfigName
            // 
            this.c_txtNewConfigName.Location = new System.Drawing.Point(87, 21);
            this.c_txtNewConfigName.Name = "c_txtNewConfigName";
            this.c_txtNewConfigName.Size = new System.Drawing.Size(207, 20);
            this.c_txtNewConfigName.TabIndex = 12;
            this.c_txtNewConfigName.TextChanged += new System.EventHandler(this.c_txtNewConfigName_TextChanged);
            this.c_txtNewConfigName.Enter += new System.EventHandler(this.c_txtNewConfigName_Enter);
            this.c_txtNewConfigName.Leave += new System.EventHandler(this.c_txtNewConfigName_Leave);
            // 
            // c_btnAddNewRealmConfig
            // 
            this.c_btnAddNewRealmConfig.Location = new System.Drawing.Point(191, 97);
            this.c_btnAddNewRealmConfig.Name = "c_btnAddNewRealmConfig";
            this.c_btnAddNewRealmConfig.Size = new System.Drawing.Size(103, 21);
            this.c_btnAddNewRealmConfig.TabIndex = 11;
            this.c_btnAddNewRealmConfig.Text = "Add Realm Config";
            this.c_btnAddNewRealmConfig.UseVisualStyleBackColor = true;
            this.c_btnAddNewRealmConfig.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Wow Version: ";
            // 
            // c_cbxWowVersion
            // 
            this.c_cbxWowVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_cbxWowVersion.FormattingEnabled = true;
            this.c_cbxWowVersion.Location = new System.Drawing.Point(87, 97);
            this.c_cbxWowVersion.Name = "c_cbxWowVersion";
            this.c_cbxWowVersion.Size = new System.Drawing.Size(98, 21);
            this.c_cbxWowVersion.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "RealmList(URL): ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Realm Name: ";
            // 
            // c_txtNewRealmName
            // 
            this.c_txtNewRealmName.Location = new System.Drawing.Point(87, 47);
            this.c_txtNewRealmName.Name = "c_txtNewRealmName";
            this.c_txtNewRealmName.Size = new System.Drawing.Size(207, 20);
            this.c_txtNewRealmName.TabIndex = 6;
            this.c_txtNewRealmName.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            this.c_txtNewRealmName.Enter += new System.EventHandler(this.c_txtNewRealmName_Enter);
            this.c_txtNewRealmName.Leave += new System.EventHandler(this.c_txtNewRealmName_Leave);
            // 
            // c_txtNewRealmListURL
            // 
            this.c_txtNewRealmListURL.Location = new System.Drawing.Point(87, 73);
            this.c_txtNewRealmListURL.Name = "c_txtNewRealmListURL";
            this.c_txtNewRealmListURL.Size = new System.Drawing.Size(207, 20);
            this.c_txtNewRealmListURL.TabIndex = 5;
            this.c_txtNewRealmListURL.Enter += new System.EventHandler(this.c_txtNewRealmListURL_Enter);
            this.c_txtNewRealmListURL.Leave += new System.EventHandler(this.c_txtNewRealmListURL_Leave);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.c_txtSelectedWowVersion);
            this.groupBox2.Controls.Add(this.c_txtSelectedRealmListURL);
            this.groupBox2.Controls.Add(this.c_txtSelectedRealmName);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(186, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 140);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selected Realm Configuration";
            // 
            // c_txtSelectedWowVersion
            // 
            this.c_txtSelectedWowVersion.Enabled = false;
            this.c_txtSelectedWowVersion.Location = new System.Drawing.Point(6, 112);
            this.c_txtSelectedWowVersion.Name = "c_txtSelectedWowVersion";
            this.c_txtSelectedWowVersion.Size = new System.Drawing.Size(188, 20);
            this.c_txtSelectedWowVersion.TabIndex = 5;
            // 
            // c_txtSelectedRealmListURL
            // 
            this.c_txtSelectedRealmListURL.Enabled = false;
            this.c_txtSelectedRealmListURL.Location = new System.Drawing.Point(6, 73);
            this.c_txtSelectedRealmListURL.Name = "c_txtSelectedRealmListURL";
            this.c_txtSelectedRealmListURL.Size = new System.Drawing.Size(188, 20);
            this.c_txtSelectedRealmListURL.TabIndex = 4;
            // 
            // c_txtSelectedRealmName
            // 
            this.c_txtSelectedRealmName.Enabled = false;
            this.c_txtSelectedRealmName.Location = new System.Drawing.Point(6, 33);
            this.c_txtSelectedRealmName.Name = "c_txtSelectedRealmName";
            this.c_txtSelectedRealmName.Size = new System.Drawing.Size(188, 20);
            this.c_txtSelectedRealmName.TabIndex = 3;
            this.c_txtSelectedRealmName.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(188, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "Wow Version:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(188, 14);
            this.label6.TabIndex = 1;
            this.label6.Text = "RealmList(URL):";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(188, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "Realm Name:";
            // 
            // c_btnClose
            // 
            this.c_btnClose.Location = new System.Drawing.Point(327, 351);
            this.c_btnClose.Name = "c_btnClose";
            this.c_btnClose.Size = new System.Drawing.Size(75, 23);
            this.c_btnClose.TabIndex = 7;
            this.c_btnClose.Text = "Close";
            this.c_btnClose.UseVisualStyleBackColor = true;
            this.c_btnClose.Click += new System.EventHandler(this.button3_Click);
            // 
            // c_btnResetRealmLists
            // 
            this.c_btnResetRealmLists.Location = new System.Drawing.Point(186, 214);
            this.c_btnResetRealmLists.Name = "c_btnResetRealmLists";
            this.c_btnResetRealmLists.Size = new System.Drawing.Size(200, 23);
            this.c_btnResetRealmLists.TabIndex = 8;
            this.c_btnResetRealmLists.Text = "Reset to Default RealmLists";
            this.c_btnResetRealmLists.UseVisualStyleBackColor = true;
            this.c_btnResetRealmLists.Click += new System.EventHandler(this.c_btnResetRealmLists_Click);
            // 
            // RealmListSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 383);
            this.Controls.Add(this.c_btnResetRealmLists);
            this.Controls.Add(this.c_btnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.c_lstRealmConfigurations);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RealmListSettingsForm";
            this.Text = "Realm Configurations";
            this.Load += new System.EventHandler(this.RealmListSettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox c_lstRealmConfigurations;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox c_txtNewRealmName;
        private System.Windows.Forms.TextBox c_txtNewRealmListURL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox c_cbxWowVersion;
        private System.Windows.Forms.Button c_btnAddNewRealmConfig;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox c_txtSelectedWowVersion;
        private System.Windows.Forms.TextBox c_txtSelectedRealmListURL;
        private System.Windows.Forms.TextBox c_txtSelectedRealmName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button c_btnClose;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox c_txtNewConfigName;
        private System.Windows.Forms.Button c_btnResetRealmLists;


    }
}