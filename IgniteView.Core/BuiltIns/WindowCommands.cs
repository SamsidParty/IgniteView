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
                    if (PlatformManager.HasPlatformHint("windows"))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    else if (PlatformManager.HasPlatformHint("linux"))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (PlatformManager.HasPlatformHint("macos"))
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

        [Command("igniteview_window_toggle_maximize")]
        public static void ToggleMaximize(WebWindow window)
        {
            window.WithMaximized(!window.IsMaximized);
        }

        [Command("igniteview_window_enter_fullscreen")]
        public static void EnterFullscreen(WebWindow window)
        {
            window.EnterFullscreen();
        }

        [Command("igniteview_window_exit_fullscreen")]
        public static void ExitFullscreen(WebWindow window)
        {
            window.ExitFullscreen();
        }

        [Command("igniteview_window_toggle_fullscreen")]
        public static void ToggleFullscreen(WebWindow window)
        {
            window.ToggleFullscreen();
        }

        [Command("igniteview_window_is_fullscreen")]
        public static bool IsFullscreen(WebWindow window)
        {
            return window.IsFullscreen;
        }
    }
}
