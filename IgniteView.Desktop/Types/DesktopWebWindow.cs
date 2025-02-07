using IgniteView.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Desktop
{
    public class DesktopWebWindow : WebWindow
    {
        public DesktopWebWindow(): base() {
            Console.WriteLine("Hello, World");
        }
    }
}
