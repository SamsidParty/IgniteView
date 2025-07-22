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
        public string PickFolder(string initialPath) => NFDBindings.PickFolderU8(initialPath);
        public string[] PickMultipleFiles(FileFilter[] fileFilters, string initialPath) => NFDBindings.OpenDialogMultipleU8(initialPath, fileFilters.ToKeyValuePairs());
        public string SaveFile(FileFilter[] fileFilters, string initialName, string initialPath) => NFDBindings.SaveDialogU8(initialPath, initialName, fileFilters.ToKeyValuePairs());
    }
}
