using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        #region Window Titlebar

        /// <summary>
        /// Sets whether the window titlebar is visible
        /// </summary>
        protected virtual bool TitleBarVisible { get; set; }

        /// <summary>
        /// Enables or disables the titlebar on the window
        /// </summary>
        public virtual WebWindow WithTitleBar(bool visible)
        {
            TitleBarVisible = visible;
            return this;
        }

        /// <summary>
        /// Enables the titlebar on the window
        /// </summary>
        public virtual WebWindow WithTitleBar() => WithTitleBar(true);

        /// <summary>
        /// Disables the titlebar on the window
        /// </summary>
        public virtual WebWindow WithoutTitleBar() => WithTitleBar(false);

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
        /// Hides or suspends the WebWindow
        /// </summary>
        public virtual void Hide() { }

        /// <summary>
        /// Executes raw JavaScript code in the root page of the window
        /// </summary>
        public virtual void ExecuteJavaScript(string scriptData) { }

        /// <summary>
        /// Executes a JSFunction in the root page of the window
        /// </summary>
        public virtual void ExecuteJavaScript(JSFunctionCall functionToExecute) => ExecuteJavaScript(functionToExecute.ToString());

        /// <summary>
        /// Calls a function on the window
        /// </summary>
        /// <param name="functionName">The name of the JavaScript function to call, make sure this is available in the global scope</param>
        /// <param name="parameters">The parameters passed to the JavaScript function</param>
        public virtual void CallFunction(string functionName, params object[] parameters) => ExecuteJavaScript(new JSFunctionCall(functionName, parameters));

        protected virtual void ExecuteCommand(CommandData commandData) => CommandManager.ExecuteCommand(this, commandData);

        /// <summary>
        /// Invokes the function provided with this WebWindow as a parameter,
        /// Useful for running extra code while chaining methods
        /// </summary>
        public virtual WebWindow With(Action<WebWindow> mutatorFunction)
        {
            mutatorFunction.Invoke(this);
            return this;
        }

        #endregion

        #region Constructors

        public static WebWindow Create() => BeforeCreate().CreateWebWindow().AfterCreate();
        public static WebWindow Create(string url) => Create().WithURL(url);

        /// <summary>
        /// Only inherited classes should call this constructor
        /// </summary>
        protected WebWindow() 
        {
            if (CurrentAppManager.MainWindow == null)
            {
                CurrentAppManager.MainWindow = this;
            }

            CurrentAppManager.OpenWindows.Add(this);

            // Create an ID for this window
            AppManager.LastWindowID++;
            ID = AppManager.LastWindowID;
        }

        private static PlatformManager BeforeCreate()
        {
            if (AppManager.Instance.MainWindow == null)
            {
                AppManager.Instance.OnBeforeMainWindowCreated?.Invoke();
            }

            return PlatformManager.Instance;
        }

        private WebWindow AfterCreate()
        {
            // Try to use the default favicon if it exists
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && CurrentAppManager.CurrentServerManager.Resolver.DoesFileExist("/favicon_mac.png"))
            {
                IconPath = "/favicon_mac.png"; // Macs use a different icon with a background and rounded corners
            }
            else if (CurrentAppManager.CurrentServerManager.Resolver.DoesFileExist("/favicon.png"))
            {
                IconPath = "/favicon.png";
            }

            CurrentAppManager.OnMainWindowCreated?.Invoke(this);

            return this;
        }

        #endregion
    }
}
