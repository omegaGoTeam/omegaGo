﻿using OmegaGo.UI.Services.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace OmegaGo.UI.WindowsUniversal.Services.Files
{
    /// <summary>
    /// Implements file picking using UWP FileOpenPicker
    /// </summary>
    class FilePickerService : IFilePickerService
    {
        public async Task<FileContentInfo> PickAndReadFileAsync(params string[] extensions)
        {
            //prepare file picker
            FileOpenPicker fileOpen = new FileOpenPicker();
            foreach (var extension in extensions)
            {
                fileOpen.FileTypeFilter.Add(extension);
            }
            var file = await fileOpen.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    string contents = await FileIO.ReadTextAsync(file);
                    var basicProperties = await file.GetBasicPropertiesAsync();
                    return new FileContentInfo(file.Name, basicProperties.Size, basicProperties.DateModified, contents);
                }
                catch
                {
                    //ignore read errors and return null
                }
            }
            return null;
        }

        public async Task<bool> PickAndWriteFileAsync(string suggestedFileName, string contents)
        {
            //prepare file picker
            FileSavePicker fileSave = new FileSavePicker();
            fileSave.SuggestedFileName = suggestedFileName;
            fileSave.SuggestedStartLocation = PickerLocationId.Desktop;
            fileSave.FileTypeChoices.Add("SGF", new List<string>() {".sgf"});
            var file = await fileSave.PickSaveFileAsync();
            if (file != null)
            {
                try
                {
                    await FileIO.WriteTextAsync(file, contents);
                    return true;
                }
                catch
                {
                    //ignore write errors
                }
            }
            return false;
        }
    }
}
