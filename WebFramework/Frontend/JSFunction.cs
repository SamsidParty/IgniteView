﻿using System;
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
        /// Makes Sure A Function Name Has No Special Characters
        /// </summary>
        public static string SanitizeFunctionName(string name)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9_-]");

            if (rgx.IsMatch(name))
            {
                Logger.LogWarning("An Invalid Function Name " + name + " Was Provided, It Will Be Sanitized To " + rgx.Replace(name, ""));
            }

            return rgx.Replace(name, "");
        }
    }
}