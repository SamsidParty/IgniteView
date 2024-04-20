using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public enum OperatingMode
    {
        Headless, // Don't Create A UI
        DesktopDynamic, // Dynamically Loaded DLL/SO/DYLIB
        DesktopDynamicReadOnly, // Dynamically Loaded DLL/SO/DYLIB, But Saved In A Location Outside The App's Directory
    }

    public class Platform
    {

        public static bool IsNotUWP {
            get {
                return !AppDomain.CurrentDomain.BaseDirectory.Contains("WindowsApps");
            }
        }

        public static bool isMAUI
        {
            get
            {
                return (Type.GetType("WebFramework.MAUI.MAUIHelper, WebFramework.MAUI") != null);
            }
        }

        public static OperatingMode GetOperatingMode()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {

                try
                {
                    var testFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helpers.SharedRandom.Next(0, 100000) + ".txt" + Helpers.SharedRandom.Next(0, 100000));
                    File.WriteAllText(testFile, "Installation Folder Is Not Read-Only");
                    File.Delete(testFile);
                }
                catch {
                    return OperatingMode.DesktopDynamicReadOnly;
                }

                return OperatingMode.DesktopDynamic;
            }

            return OperatingMode.Headless;
        }
    }
}
