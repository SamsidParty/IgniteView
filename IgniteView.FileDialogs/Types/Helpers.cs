using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs
{
    public static class Helpers
    {
        public static Dictionary<string, string> ToKeyValuePairs(this FileFilter[] filters)
        {
            var dict = new Dictionary<string, string>();
            foreach (var filter in filters) {
                dict[filter.Name] = filter.Pattern;
            }
            return dict;
        }
    }
}
