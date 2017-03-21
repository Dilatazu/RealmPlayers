namespace VF_WoWLauncherServer
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
            this.components = new System.ComponentModel.Container();
            this.c_btnAddAddonPackage = new System.Windows.Forms.Button();
            this.c_btnAddContributor = new System.Windows.Forms.Button();
            this.c_txtAddContributorName = new System.Windows.Forms.TextBox();
            this.c_lsContributors = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.c_txtBetaAddon = new System.Windows.Forms.TextBox();
            this.button9 = new System.Windows.Forms.Button();
            this.c_txtBetaUserID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.c_lsBetaUsers = new System.Windows.Forms.ListBox();
            this.c_cmsBetaUsers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.c_cmsBetaUsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // c_btnAddAddonPackage
            // 
            this.c_btnAddAddonPackage.Location = new System.Drawing.Point(13, 13);
            this.c_btnAddAddonPackage.Name = "c_btnAddAddonPackage";
            this.c_btnAddAddonPackage.Size = new System.Drawing.Size(120, 23);
            this.c_btnAddAddonPackage.TabIndex = 0;
            this.c_btnAddAddonPackage.Text = "Add AddonPackage";
            this.c_btnAddAddonPackage.UseVisualStyleBackColor = true;
            this.c_btnAddAddonPackage.Click += new System.EventHandler(this.button1_Click);
            // 
            // c_btnAddContributor
            // 
            this.c_btnAddContributor.Location = new System.Drawing.Point(601, 164);
            this.c_btnAddContributor.Name = "c_btnAddContributor";
            this.c_btnAddContributor.Size = new System.Drawing.Size(91, 23);
            this.c_btnAddContributor.TabIndex = 1;
            this.c_btnAddContributor.Text = "Add Contributor";
            this.c_btnAddContributor.UseVisualStyleBackColor = true;
            this.c_btnAddContributor.Click += new System.EventHandler(this.c_btnAddContributor_Click);
            // 
            // c_txtAddContributorName
            // 
            this.c_txtAddContributorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_txtAddContributorName.Location = new System.Drawing.Point(467, 165);
            this.c_txtAddContributorName.Name = "c_txtAddContributorName";
            this.c_txtAddContributorName.Size = new System.Drawing.Size(128, 22);
            this.c_txtAddContributorName.TabIndex = 2;
            // 
            // c_lsContributors
            // 
            this.c_lsContributors.FormattingEnabled = true;
            this.c_lsContributors.Location = new System.Drawing.Point(467, 12);
            this.c_lsContributors.Name = "c_lsContributors";
            this.c_lsContributors.Size = new System.Drawing.Size(225, 147);
            this.c_lsContributors.TabIndex = 3;
            this.c_lsContributors.SelectedIndexChanged += new System.EventHandler(this.c_lsContributors_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(270, 142);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 36);
            this.button1.TabIndex = 4;
            this.button1.Text = "Create RaidStats SummaryDatabase";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 141);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 36);
            this.button2.TabIndex = 5;
            this.button2.Text = "Create RealmPlayers GuildSummaryDatabase";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(141, 141);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 36);
            this.button3.TabIndex = 6;
            this.button3.Text = "Create RealmPlayers ItemSummaryDatabase";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(13, 183);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(121, 36);
            this.button4.TabIndex = 7;
            this.button4.Text = "Create RealmPlayers PlayerSummaryDatabase";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(268, 184);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(123, 36);
            this.button5.TabIndex = 8;
            this.button5.Text = "Fix bugged RaidStats SummaryDatabase";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(142, 185);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(120, 34);
            this.button6.TabIndex = 9;
            this.button6.Text = "Reset Archangel";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(142, 400);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 10;
            this.button7.Text = "MongoDB";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(181, 12);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(109, 23);
            this.button8.TabIndex = 11;
            this.button8.Text = "Toggle Locked";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // c_txtBetaAddon
            // 
            this.c_txtBetaAddon.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_txtBetaAddon.Location = new System.Drawing.Point(439, 372);
            this.c_txtBetaAddon.Name = "c_txtBetaAddon";
            this.c_txtBetaAddon.Size = new System.Drawing.Size(253, 22);
            this.c_txtBetaAddon.TabIndex = 13;
            this.c_txtBetaAddon.Text = "VF_RaidStatsTBC";
            this.c_txtBetaAddon.TextChanged += new System.EventHandler(this.c_txtBetaAddon_TextChanged);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(601, 400);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(91, 22);
            this.button9.TabIndex = 12;
            this.button9.Text = "Add Beta User";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // c_txtBetaUserID
            // 
            this.c_txtBetaUserID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_txtBetaUserID.Location = new System.Drawing.Point(439, 400);
            this.c_txtBetaUserID.Name = "c_txtBetaUserID";
            this.c_txtBetaUserID.Size = new System.Drawing.Size(156, 22);
            this.c_txtBetaUserID.TabIndex = 14;
            this.c_txtBetaUserID.TextChanged += new System.EventHandler(this.c_txtBetaUserID_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(395, 377);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Addon";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(395, 405);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "UserID";
            // 
            // c_lsBetaUsers
            // 
            this.c_lsBetaUsers.ContextMenuStrip = this.c_cmsBetaUsers;
            this.c_lsBetaUsers.FormattingEnabled = true;
            this.c_lsBetaUsers.Location = new System.Drawing.Point(398, 206);
            this.c_lsBetaUsers.Name = "c_lsBetaUsers";
            this.c_lsBetaUsers.Size = new System.Drawing.Size(294, 160);
            this.c_lsBetaUsers.TabIndex = 17;
            this.c_lsBetaUsers.SelectedIndexChanged += new System.EventHandler(this.c_lsBetaUsers_SelectedIndexChanged);
            this.c_lsBetaUsers.KeyDown += new System.Windows.Forms.KeyEventHandler(this.c_lsBetaUsers_KeyDown);
            // 
            // c_cmsBetaUsers
            // 
            this.c_cmsBetaUsers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.c_cmsBetaUsers.Name = "c_cmsBetaUsers";
            this.c_cmsBetaUsers.Size = new System.Drawing.Size(204, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(203, 22);
            this.toolStripMenuItem1.Text = "Remove Beta Participant";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(13, 359);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(120, 23);
            this.button10.TabIndex = 18;
            this.button10.Text = "Purge RealmPlayers";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(13, 99);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(249, 36);
            this.button11.TabIndex = 19;
            this.button11.Text = "Update RealmPlayers SummaryDatabases";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(171, 301);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(165, 23);
            this.button12.TabIndex = 20;
            this.button12.Text = "migrate itemsummary to SQL";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(24, 270);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(109, 23);
            this.button13.TabIndex = 21;
            this.button13.Text = "TryResetSQL";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(287, 400);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(102, 23);
            this.button14.TabIndex = 22;
            this.button14.Text = "Delete UserID";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(287, 359);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(102, 23);
            this.button15.TabIndex = 23;
            this.button15.Text = "Add Cornholi";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 434);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.c_lsBetaUsers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.c_txtBetaUserID);
            this.Controls.Add(this.c_txtBetaAddon);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.c_lsContributors);
            this.Controls.Add(this.c_txtAddContributorName);
            this.Controls.Add(this.c_btnAddContributor);
            this.Controls.Add(this.c_btnAddAddonPackage);
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.Activated += new System.EventHandler(this.MainWindow_Activated);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.c_cmsBetaUsers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button c_btnAddAddonPackage;
        private System.Windows.Forms.Button c_btnAddContributor;
        private System.Windows.Forms.TextBox c_txtAddContributorName;
        private System.Windows.Forms.ListBox c_lsContributors;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox c_txtBetaAddon;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.TextBox c_txtBetaUserID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox c_lsBetaUsers;
        private System.Windows.Forms.ContextMenuStrip c_cmsBetaUsers;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
    }
}

