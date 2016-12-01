using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Represents a decision made by an AI agent in response to a move request.
    /// </summary>
    public class AiDecision
    {
        /// <summary>
        /// Gets the form of decision that the agent took. The most common decisions are making a move or resigning.
        /// </summary>
        public AgentDecisionKind Kind { get; private set; }
        /// <summary>
        /// If the <see cref="Kind"/> is <see cref="AgentDecisionKind.Move"/>, then this gets the <see cref="Move"/> that the agent wants to make.  
        /// </summary>
        public Move Move { get; private set; }
        /// <summary>
        /// Gets the agent's explanation for why it made this decision.
        /// </summary>
        public string Explanation { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="AiDecision"/> class from being created. Use <see cref="MakeMove(Core.Move,string)"/>
        /// or <see cref="Resign(string)"/> instead.  
        /// </summary>
        private AiDecision()
        {
           
        }
        public static AiDecision MakeMove(Move move, string why)
        {
            return new AiDecision()
            {
                Kind = AgentDecisionKind.Move,
                Move = move,
                Explanation = why
            };
        }
        public static AiDecision Resign(string why)
        {
            return new AiDecision()
            {
                Kind = AgentDecisionKind.Resign,
                Explanation = why
            };
        }

        public override string ToString()
        {
            switch (Kind)
            {
                case AgentDecisionKind.Move:
                    return "MOVE: " + Move;
                case AgentDecisionKind.Resign:
                    return "RESIGN";
            }
            throw new Exception("This AgentDecisionKind does not exist.");
        }
    }


    /// <summary>
    /// Represents the kind of the decision: whether it's making a move or resigning.
    /// </summary>
    public enum AgentDecisionKind
    {
        /// <summary>
        /// The agent wishes to make a move - either to place a stone or to pass.
        /// </summary>
        Move,
        /// <summary>
        /// Only AI programs will use the "resign" option. Human players will use a different channel to resign.
        /// </summary>
        Resign
    }
}
