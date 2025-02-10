using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Represents the dimensions and dimension restrictions of a window
    /// </summary>
    public class WindowBounds
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


        /// <summary>
        /// Default window bounds
        /// </summary>
        public WindowBounds() {
            MinHeight = 0;
            MinWidth = 0;
            MaxHeight = 999999;
            MaxWidth = 999999;
            InitialWidth = 1280;
            InitialHeight = 720;
        }
    }
}
