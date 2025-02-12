using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class WebWindow
    {
        public AppManager CurrentAppManager
        {
            get
            {
                return AppManager.Instance;
            }
        }

        #region Window Bounds

        public virtual WindowBounds Bounds { get; set; }

        /// <summary>
        /// Sets the window bounds of this WebWindow
        /// </summary>
        public WebWindow WithBounds(WindowBounds bounds)
        {
            Bounds = bounds;
            return this;
        }

        #endregion

        #region Window Title

        /// <summary>
        /// Gets or sets the title of this WebWindow
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Sets the title of this WebWindow
        /// </summary>
        public virtual WebWindow WithTitle(string newTitle)
        {
            Title = newTitle;
            return this;
        }

        #endregion

        #region Window Handle

        /// <summary>
        /// Gets the handle of the window, on supported platforms this will return the native handle of the window
        /// </summary>
        public virtual IntPtr NativeHandle { get; }

        #endregion

        #region Window ID

        /// <summary>
        /// Gets the unique ID for this window
        /// </summary>
        public int ID;

        #endregion

        #region Window URL

        /// <summary>
        /// Gets the relative URL of this WebWindow
        /// </summary>
        public virtual string URL
        {
            get
            {
                if (_URL == null)
                {
                    return CurrentAppManager.CurrentServerManager.BaseURL;
                }

                if (_URL.StartsWith("http")) // Avoid prepending the base URL to full URLs
                {
                    return _URL;
                }

                return CurrentAppManager.CurrentServerManager.BaseURL + "/" + _URL;
            }
            set
            {
                if (value.StartsWith("/"))
                {
                    value = value.Substring(1);
                }

                _URL = value;
            }
        }

        private string _URL = null;

        /// <summary>
        /// Sets the relative URL of this WebWindow
        /// </summary>
        public virtual WebWindow WithURL(string newURL)
        {
            URL = newURL;
            return this;
        }

        #endregion

        #region Window Icon

        /// <summary>
        /// Gets or sets the relative icon path for this window. This path must be relative to the www root (with a leading /)
        /// </summary>
        public virtual string IconPath { get; set; }

        /// <summary>
        /// Sets the icon of this window. This path must be relative to the www root (with a leading /)
        /// </summary>
        public virtual WebWindow WithIcon(string newIconPath)
        {
            IconPath = newIconPath;
            return this;
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Call this after configuring the window to display it on screen
        /// </summary>
        public virtual WebWindow Show()
        {
            if (Bounds == null) { 
                Bounds = new WindowBounds();
            }

            return this;
        }

        /// <summary>
        /// Closes the WebWindow, once you call this method you should remove all references to this WebWindow
        /// </summary>
        public virtual void Close() => CurrentAppManager.OpenWindows.Remove(this);

        /// <summary>
        /// Executes raw JavaScript code in the root page of the window
        /// </summary>
        public virtual void ExecuteJavaScript(string scriptData) { }

        /// <summary>
        /// Executes a JSFunction in the root page of the window
        /// </summary>
        public virtual void ExecuteJavaScript(JSFunction functionToExecute) => ExecuteJavaScript(functionToExecute.ToString());

        public virtual void ExecuteCommand(CommandData commandData) => CommandManager.ExecuteCommand(this, commandData);

        #endregion

        #region Constructors

        public static WebWindow Create() => PlatformManager.Instance.CreateWebWindow().AfterCreate();
        public static WebWindow Create(string url) => Create().WithURL(url);

        /// <summary>
        /// Only inherited classes should call this constructor
        /// </summary>
        protected WebWindow() 
        { 
            CurrentAppManager.OpenWindows.Add(this);

            // Create an ID for this window
            AppManager.LastWindowID++;
            ID = AppManager.LastWindowID;
        }

        private WebWindow AfterCreate()
        {
            // Try to use the default favicon.ico if it exists
            if (CurrentAppManager.CurrentServerManager.Resolver.DoesFileExist("/favicon.ico"))
            {
                IconPath = "/favicon.ico";
            }

            return this;
        }

        #endregion
    }
}
