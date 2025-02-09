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

        [DllImport("IgniteView.Desktop.Native", CharSet = CharSet.Ansi)]
        static extern void NewWebWindow(string url);

        #endregion

        public override WebWindow Show()
        {
            NewWebWindow(CurrentAppManager.CurrentServerManager.BaseURL);
            return base.Show();
        }

        public DesktopWebWindow() : base() { }
    }
}
