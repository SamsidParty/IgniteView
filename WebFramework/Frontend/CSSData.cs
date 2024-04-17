using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public class CSSValue
    {
        public string _value;

        public static implicit operator string(CSSValue d) => d._value;
        public static implicit operator CSSValue(string d) => new CSSValue() { _value = d };

        public static implicit operator CSSValue(Color c)
        {
            var v = new CSSValue();
            v._value = "rgba(" + c.R + ", " + c.G + ", " + c.B + ", " + c.A + ")";
            return v;
        }
    }
}
