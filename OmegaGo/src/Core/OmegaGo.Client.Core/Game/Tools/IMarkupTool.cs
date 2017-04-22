using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public interface IMarkupTool : ITool
    {
        IMarkup GetShadowItem(IToolServices toolService);
    }
}
