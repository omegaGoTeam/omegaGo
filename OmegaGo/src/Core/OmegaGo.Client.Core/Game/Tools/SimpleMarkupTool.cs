using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class SimpleMarkupTool : IPlacementTool
    {
        public SimpleMarkupKind Markup { get; }

        public SimpleMarkupTool(SimpleMarkupKind markupKind)
        {
            Markup = markupKind;
        }

        public void Execute(IToolServices toolService)
        {
            Position position = toolService.PointerOverPosition;
            MarkupInfo markups = toolService.Node.Markups;

            MarkupKind markupKindOnPosition = markups.RemoveMarkupOnPosition(position);
            
            // If the removed markup is the same as the new one than do not add anything.
            if (IsMarkupEqual(Markup, markupKindOnPosition))
                return;

            if (Markup == SimpleMarkupKind.Circle)
                markups.AddMarkup<Circle>(new Circle(position));
            if (Markup == SimpleMarkupKind.Cross)
                markups.AddMarkup<Cross>(new Cross(position));
            if (Markup == SimpleMarkupKind.Square)
                markups.AddMarkup<Square>(new Square(position));
            if (Markup == SimpleMarkupKind.Triangle)
                markups.AddMarkup<Triangle>(new Triangle(position));
        }

        public IShadowItem GetShadowItem(IToolServices toolServices)
        {
            if (Markup == SimpleMarkupKind.Circle)
                return new Circle(toolServices.PointerOverPosition);
            if (Markup == SimpleMarkupKind.Cross)
                return new Cross(toolServices.PointerOverPosition);
            if (Markup == SimpleMarkupKind.Square)
                return new Square(toolServices.PointerOverPosition);
            if (Markup == SimpleMarkupKind.Triangle)
                return new Triangle(toolServices.PointerOverPosition);

            return null;
        }

        private bool IsMarkupEqual(SimpleMarkupKind simpleMarkupKind, MarkupKind markupKind)
        {
            switch(simpleMarkupKind)
            {
                case SimpleMarkupKind.Circle:
                    return markupKind == MarkupKind.Circle;
                case SimpleMarkupKind.Cross:
                    return markupKind == MarkupKind.Cross;
                case SimpleMarkupKind.Square:
                    return markupKind == MarkupKind.Square;
                case SimpleMarkupKind.Triangle:
                    return markupKind == MarkupKind.Triangle;
            }

            return false;
        }
    }
}
