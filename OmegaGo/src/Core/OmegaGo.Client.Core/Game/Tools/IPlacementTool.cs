using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public interface IPlacementTool : ITool
    {
        IShadowItem GetShadowItem(IToolServices toolService);
    }
}
