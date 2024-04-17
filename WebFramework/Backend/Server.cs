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
using System.Net.Sockets;

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
            while (true)
            {
                TcpListener l = new TcpListener(IPAddress.Loopback, 0);
                l.Start();
                int testPort = ((IPEndPoint)l.LocalEndpoint).Port;
                l.Stop();
                return testPort;
            }
        }

        public static bool isValidPort(int port)
        {
            if (port < 2048) { return false; }

            //List Of Ports Chromium Doesn't Like
            var unsafePorts = new int[]
            {
                1,      // tcpmux
                7,      // echo
                9,      // discard
                11,     // systat
                13,     // daytime
                15,     // netstat
                17,     // qotd
                19,     // chargen
                20,     // ftp data
                21,     // ftp access
                22,     // ssh
                23,     // telnet
                25,     // smtp
                37,     // time
                42,     // name
                43,     // nicname
                53,     // domain
                69,     // tftp
                77,     // priv-rjs
                79,     // finger
                87,     // ttylink
                95,     // supdup
                101,    // hostriame
                102,    // iso-tsap
                103,    // gppitnp
                104,    // acr-nema
                109,    // pop2
                110,    // pop3
                111,    // sunrpc
                113,    // auth
                115,    // sftp
                117,    // uucp-path
                119,    // nntp
                123,    // NTP
                135,    // loc-srv /epmap
                137,    // netbios
                139,    // netbios
                143,    // imap2
                161,    // snmp
                179,    // BGP
                389,    // ldap
                427,    // SLP (Also used by Apple Filing Protocol)
                465,    // smtp+ssl
                512,    // print / exec
                513,    // login
                514,    // shell
                515,    // printer
                526,    // tempo
                530,    // courier
                531,    // chat
                532,    // netnews
                540,    // uucp
                548,    // AFP (Apple Filing Protocol)
                554,    // rtsp
                556,    // remotefs
                563,    // nntp+ssl
                587,    // smtp (rfc6409)
                601,    // syslog-conn (rfc3195)
                636,    // ldap+ssl
                993,    // ldap+ssl
                995,    // pop3+ssl
                1719,   // h323gatestat
                1720,   // h323hostcall
                1723,   // pptp
                2049,   // nfs
                3659,   // apple-sasl / PasswordServer
                4045,   // lockd
                5060,   // sip
                5061,   // sips
                6000,   // X11
                6566,   // sane-port
                6665,   // Alternate IRC [Apple addition]
                6666,   // Alternate IRC [Apple addition]
                6667,   // Standard IRC [Apple addition]
                6668,   // Alternate IRC [Apple addition]
                6669,   // Alternate IRC [Apple addition]
                6697,   // IRC + TLS
                10080,  // Amanda
            };

            if (unsafePorts.Contains(port))
            {
                return false;
            }

            return true;
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
