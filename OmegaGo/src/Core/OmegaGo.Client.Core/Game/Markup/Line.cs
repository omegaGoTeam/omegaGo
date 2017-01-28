namespace OmegaGo.Core.Game.Markup
{
    public sealed class Line : IMarkup
    {
        private Position _from;
        private Position _to;

        public MarkupKind Kind => MarkupKind.Line;
        public Position From => _from;
        public Position To => _to;

        public Line(Position from, Position to)
        {
            _from = from;
            _to = to;
        }
    }
}
