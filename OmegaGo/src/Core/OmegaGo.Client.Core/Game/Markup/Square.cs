namespace OmegaGo.Core.Game.Markup
{
    public sealed class Square : IMarkup
    {
        public MarkupKind Kind => MarkupKind.Square;

        public Position Position { get; }

        public Square(Position position)
        {
            Position = position;
        }
    }
}
