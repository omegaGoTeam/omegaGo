using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    public class AICapabilities
    {
        public int MinimumBoardSize { get; } = 2;
        public int MaximumBoardSize { get; } = 25;
        public bool HandlesNonSquareBoards { get; } = false;

        public AICapabilities(bool handlesNonSquareBoards)
        {
            HandlesNonSquareBoards = handlesNonSquareBoards;
        }
    }
}
