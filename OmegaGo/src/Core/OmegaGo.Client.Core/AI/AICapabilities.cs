using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    public class AICapabilities
    {
        private bool HandlesNonSquareBoards { get; } = false;

        public AICapabilities(bool handlesNonSquareBoards)
        {
            HandlesNonSquareBoards = handlesNonSquareBoards;
        }
    }
}
