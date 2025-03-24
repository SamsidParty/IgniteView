using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Add this attribute to a method to expose it to JavaScript
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        public string FunctionName;

        /// <summary>
        /// Add this attribute to a method to expose it to JavaScript, it can be called via "window.igniteView.commandBridge.[functionName]()"
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        public CommandAttribute(string functionName)
        {
            FunctionName = functionName;
        }
    }
}
