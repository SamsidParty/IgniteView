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
        private void BeepBeep()
        {
            Console.Beep();
        }
    }
}
