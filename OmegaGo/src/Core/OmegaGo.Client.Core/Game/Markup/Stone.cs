namespace OmegaGo.Core.Game.Markup
{
    /// <summary>
    /// Represents a stone, that will be placed on the board or shown as a shadow item.
    /// </summary>
    public sealed class Stone : IShadowItem
    {
        public ShadowItemKind ShadowItemKind => ShadowItemKind.Stone;

        /// <summary>
        /// Color of stone.
        /// </summary>
        public StoneColor Color { get; }

        /// <summary>
        /// Position of stone on the board.
        /// </summary>
        public Position Position { get; }

        public Stone(StoneColor color, Position position)
        {
            Position = position;
            Color = color;
        }
    }
}
