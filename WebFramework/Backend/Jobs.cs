using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebFramework.Backend;

namespace WebFramework
{
    
    public class Jobs {

        public static List<Action> Queue = new List<Action>();

        /// <summary>
        /// Queue A Job To Be Run On The UI Thread
        /// </summary>
        public static void Add(Action j, DOM ctx) {
            Add(j, ctx.Window);
        }

        /// <summary>
        /// Queue A Job To Be Run On The UI Thread
        /// </summary>
        public static void Add(Action j, WebWindow ctx)
        {
            Queue.Add(j);
            RunAllFromRemoteThread(ctx);
        }

        /// <summary>
        /// Invokes Pending Jobs Directly (Must Be Run On The UI Thread)
        /// </summary>
        public static void RunAll() {
            foreach (var j in Queue){
                j.Invoke();
            }
            Queue.Clear();
        }

        /// <summary>
        /// Invokes Pending Jobs On The UI Thread
        /// </summary>
        public static void RunAllFromRemoteThread(DOM ctx){
            RunAllFromRemoteThread(ctx.Window);
        }

        /// <summary>
        /// Invokes Pending Jobs On The UI Thread
        /// </summary>
        public static void RunAllFromRemoteThread(WebWindow ctx)
        {
            if (!ctx.ReadyEventFired) // Wait Until The Window Is Ready
            {
                Task.Run(async () =>
                {
                    while (!ctx.ReadyEventFired)
                    {
                        await Task.Yield();
                    }
                    RunAllFromRemoteThread(ctx);
                });
                return;
            }

            ctx.ExecuteJavascript("JSI_Send('firejobs');");
        }

    }

}
