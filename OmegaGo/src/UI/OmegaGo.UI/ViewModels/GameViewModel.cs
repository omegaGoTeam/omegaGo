using OmegaGo.Core;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Game;

namespace OmegaGo.UI.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private Game _game;
        private GameController _gameController;

        private BoardViewModel _boardViewModel;
        private ChatViewModel _chatViewModel;
        private TimelineViewModel _timelineViewModel;

        private BoardState _boardState;
        
        public Game Game
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
            try
            {
                // Works for SinglePlayer ViewModel
                _game = Mvx.GetSingleton<Game>();
            }
            catch(Exception)
            {
                // For all others game parameters not yet implemented
                // Do it old way
                _game = new Game();
                _game.BoardSize = new GameBoardSize(19);

                _game.Players.Add(new Player("Black Player", "??", _game));
                _game.Players.Add(new Player("White Player", "??", _game));
                foreach (var player in _game.Players)
                {
                    player.Agent = new Core.Agents.GameViewModelAgent();
                }

                _game.Ruleset = new ChineseRuleset(_game.White, _game.Black, _game.BoardSize);
            }

            _gameController = new GameController(_game);
            _gameController.BoardMustBeRefreshed += () =>
            {
                OnBoardRefreshRequested(_game.GameTree.LastNode);
            };

            BoardState = new BoardState();
            BoardState.BoardHeight = _game.BoardSize.Height;
            BoardState.BoardWidth = _game.BoardSize.Width;

            BoardViewModel = new BoardViewModel() { BoardState = BoardState }; // Mindfuck inception o.O
            BoardViewModel.BoardTapped += (s, e) => MakeMove(e);

            ChatViewModel = new ChatViewModel();

            TimelineViewModel = new TimelineViewModel();
            TimelineViewModel.GameTree = _game.GameTree;
            TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRefreshRequested(e);

            // TODO Could cause problems as this does not wait until the UI is loaded. 
            this.BeginGame();
        }

        public void BeginGame()
        {
            _gameController.BeginGame();
        }
        
        public void MakeMove(Position selectedPosition)
        {
            _gameController.TurnPlayer.Agent.Click(_gameController.TurnPlayer.Color, selectedPosition);
              /*  
              
            */
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
