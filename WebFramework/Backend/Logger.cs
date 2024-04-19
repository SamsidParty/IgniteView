using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.Backend
{
    public class Logger
    {
        public static void LogRaw(string log)
        {
            Console.WriteLine(log);
            Debug.WriteLine(log);
        }

        public static void LogInfo(string log)
        {
            LogRaw("[INFO]" + log);
        }

        public static void LogError(string log)
        {
            LogRaw("[ERROR]" + log);
        }
    }
}
