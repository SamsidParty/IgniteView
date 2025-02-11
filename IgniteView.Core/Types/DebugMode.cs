using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class DebugMode
    {
        public static bool IsDebugMode
        {
            get
            {
                return Debugger.IsAttached;
            }
        }
    }
}
