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
    public class TsumegoMenuViewModel : ViewModelBase
    {
        public ObservableCollection<TsumegoProblem> TsumegoProblems
            => new ObservableCollection<TsumegoProblem>(Problems.AllProblems);

        public TsumegoMenuViewModel()
        {

        }

        public void MoveToSolveTsumegoProblem(TsumegoProblem problem)
        {
            Mvx.RegisterSingleton<TsumegoProblem>(problem);
            ShowViewModel<TsumegoViewModel>();
        }
    }
}
