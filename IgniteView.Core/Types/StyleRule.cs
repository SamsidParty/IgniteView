using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Wrapper that represents a CSS style rule
    /// </summary>
    public class StyleRule
    {
        public string Selector;
        public string Property;
        public string Value;

        /// <summary>
        /// Creates a style rule with a custom selector
        /// </summary>
        public StyleRule(string selector, string property, string value)
        {
            Selector = selector;
            Property = property;
            Value = value;
        }

        /// <summary>
        /// Creates a style rule with a wildcard selector
        /// </summary>
        public StyleRule(string property, string value)
        {
            Selector = "*";
            Property = property;
            Value = value;
        }

        public override string ToString() => $"{Selector} {{{Property}: {Value};}}";
        public static implicit operator string(StyleRule r) => r.ToString();
    }
}
