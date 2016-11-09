﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Represents a decision made by an agent (such as the player, the online opponent or an AI program) in response to a move request.
    /// </summary>
    public class AgentDecision
    {
        public AgentDecisionKind Kind { get; private set; }
        public Move Move { get; private set; }
        public string Explanation { get; private set; }

        public static AgentDecision MakeMove(Move move, string why)
        {
            return new AgentDecision()
            {
                Kind = AgentDecisionKind.Move,
                Move = move,
                Explanation = why
            };
        }
        public static AgentDecision Resign(string why)
        {
            return new AgentDecision()
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