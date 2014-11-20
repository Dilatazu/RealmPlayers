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
            this.c_btnAddContributor.Location = new System.Drawing.Point(430, 399);
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
            this.c_txtAddContributorName.Location = new System.Drawing.Point(296, 400);
            this.c_txtAddContributorName.Name = "c_txtAddContributorName";
            this.c_txtAddContributorName.Size = new System.Drawing.Size(128, 22);
            this.c_txtAddContributorName.TabIndex = 2;
            // 
            // c_lsContributors
            // 
            this.c_lsContributors.FormattingEnabled = true;
            this.c_lsContributors.Location = new System.Drawing.Point(296, 12);
            this.c_lsContributors.Name = "c_lsContributors";
            this.c_lsContributors.Size = new System.Drawing.Size(225, 381);
            this.c_lsContributors.TabIndex = 3;
            this.c_lsContributors.SelectedIndexChanged += new System.EventHandler(this.c_lsContributors_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 99);
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
            this.button3.Location = new System.Drawing.Point(13, 183);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 36);
            this.button3.TabIndex = 6;
            this.button3.Text = "Create RealmPlayers ItemSummaryDatabase";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(13, 225);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(121, 36);
            this.button4.TabIndex = 7;
            this.button4.Text = "Create RealmPlayers PlayerSummaryDatabase";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(139, 99);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(123, 36);
            this.button5.TabIndex = 8;
            this.button5.Text = "Fix bugged RaidStats SummaryDatabase";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(13, 267);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(120, 34);
            this.button6.TabIndex = 9;
            this.button6.Text = "Reset Archangel";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(122, 388);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 10;
            this.button7.Text = "button7";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 434);
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
            this.Load += new System.EventHandler(this.MainWindow_Load);
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
    }
}

