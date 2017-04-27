namespace OmegaGo.Core.Game.Markup
{
    public sealed class Cross : IMarkup, IShadowItem
    {
        public MarkupKind MarkupKind => MarkupKind.Cross;

        public ShadowItemKind ShadowItemKind => ShadowItemKind.Cross;

        public Position Position { get; }

        public Cross(Position position)
        {
            Position = position;
        }
    }
}
