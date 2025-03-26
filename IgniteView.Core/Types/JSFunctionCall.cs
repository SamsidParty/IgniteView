using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IgniteView.Core
{
    /// <summary>
    /// Represents a call to a JavaScript function
    /// </summary>
    public class JSFunctionCall
    {
        public required string FunctionName;
        public required object[] Parameters;

        /// <summary>
        /// Ensures the function name is valid
        /// </summary>
        public static bool IsFunctionNameValid(string functionName)
        {
            return functionName.Contains(".") || Regex.IsMatch(functionName, "^[A-Za-z_$][A-Za-z0-9_$]*$");
        }

        /// <summary>
        /// Creates a JavaScript function with a name and parameters
        /// </summary>
        [SetsRequiredMembers]
        public JSFunctionCall(string functionName, params object[] parameters)
        {
            FunctionName = functionName;
            Parameters = parameters;
        }

        /// <summary>
        /// Converts the function to a JavaScript string (eg. "console.log('hello, world');")
        /// </summary>
        public override string ToString()
        {
            if (!IsFunctionNameValid(FunctionName)) { throw new FormatException(FunctionName + " is not a valid name for a JavaScipt function"); }

            var js = FunctionName + "(";
            for (var i = 0; i < Parameters.Length; i++)
            {
                var value = Parameters[i];
                js += value.AsJavaScript() + ",";
            }

            if (js.EndsWith(","))
            {
                js = js.Substring(0, js.Length - 1); // Remove trailing comma
            }
            js += ");";
            return js;
        }

        public static implicit operator string(JSFunctionCall c) => c.ToString();
    }
}
