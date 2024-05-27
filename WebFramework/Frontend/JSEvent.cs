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
        /// <summary>
        /// Keeps Track Of Registered Event Listeners
        /// </summary>
        public static Dictionary<string, Action<JSEvent>> Listeners = new Dictionary<string, Action<JSEvent>>();

        /// <summary>
        /// Keeps Track Of Function IDs & Return Values Of Functions That Have Not Been Returned Yet
        /// </summary>
        public static ConcurrentDictionary<string, string> PendingFunctions = new ConcurrentDictionary<string, string>();

        public string JSONData;
        public JObject Data;

        public JSEvent(string j)
        {
            JSONData = j;
            Data = JObject.Parse(JSONData);
        }

    }
}
