using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Rules;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Time;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.GameCreationBundle;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;
using System;

namespace OmegaGo.UI.ViewModels
{
    public class GameCreationViewModel : ViewModelBase
    {
        private static readonly List<GameCreationViewPlayer> PlayerList = new List<GameCreationViewPlayer>(
            new GameCreationViewPlayer[] { new GameCreationViewHumanPlayer("Human") }
                .Concat(Core.AI.AISystems.AIPrograms.Select(
                    program => new GameCreationViewAiPlayer(program))));

        private readonly IGameSettings _gameSettings;

        private int _handicap = 0;
        private bool _isHandicapFixed = true;
        private float _compensation = 0;
        private GameBoardSize _selectedGameBoardSize = new GameBoardSize(19);
        private RulesetType _selectedRuleset = RulesetType.Chinese;        

        private int _customWidth = 19;
        private int _customHeight = 19;
        private string _server = "Local Game";

        // Game Mode specific View Model
        private readonly Type _gameModeViewModel;

        private GameCreationViewPlayer _blackPlayer = GameCreationViewModel.PlayerList[0];
        private GameCreationViewPlayer _whitePlayer = GameCreationViewModel.PlayerList[0];

        private TimeControlSettingsViewModel _timeControl = new TimeControlSettingsViewModel();

        private IMvxCommand _setDefaultCompensationCommand;
        private IMvxCommand _navigateToGameCommand;

        public GameCreationViewModel( IGameSettings gameSettings )
        {
            _gameSettings = gameSettings;
            _customWidth = _gameSettings.Interface.BoardWidth;
            _customHeight = _gameSettings.Interface.BoardHeight;
            SetCustomBoardSize();

            var bundle = Mvx.GetSingleton<GameCreationBundle>();
            bundle?.OnLoad(this);

            // Get View Model type for the provided bundle.
            _gameModeViewModel = GetGameModeViewModel(bundle);
        }

        /// <summary>
        /// Gets the black player settings
        /// </summary>
        public PlayerSettingsViewModel BlackPlayerSettings { get; } =
            new PlayerSettingsViewModel(PlayerList.First(), false);

        /// <summary>
        /// Gets the white player settings
        /// </summary>
        public PlayerSettingsViewModel WhitePlayerSettings { get; } =
            new PlayerSettingsViewModel(PlayerList.First(), false);

        public bool IgsLimitation { get; set; }

        public bool RulesetCanBeSelected => !IgsLimitation;

        public TimeControlSettingsViewModel TimeControl
        {
            get { return _timeControl; }
            set { SetProperty(ref _timeControl, value); }
        }

        public ObservableCollection<TimeControlStyle> TimeControlStyles { get; } =
            new ObservableCollection<TimeControlStyle>
            {
                TimeControlStyle.None,
                TimeControlStyle.Absolute,
                TimeControlStyle.Canadian
            };

        /// <summary>
        /// Gets the default offered game board sizes
        /// </summary>
        public ObservableCollection<GameBoardSize> BoardSizes { get; } =
            new ObservableCollection<GameBoardSize>()
            {
                new GameBoardSize(9),
                new GameBoardSize(13),
                new GameBoardSize(19)
            };

        /// <summary>
        /// Selected game board size
        /// </summary>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public GameBoardSize SelectedGameBoardSize
        {
            get
            {
                return _selectedGameBoardSize;
            }
            set
            {
                SetProperty(ref _selectedGameBoardSize, value);
                RaisePropertyChanged(() => SampleGameBoard);
                _customHeight = value.Height;
                _customWidth = value.Width;
                RaisePropertyChanged(nameof(CustomHeight));
                RaisePropertyChanged(nameof(CustomWidth));
                _gameSettings.Interface.BoardWidth = _customWidth;
                _gameSettings.Interface.BoardHeight = _customHeight;
                SetDefaultCompensation();
            }
        }

