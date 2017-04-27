namespace OmegaGo.Core.Game.Markup
{
    /// <summary>
    /// Markup for dimming a given area on board
    /// </summary>
    public sealed class AreaDim : IMarkup
    {
        public MarkupKind MarkupKind => MarkupKind.AreaDim;

        public Position From { get; }

        public Position To { get; }

        public AreaDim(Position from, Position to)
        {
            From = from;
            To = to;
        }
    }
}
