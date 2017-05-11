using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<string> ReadFileAsync(string filePath, string folder = null)
        {
            StorageFolder storageFolder = folder == null ? _rootFolder : await _rootFolder.CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);
            var storageFile = await storageFolder.GetFileAsync(filePath);
            return await FileIO.ReadTextAsync(storageFile);
        }

        public async Task WriteFileAsync(string filePath, string fileContent, string folder = null)
        {
            StorageFolder storageFolder = folder == null ? _rootFolder : await _rootFolder.CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);
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
            return (await storageFolder.GetFilesAsync()).Select(f => f.Name);
        }

        public async Task<FileInfo> GetFileInfoAsync(string fileName, string subfolder = null)
        {
            StorageFolder storageFolder = subfolder == null ? _rootFolder : await _rootFolder.CreateFolderAsync(subfolder, CreationCollisionOption.OpenIfExists);
            var storageFile = await storageFolder.GetFileAsync(fileName);
            var basicInfo = await storageFile.GetBasicPropertiesAsync();
            return new FileInfo(fileName, basicInfo.Size, basicInfo.DateModified);
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

        public async Task<bool> FileExistsAsync(string filename, string subfolderName = null)
        {
            StorageFolder storageFolder = subfolderName == null ? _rootFolder : await _rootFolder.CreateFolderAsync(subfolderName, CreationCollisionOption.OpenIfExists);
            try
            {
                await storageFolder.GetFileAsync(filename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
