using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class CommandManager
    {
        public static Dictionary<string, MethodInfo> Commands
        {
            get
            {
                if (_Commands == null) {
                    _Commands = new Dictionary<string, MethodInfo>();

                    // Find all commands through reflection
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (Type type in assembly!.GetTypes())
                        {
                            foreach (var method in type.GetMethods())
                            {
                                var attributes = method.GetCustomAttributes(true).ToList();
                                attributes = attributes.Where((atr) => atr is CommandAttribute).ToList();
                                if (attributes.Any())
                                {
                                    _Commands[(attributes.FirstOrDefault() as CommandAttribute).FunctionName] = method;
                                }
                            }
                        }
                    }
                }

                return _Commands;
            }
        }

        static Dictionary<string, MethodInfo> _Commands;

        public static void ExecuteCommand(WebWindow target, CommandData commandData)
        {
            if (!Commands.ContainsKey(commandData.Function)) { return; }

            var method = Commands[commandData.Function]; // Find the MethodInfo of the command

            // Run the method
            var paramList = new List<object>();

            var paramIndex = 0;
            foreach (var parameter in method.GetParameters()) {
                if (parameter.ParameterType == typeof(WebWindow)) { 
                    paramList.Add(target); // Inject the target WebWindow as a parameter
                }
                else
                {
                    paramList.Add(commandData.Parameters[paramIndex]);
                    paramIndex++;
                }
            }

            var result = method.Invoke(null, paramList.ToArray());

            var returnFunction = new JSFunction("window.igniteView.commandQueue.resolve", commandData.CallbackID, result);
            target.ExecuteJavaScript(returnFunction);
        }
    }
}
