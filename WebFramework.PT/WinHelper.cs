using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework.PT
{
    public class WinHelper
    {
        #region WinAPI

        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetOpenFileName(ref OpenFileName ofn);

        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetSaveFileName(ref OpenFileName ofn);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct OpenFileName
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;
            public string lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public IntPtr lpstrFile;
            public int nMaxFile;
            public string lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public IntPtr lCustData;
            public IntPtr lpfnHook;
            public string lpTemplateName;
            public IntPtr pvReserved;
            public int dwReserved;
            public int flagsEx;
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, int[] pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        public bool isWindows11
        {
            get
            {
                var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

                var currentBuildStr = (string)reg.GetValue("CurrentBuild");
                var currentBuild = int.Parse(currentBuildStr);

                return currentBuild >= 22000;
            }
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        #endregion

        public void OnLoad()
        {
            Logger.LogInfo("Loaded WinHelper");
        }

        public void EnableMica(IntPtr hwnd)
        {
            if (!isWindows11) { return; }

            //Enable Mica On Parent Window
            Logger.LogInfo("Enabling Mica On Window: " + hwnd);
            int enable = 0x02;
            DwmSetWindowAttribute(hwnd, 38, ref enable, Marshal.SizeOf(typeof(int)));
        }

        public void SetTitlebarColor(IntPtr hwnd, int color)
        {
            DwmSetWindowAttribute(hwnd, 35, new int[] { color }, 4);
        }
        public void EnableRoundedCorners(IntPtr hwnd)
        {
            DwmSetWindowAttribute(hwnd, 33, new int[] { 2 }, 4);
        }


        //https://stackoverflow.com/a/72172845/18071273
        public bool IsDark()
        {
            int res = 1;
            try
            {
                res = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1);
            }
            catch
            {
                //Exception Handling     
            }

            return res == 0;
        }

        public async Task<string> OpenFileSaver(DOM ctx, string fileExtension)
        {
            var r = "";

            var ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);

            ofn.lpstrFilter = fileExtension.ToUpper() + " Files\0*." + fileExtension + "\0\0";

            ofn.lpstrFile = Marshal.StringToBSTR(new string(' ', 1024)); // 1KB
            ofn.nMaxFile = 1024;
            ofn.lpstrFileTitle = new string(new char[1024]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = "Save File";


            if (GetSaveFileName(ref ofn))
            {
                IntPtr ptr = ofn.lpstrFile;
                string val = Marshal.PtrToStringUni(ptr);

                r = val;
            }

            Marshal.FreeBSTR(ofn.lpstrFile);

            return r;
        }

        //WARNING: Extremely Long Function
        public async Task<string[]> OpenFilePicker(DOM ctx, FilePickerOptions options)
        {
            var r = new string[0];

            var ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);

            ofn.lpstrFilter += "All Accepted Files\0";
            for (var i = 0; i < options.AllowedFileTypes.Length; i++)
            {
                var ext = options.AllowedFileTypes[i];
                if (i != 0)
                {
                    ofn.lpstrFilter += ";";
                }
                ofn.lpstrFilter += $"*.{ext}";
            }
            ofn.lpstrFilter += "\0";

            for (var i = 0; i < options.AllowedFileTypes.Length; i++)
            {
                var ext = options.AllowedFileTypes[i];
                ofn.lpstrFilter += $"{ext.ToUpper()} Files\0*.{ext}\0";
            }
            ofn.lpstrFilter += "\0";

            ofn.lpstrFile = Marshal.StringToBSTR(new string(' ', 1024 * 1024)); // 1MB
            ofn.nMaxFile = 1024 * 1024;
            ofn.lpstrFileTitle = new string(new char[1024]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = "Select File";

            if (options.AllowMultiSelection)
            {
                ofn.Flags += 0x00000200; // OFN_ALLOWMULTISELECT
                ofn.Flags += 0x00080000; // OFN_EXPLORER
                ofn.Flags += 0x00001000; // OFN_FILEMUSTEXIST
            }

            if (GetOpenFileName(ref ofn))
            {
                IntPtr ptr = ofn.lpstrFile;
                string val = Marshal.PtrToStringUni(ptr);

                //Check If It's In The Easy Format Or The Annoying Format
                if (File.Exists(val))
                {
                    //Easy To Read, Just Return The String
                    r = new string[] { val };
                }
                else
                {
                    //Annoying

                    var files = new List<string>();

                    //Read Until Null, Repeat Until Double Null
                    while (true)
                    {
                        string str = Marshal.PtrToStringUni(ptr);
                        if (str.Length == 0)
                            break;

                        if (str != val)
                        {
                            files.Add(Path.Combine(val, str)); // Directory + File Name
                        }

                        ptr = new IntPtr(ptr.ToInt64() + (str.Length + 1 /* NULL */) * sizeof(char));
                    }

                    r = files.ToArray();
                }

            }

            Marshal.FreeBSTR(ofn.lpstrFile);

            return r;
        }
    }
}
