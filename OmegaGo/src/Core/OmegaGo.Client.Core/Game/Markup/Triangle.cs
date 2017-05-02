namespace OmegaGo.Core.Game.Markup
{
    public sealed class Triangle : IMarkup, IShadowItem
    {
        public MarkupKind MarkupKind => MarkupKind.Triangle;

        public ShadowItemKind ShadowItemKind => ShadowItemKind.Triangle;

        public Position Position { get; }

        public Triangle(Position position)
        {
            Position = position;
        }
    }
}
