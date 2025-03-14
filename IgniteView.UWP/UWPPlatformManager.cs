using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IgniteView.Core;

namespace IgniteView.UWP
{
    public class UWPPlatformManager : PlatformManager
    {

        public static Type Activate()
        {
            return typeof(IgniteViewPage);
        }

        public override void Create()
        {
            throw new NotImplementedException();
        }

        public override WebWindow CreateWebWindow()
        {
            throw new NotImplementedException();
        }

        public override ScriptInjectionMode GetScriptInjectionMode()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
