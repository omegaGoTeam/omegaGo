using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Common;

namespace OmegaGo.Core.Agents
{
    /// <summary>
    /// Represents the agent that makes move for AI programs.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Agents.AgentBase" />
    public class AIAgent : AgentBase
    {
        /// <summary>
        /// The AI program that feeds moves to this agent.
        /// </summary>
        IAIProgram _aiProgram;
        /// <summary>
        /// The strength (1-10) that this AI program should play at.
        /// </summary>
        public int Strength = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="AIAgent"/> class, with the specified AI program making the decisions.
        /// </summary>
        /// <param name="aiProgram">The AI program that makes decisions.</param>
        public AIAgent(IAIProgram aiProgram)
        {
            this._aiProgram = aiProgram;
        }

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.MakeRandomMove;
        public override async void PleaseMakeAMove()
        { 
            GameBoard createdBoard = FastBoard.CreateBoardFromGame(Game);
            var aiTask = Task.Run(() => this._aiProgram.RequestMove(new AIPreMoveInformation(
              Player.Color,
              createdBoard,
              Game.BoardSize,
              new TimeSpan(0, 0, 2),
              Strength,
              Game.PrimaryTimeline.ToList()
              )));
            AiDecision decision = await aiTask;
            switch (decision.Kind)
            {
                case AgentDecisionKind.Move:
                    Game.GameController.MakeMove(Player, decision.Move);
                    break;
                case AgentDecisionKind.Resign:
                    Game.GameController.Resign(Player);
                    break;
                default:
                    throw new Exception("This decision kind does not exist.");
            }
        }
    }
}
