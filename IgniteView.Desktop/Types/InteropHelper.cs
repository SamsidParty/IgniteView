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
        /// Reads a string from a pointer and frees its memory
        /// </summary>
        public static string PointerToString(IntPtr ptr)
        {
            var data = Marshal.PtrToStringAnsi(ptr);
            Free(ptr);
            return data;
        }
    }
}