        public string Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); }
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

        public ObservableCollection<GameCreationViewPlayer> PossiblePlayers { get; } =
            new ObservableCollection<GameCreationViewPlayer>(GameCreationViewModel.PlayerList);

        public GameCreationViewPlayer BlackPlayer
        {
            get { return _blackPlayer; }
            set
            {
                SetProperty(ref _blackPlayer, value);
                BlackPlayerSettings.ChangePlayer(value);
            }
        }
        public GameCreationViewPlayer WhitePlayer
        {
            get { return _whitePlayer; }
            set
            {
                SetProperty(ref _whitePlayer, value);
                WhitePlayerSettings.ChangePlayer(value);
            }
        }

        public string CustomWidth
        {
            get { return _customWidth.ToString(); }
            set
            {
                int parsed = 0;
                if (int.TryParse(value, out parsed))
                {
                    SetProperty(ref _customWidth, parsed);
                    SetCustomBoardSize();
                }
            } // TODO Martin: do verification in game creation view
        }

        public string CustomHeight
        {
            get { return _customHeight.ToString(); }
            set
            {
                SetProperty(ref _customHeight, int.Parse(value));
                SetCustomBoardSize();
            } // TODO Martin: do verification in game creation view
        }
        /// <summary>
        /// Handicap of white player
        /// </summary>
        public int Handicap
        {
            get { return _handicap; }
            set
            {
                SetProperty(ref _handicap, value);
                SetDefaultCompensation();
            }
        }

        /// <summary>
        /// Gets or sets the type of handicap
        /// </summary>
        public bool IsHandicapFixed
        {
            get { return _isHandicapFixed; }
            set { SetProperty(ref _isHandicapFixed, value); }
        }

        /// <summary>
        /// Compensation
        /// </summary>
        public float Compensation
        {
            get { return _compensation; }
            set { SetProperty(ref _compensation, value); }
        }

        /// <summary>
        /// Sample game board for preview
        /// </summary>
        public GameBoard SampleGameBoard => new GameBoard(SelectedGameBoardSize);

        public IMvxCommand SetDefaultCompensationCommand => _setDefaultCompensationCommand ?? (_setDefaultCompensationCommand = new MvxCommand(SetDefaultCompensation));
        public IMvxCommand NavigateToGameCommand => _navigateToGameCommand ?? (_navigateToGameCommand = new MvxCommand(NavigateToGame));





        private void SetCustomBoardSize()
        {
            var thisSize = new GameBoardSize(_customWidth, _customHeight);

            if (!BoardSizes.Contains(thisSize))
            {
                BoardSizes.Add(thisSize);
            }

            SelectedGameBoardSize = thisSize;
        }

        private void SetDefaultCompensation()
        {
            Compensation = Ruleset.GetDefaultCompensation(SelectedRuleset, SelectedGameBoardSize, Handicap, CountingType.Area);
        }

        private void NavigateToGame()
        {
            if (!Validate())
            {
                return;
            }

            CreateAndRegisterGame();
            // Navigate to specific View Model
            // ShowViewModel<GameViewModel>();
            ShowViewModel(_gameModeViewModel);
        }

        /// <summary>
        /// Creates and registers the specified game
        /// </summary>
        private void CreateAndRegisterGame()
        {
            GamePlayer blackPlayer = BlackPlayer.Build(StoneColor.Black, TimeControl, BlackPlayerSettings);
            GamePlayer whitePlayer = WhitePlayer.Build(StoneColor.White, TimeControl, WhitePlayerSettings);
            BlackPlayerSettings.SaveAsInterfaceMementos();
            WhitePlayerSettings.SaveAsInterfaceMementos();

            LocalGame game = GameBuilder.CreateLocalGame().
                BoardSize(SelectedGameBoardSize).
                Ruleset(SelectedRuleset).
                Komi(Compensation).
                Handicap(Handicap).
                HandicapPlacementType(
                    IsHandicapFixed ?
                        HandicapPlacementType.Fixed :
                        HandicapPlacementType.Free).
                WhitePlayer(whitePlayer).
                BlackPlayer(blackPlayer).
                Build();
            Mvx.RegisterSingleton<IGame>(game);
        }

        private bool Validate()
        {
            if (IgsLimitation)
            {
                if (!SelectedGameBoardSize.IsSquare) return false;
                if (SelectedGameBoardSize.Width < 5 || SelectedGameBoardSize.Width > 19) return false;
            }
            return true;
        }

        /// <summary>
        /// Parses the provided GameCreationBundle and returns the appropriate View Model.
        /// </summary>
        /// <param name="gameCreationBundle"></param>
        /// <returns>the appropriate View Model</returns>
        private Type GetGameModeViewModel(GameCreationBundle gameCreationBundle)
        {
            // TODO Do we have any game mode enum? We could probably embbed it into GameCreationBundle so that we wouldnt need to do this.

            //if (gameCreationBundle is GameCreationViewAiPlayer)
            //    return typeof(LocalGameViewModel);
            //if (gameCreationBundle is GameCreationViewHumanPlayer)
            return typeof(LocalGameViewModel);
            if (gameCreationBundle is HotseatBundle)
                return typeof(LocalGameViewModel);
            if (gameCreationBundle is IgsChallengeBundle)
                return typeof(OnlineGameViewModel);
            //if (gameCreationBundle is SoloBundle)
            //    return typeof(LocalGameViewModel);

            throw new ArgumentException($"Specific Game View Model not yet implemented for Bundle: {gameCreationBundle.GetType().Name}");
        }
    }
}
