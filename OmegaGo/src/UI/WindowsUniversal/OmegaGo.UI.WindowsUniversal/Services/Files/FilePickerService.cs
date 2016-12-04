using OmegaGo.UI.Services.Files;
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
        public async Task<string> PickAndReadFileAsync(params string[] extensions)
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
                    return await FileIO.ReadTextAsync(file);
                }
                catch
                {
                    //ignore read errors and return null
                }
            }
            return null;
        }
    }
}
