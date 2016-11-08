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

        public async Task<AgentDecision> RequestMove(Game game)
        {
            Color[,] createdBoard = new Color[game.SquareBoardSize, game.SquareBoardSize];
            foreach (Move move in game.PrimaryTimeline)
            {
                if (move.Kind == MoveKind.PlaceStone)
                {
                    createdBoard[move.Coordinates.X, move.Coordinates.Y] = move.WhoMoves;
                }
            }

            var aiTask = Task.Run(() => this.aiProgram.RequestMove(new AIPreMoveInformation(
                game.Players[0].Agent == this ? Color.Black : Color.White,
                createdBoard,
                game.BoardSize,
                new TimeSpan(0, 0, 2),
                10,
                game.PrimaryTimeline.ToList()
                )));
            return await aiTask;
        }

        public IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.MakeRandomMove;
        public void ForceHistoricMove(int moveIndex, Move move)
        {
            throw new NotImplementedException();
        }
    }
}
