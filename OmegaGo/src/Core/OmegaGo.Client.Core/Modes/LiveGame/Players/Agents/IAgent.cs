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

        void MovePerformed(Move move, GamePlayer player);

        void MoveIllegal(MoveResult move);

        IllegalMoveHandling IllegalMoveHandling { get; }

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
        void GamePhaseChanged( GamePhaseType phase );

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
    }
}
