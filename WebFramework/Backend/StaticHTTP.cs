//Modified From https://gist.github.com/abc126/40906c32d4f076ab26e1228294638339


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
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] listeners = properties.GetActiveTcpListeners();
            int[] openPorts = listeners.Select(item => item.Port).ToArray<int>();
            return openPorts.All(openPort => openPort != port);
        }

        //Find An Empty Port Recursively
        static int EmptyPort()
        {
            try
            {
                var port = Random.Shared.Next(2048, 65500);

                if (port < 10081) // Chrome Doesn't Like Some Ports (ERR_UNSAFE_PORT)
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

        /// <summary>
        /// Construct server with suitable port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        public SimpleHttpServer(string path)
        {
            //get an empty port
            int port = EmptyPort();

            Initialize(path, port);
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
            _listener.Prefixes.Add("http://localhost:" + _port.ToString() + "/");
            _listener.Prefixes.Add("http://127.0.0.1:" + _port.ToString() + "/");
            _listener.Start();

            Started = true;

            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }
        }


        /// <summary>
        /// Process an individual request. Handles only static file based requests
        /// </summary>
        /// <param name="context"></param>
        private void Process(HttpListenerContext context)
        {
            string filename = context.Request.Url.AbsolutePath;


            if (RequestHandler != null)
            {

            }

            filename = filename.Substring(1);

            if (string.IsNullOrEmpty(filename))
            {
                foreach (string indexFile in DefaultDocuments)
                {
                    if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
                    {
                        filename = indexFile;
                        break;
                    }
                }
            }

            filename = Path.Combine(_rootDirectory, filename);

            if (File.Exists(filename))
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
                        var injection = "<script>/* Injection For Interop */\r\n window.external = window.external || { receiveMessage: () => {}, sendMessage: () => {} }; \r\n window.external.receiveMessage(message => eval.call(window, message));\r\n addEventListener(\"DOMContentLoaded\", () => { window.external.sendMessage(`{\"Type\":\"load\"}`);\r\n });</script>\r\n";
                        var output = injection + File.ReadAllText(filename);

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
                        input = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    }



                    //Adding permanent http response headers
                    context.Response.ContentLength64 = input.Length;
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
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Close();
        }

        private void Initialize(string path, int port)
        {
            Logger.LogInfo("Starting HTTP Server On Port: " + port);
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