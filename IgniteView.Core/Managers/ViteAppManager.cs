using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

            if (DebugMode.IsDebugMode)
            {
                CurrentServerManager.BaseURL = "http://localhost:5173"; // Vite dev URL
            }

            Init(GetImplicitIdentity());
        }

        protected override void Init(AppIdentity identity)
        {
            base.Init(identity);
        }
    }
}
