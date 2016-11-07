using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.ViewModels
{
    public class SingleplayerViewModel : ViewModelBase
    {
        private ObservableCollection<string> _boardSizes;
        private ObservableCollection<string> _difficulties;
        private ObservableCollection<string> _rulesets;
        private int _whiteHandicap;

        private int _selectedBoardSizeItemIndex;
        private int _selectedDifficultiesItemIndex;
        private int _selectedRulesetItemIndex;
        private int _selectedStoneColorsItemIndex;

        private IMvxCommand _navigateToGame;

        public ObservableCollection<string> BoardSizes
        {
            get { return _boardSizes; }
        }

        public ObservableCollection<string> Difficulties
        {
            get { return _difficulties; }
        }

        public ObservableCollection<string> Rulesets
        {
            get { return _rulesets; }
        }

        public int WhiteHandicap
        {
            get { return _whiteHandicap; }
            set { SetProperty(ref _whiteHandicap, value); }
        }

        public int SelectedBoardSizeItemIndex
        {
            get { return _selectedBoardSizeItemIndex; }
            set { SetProperty(ref _selectedBoardSizeItemIndex, value); }
        }

        public int SelectedDifficultiesItemIndex
        {
            get { return _selectedDifficultiesItemIndex; }
            set { SetProperty(ref _selectedDifficultiesItemIndex, value); }
        }

        public int SelectedRulesetItemIndex
        {
            get { return _selectedRulesetItemIndex; }
            set { SetProperty(ref _selectedRulesetItemIndex, value); }
        }

        public SingleplayerViewModel()
        {
            _boardSizes = new ObservableCollection<string>() { "9x9", "14x14", "19x19", "25x25" };
            _difficulties = new ObservableCollection<string>() { "Easy", "Medium", "Hard" };
            _rulesets = new ObservableCollection<string>() { "Chinese", "Japonese", "Víťa Ultimate" };
            
            _selectedBoardSizeItemIndex = 0;
            _selectedDifficultiesItemIndex = 0;
            _selectedRulesetItemIndex = 0;
            
            _whiteHandicap = 0;
        }

        public IMvxCommand NavigateToGame => _navigateToGame ?? (_navigateToGame = new MvxCommand(() => ShowViewModel<GameViewModel>()));
    }
}
