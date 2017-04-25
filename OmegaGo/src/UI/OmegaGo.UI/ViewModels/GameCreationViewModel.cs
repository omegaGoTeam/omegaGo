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
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.Infrastructure.Tabbed;

namespace OmegaGo.UI.ViewModels
{
    public class GameCreationViewModel : ViewModelBase
    {
        private static readonly List<GameCreationViewPlayer> PlayerList = new List<GameCreationViewPlayer>(
            new GameCreationViewPlayer[] { new GameCreationViewHumanPlayer() }
                .Concat(Core.AI.AISystems.AIPrograms.Select(
                    program => new GameCreationViewAiPlayer(program))));
        private readonly IGameSettings _gameSettings;

        // Backing fields
        private bool _isHandicapFixed = true;
        private int _handicap;
        private float _compensation;
        private GameBoardSize _selectedGameBoardSize = new GameBoardSize(19);
        private RulesetType _selectedRuleset = RulesetType.Chinese;
        private string _formTitle = "";
        private string _refuseCaption = "";
        private int _customWidth = 19;
        private int _customHeight = 19;
        private string _server = "Local Game";
        private GameCreationViewPlayer _blackPlayer = GameCreationViewModel.PlayerList[0];
        private GameCreationViewPlayer _whitePlayer = GameCreationViewModel.PlayerList[0];
        private PlayerSettingsViewModel _blackSettings = new PlayerSettingsViewModel(PlayerList.First(), false);
        private PlayerSettingsViewModel _whiteSettings = new PlayerSettingsViewModel(PlayerList.First(), false);
        private TimeControlSettingsViewModel _timeControl = new TimeControlSettingsViewModel();
        private IMvxCommand _navigateToGameCommand;
        private IMvxCommand _switchColorsCommand;
        private IMvxCommand _createChallengeCommand;
        private IMvxCommand _acceptChallengeCommand;
        private IMvxCommand _refuseChallengeCommand;
        private bool _useRecommendedKomi = true;
        private string _validationErrorMessage = "";
        private string _compensationString;
        private int _selectedColorIndex = 0;
        private ObservableCollection<TimeControlStyle> _timeControlStyles =
            new ObservableCollection<TimeControlStyle>
            {
                TimeControlStyle.None,
                TimeControlStyle.Absolute,
                TimeControlStyle.Canadian,
                TimeControlStyle.Japanese
            };

    // Non-backing fields
    private GameCreationBundle _bundle;


        public GameCreationViewModel(IGameSettings gameSettings)
        {
            _gameSettings = gameSettings;
            _customWidth = _gameSettings.Interface.BoardWidth;
            _customHeight = _gameSettings.Interface.BoardHeight;
            SetCustomBoardSize();

            _bundle = Mvx.GetSingleton<GameCreationBundle>();
            _bundle.OnLoad(this);

            var thisTab = Mvx.Resolve<ITabProvider>().GetTabForViewModel(this);
            if (thisTab != null)
            {
                thisTab.Title = _bundle.TabTitle;
            }
        }

        public PlayerSettingsViewModel BlackPlayerSettings
        {
            get { return _blackSettings; }
            set { SetProperty(ref _blackSettings, value); }
        }
        
        public PlayerSettingsViewModel WhitePlayerSettings
        {
            get { return _whiteSettings; }
            set { SetProperty(ref _whiteSettings, value); }
        }

        public bool IgsLimitation { get; set; }

        public GameCreationBundle Bundle => _bundle;
        public string FormTitle
        {
            get { return _formTitle; }
            set { SetProperty(ref _formTitle, value); }
        }

        public string RefusalCaption
        {
            get { return _refuseCaption; }
            set { SetProperty(ref _refuseCaption, value); }
        }

        public TimeControlSettingsViewModel TimeControl
        {
            get { return _timeControl; }
            set { SetProperty(ref _timeControl, value); }
        }

