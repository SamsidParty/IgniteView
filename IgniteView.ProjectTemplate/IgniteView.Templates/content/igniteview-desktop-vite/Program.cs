﻿using IgniteView.Core;
using IgniteView.Desktop;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        DesktopPlatformManager.Activate();
        var app = new ViteAppManager();

        var mainWindow =
            WebWindow.Create()
            .WithTitle("My IgniteView App")
            .Show();

        app.Run();
    }
}