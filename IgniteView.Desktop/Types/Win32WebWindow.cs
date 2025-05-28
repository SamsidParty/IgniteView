using IgniteView.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IgniteView.Desktop
{
    public class Win32WebWindow : DesktopWebWindow
    {

        #region Native Imports

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, int[] pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        [DllImport("user32.dll")]
        public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);


        [DllImport("dwmapi.dll")]
        public static extern void DwmGetColorizationColor(ref uint pcrColorization, ref bool pfOpaqueBlend);

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        public enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        public enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);

        public static bool RefreshWindow(IntPtr hWnd) => SetWindowPos(hWnd, 0, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0004 | 0x0010 | 0x0020);

        public enum WindowBackgroundMode
        {
            Disabled = 0,
            Mica = 2,
            Acrylic = 3,
            DarkMica = 4,
            BlurBehind = 5,
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
        public static bool IsDarkMode
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

            int enable = (int)BackgroundMode < 5 ? (int)BackgroundMode : 0;
            DwmSetWindowAttribute(hwnd, 38, ref enable, Marshal.SizeOf(typeof(int)));

            if (BackgroundMode == WindowBackgroundMode.BlurBehind)
            {
                var accent = new AccentPolicy();
                var accentStructSize = Marshal.SizeOf(accent);
                accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

                var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new WindowCompositionAttributeData();
                data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
                data.SizeOfData = accentStructSize;
                data.Data = accentPtr;

                SetWindowCompositionAttribute(hwnd, ref data);

                Marshal.FreeHGlobal(accentPtr);
            }
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
        bool DarkModeLoopRunning = false;

        void ApplyAccentColors()
        {
            var ParseColor = (uint color, bool opaque) => Color.FromArgb(
                (byte)(opaque ? 255 : color >> 24),
                (byte)(color >> 16),
                (byte)(color >> 8),
                (byte)color
            );

            try
            {
                uint accentVal = 0;
                bool opaqueBlend = false;
                DwmGetColorizationColor(ref accentVal, ref opaqueBlend);

                var accent = ParseColor(accentVal, opaqueBlend);

                SystemStyling.GlobalStyles.Add(new StyleRule("--system-accent", $"rgb({accent.R}, {accent.G}, {accent.B}) !important"));
            }
            catch { }
        }

        public override WebWindow Show()
        {
            if (this == AppManager.Instance.MainWindow)
            {
                // Only has to be done once for all windows
                ApplyAccentColors();
            }

            UpdateDarkModeState();

            if (!DarkModeLoopRunning)
            {
                DarkModeLoopRunning = true;
                Task.Run(DarkModeCheckLoop);
            }

            base.Show();
            EnableMica(NativeHandle);
            return this;
        }
    }
}
