using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct WindowBounds
    {
        public int MinWidth;
        public int MinHeight;
        public int MaxWidth;
        public int MaxHeight;
        public int InitialWidth;
        public int InitialHeight;

        /// <summary>
        /// Creates new window bounds with the specified width and height (no minimum or maximum dimensions)
        /// </summary>
        public WindowBounds(int width, int height)
        {
            MinHeight = 0;
            MinWidth = 0;
            MaxHeight = 999999;
            MaxWidth = 999999;
            InitialWidth = width;
            InitialHeight = height;
        }
    }
}
