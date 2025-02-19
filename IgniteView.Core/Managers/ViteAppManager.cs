using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            return new AppIdentity("SamsidParty", "IgniteView Example");
        }

        void RunVite()
        {
            // Try to read the vite dev path from the file
            var viteDevPath = CurrentServerManager.Resolver.ReadFileAsText("/.vitedev").Trim();

            if (!Directory.Exists(viteDevPath)) { return; }

            var vitePort = ServerManager.GetFreePort();

            // Start the vite process
            var nodeBinary = "node";
            var viteJS = Path.Join(viteDevPath, "node_modules", "vite", "bin", "vite.js");

            if (!File.Exists(viteJS)) { return; }

            var psi = new ProcessStartInfo(nodeBinary, new string[] { viteJS, ".", "--port", vitePort.ToString() })
            {
                WorkingDirectory = viteDevPath,
                UseShellExecute = false
            };

            ViteProcess = Process.Start(psi);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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

            if (CurrentServerManager.Resolver.DoesFileExist("/.vitedev"))
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
