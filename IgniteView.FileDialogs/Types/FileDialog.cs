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
        public static string[] PickMultipleFiles(FileFilter[] fileFilters, string initialPath) => Handler.PickMultipleFiles(fileFilters, initialPath);
        public static string SaveFile(FileFilter[] fileFilters, string initialName, string initialPath) => Handler.SaveFile(fileFilters, initialName, initialPath);
        public static string PickFolder(string initialPath) => Handler.PickFolder(initialPath);
        #endregion

        #region Quality Of Life Overloads
        public static string PickFile() => PickFile(Array.Empty<FileFilter>());
        public static string PickFile(FileFilter[] fileFilters) => PickFile(fileFilters, string.Empty);
        public static string[] PickMultipleFiles() => PickMultipleFiles(Array.Empty<FileFilter>());
        public static string[] PickMultipleFiles(FileFilter[] fileFilters) => PickMultipleFiles(fileFilters, string.Empty);
        public static string SaveFile() => SaveFile(Array.Empty<FileFilter>());
        public static string SaveFile(FileFilter[] fileFilters) => SaveFile(fileFilters, string.Empty, string.Empty);
        public static string SaveFile(FileFilter[] fileFilters, string initialName) => SaveFile(fileFilters, initialName, string.Empty);
        public static string SaveFile(string initialName) => SaveFile(Array.Empty<FileFilter>(), initialName, string.Empty);
        public static string PickFolder() => PickFolder(string.Empty);
        #endregion
    }
}
