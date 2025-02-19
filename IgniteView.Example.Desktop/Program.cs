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

            var mainWindow =
                WebWindow.Create()
                .WithBounds(new WindowBounds(900, 720))
                .WithTitle("Main Window")
                .Show();

            app.Run();
        }
    }
}
