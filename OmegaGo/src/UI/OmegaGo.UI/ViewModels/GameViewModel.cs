using OmegaGo.Core;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using OmegaGo.Core.AI;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Helpers;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Connectors.UI;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly IGameSettings _settings = Mvx.Resolve<IGameSettings>();
        private readonly UIConnector _uiConnector;

        private ICommand _passCommand = null;
        private ICommand _resignCommand = null;
        private ICommand _undoCommand = null;

        private string _debugInfo = "n/a";

        private int _maximumMoveIndex = 0;

        private int _previousMoveIndex = -1;


        private int _selectedMoveIndex = 0;

        private string _systemLog;

        private int frames = 0;

        public GameViewModel()
        {
            Game = Mvx.GetSingleton<IGame>();

            _uiConnector = new UIConnector(Game.Controller);

            Game.Controller.RegisterConnector(_uiConnector);
            Game.Controller.CurrentNodeChanged += Game_CurrentGameTreeNodeChanged;
            Game.Controller.CurrentNodeStateChanged += Game_BoardMustBeRefreshed;
            Game.Controller.TurnPlayerChanged += Controller_TurnPlayerChanged;
            Game.Controller.GamePhaseChanged += Controller_GamePhaseChanged;
            
            ObserveDebuggingMessages();

            //TODO Petr: Implement - Observe the Phase changed event and wire up this event in Life and death phase
            //Game.Controller.LifeDeathTerritoryChanged += Controller_LifeDeathTerritoryChanged;
            Game.Controller.GameEnded += Controller_GameEnded;
            BoardViewModel = new BoardViewModel(Game.Info.BoardSize);
            BoardViewModel.BoardTapped += (s, e) => MakeMove(e);
            ChatViewModel = new ChatViewModel();
            //TODO Martin: Implement - online games will have their own ViewModel with specific properties
            //if (Game.Controller.IsOnlineGame)
            //{
            //    ChatViewModel.ChatService = new IgsChatService(Game as IgsGame);
            //    ChatViewModel.HumanAuthor = "You";
            //}
            BlackPortrait = new PlayerPortraitViewModel(Game.Controller.Players.Black);
            WhitePortrait = new PlayerPortraitViewModel(Game.Controller.Players.White);

            //TimelineViewModel = new TimelineViewModel(Game.Controller.GameTree);
            //TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRefreshRequested(e);
        }

        /// <summary>
        /// Observes debugging messages from controller
        /// </summary>
        [Conditional("DEBUG")]
        private void ObserveDebuggingMessages()
        {
            var debuggingMessagesProvider = Game.Controller as IDebuggingMessageProvider;
            if (debuggingMessagesProvider != null)
            {
                debuggingMessagesProvider.DebuggingMessage += (s, e) => SystemLog += e + Environment.NewLine;
            }
        }

        /// <summary>
        /// Pass command from UI
        /// </summary>
        public ICommand PassCommand => _passCommand ?? (_passCommand = new MvxCommand(Pass));

        /// <summary>
        /// Resignation command from UI
        /// </summary>
        public ICommand ResignCommand => _resignCommand ?? (_resignCommand = new MvxCommand(Resign));

        /// <summary>
        /// Undo command from UI
        /// </summary>
        public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new MvxCommand(Undo));

        public string SystemLog
        {
            get { return _systemLog; }
            set { SetProperty(ref _systemLog, value); }
        }

        public IGame Game { get; }

        public BoardViewModel BoardViewModel { get; }

        public PlayerPortraitViewModel BlackPortrait { get; }
        public PlayerPortraitViewModel WhitePortrait { get; }

        public ChatViewModel ChatViewModel { get; }

        public TimelineViewModel TimelineViewModel { get; }

        public int SelectedMoveIndex
        {
            get { return _selectedMoveIndex; }
            set
            {
                SetProperty(ref _selectedMoveIndex, value);
                GameTreeNode whatIsShowing =
                  Game.Controller.GameTree.GameTreeRoot?.GetTimelineView.Skip(value).FirstOrDefault();
                OnBoardRefreshRequested(whatIsShowing);
            }
        }

        public int MaximumMoveIndex
        {
            get { return _maximumMoveIndex; }
            set { SetProperty(ref _maximumMoveIndex, value); }
        }

        public string DebugInfo
        {
            get { return _debugInfo; }
            set { SetProperty(ref _debugInfo, value); }
        }

        private void Game_BoardMustBeRefreshed(object sender, EventArgs e)
        {
            OnBoardRefreshRequested(Game.Controller.CurrentNode);
        }

        private void Controller_GameEnded(object sender, GameEndInformation e)
        {
            _settings.Statistics.GameHasBeenCompleted(Game, e);
            _settings.Quests.Events.GameCompleted(Game, e);
        }

        private void Controller_GamePhaseChanged(object sender, GamePhaseChangedEventArgs eventArgs)
        {
            if (eventArgs.NewPhase.Type == GamePhaseType.LifeDeathDetermination ||
                eventArgs.NewPhase.Type == GamePhaseType.Finished)
            {
                BoardViewModel.BoardControlState.ShowTerritory = true;
            }
            else
            {
                BoardViewModel.BoardControlState.ShowTerritory = false;
            }
        }

        /// <summary>
        /// TODO Petr : this is not yet attached (see to-do above)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Controller_LifeDeathTerritoryChanged(object sender, TerritoryMap e)
        {
            BoardViewModel.BoardControlState.TerritoryMap = e;
        }

        private void Controller_TurnPlayerChanged(object sender, GamePlayer e)
        {
            BoardViewModel.BoardControlState.MouseOverShadowColor = e.Agent.Type == AgentType.Human ?
                e.Info.Color :
                StoneColor.None;
        }

        public void Init()
        {
            Game.Controller.BeginGame();
        }

        private async void Game_CurrentGameTreeNodeChanged(object sender, GameTreeNode e)
        {
            if (e != null)
            {
                await ConsiderPlayingASound(e);
                UpdateTimeline();
            }
        }

        private async Task ConsiderPlayingASound(GameTreeNode e)
        {
            if (e.Branches.Count == 0)
            {
                // This is the final node.
                if (e.Move != null)
                {
                    bool humanPlayed = (this.Game.Controller.Players[e.Move.WhoMoves].IsHuman);
                    bool notificationDemanded =
                        (humanPlayed
                            ? this._settings.Audio.PlayWhenYouPlaceStone
                            : this._settings.Audio.PlayWhenOthersPlaceStone);
                    if (notificationDemanded)
                    {
                        if (e.Move.Kind == MoveKind.PlaceStone)
                        {
                            await Sounds.PlaceStone.PlayAsync();
                            if (e.Move.Captures.Count > 0)
                            {
                                await Sounds.Capture.PlayAsync();
                            }
                        }
                        else if (e.Move.Kind == MoveKind.Pass)
                        {
                            await Sounds.Pass.PlayAsync();
                        }
                    }
                }
            }
        }

        private int _previousMoveIndex = -1;
        private void UpdateTimeline()
        public void Unload()
        {
            //TODO Petr : IMPLEMENT this, but using some ordinary flow like EndGame (it can be part of the IGS Game Controller logic)
            //if (this.Game is IgsGame)
            //{
            //    await ((IgsGame)this.Game).Info.Server.EndObserving((IgsGame)this.Game);
            //}
        }

        public void MakeMove(Position selectedPosition)
        {
            if (Game?.Controller.Phase.Type == GamePhaseType.LifeDeathDetermination)
            {
                //TODO Petr: if life and death will use the same event (probably yes?), then this logic is not really necessary
                //if (Game.Controller.IsOnlineGame)
                //{
                //    await Game.Controller.Server.Commands.LifeDeathMarkDeath(selectedPosition, this.Game.Controller.RemoteInfo);
                //}
                //else
                //{
                //    Game.Controller.LifeDeath_MarkGroupDead(selectedPosition);
                //}
                //Game.Controller.LifeDeath_MarkGroupDead(selectedPosition);
            }
            else
            {
                _uiConnector.MakeMove(selectedPosition);
            }
        }

        /// <summary>
        /// Resignation from UI
        /// </summary>
        private void Resign()
        {
            _uiConnector.Resign();
        }

        /// <summary>
        /// Pass from UI
        /// </summary>
        private void Pass()
        {
            _uiConnector.Pass();
        }

        /// <summary>
        /// Undo from UI
        /// </summary>
        private void Undo()
        {
            //TODO Petr: Implement this, without having to check for type of game (online / local), this should be a part of controller
            //if (VM.Game.Controller.IsOnlineGame)
            //{

            //}
            //else
            //{
            //    VM.Game.Controller.Main_Undo();
            //}
        }

        private void OnBoardRefreshRequested(GameTreeNode boardState)
        {
            BoardViewModel.GameTreeNode = boardState;
            // TODO Petr: GameTree has now LastNodeChanged event - use it to fix this - for now make public and. Called from GameViewModel
            //TimelineViewModel.OnTimelineRedrawRequested();
            frames++;
            DebugInfo = frames.ToString();
            BoardViewModel.Redraw();
        }

        private void UpdateTimeline()
        {
            var primaryTimeline = Game.Controller.GameTree.PrimaryMoveTimeline;
            int newNumber = primaryTimeline.Count() - 1;
            bool autoUpdate = newNumber == 0 || SelectedMoveIndex == newNumber - 1;
            MaximumMoveIndex = newNumber;
            if (autoUpdate && _previousMoveIndex != newNumber)
            {
                SelectedMoveIndex = newNumber;
            }
            _previousMoveIndex = newNumber;
        }
    }

}
