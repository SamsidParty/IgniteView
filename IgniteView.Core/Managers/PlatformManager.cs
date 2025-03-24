using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Base class that should be subclassed by each platform's runtime
    /// </summary>
    public abstract class PlatformManager
    {
        /// <summary>
        /// Creates a WebWindow object and returns it
        /// </summary>
        public abstract WebWindow CreateWebWindow();

        /// <summary>
        /// Runs the main loop of the application
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Runs after the AppManager has been initialized, but before any window has been created
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Gets the ScriptInjectionMode for this platform
        /// </summary>
        public abstract ScriptInjectionMode GetScriptInjectionMode();

        /// <summary>
        /// Gets the ServerListenMode for this platform
        /// </summary>
        public abstract ServerListenMode GetServerListenMode();

        /// <summary>
        /// Gets the current PlatformManager
        /// </summary>
        public static PlatformManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    throw new NullReferenceException("The PlatformManager.Instance object has never been set! you must call XXXPlatformManager.Activate()");
                }

                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }

        private static PlatformManager _Instance;
    }
}
