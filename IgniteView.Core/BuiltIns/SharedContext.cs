using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class SharedContext
    {
        [Command("igniteview_get_context_value")]
        public static object GetContextValue(WebWindow window, string key)
        {
            if (window.SharedContext.ContainsKey(key))
            {
                return window.SharedContext[key];
            }

            return null;
        }

        [Command("igniteview_set_context_value")]
        public static void SetContextValue(WebWindow window, string key, object value)
        {
            window.SharedContext[key] = value;
        }

        [Command("igniteview_get_all_context_values")]
        public static Dictionary<string, object> GetAllContextValues(WebWindow window)
        {
            return window.SharedContext;
        }
    }
}
