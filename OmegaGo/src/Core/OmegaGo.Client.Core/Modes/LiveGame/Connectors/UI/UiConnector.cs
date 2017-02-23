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
            //UI does not care about the fact that a move was actually performed
        }

        public void LifeDeath_RequestDone()
        {
            LifeDeathRequestDone?.Invoke(this, EventArgs.Empty);
        }
        public void LifeDeath_ForceReturnToMain()
        {
            LifeDeathForceReturnToMain?.Invoke(this, EventArgs.Empty);
        }
        public void LifeDeath_RequestUndoDeathMarks()
        {
            LifeDeathRequestUndoDeathMarks?.Invoke(this, EventArgs.Empty);

        }

        public event EventHandler LifeDeathForceReturnToMain;
        public event EventHandler LifeDeathRequestUndoDeathMarks;
        public event EventHandler LifeDeathForceUndoDeathMarks;
        public event EventHandler LifeDeathRequestDone;
        public event EventHandler LifeDeathForceDone;
        public event EventHandler<Position> LifeDeathRequestKillGroup;
        public event EventHandler<Position> LifeDeathForceKillGroup;


        /// <summary>
        /// Gets the human player currently on turn
        /// </summary>
        /// <returns>Human agent actions</returns>
        private IHumanAgentActions GetHumanAgentOnTurn() =>
            _gameController.TurnPlayer.Agent as IHumanAgentActions;


        public void LifeDeath_RequestKillGroup(Position selectedPosition)
        {
            LifeDeathRequestKillGroup?.Invoke(this, selectedPosition);
        }
    }
}
