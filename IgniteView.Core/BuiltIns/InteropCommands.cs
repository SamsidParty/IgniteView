using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class InteropCommands
    {
        [Command("igniteview_list_commands")]
        public static string[] ListCommands()
        {
            var commandList = new string[CommandManager.Commands.Count];
            CommandManager.Commands.Keys.CopyTo(commandList, 0);
            return commandList;
        }
    }
}
