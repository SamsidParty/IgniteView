using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Resolves files from a .igniteview (tar) file
    /// </summary>
    public class TarFileResolver : FileResolver
    {
        TarReader Reader;

        Dictionary<string, TarEntry> Files = new Dictionary<string, TarEntry>();
        Dictionary<string, string> FileContainerPaths = new Dictionary<string, string>();

        public TarFileResolver()
        {
            var tarFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "iv2runtime", "main.igniteview");
            var handle = File.OpenRead(tarFilePath);

            Reader = new TarReader(handle);

            while (Reader.GetNextEntry() is TarEntry entry)
            {
                var name = entry.Name;

                if (name.StartsWith("./")) { name = name.Substring(1); }
                if (!name.StartsWith("/")) { name = "/" + name; }

                Files[name] = entry;
                FileContainerPaths[name] = tarFilePath;
            }
        }

        public override bool DoesFileExist(string fileRelativeToRoot)
        {
            return Files.ContainsKey(fileRelativeToRoot);
        }

        public override string GetIndexFile()
        {
            return "/index.html";
        }

        public override Stream OpenFileStream(string fileRelativeToRoot)
        {
            if (DoesFileExist(fileRelativeToRoot))
            {
                return new TarStream(Files[fileRelativeToRoot], File.Open(FileContainerPaths[fileRelativeToRoot], FileMode.Open, FileAccess.Read, FileShare.Read));
            }

            return new MemoryStream();
        }
    }
}
