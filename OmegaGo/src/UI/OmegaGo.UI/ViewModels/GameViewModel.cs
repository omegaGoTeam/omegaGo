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
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.UI.Services.Game;

namespace OmegaGo.UI.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        public GameViewModel()
        {
            Game = Mvx.GetSingleton<ILiveGame>();
            Game.Controller.BoardChanged += Game_BoardChanged;            

            BoardViewModel = new BoardViewModel(Game.Info.BoardSize);
            BoardViewModel.BoardTapped += (s, e) => MakeMove(e);

            ChatViewModel = new ChatViewModel();

            TimelineViewModel = new TimelineViewModel(Game.Controller.GameTree);
            TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRefreshRequested(e);
        }

        public ILiveGame Game { get; }

        public BoardViewModel BoardViewModel { get; }

        public ChatViewModel ChatViewModel { get; }

        public TimelineViewModel TimelineViewModel { get; }

        public void Init()
        {
            Game.Controller.BeginGame();
        }

        private void Game_BoardChanged(object sender, GameTreeNode e)
        {
            if (e != null)
                OnBoardRefreshRequested(e);
        }

        public void MakeMove(Position selectedPosition)
        {
            throw new NotImplementedException();
            //the turn player should be here as a property on game view model and we should be able to call its turn method without "seeing" the fact that it sits in the controller
            //(Game.Controller.TurnPlayer.Agent as IReceiverOfLocalActions).Click(
            //    Game.Controller.TurnPlayer.Color,
            //    selectedPosition);
        }

        private void OnBoardRefreshRequested(GameTreeNode boardState)
        {
            BoardViewModel.GameTreeNode = boardState;
            // TODO GameTree should notify - NodeAddedEvent<GameTreeNode>, for now make public and. Called from GameViewModel
            TimelineViewModel.OnTimelineRedrawRequested();

            BoardViewModel.Redraw();
        }
    }
}
