using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class WebWindow
    {
        public static WebWindow Create() => PlatformManager.Instance.CreateWebWindow();

        /// <summary>
        /// Only inherited classes should call this constructor
        /// </summary>
        protected WebWindow() { }
    }
}
