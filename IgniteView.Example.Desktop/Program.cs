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
            app.RegisterPreloadScriptFromFunction(() => $"console.log('{DateTimeOffset.Now.ToUnixTimeSeconds()}');");

            var mainWindow =
                WebWindow.Create()
                .WithBounds(new WindowBounds(900, 720))
                .WithTitle("Main Window")
                .WithoutTitleBar()
                .WithSharedContext("Test", "Hello, world")
                .With((w) =>
                {
                    if (w is DesktopWebWindow desktopWindow)
                    {
                        desktopWindow.AcrylicBackground = true;
                    }

                    if (w is Win32WebWindow win32Window)
                    {
                        win32Window.BackgroundMode = Win32WebWindow.WindowBackgroundMode.Mica;
                    }
                })
                .Show();

            app.Run();
        }
    }
}
