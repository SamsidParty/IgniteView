using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace IgniteView.Core
{
    public class SystemStyling
    {
        public static List<StyleRule> GlobalStyles = new List<StyleRule>();

        [Command("igniteview_get_global_styles")]
        public static string GetGlobalStyles()
        {
            var systemStyles = new List<StyleRule>();

            // System font stack
            if (Environment.MachineName.ToLower().Contains("xbox")) {
                systemStyles.Add(new StyleRule("--system-font", "Bahnschrift, segoe ui, sans-serif"));
            }
            else {
                systemStyles.Add(new StyleRule("--system-font", "-apple-system, BlinkMacSystemFont, avenir next, avenir, segoe ui, helvetica neue, Cantarell, Ubuntu, roboto, noto, helvetica, arial, sans-serif"));
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                ApplyStylesFromJSON(systemStyles, "macos");
            }
            else if (File.Exists("/bin/kreadconfig5")) { // KDE Plasma
                try {
                    // Get the styles from the current KDE color scheme
                    systemStyles.Add(new StyleRule("--system-accent", ReadKDEColor("Colors:Button", "ForegroundLink")));
                    systemStyles.Add(new StyleRule("--system-accent-foreground", ReadKDEColor("Colors:Button", "BackgroundNormal")));
                    systemStyles.Add(new StyleRule("--system-background", ReadKDEColor("Colors:Window", "BackgroundNormal")));
                    systemStyles.Add(new StyleRule("--system-background2", ReadKDEColor("Colors:View", "BackgroundNormal")));
                    systemStyles.Add(new StyleRule("--system-foreground", ReadKDEColor("Colors:Window", "ForegroundNormal")));
                    systemStyles.Add(new StyleRule("--system-outline", ReadKDEColor("Colors:View", "ForegroundInactive", 0.2f)));
                }
                catch {
                    ApplyStylesFromJSON(systemStyles);
                }
            }
            else {
                // Fallback for other platforms
                ApplyStylesFromJSON(systemStyles);
            }
            
            // Combine all the styles into one stylesheet string and return it
            return String.Join("\n\n", systemStyles.Concat(GlobalStyles).Select(x => x.ToString()));
        }

        /// <summary>
        /// Reads a color from the KDE color scheme using kreadconfig5,
        /// This will throw an exception if the command fails
        /// </summary>
        static string ReadKDEColor(string group, string key, float opacity = 1) {
            var command = $"--group {group} --key {key}";
            var binary = "/bin/kreadconfig5";
            var psi = new ProcessStartInfo(binary, command) {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var process = Process.Start(psi)) {
                process.WaitForExit();
                return "rgba(" + process.StandardOutput.ReadToEnd().Trim() + ", " + opacity +  ")";
            }
        }

        public static void ApplyStylesFromJSON(List<StyleRule> styles, string prefix = "default") {
            var light = AppManager.Instance.CurrentServerManager.Resolver.ReadFileAsText($"/igniteview/styles/{prefix}_light.json");
            var dark = AppManager.Instance.CurrentServerManager.Resolver.ReadFileAsText($"/igniteview/styles/{prefix}_dark.json");
            styles.AddRange(StyleRule.FromJSON(light));
            styles.AddRange(StyleRule.FromJSON(dark, true));
        }
    }
}
