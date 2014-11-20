namespace MongoDBPorting
{
    partial class Form1
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
            this.MigrateRealmDBs = new System.Windows.Forms.Button();
            this.MigrateContributors = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MigrateRealmDBs
            // 
            this.MigrateRealmDBs.Location = new System.Drawing.Point(12, 41);
            this.MigrateRealmDBs.Name = "MigrateRealmDBs";
            this.MigrateRealmDBs.Size = new System.Drawing.Size(122, 23);
            this.MigrateRealmDBs.TabIndex = 0;
            this.MigrateRealmDBs.Text = "Migrate Realm DBs";
            this.MigrateRealmDBs.UseVisualStyleBackColor = true;
            this.MigrateRealmDBs.Click += new System.EventHandler(this.MigrateRealmDBs_Click);
            // 
            // MigrateContributors
            // 
            this.MigrateContributors.Location = new System.Drawing.Point(12, 12);
            this.MigrateContributors.Name = "MigrateContributors";
            this.MigrateContributors.Size = new System.Drawing.Size(122, 23);
            this.MigrateContributors.TabIndex = 1;
            this.MigrateContributors.Text = "Migrate Contributors";
            this.MigrateContributors.UseVisualStyleBackColor = true;
            this.MigrateContributors.Click += new System.EventHandler(this.MigrateContributors_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 227);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Reset Database";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.MigrateContributors);
            this.Controls.Add(this.MigrateRealmDBs);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button MigrateRealmDBs;
        private System.Windows.Forms.Button MigrateContributors;
        private System.Windows.Forms.Button button1;
    }
}

