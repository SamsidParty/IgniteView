using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Creates an application while defining explicit metadata about the app's identity
        /// </summary>
        public AppManager(AppIdentity identity)
        {
            Instance = this;

            CurrentServerManager = new ServerManager();
            CurrentIdentity = identity;

            PlatformManager.Instance.Create();
        }

        /// <summary>
        /// Call this on the main thread to start the lifecycle of the application
        /// </summary>
        public void Run()
        {
            PlatformManager.Instance.Run();
        }
    }
}
