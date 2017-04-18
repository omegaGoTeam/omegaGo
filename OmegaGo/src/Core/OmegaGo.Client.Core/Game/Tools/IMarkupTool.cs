using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public interface IMarkupTool : ITool
    {
        MarkupKind Markup { get; }

        IMarkup GetShadowItem(IToolServices toolService);
    }
}
