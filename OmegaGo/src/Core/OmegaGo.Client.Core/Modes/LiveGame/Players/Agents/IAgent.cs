using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    public interface IAgent
    {
        void MovePerformed(Move move, GamePlayer player);

        void MoveIllegal(MoveResult move);

        event EventHandler Move;

        IllegalMoveHandling IllegalMoveHandling { get; }

        /// <summary>
        /// Informs the agent that he is on turn
        /// </summary>
        void AgentOnTurn();
    }
}
