using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class ViteAppManager : AppManager
    {
        public Process ViteProcess;

        /// <summary>
        /// Uses the data from the package.json file to create an implicit AppIdentity
        /// </summary>
        AppIdentity GetImplicitIdentity()
        {
            // Find the igniteview_package.json file from the resolver
            if (!CurrentServerManager?.Resolver?.DoesFileExist("/igniteview_package.json") ?? false)
            {
                throw new FileNotFoundException("IgniteView couldn't find the package.json file in the vite output. Please ensure your project has a valid package.json file.");
            }

            dynamic packageJson;
            packageJson = JsonConvert.DeserializeObject<ExpandoObject>(CurrentServerManager!.Resolver.ReadFileAsText("/igniteview_package.json"))!;

            var packageName = packageJson.name;

            // In case the name of the app hasn't been changed from "src-vite", use the assembly name
            if (string.IsNullOrWhiteSpace(packageName) || packageName == "src-vite") {
                packageName = Assembly.GetEntryAssembly()!.GetName().Name;
            }

            return new AppIdentity(packageName, "IgniteViewApp");
        }

        void SetEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("IGNITEVIEW_RESOLVER_URL", CurrentServerManager.BaseURL);
        }

        void RunVite()
        {
            SetEnvironmentVariables();

            // Try to read the vite dev path from the file
            var viteDevPath = CurrentServerManager.Resolver.ReadFileAsText("/.vitedev").Trim();

            if (!Directory.Exists(viteDevPath)) { return; }

            var vitePort = ServerManager.GetFreePort();

            // Start the vite process
            var nodeBinary = "node";
            var viteJS = Path.Join(viteDevPath, "node_modules", "vite", "bin", "vite.js");

            if (!File.Exists(viteJS)) { return; }

            var psi = new ProcessStartInfo(nodeBinary, new string[] { viteJS, ".", "--port", vitePort.ToString(), "--strictPort" })
            {
                WorkingDirectory = viteDevPath,
                UseShellExecute = false
            };

            ViteProcess = Process.Start(psi);

            if ((PlatformManager.HasPlatformHint("win32")))
            {
                // Windows Only; Attach the vite process to this process so it closes when this process closes
                // This is useful because when pressing the stop button in Visual Studio, we can't run OnCleanUp
                ChildProcessTracker.AddProcess(ViteProcess);
            }
            else
            {
                // Close the vite process when the app closes
                OnCleanUp += () =>
                {
                    if (ViteProcess != null && !ViteProcess.HasExited)
                    {
                        ViteProcess.Kill();
                    }
                };
            }


            // Wait until vite starts
            while (!ServerManager.IsPortOpen(vitePort))
            {
                Thread.Sleep(250);
            }

            CurrentServerManager.BaseURL = "http://localhost:" + vitePort;

        }

        public ViteAppManager([CallerFilePath] string currentDirectory = "") : base(null) {
            CurrentServerManager = new ServerManager(CreateFileResolver());

            if (CurrentServerManager.Resolver.DoesFileExist("/.vitedev") && PlatformManager.Instance.GetType().Name == "DesktopPlatformManager")
            {
                RunVite();
            }

            Init(GetImplicitIdentity());
        }

        protected override void Init(AppIdentity identity)
        {
            base.Init(identity);
        }
    }
}
