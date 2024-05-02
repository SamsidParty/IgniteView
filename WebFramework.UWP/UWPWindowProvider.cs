using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;
using Windows.UI.Xaml;

namespace WebFramework.UWP
{
    public class UWPWindowProvider
    {
        public static void Activate()
        {
            Logger.LogInfo("Activating UWP Window Provider");
            UWPHelperLoader.Current = Activator.CreateInstance(typeof(UWPHelper));
            UWPHelperLoader.Current.OnLoad();
            AppManager.WindowToUse = typeof(UWPWebWindow);
            Application.Current.RequiresPointerMode = Windows.UI.Xaml.ApplicationRequiresPointerMode.WhenRequested;
        }
    }
}
