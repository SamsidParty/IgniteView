using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs.Desktop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NFDFilterU8
    {
        public string Name;
        public string Spec;
    }
}
