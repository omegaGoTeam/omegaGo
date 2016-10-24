using OmegaGo.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.WindowsUniversal.Services
{
    sealed class FileService : IFileService
    {
        private string _rootFolderPath;

        public FileService()
        {
            _rootFolderPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
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
            catch(Exception e)
            {
                // TODO Fill
            }
        }

        public string ReadSettingsFile()
        {
            // TODO Finish
            return String.Empty;
        }
    }
}
