namespace VF_WoWLauncher
{
    partial class ConfigSettingsForm
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
            this.c_btnCancel = new System.Windows.Forms.Button();
            this.c_btnSave = new System.Windows.Forms.Button();
            this.c_ddlConfigProfile = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.c_lblListExplanation = new System.Windows.Forms.Label();
            this.c_lbSettings = new System.Windows.Forms.ListBox();
            this.c_lblAddonMemory = new System.Windows.Forms.Label();
            this.c_ddlScriptMemory = new System.Windows.Forms.ComboBox();
            this.c_cbMaximized = new System.Windows.Forms.CheckBox();
            this.c_cbWindowMode = new System.Windows.Forms.CheckBox();
            this.c_ddlResolution = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // c_btnCancel
            // 
            this.c_btnCancel.Location = new System.Drawing.Point(12, 455);
            this.c_btnCancel.Name = "c_btnCancel";
            this.c_btnCancel.Size = new System.Drawing.Size(54, 23);
            this.c_btnCancel.TabIndex = 6;
            this.c_btnCancel.Text = "Cancel";
            this.c_btnCancel.UseVisualStyleBackColor = true;
            this.c_btnCancel.Click += new System.EventHandler(this.c_btnCancel_Click);
            // 
            // c_btnSave
            // 
            this.c_btnSave.Location = new System.Drawing.Point(143, 455);
            this.c_btnSave.Name = "c_btnSave";
            this.c_btnSave.Size = new System.Drawing.Size(116, 23);
            this.c_btnSave.TabIndex = 11;
            this.c_btnSave.Text = "Save Config Profile";
            this.c_btnSave.UseVisualStyleBackColor = true;
            this.c_btnSave.Click += new System.EventHandler(this.c_btnSave_Click);
            // 
            // c_ddlConfigProfile
            // 
            this.c_ddlConfigProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_ddlConfigProfile.FormattingEnabled = true;
            this.c_ddlConfigProfile.Location = new System.Drawing.Point(12, 12);
            this.c_ddlConfigProfile.Name = "c_ddlConfigProfile";
            this.c_ddlConfigProfile.Size = new System.Drawing.Size(247, 21);
            this.c_ddlConfigProfile.TabIndex = 12;
            this.c_ddlConfigProfile.SelectedIndexChanged += new System.EventHandler(this.c_ddlConfigProfile_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.c_lblListExplanation);
            this.panel1.Controls.Add(this.c_lbSettings);
            this.panel1.Controls.Add(this.c_lblAddonMemory);
            this.panel1.Controls.Add(this.c_ddlScriptMemory);
            this.panel1.Controls.Add(this.c_cbMaximized);
            this.panel1.Controls.Add(this.c_cbWindowMode);
            this.panel1.Controls.Add(this.c_ddlResolution);
            this.panel1.Location = new System.Drawing.Point(12, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(247, 409);
            this.panel1.TabIndex = 13;
            // 
            // c_lblListExplanation
            // 
            this.c_lblListExplanation.Location = new System.Drawing.Point(2, 80);
            this.c_lblListExplanation.Name = "c_lblListExplanation";
            this.c_lblListExplanation.Size = new System.Drawing.Size(238, 18);
            this.c_lblListExplanation.TabIndex = 17;
            this.c_lblListExplanation.Text = "DoubleClick any setting listed below to change";
            this.c_lblListExplanation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // c_lbSettings
            // 
            this.c_lbSettings.FormattingEnabled = true;
            this.c_lbSettings.Location = new System.Drawing.Point(3, 101);
            this.c_lbSettings.Name = "c_lbSettings";
            this.c_lbSettings.Size = new System.Drawing.Size(237, 303);
            this.c_lbSettings.TabIndex = 16;
            // 
            // c_lblAddonMemory
            // 
            this.c_lblAddonMemory.Location = new System.Drawing.Point(151, 11);
            this.c_lblAddonMemory.Name = "c_lblAddonMemory";
            this.c_lblAddonMemory.Size = new System.Drawing.Size(89, 16);
            this.c_lblAddonMemory.TabIndex = 15;
            this.c_lblAddonMemory.Text = "Addon Memory";
            this.c_lblAddonMemory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // c_ddlScriptMemory
            // 
            this.c_ddlScriptMemory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_ddlScriptMemory.FormattingEnabled = true;
            this.c_ddlScriptMemory.Location = new System.Drawing.Point(151, 30);
            this.c_ddlScriptMemory.Name = "c_ddlScriptMemory";
            this.c_ddlScriptMemory.Size = new System.Drawing.Size(89, 21);
            this.c_ddlScriptMemory.TabIndex = 14;
            this.c_ddlScriptMemory.SelectedIndexChanged += new System.EventHandler(this.c_ddlScriptMemory_SelectedIndexChanged);
            // 
            // c_cbMaximized
            // 
            this.c_cbMaximized.AutoSize = true;
            this.c_cbMaximized.Location = new System.Drawing.Point(20, 57);
            this.c_cbMaximized.Name = "c_cbMaximized";
            this.c_cbMaximized.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.c_cbMaximized.Size = new System.Drawing.Size(75, 17);
            this.c_cbMaximized.TabIndex = 13;
            this.c_cbMaximized.Text = "Maximized";
            this.c_cbMaximized.UseVisualStyleBackColor = true;
            this.c_cbMaximized.CheckedChanged += new System.EventHandler(this.c_cbMaximized_CheckedChanged);
            // 
            // c_cbWindowMode
            // 
            this.c_cbWindowMode.AutoSize = true;
            this.c_cbWindowMode.Location = new System.Drawing.Point(3, 34);
            this.c_cbWindowMode.Name = "c_cbWindowMode";
            this.c_cbWindowMode.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.c_cbWindowMode.Size = new System.Drawing.Size(92, 17);
            this.c_cbWindowMode.TabIndex = 12;
            this.c_cbWindowMode.Text = "WindowMode";
            this.c_cbWindowMode.UseVisualStyleBackColor = true;
            this.c_cbWindowMode.CheckedChanged += new System.EventHandler(this.c_cbWindowMode_CheckedChanged);
            // 
            // c_ddlResolution
            // 
            this.c_ddlResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.c_ddlResolution.FormattingEnabled = true;
            this.c_ddlResolution.Location = new System.Drawing.Point(3, 6);
            this.c_ddlResolution.Name = "c_ddlResolution";
            this.c_ddlResolution.Size = new System.Drawing.Size(92, 21);
            this.c_ddlResolution.TabIndex = 11;
            this.c_ddlResolution.SelectedIndexChanged += new System.EventHandler(this.c_ddlResolution_SelectedIndexChanged);
            // 
            // ConfigSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(269, 490);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.c_ddlConfigProfile);
            this.Controls.Add(this.c_btnSave);
            this.Controls.Add(this.c_btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigSettingsForm";
            this.Text = "ConfigSettingsForm";
            this.Load += new System.EventHandler(this.ConfigSettingsForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button c_btnCancel;
        private System.Windows.Forms.Button c_btnSave;
        private System.Windows.Forms.ComboBox c_ddlConfigProfile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label c_lblListExplanation;
        private System.Windows.Forms.ListBox c_lbSettings;
        private System.Windows.Forms.Label c_lblAddonMemory;
        private System.Windows.Forms.ComboBox c_ddlScriptMemory;
        private System.Windows.Forms.CheckBox c_cbMaximized;
        private System.Windows.Forms.CheckBox c_cbWindowMode;
        private System.Windows.Forms.ComboBox c_ddlResolution;
    }
}