using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public class DOM
    {
        public WebWindow Window;

        public Element Body;
        public Element Head;
        public Element Root;

        public async Task RunFunction(string fname, params object[] values)
        {
            await Window.ExecuteJavascript(JSEvent.GenerateFunction(fname, values));
        }

        public async Task<string> RunFunctionWithReturn(string fname, params object[] values)
        {
            var FnID = "interopfunction-" + fname + "-" + Guid.NewGuid().ToString();
            JSEvent.PendingFunctions[FnID] = "JSI_NotReturned";
            var js = "let fnResult = ";
            js += JSEvent.GenerateFunction(fname, values);
            js += "\n";
            js += JSEvent.GenerateFunction("JSI_Send", "retval", FnID, "fnResult".ToJSLiteral());
            await Window.ExecuteJavascript(js);

            while (JSEvent.PendingFunctions[FnID] == "JSI_NotReturned")
            {
                await Task.Delay(1);
            }

            var retVal = JSEvent.PendingFunctions[FnID];
            JSEvent.PendingFunctions.TryRemove(FnID, out retVal);

            return retVal;
        }

        /// <summary>
        /// Runs C# Code When A DOM Event Is Fired
        /// </summary>
        public void AddEventListener(string eventName, Action<JSEvent> callback)
        {
            var EventID = "interopevent-" + eventName + "-" + Guid.NewGuid().ToString();
            JSEvent.Listeners[EventID] = callback;
            RunFunction("JSI_EventListener", eventName, EventID);
        }
        public Action<string, Action<JSEvent>> On => AddEventListener;

        /// <summary>
        /// Runs JavaScript Code When A DOM Event Is Fired
        /// </summary>
        public void AddJSEventListener(string eventName, string codeToRunOnCallback)
        {
            RunFunction("JSI_LocalEventListener", eventName, codeToRunOnCallback);
        }

        public void SetCSSProperty(string selector, string property, CSSValue value)
        {
            RunFunction("JSI_CSS", selector, property, value._value);
        }

        public async Task Init()
        {
            Body = Element.PresetLink("body", this);
            Head = Element.PresetLink("head", this);
            Root = Element.PresetLink("root", this);
        }

        public async Task<Element> GetElementByID(string id)
        {
            return await QuerySelector("#" + id);
        }

        public async Task<Element> QuerySelector(string selector)
        {
            return await Element.Get(selector, this);
        }

        public async Task<Element> CreateElement(string type)
        {
            var id = await RunFunctionWithReturn("JSI_CreateElement", type);
            var e = new Element();
            e.InternalID = id;
            e.Document = this;
            return e;
        }

        public DOM(WebWindow w) { 
            Window = w;
        }
    }

    public class Element
    {
        public string InternalID;
        public DOM Document;

        public async Task<string> RunFunctionWithReturn(string fname, params object[] values) => await Document.RunFunctionWithReturn(fname, values);

        public void RunChildFunction(string fname, params object[] args)
        {
            Document.RunFunction("JSI_RunFunctionOnElement", InternalID, fname, args);
        }

        public static async Task<Element> Get(string selector, DOM ctx)
        {
            var exists = "true";
            if (exists == "true")
            {
                var e = new Element();
                e.InternalID = "element-" + Guid.NewGuid().ToString();
                e.Document = ctx;
                await ctx.RunFunctionWithReturn("JSI_RegisterInteropElement", selector, e.InternalID);
                return e;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// For Internal Use
        /// </summary>
        public static Element PresetLink(string id, DOM ctx)
        {
            var e = new Element();
            e.InternalID = id;
            e.Document = ctx;
            return e;
        }

        ~Element()
        {
            RunFunctionWithReturn("JSI_RemoveInteropElement", InternalID);
        }

        public async Task<string> GetProperty(string property)
        {
            return await RunFunctionWithReturn("JSI_GetProperty", InternalID, property);
        }

        public async Task<string> SetProperty(string property, object value)
        {
            return await RunFunctionWithReturn("JSI_SetProperty", InternalID, property, value);
        }

        public void AppendChild(Element child)
        {
            Document.RunFunction("JSI_AppendChild", InternalID, child.InternalID);
        }

        public void Click()
        {
            RunChildFunction("click");
        }

        /// <summary>
        /// Runs C# Code When A DOM Event Is Fired On This Element
        /// </summary>
        public void AddEventListener(string eventName, Action<JSEvent> callback)
        {
            var EventID = "interopevent-" + eventName + "-" + Guid.NewGuid().ToString();
            JSEvent.Listeners[EventID] = callback;
            Document.RunFunction("JSI_On", eventName, EventID, InternalID);
        }
        public Action<string, Action<JSEvent>> On => AddEventListener;


        /// <summary>
        /// Runs JavaScript Code When A DOM Event Is Fired On This Element
        /// </summary>
        public void AddJSEventListener(string eventName, string codeToRunOnCallback)
        {
            Document.RunFunction("JSI_LocalOn",  eventName, codeToRunOnCallback, InternalID);
        }

        public async Task<string> GetInnerHTML()
        {
            return await GetProperty("innerHTML");
        }

        public async Task<string> SetInnerHTML(string value)
        {
            return await SetProperty("innerHTML", value);
        }

        public async Task<string> SetCSSProperty(string property, CSSValue value)
        {
            return await RunFunctionWithReturn("JSI_SetCSSOnElement", InternalID, property, value._value);
        }

        public async Task<string> GetCSSProperty(string property)
        {
            return await RunFunctionWithReturn("JSI_GetCSSOnElement", InternalID, property);
        }
    }
}
