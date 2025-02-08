using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;
using WatsonWebserver.Core;

namespace IgniteView.Core
{
    public class ServerManager
    {
        public Webserver CurrentServer;

        /// <summary>
        /// Gets the URL where the server is listening (excluding a trailing slash)
        /// </summary>
        public string BaseURL
        {
            get
            {
                return "http://127.0.0.1:" + CurrentServer.Settings.Port;
            }
        }


        /// <summary>
        /// Finds a free port and returns it
        /// </summary>
        public static int GetFreePort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <summary>
        /// Starts the web server
        /// </summary>
        public void Start()
        {
            WebserverSettings settings = new WebserverSettings("127.0.0.1", GetFreePort());
            CurrentServer = new Webserver(settings, DefaultRoute);
            CurrentServer.Start();
        }

        public ServerManager()
        {
            Start();
        }

        static async Task DefaultRoute(HttpContextBase ctx) => await ctx.Response.Send("Hello from the default route!");
    }
}
