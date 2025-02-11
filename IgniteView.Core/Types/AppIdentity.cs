using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Contains information used to identify the app
    /// </summary>
    public struct AppIdentity
    {
        public required string Name;
        public required string Developer;

        /// <summary>
        /// Gets a unique ID string based on the other properties of the struct
        /// </summary>
        public string IDString
        {
            get
            {
                Func<string, string> prepare = (s) => s.ToLower().Replace("-", "_").Replace(" ", "_").Replace("@", "_").Replace(".", "_").Trim();
                return $"{prepare(Developer)}__{prepare(Name)}";
            }
        }

        [SetsRequiredMembers]
        public AppIdentity(string name, string developer)
        {
            Name = name;
            Developer = developer;
        }
    }
}
