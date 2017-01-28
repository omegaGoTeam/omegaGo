namespace OmegaGo.Core.Game.Markup
{
    public sealed class Triangle : IMarkup
    {
        private Position _position;

        public MarkupKind Kind => MarkupKind.Triangle;
        public Position Position => _position;

        public Triangle(Position position)
        {
            _position = position;
        }
    }
}
