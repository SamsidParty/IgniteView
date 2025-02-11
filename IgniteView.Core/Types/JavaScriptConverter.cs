using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IgniteView.Core
{
    public static class JavaScriptConverter
    {
        /// <summary>
        /// Converts an object to raw JavaScript code
        /// </summary>
        public static string AsJavaScript(this object value)
        {
            if (value is string)
            {
                return '"' + HttpUtility.JavaScriptStringEncode(value as string) + '"';
            }
            else if (value.IsNumeric())
            {
                return value.ToString();
            }
            else if (value is Array)
            {
                return JsonConvert.SerializeObject(value as Array);
            }
            else if (value is JSLiteral)
            {
                return (value as JSLiteral).Value;
            }
            else if (value == null)
            {
                return "null";
            }
            return "undefined";
        }


        static bool IsNumeric(this object x) => (x == null ? false : IsNumeric(x.GetType()));
        static bool IsNumeric(Type type) => IsNumeric(type, Type.GetTypeCode(type));
        static bool IsNumeric(Type type, TypeCode typeCode) => (typeCode == TypeCode.Decimal || (type.IsPrimitive && typeCode != TypeCode.Object && typeCode != TypeCode.Boolean && typeCode != TypeCode.Char));
    }
}
