using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework
{
    //All Functions Run On UI Thread
    public class InteropCallbacks
    {

        public static void OnValueReturned(string p1, string p2, string p3, WebWindow context) //[1] = Function ID, [2] = Function Returned Value
        {
            JSEvent.PendingFunctions[p1] = p2;
        }

        public static void OnEvent(string p1, string p2, string p3, WebWindow context) //[1] = Event ID, [2] = Event Data
        {
            var id = JSEvent.Listeners[p1];
            id.Invoke(new JSEvent(p2));
        }

        public static void OnReflect(string p1, string p2, string p3, WebWindow context) //[1] = Type, [2] = Method To Call, [3] = List Of Args, In JSON
        {
            try
            {
                var type = Type.GetType(p1);
                type.GetMethod(p2).Invoke(null, JsonConvert.DeserializeObject<object[]>(p3));
            }
            catch (Exception ex) {
                Logger.LogError("Failed To Invoke CSharp Function At Runtime: " + ex.ToString());
            }
        }

        public static void OnAttachRequested(string p1, string p2, string p3, WebWindow context) //[1] = Script Name
        {
            try
            {
                WebScript.AttachToWindow(p1, context);
            }
            catch { }
        }

    }

    [Serializable]
    public class CallbackAction
    {
        Action<string, string, string, WebWindow> Inner;

        public string Param1;
        public string Param2;
        public string Param3;
        public WebWindow Window;

        public CallbackAction(Action<string, string, string, WebWindow> inner, string p1, string p2, string p3, WebWindow context)
        {
            Inner = inner;
            Param1 = p1;
            Param2 = p2;
            Param3 = p3;
            Window = context;
        }

        public void Invoke()
        {
            Inner.Invoke(Param1, Param2, Param3, Window);
        }

        public void InvokeOnUI()
        {
            Invoke();
        }
    }
}
