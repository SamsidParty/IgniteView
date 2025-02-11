using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Represents bounds for a window that is locked to certain dimensions
    /// </summary>
    public class LockedWindowBounds : WindowBounds
    {
        public LockedWindowBounds(int width, int height) { 
            InitialHeight = height;
            InitialWidth = width;
            MinHeight = height;
            MinWidth = width;
            MaxHeight = height;
            MaxWidth = width;
        }
    }
}
