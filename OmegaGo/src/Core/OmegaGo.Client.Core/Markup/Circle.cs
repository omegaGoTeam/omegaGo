using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Markup
{
    public sealed class Circle : IMarkup
    {
        private Position _position;

        public MarkupKind Kind => MarkupKind.Circle;
        public Position Position => _position;

        public Circle(Position position)
        {
            _position = position;
        }
    }
}
