using System;
using System.Collections.Generic;
using System.Linq;
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
            var dynamicSystemStyles = new List<StyleRule>();

            // System font stack
            if (Environment.MachineName.ToLower().Contains("xbox")) {
                dynamicSystemStyles.Add(new StyleRule("--system-font", "Bahnschrift, segoe ui, sans-serif"));
            }
            else {
                dynamicSystemStyles.Add(new StyleRule("--system-font", "-apple-system, BlinkMacSystemFont, avenir next, avenir, segoe ui, helvetica neue, Cantarell, Ubuntu, roboto, noto, helvetica, arial, sans-serif"));
            }
            
            // Combine all the styles into one stylesheet string and return it
            return String.Join("\n\n", dynamicSystemStyles.Concat(GlobalStyles).Select(x => x.ToString()));
        }
    }
}
