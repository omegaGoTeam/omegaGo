using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    /// <summary>
    /// Simple markup tools (Circle, Cross, Square, Triangle) place a shape(markup) on the board. 
    /// If the intersection contains a markup, the tool removes or changes it (depends on the type of selected tool and markup on the position).    
    /// </summary>
    public sealed class SimpleMarkupTool : IPlacementTool
    {
        /// <summary>
        /// Map of shadow items.
        /// </summary>
        private char[,] _shadows;
        private GameTreeNode _currentNode;
        
        public SimpleMarkupTool(SimpleMarkupKind markupKind)
        {
            SimpleMarkup = markupKind;
        }

        /// <summary>
        /// Type of markup (Circle, Cross, Square, Triangle)
        /// </summary>
        public SimpleMarkupKind SimpleMarkup { get; }

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
            if (_shadows == null || !toolServices.Node.Equals(_currentNode))
            {
                _shadows = toolServices.Node.Markups.FillSimpleShadowMap(toolServices.GameTree.BoardSize, SimpleMarkup);
                _currentNode = toolServices.Node;
            }

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

        public void Set(IToolServices toolServices)
        {
            _shadows = toolServices.Node.Markups.FillSimpleShadowMap(toolServices.GameTree.BoardSize, SimpleMarkup);
            _currentNode = toolServices.Node;
        }

        /// <summary>
        /// Indicates whether the given markups have the same type.
        /// </summary>
        /// <param name="simpleMarkupKind">Markup kind.</param>
        /// <param name="markupKind">Other markup kind.</param>
        /// <returns>True, if the type of given markups equals.</returns>
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
