using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            Logger.LogInfo("App Loaded");
        }

        [JSFunction("OpenFilePicker")]
        public static async Task OpenFilePicker()
        {
            var file = await FilePicker.OpenFilePicker(WindowManager.MainWindow.Document, new FilePickerOptions());
            if (file.Length > 0)
            {
                Logger.LogInfo(await SharedIO.File.ReadAllText(file[0]));
            }
        }

        [JSFunction("OpenFileSaver")]
        public async Task OpenFileSaver()
        {
            var file = await FilePicker.OpenFileSaver(WindowManager.MainWindow.Document, "exe");
            if (file != "")
            {
                await SharedIO.File.WriteAllText(file, "Hello, World! Test");
            }
        }

    }
}
