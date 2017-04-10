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
using Newtonsoft.Json;
using OmegaGo.Core.Game;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.Services.Tsumego;

namespace OmegaGo.UI.ViewModels
{
    public class TsumegoMenuViewModel : ViewModelBase
    {
        private const string TsumegoAppDataFolder = "Tsumego";
        private const string TsumegoListFileName = "ProblemList.json";

        private readonly IAppPackageFileService _appPackageFileService;

        private ObservableCollection<TsumegoProblemInfo> _tsumegoProblems;

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
            var problemList = JsonConvert.DeserializeObject<List<TsumegoProblemDefinition>>(
                    await _appPackageFileService.ReadFileFromRelativePathAsync($"{TsumegoAppDataFolder}\\{TsumegoListFileName}"))
                .Select(p => p.ToTsumegoProblemInfo());
            TsumegoProblems = new ObservableCollection<TsumegoProblemInfo>(problemList);
            IsWorking = false;
        }

        public ObservableCollection<TsumegoProblemInfo> TsumegoProblems
        {
            get { return _tsumegoProblems; }
            set { SetProperty(ref _tsumegoProblems, value); }
        }

        public void MoveToSolveTsumegoProblem(TsumegoProblem problem)
        {
            Mvx.RegisterSingleton<TsumegoProblem>(problem);
            ShowViewModel<TsumegoViewModel>();
        }
    }
}
