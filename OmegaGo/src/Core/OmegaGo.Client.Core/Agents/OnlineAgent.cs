using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    public class OnlineAgent : IAgent
    {
        private Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();

        public IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.PermitItAnyway;

        public void ForceHistoricMove(int moveIndex, Move move)
        {
            // Ok.
            if (this._storedMoves.ContainsKey(moveIndex))
                this._storedMoves[moveIndex] = move;
            else
                this._storedMoves.Add(moveIndex, move);
        }

        public async Task<AgentDecision> RequestMove(Game game)
        {
            // Take from history.
            while (true)
            {
                if (_storedMoves.ContainsKey(game.NumberOfMovesPlayed + 1))
                {
                    return AgentDecision.MakeMove(this._storedMoves[game.NumberOfMovesPlayed + 1], "The server sent this information.");
                }
                await Task.Delay(1000); // TODO refactor
            }
        }
    }
}
