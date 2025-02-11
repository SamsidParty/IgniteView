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
                .NativeHandle.ToString();
        }

        [Command("igniteview_window_close")]
        public static void CloseWindow(WebWindow window, int windowId)
        {
            Console.WriteLine("test");
        }
    }
}
