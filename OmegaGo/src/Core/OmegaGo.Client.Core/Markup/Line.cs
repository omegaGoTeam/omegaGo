using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Markup
{
    public sealed class Line : IMarkup
    {
        private Position _from;
        private Position _to;

        public MarkupKind Kind => MarkupKind.Line;
        public Position From => _from;
        public Position To => _to;

        public Line(Position from, Position to)
        {
            _from = from;
            _to = to;
        }
    }
}
