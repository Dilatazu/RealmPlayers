using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace DetailedList
{
    public partial class RightSideForumPost : DetailedListItem.RightSide
    {
        public delegate void GotoForumActionDelegate();
        public delegate void HidePostActionDelegate(DetailedListItem _ListItem);

        public RightSideForumPost(GotoForumActionDelegate _GotoForumAction = null, HidePostActionDelegate _HidePostAction = null)
        {
            m_GotoForumAction = _GotoForumAction;
            m_HidePostAction = _HidePostAction;
            InitializeComponent();
        }
        private GotoForumActionDelegate m_GotoForumAction = null;
        private HidePostActionDelegate m_HidePostAction = null;

        private void btnHidePost_Click(object sender, EventArgs e)
        {
            if (m_HidePostAction != null)
                m_HidePostAction(m_ListItem);
        }

        private void RightSideAddonUpdate_Load(object sender, EventArgs e)
        {

        }

        private void btnGotoForum_Click(object sender, EventArgs e)
        {
            if (m_GotoForumAction != null)
                m_GotoForumAction();
        }
    }
}
