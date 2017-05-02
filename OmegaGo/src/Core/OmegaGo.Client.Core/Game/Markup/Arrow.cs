namespace OmegaGo.Core.Game.Markup
{
    public sealed class Arrow : IMarkup
    {
        public MarkupKind MarkupKind => MarkupKind.Arrow;

        public Position From { get; }

        public Position To { get; }

        public Arrow(Position from, Position to)
        {
            From = from;
            To = to;
        }
    }
}
