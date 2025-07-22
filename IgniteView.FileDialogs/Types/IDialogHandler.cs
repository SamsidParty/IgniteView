using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs
{
    public interface IDialogHandler
    {
        public abstract string PickFile(FileFilter[] fileFilters, string initialPath);
        public abstract string[] PickMultipleFiles(FileFilter[] fileFilters, string initialPath);
        public abstract string SaveFile(FileFilter[] fileFilters, string initialName, string initialPath);
        public abstract string PickFolder(string initialPath);
    }
}
