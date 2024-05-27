using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework
{
    public class WindowManager
    {
        public static WindowOptions Options;
        public static WebWindow MainWindow;

        public static List<WebWindow> OpenWindows = new List<WebWindow>();

        public static async Task<WebWindow> Create(WindowOptions op)
        {
            if (Options != null && Options._Allowed > -1)
            {
                throw new InvalidOperationException("You Can Only Create A Main Window Once");
            }
            Options = op;
            Options.Apply();

            Logger.LogInfo("Creating Main Window");

            MainWindow = AppManager.GetWebWindow();
            OpenWindows.Add(MainWindow);
            await MainWindow.Init();
            return MainWindow;
        }

        public static WebWindow GetWindowByID(string windowID)
        {
            foreach (WebWindow window in OpenWindows)
            {
                if (window.ID == windowID) { return window; }
            }

            return null;
        }
    }
}