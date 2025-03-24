using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Desktop
{
    public class InteropHelper
    {
        public const string DLLName = "IgniteView.Desktop.Native";

        [DllImport(InteropHelper.DLLName)]
        static extern void Free(IntPtr ptr);

        /// <summary>
        /// Reads an ansi string from a pointer and frees its memory
        /// </summary>
        public static string PointerToStringAnsi(IntPtr ptr)
        {
            var data = Marshal.PtrToStringAnsi(ptr);
            Free(ptr);
            return data;
        }

        /// <summary>
        /// Reads a base64 string from a pointer (doesn't free the memory)
        /// </summary>
        public static string PointerToStringBase64(IntPtr ptr)
        {
            var data = Marshal.PtrToStringAnsi(ptr);
            return Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }

        /// <summary>
        /// Reads a unicode string from a pointer and frees its memory
        /// </summary>
        public static string PointerToStringUni(IntPtr ptr)
        {
            var data = Marshal.PtrToStringUni(ptr);
            Free(ptr);
            return data;
        }
    }
}
