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

        public enum WindowBackgroundMode
        {
            Disabled = 0,
            Mica = 2,
            Acrylic = 3,
            DarkMica = 4
        }


        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the system is running windows 11 or later
        /// </summary>
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

        /// <summary>
        /// Returns true if the system is in dark mode
        /// </summary>
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

        /// <summary>
        /// Experimental; Native win32 window background mode
        /// </summary>
        public WindowBackgroundMode BackgroundMode = WindowBackgroundMode.Mica;

        #endregion

        void EnableMica(IntPtr hwnd)
        {
            if (!IsWindows11) { return; }

            int enable = (int)BackgroundMode;
            DwmSetWindowAttribute(hwnd, 38, ref enable, Marshal.SizeOf(typeof(int)));
        }

        /// <summary>
        /// Checks the system dark mode state and applies it to the window
        /// </summary>
        void UpdateDarkModeState() => SetWebWindowDark(WindowIndex, IsDarkMode);

        /// <summary>
        /// Constantly checks if the dark mode state has changed and updates accordingly.
        /// TODO: Find a better method to detect when dark mode state changes
        /// </summary>
        async Task DarkModeCheckLoop()
        {
            var lastDarkValue = IsDarkMode;
            while (true)
            {
                await Task.Delay(1000);

                if (lastDarkValue != IsDarkMode) { 
                    lastDarkValue = IsDarkMode;
                    UpdateDarkModeState();
                }
            }
        }

        public override WebWindow Show()
        {
            UpdateDarkModeState();
            Task.Run(DarkModeCheckLoop);
            base.Show();
            EnableMica(NativeHandle);
            return this;
        }
    }
}
