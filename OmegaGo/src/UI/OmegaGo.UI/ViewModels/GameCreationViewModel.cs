using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OmegaGo.Core.AI;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.AI;
using OmegaGo.Core.Modes.LiveGame.Players.Local;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.UserControls.ViewModels;

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
        
        public GameCreationViewModel()
        {
            Difficulties = new ObservableCollection<string>() { Localizer.Easy, Localizer.Medium, Localizer.Hard };
            SelectedDifficulty = Difficulties.First();
            StoneColors = new ObservableCollection<string>() { "Human", "AI (Michi)", "AI (Oakfoam)", "AI (Joker23)" };
            SelectedStoneColor = StoneColors.First();
            WhiteHandicap = 0;
        }

        public PlayerSettingsViewModel BlackPlayerSettings { get; } = new PlayerSettingsViewModel(_playerList[0]);
        
        public PlayerSettingsViewModel WhitePlayerSettings { get; } = new PlayerSettingsViewModel(_playerList[0]);

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

        public ObservableCollection<GameCreationViewPlayer> PossiblePlayers { get; } = new ObservableCollection<GameCreationViewPlayer>(
               _playerList
            );

        private static List<GameCreationViewPlayer> _playerList = new List<GameCreationViewPlayer>(
            new GameCreationViewPlayer[]
            {
                new GameCreationViewHumanPlayer("Human")
            }.Concat(
                OmegaGo.Core.AI.AISystems.AiPrograms.Select(program => new GameCreationViewAiPlayer(program))
                )
            );

        private GameCreationViewPlayer _blackPlayer = _playerList[0];
        private GameCreationViewPlayer _whitePlayer = _playerList[0];
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
            GamePlayer blackPlayer = BlackPlayer.Build(StoneColor.Black);
            GamePlayer whitePlayer = WhitePlayer.Build(StoneColor.White);

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

        public abstract class GameCreationViewPlayer
        {
            public string Name { get; protected set; }
            public abstract string Description { get; }
            public abstract bool IsAi { get; }

            public abstract GamePlayer Build(StoneColor color);

            public override string ToString()
            {
                return Name;
            }
        }
        public class GameCreationViewHumanPlayer : GameCreationViewPlayer
        {
            public GameCreationViewHumanPlayer(string name)
            {
                this.Name = name;
            }

            public override string Description
                => "This means that you (or a friend) will play this color on this device.";

            public override bool IsAi => false;

            public override GamePlayer Build(StoneColor color)
            {
                return new HumanPlayerBuilder(color)
                    .Name(color.ToString())
                    .Rank("NR")
                    .Build();
            }
        }
        public class GameCreationViewAiPlayer : GameCreationViewPlayer
        {
            private IAIProgram ai;
            public GameCreationViewAiPlayer(OmegaGo.Core.AI.IAIProgram program)
            {
                this.Name = "AI: " + program.Name;
                this.ai = program;
            }

            public override string Description => ai.Description;
            public override bool IsAi => true;
            public AICapabilities Capabilities => ai.Capabilities;

            public override GamePlayer Build(StoneColor color)
            {
                IAIProgram newInstance = (IAIProgram)Activator.CreateInstance(ai.GetType());
                return new AiPlayerBuilder(color)
                    .Name(ai.Name + "(" + color.ToIgsCharacterString() + ")")
                    .Rank("NR")
                    .AiProgram(newInstance)
                    .Build();
            }
        }

    }
 
}
