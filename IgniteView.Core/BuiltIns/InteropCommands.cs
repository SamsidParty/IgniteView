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

            for (int i = 0; i < commandList.Length; i++)
            {
                var command = CommandManager.Commands.ElementAt(i);

                if ((command.Value.ReturnType ?? typeof(void)).IsAssignableFrom(typeof(Stream)))
                {
                    commandList[i] = "streamedCommand/" + command.Key;
                    continue;
                }

                commandList[i] = command.Key;
            }

            
            return commandList;
        }
    }
}
