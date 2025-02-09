using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace IgniteView.Core
{
    public class WWWRootFileResolver : FileResolver
    {
        public string WWWRootPath;

        public WWWRootFileResolver()
        {
            if (Directory.Exists(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "wwwroot"))) { WWWRootPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "wwwroot"); }
            else if (Directory.Exists(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "WWW"))) { WWWRootPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "WWW"); }
            else
            {
                throw new DirectoryNotFoundException("Couldn't find the 'wwwroot' folder in the app's directory, make sure it exists!");
            }
        }

        public override bool DirectSetup(Webserver server)
        {
            server.Routes.PreAuthentication.Content.BaseDirectory = WWWRootPath;
            server.Routes.PreAuthentication.Content.Add("/", true);

            return true;
        }

        public override string GetIndexFile()
        {
            if (File.Exists(Path.Join(WWWRootPath, "index.html"))) { return "/index.html"; }
            else if (File.Exists(Path.Join(WWWRootPath, "index.htm"))) { return "/index.htm"; }
            else if (File.Exists(Path.Join(WWWRootPath, "default.html"))) { return "/default.html"; }
            else if (File.Exists(Path.Join(WWWRootPath, "default.htm"))) { return "/default.htm"; }

            throw new FileNotFoundException("Couldn't find an 'index.html' file in your wwwroot folder!");
        }

        public override string[] GetFiles() => throw new NotImplementedException("Manual resolving is disabled on this resolver");
    }
}
