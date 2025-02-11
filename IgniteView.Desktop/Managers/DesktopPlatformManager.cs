using IgniteView.Core;
using IgniteView.Desktop.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Desktop
{
    public class DesktopPlatformManager : PlatformManager
    {
        #region Native Imports

        [DllImport(InteropHelper.DLLName, CharSet = CharSet.Unicode)]
        static extern void CreateApp(string appID);

        [DllImport(InteropHelper.DLLName)]
        static extern void RunApp();

        #endregion

        public static void Activate()
        {
            // Set the environment variables for webview2
            Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00FFFFFF");

            NativeLibrary.SetDllImportResolver(typeof(DesktopWebWindow).Assembly, ImportResolver);
            Instance = new DesktopPlatformManager();
        }

        

        #region Import Resolving

        static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            var runtimePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "runtimes");
            var suffix = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "-arm64" : "-x64";

            IntPtr libHandle = IntPtr.Zero;
            if (libraryName.Contains(".Native"))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    libHandle = LoadLibrary(Path.Combine(runtimePath, "win" + suffix, "native", libraryName + ".dll"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libHandle = LoadLibrary(Path.Combine(runtimePath, "osx" + suffix, "native", "lib" + libraryName + ".dylib"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libHandle = LoadLibrary(Path.Combine(runtimePath, "linux" + suffix, "native", libraryName + ".so"));
                }
            }
            return libHandle;
        }

        public static IntPtr LoadLibrary(string lib)
        {
            return NativeLibrary.Load(lib);
        }

        #endregion

        public override WebWindow CreateWebWindow() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new Win32WebWindow();
            }
            return new DesktopWebWindow();
        }
        public override void Run() => RunApp();
        public override void Create() => CreateApp(AppManager.Instance.CurrentIdentity.IDString);
    }
}
