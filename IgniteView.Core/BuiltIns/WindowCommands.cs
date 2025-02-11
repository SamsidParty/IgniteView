using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class WindowCommands
    {
        [Command("igniteview_window_open")]
        public static string OpenWindow(string url)
        {
            return WebWindow
                .Create(url)
                .Show()
                .ID.ToString();
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
    }
}
