using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace IgniteView.Core
{
    /// <summary>
    /// Resolves files from a directory
    /// </summary>
    public class DirectoryFileResolver : FileResolver
    {
        public string WWWRootPath;

        /// <summary>
        /// Creates a DirectoryFileResolver that resolves files from the wwwroot/ directory relative to the executable path
        /// </summary>
        public DirectoryFileResolver()
        {
            if (Directory.Exists(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "dist"))) { WWWRootPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "dist"); }
            else if (Directory.Exists(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "WWW"))) { WWWRootPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "WWW"); }
            else if (Directory.Exists(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "wwwroot"))) { WWWRootPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "wwwroot"); }
            else
            {
                throw new DirectoryNotFoundException("Couldn't find the 'wwwroot' folder in the app's directory, make sure it exists!");
            }
        }

        /// <summary>
        /// Creates a DirectoryFileResolver that resolves files from a custom directory
        /// </summary>
        public DirectoryFileResolver(string wwwRootPath)
        {
            if (!Directory.Exists(wwwRootPath)) { throw new DirectoryNotFoundException(wwwRootPath + " Does not exist!"); }
            WWWRootPath = wwwRootPath;
        }

        public override string GetIndexFile()
        {
            if (File.Exists(Path.Join(WWWRootPath, "index.html"))) { return "/index.html"; }
            else if (File.Exists(Path.Join(WWWRootPath, "index.htm"))) { return "/index.htm"; }
            else if (File.Exists(Path.Join(WWWRootPath, "default.html"))) { return "/default.html"; }
            else if (File.Exists(Path.Join(WWWRootPath, "default.htm"))) { return "/default.htm"; }

            throw new FileNotFoundException("Couldn't find an 'index.html' file in your wwwroot folder!");
        }

        public override bool DoesFileExist(string fileRelativeToRoot)
        {
            return File.Exists(Path.Join(WWWRootPath, fileRelativeToRoot));
        }

        public override Stream OpenFileStream(string fileRelativeToRoot)
        {
            if (DoesFileExist(fileRelativeToRoot)) {
                return File.OpenRead(Path.Join(WWWRootPath, fileRelativeToRoot));
            }

            return new MemoryStream();
        }
    }
}
