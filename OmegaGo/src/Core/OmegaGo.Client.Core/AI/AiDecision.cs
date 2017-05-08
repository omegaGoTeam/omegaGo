using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Represents a decision made by an AI agent in response to a move request.
    /// </summary>
    public class AIDecision
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="AIDecision"/> class from being created. Use <see cref="MakeMove(Game.Move,string)"/>
        /// or <see cref="Resign(string)"/> instead.  
        /// </summary>
        private AIDecision()
        {
        }

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
        /// Log of AI's decisions
        /// </summary>
        public List<string> AiNotes { get; set; } = new List<string>();

        /// <summary>
        /// Creates a move decision
        /// </summary>
        /// <param name="move">Move</param>
        /// <param name="why">Reason for the move</param>
        /// <returns>AI decision</returns>
        public static AIDecision MakeMove(Move move, string why)
        {
            return new AIDecision()
            {
                Kind = AgentDecisionKind.Move,
                Move = move,
                Explanation = why
            };
        }
        
        /// <summary>
        /// Resigns
        /// </summary>
        /// <param name="why">Reason for resignation</param>
        /// <returns>AI decision</returns>
        public static AIDecision Resign(string why)
        {
            return new AIDecision()
            {
                Kind = AgentDecisionKind.Resign,
                Explanation = why
            };
        }

        /// <summary>
        /// Serializes decision into debug string
        /// </summary>
        /// <returns>Debug string representation of the AI Decision</returns>
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
}
