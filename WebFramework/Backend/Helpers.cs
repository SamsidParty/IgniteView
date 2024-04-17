using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Runtime.InteropServices;

namespace WebFramework
{
    public static class Helpers
    {

        public static JSLiteral ToJSLiteral(this string value)
        {
            return new JSLiteral() { Value = value };
        }

        public static void SetApartmentStateSafe(this Thread t, ApartmentState s){
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
                t.SetApartmentState(s);
            }
        }

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
                return JsonSerializer.Serialize(value as Array);
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


        // Extension method, call for any object, eg "if (x.IsNumeric())..."
        public static bool IsNumeric(this object x) { return (x == null ? false : IsNumeric(x.GetType())); }

        // Method where you know the type of the object
        public static bool IsNumeric(Type type) { return IsNumeric(type, Type.GetTypeCode(type)); }

        // Method where you know the type and the type code of the object
        public static bool IsNumeric(Type type, TypeCode typeCode) { return (typeCode == TypeCode.Decimal || (type.IsPrimitive && typeCode != TypeCode.Object && typeCode != TypeCode.Boolean && typeCode != TypeCode.Char)); }
    }

    public class JSLiteral
    {
        public string Value;
    }
}