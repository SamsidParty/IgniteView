using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs.Desktop
{
    public class DesktopDialogHandler : IDialogHandler
    {
        public string PickFile(FileFilter[] fileFilters, string initialPath) => NFDBindings.OpenDialogU8(initialPath, fileFilters.ToKeyValuePairs());
    }
}
