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

        [DllImport("IgniteView.Desktop.Native")]
        static extern void CreateApp();

        [DllImport("IgniteView.Desktop.Native")]
        static extern void RunApp();

        #endregion

        public static void Activate()
        {
            NativeLibrary.SetDllImportResolver(typeof(DesktopWebWindow).Assembly, ImportResolver);
            Instance = new DesktopPlatformManager();

            CreateApp();
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
                    libHandle = LoadLibrary(Path.Combine(runtimePath, "osx" + suffix, "native", libraryName + ".dylib"));
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

        public override WebWindow CreateWebWindow() => new DesktopWebWindow();
        public override void Run() => RunApp();
    }
}
