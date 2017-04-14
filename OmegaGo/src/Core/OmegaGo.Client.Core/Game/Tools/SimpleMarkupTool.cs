using OmegaGo.Core.Game.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    class SimpleMarkupTool : IMarkupTool
    {
        public ToolKind Tool { get; }
        public MarkupKind Markup { get; }

        public void Execute(IToolServices toolService)
        {
            Position position = toolService.PointerOverPosition;
            MarkupInfo markups = toolService.Node.Markups;

            markups.RemoveMarkupOnPosition(position);

            if (Markup == MarkupKind.Circle)
                markups.AddMarkup<Circle>(new Circle(position));
            if (Markup == MarkupKind.Cross)
                markups.AddMarkup<Cross>(new Cross(position));
            if (Markup == MarkupKind.Square)
                markups.AddMarkup<Square>(new Square(position));
            if (Markup == MarkupKind.Triangle)
                markups.AddMarkup<Triangle>(new Triangle(position));
        }

    }
}
