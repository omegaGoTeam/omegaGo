using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.WindowsUniversal.Helpers.Files;

namespace OmegaGo.UI.WindowsUniversal.Services.Files
{
    internal class AppPackageFileService : IAppPackageFileService
    {
        /// <summary>
        /// Reads a content file from the app package install location
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>File contents</returns>
        public async Task<string> ReadFileFromRelativePathAsync(string filePath)
        {
            var file = await StorageFile.GetFileFromPathAsync(Path.Combine(Package.Current.InstalledLocation.Path, filePath));
            return await FileIO.ReadTextAsync(file);
        }

        /// <summary>
        /// Returns the relative file paths to all files in a given folder
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <param name="includeSubfolders">Should subfolder files be included?</param>
        /// <returns>List of full file paths</returns>
        public async Task<IEnumerable<string>> GetFilePathsAsync(string folderPath, bool includeSubfolders )
        {
            var absoluteFolderPath = Path.Combine(Package.Current.InstalledLocation.Path, folderPath);
            var targetFolder = await StorageFolder.GetFolderFromPathAsync(absoluteFolderPath);
            var files = await StorageHelpers.GetFilesInFolder(targetFolder, includeSubfolders);
            return files.Select(f => f.Path);
        }    
    }
}
