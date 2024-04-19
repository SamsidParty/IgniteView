﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public static OperatingMode GetOperatingMode()
        {
            if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
            {

                try
                {
                    var testFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
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
