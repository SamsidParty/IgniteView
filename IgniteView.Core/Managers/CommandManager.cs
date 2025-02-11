using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class CommandManager
    {
        public static void ExecuteCommand(WebWindow target, string commandString)
        {
            ExecuteCommand(target, CommandData.Parse(commandString));
        }

        public static void ExecuteCommand(WebWindow target, CommandData commandData)
        {
            var returnFunction = new JSFunction("window.igniteView.commandQueue.resolve", commandData.CallbackID);
            target.ExecuteJavaScript(returnFunction);
        }
    }
}
