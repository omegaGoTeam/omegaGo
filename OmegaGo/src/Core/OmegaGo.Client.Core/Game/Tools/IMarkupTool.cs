using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    interface IMarkupTool : ITool
    {
        MarkupKind Markup { get; }

        IMarkup GetShadowItem(IToolServices toolService);
    }
}
