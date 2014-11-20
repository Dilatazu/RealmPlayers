
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace DetailedList
{
  //[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
  partial class DetailedListItem : System.Windows.Forms.UserControl
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
    private System.ComponentModel.IContainer components;

    //NOTE: The following procedure is required by the Windows Form Designer
    //It can be modified using the Windows Form Designer.  
    //Do not modify it using the code editor.
    [System.Diagnostics.DebuggerStepThrough()]
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailedListItem));
        this.ImageList1 = new System.Windows.Forms.ImageList(this.components);
        this.SuspendLayout();
        // 
        // ImageList1
        // 
        this.ImageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList1.ImageStream")));
        this.ImageList1.TransparentColor = System.Drawing.Color.Transparent;
        this.ImageList1.Images.SetKeyName(0, "default");
        // 
        // DetailedListItem
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.White;
        this.DoubleBuffered = true;
        this.Font = new System.Drawing.Font("Segoe UI Light", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        this.Name = "DetailedListItem";
        this.Size = new System.Drawing.Size(484, 75);
        this.Load += new System.EventHandler(this.DetailedListItem_Load);
        this.ResumeLayout(false);

    }
    internal System.Windows.Forms.ImageList ImageList1;

  }
}

