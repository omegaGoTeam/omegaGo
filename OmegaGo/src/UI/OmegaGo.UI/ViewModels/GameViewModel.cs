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

        public GameViewModel()
        {
            _game = new Game();
            _game.BoardSize = new GameBoardSize(19);
            _game.Players.Add(new Player("Black Player", "??", _game));
            _game.Players.Add(new Player("White Player", "??", _game));
            foreach(var player in _game.Players)
            {
                player.Agent = new GameViewModelAgent();
            }
            _game.Ruleset = new ChineseRuleset(_game.White, _game.Black, _game.BoardSize);
            _gameController = new GameController(_game);
            _gameController.BoardMustBeRefreshed += _gameController_BoardMustBeRefreshed;

            ChatViewModel = new ChatViewModel();
            TimelineViewModel = new TimelineViewModel();
            TimelineViewModel.GameTree = _game.GameTree;
        }

        public void BeginGame()
        {
            _gameController.BeginGame();
        }

        public event Action refreshThaBoard;

        private void _gameController_BoardMustBeRefreshed()
        {
            refreshThaBoard?.Invoke();
        }

        public void ClickOnPosition(Position selectedPosition)
        {
            (_gameController.TurnPlayer.Agent as GameViewModelAgent).DecisionsToMake.Post(
                AgentDecision.MakeMove(Move.Create(_gameController.TurnPlayer.Color, selectedPosition),
                    "A click."));
        }
        class GameViewModelAgent : AgentBase, IAgent
        {
            public BufferBlock<AgentDecision> DecisionsToMake = new BufferBlock<AgentDecision>();


            public GameViewModelAgent()
            {
            }

            public async Task<AgentDecision> RequestMove(Game game)
            {
                AgentDecision storedDecision = GetStoredDecision(game);
                if (storedDecision != null)
                {
                    return storedDecision;
                }
                AgentDecision decision = await DecisionsToMake.ReceiveAsync();
                return decision;
            }

            public IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;
        }
    }
}
