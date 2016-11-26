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

namespace OmegaGo.UI.ViewModels
{
    public class GameCreationViewModel : ViewModelBase
    {
        private ObservableCollection<string> _boardSizes;
        private ObservableCollection<string> _difficulties;
        private ObservableCollection<string> _rulesets;
        private ObservableCollection<string> _stoneColors;
        private int _whiteHandicap;

        private int _selectedBoardSizeItemIndex;
        private int _selectedDifficultiesItemIndex;
        private int _selectedRulesetItemIndex;
        private int _selectedStoneColorItemIndex;

        private IMvxCommand _navigateToGameCommand;

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

        public ObservableCollection<string> StoneColors
        {
            get { return _stoneColors; }
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

        public int SelectedStoneColorItemIndex
        {
            get { return _selectedStoneColorItemIndex; }
            set { _selectedStoneColorItemIndex = value; }
        }

        public GameCreationViewModel()
        {
            _boardSizes = new ObservableCollection<string>() { "9x9", "13x13", "19x19", "25x25" };
            _difficulties = new ObservableCollection<string>() { Localizer.Easy, Localizer.Medium, Localizer.Hard };
            _rulesets = new ObservableCollection<string>() { Localizer.Chinese, Localizer.Japonese };
            _stoneColors = new ObservableCollection<string>() { "Human", "AI (Michi)", "AI (Oakfoam)", "AI (Joker23)" };

            _selectedBoardSizeItemIndex = 0;
            _selectedDifficultiesItemIndex = 0;
            _selectedRulesetItemIndex = 0;
            _selectedStoneColorItemIndex = 0;

            _whiteHandicap = 0;
        }

        public IMvxCommand NavigateToGameCommand => _navigateToGameCommand ?? (_navigateToGameCommand = new MvxCommand(() => NavigateToGame()));

        private void NavigateToGame()
        {
            Game game = new Game();

            game.Players.Add(new Player("Black Player", "??", game));
            game.Players.Add(new Player("White Player", "??", game));
            foreach (var player in game.Players)
            {
                player.Agent = new GameViewModelAgent();
            }

            switch (SelectedBoardSizeItemIndex)
            {
                case 0:
                    game.BoardSize = new GameBoardSize(9);
                    break;
                case 1:
                    game.BoardSize = new GameBoardSize(13);
                    break;
                case 2:
                    game.BoardSize = new GameBoardSize(19);
                    break;
                case 3:
                    game.BoardSize = new GameBoardSize(25);
                    break;
            }

            switch (SelectedRulesetItemIndex)
            {

                case 0:
                    game.Ruleset = new ChineseRuleset(game.White, game.Black, game.BoardSize);
                    break;
                case 1:
                    game.Ruleset = new JapaneseRuleset(game.White, game.Black, game.BoardSize);
                    break;
            }

            Mvx.RegisterSingleton<Game>(game);
            ShowViewModel<GameViewModel>();
        }
    }
}
