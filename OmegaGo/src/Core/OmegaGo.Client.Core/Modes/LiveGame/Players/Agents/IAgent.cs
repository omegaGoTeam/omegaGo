using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    public interface IAgent
    {
        /// <summary>
        /// Gets the type of the agent
        /// </summary>
        AgentType Type { get; }

        /// <summary>
        /// Gets the stone color this player uses
        /// </summary>
        StoneColor Color { get; }

        /// <summary>
        /// Indicates how the agent's illegal moves should be handled
        /// </summary>
        IllegalMoveHandling IllegalMoveHandling { get; }

        /// <summary>
        /// Informs the agent, that a move was confirmed
        /// </summary>
        /// <param name="move">Move</param>
        void MovePerformed(Move move);

        /// <summary>
        /// Informs the agent that his las move was illegal
        /// </summary>
        /// <param name="reason">Reason</param>
        void MoveIllegal(MoveResult reason);

        /// <summary>
        /// Assigns the agent to a game
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="gameState">Game state</param>
        void AssignToGame(GameInfo gameInfo, IGameState gameState);

        /// <summary>
        /// Informs the agent that the game is on
        /// </summary>
        void GameInitialized();

        /// <summary>
        /// Informs the agent that the game phase changed
        /// </summary>
        void GamePhaseChanged(GamePhaseType phase);

        /// <summary>
        /// Informs the agent that he is on turn
        /// </summary>
        void OnTurn();

        /// <summary>
        /// Fired when the agent tries to place a stone on a position
        /// </summary>
        event EventHandler<Position> PlaceStone;

        /// <summary>
        /// Fired when the agent resigns
        /// </summary>
        event EventHandler Resign;
        /// <summary>
        /// Fired when the agent passes
        /// </summary>
        event EventHandler Pass;

        void PleaseMakeAMove();
    }
}
