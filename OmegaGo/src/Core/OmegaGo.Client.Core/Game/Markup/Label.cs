namespace OmegaGo.Core.Game.Markup
{
    public sealed class Label : IMarkup
    {
        private Position _position;
        private string _text;

        public MarkupKind Kind => MarkupKind.Label;
        public Position Position => _position;
        public string Text => _text;

        public Label(Position position, string text)
        {
            _position = position;
            _text = text;
        }
    }
}
