using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    class AIDecision
    {
        public AIDecisionKind Kind;
        public Move Move;
        public string Explanation;
    }
    enum AIDecisionKind
    {
        SimpleMove,
        Pass,
        Resign
    }
}
