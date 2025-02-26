using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver.Core;

namespace IgniteView.Core
{
    public class RoutingUtilities
    {
        /// <summary>
        /// Creates a server route that sends a simple string to the client
        /// </summary>
        public static Func<HttpContextBase, Task> TextRoute(string textToSend)
        {
            return async (HttpContextBase ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/plain";
                await ctx.Response.Send(textToSend);
            };
        }

        /// <summary>
        /// Creates a server route that sends a simple string to the client (with custom mime type)
        /// </summary>
        public static Func<HttpContextBase, Task> TextRoute(string textToSend, string mimeType)
        {
            return async (HttpContextBase ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = mimeType;
                await ctx.Response.Send(textToSend);
            };
        }
    }
}
