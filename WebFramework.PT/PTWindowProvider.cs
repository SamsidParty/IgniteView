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
        public static void Activate()
        {
            ExtractDependencies();
            NativeLibrary.SetDllImportResolver(typeof(PhotinoWindow).Assembly, ImportResolver);
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
                    Logger.LogInfo("Loading Lib From " + Path.Combine(runtimePath, "win" + suffix, "native", libraryName + ".dll"));
                    libHandle = NativeLibrary.Load(Path.Combine(runtimePath, "win" + suffix, "native", libraryName + ".dll"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Logger.LogInfo("Loading Lib From " + Path.Combine(runtimePath, "osx" + suffix, "native", libraryName + ".dylib"));
                    libHandle = NativeLibrary.Load(Path.Combine(runtimePath, "osx" + suffix, "native", libraryName + ".dylib"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libHandle = NativeLibrary.Load(Path.Combine(runtimePath, "linux" + suffix, "native", libraryName + ".so"));
                }
            }
            return libHandle;
        }
    }
}
