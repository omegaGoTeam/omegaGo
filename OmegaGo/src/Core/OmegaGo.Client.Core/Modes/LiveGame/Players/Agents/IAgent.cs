using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
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
        /// Informs the agent that the game is on
        /// </summary>
        void GameInitialized();

        /// <summary>
        /// Informs the agent that the game phase changed
        /// </summary>
        void GamePhaseChanged( GamePhaseType phase );

        /// <summary>
        /// Informs the agent that he is on turn
        /// </summary>
        void OnTurn();
    }
}
