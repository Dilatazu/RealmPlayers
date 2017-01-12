using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using VF_WoWLauncher;

namespace DetailedList
{
    public enum SortValue
    {
        Top = 10,
        Top_Middle = 20,
        Middle = 50,
        Bottom_Middle = 90,
        Bottom = 100,
    }
    public partial class DetailedListItem
    {
        public class RightSide : UserControl
        {
            private Control[] m_Controls = null;
            public void __MergeInitialize()
            {
                List<Control> controls = new List<Control>();
                for (int i = 0; i < Controls.Count; ++i)
                    controls.Add(Controls[i]);

                m_Controls = controls.ToArray();
            }
            public virtual void SelectedChanged(bool _Selected)
            {
                /*for (int i = 0; i < m_Controls.Length; ++i)
                {
                    if (_Selected == false)
                    {
                        m_Controls[i].BlockEventDelegates("Click");
                    }
                    else
                    {
                        m_Controls[i].UnBlockEventDelegates("Click");
                    }
                }*/
            }
            public void _SetDetailedListItem(DetailedListItem _ListItem)
            {
                m_ListItem = _ListItem;
            }
            protected DetailedListItem m_ListItem;
        }

        int m_RightSideWidth;
        int m_MinHeight = 90;
        public DetailedListItem()
        {
            InitializeComponent();
            this.Paint += PaintEvent;
            this.MouseClick += This_MouseClick;
            this.MouseDown += This_MouseDown;
            this.MouseEnter += This_MouseEnter;
            this.MouseUp += This_MouseUp;
            this.Resize += This_Resize;
            this.tmrMouseLeave.Tick += tmrMouseLeave_Tick;

            m_RightSideWidth = 0;

            //pnOverlay.BackColor = System.Drawing.Color.Transparent;
            //pnOverlay.Visible = false;
            //pnOverlay.MouseClick += ListControlItem_MouseClick;
            //pnOverlay.MouseDown += metroRadioGroup_MouseDown;
            //pnOverlay.MouseEnter += metroRadioGroup_MouseEnter;
            //pnOverlay.MouseUp += metroRadioGroup_MouseUp;
        }

        void This_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        internal System.Windows.Forms.Timer tmrMouseLeave = new System.Windows.Forms.Timer { Interval = 10 };

        private int m_SortValue;
        public int SortValue
        {
            get { return m_SortValue; }
        }
        //Should only be called from DetailedList
        public void _SetSortValue(int _SortValue)
        {
            m_SortValue = _SortValue;
        }

    #region Properties
        Image m_Image;
        public Image Image 
        {
            get { return m_Image; }
            set
            {
                m_Image = value;
                Refresh();
            }
        }

        string m_Title = "[Title]";
        public string Title 
        {
            get { return m_Title; }
            set
            {
                m_Title = value;
                Refresh();
            }
        }

        string m_Description = "[Description]";
        public string Description 
        {
            get { return m_Description; }
            set 
            {
                m_Description = value;
                Refresh();
            }
        }

        string m_Authors = "[Authors]";
        public string Authors 
        {
            get { return m_Authors; }
            set 
            {
                m_Authors = value;
                Refresh();
            }
        }

        public delegate void EventSelectedDelegate(DetailedListItem _ListItem);
        public event EventSelectedDelegate EventSelectChanged;
        private bool m_Selected;
        public bool Selected 
        {
            get { return m_Selected; }
            set 
            {
                if (m_Selected != value)
                {
                    m_Selected = value;
                    if (EventSelectChanged != null)
                        EventSelectChanged(this);
                    Refresh();
                }
            }
        }

        private bool m_Expanded = false;
        public bool Expanded
        {
            get { return m_Expanded; }
            set
            {
                if (m_Expanded != value)
                {
                    m_Expanded = value;
                    Refresh();
                }
            }
        }
    #endregion

    #region Mouse coding

        private enum MouseCapture
        {
          Outside,
          Inside
        }
        private enum ButtonState
        {
          ButtonUp,
          ButtonDown,
          Disabled
        }
        ButtonState m_ButtonState;
        MouseCapture m_MouseState;

        private void This_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Selected = true;
        }

