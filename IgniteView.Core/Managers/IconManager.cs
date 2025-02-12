using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class IconManager
    {
        /// <summary>
        /// Retrieves an icon from the current internal server and copies it to a temporary path on the filesystem
        /// </summary>
        /// <param name="iconPath">The path of the original icon relative to the www root (eg. /favicon.ico)</param>
        /// <returns>The temp path of the cloned icon</returns>
        public static string CloneIcon(string iconPath)
        {
            var clonedIconPath = Path.GetTempFileName();
            var fileResolver = AppManager.Instance.CurrentServerManager.Resolver;

            if (!fileResolver.DoesFileExist(iconPath)) { 
                throw new FileNotFoundException("The provided icon path " + iconPath + " does not exist in the current file resolver. Make sure it's a path relative to the www root (with a leading /)");
            }

            // Clone the icon into the temp path
            var clonedFile = File.OpenWrite(clonedIconPath);
            var originalStream = fileResolver.OpenFileStream(iconPath);
            originalStream.CopyTo(clonedFile);
            originalStream.Close();
            clonedFile.Close();

            return clonedIconPath;
        }
    }
}
