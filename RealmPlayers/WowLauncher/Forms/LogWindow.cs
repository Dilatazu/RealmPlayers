using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VF_WoWLauncher
{
    public partial class LogWindow : Form
    {
        LauncherWindow c_frmLauncherWindow = null;
        public LogWindow(LauncherWindow _LauncherWindow)
        {
            c_frmLauncherWindow = _LauncherWindow;
            InitializeComponent();
            Logger.sm_ExternalLog_TextLog = c_txtLog;
            this.FormClosing += LogWindow_FormClosing;
        }

        void LogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        void LogWindow_Move(object sender, EventArgs e)
        {
            c_frmLauncherWindow.MoveTo(this.Top + 5, this.Left - c_frmLauncherWindow.Width - 5);
        }

        void LogWindow_SizeChanged(object sender, EventArgs e)
        {
            c_txtLog.Width = this.Width-15;
            c_txtLog.Height = this.Height-30;
            c_txtLog.Left = 0;
            c_txtLog.Top = 0;
        }

        public void MoveTo(int _Top, int _Left)
        {
            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.Move -= LogWindow_Move;
                    this.Top = _Top;
                    this.Left = _Left;
                    this.Move += LogWindow_Move;
                }));
            }
            catch (Exception)
            { }

        }

        private void LogWindow_Load(object sender, EventArgs e)
        {
            MoveTo(c_frmLauncherWindow.Top - 5, c_frmLauncherWindow.Left + c_frmLauncherWindow.Width + 5);
            //this.TopMost = true;
            LogWindow_SizeChanged(sender, e);
            this.SizeChanged += LogWindow_SizeChanged;
            this.Move += LogWindow_Move;
        }
    }
}
