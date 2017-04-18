﻿using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class SimpleMarkupTool : IMarkupTool
    {
        public MarkupKind Markup { get; }

        public SimpleMarkupTool(MarkupKind markupKind)
        {
            Markup = markupKind;
        }

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

        public IMarkup GetShadowItem(IToolServices toolServices)
        {
            if (Markup == MarkupKind.Circle)
                return new Circle(toolServices.PointerOverPosition);
            if (Markup == MarkupKind.Cross)
                return new Cross(toolServices.PointerOverPosition);
            if (Markup == MarkupKind.Square)
                return new Square(toolServices.PointerOverPosition);
            if (Markup == MarkupKind.Triangle)
                return new Triangle(toolServices.PointerOverPosition);

            return null;
        }

    }
}
