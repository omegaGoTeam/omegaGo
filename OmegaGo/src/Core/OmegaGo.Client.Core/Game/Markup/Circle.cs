namespace OmegaGo.Core.Game.Markup
{
    public sealed class Circle : IMarkup
    {
        private Position _position;

        public MarkupKind Kind => MarkupKind.Circle;
        public Position Position => _position;

        public Circle(Position position)
        {
            _position = position;
        }
    }
}
