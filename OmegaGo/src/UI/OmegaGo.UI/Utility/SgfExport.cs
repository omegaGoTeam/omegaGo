using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Utility
{
    public static class SgfExport
    {
        public static async Task ExportAsync(string suggestedFileName, string sgfContent)
        {
            await Mvx.Resolve<IFilePickerService>().PickAndWriteFileAsync(suggestedFileName, sgfContent);
        }

        /// <summary>
        /// Saves the file to library and returns the resulting file name
        /// </summary>
        /// <param name="fileName">File name suggested by the app</param>
        /// <param name="sgfContent">SGF content to store</param>
        /// <returns>Final file name (available)</returns>
        public static async Task<string> SaveToLibraryAsync(string fileName, string sgfContent)
        {
            //make sure the file name is valid
            fileName = RemoveInvalidFileNameCharacters(fileName);

            //find available file name
            var appDataFileService = Mvx.Resolve<IAppDataFileService>();
            if (await appDataFileService.FileExistsAsync(fileName, LibraryViewModel.SgfFolderName))
            {
                int copyNumber = 1;
                while (await appDataFileService.FileExistsAsync(
                    $"{Path.GetFileNameWithoutExtension(fileName)} ({copyNumber}).sgf", LibraryViewModel.SgfFolderName))
                {
                    copyNumber++;
                }
                fileName = $"{Path.GetFileNameWithoutExtension(fileName)} ({copyNumber}).sgf";
            }
            //store the file
            await appDataFileService.WriteFileAsync(fileName, sgfContent, LibraryViewModel.SgfFolderName);
            return fileName;
        }

        private static string RemoveInvalidFileNameCharacters(string fileName)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                fileName = fileName.Replace(c.ToString(), "");
            }

            return fileName;
        }
    }
}
