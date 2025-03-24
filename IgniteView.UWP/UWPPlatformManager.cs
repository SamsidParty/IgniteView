using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IgniteView.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace IgniteView.UWP
{
    public class UWPPlatformManager : PlatformManager
    {

        public static Type Activate()
        {
            Instance = new UWPPlatformManager();
            var app = new UWPAppManager(new AppIdentity(Windows.ApplicationModel.Package.Current.PublisherDisplayName, Windows.ApplicationModel.Package.Current.DisplayName));
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            return typeof(IgniteViewPage);
        }

        public override void Create() {
            var mainWindow =
                WebWindow.Create()
                .Show();
        }

        public override WebWindow CreateWebWindow()
        {
            if (AppManager.Instance.MainWindow == null)
            {
                return new UWPWebWindow();
            }

            throw new Exception("Only one window is permitted on UWP");
        }

        public override ScriptInjectionMode GetScriptInjectionMode() => ScriptInjectionMode.ServerSide;
        public override ServerListenMode GetServerListenMode() => ServerListenMode.Tcp;

        public override void Run() { }
    }
}
