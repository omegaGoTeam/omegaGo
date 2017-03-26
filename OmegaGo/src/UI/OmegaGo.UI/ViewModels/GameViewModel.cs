using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Helpers;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Connectors.UI;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using OmegaGo.Core.AI;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.UI.ViewModels
{
    public abstract class GameViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;
        private readonly IGame _game;
        private readonly IDialogService _dialogService;
        private readonly UiConnector _uiConnector;
        private readonly IQuestsManager _questsManager;
        private Assistant Assistant;

        private readonly Dictionary<GamePhaseType, Action<IGamePhase>> _phaseStartHandlers;
        private readonly Dictionary<GamePhaseType, Action<IGamePhase>> _phaseEndHandlers;

        public IGame Game => _game;
        public BoardViewModel BoardViewModel { get; private set; }
        
        protected IGameSettings GameSettings => _gameSettings;
        protected IDialogService DialogService => _dialogService;
        protected UiConnector UiConnector => _uiConnector;
        protected IQuestsManager QuestsManager => _questsManager;
        
        public GameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
        {
            _gameSettings = gameSettings;
            _questsManager = questsManager;
            _dialogService = dialogService;

            _game = Mvx.GetSingleton<IGame>();
            Assistant = new Assistant(gameSettings, _game.Info.IsOnline);
            _game.Controller.GameEnded += (s, e) => OnGameEnded(e);

            BoardViewModel = new BoardViewModel(Game.Info.BoardSize);
            BoardViewModel.BoardTapped += (s, e) => OnBoardTapped(e);

            _uiConnector = new UiConnector(Game.Controller);
            _uiConnector.AiLog += _uiConnector_AiLog;

            _phaseStartHandlers = new Dictionary<GamePhaseType, Action<IGamePhase>>();
            _phaseEndHandlers = new Dictionary<GamePhaseType, Action<IGamePhase>>();
            SetupPhaseChangeHandlers(_phaseStartHandlers, _phaseEndHandlers);

            Game.Controller.RegisterConnector(_uiConnector);
            Game.Controller.MoveUndone += Controller_MoveUndone;
            Game.Controller.CurrentNodeChanged += (s, e) => OnCurrentNodeChanged(e);
            Game.Controller.CurrentNodeStateChanged += (s, e) => OnCurrentNodeStateChanged();
            Game.Controller.TurnPlayerChanged += (s, e) => OnTurnPlayerChanged(e);
            Game.Controller.GamePhaseChanged += (s, e) => OnGamePhaseChanged(e);
            
            ObserveDebuggingMessages();
        }

        private void Controller_MoveUndone(object sender, EventArgs e)
        {
            this.Assistant.MoveUndone();
        }

        private void _uiConnector_AiLog(object sender, string e)
        {
            _systemLog.AppendLine("AI: " + e);
        }



        ////////////////
        // Initial setup overrides      
        ////////////////

        public virtual void Init()
        {
            Game.Controller.BeginGame();
            //UpdateTimeline();
        }

        protected virtual void SetupPhaseChangeHandlers(Dictionary<GamePhaseType, Action<IGamePhase>> phaseStartHandlers, Dictionary<GamePhaseType, Action<IGamePhase>> phaseEndHandlers)
        {

        }

        ////////////////
        // State Changes      
        ////////////////
        
        protected virtual void OnGameEnded(GameEndInformation endInformation)
        {

        }

        protected virtual void OnBoardTapped(Position position)
        {

        }

        protected virtual void OnCurrentNodeChanged(GameTreeNode newNode)
        {

        }

        protected virtual void OnCurrentNodeStateChanged()
        {

        }

        protected virtual void OnTurnPlayerChanged(GamePlayer newPlayer)
        {

        }

        protected virtual void OnGamePhaseChanged(GamePhaseChangedEventArgs phaseState)
        {
            if (phaseState.PreviousPhase != null)
            {
                _phaseEndHandlers.ItemOrDefault(phaseState.PreviousPhase.Type)?
                    .Invoke(phaseState.PreviousPhase);
            }
            
            if (phaseState.NewPhase != null)
            {
                _phaseStartHandlers.ItemOrDefault(phaseState.NewPhase.Type)?
                    .Invoke(phaseState.NewPhase);
            }


            // Should be implemented by the specific registered Action
            //if (phaseState.NewPhase.Type == GamePhaseType.LifeDeathDetermination ||
            //    phaseState.NewPhase.Type == GamePhaseType.Finished)
            //{
            //    BoardViewModel.BoardControlState.ShowTerritory = true;
            //}
            //else
            //{
            //    BoardViewModel.BoardControlState.ShowTerritory = false;
            //}
        }

        public virtual void Unload()
        {
            //TODO Petr : IMPLEMENT this, but using some ordinary flow like EndGame (it can be part of the IGS Game Controller logic)
            //if (this.Game is IgsGame)
            //{
            //    await ((IgsGame)this.Game).Info.Server.EndObserving((IgsGame)this.Game);
            //}
        }


        ////////////////
        // Game View Model Services      
        ////////////////

        private IMvxCommand _getHintCommand;

        public IMvxCommand GetHint
            => _getHintCommand ?? (_getHintCommand = new MvxCommand(GetHintMethod));

        private async void GetHintMethod()
        {
            if (!Assistant.ProvidesHints) return;
            AIDecision hint =
                await
                    Assistant.Hint(this.Game.Info, this.Game.Controller.TurnPlayer, this.Game.Controller.GameTree,
                        this.Game.Controller.TurnPlayer.Info.Color);
            string content = "";
            string title = "";
            switch(hint.Kind)
            {
                case AgentDecisionKind.Resign:
                    title = "You should resign.";
                    content = "The assistant recommends you to resign.\n\nExplanation: " + hint.Explanation;
                    break;
                case AgentDecisionKind.Move:
                    title = hint.Move.ToString();
                    if (hint.Move.Kind == MoveKind.Pass)
                    {
                        content = "You should pass.\n\nExplanation: " + hint.Explanation;
                    }
                    else
                    {
                        content = "You should place a stone at " + hint.Move.Coordinates + ".\n\nExplanation: " +
                                  hint.Explanation;
                    }
                    break;
            }
            await DialogService.ShowAsync(content, title);
        }

        protected void RefreshBoard(GameTreeNode boardState)
        {
            BoardViewModel.GameTreeNode = boardState;
            // TODO Petr: GameTree has now LastNodeChanged event - use it to fix this - for now make public and. Called from GameViewModel
            BoardViewModel.Redraw();
        }

        /// <summary>
        /// Plays a sound if its is appropriate in the given state
        /// </summary>
        /// <param name="currentState">Current game tree node</param>        
        protected async Task PlaySoundIfAppropriate(GameTreeNode state)
        {
            if (state.Branches.Count == 0)
            {
                // This is the final node.
                if (state.Move != null && state.Move.Kind != MoveKind.None)
                {
                    bool humanPlayed = (Game.Controller.Players[state.Move.WhoMoves].IsHuman);
                    bool notificationDemanded =
                        (humanPlayed
                            ? _gameSettings.Audio.PlayWhenYouPlaceStone
                            : _gameSettings.Audio.PlayWhenOthersPlaceStone) &&
                        (!(Game.Info is RemoteGameInfo) || state.MoveNumber > ((RemoteGameInfo) Game.Info).PreplayedMoveCount);
                    if (notificationDemanded)
                    {
                        if (state.Move.Kind == MoveKind.PlaceStone)
                        {
                            await Sounds.PlaceStone.PlayAsync();
                            if (state.Move.Captures.Count > 0)
                            {
                                await Sounds.Capture.PlayAsync();
                            }
                        }
                        else if (state.Move.Kind == MoveKind.Pass)
                        {
                            await Sounds.Pass.PlayAsync();
                        }
                    }
                }
            }
        }

        ////////////////
        // Debugging      
        ////////////////

        private readonly StringBuilder _systemLog = new StringBuilder();
        
        public string SystemLog => _systemLog.ToString();

        /// <summary>
        /// Observes debugging messages from controller
        /// </summary>
        [Conditional("DEBUG")]
        private void ObserveDebuggingMessages()
        {
            var debuggingMessagesProvider = Game.Controller as IDebuggingMessageProvider;
            if (debuggingMessagesProvider != null)
            {
                debuggingMessagesProvider.DebuggingMessage += (s, e) => _systemLog.AppendLine(e);
            }
        }
    }
}
