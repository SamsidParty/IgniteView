using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// A StyleRule that only applies in dark mode
    /// </summary>
    public class DarkModeStyleRule : StyleRule
    {
        public DarkModeStyleRule(string selector, string property, string value) : base(selector, property, value) { }
        public DarkModeStyleRule(string property, string value) : base(property, value) { }

        public override string ToString() => $"@media (prefers-color-scheme: dark) {{ {base.ToString()} }}";
        public static implicit operator string(DarkModeStyleRule r) => r.ToString();
    }
}
