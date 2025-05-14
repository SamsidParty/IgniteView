using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver.Core;

namespace IgniteView.Core
{
    internal class DynamicRoutes
    {
        public static async Task StreamedCommandRoute(HttpContextBase ctx)
        {
            var commandString = Encoding.UTF8.GetString(Convert.FromBase64String(ctx.Request.Query.Querystring));
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
    }
}
