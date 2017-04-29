using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class SimpleMarkupTool : IPlacementTool
    {
        private char[,] _shadows;
        public SimpleMarkupKind SimpleMarkup { get; }

        public SimpleMarkupTool(SimpleMarkupKind markupKind)
        {
            SimpleMarkup = markupKind;
        }

        public void Execute(IToolServices toolService)
        {
            Position position = toolService.PointerOverPosition;
            MarkupInfo markups = toolService.Node.Markups;

            MarkupKind markupKindOnPosition = markups.RemoveMarkupOnPosition(position);
            
            // If the removed markup is the same as the new one than do not add anything.
            if (IsMarkupEqual(SimpleMarkup, markupKindOnPosition))
                return;

            if (SimpleMarkup == SimpleMarkupKind.Circle)
                markups.AddMarkup<Circle>(new Circle(position));
            if (SimpleMarkup == SimpleMarkupKind.Cross)
                markups.AddMarkup<Cross>(new Cross(position));
            if (SimpleMarkup == SimpleMarkupKind.Square)
                markups.AddMarkup<Square>(new Square(position));
            if (SimpleMarkup == SimpleMarkupKind.Triangle)
                markups.AddMarkup<Triangle>(new Triangle(position));
        }

        public IShadowItem GetShadowItem(IToolServices toolServices)
        {
            if (_shadows == null)
                _shadows = toolServices.Node.Markups.FillSimpleShadowMap(toolServices.GameTree.BoardSize, SimpleMarkup);

            char shadow = _shadows[toolServices.PointerOverPosition.X, toolServices.PointerOverPosition.Y];
            if (shadow=='r')
                return new None();
            else
                switch (SimpleMarkup) {
                    case SimpleMarkupKind.Circle:
                        return new Circle(toolServices.PointerOverPosition);
                    case SimpleMarkupKind.Cross:
                        return new Cross(toolServices.PointerOverPosition);
                    case SimpleMarkupKind.Square:
                        return new Square(toolServices.PointerOverPosition);
                    case SimpleMarkupKind.Triangle:
                        return new Triangle(toolServices.PointerOverPosition);
                }
            
            return new None();
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
