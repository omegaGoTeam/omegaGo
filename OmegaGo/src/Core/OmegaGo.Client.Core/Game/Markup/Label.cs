namespace OmegaGo.Core.Game.Markup
{
    public sealed class Label : IMarkup,IShadowItem
    {
        public MarkupKind MarkupKind => MarkupKind.Label;

        public ShadowItemKind ShadowItemKind => ShadowItemKind.Label;

        public Position Position { get; }

        public string Text { get; }

        public Label(Position position, string text)
        {
            Position = position;
            Text = text;
        }
    }
}
