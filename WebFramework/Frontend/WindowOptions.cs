using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace WebFramework
{
    public class WindowOptions
    {

        /// <summary>
        /// Path To An Icon In PNG Format
        /// </summary>
        public string IconPath = "";

        /// <summary>
        /// Color Of The Titlebar
        /// </summary>
        public DynamicColor TitlebarColor
        {
            get
            {
                return TBC;
            }
            set
            {
                if (!EnableAcrylic)
                {
                    Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "FF" + value.HexValue);
                }
                TBC = value;
                EditTitlebarColor();
            }
        }
        DynamicColor TBC = Color.White;
        public int _WinTBC = -1; // Windows Wants It In A Single int
        public int _Allowed = -1;

        /// <summary>
        /// Enables A Transparency Effect On The Window
        /// On Windows 11, The Mica Effect Will Be Used
        /// </summary>
        public bool EnableAcrylic = false;

        /// <summary>
        /// Enables Gamepads And Disables Mouse Mode On UWP
        /// </summary>
        public bool NativeGamepadSupport;


        /// <summary>
        /// Only Works On Some Platforms
        /// </summary>
        public Rectangle StartWidthHeight = new Rectangle(0, 0, 1280, 720);

        /// <summary>
        /// Only Works On Some Platforms
        /// </summary>
        public bool LockWidthHeight = false;

        void EditTitlebarColor()
        {
            TBC.Changed.Add((c) =>
            {
                ApplyTitlebarColor();
            });
            ApplyTitlebarColor();
        }

        void ApplyTitlebarColor()
        {
            var ch = $"{TBC.Value.R:X2}{TBC.Value.G:X2}{TBC.Value.B:X2}";
            string R = ch.Substring(0, 2);
            string G = ch.Substring(2, 2);
            string B = ch.Substring(4, 2);
            _WinTBC = int.Parse(B + G + R, System.Globalization.NumberStyles.HexNumber);

            foreach (var win in WindowManager.OpenWindows)
            {
                if (win != null)
                {
                    win.Flush();
                }
            }

        }

        public void Apply()
        {
            ApplyTitlebarColor();

            if (IconPath != "" && !File.Exists(IconPath))
            {
                throw new FileNotFoundException(IconPath);
            }
            else
            {
                TryIcon("favicon.png");
                TryIcon("Favicon.png");
            }
        }

        public void TryIcon(string i)
        {
            if (File.Exists(Path.Combine(AppManager.Location, i)))
            {
                IconPath = Path.Combine(AppManager.Location, i);
            }
        }
    }
}
