using IgniteView.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Desktop
{
    public class DesktopPlatformManager : PlatformManager
    {
        public static void Activate()
        {
            Instance = new DesktopPlatformManager();
        }

        public override WebWindow CreateWebWindow() => new DesktopWebWindow();
    }
}
