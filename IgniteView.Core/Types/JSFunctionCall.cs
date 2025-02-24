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
        public static string SanitizeFunctionName(string name)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9._-]");

            if (rgx.IsMatch(name))
            {
                Console.WriteLine("An invalid function name " + name + " was provided, it will be sanitized to " + rgx.Replace(name, ""));
            }

            return rgx.Replace(name, "");
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
            var js = SanitizeFunctionName(FunctionName) + "(";
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
