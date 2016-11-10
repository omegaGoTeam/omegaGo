using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Markup
{
    public sealed class Triangle : IMarkup
    {
        private Position _position;

        public MarkupKind Kind => MarkupKind.Triangle;
        public Position Position => _position;

        public Triangle(Position position)
        {
            _position = position;
        }
    }
}
