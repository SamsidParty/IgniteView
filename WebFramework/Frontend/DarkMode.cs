using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public class DarkMode
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        static extern bool ShouldSystemUseDarkMode();


        public static bool GetIsEnabled() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return ShouldSystemUseDarkMode();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                return MacHelper.IsDark();
            }
            else
            {
                //Linux users normally use dark mode
                return true;
            }

        }
    }

    /// <summary>
    /// Provides A Different Color Based On Whether Dark Mode Is Enabled
    /// </summary>
    public class ThemeBasedColor : DynamicColor
    {
        Color light;
        Color dark;

        public ThemeBasedColor(Color lightModeColor, Color darkModeColor)
        {
            light = lightModeColor;
            dark = darkModeColor;
            new Thread(ActivationThread).Start();
        }

        /// <summary>
        /// Wait Until The Main Window Is Ready, And Attach Itself To It
        /// </summary>
        void ActivationThread()
        {
            while (WindowManager.MainWindow == null || !WindowManager.MainWindow.ReadyEventFired)
            {
                Thread.Sleep(25);
            }
            Activate(WindowManager.MainWindow);
        }

        /// <summary>
        /// Start Listening For Theme Changes (Doesn't Work On macOS)
        /// </summary>
        void Activate(WebWindow window)
        {
            window.AddJSTask("window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', event => { JSI_Send('themechanged', event.matches ? 'dark' : 'light') });");
            window.MessageListeners.Add("themechanged", async (p, s) =>
            {
                await Task.Delay(500);
                UpdateValue(p.Param1 == "dark" ? dark : light);
            });
        }

        public override void OnBeforeValueRequested()
        {
            Internal = DarkMode.GetIsEnabled() ? dark : light;
        }
    }
}
