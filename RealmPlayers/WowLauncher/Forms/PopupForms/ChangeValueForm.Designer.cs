namespace VF_WoWLauncher
{
    partial class ChangeValueForm
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
            this.c_btnOK = new System.Windows.Forms.Button();
            this.c_btnCancel = new System.Windows.Forms.Button();
            this.c_lblDescription = new System.Windows.Forms.Label();
            this.c_txtValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // c_btnOK
            // 
            this.c_btnOK.Location = new System.Drawing.Point(201, 100);
            this.c_btnOK.Name = "c_btnOK";
            this.c_btnOK.Size = new System.Drawing.Size(75, 23);
            this.c_btnOK.TabIndex = 0;
            this.c_btnOK.Text = "OK";
            this.c_btnOK.UseVisualStyleBackColor = true;
            this.c_btnOK.Click += new System.EventHandler(this.c_btnOK_Click);
            // 
            // c_btnCancel
            // 
            this.c_btnCancel.Location = new System.Drawing.Point(120, 100);
            this.c_btnCancel.Name = "c_btnCancel";
            this.c_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.c_btnCancel.TabIndex = 1;
            this.c_btnCancel.Text = "Cancel";
            this.c_btnCancel.UseVisualStyleBackColor = true;
            this.c_btnCancel.Click += new System.EventHandler(this.c_btnCancel_Click);
            // 
            // c_lblDescription
            // 
            this.c_lblDescription.Location = new System.Drawing.Point(13, 13);
            this.c_lblDescription.Name = "c_lblDescription";
            this.c_lblDescription.Size = new System.Drawing.Size(263, 58);
            this.c_lblDescription.TabIndex = 2;
            this.c_lblDescription.Text = "label1";
            // 
            // c_txtValue
            // 
            this.c_txtValue.Location = new System.Drawing.Point(16, 74);
            this.c_txtValue.Name = "c_txtValue";
            this.c_txtValue.Size = new System.Drawing.Size(260, 20);
            this.c_txtValue.TabIndex = 3;
            // 
            // ChangeValueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 135);
            this.Controls.Add(this.c_txtValue);
            this.Controls.Add(this.c_lblDescription);
            this.Controls.Add(this.c_btnCancel);
            this.Controls.Add(this.c_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChangeValueForm";
            this.Text = "ChangeValueForm";
            this.Load += new System.EventHandler(this.ChangeValueForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button c_btnOK;
        private System.Windows.Forms.Button c_btnCancel;
        private System.Windows.Forms.Label c_lblDescription;
        private System.Windows.Forms.TextBox c_txtValue;
    }
}