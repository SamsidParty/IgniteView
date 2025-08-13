using IgniteView.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage;

namespace IgniteView.UWP
{
    public class UWPPersistentStorage : PersistentStorage
    {
        private string ApplyPath(string path) => Path.Join(ApplicationData.Current.LocalFolder.Path, path);

        public override async Task<Stream> GetStream(string file)
        {
            var stream = await StorageFile.GetFileFromPathAsync(ApplyPath(file));
            return await stream.OpenStreamForReadAsync();
        }

        public override async Task<Stream> GetWriteStream(string file)
        {
            var stream = await StorageFile.GetFileFromPathAsync(ApplyPath(file));
            return await stream.OpenStreamForWriteAsync();
        }

        public override async Task<string[]> EnumFiles(string folder)
        {
            var storageFolder = await StorageFolder.GetFolderFromPathAsync(ApplyPath(folder));
            var files = await storageFolder.GetFilesAsync();
            var fileList = new List<string>();

            foreach (var file in files)
            {
                fileList.Add(file.Path);
            }

            return fileList.ToArray();
        }

        public override async Task<byte[]> ReadAllBytes(string file)
        {
            try
            {
                //This Throws If The File Is Empty
                var buffer = await Windows.Storage.FileIO.ReadBufferAsync(await StorageFile.GetFileFromPathAsync(ApplyPath(file)));
                return buffer.ToArray();
            }
            catch
            {
                return new byte[0];
            }
        }

        public override async Task WriteAllBytes(string file, byte[] bytes)
        {
            file = ApplyPath(file);
            if (!await Exists(file))
            {
                await (await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(file))).CreateFileAsync(Path.GetFileName(file), Windows.Storage.CreationCollisionOption.OpenIfExists);
            }

            var buffer = CryptographicBuffer.CreateFromByteArray(bytes);
            await Windows.Storage.FileIO.WriteBufferAsync(await StorageFile.GetFileFromPathAsync(file), buffer);
        }

        public override async Task Delete(string file)
        {
            await (await StorageFile.GetFileFromPathAsync(ApplyPath(file))).DeleteAsync();
        }

        public override async Task<bool> Exists(string file)
        {
            FileInfo info = new FileInfo(ApplyPath(file));
            return info.Exists;
        }
    }
}
