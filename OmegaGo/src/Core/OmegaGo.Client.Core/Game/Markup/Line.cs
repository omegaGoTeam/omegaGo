namespace OmegaGo.Core.Game.Markup
{
    public sealed class Line : IMarkup
    {
        public MarkupKind MarkupKind => MarkupKind.Line;

        public Position From { get; }

        public Position To { get; }

        public Line(Position from, Position to)
        {
            From = from;
            To = to;
        }
    }
}
