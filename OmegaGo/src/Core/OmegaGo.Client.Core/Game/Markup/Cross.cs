namespace OmegaGo.Core.Game.Markup
{
    public sealed class Cross : IMarkup
    {
        public MarkupKind Kind => MarkupKind.Cross;
        public Position Position { get; }

        public Cross(Position position)
        {
            Position = position;
        }
    }
}
