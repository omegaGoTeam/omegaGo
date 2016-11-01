using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.ViewModels
{
    public class GameCreationViewModel : ViewModelBase
    {
        private IMvxCommand _navigateToGame;

        public GameCreationViewModel()
        {

        }

        public IMvxCommand NavigateToGame => _navigateToGame ?? (_navigateToGame = new MvxCommand(() => ShowViewModel<GameViewModel>()));
    }
}
