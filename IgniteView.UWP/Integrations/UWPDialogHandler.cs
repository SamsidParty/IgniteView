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

            if (fileFilters == null || fileFilters.Length == 0) picker.FileTypeFilter.Add("*");
            foreach (var filter in fileFilters)
            {
                foreach (var extension in filter.ExtensionsWithDot)
                {
                    picker.FileTypeFilter.Add(extension);
                }
            }

            var file = await picker.PickSingleFileAsync();

            return file?.Path;
        }

        public async Task<string> PickFolder(string initialPath)
        {
            var picker = new Windows.Storage.Pickers.FolderPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            var folder = await picker.PickSingleFolderAsync();

            return folder?.Path;
        }

        public async Task<string[]> PickMultipleFiles(FileFilter[] fileFilters, string initialPath)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            if (fileFilters == null || fileFilters.Length == 0) picker.FileTypeFilter.Add("*");
            foreach (var filter in fileFilters)
            {
                foreach (var extension in filter.ExtensionsWithDot)
                {
                    picker.FileTypeFilter.Add(extension);
                }
            }

            var files = await picker.PickMultipleFilesAsync();

            return files.Select((f) => f.Path).ToArray();
        }


        // Currently broken on UWP
        // https://github.com/microsoft/CsWinRT/issues/1998
        public async Task<string> SaveFile(FileFilter[] fileFilters, string initialName, string initialPath)
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            if (fileFilters == null || fileFilters.Length == 0) picker.FileTypeChoices.Add("All files", new List<string>() { "*.*" });
            foreach (var filter in fileFilters)
            {
                picker.FileTypeChoices.Add(filter.Name, filter.ExtensionsWithDot.ToList());
            }

            picker.SuggestedFileName = initialName;

            var file = await picker.PickSaveFileAsync();
            return file?.Path;
        }
    }
}
