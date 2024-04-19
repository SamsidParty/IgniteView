using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;
using Windows.UI.Xaml;

namespace WebFramework.UWP
{
    public class UWPHelper
    {
        public bool IsDark()
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Dark;
        }

        public void OnLoad()
        {
            Logger.LogInfo("Loaded UWP Helper");
        }
    }
}
