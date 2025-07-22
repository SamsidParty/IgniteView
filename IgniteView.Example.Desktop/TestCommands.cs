using IgniteView.Core;
using IgniteView.FileDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Example.Desktop
{
    class TestCommands
    {
        [Command("beep")]
        public static void Beep(int times)
        {
            for (int i = 0; i < times; i++)
            {
                Console.Beep();
            }
        }

        [Command("getUsername")]
        public static string GetUsername()
        {
            return Environment.UserName;
        }

        [Command("getUsernameAsync")]
        public static async Task<string> GetUsernameAsync()
        {
            await Task.Delay(1000);
            return Environment.UserName;
        }

        [Command("jsonTest")]
        public static string JsonTest()
        {
            return "{}";
        }

        [Command("fileDialogTest")]
        public static string FileDialogTest()
        {
            return FileDialog.PickFile(new FileFilter("Images", "png,jpg,webp"));
        }

        [Command("resize")]
        public static async Task<string> Resize(WebWindow target, int width, int height)
        {
            await Task.Delay(1000);

            await AppManager.Instance.InvokeOnMainThread(async () =>
            {
                target.Bounds = new WindowBounds(width, height);
            });

            return $"Resized to {target.Bounds.InitialWidth}x{target.Bounds.InitialHeight}";
        }

        [Command("streamedCommandTest")]
        public static async Task<Stream> StreamedCommandTest(string filePath)
        {
            var client = new HttpClient();
            return File.OpenRead(filePath);
        }
    }
}
