using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework
{
    public class SharedIO
    {
        public static SharedIOFile File;


        public static void FindAndLoad()
        {

            File = new SharedIOFile();
        }
    }
}
