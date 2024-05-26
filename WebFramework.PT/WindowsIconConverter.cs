using PhotinoNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework.PT
{
    internal class WindowsIconConverter
    {

        #region PNG To Ico

        //Based On https://stackoverflow.com/a/11448060/18071273
        public static string ConvertPngToIco(byte[] pngBytes)
        {
            var path = Path.GetTempFileName();

            FileStream icoStream = new FileStream(path, FileMode.OpenOrCreate);
            // ICO header
            icoStream.WriteByte(0); icoStream.WriteByte(0);
            icoStream.WriteByte(1); icoStream.WriteByte(0);
            icoStream.WriteByte(1); icoStream.WriteByte(0);

            // Image size
            icoStream.WriteByte(0);
            icoStream.WriteByte(0);
            // Palette
            icoStream.WriteByte(0);
            // Reserved
            icoStream.WriteByte(0);
            // Number of color planes
            icoStream.WriteByte(0); icoStream.WriteByte(0);
            // Bits per pixel
            icoStream.WriteByte(32); icoStream.WriteByte(0);

            // Data size, will be written after the data
            icoStream.WriteByte(0);
            icoStream.WriteByte(0);
            icoStream.WriteByte(0);
            icoStream.WriteByte(0);

            // Offset to image data, fixed at 22
            icoStream.WriteByte(22);
            icoStream.WriteByte(0);
            icoStream.WriteByte(0);
            icoStream.WriteByte(0);

            // Writing actual data
            icoStream.Write(pngBytes);

            // Getting data length (file length minus header)
            long Len = icoStream.Length - 22;

            // Write it in the correct place
            icoStream.Seek(14, SeekOrigin.Begin);
            icoStream.WriteByte((byte)Len);
            icoStream.WriteByte((byte)(Len >> 8));

            icoStream.Close();

            return path;
        }

        #endregion
    }
}
