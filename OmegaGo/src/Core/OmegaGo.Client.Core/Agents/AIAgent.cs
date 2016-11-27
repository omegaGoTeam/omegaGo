using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Common;

namespace OmegaGo.Core.Agents
{
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

        public AIAgent(IAIProgram aiProgram)
        {
            this._aiProgram = aiProgram;
        }

        public override async  Task<AgentDecision> RequestMoveAsync(Game game)
        {
            AgentDecision storedDecision = GetStoredDecision(game);
            if (storedDecision != null) return storedDecision;

            StoneColor[,] createdBoard = FastBoard.CreateBoardFromGame(game);

            var aiTask = Task.Run(() => this._aiProgram.RequestMove(new AIPreMoveInformation(
                game.Players[0].Agent == this ? StoneColor.Black : StoneColor.White,
                createdBoard,
                game.BoardSize,
                new TimeSpan(0, 0, 2),
                Strength,
                game.PrimaryTimeline.ToList()
                )));
            return await aiTask;
        }

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.MakeRandomMove;

        public static explicit operator AIAgent(Player v)
        {
            throw new NotImplementedException();
        }
    }
}
