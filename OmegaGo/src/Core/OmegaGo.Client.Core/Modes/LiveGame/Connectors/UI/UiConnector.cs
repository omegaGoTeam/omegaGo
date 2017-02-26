using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.UI
{
    public class UiConnector : BaseConnector, IGameConnector, IUiConnectorActions
    {
        private readonly IGameController _gameController;

        public UiConnector(IGameController gameController)
        {
            _gameController = gameController;
        }

        public event EventHandler LifeDeathReturnToMainForced;
        public event EventHandler LifeDeathUndoDeathMarksRequested;
        public event EventHandler LifeDeathUndoDeathMarksForced;
        public event EventHandler LifeDeathDoneRequested;
        public event EventHandler LifeDeathDoneForced;
        public event EventHandler<Position> LifeDeathKillGroupRequested;
        public event EventHandler<Position> LifeDeathKillGroupForced;
        public event EventHandler MainUndoRequested;
        public event EventHandler MainUndoForced;

        /// <summary>
        /// Tries to make a move for the turn player only if it is a human player
        /// </summary>
        /// <param name="position">Position to play</param>
        public void MakeMove(Position position)
        {
            GetHumanAgentOnTurn()?.PlaceStone(position);
        }

        /// <summary>
        /// Resigns the turn player
        /// </summary>
        public void Resign()
        {
            // TODO Petr : make this possible even on opponent's turn, and ask for confirmation first
            GetHumanAgentOnTurn()?.Resign();
        }

        /// <summary>
        /// Makes the player pass
        /// </summary>
        public void Pass()
        {
            GetHumanAgentOnTurn()?.Pass();
        }

        public void MovePerformed(Move move)
        {
        }

        public void RequestLifeDeathDone()
        {
            LifeDeathDoneRequested?.Invoke(this, EventArgs.Empty);
        }

        public void ForceLifeDeathReturnToMain()
        {
            LifeDeathReturnToMainForced?.Invoke(this, EventArgs.Empty);
        }

        public void RequestLifeDeathUndoDeathMarks()
        {
            LifeDeathUndoDeathMarksRequested?.Invoke(this, EventArgs.Empty);
        }

        public void RequestLifeDeathKillGroup(Position selectedPosition)
        {
            LifeDeathKillGroupRequested?.Invoke(this, selectedPosition);
        }

        public void RequestMainUndo()
        {
            MainUndoRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the human player currently on turn
        /// </summary>
        /// <returns>Human agent actions</returns>
        private IHumanAgentActions GetHumanAgentOnTurn() =>
            _gameController.TurnPlayer.Agent as IHumanAgentActions;
    }
}
