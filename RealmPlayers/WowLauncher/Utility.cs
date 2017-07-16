using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;

namespace VF_WoWLauncher
{
    public class Utility : VF_Utility
    {
        public static void SetPositionToMouse(System.Windows.Forms.Form _Form)
        {
            var scrPT = System.Windows.Forms.Control.MousePosition;
            _Form.Left = scrPT.X - _Form.Width / 2;
            _Form.Top = scrPT.Y - _Form.Height / 2;
        }
    }
}
