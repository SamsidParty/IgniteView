﻿using System;
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
using System.Threading;

namespace IgniteView.Core
{
    public class ServerManager
    {
        public Webserver CurrentServer;
        public FileResolver Resolver;

        #region Networking

        /// <summary>
        /// Gets the URL where the server is listening (excluding a trailing slash)
        /// </summary>
        public string BaseURL
        {
            get
            {
                if (string.IsNullOrEmpty(_BaseURL))
                {
                    return "http://127.0.0.1:" + CurrentServer.Settings.Port;
                }
                
                return _BaseURL;
            }
            set
            {
                _BaseURL = value;
            }
        }

        string _BaseURL;

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
        /// Checks if a port is open on the local machine
        /// </summary>
        public static bool IsPortOpen(int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect("localhost", port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
                    client.EndConnect(result);
                    return success;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Starts the web server
        /// </summary>
        public void Start()
        {
            WebserverSettings settings = new WebserverSettings("127.0.0.1", GetFreePort());
            CurrentServer = new Webserver(settings, DefaultRoute);

            CurrentServer.Routes.PreAuthentication.Dynamic.Add(WatsonWebserver.Core.HttpMethod.GET, new Regex(".*"), ResolverRoute);
            CurrentServer.Routes.PreAuthentication.Static.Add(WatsonWebserver.Core.HttpMethod.GET, "/igniteview/injected.js", InjectedJSRoute);

            // Tells the JS code what URL to use (in case it's coming from another origin)
            AppManager.Instance.OnBeforeMainWindowCreated += () =>
            {
                AppManager.Instance.RegisterPreloadScriptFromString(new JSAssignment("igniteView.resolverURL", BaseURL));
            };

            CurrentServer.Start();
        }

        public ServerManager(FileResolver resolver)
        {
            Resolver = resolver;
            Start();
        }

        #endregion

        #region Routing

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
            await ctx.Response.Send(InjectedScript.CombinedScriptData);
        }

        async Task HTMLInjectorRoute(HttpContextBase ctx)
        {
            // Read the html file content
            var htmlContent = Resolver.ReadFileAsText(ctx.Request.Url.RawWithoutQuery);

            var injectedCode = "<script src=\"/igniteview/injected.js\" ></script>"; // Loads a script from InjectedJSRoute (the function above)

            injectedCode += "</head>";

            // Check whether the script injection mode is server side
            if (PlatformManager.Instance.GetScriptInjectionMode() == ScriptInjectionMode.ServerSide)
            {
                htmlContent = htmlContent.Replace("</head>", injectedCode); // Adds the code inside the head
            } // Otherwise it will be injected on the client
            

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

        #endregion

        #region User Dynamic Routing

        /// <summary>
        /// Allows you to register a custom file route to return dynamic data easily.
        /// </summary>
        /// <param name="relativeURL">The relative URL of the route (eg. "/hello.txt")</param>
        /// <param name="route">The function called when the route is navigated to</param>
        public void RegisterDynamicFileRoute(string relativeURL, Func<HttpContextBase, Task> route)
        {
            // Always have a leading "/"
            if (!relativeURL.StartsWith("/"))
            {
                relativeURL = "/" + relativeURL;
            }

            // Make sure the server is running
            if (CurrentServer == null || !CurrentServer.IsListening) {
                throw new Exception("The IgniteView file server is not currently running! Please make sure the app has initialized first.");
            }

            CurrentServer.Routes.PreAuthentication.Static.Add(WatsonWebserver.Core.HttpMethod.GET, "/dynamic" + relativeURL, route);
        }

        #endregion
    }
}
