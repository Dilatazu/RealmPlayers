namespace DetailedList
{
    partial class RightSideAddonUpdate
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pgrProgress = new System.Windows.Forms.ProgressBar();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnMoreInfo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pgrProgress
            // 
            this.pgrProgress.Location = new System.Drawing.Point(3, 3);
            this.pgrProgress.Name = "pgrProgress";
            this.pgrProgress.Size = new System.Drawing.Size(72, 23);
            this.pgrProgress.TabIndex = 0;
            this.pgrProgress.Visible = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Location = new System.Drawing.Point(3, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 23);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnMoreInfo
            // 
            this.btnMoreInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoreInfo.Location = new System.Drawing.Point(3, 31);
            this.btnMoreInfo.Name = "btnMoreInfo";
            this.btnMoreInfo.Size = new System.Drawing.Size(72, 23);
            this.btnMoreInfo.TabIndex = 2;
            this.btnMoreInfo.Text = "More Info";
            this.btnMoreInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnMoreInfo.UseVisualStyleBackColor = true;
            this.btnMoreInfo.Click += new System.EventHandler(this.btnMoreInfo_Click);
            // 
            // RightSideAddonUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnMoreInfo);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.pgrProgress);
            this.Name = "RightSideAddonUpdate";
            this.Size = new System.Drawing.Size(78, 75);
            this.Load += new System.EventHandler(this.RightSideAddonUpdate_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pgrProgress;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnMoreInfo;
    }
}
