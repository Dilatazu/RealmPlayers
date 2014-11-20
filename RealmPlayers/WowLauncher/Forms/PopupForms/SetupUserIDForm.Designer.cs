namespace VF_WoWLauncher
{
    partial class SetupUserIDForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupUserIDForm));
            this.c_txtUserID = new System.Windows.Forms.TextBox();
            this.c_lblUserID = new System.Windows.Forms.Label();
            this.c_btnValidate = new System.Windows.Forms.Button();
            this.c_btnCancel = new System.Windows.Forms.Button();
            this.c_lblDescription = new System.Windows.Forms.Label();
            this.c_btnSkip = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // c_txtUserID
            // 
            this.c_txtUserID.Location = new System.Drawing.Point(65, 96);
            this.c_txtUserID.Name = "c_txtUserID";
            this.c_txtUserID.Size = new System.Drawing.Size(295, 20);
            this.c_txtUserID.TabIndex = 0;
            this.c_txtUserID.Text = "Unknown.123456";
            this.c_txtUserID.TextChanged += new System.EventHandler(this.c_txtUserID_TextChanged);
            // 
            // c_lblUserID
            // 
            this.c_lblUserID.AutoSize = true;
            this.c_lblUserID.Location = new System.Drawing.Point(12, 99);
            this.c_lblUserID.Name = "c_lblUserID";
            this.c_lblUserID.Size = new System.Drawing.Size(46, 13);
            this.c_lblUserID.TabIndex = 1;
            this.c_lblUserID.Text = "UserID: ";
            // 
            // c_btnValidate
            // 
            this.c_btnValidate.Location = new System.Drawing.Point(285, 122);
            this.c_btnValidate.Name = "c_btnValidate";
            this.c_btnValidate.Size = new System.Drawing.Size(75, 23);
            this.c_btnValidate.TabIndex = 2;
            this.c_btnValidate.Text = "Validate";
            this.c_btnValidate.UseVisualStyleBackColor = true;
            this.c_btnValidate.Click += new System.EventHandler(this.c_btnValidate_Click);
            // 
            // c_btnCancel
            // 
            this.c_btnCancel.Location = new System.Drawing.Point(12, 122);
            this.c_btnCancel.Name = "c_btnCancel";
            this.c_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.c_btnCancel.TabIndex = 3;
            this.c_btnCancel.Text = "Cancel";
            this.c_btnCancel.UseVisualStyleBackColor = true;
            this.c_btnCancel.Click += new System.EventHandler(this.c_btnCancel_Click);
            // 
            // c_lblDescription
            // 
            this.c_lblDescription.Location = new System.Drawing.Point(12, 9);
            this.c_lblDescription.Name = "c_lblDescription";
            this.c_lblDescription.Size = new System.Drawing.Size(348, 70);
            this.c_lblDescription.TabIndex = 4;
            this.c_lblDescription.Text = resources.GetString("c_lblDescription.Text");
            // 
            // c_btnSkip
            // 
            this.c_btnSkip.Location = new System.Drawing.Point(204, 122);
            this.c_btnSkip.Name = "c_btnSkip";
            this.c_btnSkip.Size = new System.Drawing.Size(75, 23);
            this.c_btnSkip.TabIndex = 5;
            this.c_btnSkip.Text = "Skip";
            this.c_btnSkip.UseVisualStyleBackColor = true;
            this.c_btnSkip.Click += new System.EventHandler(this.c_btnSkip_Click);
            // 
            // SetupUserIDForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 157);
            this.Controls.Add(this.c_btnSkip);
            this.Controls.Add(this.c_lblDescription);
            this.Controls.Add(this.c_btnCancel);
            this.Controls.Add(this.c_btnValidate);
            this.Controls.Add(this.c_lblUserID);
            this.Controls.Add(this.c_txtUserID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SetupUserIDForm";
            this.Text = "SetupUserIDForm";
            this.Load += new System.EventHandler(this.SetupUserIDForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox c_txtUserID;
        private System.Windows.Forms.Label c_lblUserID;
        private System.Windows.Forms.Button c_btnValidate;
        private System.Windows.Forms.Button c_btnCancel;
        private System.Windows.Forms.Label c_lblDescription;
        private System.Windows.Forms.Button c_btnSkip;
    }
}