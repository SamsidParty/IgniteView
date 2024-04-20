﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;
using Windows.Storage;
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
                    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
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
                    filePaths.Add(file.Path);
                }
            }



            return filePaths.ToArray();
        }

        public void OnLoad()
        {
            Logger.LogInfo("Loaded UWP Helper");
        }
    }
}
