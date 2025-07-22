using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs;

internal class ImportResolver
{
    internal static bool IsInitialized = false;

    internal static bool Initialize()
    {
        if (!IsInitialized)
        { 
            NativeLibrary.SetDllImportResolver(typeof(FileDialog).Assembly, ImportResolverFunction);
            NativeFunctions.NFD_Init();
            IsInitialized = true;
        }

        return IsInitialized;
    }

    static IntPtr ImportResolverFunction(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        var runtimePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "iv2runtime");
        var suffix = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "-arm64" : "-x64";

        IntPtr libHandle = IntPtr.Zero;
        if (libraryName == "nfd")
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

    static IntPtr LoadLibrary(string lib)
    {
        return NativeLibrary.Load(lib);
    }
}
