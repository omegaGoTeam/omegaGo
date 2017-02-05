using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Time;
using OmegaGo.UI.Services.GameCreationBundle;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class GameCreationViewModel : ViewModelBase
    {
        private int _whiteHandicap;
        private float _compensation = 0;
        private GameBoardSize _selectedGameBoardSize = new GameBoardSize(19);
        private RulesetType _selectedRuleset = RulesetType.Chinese;
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();

        private ICommand _setDefaultCompensationCommand;
        private IMvxCommand _navigateToGameCommand;
        
        public GameCreationViewModel()
        {
            WhiteHandicap = 0;
            _customWidth = _settings.Interface.BoardWidth;
            _customHeight = _settings.Interface.BoardHeight;
            SetCustomBoardSize();
            var bundle = Mvx.GetSingleton<GameCreationBundle>();
            bundle?.OnLoad(this);
        }

        public PlayerSettingsViewModel BlackPlayerSettings { get; } = new PlayerSettingsViewModel(GameCreationViewModel.playerList[0]);
        public PlayerSettingsViewModel WhitePlayerSettings { get; } = new PlayerSettingsViewModel(GameCreationViewModel.playerList[0]);
        private TimeControlSettingsViewModel _timeControl = new TimeControlSettingsViewModel();
        public TimeControlSettingsViewModel TimeControl
        {
            get { return _timeControl; }
            set { SetProperty(ref _timeControl, value); }
        }
        
        public ObservableCollection<TimeControlStyle> TimeControlStyles { get; } = new ObservableCollection
            <TimeControlStyle>
        {
            TimeControlStyle.None,
            TimeControlStyle.Absolute,
            TimeControlStyle.Canadian
        };

        /// <summary>
        /// Default offered game board sizes
        /// </summary>
        public ObservableCollection<GameBoardSize> BoardSizes { get; } =
            new ObservableCollection<GameBoardSize>() {
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
                RaisePropertyChanged(()=>SampleGameBoard);
                _customHeight = value.Height;
                _customWidth = value.Width;
                RaisePropertyChanged(nameof(CustomHeight));
                RaisePropertyChanged(nameof(CustomWidth));
                _settings.Interface.BoardWidth = _customWidth;
                _settings.Interface.BoardHeight = _customHeight;
                SetDefaultCompensation();
            }
        }

        private string _server = "Local Game";
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

        public ObservableCollection<GameCreationViewPlayer> PossiblePlayers { get; } = new ObservableCollection<GameCreationViewPlayer>(
               GameCreationViewModel.playerList
            );

        private static List<GameCreationViewPlayer> playerList = new List<GameCreationViewPlayer>(
            new GameCreationViewPlayer[]
            {
                new GameCreationViewHumanPlayer("Human")
            }.Concat(
                Core.AI.AISystems.AiPrograms.Select(program => new GameCreationViewAiPlayer(program))
                )
            );

        private GameCreationViewPlayer _blackPlayer = GameCreationViewModel.playerList[0];
        private GameCreationViewPlayer _whitePlayer = GameCreationViewModel.playerList[0];
        public GameCreationViewPlayer BlackPlayer
        {
            get { return _blackPlayer; }
            set { SetProperty(ref _blackPlayer, value);
                BlackPlayerSettings.ChangePlayer(value);
            }
        }
        public GameCreationViewPlayer WhitePlayer
        {
            get { return _whitePlayer; }
            set { SetProperty(ref _whitePlayer, value);
                WhitePlayerSettings.ChangePlayer(value);
            }
        }

        private int _customWidth = 19;
        private int _customHeight = 19;
        public string CustomWidth
        {
            get { return _customWidth.ToString(); }
            set { SetProperty(ref _customWidth, int.Parse(value));
                SetCustomBoardSize();
              
            } // TODO check for exceptions
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

        public string CustomHeight
        {
            get { return _customHeight.ToString(); }
            set { SetProperty(ref _customHeight, int.Parse(value));
                SetCustomBoardSize();
            } // TODO check for exceptions
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

        /// <summary>
        /// Sample game board for preview
        /// </summary>
        public GameBoard SampleGameBoard => new GameBoard(SelectedGameBoardSize);


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
            if (!Validate())
            {
                return;
            }
            GamePlayer blackPlayer = BlackPlayer.Build(StoneColor.Black, TimeControl);
            GamePlayer whitePlayer = WhitePlayer.Build(StoneColor.White, TimeControl);

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

        private bool Validate()
        {
            if (IgsLimitation)
            {
                if (!SelectedGameBoardSize.IsSquare) return false;
                if (SelectedGameBoardSize.Width < 5 || SelectedGameBoardSize.Width > 19) return false;
            }
            return true;
        }

        public bool IgsLimitation { get; set; }
        public bool RulesetCanBeSelected => !IgsLimitation;
    }
}
