﻿using IgniteView.Core;
using IgniteView.Desktop;
using IgniteView.Desktop.Types;

namespace IgniteView.Example.Desktop
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            DesktopPlatformManager.Activate();
            var app = new ViteAppManager();

            var mainWindow =
                WebWindow.Create()
                .WithBounds(new WindowBounds(900, 720))
                .WithTitle("Main Window")
                .With((w) =>
                {
                    // Cool acrylic effect on Windows 11
                    if (w.GetType() == typeof(Win32WebWindow))
                    {
                        ((Win32WebWindow)w).BackgroundMode = Win32WebWindow.WindowBackgroundMode.Acrylic;
                    }
                })
                .Show();

            var bounds = new WindowBounds(900, 720);
            bounds.MinWidth = 400;
            bounds.MinHeight = 400;
            bounds.MaxWidth = 0;
            bounds.MaxHeight = 0;

            mainWindow.Bounds = bounds;

            app.Run();
        }
    }
}
