using OmegaGo.Core;
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
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.Services.Game;

namespace OmegaGo.UI.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        public GameViewModel()
        {
            Game = Mvx.GetSingleton<ILiveGame>();
            Game.Controller.CurrentGameTreeNodeChanged += Game_CurrentGameTreeNodeChanged;
            Game.Controller.TurnPlayerChanged += Controller_TurnPlayerChanged;
            Game.Controller.DebuggingMessage += (s, e) => SystemLog += e + Environment.NewLine;
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

        public void Init()
        {
            Game.Controller.BeginGame();
        }

        private void Game_CurrentGameTreeNodeChanged(object sender, GameTreeNode e)
        {
            if (e != null)
                OnBoardRefreshRequested(e);
        }

        public void MakeMove(Position selectedPosition)
        {
            if (Game?.Controller?.TurnPlayer?.IsHuman ?? false)
            {
                (Game.Controller.TurnPlayer.Agent as IHumanAgentActions)?.PlaceStone(selectedPosition);
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
