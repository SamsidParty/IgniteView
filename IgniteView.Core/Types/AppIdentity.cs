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

        [SetsRequiredMembers]
        public AppIdentity(string name, string developer)
        {
            Name = name;
            Developer = developer;
        }
    }
}
