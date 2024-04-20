using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;

namespace WebFramework.UWP
{
    public class UWPHelper
    {
        public bool IsDark()
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Dark;
        }

        public async Task<string[]> OpenFilePicker(FilePickerOptions options)
        {

            var filePaths = new List<string>();

            if (options is FolderPickerOptions)
            {
                var picker = new Windows.Storage.Pickers.FolderPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

                StorageFolder folder;

                folder = await picker.PickSingleFolderAsync();

                if (folder != null)
                {
                    StorageApplicationPermissions.FutureAccessList.Add(folder);
                    filePaths.Add(folder.Path);
                }

            }
            else
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                foreach (var fileType in options.AllowedFileTypes)
                {
                    picker.FileTypeFilter.Add(fileType);
                }

                StorageFile[] files;

                if (options.AllowMultiSelection)
                {
                    files = (await picker.PickMultipleFilesAsync()).ToArray();
                }
                else
                {
                    files = new StorageFile[] { await picker.PickSingleFileAsync() };
                }


                foreach (var file in files)
                {
                    StorageApplicationPermissions.FutureAccessList.Add(file);
                    filePaths.Add(file.Path);
                }
            }



            return filePaths.ToArray();
        }

        public async Task<string> OpenFileSaver(string extension)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            
            savePicker.FileTypeChoices.Add(extension + " File", new List<string>() { "." + extension });
            savePicker.SuggestedFileName = "file";

            var picked = await savePicker.PickSaveFileAsync();

            if (picked != null)
            {
                StorageApplicationPermissions.FutureAccessList.Add(picked);
                return picked.Path;
            }
            return "";
        }

        public SharedIOFile GetPlatformIOFile()
        {
            return new UWPPlatformIOFile();
        }

        public void OnLoad()
        {
            Logger.LogInfo("Loaded UWP Helper");
        }
    }
}
