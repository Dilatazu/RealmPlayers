namespace VF_WoWLauncher
{
    partial class LogWindow
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
            this.c_txtLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // c_txtLog
            // 
            this.c_txtLog.Location = new System.Drawing.Point(12, 36);
            this.c_txtLog.Multiline = true;
            this.c_txtLog.Name = "c_txtLog";
            this.c_txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.c_txtLog.Size = new System.Drawing.Size(268, 260);
            this.c_txtLog.TabIndex = 3;
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 332);
            this.Controls.Add(this.c_txtLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "LogWindow";
            this.Text = "LogWindow";
            this.Load += new System.EventHandler(this.LogWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox c_txtLog;
    }
}