using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Threading;

namespace WebFramework
{
    public class Server
    {

        public static int HTTPPort;

        public static async Task Start()
        {
            HTTPPort = SimpleHttpServer.EmptyPort();
            SimpleHttpServer.StartHttpServerOnThread(AppManager.Location, port: HTTPPort);

            while (!SimpleHttpServer.Started) { Thread.Sleep(25); }
            Thread.Sleep(25);
        }
    }

    [Serializable]
    public class WSMessage
    {
        public string Type;
        public string Param1;
        public string Param2;
        public string Param3;
    }

    public class MSGHandler
    {
        public static async Task OnMessage(string d, WebWindow context)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<WSMessage>(d);

                foreach (var k in context.MessageListeners.Keys)
                {
                    if (k == msg.Type || k.StartsWith("*"))
                    {
                        context.MessageListeners[k].Invoke(msg, context);
                    }
                }

                if (msg.Type == "load")
                {
                    await context.WindowReady();
                }
                else if (msg.Type == "event")
                {
                    var cb = new CallbackAction(InteropCallbacks.OnEvent, msg.Param1, msg.Param2, msg.Param3, context);
                    cb.InvokeOnUI();
                }
                else if (msg.Type == "reflect")
                {
                    var cb = new CallbackAction(InteropCallbacks.OnReflect, msg.Param1, msg.Param2, msg.Param3, context);
                    cb.InvokeOnUI();
                }
                else if (msg.Type == "attach")
                {
                    var cb = new CallbackAction(InteropCallbacks.OnAttachRequested, msg.Param1, msg.Param2, msg.Param3, context);
                    cb.InvokeOnUI();
                }
                else if (msg.Type == "retval")
                {
                    var cb = new CallbackAction(InteropCallbacks.OnValueReturned, msg.Param1, msg.Param2, msg.Param3, context);
                    cb.InvokeOnUI();
                }
                else if (msg.Type == "closeme")
                {
                    Jobs.Add(async () =>
                    {
                        await context.Close();
                    }, WindowManager.MainWindow.Document);
                    Jobs.RunAllFromRemoteThread(WindowManager.MainWindow.Document);
                }
                else if (msg.Type == "title")
                {
                    await context.UpdateTitle(msg.Param1);
                }
                else if (msg.Type == "firejobs")
                {
                    Jobs.RunAll();
                }
            }
            catch (Exception ex) { context.Document.RunFunction("console.error", ex.ToString()); }
        }

    }
}
