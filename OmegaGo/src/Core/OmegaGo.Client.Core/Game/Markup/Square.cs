namespace OmegaGo.Core.Game.Markup
{
    public sealed class Square : IMarkup
    {
        private Position _position;

        public MarkupKind Kind => MarkupKind.Square;
        public Position Position => _position;

        public Square(Position position)
        {
            _position = position;
        }
    }
}
