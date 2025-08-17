using IgniteView.FileDialogs.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
                    _Handler = new DesktopDialogHandler(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)); // Disable unicode support on Windows because it causes issues
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
        public static async Task<string> PickFile(FileFilter[] fileFilters, string initialPath) => await Handler.PickFile(fileFilters, initialPath);
        public static async Task<string[]> PickMultipleFiles(FileFilter[] fileFilters, string initialPath) => await Handler.PickMultipleFiles(fileFilters, initialPath);
        public static async Task<string> SaveFile(FileFilter[] fileFilters, string initialName, string initialPath) => await Handler.SaveFile(fileFilters, initialName, initialPath);
        public static async Task<string> PickFolder(string initialPath) => await Handler.PickFolder(initialPath);
        #endregion

        #region Quality Of Life Overloads
        public static async Task<string> PickFile() => await PickFile(Array.Empty<FileFilter>());
        public static async Task<string> PickFile(FileFilter[] fileFilters) => await PickFile(fileFilters, string.Empty);
        public static async Task<string[]> PickMultipleFiles() => await PickMultipleFiles(Array.Empty<FileFilter>());
        public static async Task<string[]> PickMultipleFiles(FileFilter[] fileFilters) => await PickMultipleFiles(fileFilters, string.Empty);
        public static async Task<string> SaveFile() => await SaveFile(Array.Empty<FileFilter>());
        public static async Task<string> SaveFile(FileFilter[] fileFilters) => await SaveFile(fileFilters, string.Empty, string.Empty);
        public static async Task<string> SaveFile(FileFilter[] fileFilters, string initialName) => await SaveFile(fileFilters, initialName, string.Empty);
        public static async Task<string> SaveFile(string initialName) => await SaveFile(Array.Empty<FileFilter>(), initialName, string.Empty);
        public static async Task<string> PickFolder() => await PickFolder(string.Empty);
        #endregion
    }
}
