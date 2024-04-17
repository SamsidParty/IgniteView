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
using PhotinoNET;

namespace WebFramework
{
    public class AppManager
    {
        public static bool IsValid;
        public static string Location;
        public static Func<WebWindow, Task> OnReady;


        /// <summary>
        /// Makes Sure The App Can Run In It's Current State
        /// </summary>
        public static void Validate(string[] args)
        {
            ExtractDependencies();

            if (ExecFunction.IsExecFunctionCommand(args))
            {
                var returnCode = ExecFunction.Program.Main(args);
                Environment.Exit(returnCode);

                Thread.Sleep(1024*1024);
            }

            IsValid = true;
        }

        public static async Task Start(string location, Func<WebWindow, Task> onReady)
        {

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

        public static string GetRuntimePath()
        {
            if (AppDomain.CurrentDomain.BaseDirectory.Contains("WindowsApps"))
            {
                //MSIX Mode
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IgniteView", "runtimes");
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes");
        }

        public static void ExtractDependencies()
        {
            var runtimePath = GetRuntimePath();
            if (!Directory.Exists(runtimePath))
            {
                Directory.CreateDirectory(runtimePath);
                Console.WriteLine("Extracting Dependencies...");
                var archive = new ZipArchive(new MemoryStream(Properties.Resources.runtimes));
                archive.ExtractToDirectory(Directory.GetParent(runtimePath).FullName);
            }

            NativeLibrary.SetDllImportResolver(typeof(PhotinoWindow).Assembly, ImportResolver);
        }

        static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            var runtimePath = GetRuntimePath();
            var suffix = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "-arm64" : "-x64";

            IntPtr libHandle = IntPtr.Zero;
            if (libraryName == "Photino.Native" || libraryName == "IgniteViewMac" || libraryName.Contains("IVPlugin"))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.WriteLine("Loading Lib From " + Path.Combine(runtimePath, "win" + suffix, "native", libraryName + ".dll"));
                    libHandle = NativeLibrary.Load(Path.Combine(runtimePath, "win" + suffix, "native", libraryName + ".dll"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libHandle = NativeLibrary.Load(Path.Combine(runtimePath, "osx" + suffix, "native", libraryName + ".dylib"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libHandle = NativeLibrary.Load(Path.Combine(runtimePath, "linux" + suffix, "native", libraryName + ".so"));
                }
            }
            return libHandle;
        }

        public static void RunGTK()
        {
            HelperWindow.Main();
        }

    }
}
