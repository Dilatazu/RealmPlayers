namespace VF_WoWLauncher
{
    partial class CreateAddonPackageForm
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
            this.c_btnBrowse = new System.Windows.Forms.Button();
            this.c_txtAddonFolder = new System.Windows.Forms.TextBox();
            this.c_txtVersion = new System.Windows.Forms.TextBox();
            this.c_txtDescription = new System.Windows.Forms.TextBox();
            this.c_lblDescription = new System.Windows.Forms.Label();
            this.c_lblVersion = new System.Windows.Forms.Label();
            this.c_lblAddonPackage = new System.Windows.Forms.Label();
            this.c_btnCreate = new System.Windows.Forms.Button();
            this.c_btnCancel = new System.Windows.Forms.Button();
            this.c_cbUpdateImportance = new System.Windows.Forms.ComboBox();
            this.c_lblUpdateImportance = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // c_btnBrowse
            // 
            this.c_btnBrowse.Location = new System.Drawing.Point(331, 25);
            this.c_btnBrowse.Name = "c_btnBrowse";
            this.c_btnBrowse.Size = new System.Drawing.Size(56, 35);
            this.c_btnBrowse.TabIndex = 12;
            this.c_btnBrowse.Text = "Browse";
            this.c_btnBrowse.UseVisualStyleBackColor = true;
            this.c_btnBrowse.Click += new System.EventHandler(this.c_btnBrowse_Click);
            // 
            // c_txtAddonFolder
            // 
            this.c_txtAddonFolder.Location = new System.Drawing.Point(12, 25);
            this.c_txtAddonFolder.Multiline = true;
            this.c_txtAddonFolder.Name = "c_txtAddonFolder";
            this.c_txtAddonFolder.Size = new System.Drawing.Size(313, 35);
            this.c_txtAddonFolder.TabIndex = 11;
            this.c_txtAddonFolder.TextChanged += new System.EventHandler(this.c_txtAddonFolder_TextChanged);
            // 
            // c_txtVersion
            // 
            this.c_txtVersion.Location = new System.Drawing.Point(60, 85);
            this.c_txtVersion.Name = "c_txtVersion";
            this.c_txtVersion.ReadOnly = true;
            this.c_txtVersion.Size = new System.Drawing.Size(327, 20);
            this.c_txtVersion.TabIndex = 13;
            // 
            // c_txtDescription
            // 
            this.c_txtDescription.Location = new System.Drawing.Point(12, 174);
            this.c_txtDescription.Multiline = true;
            this.c_txtDescription.Name = "c_txtDescription";
            this.c_txtDescription.Size = new System.Drawing.Size(375, 202);
            this.c_txtDescription.TabIndex = 14;
            // 
            // c_lblDescription
            // 
            this.c_lblDescription.AutoSize = true;
            this.c_lblDescription.Location = new System.Drawing.Point(12, 158);
            this.c_lblDescription.Name = "c_lblDescription";
            this.c_lblDescription.Size = new System.Drawing.Size(60, 13);
            this.c_lblDescription.TabIndex = 15;
            this.c_lblDescription.Text = "Description";
            // 
            // c_lblVersion
            // 
            this.c_lblVersion.AutoSize = true;
            this.c_lblVersion.Location = new System.Drawing.Point(12, 88);
            this.c_lblVersion.Name = "c_lblVersion";
            this.c_lblVersion.Size = new System.Drawing.Size(42, 13);
            this.c_lblVersion.TabIndex = 16;
            this.c_lblVersion.Text = "Version";
            // 
            // c_lblAddonPackage
            // 
            this.c_lblAddonPackage.AutoSize = true;
            this.c_lblAddonPackage.Location = new System.Drawing.Point(9, 9);
            this.c_lblAddonPackage.Name = "c_lblAddonPackage";
            this.c_lblAddonPackage.Size = new System.Drawing.Size(67, 13);
            this.c_lblAddonPackage.TabIndex = 17;
            this.c_lblAddonPackage.Text = "Addon folder";
            // 
            // c_btnCreate
            // 
            this.c_btnCreate.Enabled = false;
            this.c_btnCreate.Location = new System.Drawing.Point(254, 442);
            this.c_btnCreate.Name = "c_btnCreate";
            this.c_btnCreate.Size = new System.Drawing.Size(133, 23);
            this.c_btnCreate.TabIndex = 18;
            this.c_btnCreate.Text = "Create AddonPackage";
            this.c_btnCreate.UseVisualStyleBackColor = true;
            this.c_btnCreate.Click += new System.EventHandler(this.c_btnCreate_Click);
            // 
            // c_btnCancel
            // 
            this.c_btnCancel.Location = new System.Drawing.Point(12, 442);
            this.c_btnCancel.Name = "c_btnCancel";
            this.c_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.c_btnCancel.TabIndex = 19;
            this.c_btnCancel.Text = "Cancel";
            this.c_btnCancel.UseVisualStyleBackColor = true;
            // 
            // c_cbUpdateImportance
            // 
            this.c_cbUpdateImportance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_cbUpdateImportance.FormattingEnabled = true;
            this.c_cbUpdateImportance.Location = new System.Drawing.Point(116, 111);
            this.c_cbUpdateImportance.Name = "c_cbUpdateImportance";
            this.c_cbUpdateImportance.Size = new System.Drawing.Size(168, 21);
            this.c_cbUpdateImportance.TabIndex = 20;
            // 
            // c_lblUpdateImportance
            // 
            this.c_lblUpdateImportance.AutoSize = true;
            this.c_lblUpdateImportance.Location = new System.Drawing.Point(12, 114);
            this.c_lblUpdateImportance.Name = "c_lblUpdateImportance";
            this.c_lblUpdateImportance.Size = new System.Drawing.Size(98, 13);
            this.c_lblUpdateImportance.TabIndex = 21;
            this.c_lblUpdateImportance.Text = "Update Importance";
            // 
            // CreateAddonPackageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 477);
            this.Controls.Add(this.c_lblUpdateImportance);
            this.Controls.Add(this.c_cbUpdateImportance);
            this.Controls.Add(this.c_btnCancel);
            this.Controls.Add(this.c_btnCreate);
            this.Controls.Add(this.c_lblAddonPackage);
            this.Controls.Add(this.c_lblVersion);
            this.Controls.Add(this.c_lblDescription);
            this.Controls.Add(this.c_txtDescription);
            this.Controls.Add(this.c_txtVersion);
            this.Controls.Add(this.c_btnBrowse);
            this.Controls.Add(this.c_txtAddonFolder);
            this.Name = "CreateAddonPackageForm";
            this.Text = "CreateAddonPackageForm";
            this.Load += new System.EventHandler(this.CreateAddonPackageForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button c_btnBrowse;
        private System.Windows.Forms.TextBox c_txtAddonFolder;
        private System.Windows.Forms.TextBox c_txtVersion;
        private System.Windows.Forms.TextBox c_txtDescription;
        private System.Windows.Forms.Label c_lblDescription;
        private System.Windows.Forms.Label c_lblVersion;
        private System.Windows.Forms.Label c_lblAddonPackage;
        private System.Windows.Forms.Button c_btnCreate;
        private System.Windows.Forms.Button c_btnCancel;
        private System.Windows.Forms.ComboBox c_cbUpdateImportance;
        private System.Windows.Forms.Label c_lblUpdateImportance;
    }
}