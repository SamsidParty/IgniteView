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
        private const int MaxValue = 999999;

        public int MinWidth;
        public int MinHeight;
        public int MaxWidth;
        public int MaxHeight;
        public int InitialWidth;
        public int InitialHeight;

        /// <summary>
        /// Modifies the provided window bounds to conform to the standards of an actual window.
        /// For example, this function will replace 0 values with their respective default values
        /// </summary>
        public WindowBounds AppliedBounds()
        {
            var newBounds = new WindowBounds()
            {
                MinWidth = MinWidth,
                MinHeight = MinHeight,
                MaxWidth = MaxWidth < 1 ? MaxValue : MaxWidth,
                MaxHeight = MaxHeight < 1 ? MaxValue : MaxHeight,
                InitialWidth = InitialWidth < 1 ? 1280 : InitialWidth,
                InitialHeight = InitialHeight < 1 ? 720 : InitialHeight
            };

            return newBounds;
        }

        /// <summary>
        /// Creates new window bounds with the specified width and height (no minimum or maximum dimensions)
        /// </summary>
        public WindowBounds(int width, int height)
        {
            MinHeight = 0;
            MinWidth = 0;
            MaxHeight = 0;
            MaxWidth = 0;
            InitialWidth = width;
            InitialHeight = height;
        }


        /// <summary>
        /// Default window bounds
        /// </summary>
        public WindowBounds() {
            MinHeight = 0;
            MinWidth = 0;
            MaxHeight = 0;
            MaxWidth = 0;
            InitialWidth = 1280;
            InitialHeight = 720;
        }
    }
}
