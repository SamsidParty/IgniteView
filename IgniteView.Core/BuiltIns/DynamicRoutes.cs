using IgniteView.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WatsonWebserver.Core;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace IgniteView.Core
{
    internal class DynamicRoutes
    {
        // Used when a command returns a Stream object
        public static async Task StreamedCommandRoute(HttpContextBase ctx)
        {
            var commandString = HttpUtility.UrlDecode(ctx.Request.Query.Querystring);
            var commandData = new CommandData(commandString);
            var commandResult = await CommandManager.ExecuteCommand(null, commandData);

            if (commandResult is Stream)
            {
                ctx.Response.StatusCode = 200;
                var stream = (Stream)commandResult;

                if (stream.CanSeek) {
                    await ctx.Response.Send(stream.Length, stream);
                }
                else
                {
                    ctx.Response.ChunkedTransfer = true;
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        var numRead = await stream.ReadAsync(buffer);

                        if (numRead < 1024)
                        {
                            var tempBuffer = new byte[numRead];
                            Array.Copy(buffer, tempBuffer, numRead);
                            await ctx.Response.SendChunk(buffer, true);
                            stream.Close();
                            break;
                        }

                        if (!await ctx.Response.SendChunk(buffer, false))
                        {
                            break;
                        }
                    }
                    
                }
                
                return;
            }

            ctx.Response.StatusCode = 201;
        }

        // Used when a command has a Stream parameter
        public static async Task BlobParameterUploadRoute(HttpContextBase ctx)
        {
            if (ctx.Request.Method != HttpMethod.POST)
            {
                ctx.Response.StatusCode = 405; // Method Not Allowed
                return;
            }

            var blobID = ctx.Request.Query.Elements.Get("blobID");
            var jsBlob = new JSBlob(blobID, ctx.Request.Data);
            await jsBlob.ReadCompletionSource.Task.WaitAsync(CancellationToken.None);
            await ctx.Response.Send();
        }

        // Used to inject code into the webwindow
        public static async Task PreloadInjectionRoute(HttpContextBase ctx)
        {
            if (ctx.Request.Url != null)
            {
                var dynamicScriptData = ScriptManager.CombinedScriptData;

                // LocalStorage
                var host = ctx.Request.Headers.Get("Origin") ?? "default";
                var localStorage = LocalStorage.GetAllItems(host);
                dynamicScriptData += "\n" + (new JSFunctionCall("window._localStorage.hydrate", localStorage)).ToString();

                var bytes = Encoding.UTF8.GetBytes(dynamicScriptData);
                ctx.Response.ContentLength = bytes.Length;
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/javascript";
                await ctx.Response.Send(bytes);
            }

            ctx.Response.StatusCode = 404;
        }
    }
}
