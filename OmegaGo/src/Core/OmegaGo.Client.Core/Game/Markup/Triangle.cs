namespace OmegaGo.Core.Game.Markup
{
    public sealed class Triangle : IMarkup
    {
        public MarkupKind Kind => MarkupKind.Triangle;

        public Position Position { get; }

        public Triangle(Position position)
        {
            Position = position;
        }
    }
}
