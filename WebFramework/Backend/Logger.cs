using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.Backend
{
    public class Logger
    {
        internal static Stream LogStream = null;

        /// <summary>
        /// Sets The File Name To Write Logs To (Just The Name, Excluding Extension, No Slashes)
        /// The File Will Be Stored In LocalAppData
        /// </summary>
        public static async Task SetFileName(string name)
        {
            try
            {
                LogStream = await SharedIO.File.GetWriteStream(Path.Combine(await SharedIO.File.GetAppdataDirectory(), name + ".ivlog"));
            }
            catch {
                LogStream = await SharedIO.File.GetWriteStream(Path.Combine(await SharedIO.File.GetAppdataDirectory(), DateTime.UtcNow + ".ivlog"));
            }

            LogRaw("\n\n\n\n------ New Log Start ------\n\n\n\n");
        }

        public static void CloseLog()
        {
            if (LogStream != null)
            {
                LogStream.Close();
            }
        }

        public static void LogRaw(string log)
        {
            Console.WriteLine(log);
            Debug.WriteLine(log);

            if (LogStream != null)
            {
                LogStream.Write(Encoding.UTF8.GetBytes(log + "\n"), 0, log.Length + 1);
            }
        }

        public static void LogInfo(string log)
        {
            LogRaw("[INFO] " + log);
        }

        public static void LogError(string log)
        {
            LogRaw("[ERROR] " + log);
        }
    }
}
