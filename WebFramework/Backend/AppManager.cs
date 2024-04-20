using IgniteView.Dispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;
using WebFramework.Backend;
using System.IO;
using System.Threading;

namespace WebFramework
{
    public class AppManager
    {
        public static bool IsValid;
        public static string Location;
        public static Func<WebWindow, Task> OnReady;

        public static AppManager Instance;

        public static WebWindow GetWebWindow()
        {
            //Find First Class That Inherits WebWindow
            Type type = null;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var possibleTypes = asm.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(WebWindow)));
                if (possibleTypes.Count() > 0)
                {
                    type = possibleTypes.First();
                    Logger.LogInfo("Found Window Provider To Use: " + type.FullName);
                }
            }

            if (type == null)
            {
                Logger.LogError("No Suitable Window Provider Was Found");
                throw new Exception("No Suitable Window Provider Was Found");
            }

            return Activator.CreateInstance(type) as WebWindow;
        }

        /// <summary>
        /// Makes Sure The App Can Run In It's Current State
        /// </summary>
        public static void Validate(string[] args)
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                MacHelperLoader.FindAndLoad(); // Load MacHelper From WebFramework.PT
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Platform.isWindowsPT) // Simple Way Of Checking If We Are Win32 Or UWP
            {
                UWPHelperLoader.FindAndLoad();
            }
            else if (Platform.isMAUI)
            {
                MAUIHelperLoader.FindAndLoad();
            }

            if (ExecFunction.IsExecFunctionCommand(args))
            {
                var returnCode = ExecFunction.Program.Main(args);
                Environment.Exit(returnCode);

                Thread.Sleep(1024*1024);
            }

            Logger.LogInfo("App Validation Complete");

            IsValid = true;
        }

        public static async Task Start(string location, Func<WebWindow, Task> onReady)
        {
            Logger.LogInfo("Starting Application In " + location);

            if (!IsValid)
            {
                throw new InvalidProgramException("The App Was Never Validated, Call AppManager.Validate As Soon As The Program Starts.");
            }

            OnReady = onReady;
            Location = location;

            if (!string.IsNullOrEmpty(DevTools.HotReloadPath))
            {
                Location = DevTools.HotReloadPath;
            }

            await Server.Start();
            await WindowManager.Create(WindowManager.Options);
        }

        public static void Quit()
        {
            WindowManager.MainWindow.Close();
        }

        public static string GetRuntimePath()
        {
            var opMode = Platform.GetOperatingMode();
            if (opMode == OperatingMode.DesktopDynamic)
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes");
            }

            //If App Directory Is Read Only
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IgniteView", "runtimes");

        }


    }
}
