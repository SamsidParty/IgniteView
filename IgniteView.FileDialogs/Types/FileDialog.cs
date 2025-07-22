using IgniteView.FileDialogs.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs
{
    public static class FileDialog
    {
        public static IDialogHandler Handler
        {
            get
            {
                if (_Handler == null)
                {
                    // TODO: Support for more platforms later
                    _Handler = new DesktopDialogHandler();
                }
                return _Handler;
            }
            set
            {
                _Handler = value;
            }
        }

        private static IDialogHandler? _Handler;

        #region Passthrough To Handler
        public static string PickFile(FileFilter[] fileFilters, string initialPath) => Handler.PickFile(fileFilters, initialPath);
        #endregion

        #region Quality Of Life Overloads
        public static string PickFile() => PickFile(new FileFilter[0]);
        public static string PickFile(FileFilter[] fileFilters) => PickFile(fileFilters, string.Empty);
        #endregion
    }
}
