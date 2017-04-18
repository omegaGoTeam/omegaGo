using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class SequenceMarkupTool : IMarkupTool
    {
        public SequenceMarkupKind SequenceMarkup { get; }
        public bool AreMarksAvailable { get; private set; }

        SequenceMarkupTool(SequenceMarkupKind kind)
        {
            SequenceMarkup = kind;
            AreMarksAvailable = false;
        }

        public void Execute(IToolServices toolService)
        {
            Position position = toolService.PointerOverPosition;
            MarkupInfo markups = toolService.Node.Markups;

            if (SequenceMarkup == SequenceMarkupKind.Letter)
            {
                char letter = markups.GetSmallestUnusedLetter();
                if (letter != '0')
                {
                    markups.RemoveMarkupOnPosition(position);
                    markups.AddMarkup<Label>(new Label(position, letter.ToString()));
                }
            }
            else
            {
                int number = markups.GetSmallestUnusedNumber();
                markups.RemoveMarkupOnPosition(position);
                markups.AddMarkup<Label>(new Label(position, number.ToString()));
            }
        }

        public IMarkup GetShadowItem(IToolServices toolService)
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
