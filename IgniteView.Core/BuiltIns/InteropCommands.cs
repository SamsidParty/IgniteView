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
                var returnType = command.Value.ReturnType ?? typeof(void);

                // Convert Task<Example> to Example 
                if (returnType?.FullName.Contains("System.Threading.Tasks.Task") ?? false && returnType.ContainsGenericParameters)
                {
                    returnType = returnType.GenericTypeArguments.FirstOrDefault(typeof(void));
                }

                if (returnType?.IsAssignableFrom(typeof(Stream)) ?? false)
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
