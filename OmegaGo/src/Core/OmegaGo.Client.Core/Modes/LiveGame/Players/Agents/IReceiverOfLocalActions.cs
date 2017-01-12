using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// This interface is what the user interface classes use to make a player perform an action in the game. This merits further refactoring,
    /// perhaps making a Core-only receiver class that distributes moves to correct players.
    /// </summary>
    public interface IReceiverOfLocalActions
    {
        /// <summary>
        /// GUI interface will call this method on a GUI agent when the user requests that a stone be placed at a position.
        /// </summary>
        /// <param name="color">The color of the placed stone.</param>
        /// <param name="selectedPosition">The position to place the stone on.</param>
        void Click(StoneColor color, Position selectedPosition);

        /// <summary>
        /// GUI interface will call this method on a GUI agent when the user requests that a player passes.
        /// </summary>
        /// <param name="color">The color of this agent's player.</param>
        void ForcePass(StoneColor color);
    }
}
