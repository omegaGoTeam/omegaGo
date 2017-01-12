namespace OmegaGo.Core.Game.Markup
{
    public sealed class Arrow : IMarkup
    {
        private Position _from;
        private Position _to;

        public MarkupKind Kind => MarkupKind.Arrow;
        public Position From => _from;
        public Position To => _to;
        
        public Arrow(Position from, Position to)
        {
            _from = from;
            _to = to;
        }
    }
}
