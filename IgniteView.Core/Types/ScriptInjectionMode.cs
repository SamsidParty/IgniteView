using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Represents the mode in which scripts are injected into the webview
    /// </summary>
    public enum ScriptInjectionMode
    {
        /// <summary>
        /// Scripts are injected into the HTML content on the server side
        /// </summary>
        ServerSide,
        /// <summary>
        /// Scripts are injected via the webview's native APIs
        /// </summary>
        ClientSide
    }
}
