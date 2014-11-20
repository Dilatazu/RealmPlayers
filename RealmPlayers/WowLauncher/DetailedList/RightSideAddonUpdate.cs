using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace DetailedList
{
    public partial class RightSideAddonUpdate : DetailedListItem.RightSide
    {
        public delegate void SetProgressBarDelegate(float _Percentage);
        public delegate void UpdateActionDelegate(DetailedListItem _ListItem, SetProgressBarDelegate _SetProgressBar);
        public delegate void MoreInfoActionDelegate();

        public RightSideAddonUpdate(string _UpdateButtonText, UpdateActionDelegate _UpdateAction = null, MoreInfoActionDelegate _MoreInfoAction = null)
        {
            m_UpdateAction = _UpdateAction;
            m_MoreInfoAction = _MoreInfoAction;
            InitializeComponent();
            btnUpdate.Text = _UpdateButtonText;
        }
        public RightSideAddonUpdate(UpdateActionDelegate _UpdateAction = null, MoreInfoActionDelegate _MoreInfoAction = null)
        {
            m_UpdateAction = _UpdateAction;
            m_MoreInfoAction = _MoreInfoAction;
            InitializeComponent();
        }
        private UpdateActionDelegate m_UpdateAction = null;
        private MoreInfoActionDelegate m_MoreInfoAction = null;

        public void SetProgressBar(float _Percentage)
        {
            int newValue = (int)(_Percentage * 100.0f);
            pgrProgress.BeginInvoke(new Action(() =>
            {
                if (newValue != pgrProgress.Value 
                    && newValue >= pgrProgress.Minimum 
                    && newValue <= pgrProgress.Maximum)
                {
                    pgrProgress.Value = newValue;
                }
            }));
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (m_UpdateAction != null)
            {
                btnUpdate.Visible = false;
                pgrProgress.Visible = true;
                (new System.Threading.Tasks.Task(() => {m_UpdateAction(m_ListItem, SetProgressBar);})).Start();
            }
            /*(new System.Threading.Tasks.Task(() =>
            {
                for (int i = 0; i < 20; ++i)
                {
                    pgrProgress.BeginInvoke(new Action(() =>
                    {
                        pgrProgress.Value += 5;
                    }));
                    System.Threading.Thread.Sleep(300);
                }
                System.Threading.Thread.Sleep(1500);
                pgrProgress.BeginInvoke(new Action(() =>
                {
                    pgrProgress.Value = 0;
                    btnUpdate.Visible = true;
                    pgrProgress.Visible = false;
                }));
            })).Start();*/
        }

        private void RightSideAddonUpdate_Load(object sender, EventArgs e)
        {

        }

        private void btnMoreInfo_Click(object sender, EventArgs e)
        {
            if (m_MoreInfoAction != null)
                m_MoreInfoAction();
        }
    }
}
