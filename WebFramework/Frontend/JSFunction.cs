using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WebFramework.Backend;

namespace WebFramework
{
    public class JSFunctionAttribute : Attribute
    {
        /// <summary>
        /// The Function Name In The JS Context
        /// </summary>
        public string JSFunctionName;

        public JSFunctionAttribute(string jsFunctionName) { 
            JSFunctionName = JSFunction.SanitizeFunctionName(jsFunctionName);
        }
    }

    public class JSFunction
    {
        /// <summary>
        /// Keeps Track Of Function IDs & Return Values Of Functions That Have Not Been Returned Yet
        /// </summary>
        public static ConcurrentDictionary<string, string> PendingFunctions = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Makes Sure A Function Name Has No Special Characters
        /// </summary>
        public static string SanitizeFunctionName(string name)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9._-]");

            if (rgx.IsMatch(name))
            {
                Logger.LogWarning("An Invalid Function Name " + name + " Was Provided, It Will Be Sanitized To " + rgx.Replace(name, ""));
            }

            return rgx.Replace(name, "");
        }

        /// <summary>
        /// Generates A String To Call A Function In JavaScript
        /// </summary>
        public static string GenerateFunction(string fname, params object[] values)
        {
            var js = JSFunction.SanitizeFunctionName(fname) + "(";
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                js += value.AsJavaScript() + ",";
            }

            js += ");";

            return js;
        }
    }
}
