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
            var app = new AppManager(new AppIdentity("SamsidParty", "IgniteView Example"));

            var mainWindow = WebWindow.Create();

            app.Run();
        }
    }
}
