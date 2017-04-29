using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class SequenceMarkupTool : IPlacementTool
    {
        private GameTreeNode _currentNode;
        private IMarkup _currentMarkup;

        public SequenceMarkupKind SequenceMarkup { get; }
        public bool AreMarksAvailable { get; private set; }
        

        public SequenceMarkupTool(SequenceMarkupKind kind)
        {
            SequenceMarkup = kind;
            AreMarksAvailable = false;
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
        }

        public IShadowItem GetShadowItem(IToolServices toolService)
        {
            if (SequenceMarkup == SequenceMarkupKind.Letter)
            {
                char letter = toolService.Node.Markups.GetSmallestUnusedLetter();
                if (letter != '0')
                {
                    AreMarksAvailable = true;
                    return new Label(toolService.PointerOverPosition, letter.ToString());
                }
                else
                {
                    AreMarksAvailable = false;
                }
            }
            else
            {
                int number = toolService.Node.Markups.GetSmallestUnusedNumber();
                AreMarksAvailable = true;
                return new Label(toolService.PointerOverPosition, number.ToString());
            }

            return null;
        }
    }
}
