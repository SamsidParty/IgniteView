using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace IgniteView.Core
{
    public abstract class FileResolver
    {
        /// <summary>
        /// Gets the relative path of the index file (eg. "/index.html")
        /// </summary>
        public abstract string GetIndexFile();

        /// <summary>
        /// Returns an array of relative paths of all the files present (eg. ["/index.html", "/JS/main.js", "/CSS/main.css"]).
        /// Only used during manual resolving (if DirectSetup() returns false)
        /// </summary>
        public abstract string[] GetFiles();

        /// <summary>
        /// Allows the resolver to setup the web server to work with the resolver, return true if the server should ignore manual resolving
        /// </summary>
        public abstract bool DirectSetup(Webserver server);
    }
}
