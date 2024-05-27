using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebFramework
{
    public class JSEvent : Dictionary<string, object>
    {

        public static Dictionary<string, Action<JSEvent>> Listeners = new Dictionary<string, Action<JSEvent>>();
        public static ConcurrentDictionary<string, string> PendingFunctions = new ConcurrentDictionary<string, string>();

        public string JSONData;
        public JObject Data;

        public static string GenerateFunction(string fname, params object[] values)
        {
            var js = JSFunction.SanitizeFunctionName(fname) + "(";
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                js += value.AsJavaScript() + ",";
            }

            js += ");";

            return js;
        }

        public JSEvent(string j)
        {
            JSONData = j;
            Data = JObject.Parse(JSONData);
        }

    }
}
