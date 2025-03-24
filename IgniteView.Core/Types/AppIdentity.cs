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
    public class AppIdentity
    {
        public required string Name;
        public required string Developer;

        /// <summary>
        /// Gets a unique ID string based on the name and developer
        /// </summary>
        public virtual string IDString
        {
            get
            {
                Func<string, string> prepare = (s) => s.ToLower().Replace("-", "_").Replace(" ", "_").Replace("@", "_").Replace(".", "_").Trim();
                return $"{prepare(Developer)}__{prepare(Name)}";
            }
        }

        /// <summary>
        /// Gets the app's data path based on the name and developer, and ensures the path exists
        /// </summary>
        public virtual string AppDataPath
        {
            get
            {
                if (PlatformManager.Instance.GetType().Name == "UWPPlatformManager")
                {
                    return ".\\";
                }

                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var fullPath = Path.Join(basePath, Developer, Name);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                return fullPath;
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
