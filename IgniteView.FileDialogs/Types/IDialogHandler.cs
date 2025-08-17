using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs
{
    public interface IDialogHandler
    {
        public abstract Task<string> PickFile(FileFilter[] fileFilters, string initialPath);
        public abstract Task<string[]> PickMultipleFiles(FileFilter[] fileFilters, string initialPath);
        public abstract Task<string> SaveFile(FileFilter[] fileFilters, string initialName, string initialPath);
        public abstract Task<string> PickFolder(string initialPath);
    }
}
