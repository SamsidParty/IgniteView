using IgniteView.FileDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.UWP.Integrations
{
    public class UWPDialogHandler : IDialogHandler
    {
        public async Task<string> PickFile(FileFilter[] fileFilters, string initialPath)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            
            foreach (var filter in fileFilters)
            {
                foreach (var extension in filter.ExtensionsWithDot)
                {
                    picker.FileTypeFilter.Add(extension);
                }
            }

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                return file.Path;
            }

            return null;
        }

        public async Task<string> PickFolder(string initialPath)
        {
            throw new NotImplementedException();
        }

        public async Task<string[]> PickMultipleFiles(FileFilter[] fileFilters, string initialPath)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SaveFile(FileFilter[] fileFilters, string initialName, string initialPath)
        {
            throw new NotImplementedException();
        }
    }
}
