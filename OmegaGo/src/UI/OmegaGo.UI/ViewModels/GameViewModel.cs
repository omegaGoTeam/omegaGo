﻿using OmegaGo.Core;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Online.Igs;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();
        public GameViewModel()
        {
            Game = Mvx.GetSingleton<ILiveGame>();
            Game.Controller.CurrentGameTreeNodeChanged += Game_CurrentGameTreeNodeChanged;
            Game.Controller.TurnPlayerChanged += Controller_TurnPlayerChanged;
            Game.Controller.GamePhaseChanged += Controller_GamePhaseChanged;
            Game.Controller.DebuggingMessage += (s, e) => SystemLog += e + Environment.NewLine;
            Game.Controller.LifeDeathTerritoryChanged += Controller_LifeDeathTerritoryChanged;
            Game.Controller.GameEnded += Controller_GameEnded;
            BoardViewModel = new BoardViewModel(Game.Info.BoardSize);
            BoardViewModel.BoardTapped += (s, e) => MakeMove(e);
            ChatViewModel = new ChatViewModel();
            if (Game.Controller.IsOnlineGame)
            {
                ChatViewModel.ChatService = new IgsChatService(Game as IgsGame);
                ChatViewModel.HumanAuthor = "You";
            }
            BlackPortrait = new PlayerPortraitViewModel(Game.Controller.Players.Black);
            WhitePortrait = new UserControls.ViewModels.PlayerPortraitViewModel(Game.Controller.Players.White);

            //TimelineViewModel = new TimelineViewModel(Game.Controller.GameTree);
            //TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRefreshRequested(e);
        }

        private void Controller_GameEnded(object sender, GameEndInformation e)
        {
            _settings.Statistics.GameHasBeenCompleted(this.Game, e);
            _settings.Quests.Events.GameCompleted(this.Game, e);
        }

        private void Controller_GamePhaseChanged(object sender, Core.Modes.LiveGame.Phases.GamePhaseType e)
        {
            if (e == Core.Modes.LiveGame.Phases.GamePhaseType.LifeDeathDetermination ||
                e == Core.Modes.LiveGame.Phases.GamePhaseType.Finished)
            {
                BoardViewModel.BoardControlState.ShowTerritory = true;
            }
            else
            {
                BoardViewModel.BoardControlState.ShowTerritory = false;
            }
        }

        private void Controller_LifeDeathTerritoryChanged(object sender, TerritoryMap e)
        {
            BoardViewModel.BoardControlState.TerritoryMap = e;
        }

        private void Controller_TurnPlayerChanged(object sender, Core.Modes.LiveGame.Players.GamePlayer e)
        {
            if (e.IsHuman)
            {
                BoardViewModel.BoardControlState.MouseOverShadowColor =
                    e.Info.Color;
            }
            else
            {
                BoardViewModel.BoardControlState.MouseOverShadowColor = StoneColor.None;
            }
        }

        private string _systemLog;
        public string SystemLog
        {
            get { return _systemLog; }
            set { SetProperty(ref _systemLog, value); }
        }

        public ILiveGame Game { get; }

        public BoardViewModel BoardViewModel { get; }

        public PlayerPortraitViewModel BlackPortrait { get; }
        public PlayerPortraitViewModel WhitePortrait { get; }

        public ChatViewModel ChatViewModel { get; }

        public TimelineViewModel TimelineViewModel { get; }

        private int _selectedMoveIndex = 0;
        public int SelectedMoveIndex
        {
            get { return _selectedMoveIndex; }
            set {
                SetProperty(ref _selectedMoveIndex, value);
                GameTreeNode whatIsShowing =
                  Game.Controller.GameTree.GameTreeRoot?.GetTimelineView.Skip(value).FirstOrDefault();
                OnBoardRefreshRequested(whatIsShowing);
            }
        }

        private int _maximumMoveIndex = 0;
        public int MaximumMoveIndex
        {
            get { return _maximumMoveIndex; }
            set { SetProperty(ref _maximumMoveIndex, value); }
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
        {
            var primaryTimeline = this.Game.Controller.GameTree.PrimaryMoveTimeline;
            int newNumber = primaryTimeline.Count() - 1;
            bool autoUpdate = newNumber == 0 || SelectedMoveIndex == newNumber - 1;
            MaximumMoveIndex = newNumber;
            if (autoUpdate && _previousMoveIndex != newNumber)
            {
                SelectedMoveIndex = newNumber;
            }
            _previousMoveIndex = newNumber;
        }

        public async void MakeMove(Position selectedPosition)
        {
            if (Game?.Controller.Phase == Core.Modes.LiveGame.Phases.GamePhaseType.LifeDeathDetermination)
            {
                if (Game.Controller.IsOnlineGame)
                {
                    await Game.Controller.Server.Commands.LifeDeathMarkDeath(selectedPosition, this.Game.Controller.RemoteInfo);
                }
                else
                {
                    Game.Controller.LifeDeath_MarkGroupDead(selectedPosition);
                }
                Game.Controller.LifeDeath_MarkGroupDead(selectedPosition);
            }
            else
            {
                if (Game?.Controller.TurnPlayer?.IsHuman ?? false)
                {
                    (Game.Controller.TurnPlayer.Agent as IHumanAgentActions)?.PlaceStone(selectedPosition);
                }
            }
            //the turn player should be here as a property on game view model and we should be able to call its turn method without "seeing" the fact that it sits in the controller
        }

        private void OnBoardRefreshRequested(GameTreeNode boardState)
        {
            BoardViewModel.GameTreeNode = boardState;
            // TODO GameTree should notify - NodeAddedEvent<GameTreeNode>, for now make public and. Called from GameViewModel
            //TimelineViewModel.OnTimelineRedrawRequested();
            frames++;
            DebugInfo = frames.ToString();
            BoardViewModel.Redraw();
        }

        private int frames = 0;
        private string _debugInfo = "n/a";
        public string DebugInfo
        {
            get { return _debugInfo; }
            set { SetProperty(ref _debugInfo, value); }
        }

        public async void Unload()
        {
            if (this.Game is IgsGame)
            {
                await ((IgsGame) this.Game).Metadata.Server.EndObserving((IgsGame) this.Game);
            }
        }
    }
    
}
