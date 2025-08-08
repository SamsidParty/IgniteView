using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// Returns a list of platform "hints" that can be used to determine not only the OS, but also what kind of device it is running on
        /// This is useful for determining what kind of UI to use, or what features to enable/disable
        /// For example, if running on xbox, it will return ["windows", "uwp", "xbox"]
        /// </summary>
        public static List<string> PlatformHints 
        {
            get
            {
                if (_PlatformHints == null)
                {
                    var newPlatformHints = new List<string>();

                    // Add hints for operating system
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        newPlatformHints.Add("windows");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        newPlatformHints.Add("linux");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        newPlatformHints.Add("macos");
                    }

                    // Add hints for architecture
                    newPlatformHints.Add(RuntimeInformation.ProcessArchitecture.ToString().ToLower());

                    // Add hints for consoles, like xbox and steamdeck
                    if (Environment.MachineName.ToLower().Contains("xbox"))
                    {
                        newPlatformHints.Add("xbox");
                    }
                    else if (newPlatformHints.Contains("linux") && (Environment.MachineName.ToLower().Contains("steamdeck") || Directory.Exists("/home/deck")))
                    {
                        newPlatformHints.Add("steamdeck");
                    }

                    // Differentiate desktop environments on linux
                    if (newPlatformHints.Contains("linux")) {
                        var desktopEnv = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP")?.ToLower();
                        newPlatformHints.Add(desktopEnv ?? "unknown_desktop");
                    }

                    _PlatformHints = newPlatformHints;
                }

                return _PlatformHints;
            }
        }

        public static bool HasPlatformHint(string h) => PlatformHints.Contains(h);

        private static PlatformManager _Instance;
        private static List<string> _PlatformHints;
    }
}
