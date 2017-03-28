using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using OmegaGo.UI.Services.Files;

namespace OmegaGo.UI.WindowsUniversal.Services.Files
{
    sealed class FileService : IFileService
    {
        private readonly string _rootFolderPath = ApplicationData.Current.LocalFolder.Path;

        public FileService()
        {
            
        }
        

        public string ReadFile(string filePath)
        {
            string combinedPath = $"{_rootFolderPath}\\{filePath}";

            if (!System.IO.File.Exists(combinedPath))
            {
                throw new ArgumentException("File does not exist ");
            }

            string fileText = System.IO.File.ReadAllText(combinedPath);

            return fileText;
        }

        public void WriteFile(string filePath, string fileContent)
        {
            string combinedPath = $"{_rootFolderPath}\\{filePath}";

            try
            {
                System.IO.File.WriteAllText(combinedPath, fileContent);
            }
            catch(Exception)
            {
                // TODO Martin : Fill
            }
        }

        public string ReadSettingsFile()
        {
            // TODO Martin : Finish
            return String.Empty;
        }

        public void EnsureFolderExists(string folderPath)
        {
            string combinedPath = $"{_rootFolderPath}\\{folderPath}";
            if (!System.IO.Directory.Exists(combinedPath))
            {
                System.IO.Directory.CreateDirectory(combinedPath);
            }
        }

        public IEnumerable<string> EnumerateFilesInFolder(string folderPath)
        {
            string combinedPath = $"{_rootFolderPath}\\{folderPath}";
            foreach(var path in System.IO.Directory.EnumerateFiles(combinedPath))
            {
                yield return System.IO.Path.GetFileName(path);
            }
        }

        public async Task LaunchFolderAsync(string folderPath)
        {
            await Launcher.LaunchFolderAsync((await ApplicationData.Current.LocalFolder.GetFolderAsync(folderPath)));
        }

        public void DeleteFile(string filename)
        {
            string full = Combine(filename);
            System.IO.File.Delete(full);
        }
        /// <summary>
        /// Changes a local path into a path that is appended to the full absolute path of ApplicationData.LocalFolder.
        /// </summary>
        /// <param name="path">The path to append to LocalFolder.</param>
        /// <returns></returns>
        private string Combine(string path)
        {
            string combinedPath = $"{_rootFolderPath}\\{path}";
            return combinedPath;
        }
    }
}
