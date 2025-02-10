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
        /// <summary>
        /// Represents the internal index of the window in the C++ code
        /// </summary>
        protected int WindowIndex;

        #region Native Imports

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        protected static extern int NewWebWindow(string url);

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        protected static extern void ShowWebWindow(int index);

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        protected static extern int SetWebWindowTitle(int index, string newTitle);

        [DllImport(InteropHelper.DLLName)]
        protected static extern void SetWebWindowDark(int index, bool isDark);        
        
        [DllImport(InteropHelper.DLLName)]
        protected static extern void SetWebWindowBounds(int index, int w, int h, int minW, int minH, int maxW, int maxH);

        [DllImport(InteropHelper.DLLName)]
        protected static extern IntPtr GetWebWindowTitle(int index);

        [DllImport(InteropHelper.DLLName)]
        protected static extern IntPtr GetWebWindowHandle(int index);

        #endregion

        #region Properties

        public override string Title { get => InteropHelper.PointerToString(GetWebWindowTitle(WindowIndex)); set => SetWebWindowTitle(WindowIndex, value); }
        public override IntPtr NativeHandle { get => GetWebWindowHandle(WindowIndex); }

        public override WindowBounds Bounds { 
            get => base.Bounds;
            set {
                SetWebWindowBounds(WindowIndex, value.InitialWidth, value.InitialHeight, value.MinWidth, value.MinHeight, value.MaxWidth, value.MaxHeight);
                base.Bounds = value;
            } 
        }

        #endregion

        public override WebWindow Show()
        {
            base.Show();
            ShowWebWindow(WindowIndex);
            return this;
        }

        public DesktopWebWindow() : base() { 
            WindowIndex = NewWebWindow(CurrentAppManager.CurrentServerManager.BaseURL);
        }
    }
}
