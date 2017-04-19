using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using Newtonsoft.Json;
using OmegaGo.UI.Services.Files;

namespace OmegaGo.UI.Services.Tsumego
{
    internal class TsumegoProblemsLoader : ITsumegoProblemsLoader
    {
        private const string TsumegoAppDataFolder = "Tsumego";
        private const string TsumegoListFileName = "ProblemList.json";

        private readonly IAppPackageFileService _appPackageFileService;

        private List<TsumegoProblemInfo> _tsumegoProblems = null;

        private Task _loadTask = null;

        public TsumegoProblemsLoader(IAppPackageFileService appPackageFileService)
        {
            _appPackageFileService = appPackageFileService;
        }

        public async Task<IReadOnlyCollection<TsumegoProblemInfo>> GetProblemListAsync()
        {
            await EnsureProblemListInitializedAsync();
            return _tsumegoProblems;
        }

        public async Task<TsumegoProblem> GetProblemAsync(TsumegoProblemInfo problemInfo)
        {
            await EnsureProblemListInitializedAsync();
            var problemDefinition = await _appPackageFileService.ReadFileFromRelativePathAsync(Path.Combine(TsumegoAppDataFolder,problemInfo.FilePath));
            return TsumegoProblem.CreateFromSgfText(problemDefinition);
        }

        private async Task EnsureProblemListInitializedAsync()
        {
            if (_tsumegoProblems == null && _loadTask == null)
            {
                _loadTask = LoadProblemListAsync();
            }
            await _loadTask;
        }

        private async Task LoadProblemListAsync()
        {
            //Load tsumego problems
            var problemList = JsonConvert.DeserializeObject<List<TsumegoProblemDefinition>>(
                    await _appPackageFileService.ReadFileFromRelativePathAsync($"{TsumegoAppDataFolder}\\{TsumegoListFileName}"))
                .Select(p => p.ToTsumegoProblemInfo());
            _tsumegoProblems = new List<TsumegoProblemInfo>(problemList);
        }
    }
}
