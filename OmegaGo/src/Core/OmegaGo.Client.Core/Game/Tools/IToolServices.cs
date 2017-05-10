using OmegaGo.Core.Rules;
using System.Threading.Tasks;

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

        /// <summary>
        /// Sets the provided node as the current node.
        /// </summary>
        /// <param name="node">a node to set as current</param>
        void SetNode(GameTreeNode node);

        // Maybe add SetPointerPosition(Position position, int miliseconds) variant to provide a way to lock pointer position for an ammount of time?
        /// <summary>
        /// Sets the current pointer position to a given position.
        /// </summary>
        /// <param name="position">target position to set</param>
        void SetPointerPosition(Position position);

        Task<ToolConfirmationResult> ShowMessage(ToolMessage message);

        /// <summary>
        /// Causes the "pass" sound effect to be played.
        /// </summary>
        void PlayPassSound();

        /// <summary>
        /// Causes the "place stone" sound effect to be played. Possibly also "stones captures" sound effect may be played.
        /// </summary>
        /// <param name="wereThereAnyCaptures">True, if the placed stone triggered any captures.</param>
        void PlayStonePlacementSound(bool wereThereAnyCaptures);
    }
}
