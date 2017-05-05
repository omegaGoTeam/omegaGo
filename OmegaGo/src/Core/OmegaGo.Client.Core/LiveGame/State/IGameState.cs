using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame.State
{
    /// <summary>
    /// Provides the state of a game
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Gets the player on turn
        /// </summary>
        GamePlayer TurnPlayer { get; }

        /// <summary>
        /// Gets the tree of the game
        /// </summary>
        GameTree GameTree { get; }

        /// <summary>
        /// Players in the game.
        /// </summary>
        PlayerPair Players { get; }

        /// <summary>
        /// Gets the current game phase
        /// </summary>
        IGamePhase Phase { get; }

        /// <summary>
        /// Gets the number of moves that were played
        /// </summary>
        int NumberOfMoves { get; }
    }
}
