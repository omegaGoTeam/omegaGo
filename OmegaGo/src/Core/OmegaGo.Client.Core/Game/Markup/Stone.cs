using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Markup
{
    public sealed class Stone : IShadowItem
    {
        public ShadowItemKind ShadowItemKind => ShadowItemKind.Stone;

        public StoneColor Color { get; }

        public Position Position { get; }

        public Stone(StoneColor color, Position position)
        {
            Position = position;
            Color = color;
        }
    }
}
