using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

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
        public static int LastWindowID = 0;

        public delegate void CleanUpCallback();

        /// <summary>
        /// This event is called after all the windows are closed and the application is about to exit.
        /// There are some situations where this is not called (crash, task manager, etc)
        /// </summary>
        public CleanUpCallback OnCleanUp;

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
            if (CurrentServerManager == null) { CurrentServerManager = new ServerManager(CreateFileResolver()); }
            if (CurrentIdentity == null) { CurrentIdentity = identity; }
            if (CurrentScriptManager == null) { CurrentScriptManager = new ScriptManager(); }

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

        public void RegisterPreloadScriptFromPath(string p) => CurrentScriptManager.RegisterPreloadScriptFromPath(p);
        public void RegisterPreloadScriptFromString(string p) => CurrentScriptManager.RegisterPreloadScriptFromString(p);

        #endregion
    }
}
