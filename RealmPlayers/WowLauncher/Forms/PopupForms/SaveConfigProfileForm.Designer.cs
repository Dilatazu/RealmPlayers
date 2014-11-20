namespace VF_WoWLauncher
{
    partial class SaveConfigProfileForm
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
            this.c_btnSave = new System.Windows.Forms.Button();
            this.c_btnCancel = new System.Windows.Forms.Button();
            this.c_ddlConfigProfiles = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // c_btnSave
            // 
            this.c_btnSave.Location = new System.Drawing.Point(144, 39);
            this.c_btnSave.Name = "c_btnSave";
            this.c_btnSave.Size = new System.Drawing.Size(75, 23);
            this.c_btnSave.TabIndex = 0;
            this.c_btnSave.Text = "Save";
            this.c_btnSave.UseVisualStyleBackColor = true;
            this.c_btnSave.Click += new System.EventHandler(this.c_btnSave_Click);
            // 
            // c_btnCancel
            // 
            this.c_btnCancel.Location = new System.Drawing.Point(12, 39);
            this.c_btnCancel.Name = "c_btnCancel";
            this.c_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.c_btnCancel.TabIndex = 1;
            this.c_btnCancel.Text = "Cancel";
            this.c_btnCancel.UseVisualStyleBackColor = true;
            this.c_btnCancel.Click += new System.EventHandler(this.c_btnCancel_Click);
            // 
            // c_ddlConfigProfiles
            // 
            this.c_ddlConfigProfiles.FormattingEnabled = true;
            this.c_ddlConfigProfiles.Location = new System.Drawing.Point(12, 12);
            this.c_ddlConfigProfiles.Name = "c_ddlConfigProfiles";
            this.c_ddlConfigProfiles.Size = new System.Drawing.Size(207, 21);
            this.c_ddlConfigProfiles.TabIndex = 2;
            // 
            // SaveConfigProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 73);
            this.Controls.Add(this.c_ddlConfigProfiles);
            this.Controls.Add(this.c_btnCancel);
            this.Controls.Add(this.c_btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SaveConfigProfileForm";
            this.Text = "SaveConfigProfileForm";
            this.Load += new System.EventHandler(this.SaveConfigProfileForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button c_btnSave;
        private System.Windows.Forms.Button c_btnCancel;
        private System.Windows.Forms.ComboBox c_ddlConfigProfiles;
    }
}