using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Resolves files from a .igniteview (tar) file
    /// </summary>
    public class TarFileResolver : FileResolver
    {
        Dictionary<string, TarEntry> Files = new Dictionary<string, TarEntry>();
        Dictionary<string, string> FileContainerPaths = new Dictionary<string, string>();

        public TarFileResolver()
        {
            var runtimeDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "iv2runtime");
            var tarFiles = Directory.GetFiles(runtimeDirectory).Where((f) => f.EndsWith(".igniteview")); // Can't use "*.igniteview" filter because it doesn't match files with multiple dots (eg IgniteView.Core.igniteview)

            // Add every .igniteview file in the iv2runtime folder
            foreach (var tarFile in tarFiles) {
                var handle = File.OpenRead(tarFile);
                var reader = new TarReader(handle);

                while (reader.GetNextEntry() is TarEntry entry)
                {
                    var name = entry.Name;

                    if (name.StartsWith("./")) { name = name.Substring(1); }
                    if (!name.StartsWith("/")) { name = "/" + name; }

                    Files[name] = entry;
                    FileContainerPaths[name] = tarFile;
                }
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
