using IgniteView.Core;
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

            app.CurrentScriptManager.RegisterPreloadScriptFromPath("/preload.js");

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

            app.Run();
        }
    }
}
