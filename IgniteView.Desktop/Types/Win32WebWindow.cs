using IgniteView.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Desktop.Types
{
    public class Win32WebWindow : DesktopWebWindow
    {

        #region Native Imports

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, int[] pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        #endregion

        #region Properties

        public bool IsWindows11
        {
            get
            {
                var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

                var currentBuildStr = (string)reg.GetValue("CurrentBuild");
                var currentBuild = int.Parse(currentBuildStr);

                return currentBuild >= 22000;
            }
        }

        public bool IsDarkMode
        {
            get
            {
                int isDark = 1;
                try
                {
                    isDark = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1);
                }
                catch { }

                return isDark == 0;
            }
        }

        #endregion

        void EnableMica(IntPtr hwnd)
        {
            if (!IsWindows11) { return; }

            int enable = 0x02;
            DwmSetWindowAttribute(hwnd, 38, ref enable, Marshal.SizeOf(typeof(int)));
        }

        /// <summary>
        /// Checks the system dark mode state and applies it to the window
        /// </summary>
        void UpdateDarkModeState() => SetWebWindowDark(WindowIndex, IsDarkMode);

        public override WebWindow Show()
        {
            UpdateDarkModeState();
            base.Show();
            EnableMica(NativeHandle);
            return this;
        }
    }
}
