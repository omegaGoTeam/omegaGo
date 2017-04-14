using OmegaGo.Core.Game.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    //TODO Aniko: Notify UI - no more unused capital letter
                }
            }
            else
            {
                int number = markups.GetSmallestUnusedNumber();
                markups.RemoveMarkupOnPosition(position);
                markups.AddMarkup<Label>(new Label(position, number.ToString()));
            }
                

        }
    }
}
