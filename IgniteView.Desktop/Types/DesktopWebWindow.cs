using IgniteView.Core;
using IgniteView.Desktop.Types;
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

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        static extern int NewWebWindow(string url);

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        static extern void ShowWebWindow(int index);

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        static extern int SetWebWindowTitle(int index, string newTitle);

        [DllImport(InteropHelper.DLLName)]
        static extern IntPtr GetWebWindowTitle(int index);

        [DllImport(InteropHelper.DLLName)]
        static extern IntPtr GetWebWindowHandle(int index);

        #endregion

        #region Platform Helpers

        Win32Helper CurrentWin32Helper;

        void LoadHelpers()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CurrentWin32Helper = new Win32Helper();
            }
        }

        #endregion

        /// <summary>
        /// Represents the internal index of the window in the C++ code
        /// </summary>
        int WindowIndex;

        public override string Title { get => InteropHelper.PointerToString(GetWebWindowTitle(WindowIndex)); set => SetWebWindowTitle(WindowIndex, value); }
        public override IntPtr NativeHandle { get => GetWebWindowHandle(WindowIndex); }

        public override WebWindow Show()
        {
            ShowWebWindow(WindowIndex);
            CurrentWin32Helper?.EnableMica(NativeHandle);
            return base.Show();
        }

        public DesktopWebWindow() : base() { 
            LoadHelpers();
            WindowIndex = NewWebWindow(CurrentAppManager.CurrentServerManager.BaseURL);
        }
    }
}
