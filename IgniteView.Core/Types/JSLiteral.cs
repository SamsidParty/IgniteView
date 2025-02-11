using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Wrapper that represents raw JavaScript code
    /// </summary>
    public class JSLiteral
    {
        public string Value;

        public JSLiteral(string value)
        {
            Value = value;
        }

        public static implicit operator string(JSLiteral l) => l.Value;
        public static implicit operator JSLiteral(string l) => new JSLiteral(l);
    }
}
