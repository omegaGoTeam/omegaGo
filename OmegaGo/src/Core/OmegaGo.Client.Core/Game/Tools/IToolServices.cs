using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game.Tools
{
    public interface IToolServices
    {
        /// <summary>
        /// Gets the active ruleset for the current game.
        /// </summary>
        IRuleset Ruleset { get; }

        /// <summary>
        /// Gets the GameTree representing the current game.
        /// </summary>
        GameTree GameTree { get; }

        /// <summary>
        /// Gets the current game node on which the tools should operate.
        /// </summary>
        GameTreeNode Node { get; }

        /// <summary>
        /// Gets the board coordinates of the current pointer position.
        /// </summary>
        Position PointerOverPosition { get; }
    }
}
