using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using OmegaGo.UI.Services.Files;

namespace OmegaGo.UI.WindowsUniversal.Services.Files
{
    sealed class AppDataFileService : IAppDataFileService
    {
        private readonly string _rootFolderPath = ApplicationData.Current.LocalFolder.Path;
        private readonly StorageFolder _rootFolder = ApplicationData.Current.LocalFolder;

        public async Task<string> ReadFileAsync(string folder, string filePath)
        {
            var storageFolder = await _rootFolder.CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);
            var storageFile = await storageFolder.GetFileAsync(filePath);
            return await FileIO.ReadTextAsync(storageFile);
        }

        public async Task WriteFileAsync(string folder, string filePath, string fileContent)
        {
            var storageFolder = await _rootFolder.CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);
            var storageFile = await storageFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(storageFile, fileContent);
        }

        public async Task EnsureFolderExistsAsync(string folderPath)
        {
            try
            {
                await _rootFolder.GetFolderAsync(folderPath);
            }
            catch (System.IO.FileNotFoundException)
            {
                await _rootFolder.CreateFolderAsync(folderPath);
            }
        }

        public async Task<IEnumerable<string>> EnumerateFilesInFolderAsync(string folderPath)
        {
            var storageFolder = await _rootFolder.CreateFolderAsync(folderPath, CreationCollisionOption.OpenIfExists);
            List<string> filenames = new List<string>();
            foreach (var storageFile in await storageFolder.GetFilesAsync())
            {
                filenames.Add(storageFile.Name);
            }
            return filenames;
        }

        public async Task LaunchFolderAsync(string folderPath)
        {
            await Launcher.LaunchFolderAsync(await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderPath, CreationCollisionOption.OpenIfExists));
        }

        public async Task DeleteFileAsync(string folder, string filename)
        {
            var storageFolder = await _rootFolder.GetFolderAsync(folder);
            var storageFile = await storageFolder.GetFileAsync(filename);
            await storageFile.DeleteAsync();
        }

        public async Task<bool> FileExistsAsync(string subfolderName, string filename)
        {            
            var subfolder = await _rootFolder.CreateFolderAsync(subfolderName, CreationCollisionOption.OpenIfExists);
            try
            {
                await subfolder.GetFileAsync(filename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
