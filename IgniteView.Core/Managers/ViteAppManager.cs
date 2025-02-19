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
        /// <summary>
        /// Uses the data from the package.json file to create an implicit AppIdentity
        /// </summary>
        AppIdentity GetImplicitIdentity()
        {
            return new AppIdentity("SamsidParty", "IgniteView Example");
        }


        public ViteAppManager([CallerFilePath] string currentDirectory = "") : base(null) {
            CurrentServerManager = new ServerManager(CreateFileResolver());

            if (CurrentServerManager.Resolver.DoesFileExist("/.vitedev"))
            {
                // Try to read the vite dev path from the file
                var viteDevPath = CurrentServerManager.Resolver.ReadFileAsText("/.vitedev").Trim();

                if (Directory.Exists(viteDevPath))
                {
                    var vitePort = ServerManager.GetFreePort();

                    // Start the vite process
                    var npx = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "npx.cmd" : "npx";
                    var psi = new ProcessStartInfo(npx, new string[] { "vite", ".", "--port", vitePort.ToString() })
                    {
                        WorkingDirectory = viteDevPath,
                        UseShellExecute = true
                    };

                    var vite = Process.Start(psi);

                    // Wait until vite starts
                    while (!ServerManager.IsPortOpen(vitePort)) {
                        Thread.Sleep(250);
                    }

                    CurrentServerManager.BaseURL = "http://localhost:" + vitePort;
                }

                
            }

            Init(GetImplicitIdentity());
        }

        protected override void Init(AppIdentity identity)
        {
            base.Init(identity);
        }
    }
}
