using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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

        /// <summary>
        /// Unique Identifier Used To Identify This Window With JS Interop
        /// </summary>
        public string ID = "WebScriptID_" + Guid.NewGuid().ToString();

        public static void Register<T>(string scriptName) where T : WebScript
        {
            Logger.LogInfo("Registered WebScript: " + scriptName);
            RegisteredScripts[scriptName] = typeof(T);
        }

        public static void AttachToWindow(string scriptName, WebWindow context)
        {
            Type type = WebScript.RegisteredScripts[scriptName];
            var script = (WebScript)Activator.CreateInstance(type);
            script.Document = context.Document;
            context.AttachedScripts.Add(script);

            //Find JSFunctions To Bind
            MethodInfo[] methods = type.GetMethods();
            foreach (MethodInfo method in methods)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(method, true);
                foreach (Attribute attr in attrs)
                {
                    if (attr is JSFunctionAttribute)
                    {
                        BindJSFunction(attr, method, context, script);
                    }
                }

            }

            Task.Run(script.DOMContentLoaded);
        }

        public static void BindJSFunction(Attribute attr, MethodInfo method, WebWindow context, WebScript script)
        {
            JSFunctionAttribute jsFunctionBind = (JSFunctionAttribute)attr;
            var type = method.DeclaringType;

            var injection = $"window[`{jsFunctionBind.JSFunctionName}`] = async (...arguments) => await CallCSharp(`{type.FullName}, {type.Assembly.FullName}`, `{method.Name}`, ...(Array.from(arguments)));";

            if (!method.IsStatic) // Link Instance If It's A Non-Static Method
            {
                injection = $"window[`{jsFunctionBind.JSFunctionName}`] = async (...arguments) => await CallCSharp(`{script.ID}`, `{method.Name}`, ...(Array.from(arguments)));";
            }
     
            context.ExecuteJavascript(injection);
        }

        public virtual async Task DOMContentLoaded()
        {

        }

        public static WebScript GetWebScriptByID(string scriptID)
        {
            foreach (WebWindow window in WindowManager.OpenWindows)
            {
                foreach (WebScript script in window.AttachedScripts)
                {
                    if (script.ID == scriptID)
                    {
                        return script;
                    }
                }
            }

            return null;
        }
    }
}
