// This file is generated, to edit it you must edit IgniteView.InjectedScript/InjectedScript.cs

namespace IgniteView.Core
{
    /// <summary>
    /// Generated file that holds the javascript to inject into the html files
    /// </summary>
    public class InjectedScript
    {
        /// <summary>
        /// The built-in IgniteView preload script that is required for IgniteView to work
        /// </summary>
        public const string ScriptData = "%SCRIPT_DATA%"; // Replaced when running build.py

        /// <summary>
        /// List of scripts that are loaded into the WebView before page load, you must set this before any windows are created
        /// </summary>
        public static List<string> PreloadScripts = new List<string>();

        /// <summary>
        /// Combination of all the preload scripts and the IgniteView script
        /// </summary>
        public static string CombinedScriptData {
            get {
                var combinedScripts = ScriptData;
                PreloadScripts.ForEach(script => combinedScripts += "\n" + script);
                return combinedScripts;
            }
        }
    }
}
