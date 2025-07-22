using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs
{
    public interface IDialogHandler
    {
        #region Base Implementations
        public abstract string PickFile(FileFilter[] fileFilters, string defaultPath);
        #endregion


    }
}
