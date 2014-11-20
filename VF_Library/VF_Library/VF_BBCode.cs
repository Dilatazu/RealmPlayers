using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF
{
    class BBCode
    {
        public class Color
        {
            public static string Start(string _Color)
            {
                return "[color=" + _Color + "]";
            }
            public static string End()
            {
                return "[/color]";
            }
        }
        public class URL
        {
            public static string Start(string _URL)
            {
                return "[url=" + _URL + "]";
            }
            public static string Start()
            {
                return "[url]";
            }
            public static string End()
            {
                return "[/url]";
            }
        }
        public class Size
        {
            public enum E
            {
                Tiny = 50,
                Small = 85,
                Normal = 100,
                Semi_Large = 125,
                Large = 150,
                Huge = 200,
            }
            public static string Start(Size.E _Size)
            {
                return "[size=" + (int)_Size + "]";
            }
            public static string End()
            {
                return "[/size]";
            }
        }
        public class List
        {
            public static string Start()
            {
                return "[list]";
            }
            public static string NewItem()
            {
                return "[*]";
            }
            public static string End()
            {
                return "[/list]";
            }
        }
        public class Code
        {
            public static string Start()
            {
                return "[code]";
            }
            public static string End()
            {
                return "[/code]";
            }
        }
        public class Bold
        {
            public static string Start()
            {
                return "[b]";
            }
            public static string End()
            {
                return "[/b]";
            }
        }
        public class Italic
        {
            public static string Start()
            {
                return "[i]";
            }
            public static string End()
            {
                return "[/i]";
            }
        }
        public class Underline
        {
            public static string Start()
            {
                return "[u]";
            }
            public static string End()
            {
                return "[/u]";
            }
        }



        public static string Create_List(string[] _List)
        {
            string retValue = List.Start();
            foreach (var item in _List)
            {
                retValue += List.NewItem() + item;
            }
            return retValue + List.End();
        }
        public static string Create_Sized(string _Text, Size.E _Size)
        {
            return Size.Start(_Size) + _Text + Size.End();
        }
        public static string Create_Colored(string _Text, string _Color)
        {
            return Color.Start(_Color) + _Text + Color.End();
        }
        public static string Create_Bold(string _Text)
        {
            return Bold.Start() + _Text + Bold.End();
        }
        public static string Create_Italic(string _Text)
        {
            return Italic.Start() + _Text + Italic.End();
        }
        public static string Create_Underline(string _Text)
        {
            return Underline.Start() + _Text + Underline.End();
        }
        public static string Create_Code(string _Text)
        {
            return Code.Start() + _Text + Code.End();
        }
        public static string Create_URL(string _URL, string _Text)
        {
            return URL.Start(_URL) + _Text + URL.End();
        }
        public static string Create_URL(string _URL)
        {
            return URL.Start() + _URL + URL.End();
        }
    }
}
