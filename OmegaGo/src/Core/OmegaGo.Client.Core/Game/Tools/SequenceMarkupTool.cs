using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    class SequenceMarkupTool : IMarkupTool
    {
        public ToolKind Tool { get; }
        public MarkupKind Markup { get; }

        public void Execute(IToolServices toolService)
        {
            Position position = toolService.PointerOverPosition;
            MarkupInfo markups = toolService.Node.Markups;

            if (Tool == ToolKind.Letter)
            {
                char letter = markups.GetSmallestUnusedLetter();
                if (letter != '0')
                {
                    markups.RemoveMarkupOnPosition(position);
                    markups.AddMarkup<Label>(new Label(position, letter.ToString()));
                }
                else
                {
                    //TODO Aniko: Inform UI - no more unused capital letter
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
            if (Tool == ToolKind.Letter)
            {
                char letter = toolService.Node.Markups.GetSmallestUnusedLetter();
                if (letter != '0')
                    return new Label(toolService.PointerOverPosition, letter.ToString());
                //TODO Aniko:  else Notify UI - no more unused capital letter
            }
            else
            {
                int number = toolService.Node.Markups.GetSmallestUnusedNumber();
                return new Label(toolService.PointerOverPosition, number.ToString());
            }

            return null;
        }
    }
}
