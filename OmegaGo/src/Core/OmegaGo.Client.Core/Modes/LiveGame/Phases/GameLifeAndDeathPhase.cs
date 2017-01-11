using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    class GameLifeAndDeathPhase : IGameLifeAndDeathPhase
    {
        public GamePhaseType PhaseType => GamePhaseType.LifeDeathDetermination;

        public void MarkGroupDead(Position position)
        {
            var board = FastBoard.CreateBoardFromGame(_game);
            if (board[position.X, position.Y] == StoneColor.None)
            {
                return;
            }
            var group = _game.Ruleset.DiscoverGroup(position, board);
            foreach (var deadStone in group)
            {
                if (!this._deadPositions.Contains(deadStone))
                {
                    this._deadPositions.Add(deadStone);
                }
            }
            _playersDoneWithLifeDeath.Clear();

            OnBoardMustBeRefreshed();
        }
        public void LifeDeath_Done(GamePlayer player)
        {
            OnDebuggingMessage(player + " has completed his part of the Life/Death determination phase.");
            if (!_playersDoneWithLifeDeath.Contains(player))
            {
                _playersDoneWithLifeDeath.Add(player);
            }
            // TODO maybe infinite recursion here?
            this._game.Server?.LifeDeath_Done(this._game);
            if (_playersDoneWithLifeDeath.Count == 2 && this._game.Server == null)
            {
                SetGamePhase(GamePhase.Completed);
            }
            OnBoardMustBeRefreshed();
        }

        public void LifeDeath_UndoPhase()
        {
            this._deadPositions = new List<Position>();
            _playersDoneWithLifeDeath.Clear();
            OnBoardMustBeRefreshed();
        }

        public void LifeDeath_Resume()
        {
            this._deadPositions = new List<Position>();
            SetGamePhase(GamePhase.MainPhase);
            _playersDoneWithLifeDeath.Clear();
            OnBoardMustBeRefreshed();
            MainPhase_AskPlayerToMove(_game.Black);
        }

    }
}
