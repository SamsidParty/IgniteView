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
        public AppIdentity CurrentIdentity;

        /// <summary>
        /// Creates an application while defining explicit metadata about the app's identity
        /// </summary>
        public AppManager(AppIdentity identity)
        {
            CurrentIdentity = identity;
        }

        /// <summary>
        /// Call this on the main thread to start the lifecycle of the application
        /// </summary>
        public void Run()
        {

        }
    }
}
