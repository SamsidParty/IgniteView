using IgniteView.Core;
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

        [Command("resize")]
        public static void Resize(WebWindow target, int width, int height)
        {
            target.Bounds = new WindowBounds(width, height);
        }

        [Command("streamedCommandTest")]
        public static Stream StreamedCommandTest()
        {
            return null;
        }
    }
}
