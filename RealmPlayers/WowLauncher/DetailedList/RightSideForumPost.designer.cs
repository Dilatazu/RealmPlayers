namespace DetailedList
{
    partial class RightSideForumPost
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
            this.btnHidePost = new System.Windows.Forms.Button();
            this.btnGotoForum = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnHidePost
            // 
            this.btnHidePost.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHidePost.Location = new System.Drawing.Point(3, 32);
            this.btnHidePost.Name = "btnHidePost";
            this.btnHidePost.Size = new System.Drawing.Size(72, 23);
            this.btnHidePost.TabIndex = 1;
            this.btnHidePost.Text = "Hide";
            this.btnHidePost.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnHidePost.UseVisualStyleBackColor = true;
            this.btnHidePost.Click += new System.EventHandler(this.btnHidePost_Click);
            // 
            // btnGotoForum
            // 
            this.btnGotoForum.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGotoForum.Location = new System.Drawing.Point(3, 3);
            this.btnGotoForum.Name = "btnGotoForum";
            this.btnGotoForum.Size = new System.Drawing.Size(72, 23);
            this.btnGotoForum.TabIndex = 2;
            this.btnGotoForum.Text = "Goto thread";
            this.btnGotoForum.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnGotoForum.UseVisualStyleBackColor = true;
            this.btnGotoForum.Click += new System.EventHandler(this.btnGotoForum_Click);
            // 
            // RightSideForumPost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnGotoForum);
            this.Controls.Add(this.btnHidePost);
            this.Name = "RightSideForumPost";
            this.Size = new System.Drawing.Size(78, 60);
            this.Load += new System.EventHandler(this.RightSideAddonUpdate_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnHidePost;
        private System.Windows.Forms.Button btnGotoForum;
    }
}
