using IgniteView.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Desktop
{
    public class DesktopWebWindow : WebWindow
    {
        #region Native Imports

        [DllImport("IgniteView.Desktop.Native")]
        static extern void NewWebWindow();

        #endregion

        public DesktopWebWindow(): base() {
            NewWebWindow();
        }
    }
}
