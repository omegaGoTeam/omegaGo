using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Represents a decision made by an AI program in response to a move request.
    /// </summary>
    class AIDecision
    {
        public AIDecisionKind Kind { get; private set; }
        public Move Move { get; private set; }
        public string Explanation { get; private set; }

        public static AIDecision MakeMove(Move move, string why)
        {
            return new AIDecision()
            {
                Kind = AIDecisionKind.Move,
                Move = move,
                Explanation = why
            };
        }
        public static AIDecision Resign(string why)
        {
            return new AIDecision()
            {
                Kind = AIDecisionKind.Resign,
                Explanation = why
            };
        } 
    }
    enum AIDecisionKind
    {
        Move,
        Resign
    }
}
