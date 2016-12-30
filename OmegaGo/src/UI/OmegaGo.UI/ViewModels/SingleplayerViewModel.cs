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
using OmegaGo.UI.Services.Tsumego;

namespace OmegaGo.UI.ViewModels
{
    public class SingleplayerViewModel : ViewModelBase
    {
        public IMvxCommand GoToTutorial => new MvxCommand(() => ShowViewModel<TutorialViewModel>());
        public MvxCommand GoToStatistics => new MvxCommand(() => ShowViewModel<StatisticsViewModel>());

        public MvxCommand GoToLocalGame => new MvxCommand(() => ShowViewModel<GameCreationViewModel>(
            new Dictionary<string, string> {  ["AgainstAI"] = "true" }
            ));


        public ObservableCollection<TsumegoProblem> TsumegoProblems
            => new ObservableCollection<TsumegoProblem>(Problems.AllProblems);

        public SingleplayerViewModel()
        {

        }

        public void MoveToSolveTsumegoProblem(TsumegoProblem problem)
        {
            Mvx.RegisterSingleton<TsumegoProblem>(problem);
            ShowViewModel<TsumegoViewModel>();
        }
    }
}
