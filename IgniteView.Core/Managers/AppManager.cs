using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver.Core;

namespace IgniteView.Core
{
    /// <summary>
    /// Manages the lifecycle of the IgniteView application
    /// </summary>
    public class AppManager
    {
        public static AppManager Instance;

        public AppIdentity CurrentIdentity;
        public ServerManager CurrentServerManager;
        public ScriptManager CurrentScriptManager;

        public List<WebWindow> OpenWindows = new();
        public WebWindow? MainWindow;

        public static int LastWindowID = 0;

        public delegate void LifecycleCallback();
        public delegate void WindowCallback(WebWindow window);

        /// <summary>
        /// This event is called after all the windows are closed and the application is about to exit.
        /// There are some situations where this is not called (crash, task manager, etc)
        /// </summary>
        public LifecycleCallback? OnCleanUp;

        /// <summary>
        /// This event is called just before the main window has been created.
        /// </summary>
        public LifecycleCallback? OnBeforeMainWindowCreated;

        /// <summary>
        /// This event is called just after the main window has been created.
        /// </summary>
        public WindowCallback? OnMainWindowCreated;

        /// <summary>
        /// Creates the file resolver to be used by the server
        /// </summary>
        /// <returns></returns>
        protected virtual FileResolver CreateFileResolver()
        {
            return new DirectoryFileResolver();
        }

        /// <summary>
        /// Creates an application while defining explicit metadata about the app's identity
        /// </summary>
        public AppManager(AppIdentity identity)
        {
            Instance = this;

            if (identity != null)
            {
                Init(identity);
            }
        }

        protected virtual void Init(AppIdentity identity)
        {
            if (CurrentScriptManager == null) { CurrentScriptManager = new ScriptManager(); }
            if (CurrentServerManager == null) { CurrentServerManager = new ServerManager(CreateFileResolver()); }
            if (CurrentIdentity == null) { CurrentIdentity = identity; }

            // Prebuild the command bridge so that the commands are instantly available to the webview
            var registerCall = new JSFunctionCall("window.igniteView.commandBridge.fillCommandList", (object)InteropCommands.ListCommands());
            RegisterPreloadScriptFromString(registerCall);

            PlatformManager.Instance.Create();
        }

        /// <summary>
        /// Call this on the main thread to start the lifecycle of the application
        /// </summary>
        public void Run()
        {
            PlatformManager.Instance.Run();
            OnCleanUp?.Invoke();
        }

        #region Method Forwarders

        /// <summary>
        /// Registers a script to load as soon as any WebWindow loads.
        /// This function MUST be called BEFORE any WebWindows are created.
        /// </summary>
        /// <param name="pathRelativeToRoot">The path of the file, relative to the URL root (eg. /preload.js)</param>
        public void RegisterPreloadScriptFromPath(string p) => CurrentScriptManager.RegisterPreloadScriptFromPath(p);

        /// <summary>
        /// Registers a script to load as soon as any WebWindow loads.
        /// This function MUST be called BEFORE any WebWindows are created.
        /// </summary>
        /// <param name="scriptContent">The raw script data</param>
        public void RegisterPreloadScriptFromString(string p) => CurrentScriptManager.RegisterPreloadScriptFromString(p);

        /// <summary>
        /// Allows you to register a custom file route to return dynamic data easily.
        /// </summary>
        /// <param name="relativeURL">The relative URL of the route (eg. "/hello.txt")</param>
        /// <param name="route">The function called when the route is navigated to</param>
        public void RegisterDynamicFileRoute(string relativeURL, Func<HttpContextBase, Task> route) => CurrentServerManager.RegisterDynamicFileRoute(relativeURL, route);
        #endregion
    }
}
