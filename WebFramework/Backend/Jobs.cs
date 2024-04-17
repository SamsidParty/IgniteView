using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebFramework
{
    
    public class Jobs {

        public static List<Action> Queue = new List<Action>();

        public static void Push(Action j, DOM ctx) {
            Queue.Add(j);
            Fire(ctx);
        }

        //Invokes The Jobs On The UI Thread
        public static void Pop() {
            foreach (var j in Queue){
                j.Invoke();
            }
            Queue.Clear();
        }

        public static void Fire(DOM ctx){
            ctx.Window.ExecuteJavascript("JSI_Send('firejobs');");
        }

    }

}
