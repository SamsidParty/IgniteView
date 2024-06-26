﻿//Modified From https://gist.github.com/abc126/40906c32d4f076ab26e1228294638339


using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Text;
using System.Linq;
using System.Xml;
using System.Runtime.InteropServices;
using WebFramework.Backend;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WebFramework
{
    /// <summary>
    /// A simple self-contained static file Web Server that can be
    /// launched in a folder with a port and serve static files
    /// from that folder. Very basic features but easy to integrate.
    /// </summary>
    /// <example>
    /// StartHttpServerOnThread(@"c:\temp\http",8080);
    /// ...
    /// StopHttpServerOnThread();
    /// </example>
    /// <remarks>
    /// Based mostly on:
    /// https://gist.github.com/aksakalli/9191056
    /// 
    /// Additions to make it easier to host server inside of an
    /// external, non-.NET application.
    ///</remarks>

    public class SimpleHttpServer
    {
        public string[] DefaultDocuments =
        {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
        };

        public static SimpleHttpServer Current;

        public static bool Started = false;

        /// <summary>
        /// This method can be used externally to start a singleton instance of 
        /// the Web Server and keep it running without tracking a reference.                
        /// 
        /// If a server instance is already running it's shut down.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="port"></param>        
        /// <param name="requestHandler">
        /// Optional parameter of an object that has a Process method that gets passed a context 
        /// and returns true if the request is handled or false if default processing should occur
        /// </param>
        public static void StartHttpServerOnThread(string path, int port = 8080, object requestHandler = null)
        {
            var t = new Thread(StartHttpServerThread);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                t.SetApartmentStateSafe(ApartmentState.STA);
            }

            t.Start(new ServerStartParameters { Path = path, Port = port, RequestHandler = requestHandler });
        }

        /// <summary>
        /// Call this method to stop the Singleton instance of the server.
        /// </summary>
        public static void StopHttpServerOnThread()
        {
            Current.Stop();
            Current = null;
        }


        /// <summary>
        /// Internal method that instantiates the server instance
        /// </summary>
        /// <param name="parms"></param>
        private static void StartHttpServerThread(object parms)
        {

            if (Current != null)
                StopHttpServerOnThread();

            var httpParms = parms as ServerStartParameters;
            Current = new SimpleHttpServer(httpParms.Path, httpParms.Port);
            Current.RequestHandler = httpParms.RequestHandler;
        }


        /// <summary>
        /// Mime Type conversion table
        /// </summary>
        public static IDictionary<string, string> _mimeTypeMappings =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                #region extension to MIME type list
                {".asf", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".avi", "video/x-msvideo"},
                {".bin", "application/octet-stream"},
                {".cco", "application/x-cocoa"},
                {".crt", "application/x-x509-ca-cert"},
                {".css", "text/css"},
                {".deb", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dll", "application/octet-stream"},
                {".dmg", "application/octet-stream"},
                {".ear", "application/java-archive"},
                {".eot", "application/octet-stream"},
                {".exe", "application/octet-stream"},
                {".flv", "video/x-flv"},
                {".gif", "image/gif"},
                {".hqx", "application/mac-binhex40"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".ico", "image/x-icon"},
                {".img", "application/octet-stream"},
                {".iso", "application/octet-stream"},
                {".jar", "application/java-archive"},
                {".jardiff", "application/x-java-archive-diff"},
                {".jng", "image/x-jng"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/x-javascript"},
                {".mml", "text/mathml"},
                {".mng", "video/x-mng"},
                {".mov", "video/quicktime"},
                {".mp3", "audio/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpg", "video/mpeg"},
                {".msi", "application/octet-stream"},
                {".msm", "application/octet-stream"},
                {".msp", "application/octet-stream"},
                {".pdb", "application/x-pilot"},
                {".pdf", "application/pdf"},
                {".pem", "application/x-x509-ca-cert"},
                {".pl", "application/x-perl"},
                {".pm", "application/x-perl"},
                {".png", "image/png"},
                {".prc", "application/x-pilot"},
                {".ra", "audio/x-realaudio"},
                {".rar", "application/x-rar-compressed"},
                {".rpm", "application/x-redhat-package-manager"},
                {".rss", "text/xml"},
                {".run", "application/x-makeself"},
                {".sea", "application/x-sea"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".swf", "application/x-shockwave-flash"},
                {".tcl", "application/x-tcl"},
                {".tk", "application/x-tcl"},
                {".txt", "text/plain"},
                {".war", "application/java-archive"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wmv", "video/x-ms-wmv"},
                {".xml", "text/xml"},
                {".xpi", "application/x-xpinstall"},
                {".zip", "application/zip"},
                {".svg", "image/svg+xml"},

                #endregion
            };

        private Thread _serverThread;
        private string _rootDirectory;
        private HttpListener _listener;
        private int _port;

        public int Port
        {
            get { return _port; }
        }


        /// <summary>
        /// Instance of an object whose Process() method is called on each request.
        /// Return true if the reuqest is handled, fase if it's not.
        /// </summary>
        public object RequestHandler { get; set; }

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public SimpleHttpServer(string path, int port = 8080)
        {
            Initialize(path, port);
        }

        //https://stackoverflow.com/a/53936819/18071273

        static bool IsFree(int port)
        {

            if (Platform.isUWP || Platform.isMAUI) { return true; } // Network Isolation

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] listeners = properties.GetActiveTcpListeners();
            int[] openPorts = listeners.Select(item => item.Port).ToArray<int>();
            return openPorts.All(openPort => openPort != port);
        }

        //Find An Empty Port Recursively
        public static int EmptyPort()
        {
            if (DevTools.ForcedPort > 0)
            {
                return DevTools.ForcedPort;
            }

            try
            {
                var port = Helpers.SharedRandom.Next(10081, 65500);

                if (port < 10081 || !isValidPort(port)) // Chrome Doesn't Like Some Ports (ERR_UNSAFE_PORT)
                {
                    throw new Exception("Unsafe Port");
                }

                if (!IsFree(port))
                {
                    throw new Exception("Port In Use");
                }
                return port;
            }
            catch
            {
                return EmptyPort();
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

    /// <summary>
    /// Stop server and dispose all functions.
    /// </summary>
    public void Stop()
    {
        _serverThread.Abort();
        _listener.Stop();
    }

    /// <summary>
    /// Internal Handler
    /// </summary>
    private void Listen()
    {
        _listener = new HttpListener();
        Logger.LogInfo("Starting HTTP Server On Port: " + _port);
        _listener.Prefixes.Add("http://localhost:" + _port+ "/");
        _listener.Start();

        Logger.LogInfo("Started HTTP Server (Listening On localhost:" + _port);

            Started = true;

        while (true)
        {
            try
            {
                HttpListenerContext context = _listener.GetContext();

                //Enable CORS On Dev Mode Only
                if (DevTools.Enabled)
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                }

                //Intercepted Requests
                var handled = false;
                for (int i = 0; i < RequestInterceptor.Interceptors.Count; i++)
                {
                    if (RequestInterceptor.Interceptors[i](context))
                    {
                        handled = true;
                        break;
                    }
                }

                if (handled)
                {
                    continue;
                }

                //Get Requests Are Handled By The Static HTTP Server
                if (context.Request.HttpMethod == "GET")
                {
                    Process(context);
                }
                //Post Requests Are Passed Through For JS-Interop
                else if (context.Request.HttpMethod == "POST")
                {
                    ProcessJSI(context);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }
    }

        private async Task ProcessJSI(HttpListenerContext context)
        {
            var reader = new StreamReader(context.Request.InputStream);
            var jsiData = reader.ReadToEnd();

            if (!jsiData.StartsWith("{"))
            {
                //The Client Didn't Send Valid JSON
                //In This Situation, The Client Has Sent A File Read Request
                //Return The File

                var stream = await SharedIO.File.GetStream(jsiData);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await stream.CopyToAsync(context.Response.OutputStream);
                context.Response.OutputStream.Close();
                stream.Close();
            }
            else
            {
                MSGHandler.OnMessage(jsiData, WindowManager.MainWindow);

                context.Response.StatusCode = (int)HttpStatusCode.Accepted;
                context.Response.OutputStream.Close();
            }


        }


        /// <summary>
        /// Process an individual request. Handles only static file based requests
        /// </summary>
        /// <param name="context"></param>
        private async Task Process(HttpListenerContext context)
        {
            Logger.LogInfo("Received HTTP Request At: " + context.Request.Url);

            string filename = context.Request.Url.AbsolutePath;
            string rawFilename = context.Request.Url.AbsolutePath;


            filename = filename.Substring(1);
            rawFilename = rawFilename.Substring(1);

            if (string.IsNullOrEmpty(filename))
            {
                foreach (string indexFile in DefaultDocuments)
                {
                    if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
                    {
                        filename = indexFile;
                        rawFilename = indexFile;
                        break;
                    }
                }
            }

            filename = Path.Combine(_rootDirectory, filename);

            var mauiFileExists = false;
            if (Platform.isMAUI)
            {
                var fs = MAUIHelperLoader.Current.GetFileSystem();
                try
                {
                    //Throws Exception
                    mauiFileExists = await fs.AppPackageFileExistsAsync(Path.Combine("WWW", rawFilename));
                }
                catch { }
            }


            if (File.Exists(filename) || mauiFileExists)
            {
                try
                {
                    string mime;
                    context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime)
                        ? mime
                        : "application/octet-stream";

                    Stream input;
                    if (mime == "text/html")
                    {
                        //Inject Script
                        var injection = "<script> /* Injection For Interop */\r\n window.external = (window.external.sendMessage != undefined) ? window.external : { receiveMessage: (e) => {}, sendMessage: (data) => { fetch(\"./jsi.dat\", { method: \"POST\", body: data }); } }; \r\n window.external.receiveMessage(message => eval.call(window, message));\r\n addEventListener(\"DOMContentLoaded\", () => { window.external.sendMessage(`{\"Type\":\"load\"}`);\r\n });</script>\r\n";

                        if (WindowManager.MainWindow != null)
                        {
                            Logger.LogInfo("Sending Modified Index File");
                            injection = WindowManager.MainWindow.OverrideLib(injection);
                        }

                        var output = injection + (!Platform.isMAUI ? File.ReadAllText(filename) : "");
                        if (Platform.isMAUI)
                        {
                            //MAUI Needs It's Own File Protocol
                            var fs = MAUIHelperLoader.Current.GetFileSystem();
                            Stream fileStream = await fs.OpenAppPackageFileAsync(Path.Combine("WWW", rawFilename));
                            StreamReader reader = new StreamReader(fileStream);
                            output += reader.ReadToEnd();
                        }

                        //Inject Requested Inline Files
                        var pFrom = output.IndexOf("<inlines>") + "<inlines>".Length;
                        var pTo = output.LastIndexOf("</inlines>");

                        if (pFrom > -1 && pTo > -1)
                        {
                            var inlines = "<root>" + output.Substring(pFrom, pTo - pFrom) + "</root>";

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(inlines);

                            var styles = doc.GetElementsByTagName("inline-style");
                            var scripts = doc.GetElementsByTagName("inline-script");

                            var outStyles = "";
                            var outScripts = "";


                            foreach (XmlNode s in styles)
                            {
                                var iPath = Path.Combine(_rootDirectory, s.InnerText);
                                var iData = File.ReadAllText(iPath);
                                outStyles += "\n<style>\n" + iData + "\n</style>\n";
                            }

                            foreach (XmlNode s in scripts)
                            {
                                var iPath = Path.Combine(_rootDirectory, s.InnerText);
                                var iData = File.ReadAllText(iPath);
                                outStyles += "\n<script>\n" + iData + "\n</script>\n";
                            }

                            output = output.Replace("<inlines-go-here></inlines-go-here>", outStyles + outScripts);
                        }

                        input = new MemoryStream(Encoding.UTF8.GetBytes(output));
                    }
                    else
                    {
                        if (Platform.isMAUI)
                        {
                            //MAUI Needs It's Own File Protocol
                            var fs = MAUIHelperLoader.Current.GetFileSystem();
                            input = await fs.OpenAppPackageFileAsync(Path.Combine("WWW", rawFilename));
                        }
                        else
                        {
                            input = new FileStream(filename, FileMode.Open, FileAccess.Read);
                        }

                    }



                    //Adding permanent http response headers
                    if (input.CanSeek)
                    {
                        context.Response.ContentLength64 = input.Length;
                    }

                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                    context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(filename).ToString("r"));

                    byte[] buffer = new byte[1024 * 32];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, nbytes);
                    input.Close();
                    context.Response.OutputStream.Flush();

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

            }
            else
            {
                Logger.LogError("File Not Found: " +  filename);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Close();
        }

        private void Initialize(string path, int port)
        {
            Logger.LogInfo("Found Port To Listen On: " + port);
            _rootDirectory = path;
            _port = port;
            _serverThread = new Thread(Listen);
            _serverThread.Start();
        }


    }

    /// <summary>
    /// Parameters thatr are passed to the thread method
    /// </summary>
    public class ServerStartParameters
    {
        public string Path { get; set; }
        public int Port { get; set; }

        /// <summary>
        ///  Any object that implements a Process method
        ///  method should return true (request is handled) 
        /// or false (to fall through and handle as files)
        /// </summary>
        public object RequestHandler { get; set; }
    }

}