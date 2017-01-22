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
using System.Windows.Input;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Local;

namespace OmegaGo.UI.ViewModels
{
    public class GameCreationViewModel : ViewModelBase
    {
        private int _whiteHandicap;
        private float _compensation = 0;
        private GameBoardSize _selectedGameBoardSize = new GameBoardSize(19);
        private string _selectedDifficulty = null;
        private RulesetType _selectedRuleset = RulesetType.Chinese;
        private string _selectedStoneColor = null;

        private ICommand _setDefaultCompensationCommand = null;
        private IMvxCommand _navigateToGameCommand;

        /// <summary>
        /// Default offered game board sizes
        /// </summary>
        public ObservableCollection<GameBoardSize> BoardSizes { get; } =
            new ObservableCollection<GameBoardSize>() { new GameBoardSize(9), new GameBoardSize(13), new GameBoardSize(19), new GameBoardSize(25) };

        /// <summary>
        /// Selected game board size
        /// </summary>
        public GameBoardSize SelectedGameBoardSize
        {
            get
            {
                return _selectedGameBoardSize;
            }
            set
            {
                SetProperty(ref _selectedGameBoardSize, value);
                SetDefaultCompensation();
            }
        }

        /// <summary>
        /// Difficulties
        /// </summary>
        public ObservableCollection<string> Difficulties { get; }

        /// <summary>
        /// Selected difficulty
        /// </summary>
        public string SelectedDifficulty
        {
            get
            {
                return _selectedDifficulty;
            }
            set
            {
                SetProperty(ref _selectedDifficulty, value);
            }
        }

        /// <summary>
        /// Rulesets
        /// </summary>
        public ObservableCollection<RulesetType> Rulesets { get; } =
            new ObservableCollection<RulesetType>() { RulesetType.Chinese, RulesetType.Japanese, RulesetType.AGA };

        /// <summary>
        /// Selected ruleset
        /// </summary>
        public RulesetType SelectedRuleset
        {
            get
            {
                return _selectedRuleset;
            }
            set
            {
                SetProperty(ref _selectedRuleset, value);
                SetDefaultCompensation();
            }
        }

        /// <summary>
        /// Stone colors
        /// </summary>
        public ObservableCollection<string> StoneColors { get; }

        /// <summary>
        /// Selected stone color
        /// </summary>
        public string SelectedStoneColor
        {
            get
            {
                return _selectedStoneColor;
            }
            set
            {
                SetProperty(ref _selectedStoneColor, value);
            }
        }

        /// <summary>
        /// Handicap of white player
        /// </summary>
        public int WhiteHandicap
        {
            get { return _whiteHandicap; }
            set
            {
                SetProperty(ref _whiteHandicap, value);
                SetDefaultCompensation();
            }
        }

        /// <summary>
        /// Compensation
        /// </summary>
        public float Compensation
        {
            get { return _compensation; }
            set { SetProperty(ref _compensation, value); }
        }

        public GameCreationViewModel()
        {
            Difficulties = new ObservableCollection<string>() { Localizer.Easy, Localizer.Medium, Localizer.Hard };
            SelectedDifficulty = Difficulties.First();
            StoneColors = new ObservableCollection<string>() { "Human", "AI (Michi)", "AI (Oakfoam)", "AI (Joker23)" };
            SelectedStoneColor = StoneColors.First();
            WhiteHandicap = 0;
        }

        public ICommand SetDefaultCompensationCommand => _setDefaultCompensationCommand ?? (_setDefaultCompensationCommand = new MvxCommand(SetDefaultCompensation));

        private void SetDefaultCompensation()
        {
            Compensation = Ruleset.GetDefaultCompensation(SelectedRuleset, SelectedGameBoardSize, WhiteHandicap, CountingType.Area);
        }

        public IMvxCommand NavigateToGameCommand => _navigateToGameCommand ?? (_navigateToGameCommand = new MvxCommand(NavigateToGame));

        private void NavigateToGame()
        {
            CreateAndRegisterGame();
            ShowViewModel<GameViewModel>();
        }

        /// <summary>
        /// Creates and registers the specified game
        /// </summary>
        private void CreateAndRegisterGame()
        {
            //TODO: set some options
            HumanPlayer blackPlayer = new HumanPlayerBuilder(StoneColor.Black).Build();

            //TODO: set some options
            HumanPlayer whitePlayer = new HumanPlayerBuilder(StoneColor.White).Build();

            //TODO: set counting type
            LocalGame game = GameBuilder.CreateLocalGame().
                BoardSize(SelectedGameBoardSize).
                Ruleset(SelectedRuleset).
                Komi(Compensation).
                WhitePlayer(whitePlayer).
                BlackPlayer(blackPlayer).
                Build();

            Mvx.RegisterSingleton<ILiveGame>(game);
        }
    }
}
