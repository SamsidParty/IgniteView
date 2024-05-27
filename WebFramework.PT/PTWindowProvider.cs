using PhotinoNET;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework.PT
{
    public class PTWindowProvider
    {
        public static Dictionary<string, IntPtr> LibCache = new Dictionary<string, IntPtr>();

        public static void Activate()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                MacHelperLoader.Current = Activator.CreateInstance(typeof(MacHelper));
                MacHelperLoader.Current.OnLoad();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WinHelperLoader.Current = Activator.CreateInstance(typeof(WinHelper));
                WinHelperLoader.Current.OnLoad();
            }

            AppManager.WindowToUse = typeof(PTWebWindow);
            ExtractDependencies();
            NativeLibrary.SetDllImportResolver(typeof(PhotinoWindow).Assembly, ImportResolver);
        }

        public static IntPtr GetLib(string lib)
        {
            Logger.LogInfo("Loading Lib From " + lib);

            if (LibCache.ContainsKey(lib)) { return LibCache[lib]; }

            LibCache[lib] = NativeLibrary.Load(lib);
            return LibCache[lib];
        }

        public static void ExtractDependencies()
        {
            var runtimePath = AppManager.GetRuntimePath();
            if (!Directory.Exists(runtimePath))
            {
                Directory.CreateDirectory(runtimePath);
                Logger.LogInfo("Extracting Dependencies...");
                var archive = new ZipArchive(new MemoryStream(Properties.Resources.runtimes));
                archive.ExtractToDirectory(Directory.GetParent(runtimePath).FullName);
            }
        }

        static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            var runtimePath = AppManager.GetRuntimePath();
            var suffix = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "-arm64" : "-x64";

            IntPtr libHandle = IntPtr.Zero;
            if (libraryName == "Photino.Native" || libraryName == "IgniteViewMac" || libraryName.Contains("IVPlugin"))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    libHandle = GetLib(Path.Combine(runtimePath, "win" + suffix, "native", libraryName + ".dll"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libHandle = GetLib(Path.Combine(runtimePath, "osx" + suffix, "native", libraryName + ".dylib"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libHandle = GetLib(Path.Combine(runtimePath, "linux" + suffix, "native", libraryName + ".so"));
                }
            }
            return libHandle;
        }
    }
}
