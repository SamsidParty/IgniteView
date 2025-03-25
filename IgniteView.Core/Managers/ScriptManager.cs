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
        /// List of scripts that are loaded into the WebView before page load, you must set this before any windows are created
        /// </summary>
        public static List<string> PreloadScripts = new List<string>();

        /// <summary>
        /// JS code string containing all the preload scripts merged into one
        /// </summary>
        public static string CombinedScriptData
        {
            get
            {
                var combinedScripts = "";
                PreloadScripts.ForEach(script => combinedScripts += "\n" + script);

                // Wrap the code in base64, this is because some of the webview implementations don't allow unicode characters
                return "if (!window.igniteView) { eval(atob('" + Convert.ToBase64String(Encoding.UTF8.GetBytes(combinedScripts)) + "')); }";
            }
        }

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

            PreloadScripts.Add(scriptContent);
        }
    }
}
