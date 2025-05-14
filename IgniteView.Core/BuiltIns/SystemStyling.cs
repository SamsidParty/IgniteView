using System;
using System.Collections.Generic;
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
                // TODO: Add macOS specific styles based on apple UI guidelines
            }
            else {
                // Fallback for other platforms
                var light = AppManager.Instance.CurrentServerManager.Resolver.ReadFileAsText("/igniteview/styles/default_light.json");
                var dark = AppManager.Instance.CurrentServerManager.Resolver.ReadFileAsText("/igniteview/styles/default_dark.json");
                systemStyles.AddRange(StyleRule.FromJSON(light));
                systemStyles.AddRange(StyleRule.FromJSON(dark, true));
            }
            
            // Combine all the styles into one stylesheet string and return it
            return String.Join("\n\n", systemStyles.Concat(GlobalStyles).Select(x => x.ToString()));
        }
    }
}
