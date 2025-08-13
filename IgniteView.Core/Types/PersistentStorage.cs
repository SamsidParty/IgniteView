using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core.Types
{
    /// <summary>
    /// Allows you to read and write files in the persistent storage of the application.
    /// </summary>
    public class PersistentStorage
    {
        #region Abstractions
        public async Task WriteAllText(string file, string text)
        {
            await WriteAllBytes(file, Encoding.UTF8.GetBytes(text));
        }

        public async Task<string> ReadAllText(string file)
        {
            try
            {
                return Encoding.UTF8.GetString(await ReadAllBytes(file));
            }
            catch
            {
                return null;
            }
        }
        #endregion

        private string BasePath => Path.Join(AppManager.Instance.CurrentIdentity.AppDataPath, "PersistentStorage");
        private string ApplyPath(string p)
        {
            Directory.CreateDirectory(BasePath);
            return Path.Join(BasePath, p);
        }

        public virtual async Task<string[]> EnumFiles(string folder)
        {
            return Directory.GetFiles(ApplyPath(folder));
        }

        public virtual async Task WriteAllBytes(string file, byte[] bytes)
        {
            System.IO.File.WriteAllBytes(ApplyPath(file), bytes);
        }

        public virtual async Task<byte[]> ReadAllBytes(string file)
        {
            return System.IO.File.ReadAllBytes(ApplyPath(file));
        }

        public virtual async Task<bool> Exists(string file)
        {
            return System.IO.File.Exists(ApplyPath(file));
        }

        public virtual async Task Delete(string file)
        {
            System.IO.File.Delete(ApplyPath(file));
        }

        public virtual async Task<Stream> GetStream(string file)
        {
            return File.OpenRead(ApplyPath(file));
        }

        public virtual async Task<Stream> GetWriteStream(string file)
        {
            return File.OpenWrite(ApplyPath(file));
        }
    }
}