        private void This_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_ButtonState = ButtonState.ButtonDown;
            Refresh();
        }

        private void This_MouseEnter(object sender, System.EventArgs e)
        {
            m_MouseState = MouseCapture.Inside;
            tmrMouseLeave.Start();
            Refresh();
        }
        public void _MouseHoverOut()
        {
            m_MouseState = MouseCapture.Outside;
            tmrMouseLeave.Stop();
        }
        public void _MouseHoverIn()
        {
            m_MouseState = MouseCapture.Inside;
            tmrMouseLeave.Start();
        }

        private void This_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_ButtonState = ButtonState.ButtonUp;
            Refresh();
        }

        private void tmrMouseLeave_Tick(System.Object sender, System.EventArgs e)
        {
            Point scrPT = Control.MousePosition;
            Point ctlPT = this.PointToClient(scrPT);

            if (ctlPT.X < 0 || ctlPT.Y < 0 || ctlPT.X > this.Width || ctlPT.Y > this.Height) 
            {
                // Stop timer
                if(m_ButtonState != ButtonState.ButtonDown)
                    tmrMouseLeave.Stop();
                m_MouseState = MouseCapture.Outside;
                Refresh();
            } 
            else 
            {
                m_MouseState = MouseCapture.Inside;
                Refresh();
            }
        }
    #endregion

    #region Painting

        Dictionary<Control, Delegate[]> m_ControlClickDelegates = new Dictionary<Control, Delegate[]>();
        public void SetRightSideControls(RightSide _RightSide)
        {
            _RightSide._SetDetailedListItem(this);
            _RightSide.__MergeInitialize();
            m_RightSideWidth = _RightSide.Width;
            if (_RightSide.Height > m_MinHeight)
                m_MinHeight = _RightSide.Height;
            for (int i = 0; i < _RightSide.Controls.Count; ++i)
            {
                Control control = _RightSide.Controls[i];
                control.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                control.Location = new Point((this.Width - m_RightSideWidth) + control.Left, control.Top);

                control.MouseClick += This_MouseClick;
                control.MouseDown += This_MouseDown;
                control.MouseEnter += This_MouseEnter;
                control.MouseUp += This_MouseUp;

                _RightSide.Controls.Remove(control);
                this.Controls.Add(control);
                --i;
            }
            var eventLambda = new EventSelectedDelegate((DetailedListItem _ListItem) =>
            {
                _RightSide.SelectedChanged(m_Selected);
            });
            EventSelectChanged += eventLambda;
            eventLambda(this);
        }

        /*private void _______SetRightSideControls2(ContainerControl _Controls)
        {
            m_RightSideWidth = _Controls.Width;
            m_MinHeight = _Controls.Height;
            //this.SuspendLayout();
            for (int i = 0; i < _Controls.Controls.Count; ++i)
            {
                Control control = _Controls.Controls[i];
                control.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                control.Location = new Point((this.Width - m_RightSideWidth) + control.Left, control.Top);

                if (control.GetEventDelegates("Click").Length > 0)
                {
                    /*var eventLambda = new EventSelectedDelegate(() =>
                    {
                        if (mSelected == false)
                            pnOverlay.Visible = true;
                        else
                            pnOverlay.Visible = false;
                    });*//*
                    var eventLambda = new EventSelectedDelegate((DetailedListItem _ListItem) =>
                    {
                        if (m_Selected == false)
                        {
                            control.BlockEventDelegates("Click");
                        }
                        else
                        {
                            control.UnBlockEventDelegates("Click");
                        }
                    });

                    EventSelectChanged += eventLambda;
                    eventLambda(this);
                }
                else
                {
                    //control.RemoveEvents("Click");
                }
                control.MouseClick += This_MouseClick;
                control.MouseDown += This_MouseDown;
                control.MouseEnter += This_MouseEnter;
                control.MouseUp += This_MouseUp;
            
                //_Controls.Controls.Remove(control);
                this.Controls.Add(control);
                --i;
            }
            _Controls.Enabled = false; //fungerar(?) istället för //_Controls.Controls.Remove(control);
            //this.ResumeLayout(true);
        }*/
        private void Paint_DrawBackground(Graphics gfx)
        {
            //
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            ///// Build a rounded rectangle
            GraphicsPath p = new GraphicsPath();
            const int Roundness = 5;
            p.StartFigure();
            p.AddArc(new Rectangle(rect.Left, rect.Top, Roundness, Roundness), 180, 90);
            p.AddLine(rect.Left + Roundness, 0, rect.Right - Roundness, 0);
            p.AddArc(new Rectangle(rect.Right - Roundness, 0, Roundness, Roundness), -90, 90);
            p.AddLine(rect.Right, Roundness, rect.Right, rect.Bottom - Roundness);
            p.AddArc(new Rectangle(rect.Right - Roundness, rect.Bottom - Roundness, Roundness, Roundness), 0, 90);
            p.AddLine(rect.Right - Roundness, rect.Bottom, rect.Left + Roundness, rect.Bottom);
            p.AddArc(new Rectangle(rect.Left, rect.Height - Roundness, Roundness, Roundness), 90, 90);
            p.CloseFigure();


            ///// Draw the background ///
            Color[] ColorScheme = null;
            SolidBrush brdr = null;

            if (m_ButtonState == ButtonState.Disabled)
            {
                // normal
                brdr = ColorSchemes.DisabledBorder;
                ColorScheme = ColorSchemes.DisabledAllColor;
            } 
            else 
            {
                if (m_Selected) 
                {
                    // Selected
                    brdr = ColorSchemes.SelectedBorder;

                    if (m_ButtonState == ButtonState.ButtonUp & m_MouseState == MouseCapture.Outside) // normal
                        ColorScheme = ColorSchemes.SelectedNormal;
                    else if (m_ButtonState == ButtonState.ButtonUp & m_MouseState == MouseCapture.Inside) // hover 
                        ColorScheme = ColorSchemes.SelectedHover;
                    else if (m_ButtonState == ButtonState.ButtonDown & m_MouseState == MouseCapture.Outside)
                        ColorScheme = ColorSchemes.SelectedNormal;
                    else if (m_ButtonState == ButtonState.ButtonDown & m_MouseState == MouseCapture.Inside) // pressed
                        ColorScheme = ColorSchemes.SelectedPressed;
                } 
                else 
                {
                    // Not selected
                    brdr = ColorSchemes.UnSelectedBorder;

                    if (m_ButtonState == ButtonState.ButtonUp & m_MouseState == MouseCapture.Outside)
                    {
                        // normal
                        brdr = ColorSchemes.DisabledBorder;
                        ColorScheme = ColorSchemes.UnSelectedNormal;
                    }
                    else if (m_ButtonState == ButtonState.ButtonUp & m_MouseState == MouseCapture.Inside) // hover 
                        ColorScheme = ColorSchemes.UnSelectedHover;
                    else if (m_ButtonState == ButtonState.ButtonDown & m_MouseState == MouseCapture.Outside)
                    {
                        brdr = ColorSchemes.DisabledBorder;
                        ColorScheme = ColorSchemes.UnSelectedNormal;
                    }
                    else if (m_ButtonState == ButtonState.ButtonDown & m_MouseState == MouseCapture.Inside) // pressed
                        ColorScheme = ColorSchemes.UnSelectedPressed;
                }
            }

            // Draw
            LinearGradientBrush b = new LinearGradientBrush(rect, Color.White, Color.Black, LinearGradientMode.Vertical);
            ColorBlend blend = new ColorBlend();
            blend.Colors = ColorScheme;
            blend.Positions = new float[] { 0f, 0.1f, 0.9f, 0.95f, 1f };
            b.InterpolationColors = blend;
            gfx.FillPath(b, p);

            //// Draw border
            gfx.DrawPath(new Pen(brdr), p);

            //// Draw bottom border if Normal state (not hovered)
            //Gör inte att det ser bättre ut. blir helt weird.
            /*if (m_MouseState == MouseCapture.Outside) 
            {
                rect = new Rectangle(rect.Left, this.Height - 1, rect.Width, 1);
                b = new LinearGradientBrush(rect, Color.Blue, Color.Yellow, LinearGradientMode.Horizontal);
                blend = new ColorBlend();
                blend.Colors = new Color[] { Color.White, Color.LightGray, Color.White };
                blend.Positions = new float[] { 0f, 0.5f, 1f };
                b.InterpolationColors = blend;

                gfx.FillRectangle(b, rect);
            }*/
        }

        private void Paint_DrawButton(Graphics gfx)
        {

            Font fnt = null;
            SizeF sz;
            RectangleF layoutRect;
            StringFormat SF = new StringFormat { Trimming = StringTrimming.EllipsisCharacter };
            Rectangle workingRect = new Rectangle(40, 0, (this.Width - m_RightSideWidth) - 40 - 6, this.Height);

            // Draw song name
            fnt = new Font("Arial", 14);
            sz = gfx.MeasureString(m_Title, fnt);
            layoutRect = new RectangleF(40, 5, workingRect.Width, sz.Height);
            gfx.DrawString(m_Title, fnt, Brushes.Black, layoutRect, SF);

            // Draw artist name
            fnt = new Font("Arial", 10);
            sz = gfx.MeasureString(m_Description, fnt, workingRect.Width);

            int thisNewHeight = 0;
            if (m_Expanded == true)
            {
                thisNewHeight = (int)sz.Height + 56;
                if (thisNewHeight < m_MinHeight)
                    thisNewHeight = m_MinHeight;
            }
            else
            {
                thisNewHeight = m_MinHeight;
            }
            if (thisNewHeight < 75)
            {
                thisNewHeight = 75;//Absolut min height för all texten att fungera
            }
            this.Height = thisNewHeight;
            layoutRect = new RectangleF(42, 30, workingRect.Width, this.Height - 56);
            gfx.DrawString(m_Description, fnt, Brushes.Black, layoutRect, SF);

            // Draw album name
            fnt = new Font("Arial", 10);
            sz = gfx.MeasureString(m_Authors, fnt);
            layoutRect = new RectangleF(42, this.Height - 25, workingRect.Width, sz.Height);
            gfx.DrawString(m_Authors, fnt, Brushes.Black, layoutRect, SF);

            // Album Image
            if (m_Image != null)
                gfx.DrawImage(m_Image, new Point(7, 7));
            else
                gfx.DrawImage(ImageList1.Images[0], new Point(7, 7));
        }

        private void PaintEvent(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (DisableRedraw == false)
            {
                Graphics gfx = e.Graphics;
                Paint_DrawBackground(gfx);
                Paint_DrawButton(gfx);
            }
        }

    #endregion


        public bool DisableRedraw = false;

        private void RatingBar1_Load(object sender, EventArgs e)
        {
        }

        private void lblDuration_Click(object sender, EventArgs e)
        {
        
        }

        private void DetailedListItem_Load(object sender, EventArgs e)
        {

        }
    }
}

