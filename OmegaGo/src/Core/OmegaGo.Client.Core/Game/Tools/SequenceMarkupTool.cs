using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    /// <summary>
    /// Sequence markup tools (Letter, Number) place a label(kind of markup) on the board. 
    /// If the intersection contains a label, the tool removes it.    
    /// </summary>
    public sealed class SequenceMarkupTool : IPlacementTool
    {
        /// <summary>
        /// Map of shadow items.
        /// </summary>
        private string[,] _shadows;
        private GameTreeNode _currentNode;

        public SequenceMarkupTool(SequenceMarkupKind kind)
        {
            SequenceMarkup = kind;
        }

        /// <summary>
        /// Type of label (Letter, Number)
        /// </summary>
        public SequenceMarkupKind SequenceMarkup { get; }
        
        public void Execute(IToolServices toolService)
        {
            Position position = toolService.PointerOverPosition;
            MarkupInfo markups = toolService.Node.Markups;

            MarkupKind markupKindOnPosition = markups.RemoveMarkupOnPosition(position);

            if (SequenceMarkup == SequenceMarkupKind.Letter)
            {
                char letter = markups.GetSmallestUnusedLetter();
                if (letter != '0' && markupKindOnPosition!= MarkupKind.Label)
                    markups.AddMarkup<Label>(new Label(position, letter.ToString()));
            }
            else
            {
                int number = markups.GetSmallestUnusedNumber();
                if (markupKindOnPosition != MarkupKind.Label)
                    markups.AddMarkup<Label>(new Label(position, number.ToString()));
            }

            _shadows = toolService.Node.Markups.FillSequenceShadowMap(toolService.GameTree.BoardSize, SequenceMarkup);
        }

        public IShadowItem GetShadowItem(IToolServices toolService)
        {
            if (_shadows == null || !toolService.Node.Equals(_currentNode))
            {
                _shadows = toolService.Node.Markups.FillSequenceShadowMap(toolService.GameTree.BoardSize, SequenceMarkup);
                _currentNode = toolService.Node;
            }
            
            string labelText=_shadows[toolService.PointerOverPosition.X, toolService.PointerOverPosition.Y];
            if (labelText.Equals("r") || labelText.Equals("0"))
                return new None();
            else
                return new Label(toolService.PointerOverPosition, labelText);            
        }

        public void Set(IToolServices toolServices)
        {
            _shadows = toolServices.Node.Markups.FillSequenceShadowMap(toolServices.GameTree.BoardSize, SequenceMarkup);
            _currentNode = toolServices.Node;
        }
    }
}
