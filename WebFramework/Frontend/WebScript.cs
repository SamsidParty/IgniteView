using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework
{
    public class WebScript
    {
        public static Dictionary<string, Type> RegisteredScripts = new Dictionary<string, Type>();

        public DOM Document;
        public WebWindow Window;

        public static void Register<T>(string scriptName) where T : WebScript
        {
            Logger.LogInfo("Registered WebScript: " + scriptName);
            RegisteredScripts[scriptName] = typeof(T);
        }

        public static void AttachToWindow(string scriptName, WebWindow w)
        {
            var type = WebScript.RegisteredScripts[scriptName];
            var script = (WebScript)Activator.CreateInstance(type);
            script.Document = w.Document;
            Task.Run(script.DOMContentLoaded);
        }

        public virtual async Task DOMContentLoaded()
        {

        }
    }
}
