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
        private readonly ITsumegoProblemsLoader _tsumegoProlemsLoader;
        private ObservableCollection<TsumegoProblemInfo> _tsumegoProblems;

        public TsumegoMenuViewModel(ITsumegoProblemsLoader tsumegoProlemsLoader)
        {
            _tsumegoProlemsLoader = tsumegoProlemsLoader;
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
            TsumegoProblems = new ObservableCollection<TsumegoProblemInfo>(await _tsumegoProlemsLoader.GetProblemListAsync());
            IsWorking = false;
        }

        public ObservableCollection<TsumegoProblemInfo> TsumegoProblems
        {
            get { return _tsumegoProblems; }
            set { SetProperty(ref _tsumegoProblems, value); }
        }

        public async void MoveToSolveTsumegoProblem(TsumegoProblemInfo problemInfo)
        {
            var problem = await _tsumegoProlemsLoader.GetProblemAsync(problemInfo);
            Mvx.RegisterSingleton<TsumegoProblem>(problem);
            ShowViewModel<TsumegoViewModel>();
        }
    }
}
