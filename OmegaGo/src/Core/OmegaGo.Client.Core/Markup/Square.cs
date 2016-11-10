using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Markup
{
    public sealed class Square : IMarkup
    {
        private Position _position;

        public MarkupKind Kind => MarkupKind.Square;
        public Position Position => _position;

        public Square(Position position)
        {
            _position = position;
        }
    }
}
