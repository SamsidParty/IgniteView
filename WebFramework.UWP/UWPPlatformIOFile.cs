﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WebFramework.UWP
{
    public class UWPPlatformIOFile : SharedIOFile
    {
        public override async Task<byte[]> ReadAllBytes(string file)
        {
            try
            {
                //This Throws If The File Is Empty
                var buffer = await Windows.Storage.FileIO.ReadBufferAsync(await StorageFile.GetFileFromPathAsync(file));
                return buffer.ToArray();
            }
            catch
            {
                return new byte[0];
            }
        }

        public override async Task WriteAllBytes(string file, byte[] bytes)
        {
            var buffer = CryptographicBuffer.CreateFromByteArray(bytes);
            await Windows.Storage.FileIO.WriteBufferAsync(await StorageFile.GetFileFromPathAsync(file), buffer);
        }

        public override async Task Delete(string file)
        {
            await (await StorageFile.GetFileFromPathAsync(file)).DeleteAsync();
        }

        public override async Task<bool> Exists(string file)
        {
            FileInfo info = new FileInfo(file);
            return info.Exists;
        }
    }
}