using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
//namespace System.Windows.Forms
//{
//    public class FlowLayoutPanel2 : FlowLayoutPanel
//    {
//        protected override CreateParams CreateParams
//        {
//            get
//            {
//                CreateParams cp = base.CreateParams;
//                cp.ExStyle |= 0x02000000; // WS_CLIPCHILDREN
//                return cp;
//            }
//        }
//    }
//}
namespace DetailedList
{
  //[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
  public partial class DetailedList : System.Windows.Forms.UserControl
  {
    //UserControl overrides dispose to clean up the component list.
    [System.Diagnostics.DebuggerNonUserCode()]
    protected override void Dispose(bool disposing)
    {
      try {
        if (disposing && components != null) {
          components.Dispose();
        }
      } finally {
        base.Dispose(disposing);
      }
    }

    //Required by the Windows Form Designer
    private System.ComponentModel.IContainer components = null;

    //NOTE: The following procedure is required by the Windows Form Designer
    //It can be modified using the Windows Form Designer.  
    //Do not modify it using the code editor.
    [System.Diagnostics.DebuggerStepThrough()]
    private void InitializeComponent()
    {
        this.flpListBox = new System.Windows.Forms.FlowLayoutPanel();
        this.SuspendLayout();
        // 
        // flpListBox
        // 
        this.flpListBox.AutoScroll = true;
        this.flpListBox.BackColor = System.Drawing.Color.White;
        this.flpListBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this.flpListBox.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.flpListBox.Location = new System.Drawing.Point(0, 0);
        this.flpListBox.Margin = new System.Windows.Forms.Padding(0);
        this.flpListBox.Name = "flpListBox";
        this.flpListBox.Size = new System.Drawing.Size(148, 148);
        this.flpListBox.TabIndex = 0;
        this.flpListBox.WrapContents = false;
        // 
        // DetailedList
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.SystemColors.Window;
        this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.Controls.Add(this.flpListBox);
        this.Margin = new System.Windows.Forms.Padding(0);
        this.Name = "DetailedList";
        this.Size = new System.Drawing.Size(148, 148);
        this.ResumeLayout(false);

    }
    internal System.Windows.Forms.FlowLayoutPanel flpListBox;

  }
}

