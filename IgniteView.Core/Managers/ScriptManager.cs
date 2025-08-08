using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver.Core;

namespace IgniteView.Core
{
    public class ScriptManager
    {
        private static List<string> StaticPreloadScripts = new List<string>();
        private static List<Func<string>> DynamicPreloadScripts = new List<Func<string>>();

        /// <summary>
        /// JS code string containing all the preload scripts merged into one
        /// </summary>
        public static string CombinedScriptData
        {
            get
            {
                return $"if (!window.igniteView) {{ \n\n {StaticScriptData} \n\n {DynamicScriptData} \n\n }}";
            }
        }

        /// <summary>
        /// JS code string containing all the static preload scripts merged into one
        /// </summary>
        public static string StaticScriptData
        {
            get
            {
                var combinedScripts = "";
                StaticPreloadScripts.ForEach(script => combinedScripts += "\n\n" + script + "\n\n");
                return combinedScripts;
            }
        }

        /// <summary>
        /// JS code string containing all the dynamic preload scripts merged into one
        /// </summary>
        public static string DynamicScriptData
        {
            get
            {
                var combinedScripts = "";
                DynamicPreloadScripts.ForEach(script => combinedScripts += "\n\n" + script.Invoke() + "\n\n");
                return combinedScripts;
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

            StaticPreloadScripts.Add(scriptContent);
        }

        /// <summary>
        /// Registers a dynamic script to load as soon as any WebWindow loads.
        /// This function MUST be called BEFORE any WebWindows are created.
        /// </summary>
        /// <param name="scriptContent">The raw script data</param>
        public void RegisterPreloadScriptFromFunction(Func<string> preloadFunction)
        {
            if (AppManager.Instance.OpenWindows.Count > 0)
            {
                throw new InvalidOperationException("Registering preload scripts must happen BEFORE any windows are created.");
            }

            DynamicPreloadScripts.Add(preloadFunction);
        }
    }
}
