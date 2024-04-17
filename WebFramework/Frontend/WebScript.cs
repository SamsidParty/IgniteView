using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public class WebScript
    {
        public static Dictionary<string, Type> RegisteredScripts = new Dictionary<string, Type>();

        public DOM Document;

        public static void Register<T>(string scriptname) where T : WebScript
        {
            RegisteredScripts[scriptname] = typeof(T);
        }

        public virtual async Task DOMContentLoaded()
        {

        }
    }
}
