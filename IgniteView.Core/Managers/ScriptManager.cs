using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class ScriptManager
    {
        /// <summary>
        /// Registers a script to load as soon as any WebWindow loads.
        /// This function MUST be called BEFORE any WebWindows are created.
        /// </summary>
        /// <param name="pathRelativeToRoot">The path of the file, relative to the URL root (eg. /preload.js)</param>
        public void RegisterPreloadScriptFromPath(string pathRelativeToRoot)
        {
            var resolver = AppManager.Instance.CurrentServerManager.Resolver;

            if (!resolver.DoesFileExist(pathRelativeToRoot)) {
                throw new FileNotFoundException($"Preload script {pathRelativeToRoot} does not exist, make sure the path is relative to the web root and starts with a '/'");
            }

            RegisterPreloadScriptFromString(resolver.ReadFileAsText(pathRelativeToRoot));
        }

        /// <summary>
        /// Registers a script to load as soon as any WebWindow loads.
        /// This function MUST be called BEFORE any WebWindows are created.
        /// </summary>
        /// <param name="scriptContent">The raw script data</param>
        public void RegisterPreloadScriptFromString(string scriptContent)
        {
            if (AppManager.Instance.OpenWindows.Count > 0)
            {
                throw new InvalidOperationException("Registering preload scripts must happen BEFORE any windows are created.");
            }

            InjectedScript.PreloadScripts.Add(scriptContent);
        }
    }
}
