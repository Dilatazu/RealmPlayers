namespace VF_WoWLauncher
{
    partial class SetupWowDirectoryForm
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
            this.c_lblDescription = new System.Windows.Forms.Label();
            this.c_btnCancel = new System.Windows.Forms.Button();
            this.c_btnOK = new System.Windows.Forms.Button();
            this.c_lblWowDirectory = new System.Windows.Forms.Label();
            this.c_txtWowDirectory = new System.Windows.Forms.TextBox();
            this.c_btnBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // c_lblDescription
            // 
            this.c_lblDescription.Location = new System.Drawing.Point(12, 9);
            this.c_lblDescription.Name = "c_lblDescription";
            this.c_lblDescription.Size = new System.Drawing.Size(348, 18);
            this.c_lblDescription.TabIndex = 9;
            this.c_lblDescription.Text = "Please help me find your World of Warcraft Classic(or TBC) directory.";
            // 
            // c_btnCancel
            // 
            this.c_btnCancel.Location = new System.Drawing.Point(231, 96);
            this.c_btnCancel.Name = "c_btnCancel";
            this.c_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.c_btnCancel.TabIndex = 8;
            this.c_btnCancel.Text = "Cancel";
            this.c_btnCancel.UseVisualStyleBackColor = true;
            this.c_btnCancel.Click += new System.EventHandler(this.c_btnCancel_Click);
            // 
            // c_btnOK
            // 
            this.c_btnOK.Location = new System.Drawing.Point(312, 96);
            this.c_btnOK.Name = "c_btnOK";
            this.c_btnOK.Size = new System.Drawing.Size(75, 23);
            this.c_btnOK.TabIndex = 7;
            this.c_btnOK.Text = "OK";
            this.c_btnOK.UseVisualStyleBackColor = true;
            this.c_btnOK.Click += new System.EventHandler(this.c_btnOK_Click);
            // 
            // c_lblWowDirectory
            // 
            this.c_lblWowDirectory.AutoSize = true;
            this.c_lblWowDirectory.Location = new System.Drawing.Point(12, 39);
            this.c_lblWowDirectory.Name = "c_lblWowDirectory";
            this.c_lblWowDirectory.Size = new System.Drawing.Size(83, 13);
            this.c_lblWowDirectory.TabIndex = 6;
            this.c_lblWowDirectory.Text = "Wow Directory: ";
            // 
            // c_txtWowDirectory
            // 
            this.c_txtWowDirectory.Location = new System.Drawing.Point(12, 55);
            this.c_txtWowDirectory.Multiline = true;
            this.c_txtWowDirectory.Name = "c_txtWowDirectory";
            this.c_txtWowDirectory.Size = new System.Drawing.Size(313, 35);
            this.c_txtWowDirectory.TabIndex = 5;
            // 
            // c_btnBrowse
            // 
            this.c_btnBrowse.Location = new System.Drawing.Point(331, 55);
            this.c_btnBrowse.Name = "c_btnBrowse";
            this.c_btnBrowse.Size = new System.Drawing.Size(56, 35);
            this.c_btnBrowse.TabIndex = 10;
            this.c_btnBrowse.Text = "Browse";
            this.c_btnBrowse.UseVisualStyleBackColor = true;
            this.c_btnBrowse.Click += new System.EventHandler(this.c_btnBrowse_Click);
            // 
            // SetupWowDirectoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 130);
            this.Controls.Add(this.c_btnBrowse);
            this.Controls.Add(this.c_lblDescription);
            this.Controls.Add(this.c_btnCancel);
            this.Controls.Add(this.c_btnOK);
            this.Controls.Add(this.c_lblWowDirectory);
            this.Controls.Add(this.c_txtWowDirectory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SetupWowDirectoryForm";
            this.Text = "SetupWowDirectoryForm";
            this.Load += new System.EventHandler(this.SetupWowDirectoryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label c_lblDescription;
        private System.Windows.Forms.Button c_btnCancel;
        private System.Windows.Forms.Button c_btnOK;
        private System.Windows.Forms.Label c_lblWowDirectory;
        private System.Windows.Forms.TextBox c_txtWowDirectory;
        private System.Windows.Forms.Button c_btnBrowse;

    }
}