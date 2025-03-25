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
        public Dictionary<string, TarEntry> Files = new Dictionary<string, TarEntry>();

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
                var stream = Files[fileRelativeToRoot].DataStream; // Do not dispose this stream otherwise it will crash the whole TarReader

                lock (Files)
                {
                    var tempStream = new MemoryStream((int)stream.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(tempStream);
                    tempStream.Seek(0, SeekOrigin.Begin);
                    return tempStream;
                }

            }

            return new MemoryStream();
        }
    }
}
