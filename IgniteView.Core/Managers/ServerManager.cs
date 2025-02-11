using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;
using WatsonWebserver.Core;
using System.Text.RegularExpressions;
using MimeMapping;
using System.Net.Http;

namespace IgniteView.Core
{
    public class ServerManager
    {
        public Webserver CurrentServer;
        public FileResolver Resolver;

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

            CurrentServer.Routes.PreAuthentication.Dynamic.Add(WatsonWebserver.Core.HttpMethod.GET, new Regex(".*"), ResolverRoute);
            CurrentServer.Routes.PreAuthentication.Static.Add(WatsonWebserver.Core.HttpMethod.GET, "/igniteview/injected.js", InjectedJSRoute);

            CurrentServer.Start();
        }

        public ServerManager()
        {
            Resolver = new DirectoryFileResolver();
            Start();
        }

        async Task DefaultRoute(HttpContextBase ctx) => ctx.Response.Headers.Set("Location", Resolver.GetIndexFile());

        async Task RedirectToRoot(HttpContextBase ctx)
        {
            ctx.Response.StatusCode = 307; // Temporary redirect
            ctx.Response.Headers.Add("Location", Resolver.GetIndexFile());
            await ctx.Response.Send("");
        }

        async Task InjectedJSRoute(HttpContextBase ctx)
        {
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = "text/javascript";
            await ctx.Response.Send(InjectedScript.ScriptData);
        }

        async Task HTMLInjectorRoute(HttpContextBase ctx)
        {
            // Read the html file content
            var fileStream = Resolver.OpenFileStream(ctx.Request.Url.RawWithoutQuery);
            var reader = new StreamReader(fileStream);
            var htmlContent = reader.ReadToEnd();
            await fileStream.DisposeAsync();

            var injectedCode = "<script src=\"/igniteview/injected.js\" ></script></head>"; // Loads a script from InjectedJSRoute (the function above)
            htmlContent = htmlContent.Replace("</head>", injectedCode); // Adds the code inside the head

            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = "text/html";
            await ctx.Response.Send(htmlContent);
        }

        async Task ResolverRoute(HttpContextBase ctx)
        {
            var relativePath = ctx.Request.Url.RawWithoutQuery;

            // Redirect requests to "/" to "index.html" (or whatever the resolver wants us to redirect to)
            if (relativePath == "/")
            {
                await RedirectToRoot(ctx);
                return;
            }
            else if (relativePath.ToLower().EndsWith(".html") || relativePath.ToLower().EndsWith(".htm"))
            {
                // HTML files need to be injected with custom javascript code
                await HTMLInjectorRoute(ctx);
                return;
            }

            if (Resolver.DoesFileExist(relativePath)) {
                // Read the file and send to the client
                var fileStream = Resolver.OpenFileStream(relativePath);

                ctx.Response.StatusCode = 200;
                ctx.Response.ContentLength = fileStream.Length;
                ctx.Response.ContentType = MimeUtility.GetMimeMapping(Path.GetExtension(relativePath));
                await ctx.Response.Send(fileStream.Length, fileStream);

                await fileStream.DisposeAsync();
            }

            ctx.Response.StatusCode = 404;
            await ctx.Response.Send("404 Not Found");
        }
    }
}
