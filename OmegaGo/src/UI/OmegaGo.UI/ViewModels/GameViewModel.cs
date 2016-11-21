using OmegaGo.Core;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using MvvmCross.Platform;

namespace OmegaGo.UI.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private Game _game;
        private GameController _gameController;

        private ChatViewModel _chatViewModel;
        private TimelineViewModel _timelineViewModel;
        
        public Game Game
        {
            get { return _game; }
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

        public event EventHandler<GameTreeNode> BoardRedrawRequsted;

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
                    player.Agent = new GameViewModelAgent();
                }

                _game.Ruleset = new ChineseRuleset(_game.White, _game.Black, _game.BoardSize);
            }

            _gameController = new GameController(_game);
            _gameController.BoardMustBeRefreshed += () => OnBoardRedraw(_game.GameTree.LastNode);

            ChatViewModel = new ChatViewModel();
            TimelineViewModel = new TimelineViewModel();
            TimelineViewModel.GameTree = _game.GameTree;
            TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRedraw(e);
        }

        public void BeginGame()
        {
            _gameController.BeginGame();
        }
        
        public void MakeMove(Position selectedPosition)
        {
            (_gameController.TurnPlayer.Agent as GameViewModelAgent).DecisionsToMake.Post(
                AgentDecision.MakeMove(Move.PlaceStone(_gameController.TurnPlayer.Color, selectedPosition),
                    "A click."));
        }

        private void OnBoardRedraw(GameTreeNode boardState)
        {
            // TODO GameTree should notify - NodeAddedEvent<GameTreeNode>, for now make public and. Called from GameViewModel
            TimelineViewModel.OnTimelineRedrawRequested();

            BoardRedrawRequsted?.Invoke(this, boardState);
        }
    }
}
