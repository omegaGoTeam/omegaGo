using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.Services.Tsumego;

namespace OmegaGo.UI.ViewModels
{
    public class TsumegoMenuViewModel : ViewModelBase
    {
        private const string TsumegoAppDataFolder = "Tsumego";
        private const string TsumegoListFileName = "TsumegoList.json";

        private readonly IAppPackageFileService _appPackageFileService;

        public TsumegoMenuViewModel(IAppPackageFileService appPackageFileService)
        {
            _appPackageFileService = appPackageFileService;
        }

        public async void Init()
        {
            await LoadProblemsAsync();
        }

        /// <summary>
        /// Loads the tsumego problems
        /// </summary>        
        private async Task LoadProblemsAsync()
        {
            IsWorking = true;

            //Load tsumego problems
            var tsumegoFiles = await _appPackageFileService.GetFilePathsAsync("Tsumego", true);
            var tsumegoFileLoadTasks = new List<Task<TsumegoProblem>>();
            foreach (var tsumegoFile in tsumegoFiles)
            {
                tsumegoFileLoadTasks.Add(LoadTsumegoProblemInfo(tsumegoFile));
            }
            var results = await Task.WhenAll(tsumegoFileLoadTasks);
            foreach (var result in results) TsumegoProblems.Add(result);            
            IsWorking = false;
        }

        /// <summary>
        /// Loads tsumego problem info from file
        /// </summary>
        /// <param name="tsumegoFile">Tsumego file</param>
        /// <returns>Problem info</returns>
        private async Task<TsumegoProblem> LoadTsumegoProblemInfo(string tsumegoFile)
        {
            var fileContent = await _appPackageFileService.ReadContentFileFromPathAsync(tsumegoFile).ConfigureAwait(false);
            return TsumegoProblem.CreateFromSgfText(fileContent);
        }

        public ObservableCollection<TsumegoProblem> TsumegoProblems { get; } = new ObservableCollection<TsumegoProblem>();

        public void MoveToSolveTsumegoProblem(TsumegoProblem problem)
        {
            Mvx.RegisterSingleton<TsumegoProblem>(problem);
            ShowViewModel<TsumegoViewModel>();
        }
    }
}
