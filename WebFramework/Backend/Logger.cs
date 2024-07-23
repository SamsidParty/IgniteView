using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.Backend
{
    public class Logger
    {
        internal static Stream LogStream = null;
        internal static bool LogToConsole = false;

        /// <summary>
        /// If Enabled, Log Info Messages Will Include A ms Field, Representing The Time In MS Since The First Log
        /// </summary>
        public static bool EnableTimer = false;
        internal static Stopwatch TimeMeasure;


        #region WinAPI

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        #endregion

        /// <summary>
        /// Sets The File Name To Write Logs To (Just The Name, Excluding Extension, No Slashes)
        /// The File Will Be Stored In LocalAppData
        /// </summary>
        public static async Task SetFileName(string name)
        {
            try
            {
                if (!Platform.isMAUI)
                {
                    if (SharedIO.File == null)
                    {
                        //For Programs That Don't Use IgniteView (Like Background Services) 
                        var file = new SharedIOFile();
                        LogStream = await file.GetWriteStream(Path.Combine(await file.GetAppdataDirectory(), name + ".ivlog"));
                    }
                    else
                    {
                        LogStream = await SharedIO.File.GetWriteStream(Path.Combine(await SharedIO.File.GetAppdataDirectory(), name + ".ivlog"));
                    }
                    
                }

            }
            catch {
                
            }

            LogRaw("\n\n\n\n------ New Log Start ------\n\n\n\n");
        }


        /// <summary>
        /// Windows Only: Opens The Console Window
        /// Warning: Must Be Called Before Any Console.WriteLine Statements In The Program
        /// </summary>
        public static void ForceOpenConsole()
        {
            LogToConsole = true;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Platform.isWindowsPT)
            {
                AllocConsole();
            }
        }

        public static void CloseLog()
        {
            if (LogStream != null)
            {
                LogStream.Close();
                LogStream = null;
            }
        }

        public static void LogRaw(string log)
        {
            if (LogToConsole)
            {
                Console.WriteLine(log);
            }
            Debug.WriteLine(log);
            Trace.WriteLine(log);

            if (LogStream != null)
            {
                LogStream.Write(Encoding.UTF8.GetBytes(log + "\n"), 0, log.Length + 1);
            }
        }

        public static void LogInfo(string log)
        {
            if (EnableTimer)
            {
                if (TimeMeasure == null) { TimeMeasure = Stopwatch.StartNew(); }
                log = "[" + TimeMeasure.ElapsedMilliseconds + "MS] " + log;
            }
            LogRaw("[INFO] " + log);
        }

        public static void LogError(string log)
        {
            LogRaw("\x1b[31m[ERROR]\x1b[0m " + log);
        }

        public static void LogWarning(string log)
        {
            LogRaw("\x1b[33m[WARNING]\x1b[0m " + log);
        }
    }
}
