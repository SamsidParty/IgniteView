using IgniteView.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;

namespace IgniteView.UWP
{
    public class UWPFileResolver : FileResolver
    {
        StorageFolder BaseFolder = Package.Current.InstalledLocation;
        TarReader Reader;
        public Dictionary<string, TarEntry> Files = new Dictionary<string, TarEntry>();

        public UWPFileResolver()
        {
            var appFileRequest = BaseFolder.GetFileAsync("iv2runtime\\main.igniteview");
            appFileRequest.Wait();
            var appFile = appFileRequest.GetResults();

            var readRequest = appFile.OpenStreamForReadAsync();
            readRequest.Wait();

            Reader = new TarReader(readRequest.Result);

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
