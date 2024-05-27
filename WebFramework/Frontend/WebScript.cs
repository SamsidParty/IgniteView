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

            //Find JSFunctions To Bind
            MethodInfo[] methods = type.GetMethods();
            foreach (MethodInfo method in methods)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(method, true);
                foreach (Attribute attr in attrs)
                {
                    if (attr is JSFunctionAttribute)
                    {
                        BindJSFunction(attr, method, context);
                    }
                }

            }

            Task.Run(script.DOMContentLoaded);
        }

        public static void BindJSFunction(Attribute attr, MethodInfo method, WebWindow context)
        {
            JSFunctionAttribute jsFunctionBind = (JSFunctionAttribute)attr;
            var type = method.DeclaringType;
            var injection = $"window[`{jsFunctionBind.JSFunctionName}`] = async (...arguments) => await CallCSharp(`{type.FullName}, {type.Assembly.FullName}`, `{method.Name}`, ...(Array.from(arguments)));";
            context.ExecuteJavascript(injection);
        }

        public virtual async Task DOMContentLoaded()
        {

        }
    }
}
