using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    /// <summary>
    /// Interface for tools, which place an item to the board.
    /// </summary>
    public interface IPlacementTool : ITool
    {
        /// <summary>
        /// Returns a shadow item (stone, markup or empty item) that should be shown over the given position.
        /// </summary>
        /// <param name="toolService"></param>
        /// <returns></returns>
        IShadowItem GetShadowItem(IToolServices toolService);
    }
}
