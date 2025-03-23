using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class WindowCommands
    {
        [Command("igniteview_window_open")]
        public static string OpenWindow(string url)
        {
            if (!url.Contains("://"))
            {
                // Open a new IgniteView window
                return WebWindow
                    .Create(url)
                    .Show()
                    .ID.ToString();
            }
            else
            {
                // Open a new browser window
                // https://stackoverflow.com/a/43232486

                try
                {
                    Process.Start(url);
                }
                catch
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                }
            }

            return "";
        }

        [Command("igniteview_window_close")]
        public static void CloseWindow(WebWindow window, int windowId)
        {
            if (windowId == -1) {
                window.Close(); // Close the calling window
                return;
            }

            // Close the window with id windowId
            var windowToClose = window.CurrentAppManager.OpenWindows.Where((w) => w.ID == windowId);
            windowToClose.FirstOrDefault(window).Close();
        }

        [Command("igniteview_window_hide")]
        public static void HideWindow(WebWindow window, int windowId)
        {
            if (windowId == -1)
            {
                window.Hide(); // Hide the calling window
                return;
            }

            // Hide the window with id windowId
            var windowToHide = window.CurrentAppManager.OpenWindows.Where((w) => w.ID == windowId);
            windowToHide.FirstOrDefault(window).Hide();
        }
    }
}
