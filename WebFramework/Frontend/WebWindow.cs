using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework
{
    public class WebWindow
    {
        public bool ReadyEventFired = false;

        public DOM Document;
        public Dictionary<string, Action<WSMessage, WebWindow>> MessageListeners = new Dictionary<string, Action<WSMessage, WebWindow>>();

        public List<WebScript> AttachedScripts = new List<WebScript>();

        public WindowOptions Options;

        /// <summary>
        /// Unique Identifier Used To Identify This Window With JS Interop
        /// </summary>
        public string ID = Guid.NewGuid().ToString();

        public WebWindow(WindowOptions options)
        {
            Options = options;
        }

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
            ExecuteJavascript(OverrideLib(InteropLibrary.Content));
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
}
