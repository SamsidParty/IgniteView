using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class CommandData
    {
        /// <summary>
        /// The binded name of the function that this command calls
        /// </summary>
        public string Function;

        /// <summary>
        /// The ID of the callback in the command queue, used to send the result to JavaScript code
        /// </summary>
        public string CallbackID;

        /// <summary>
        /// The parameters passed to the command
        /// </summary>
        public object[] Parameters;


        public static implicit operator CommandData(string commandString) => new CommandData(commandString);


        /// <summary>
        /// Creates a CommandData object from a string in format "function:id;param"
        /// </summary>
        public CommandData(string commandString)
        {
            var function = commandString.Substring(0, commandString.IndexOf(":"));
            var commandId = commandString.Substring(commandString.IndexOf(":") + 1, commandString.IndexOf(";") - commandString.IndexOf(":") - 1);
            var paramString = commandString.Substring(commandString.IndexOf(";") + 1);

            Function = function;
            CallbackID = commandId;
            Parameters = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(paramString)).paramList.ToArray();
        }
    }
}
