﻿using IgniteView.Core;
using IgniteView.Desktop;

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
                .WithBounds(new LockedWindowBounds(640, 480))
                .WithTitle("Main Window")
                .Show();

            var secondWindow =
                WebWindow.Create()
                .Show()
                .WithBounds(new WindowBounds(900, 720))
                .WithTitle("Test Window");

            app.Run();
        }
    }
}
