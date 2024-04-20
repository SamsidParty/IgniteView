using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework.Test
{
    public class TestScript : WebScript
    {
        public override async Task DOMContentLoaded()
        {

        }

        public static void OpenFilePicker()
        {
            FilePicker.OpenFolderPicker(WindowManager.MainWindow.Document);
        }
    }
}
