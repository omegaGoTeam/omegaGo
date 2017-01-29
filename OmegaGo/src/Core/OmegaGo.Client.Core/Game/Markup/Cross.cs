namespace OmegaGo.Core.Game.Markup
{
    public sealed class Cross : IMarkup
    {
        private Position _position;

        public MarkupKind Kind => MarkupKind.Cross;
        public Position Position => _position;

        public Cross(Position position)
        {
            _position = position;
        }
    }
}
