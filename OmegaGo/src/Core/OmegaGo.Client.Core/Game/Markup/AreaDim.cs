namespace OmegaGo.Core.Game.Markup
{
    public sealed class AreaDim : IMarkup
    {
        public MarkupKind Kind => MarkupKind.AreaDim;

        public Position From { get; }

        public Position To { get; }

        public AreaDim(Position from, Position to)
        {
            From = from;
            To = to;
        }
    }
}
