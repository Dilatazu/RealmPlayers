using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;  

namespace VF_WoWLauncher
{
    public class ConsoleUtility
    {
        [DllImport("kernel32.dll",
               EntryPoint = "GetStdHandle",
               SetLastError = true,
               CharSet = CharSet.Auto,
               CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();
        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;

        public static bool g_ConsoleCreated = false;
        public static void CreateConsole()
        {
            //Console.WriteLine("This text you can see in debug output window.");
            if (g_ConsoleCreated == false)
            {
                AllocConsole();
                IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
                SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
                StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
                g_ConsoleCreated = true;
            }
            //Console.WriteLine("This text you can see in console window.");
        }  
    }
}
