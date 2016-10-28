using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Agents
{
    public class AIAgent : IAgent
    {
        IAIProgram aiProgram;

        public AIAgent(IAIProgram aiProgram)
        {
            this.aiProgram = aiProgram;
        }

        public Task<AIDecision> RequestMove(Game game)
        {
            Color[,] createdBoard = new Color[game.BoardSize, game.BoardSize];
            foreach (Move move in game.PrimaryTimeline)
            {
                if (move.Kind == MoveKind.PlaceStone)
                {
                    createdBoard[move.Coordinates.X, move.Coordinates.Y] = move.WhoMoves;
                }
            }

            return aiProgram.RequestMove(new AIPreMoveInformation(
                game.Players[0].Agent == this ? Color.Black : Color.White,
                createdBoard,
                game.BoardSize,
                new TimeSpan(0,0,2),
                10
                ));
        }
    }
}
