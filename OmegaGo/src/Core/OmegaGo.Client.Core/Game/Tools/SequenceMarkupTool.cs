using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class SequenceMarkupTool : IPlacementTool
    {
        private string[,] _shadows;

        public SequenceMarkupKind SequenceMarkup { get; }
        
        public SequenceMarkupTool(SequenceMarkupKind kind)
        {
            SequenceMarkup = kind;
        }

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
            if (_shadows == null)
                _shadows = toolService.Node.Markups.FillSequenceShadowMap(toolService.GameTree.BoardSize, SequenceMarkup);
            
            string labelText=_shadows[toolService.PointerOverPosition.X, toolService.PointerOverPosition.Y];
            if (labelText.Equals("r") || labelText.Equals("0"))
                return new None();
            else
                return new Label(toolService.PointerOverPosition, labelText);            
        }
    }
}
