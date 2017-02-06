namespace OmegaGo.Core.Game.Markup
{
    public sealed class Circle : IMarkup
    {
        public MarkupKind Kind => MarkupKind.Circle;
        public Position Position { get; }

        public Circle(Position position)
        {
            Position = position;
        }
    }
}
