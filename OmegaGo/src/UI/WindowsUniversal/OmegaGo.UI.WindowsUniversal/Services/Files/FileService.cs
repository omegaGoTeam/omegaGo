using System;
using Windows.Storage;
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
    }
}
