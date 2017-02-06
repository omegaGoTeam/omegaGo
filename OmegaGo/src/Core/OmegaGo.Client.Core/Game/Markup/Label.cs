namespace OmegaGo.Core.Game.Markup
{
    public sealed class Label : IMarkup
    {
        public MarkupKind Kind => MarkupKind.Label;

        public Position Position { get; }

        public string Text { get; }

        public Label(Position position, string text)
        {
            Position = position;
            Text = text;
        }
    }
}
