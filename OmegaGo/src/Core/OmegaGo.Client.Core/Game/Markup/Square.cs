namespace OmegaGo.Core.Game.Markup
{
    public sealed class Square : IMarkup, IShadowItem
    {
        public MarkupKind MarkupKind => MarkupKind.Square;

        public ShadowItemKind ShadowItemKind => ShadowItemKind.Square;

        public Position Position { get; }

        public Square(Position position)
        {
            Position = position;
        }
    }
}
