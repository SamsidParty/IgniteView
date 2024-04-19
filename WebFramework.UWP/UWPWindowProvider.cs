using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace WebFramework.UWP
{
    public class UWPWindowProvider
    {
        public static void Activate()
        {
            Application.Current.RequiresPointerMode = Windows.UI.Xaml.ApplicationRequiresPointerMode.WhenRequested;
        }
    }
}
