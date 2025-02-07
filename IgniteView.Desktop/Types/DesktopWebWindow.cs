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
        static extern void InitWebWindow();

        #endregion

        public DesktopWebWindow(): base() {

            // Fixes weird bug (see https://github.com/webview/webview/issues/1043#issuecomment-2480839753)
            Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", null);

            InitWebWindow();
            Console.WriteLine("Hello, World");
        }
    }
}
