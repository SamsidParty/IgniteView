using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Represents a JavaScript variable assignment
    /// </summary>
    public class JSAssignment
    {
        public required string VariableName;
        public required object Value;


        /// <summary>
        /// Creates a JavaScript variable assignment with a name and value
        /// </summary>
        [SetsRequiredMembers]
        public JSAssignment(string variableName, object value)
        {
            VariableName = variableName;
            Value = value;
        }

        /// <summary>
        /// Converts the assignment to a JavaScript string (eg. "window.x = 0")
        /// </summary>
        public override string ToString()
        {
            var js = VariableName + " = " + Value.AsJavaScript() + ";";

            if (!js.StartsWith("window."))
            {
                js = "window." + js;
            }

            return js;
        }

        public static implicit operator string(JSAssignment c) => c.ToString();
    }
}
