using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework.MAUI
{
    public class MAUIHelper
    {
        public IFileSystem GetFileSystem()
        {
            return FileSystem.Current;
        }

        public void OnLoad()
        {
            Logger.LogInfo("Loaded MAUI Helper");
        }
    }
}
