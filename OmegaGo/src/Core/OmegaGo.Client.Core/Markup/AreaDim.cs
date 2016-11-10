using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Markup
{
    public sealed class AreaDim : IMarkup
    {
        private Position _from;
        private Position _to;

        public MarkupKind Kind => MarkupKind.AreaDim;

        public Position From => _from;
        public Position To => _to;

        public AreaDim(Position from, Position to)
        {
            _from = from;
            _to = to;
        }
    }
}
