using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public class DevTools
    {
        public static string HotReloadPath { get; internal set; }
        public static bool Enabled { get; internal set; }
        public static string OverridenURL = null;

        static FileSystemWatcher HotWatcher;
        public static int ForcedPort = 0;

        public static void EnsureNotRunning()
        {
            if (WindowManager.MainWindow != null)
            {
                throw new Exception("Cannot Do This Operation After The App Has Started");
            }
        }

        public static void Enable()
        {
            EnsureNotRunning();
            Enabled = true;
        }

        public static void HotReload(string devPath)
        {
            EnsureNotRunning();
            if (!Enabled) { throw new Exception("DevTools Are Not Enabled, Call DevTools.Enable() First"); }

            //Select Either HTTP Hot Reload
            //Or File System Hot Reload
            if (devPath.StartsWith("http"))
            {
                OverridenURL = devPath;
            }
            else
            {
                HotReloadPath = devPath;
                HotWatcher = new FileSystemWatcher(HotReloadPath);
                HotWatcher.Filter = "*.*";
                HotWatcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size;

                HotWatcher.Changed += HotWatcher_Changed;
                HotWatcher.Created += HotWatcher_Changed;
                HotWatcher.Renamed += HotWatcher_Changed;
                HotWatcher.Deleted += HotWatcher_Changed;
                HotWatcher.IncludeSubdirectories = true;
                HotWatcher.EnableRaisingEvents = true;
            }


        }

        public static void ForcePort(int port)
        {
            EnsureNotRunning();
            if (!Enabled) { throw new Exception("DevTools Are Not Enabled, Call DevTools.Enable() First"); }
            ForcedPort = port;
        }

        private static void HotWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            foreach (var w in WindowManager.OpenWindows)
            {
                w.ExecuteJavascript("location.reload();");
            }
        }
    }
}
