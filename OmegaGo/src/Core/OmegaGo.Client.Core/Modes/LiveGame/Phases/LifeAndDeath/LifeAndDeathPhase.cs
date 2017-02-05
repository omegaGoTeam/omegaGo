using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    class LifeAndDeathPhase : GamePhaseBase, ILifeAndDeathPhase
    {
        private List<GamePlayer> _playersDoneWithLifeDeath = new List<GamePlayer>();

        public LifeAndDeathPhase(GameController gameController) : base(gameController)
        {
        }

        public override GamePhaseType PhaseType => GamePhaseType.LifeDeathDetermination;

        public void MarkGroupDead(Position position)
        {
            var board = Controller.GameTree.LastNode.BoardState;
            if (board[position.X, position.Y] == StoneColor.None)
            {
                return;
            }
            var group = Controller.Ruleset.DiscoverGroup(position, board);
            foreach (var deadStone in group)
            {
                if (!Controller.DeadPositions.Contains(deadStone))
                {
                    Controller.DeadPositions.Add(deadStone);
                }
            }
            _playersDoneWithLifeDeath.Clear();
            Controller.OnBoardMustBeRefreshed();
        }
        public void Done(GamePlayer player)
        {
            Controller.OnDebuggingMessage(player + " has completed his part of the Life/Death determination phase.");
            if (!_playersDoneWithLifeDeath.Contains(player))
            {
                _playersDoneWithLifeDeath.Add(player);
            }
            // TODO maybe infinite recursion here?
           //  this._game.Server?.LifeDeath_Done(this._game);
            if (_playersDoneWithLifeDeath.Count == 2) // && this._game.Server == null)
            {
                // TODO proper scoring
                GoToPhase(GamePhaseType.Finished);
            }
            Controller.OnBoardMustBeRefreshed();
        }

        public void UndoPhase()
        {
            Controller.DeadPositions = new List<Position>();
            _playersDoneWithLifeDeath.Clear();
            Controller.OnBoardMustBeRefreshed();
            // TODO online
        }

        public void Resume()
        {
            Controller.DeadPositions = new List<Position>();
            GoToPhase(GamePhaseType.Main);
            _playersDoneWithLifeDeath.Clear();
            Controller.OnBoardMustBeRefreshed();
            // TODO check
        }

        public override void StartPhase()
        {

            //GoToPhase( GamePhaseType.Finished );
        }
    }
}
