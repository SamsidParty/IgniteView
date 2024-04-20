using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework
{
    public class SharedIO
    {
        /// <summary>
        /// This Class Contains A Subset Of System.IO.File
        /// It Is Designed To Be A Compatibiliy Layer That Works Across Platforms
        /// </summary>
        public static SharedIOFile File;


        public static void FindAndLoad()
        {
            if (Platform.isUWP)
            {
                File = UWPHelperLoader.Current.GetPlatformIOFile();
                return;
            }

            File = new SharedIOFile();
        }
    }
}
