using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace OmegaGo.UI.WindowsUniversal.Helpers.Files
{
    /// <summary>
    /// Helpers for UWP Storage manipulation
    /// </summary>
    public static class StorageHelpers
    {
        /// <summary>
        /// Get all files in a given folder
        /// </summary>
        /// <param name="folder">Folder</param>
        /// <param name="includeSubfolders">Should files from subfolders be included?</param>
        /// <returns>All files</returns>
        public static async Task<IEnumerable<StorageFile>> GetFilesInFolder(StorageFolder folder, bool includeSubfolders)
        {
            List<StorageFile> results = new List<StorageFile>();
            var files = await folder.GetFilesAsync();
            results.AddRange(files);
            if (includeSubfolders)
            {
                var folders = await folder.GetFoldersAsync();
                foreach (var subfolder in folders)
                {
                    var subfolderFiles = await GetFilesInFolder(subfolder, true);
                    results.AddRange(subfolderFiles);
                }
            }
            return results;
        }
    }
}
