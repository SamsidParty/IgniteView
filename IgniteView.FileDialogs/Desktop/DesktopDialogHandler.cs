using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs.Desktop
{
    public class DesktopDialogHandler : IDialogHandler
    {
        public bool UnicodeEncoding = true;

        public DesktopDialogHandler() {  }
        public DesktopDialogHandler(bool unicodeEncoding) { UnicodeEncoding = unicodeEncoding; }

        public string PickFile(FileFilter[] fileFilters, string initialPath)
            => UnicodeEncoding
            ? NFDBindings.OpenDialogU8(initialPath, fileFilters.ToKeyValuePairs())
            : NFDBindings.OpenDialogN(initialPath, fileFilters.ToKeyValuePairs());

        public string PickFolder(string initialPath)
            => UnicodeEncoding
            ? NFDBindings.PickFolderU8(initialPath)
            : NFDBindings.PickFolderN(initialPath);

        public string[] PickMultipleFiles(FileFilter[] fileFilters, string initialPath)
            => UnicodeEncoding 
            ? NFDBindings.OpenDialogMultipleU8(initialPath, fileFilters.ToKeyValuePairs())
            : NFDBindings.OpenDialogMultipleN(initialPath, fileFilters.ToKeyValuePairs());

        public string SaveFile(FileFilter[] fileFilters, string initialName, string initialPath)
            => UnicodeEncoding
            ? NFDBindings.SaveDialogU8(initialPath, initialName, fileFilters.ToKeyValuePairs())
            : NFDBindings.SaveDialogN(initialPath, initialName, fileFilters.ToKeyValuePairs());
    }
}
