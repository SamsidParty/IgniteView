using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public class SharedIOFile
    {
        public async Task WriteAllText(string file, string text)
        {
            await WriteAllBytes(file, Encoding.UTF8.GetBytes(text));
        }

        public async Task<string> ReadAllText(string file)
        {
            return Encoding.UTF8.GetString(await ReadAllBytes(file));
        }

        //Virtual Methods

        public virtual async Task WriteAllBytes(string file, byte[] bytes)
        {
            System.IO.File.WriteAllBytes(file, bytes);
        }

        public virtual async Task<byte[]> ReadAllBytes(string file)
        {
            return System.IO.File.ReadAllBytes(file);
        }

        public virtual async Task<bool> Exists(string file)
        {
            return System.IO.File.Exists(file);
        }

        public virtual async Task Delete(string file)
        {
            System.IO.File.Delete(file);
        }
    }
}
