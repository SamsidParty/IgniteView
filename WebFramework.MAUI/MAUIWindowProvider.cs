using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.MAUI
{
    public class MAUIWindowProvider
    {
        public static MauiAppBuilder Activate(MauiAppBuilder builder)
        {
            MAUIHelperLoader.Current = Activator.CreateInstance(typeof(MAUIHelper));
            MAUIHelperLoader.Current.OnLoad();
            AppManager.WindowToUse = typeof(MAUIWebWindow);
            return builder;
        }
    }
}
