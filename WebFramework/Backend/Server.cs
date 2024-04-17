using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Runtime.InteropServices;

namespace WebFramework
{
    public class Server
    {


        public static int HTTPPort;

        public static async Task Start()
        {
            HTTPPort = NextFreePort();
            SimpleHttpServer.StartHttpServerOnThread(AppManager.Location, port: HTTPPort);

            while (!SimpleHttpServer.Started) { Thread.Sleep(1); }

            Thread.Sleep(1);
        }



        static bool IsFree(int port)
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] listeners = properties.GetActiveTcpListeners();
            int[] openPorts = listeners.Select(item => item.Port).ToArray<int>();
            return openPorts.All(openPort => openPort != port);
        }

        public static int NextFreePort(int port = 0)
        {
            port = (port > 0) ? port : new Random().Next(2048, 65535);
            while (!IsFree(port))
            {
                port += 1;
            }
            return port;
        }
    }

    public class MSGHandler
    {
        public static async Task OnMessage(string d, WebWindow context)
        {
            try
            {
                var msg = JsonSerializer.Deserialize<WSMessage>(d, WSMessage.Options);

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
                    var cb = new CallbackAction(WSCallbacks.OnEvent, msg.Param1, msg.Param2, msg.Param3);
                    cb.InvokeOnUI();
                }
                else if (msg.Type == "attach")
                {
                    var cb = new CallbackAction(WSCallbacks.OnAttachRequested, msg.Param1, msg.Param2, msg.Param3);
                    cb.InvokeOnUI();
                }
                else if (msg.Type == "retval")
                {
                    var cb = new CallbackAction(WSCallbacks.OnValueReturned, msg.Param1, msg.Param2, msg.Param3);
                    cb.InvokeOnUI();
                }
                else if (msg.Type == "closeme")
                {
                    Jobs.Push(() =>
                    {
                        (context as PTWebWindow).Native.Close();
                    }, WindowManager.MainWindow.Document);
                    Jobs.Fire(WindowManager.MainWindow.Document);
                }
                else if (msg.Type == "title")
                {
                    await context.UpdateTitle(msg.Param1);
                }
                else if (msg.Type == "firejobs"){
                    Jobs.Pop();
                }
            }
            catch (Exception ex) { context.Document.RunFunction("console.error", ex.ToString()); }
        }

    }

    [Serializable]
    public class WSMessage
    {
        public static JsonSerializerOptions Options = new JsonSerializerOptions() { IncludeFields = true };

        public string Type;
        public string Param1;
        public string Param2;
        public string Param3;
    }
}
