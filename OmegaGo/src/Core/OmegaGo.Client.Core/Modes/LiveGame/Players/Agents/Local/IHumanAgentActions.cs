using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents.Local
{
    /// <summary>
    /// This interface is what the user interface classes use to make a player perform an action in the game. This merits further refactoring,
    /// perhaps making a Core-only receiver class that distributes moves to correct players.
    /// </summary>
    public interface IHumanAgentActions
    {
        /// <summary>
        /// GUI interface will call this method on a GUI agent when the user requests that a stone be placed at a position.
        /// </summary>
        /// <param name="selectedPosition">The position to place the stone on.</param>
        void PlaceStone( Position selectedPosition );

        /// <summary>
        /// GUI interface will call this method on a GUI agent when the user requests that the player passes.
        /// </summary>
        void Pass();
        
        /// <summary>
        /// GUI interface will call this method on a GUI agent when the user requests that a player passes.
        /// </summary>
        void Resign();
    }
}