        public ObservableCollection<TimeControlStyle> TimeControlStyles => _timeControlStyles;

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
                _customHeight = value.Height;
                _customWidth = value.Width;
                RaisePropertyChanged(nameof(CustomHeight));
                RaisePropertyChanged(nameof(CustomWidth));
                _gameSettings.Interface.BoardWidth = _customWidth;
                _gameSettings.Interface.BoardHeight = _customHeight;
                SetDefaultCompensation();
            }
        }

        public string ValidationErrorMessage
        {
            get { return _validationErrorMessage; }
            set { SetProperty(ref _validationErrorMessage, value); }
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

        public IMvxCommand SwitchColorsCommand => _switchColorsCommand ?? (_switchColorsCommand = new MvxCommand(() =>
        {
            var white = this.WhitePlayer;
            var whiteSettings = this.WhitePlayerSettings;
            var black = this.BlackPlayer;
            var blackSettings = this.BlackPlayerSettings;
            // Order probably still matters due to WhitePlayer and BlackPlayer setters.
            this.WhitePlayerSettings = blackSettings;
            this.WhitePlayer = black;
            this.BlackPlayerSettings = whiteSettings;
            this.BlackPlayer = white;
            this.WhitePlayerSettings.ChangePlayer(this.WhitePlayer);
            this.BlackPlayerSettings.ChangePlayer(this.BlackPlayer);
        }));

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

        public TimeControlStyle TimeControlStyle
        {
            get { return TimeControl.Style; }
            set
            {
                TimeControl.Style = value;
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
        public bool UseRecommendedKomi
        {
            get { return _useRecommendedKomi; }
            set { SetProperty(ref _useRecommendedKomi, value);
                SetDefaultCompensation();
            }
        }
        public string CustomSquareSize
        {
            get { return _customWidth.ToString(); }
            set {
                SetProperty(ref _customWidth, int.Parse(value));
                SetProperty(ref _customHeight, int.Parse(value));
                SetCustomBoardSize();
            }
        }

        public string CompensationString
        {
            get { return _compensationString; }
            set { SetProperty(ref _compensationString, value); }
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

        public int SelectedColorIndex
        {
            get { return _selectedColorIndex; }
            set { SetProperty(ref _selectedColorIndex, value); }
        }
        public StoneColor SelectedColor
        {
            get
            {
                switch (SelectedColorIndex)
                {
                    case 0:
                        return StoneColor.Black;
                    case 1:
                        return StoneColor.White;
                    default:
                        return StoneColor.None;
                }
            }
            set
            {
                switch (value)
                {
                    case StoneColor.Black:
                        SelectedColorIndex = 0;
                        break;
                    case StoneColor.White:
                        SelectedColorIndex = 1;
                        break;
                    case StoneColor.None:
                        SelectedColorIndex = 2;
                        break;
                }
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

        public IMvxCommand NavigateToGameCommand => _navigateToGameCommand ?? (_navigateToGameCommand = new MvxCommand(StartGameImmediately));

        public IMvxCommand CreateChallengeCommand => _createChallengeCommand ?? (_createChallengeCommand = new MvxCommand(
            async () => { await CreateChallenge(); }));

        public IMvxCommand AcceptChallengeCommand
            => _acceptChallengeCommand ?? (_acceptChallengeCommand = new MvxCommand(
                async () => { await AcceptChallenge(); }));
        public IMvxCommand RefuseChallengeCommand
            => _refuseChallengeCommand ?? (_refuseChallengeCommand = new MvxCommand(
                async () => { await RefuseChallenge(); }));


        public override void Appearing()
        {
            TabTitle = Bundle.TabTitle;
        }

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
            if (UseRecommendedKomi)
            {
                CompensationString = Ruleset.GetDefaultCompensation(SelectedRuleset, SelectedGameBoardSize, Handicap,
                    CountingType.Area).ToString(CultureInfo.InvariantCulture);
            }
        }

        // The four button-confirmation methods
        private async Task CreateChallenge()
        {
            if (!Validate())
            {
                return;
            }
            await Bundle.CreateChallenge(this);
            GoBack();
        }
        private async Task AcceptChallenge()
        {
            if (!Validate())
            {
                // Requires validation because of AI.
                return;
            }
            IGame game = await Bundle.AcceptChallenge(this);
            Mvx.RegisterSingleton<IGame>(game);
            ShowViewModel<OnlineGameViewModel>();
        }
        private async Task RefuseChallenge()
        {
            // Refusing does not require validation.
            await Bundle.RefuseChallenge(this);
            this.CloseSelf();
        }

        private void StartGameImmediately()
        {
            if (!Validate())
            {
                return;
            }

            CreateAndRegisterGame();

            // Navigate to specific View Model
            if (_bundle.Style == GameCreationFormStyle.LocalGame)
            {
                ShowViewModel<LocalGameViewModel>();
            }
            else
            {
                ShowViewModel<OnlineGameViewModel>();
            }
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
                Komi(float.Parse(CompensationString, CultureInfo.InvariantCulture)).
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
            ValidationErrorMessage = "";
            if (Bundle.SupportsOnlySquareBoards && !SelectedGameBoardSize.IsSquare)
            {
                ValidationErrorMessage = "You must select a square board.";
                return false;
            }
            if (SelectedGameBoardSize.Width < 2 || SelectedGameBoardSize.Height < 2)
            {
                ValidationErrorMessage = "Board size must be 2x2 or larger.";
                return false;
            }
            float compensation;
            if (float.TryParse(CompensationString, NumberStyles.Any, CultureInfo.InvariantCulture, out compensation))
            {
                float fractionalpart = compensation - (int) compensation;
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (fractionalpart != 0 && fractionalpart != 0.5f)
                {
                    ValidationErrorMessage = "Komi must be a half-integer, such as '0' or '7.5'.";
                    return false;
                }
                // ReSharper restore CompareOfFloatsByEqualityOperator
                if (Bundle.IsKgs)
                {
                    if (compensation < -100 || compensation > 100)
                    {
                        ValidationErrorMessage = "KGS requires that komi be between -100 and 100.";
                        return false;
                    }
                }
            }
            else
            {
                ValidationErrorMessage = "Komi must be a half-integer, such as '0' or '7.5'.";
                return false;
            }
            string errorMessage = "Error loading AI information."; // <-- Should never display.
            if (!BlackPlayerSettings.Validate(this, ref errorMessage))
            {
                ValidationErrorMessage = errorMessage;
                return false;
            }
            if (!WhitePlayerSettings.Validate(this, ref errorMessage))
            {
                ValidationErrorMessage = errorMessage;
                return false;
            }
            string timeErrorMessage = "Error parsing time control settings."; // <-- Should never display.
            if (!TimeControl.Validate(ref timeErrorMessage))
            {
                ValidationErrorMessage = timeErrorMessage;
                return false;
            }
            if (Bundle.IsIgs)
            {
                if (SelectedGameBoardSize.Width < 5)
                {
                    ValidationErrorMessage = "Pandanet does not allow board sizes smaller than 5x5.";
                    return false;
                }
                if (SelectedGameBoardSize.Width > 19)
                {
                    ValidationErrorMessage = "Pandanet does not allow board sizes larger than 19x19.";
                    return false;
                }
                if (SelectedColor == StoneColor.None)
                {
                    // This should never happen.
                    ValidationErrorMessage = "Pandanet does not allow the use of nigiri to choose a color.";
                    return false;
                }
            }
            if (Bundle.IsKgs)
            {
                if (SelectedGameBoardSize.Width > 38)
                {
                    ValidationErrorMessage = "KGS does not allow board sizes larger than 38x38.";
                    return false;
                }
            }
            // TODO Petr: validate time control
            return true;
        }
    }
}
