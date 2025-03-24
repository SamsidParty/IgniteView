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
                            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
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
            if (!Commands.ContainsKey(commandData.Function)) {
                target.ExecuteJavaScript(new JSFunctionCall("console.error", $"The command bridge couldn't locate a binding for command '{commandData.Function}', please ensure the command is correct."));
                return;
            }

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
                    if (commandData.Parameters.Length <= paramIndex)
                    {
                        target.ExecuteJavaScript(new JSFunctionCall("console.error", $"The command bridge couldn't transform parameter {paramIndex + 1} '{parameter.Name}' for command '{commandData.Function}', make sure you have passed this parameter when calling the function"));
                        return;
                    }

                    var providedParam = commandData.Parameters[paramIndex];
                    var providedParamType = providedParam.GetType();
                    var expectedParamType = parameter.ParameterType;

                    if (providedParamType != expectedParamType)
                    {
                        // Try to automatically convert types
                        var recoveredFromTypeError = false;

                        if (expectedParamType == typeof(string)) { // Easy conversion
                            recoveredFromTypeError = true;
                            providedParam = providedParam.ToString();
                        }
                        else if (expectedParamType == typeof(Int32) && providedParamType == typeof(Int64)) // Common error cause JS numbers are int64
                        {
                            recoveredFromTypeError = true;
                            providedParam = Convert.ToInt32(providedParam);
                        }

                        if (!recoveredFromTypeError)
                        {
                            // Cannot automatically convert types, throw an error
                            throw new ArgumentException("The provided parameter of type " + providedParamType.Name + " does not match expected type " + expectedParamType.Name);
                        }

                    }

                    paramList.Add(providedParam);
                    paramIndex++;
                }
            }

            var result = method.Invoke(null, paramList.ToArray());

            var returnFunction = new JSFunctionCall("window.igniteView.commandQueue.resolve", commandData.CallbackID, result);
            target.ExecuteJavaScript(returnFunction);
        }
    }
}
