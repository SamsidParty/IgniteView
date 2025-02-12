using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class IconManager
    {
        private static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
            return Path.Combine(path, fileName);
        }

        /// <summary>
        /// Retrieves an icon from the current internal server and copies it to a temporary path on the filesystem
        /// </summary>
        /// <param name="iconPath">The path of the original icon relative to the www root (eg. /favicon.png) This file MUST be in PNG format</param>
        /// <returns>The temp path of the cloned icon</returns>
        public static string CloneIcon(string iconPath)
        {

            if (Path.GetExtension(iconPath) != ".png")
            {
                throw new FormatException("The provided iconPath must be a .png file");
            }

            var clonedIconPath = GetTempFilePathWithExtension("png");
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
