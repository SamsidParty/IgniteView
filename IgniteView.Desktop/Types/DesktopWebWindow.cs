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
        /// <summary>
        /// Represents the internal index of the window in the C++ code
        /// </summary>
        protected int WindowIndex;

        #region Native Imports

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        protected static extern int NewWebWindow(string url, CommandBridgeCallback commandBridge, string preloadScript, IntPtr path);

        [DllImport(InteropHelper.DLLName)]
        protected static extern void ShowWebWindow(int index);

        [DllImport(InteropHelper.DLLName)]
        protected static extern void CloseWebWindow(int index);

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        protected static extern void ExecuteJavaScriptOnWebWindow(int index, string jsCode);

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        protected static extern int SetWebWindowTitle(int index, string newTitle);

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Ansi)]
        protected static extern int SetWebWindowURL(int index, string newURL);

        [DllImport(InteropHelper.DLLName)]
        protected static extern int SetWebWindowIcon(int index, IntPtr iconPath);

        [DllImport(InteropHelper.DLLName)]
        protected static extern void SetWebWindowDark(int index, bool isDark);

        [DllImport(InteropHelper.DLLName)]
        protected static extern void SetWebWindowDevToolsEnabled(int index, bool devToolsEnabled);

        [DllImport(InteropHelper.DLLName)]
        protected static extern void SetWebWindowBounds(int index, int w, int h, int minW, int minH, int maxW, int maxH);

        [DllImport(InteropHelper.DLLName)]
        protected static extern IntPtr GetWebWindowTitle(int index);

        [DllImport(InteropHelper.DLLName)]
        protected static extern IntPtr GetWebWindowHandle(int index);

        #endregion

        #region Properties

        public override string Title { get => InteropHelper.PointerToStringAnsi(GetWebWindowTitle(WindowIndex)); set => SetWebWindowTitle(WindowIndex, value); }
        public override string IconPath { get => base.IconPath; set { base.IconPath = value; SetWebWindowIcon(WindowIndex, Marshal.StringToCoTaskMemUTF8(IconManager.CloneIcon(base.IconPath))); } }
        public override string URL { get => base.URL; set { base.URL = value; SetWebWindowURL(WindowIndex, base.URL); } }
        public override IntPtr NativeHandle { get => GetWebWindowHandle(WindowIndex); }

        public override WindowBounds Bounds { 
            get => base.Bounds;
            set {
                var applied = value.AppliedBounds();
                SetWebWindowBounds(WindowIndex, applied.InitialWidth, applied.InitialHeight, applied.MinWidth, applied.MinHeight, applied.MaxWidth, applied.MaxHeight);
                base.Bounds = value;
            } 
        }

        #endregion

        #region Command Bridge

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CommandBridgeCallback(IntPtr param);

        public CommandBridgeCallback CommandExecuteRequested;
        void OnCommandExecuteRequested(IntPtr param)
        {
            var commandString = InteropHelper.PointerToStringBase64(param);
            ExecuteCommand(commandString);
        }

        #endregion

        #region Virtual Method Overrides

        public override WebWindow Show()
        {
            base.Show();
            ShowWebWindow(WindowIndex);
            return this;
        }

        public override void Close()
        {
            base.Close();
            CloseWebWindow(WindowIndex);
        }

        public override void ExecuteJavaScript(string scriptData)
        {
            ExecuteJavaScriptOnWebWindow(WindowIndex, JavaScriptConverter.WrapCode(scriptData));
            base.ExecuteJavaScript(scriptData);
        }

        #endregion

        public DesktopWebWindow() : base() {
            CommandExecuteRequested = new CommandBridgeCallback(OnCommandExecuteRequested);
            var appPath = Marshal.StringToCoTaskMemUTF8(Path.Join(CurrentAppManager.CurrentIdentity.AppDataPath, "DesktopNative"));
            WindowIndex = NewWebWindow(URL, CommandExecuteRequested, InjectedScript.CombinedScriptData, appPath);

            // Enable dev tools if debug mode
            SetWebWindowDevToolsEnabled(WindowIndex, DebugMode.IsDebugMode);
        }
    }
}
