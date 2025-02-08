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
