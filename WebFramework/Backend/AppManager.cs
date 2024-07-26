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
        public static string Publisher;
        public static string AppID;

        public static AppManager Instance;
        public static Type WindowToUse; // Called By WindowProvider

        public static WebWindow GetWebWindow()
        {
            if (WindowToUse == null)
            {
                throw new NullReferenceException("No Suitable Window Provider Was Found");
            }

            Logger.LogInfo("Found Window To Use: " + WindowToUse.Name);
            return Activator.CreateInstance(WindowToUse) as WebWindow;
        }

        /// <summary>
        /// Initializes The App And Fills In Platform-Specific Info
        /// </summary>
        public static void Validate(string[] args, string publisher, string appID)
        {
            Publisher = publisher;
            AppID = appID;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Platform.isWindowsPT) 
            {
                //Needed For Photino Cache
                var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppManager.Publisher, AppManager.AppID);
                if (!Directory.Exists(appDir))
                {
                    Directory.CreateDirectory(appDir);
                }
            }

            SharedIO.FindAndLoad();

            Logger.LogInfo("App Validation Complete");

            IsValid = true;
        }

        /// <summary>
        /// Called By Platform Providers To Get The URL To Display
        /// </summary>
        /// <returns></returns>
        public static string GetMainURL()
        {
            if (!String.IsNullOrEmpty(DevTools.OverridenURL))
            {
                return DevTools.OverridenURL;
            }
            else
            {
                return "http://localhost:" + Server.HTTPPort + "/index.html";
            }
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
            Logger.CloseLog();
            WindowManager.MainWindow.Close();
        }

        public static string GetRuntimePath()
        {
            var runtimeID = "ivruntime4"; // TODO: Make This Less Hardcoded

            var opMode = Platform.GetOperatingMode();
            if (opMode == OperatingMode.DesktopDynamic)
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, runtimeID);
            }

            //If App Directory Is Read Only
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IgniteView", runtimeID);

        }


    }
}
