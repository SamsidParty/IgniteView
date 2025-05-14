using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public static List<StyleRule> FromJSON(string json, bool darkMode = false) 
        {
            var styles = new List<StyleRule>();
            var styleObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            foreach (var property in styleObject.Keys)
            {
                if (darkMode) {
                    styles.Add(new DarkModeStyleRule("--system-" + property, styleObject[property]));
                }
                else {
                    styles.Add(new StyleRule("--system-" + property, styleObject[property]));
                }
                
            }

            return styles;
        }
    }
}
