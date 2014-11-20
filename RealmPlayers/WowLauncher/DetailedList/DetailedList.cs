using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DetailedList
{
    public partial class DetailedList
    {
        internal System.Windows.Forms.Timer tmrMouseLeave = new System.Windows.Forms.Timer { Interval = 100 };

        public event ItemSelectEventHandler EventItemSelected;
        public delegate void ItemSelectEventHandler(int _ItemIndex);

        public DetailedList()
        {
            InitializeComponent();
            flpListBox.Resize += flpListBox_Resize;
            flpListBox.MouseEnter += flpListBox_MouseEnter;
            tmrMouseLeave.Tick += tmrMouseLeave_Tick;

            this.EnabledChanged += DetailedList_EnabledChanged;
            //flpListBox.MouseWheel += flpListBox_MouseWheel;
            //Application.AddMessageFilter(new MouseWheelMessageFilter(Global_MouseScroll));
        }

        public new void Focus()
        {
            flpListBox.Focus();
        }

        void DetailedList_EnabledChanged(object sender, EventArgs e)
        {
            flpListBox.Enabled = this.Enabled;
            if (this.Enabled == false)
                flpListBox.AutoScroll = false;
            else
                flpListBox.AutoScroll = true;
        }

        //public void EnableRedraw()
        //{
        //    foreach(DetailedListItem item in flpListBox.Controls)
        //    {
        //        item.DisableRedraw = false;
        //    }
        //}
        
        //public void DisableRedraw()
        //{
        //    foreach(DetailedListItem item in flpListBox.Controls)
        //    {
        //        item.DisableRedraw = true;
        //    }
        //}

        public bool Global_MouseScroll(Point _Pos, int _ScrollDir)
        {
            if (this.Enabled == false || flpListBox.Enabled == false)
                return false;

            Point ctlPT = flpListBox.PointToClient(_Pos);
            if (ctlPT.X > 0 && ctlPT.Y > 0 && ctlPT.X < flpListBox.Width && ctlPT.Y < flpListBox.Height)
            {
                int newScrollValue = int.MaxValue;
                if (_ScrollDir > 0)
                {
                    int scrollValue = -9999;
                    for (int i = 0; i < flpListBox.Controls.Count; i++)
                    {
                        int currValue = flpListBox.Controls[i].Top + 50;
                        if (currValue < 50 && currValue < flpListBox.Controls[i].Height && currValue > scrollValue)
                        {
                            scrollValue = currValue;
                            newScrollValue = flpListBox.VerticalScroll.Value + scrollValue - 50;
                        }
                    }
                }
                else if (_ScrollDir < 0)
                {
                    int scrollValue = 9999;
                    for (int i = 0; i < flpListBox.Controls.Count; i++)
                    {
                        int currValue = (flpListBox.Controls[i].Top - 50 + flpListBox.Controls[i].Height) - (flpListBox.Height);
                        if (currValue > -50 && currValue > -flpListBox.Controls[i].Height && currValue < scrollValue)
                        {
                            scrollValue = currValue;
                            newScrollValue = flpListBox.VerticalScroll.Value + scrollValue + 50;
                        }
                    }
                }
                if (newScrollValue != int.MaxValue)
                {
                    if (newScrollValue > flpListBox.VerticalScroll.Maximum)
                        newScrollValue = flpListBox.VerticalScroll.Maximum;
                    if (newScrollValue < flpListBox.VerticalScroll.Minimum)
                        newScrollValue = flpListBox.VerticalScroll.Minimum;
                    flpListBox.VerticalScroll.Value = newScrollValue;
                    flpListBox.PerformLayout();
                }
                return true;
            }
            return false;
        }
        void flpListBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 || e.Delta < 0)
            {
                //foreach (DetailedListItem item in flpListBox.Controls)
                //{
                //    item._MouseHoverOut();
                //}
            }
            if (e.Delta > 0)
            {
                int scrollValue = -9999;
                for (int i = 0; i < flpListBox.Controls.Count; i++)
                {
                    int currValue = flpListBox.Controls[i].Top;
                    if (currValue < 120 && currValue < flpListBox.Controls[i].Height && currValue > scrollValue)
                    {
                        scrollValue = currValue;
                    }
                }
                if (scrollValue != -9999)
                {
                    flpListBox.VerticalScroll.Value = flpListBox.VerticalScroll.Value + scrollValue;
                    flpListBox.PerformLayout();
                }
            }
            else if (e.Delta < 0)
            {
                int scrollValue = 9999;
                for (int i = 0; i < flpListBox.Controls.Count; i++)
                {
                    int currValue = (flpListBox.Controls[i].Top + flpListBox.Controls[i].Height) - (flpListBox.Height - (flpListBox.HorizontalScroll.Visible == true ? SystemInformation.HorizontalScrollBarHeight : 0));
                    if (currValue > -120 && currValue > -flpListBox.Controls[i].Height && currValue < scrollValue)
                    {
                        scrollValue = currValue;
                    }
                }
                if (scrollValue != 9999)
                {
                    flpListBox.VerticalScroll.Value = flpListBox.VerticalScroll.Value + scrollValue;
                    flpListBox.PerformLayout();
                }
            }
            //if (e.Delta > 0 || e.Delta < 0)
            //{
            //    Point scrPT = Control.MousePosition;
            //    Point ctlPT = flpListBox.PointToClient(scrPT);
            //    ((DetailedListItem)flpListBox.GetChildAtPoint(ctlPT))._MouseHoverIn();
            //    EnableRedraw();
            //    foreach (DetailedListItem item in flpListBox.Controls)
            //    {
            //        item.Refresh();
            //    }
            //}
        }

        void CloseInvisibleExpanded(bool _ForceCloseAll)
        {
            if (m_CurrentSelected == null)
                return;

            int topBefore = m_CurrentSelected.Top;
            for (int i = 0; i < flpListBox.Controls.Count; i++)
            {
                var currItem = (DetailedListItem)flpListBox.Controls[i];
                if (currItem != m_CurrentSelected)
                {
                    if (_ForceCloseAll == true || currItem.Top + currItem.Height < 0 || currItem.Top > flpListBox.Height)
                    {
                        currItem.Expanded = false;
                    }
                }
            }
            int topChange = topBefore - m_CurrentSelected.Top;
            if (topChange != 0)
            {
                //Form1.ActiveForm.Text = "m_CurrentSelected.Top=(" + topBefore + "->" + m_CurrentSelected.Top + ")" + DateTime.Now.Second + "." + DateTime.Now.Millisecond;
                //m_CurrentSelected.Top
                if (topChange > flpListBox.VerticalScroll.Value)
                    flpListBox.VerticalScroll.Value = 0;
                else
                    flpListBox.VerticalScroll.Value -= topChange;
                //topBefore - m_CurrentSelected.Top
            }
        }

        void tmrMouseLeave_Tick(object sender, EventArgs e)
        {
            CloseInvisibleExpanded(false);//Experimental
            return;//Experimental

            /*Point scrPT = Control.MousePosition;
            Point ctlPT = flpListBox.PointToClient(scrPT);

            int margin = 20;

            if (ctlPT.X < 0 - margin || ctlPT.Y < 0 - margin || ctlPT.X > flpListBox.Width + margin || ctlPT.Y > flpListBox.Height + margin)
            {
                //Form1.ActiveForm.Text = "Outside " + DateTime.Now.Second + "." + DateTime.Now.Millisecond;

                CloseInvisibleExpanded(true);
                if (Control.MouseButtons.HasFlag(MouseButtons.Left) == false)
                    tmrMouseLeave.Stop();
            }*/
        }
        void flpListBox_MouseEnter(object sender, EventArgs e)
        {
            if(tmrMouseLeave.Enabled == false)
                tmrMouseLeave.Start();
        }
        private int UniqueIDCounter = 0;
        public void AddItem(int _SortValue, Image _LeftSideImage, string _Title, string _Description, string _Authors, DetailedListItem.RightSide _RightSide = null)
        {
            DetailedListItem newListItem = new DetailedListItem();
            newListItem.Name = "item" + UniqueIDCounter++;
            newListItem.Margin = new Padding(0);
            newListItem.Title = _Title;
            newListItem.Description = _Description;
            newListItem.Authors = _Authors;
            newListItem.Image = _LeftSideImage;
            newListItem._SetSortValue(int.MinValue);

            if (_RightSide != null)
                newListItem.SetRightSideControls(_RightSide);

            if (flpListBox.Controls.Count == 0)
            {
                flpListBox.Controls.Add(newListItem);
                newListItem._SetSortValue(_SortValue);
                SetupAnchors();
            }
            else
            {
                newListItem.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                flpListBox.Controls.Add(newListItem);
                SetSortValue(newListItem, _SortValue);
            }
            newListItem.EventSelectChanged += Item_SelectionChanged;
            newListItem.MouseEnter += flpListBox_MouseEnter;
        }

        public void SetSortValue(int _Index, int _SortValue)
        {
            DetailedListItem item = (DetailedListItem)flpListBox.Controls[_Index];
            SetSortValue(item, _SortValue);
        }
        public void SetSortValue(DetailedListItem _Item, int _SortValue)
        {
            if (_Item.SortValue == _SortValue)
                return;
            int prevIndex = int.MaxValue;
            _Item._SetSortValue(_SortValue);
            if (flpListBox.Controls.Count > 1)
            {
                for (int i = 0; i < flpListBox.Controls.Count; ++i)
                {
                    var currItem = (DetailedListItem)flpListBox.Controls[i];
                    if (currItem == _Item)
                    {
                        prevIndex = i;
                    }
                    else if (currItem.SortValue < _SortValue)
                    {
                        if (i - 1 == prevIndex)//>= 0 && (DetailedListItem)flpListBox.Controls[i - 1] == _Item)
                            break;//Redan på rätt plats

                        if (prevIndex != int.MaxValue && i > 0)
                        {
                            i = i - 1;
                        }

                        flpListBox.Controls.SetChildIndex(_Item, i);
                        if (prevIndex == 0)
                        {
                            flpListBox.Controls[0].Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            flpListBox.Controls[0].Width = flpListBox.Width - (flpListBox.VerticalScroll.Visible == true ? SystemInformation.VerticalScrollBarWidth : 0);
                            _Item.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                        }
                        else if (i == 0)
                        {
                            _Item.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            _Item.Width = flpListBox.Width - (flpListBox.VerticalScroll.Visible == true ? SystemInformation.VerticalScrollBarWidth : 0);
                            flpListBox.Controls[1].Anchor = AnchorStyles.Left | AnchorStyles.Right;
                        }
                        break;
                    }
                }
            }
        }

        public void RemoveItem(int _Index)
        {
            DetailedListItem item = (DetailedListItem)flpListBox.Controls[_Index];

            int prevTop = item.Top;// int.MaxValue;
            DetailedListItem newSelected = null;
            item.tmrMouseLeave.Stop();
            item.EventSelectChanged -= Item_SelectionChanged;
            item.MouseEnter -= flpListBox_MouseEnter;
            if (item.Selected == true)
            {
                if (flpListBox.Controls.Count > _Index + 1)
                {
                    newSelected = ((DetailedListItem)flpListBox.Controls[_Index + 1]);
                    newSelected.Selected = true;
                }
                else if(_Index - 1 >= 0)
                {
                    newSelected = ((DetailedListItem)flpListBox.Controls[_Index - 1]);
                    newSelected.Selected = true;
                }
            }
            if (_Index == 0 && flpListBox.Controls.Count > 1)
            {
                flpListBox.Controls[1].Anchor = AnchorStyles.Left | AnchorStyles.Top;
                flpListBox.Controls[1].Width = flpListBox.Width - (flpListBox.VerticalScroll.Visible == true ? SystemInformation.VerticalScrollBarWidth : 0);
            }
            flpListBox.Controls.Remove(item);
            item.Dispose();
            if (newSelected != null)
            {
                int deltaMove = newSelected.Top - prevTop;
                int newScrollValue = flpListBox.VerticalScroll.Value + deltaMove;
                if (newScrollValue > flpListBox.VerticalScroll.Maximum)
                    newScrollValue = flpListBox.VerticalScroll.Maximum;
                if (newScrollValue < flpListBox.VerticalScroll.Minimum)
                    newScrollValue = flpListBox.VerticalScroll.Minimum;

                flpListBox.VerticalScroll.Value = newScrollValue;
                flpListBox.PerformLayout();
            }
        }

        public void RemoveItem(string name)
        {
            DetailedListItem item = (DetailedListItem)flpListBox.Controls[name];
            RemoveItem(item);
        }
        public void RemoveItem(DetailedListItem _Item)
        {
            for (int i = 0; i < flpListBox.Controls.Count; ++i)
            {
                if (((DetailedListItem)flpListBox.Controls[i]) == _Item)
                {
                    RemoveItem(i);
                    return;
                }
            }
            throw new Exception("Could not RemoveItem" + _Item.Name);
            //_Item.tmrMouseLeave.Stop();
            //_Item.EventSelectChanged -= Item_SelectionChanged;
            //_Item.MouseEnter -= flpListBox_MouseEnter;
            //flpListBox.Controls.Remove(_Item);
            
            //_Item.Dispose();
            //SetupAnchors();
        }

        public void Clear()
        {
            while(flpListBox.Controls.Count != 0)
            {
                DetailedListItem item = (DetailedListItem)flpListBox.Controls[0];
                flpListBox.Controls.Remove(item);
                // remove the event hook
                item.EventSelectChanged -= Item_SelectionChanged;
                item.MouseEnter -= flpListBox_MouseEnter;

                item.Dispose();
            }
            m_CurrentSelected = null;
        }

        public int Count {
            get { return flpListBox.Controls.Count; }
        }

        private void SetupAnchors()
        {
            if (flpListBox.Controls.Count > 0) 
            {
                for (int i = 0; i <= flpListBox.Controls.Count - 1; i++) {
                    Control c = flpListBox.Controls[i];

                    if (i == 0) {
                    // Its the first control, all subsequent controls follow 
                    // the anchor behavior of this control.
                        c.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    c.Width = flpListBox.Width - SystemInformation.VerticalScrollBarWidth;

                    } else {
                    // It is not the first control. Set its anchor to
                    // copy the width of the first control in the list.
                    c.Anchor = AnchorStyles.Left | AnchorStyles.Right;

                    }

                }
            }
            flpListBox_Resize(this, new EventArgs());
        }

        private void flpListBox_Resize(object sender, System.EventArgs e)
        {
            if (flpListBox.Controls.Count > 0) 
            {
                int totalHeight = 0;
                for (int i = 0; i < flpListBox.Controls.Count; i++)
                {
                    totalHeight += flpListBox.Controls[i].Height;
                }
                
                if (flpListBox.HorizontalScroll.Visible == true)
                {
                    //((DetailedListItem)flpListBox.Controls[0]).Visible = false;
                }
                if (totalHeight >= this.Height 
                    || (flpListBox.HorizontalScroll.Visible == true && totalHeight >= this.Height - SystemInformation.HorizontalScrollBarHeight))
                    flpListBox.Controls[0].Width = flpListBox.Width - SystemInformation.VerticalScrollBarWidth;
                else
                    flpListBox.Controls[0].Width = flpListBox.Width;
                if(flpListBox.HorizontalScroll.Visible == true)
                    flpListBox.HorizontalScroll.Value = 0;

                //flpListBox.PerformLayout();
                //flpListBox.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
                //flpListBox.Dock = DockStyle.Fill;
                //flpListBox.HorizontalScroll.Enabled = false;
                //flpListBox.HorizontalScroll.Visible = false;
                //Invalidate();

                if (flpListBox.HorizontalScroll.Visible == true)
                {
                    System.Threading.Tasks.Task resizeFixTask = new System.Threading.Tasks.Task(() =>
                    {
                        System.Threading.Thread.Sleep(0);
                        flpListBox.BeginInvoke(new Action(() =>
                        {
                            flpListBox.Resize -= flpListBox_Resize;
                            //flpListBox.Width += 1;
                            if (totalHeight < this.Height)
                            {
                                if(flpListBox.Controls[0].Width != flpListBox.Width)
                                    flpListBox.Controls[0].Width = flpListBox.Width;
                            }
                            //((DetailedListItem)flpListBox.Controls[0]).Visible = true;
                            //((DetailedListItem)flpListBox.Controls[0]).Refresh();
                            flpListBox.Resize += flpListBox_Resize;
                            //flpListBox.HorizontalScroll.Visible = false;
                            //flpListBox.HorizontalScroll.Enabled = false;
                            //flpListBox.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
                            //flpListBox.Dock = DockStyle.Fill;
                            //flpListBox_Resize(null, e);
                        }));
                    });
                    resizeFixTask.Start();
                }
            }
        }

        DetailedListItem m_CurrentSelected = null;
        public DetailedListItem CurrentSelected
        {
            get { return m_CurrentSelected; }
        }
        public int CurrentSelectedIndex
        {
            get { return flpListBox.Controls.IndexOf(m_CurrentSelected); }
        }
        private void Item_SelectionChanged(DetailedListItem _ListItem)
        {
            if (_ListItem.Selected == true)
            {
                int newSelectIndex = flpListBox.Controls.IndexOf(_ListItem);
                if (m_CurrentSelected != null)
                {
                    int prevSelectIndex = flpListBox.Controls.IndexOf(m_CurrentSelected);
                    m_CurrentSelected.Selected = false;

                    if (newSelectIndex < prevSelectIndex)
                    {
                        for (int i = newSelectIndex + 1; i < flpListBox.Controls.Count; i++)
                        {
                            var currItem = (DetailedListItem)flpListBox.Controls[i];
                            currItem.Expanded = false;
                        }
                    }
                }
                m_CurrentSelected = _ListItem;
                _ListItem.Expanded = true;
                if(EventItemSelected != null)
                    EventItemSelected(newSelectIndex);
            }
        }
    }
}

