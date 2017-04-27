namespace OmegaGo.Core.Game.Markup
{
    public sealed class Circle : IMarkup, IShadowItem
    {
        public MarkupKind MarkupKind => MarkupKind.Circle;

        public ShadowItemKind ShadowItemKind => ShadowItemKind.Circle;

        public Position Position { get; }

        public Circle(Position position)
        {
            Position = position;
        }
    }
}
