namespace OmegaGo.Core.Game.Markup
{
    public sealed class AreaDim : IMarkup
    {
        private Position _from;
        private Position _to;

        public MarkupKind Kind => MarkupKind.AreaDim;

        public Position From => _from;
        public Position To => _to;

        public AreaDim(Position from, Position to)
        {
            _from = from;
            _to = to;
        }
    }
}
