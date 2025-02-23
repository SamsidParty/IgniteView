﻿using IgniteView.Core;
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
    }
}
