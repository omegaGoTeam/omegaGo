using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Markup
{
    public sealed class Cross : IMarkup
    {
        private Position _position;

        public MarkupKind Kind => MarkupKind.Cross;
        public Position Position => _position;

        public Cross(Position position)
        {
            _position = position;
        }
    }
}
