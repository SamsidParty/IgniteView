using IgniteView.Core.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class CommandManager
    {
        public static ConcurrentDictionary<string, MethodInfo> Commands
        {
            get
            {
                if (_Commands == null)
                {
                    _Commands = new ConcurrentDictionary<string, MethodInfo>();

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
                                    var functionName = (attributes.FirstOrDefault() as CommandAttribute).FunctionName;

                                    if (functionName == null)
                                    {
                                        functionName = method.Name;
                                    }
                                    else if (!JSFunctionCall.IsFunctionNameValid(functionName))
                                    {
                                        throw new FormatException($"Function name {functionName} is not a valid JavaScript function name");
                                    }

                                    _Commands[functionName] = method;
                                }
                            }
                        }
                    }
                }

                return _Commands;
            }
        }

        internal static ConcurrentDictionary<string, MethodInfo> _Commands;
        internal static ConcurrentDictionary<string, object> _CommandTargets = new();

        public static async Task<object> ExecuteCommand(WebWindow? target, CommandData commandData)
        {
            try
            {
                if (!Commands.ContainsKey(commandData.Function))
                {
                    target?.ExecuteJavaScript(new JSFunctionCall("console.error", $"The command bridge couldn't locate a binding for command '{commandData.Function}', please ensure the command is correct."));
                    return null;
                }

                var method = Commands[commandData.Function]; // Find the MethodInfo of the command
                var cleanUpActions = new List<Action>(); // Some parameter types need to be cleaned up after

                // Run the method
                var paramList = new List<object>();

                var paramIndex = 0;
                foreach (var parameter in method.GetParameters())
                {
                    if (parameter.ParameterType == typeof(WebWindow))
                    {
                        paramList.Add(target); // Inject the target WebWindow as a parameter
                    }
                    else
                    {
                        if (commandData.Parameters.Length <= paramIndex)
                        {
                            target?.ExecuteJavaScript(new JSFunctionCall("console.error", $"The command bridge couldn't transform parameter {paramIndex + 1} '{parameter.Name}' for command '{commandData.Function}', make sure you have passed this parameter when calling the function"));
                            return null;
                        }

                        var providedParam = commandData.Parameters[paramIndex];
                        var providedParamType = providedParam.GetType();
                        var expectedParamType = parameter.ParameterType;

                        // If the parameter is a stream, wait for the JS code to upload the blob
                        if (expectedParamType == typeof(Stream) && providedParamType == typeof(string))
                        {
                            var jsBlob = (await JSBlob.WaitForBlobResolution((string)providedParam));
                            providedParam = jsBlob.Stream;

                            cleanUpActions.Add(jsBlob.Dispose);
                        }
                        else if (providedParamType != expectedParamType)
                        {
                            // Try to automatically convert types
                            var recoveredFromTypeError = false;

                            if (expectedParamType == typeof(string))
                            { // Easy conversion
                                recoveredFromTypeError = true;
                                providedParam = providedParam.ToString();
                            }
                            else if (expectedParamType == typeof(Int32) && providedParamType == typeof(Int64)) // Common error cause JS numbers are int64
                            {
                                recoveredFromTypeError = true;
                                providedParam = Convert.ToInt32(providedParam);
                            }
                            else if (expectedParamType == typeof(double) && providedParamType == typeof(Int64))
                            {
                                recoveredFromTypeError = true;
                                providedParam = Convert.ToDouble(providedParam);
                            }
                            else if (expectedParamType == typeof(Int64) && providedParamType == typeof(double))
                            {
                                recoveredFromTypeError = true;
                                providedParam = Convert.ToInt64(providedParam);
                            }
                            else if (expectedParamType == typeof(object))
                            {
                                // No need to do anything
                                recoveredFromTypeError = true;
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

                var methodTarget = _CommandTargets!.GetValueOrDefault(commandData.Function, null); // For non static methods
                var result = method.Invoke(methodTarget, paramList.ToArray());
                var resultType = result?.GetType() ?? typeof(void);

                // Handle async commands
                if (result is Task)
                {
                    await (Task)result;

                    if (resultType.GetProperty("Result") != null && !resultType.GetProperty("Result")!.PropertyType.Name.Contains("VoidTaskResult"))
                    {
                        result = ((dynamic)result).Result;

                        if (result == null)
                        {
                            result = JSLiteral.Undefined; // In JavaScript, an empty function return should be undefined
                        }
                    }
                    else
                    {
                        result = JSLiteral.Undefined;
                    }
                }

                var returnFunction = new JSFunctionCall("window.igniteView.commandQueue.resolve", commandData.CallbackID, result);
                target?.ExecuteJavaScript(returnFunction);

                // Clean up
                cleanUpActions.ForEach(action => action());

                return result;
            }
            catch (Exception ex)
            {
                target?.ExecuteJavaScript(new JSFunctionCall("console.error", $"The command bridge encountered an error while executing the command '{commandData.Function}': {ex.ToString()}"));
                return null;
            }
        }
    }
}
