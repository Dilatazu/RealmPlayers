using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace DetailedList
{
    class MouseWheelMessageFilter : IMessageFilter
    {
        public MouseWheelMessageFilter(Func<Point, int, bool> _Action)
        {
            m_Action = _Action;
        }
        Func<Point, int, bool> m_Action;
        const int WM_MOUSEWHEEL = 0x20a;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEWHEEL)
            {
                Point pos = new Point(0, 0);
                int scrollDir = 0;
                try
                {
                    pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                    //IntPtr hWnd = WindowFromPoint(pos);
                    scrollDir = m.WParam.ToInt32() >> 16;
                }
                catch (OverflowException)
                { }
                return m_Action(pos, scrollDir);
            }
            return false;
        }
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
    }

}
