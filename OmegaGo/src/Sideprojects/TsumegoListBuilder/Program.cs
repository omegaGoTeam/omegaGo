using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmegaGo.Core.Game;
using OmegaGo.UI.Services.Tsumego;

namespace TsumegoListBuilder
{
    class Program
    {
        static readonly List<TsumegoProblemDefinition> Definitions = new List<TsumegoProblemDefinition>();

        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();
            while (Path.GetFileName(path) != "src")
            {
                path = GoToParent(path);
            }
            string tsumegoFolder = Path.Combine(path, "UI\\Shared\\OmegaGo.UI.ContentFiles\\Tsumego");

            GetProblemsInFolder(tsumegoFolder, "");
            File.WriteAllText(Path.Combine(tsumegoFolder, "ProblemList.json"),
                JsonConvert.SerializeObject(Definitions));
        }

        private static void GetProblemsInFolder(string tsumegoFolder, string relativePath)
        {
            var files = Directory.GetFiles(tsumegoFolder);
            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension.IndexOf("sgf", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    ProcessTsumegoFile(fileInfo, relativePath);
                }
            }
            var subFolders = Directory.GetDirectories(tsumegoFolder);
            foreach (var subFolder in subFolders)
            {
                var dirInfo = new DirectoryInfo(subFolder);
                GetProblemsInFolder(subFolder, Path.Combine(relativePath, dirInfo.Name));
            }
        }

        private static void ProcessTsumegoFile(FileInfo fileInfo, string relativePath)
        {
            string content = File.ReadAllText(fileInfo.FullName);
            var problem = TsumegoProblem.CreateFromSgfText(content);
            var plainBoard = new StoneColor[problem.InitialBoard.Size.Width, problem.InitialBoard.Size.Height];
            for (int x = 0; x < problem.InitialBoard.Size.Width; x++)
            {
                for (int y = 0; y < problem.InitialBoard.Size.Height; y++)
                {
                    plainBoard[x, y] = problem.InitialBoard[x, y];
                }
            }
            TsumegoProblemDefinition definition = new TsumegoProblemDefinition(problem.Name, problem.InitialBoard.Size.Width, problem.InitialBoard.Size.Height,
                plainBoard, Path.Combine(relativePath, fileInfo.Name));
            Definitions.Add(definition);
        }


        private static string GoToParent(string path)
        {
            return Path.GetFullPath(Path.Combine(path, ".."));
        }
    }
}
