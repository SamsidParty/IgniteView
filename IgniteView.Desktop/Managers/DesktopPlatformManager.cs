using IgniteView.Core;
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

        /// <summary>
        /// Tells IgniteView.Core to use the desktop runtime and facilitates loading of native code
        /// </summary>
        public static void Activate() => Activate(Assembly.GetCallingAssembly());


        /// <summary>
        /// Tells IgniteView.Core to use the desktop runtime and facilitates loading of native code.
        /// This overload takes an extra Assembly parameter, which is used for the native import resolver.
        /// </summary>
        public static void Activate(Assembly entryAssembly)
        {
            PlatformManager.PlatformHints.Add("desktop");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                PlatformManager.PlatformHints.Add("win32");
            }
            
            
            // Set the environment variables for webview2
            Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00FFFFFF");

            NativeLibrary.SetDllImportResolver(typeof(DesktopWebWindow).Assembly, ImportResolver);
            NativeLibrary.SetDllImportResolver(entryAssembly, ImportResolver);
            Instance = new DesktopPlatformManager();
        }

        #region Import Resolving

        static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            var runtimePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "iv2runtime");
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
                    // Suffix is ignored on mac, since we can use universal dylibs
                    libHandle = LoadLibrary(Path.Combine(runtimePath, "osx-universal", "native", "lib" + libraryName + ".dylib"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libHandle = LoadLibrary(Path.Combine(runtimePath, "linux" + suffix, "native", "lib" + libraryName + ".so"));
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
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new MacWebWindow();
            }
            return new DesktopWebWindow();
        }
        public override void Run() => RunApp();
        public override void Create() => CreateApp(AppManager.Instance.CurrentIdentity.IDString);

        public override ScriptInjectionMode GetScriptInjectionMode() => ScriptInjectionMode.ClientSide;
        public override ServerListenMode GetServerListenMode() => ServerListenMode.Http;
    }
}
