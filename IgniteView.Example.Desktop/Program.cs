using IgniteView.Core;
using IgniteView.Desktop;
using System.Runtime.InteropServices;

namespace IgniteView.Example.Desktop
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            DesktopPlatformManager.Activate();
            var app = new ViteAppManager();

            app.RegisterDynamicFileRoute("/hello.txt", RoutingUtilities.TextRoute("Hello from IgniteView!"));

            app.RegisterPreloadScriptFromPath("/preload.js");

            var mainWindow =
                WebWindow.Create()
                .WithBounds(new LockedWindowBounds(900, 720))
                .WithTitle("Main Window")
                .WithoutTitleBar()
                .With((w) =>
                {
                    // Cool acrylic effect on Windows 11
                    if (w.GetType() == typeof(Win32WebWindow))
                    {
                        ((Win32WebWindow)w).BackgroundMode = Win32WebWindow.WindowBackgroundMode.BlurBehind;
                    }
                })
                .Show();

            app.Run();
        }
    }
}
