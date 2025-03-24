using IgniteView.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.UWP
{
    public class UWPAppManager : AppManager
    {
        protected override FileResolver CreateFileResolver()
        {
            return new UWPFileResolver();
        }

        public UWPAppManager(AppIdentity identity) : base(identity)
        {
            
        }
    }
}
