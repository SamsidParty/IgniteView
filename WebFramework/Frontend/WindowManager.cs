using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework
{
    public class WindowManager
    {
        public static WindowOptions Options;
        public static WebWindow MainWindow;

        public static List<WebWindow> OpenWindows = new List<WebWindow>();

        public static async Task<WebWindow> Create(WindowOptions op)
        {
            if (Options != null && Options._Allowed > -1)
            {
                throw new InvalidOperationException("You Can Only Create A Main Window Once");
            }
            Options = op;
            Options.Apply();

            Logger.LogInfo("Creating Main Window");

            MainWindow = AppManager.GetWebWindow();
            OpenWindows.Add(MainWindow);
            await MainWindow.Init();
            return MainWindow;
        }
    }

    public class WebWindow
    {
        public bool ReadyEventFired = false;

        public DOM Document;
        public Dictionary<string, Action<WSMessage, WebWindow>> MessageListeners = new Dictionary<string, Action<WSMessage, WebWindow>>();

        public DynamicColor BackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }
            set
            {
                //Remove Any Existing Listeners
                try
                {
                    if (_BackgroundColor != null)
                    {
                        _BackgroundColor.Changed.Remove(WebWindow_SetBackgroundColor);
                    }
                }
                catch { }

                _BackgroundColor = value;
                UpdateBackgroundColor();
            }
        }

        private DynamicColor _BackgroundColor;

        public virtual string OverrideLib(string lib)
        {
            return lib;
        }

        public void LoadLib()
        {
            Logger.LogInfo("Loading InteropLibrary.js Into Window");
            ExecuteJavascript(OverrideLib(Properties.Resources.InteropLibrary));
        }

        private void UpdateBackgroundColor()
        {
            if (_BackgroundColor != null)
            {
                WebWindow_SetBackgroundColor(_BackgroundColor);
                _BackgroundColor.Changed.Add(WebWindow_SetBackgroundColor);
            }
        }

        private void WebWindow_SetBackgroundColor(DynamicColor value)
        {
            if (Document != null && value != null)
            {
                Document.SetCSSProperty("body", "background-color", $"rgb({value.Value.R.ToString()}, {value.Value.G.ToString()}, {value.Value.B.ToString()})");
            }
        }

        public virtual async Task WindowReady()
        {
            LoadLib();
            Document = new DOM(this);
            Document.RunFunction("JSI_Ready");
            Document.RunFunction("JSI_CSSRaw", "* { user-select: none; -webkit-touch-callout: none; -webkit-user-select: none; -moz-user-select: none; }");

            //Run Load Tasks
            LoadedTasks.Clear();
            foreach (var t in Tasks)
            {
                if (!LoadedTasks.Contains(t))
                {
                    ExecuteJavascript(t);
                    LoadedTasks.Add(t);
                }
            }

            await Document.Init();

            if (!ReadyEventFired)
            {
                await AppManager.OnReady(this);
                ReadyEventFired = true;
            }

            Logger.LogInfo("Window Ready");
        }

        public virtual async Task ExecuteJavascript(string js)
        {

        }

        public virtual async Task Init()
        {
            
        }

        public virtual async Task Close()
        {

        }

        public virtual async Task UpdateTitle(string title)
        {
            WebWindow_SetBackgroundColor(_BackgroundColor);
        }

        /// <summary>
        /// Sends A UI Update To The Window
        /// </summary>
        public void Flush()
        {
            ExecuteJavascript("JSI_Send('flush')");
        }

        public List<string> Tasks = new List<string>();
        public List<string> LoadedTasks = new List<string>();
        /// <summary>
        /// Add Some JavaScript To Execute On Page Load
        /// </summary>
        public void AddJSTask(string js)
        {
            Tasks.Add(js);

            if (!LoadedTasks.Contains(js))
            {
                ExecuteJavascript(js);
                LoadedTasks.Add(js);
            }
        }
    }

    public class WindowOptions
    {

        /// <summary>
        /// Path To An Icon In PNG Format
        /// </summary>
        public string IconPath = "";

        /// <summary>
        /// Color Of The Titlebar
        /// </summary>
        public DynamicColor TitlebarColor
        {
            get
            {
                return TBC;
            }
            set
            {
                Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "FF" + value.HexValue);
                TBC = value;
                EditTitlebarColor();
            }
        }
        DynamicColor TBC = Color.White;
        public int _WinTBC = -1; // Windows Wants It In A Single int
        public int _Allowed = -1;

        /// <summary>
        /// Enables Gamepads And Disables Mouse Mode On UWP
        /// </summary>
        public bool NativeGamepadSupport;

        public Rectangle StartWidthHeight = new Rectangle(0, 0, 1280, 720);

        /// <summary>
        /// Only Works On Some Platforms
        /// </summary>
        public bool LockWidthHeight = false;

        void EditTitlebarColor()
        {
            TBC.Changed.Add((c) =>
            {
                ApplyTitlebarColor();
            });
            ApplyTitlebarColor();
        }

        void ApplyTitlebarColor()
        {
            var ch = $"{TBC.Value.R:X2}{TBC.Value.G:X2}{TBC.Value.B:X2}";
            string R = ch.Substring(0, 2);
            string G = ch.Substring(2, 2);
            string B = ch.Substring(4, 2);
            _WinTBC = int.Parse(B + G + R, System.Globalization.NumberStyles.HexNumber);

            foreach (var win in WindowManager.OpenWindows)
            {
                if (win != null)
                {
                    win.Flush();
                }
            }

        }

        public void Apply()
        {
            ApplyTitlebarColor();

            if (IconPath != "" && !File.Exists(IconPath)){
                throw new FileNotFoundException(IconPath);
            }
            else {
                TryIcon("favicon.png");
                TryIcon("Favicon.png");
            }
        }

        public void TryIcon(string i){
            if (File.Exists(Path.Combine(AppManager.Location, i))){
                IconPath = Path.Combine(AppManager.Location, i);
            }
        }
    }
}