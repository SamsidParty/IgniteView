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
            var createdWindow = AppManager.GetWebWindow(op);

            if (MainWindow == null)
            {
                if (op != null)
                {
                    Options = op;
                    Options.Apply();
                }

                Logger.LogInfo("Creating Main Window");
                MainWindow = createdWindow;
            }

            OpenWindows.Add(createdWindow);
            await createdWindow.Init();

            return createdWindow;
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