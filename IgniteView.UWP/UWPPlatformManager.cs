using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IgniteView.Core;
using IgniteView.Core.Types;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace IgniteView.UWP
{
    public class UWPPlatformManager : PlatformManager
    {

        public static Type Activate()
        {
            Instance = new UWPPlatformManager();
            Instance.Storage = new UWPPersistentStorage();
            PlatformManager.PlatformHints.Add("uwp");

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

        public override ServerListenMode GetServerListenMode() => ServerListenMode.Tcp;

        public override void Run() { }
    }
}
