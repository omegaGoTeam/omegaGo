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
        private IObsoleteGame _game;

        private BoardViewModel _boardViewModel;
        private ChatViewModel _chatViewModel;
        private TimelineViewModel _timelineViewModel;

        private BoardState _boardState;
        
        public IObsoleteGame Game
        {
            get { return _game; }
        }

        public BoardViewModel BoardViewModel
        {
            get { return _boardViewModel; }
            set { SetProperty(ref _boardViewModel, value); }
        }

        public ChatViewModel ChatViewModel
        {
            get { return _chatViewModel; }
            set { SetProperty(ref _chatViewModel, value); }
        }

        public TimelineViewModel TimelineViewModel
        {
            get { return _timelineViewModel; }
            set { SetProperty(ref _timelineViewModel, value); }
        }

        public BoardState BoardState
        {
            get { return _boardState; }
            set { SetProperty(ref _boardState, value); }
        }
        
        public GameViewModel()
        {
            _game = Mvx.GetSingleton<IObsoleteGame>();
            _game.BoardChanged += Game_BoardChanged;

            BoardState = new BoardState();
            BoardState.BoardHeight = _game.Info.BoardSize.Height;
            BoardState.BoardWidth = _game.Info.BoardSize.Width;

            BoardViewModel = new BoardViewModel() { BoardState = this.BoardState }; // Mindfuck inception o.O
            BoardViewModel.BoardTapped += (s, e) => MakeMove(e);

            ChatViewModel = new ChatViewModel();

            TimelineViewModel = new TimelineViewModel();
            TimelineViewModel.GameTree = _game.Info.GameTree;
            TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRefreshRequested(e);

            // TODO Could cause problems as this does not wait until the UI is loaded. 
            _game.Controller.BeginGame();
        }

        private void Game_BoardChanged(object sender, GameTreeNode e)
        {
            if (e != null)
                OnBoardRefreshRequested(e);
        }
                
        public void MakeMove(Position selectedPosition)
        {
            (_game.Controller.TurnPlayer.Agent as IReceiverOfLocalActions).Click(
                _game.Controller.TurnPlayer.Color,
                selectedPosition);
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
