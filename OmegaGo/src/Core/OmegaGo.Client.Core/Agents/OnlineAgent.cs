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
        public void ForceHistoricMove(int moveIndex, Move move)
        {
            // Ok.
            if (this._storedMoves.ContainsKey(moveIndex))
                this._storedMoves[moveIndex] = move;
            else
                this._storedMoves.Add(moveIndex, move);
        }

        public void Click(StoneColor color, Position selectedPosition)
        {
            throw new InvalidOperationException("An online agent cannot click.");
        }

        public void ForcePass(StoneColor color)
        {
            throw new InvalidOperationException("An online agent is not a GUI agent.");
        }

        public void GameBegins(Player player, Game game)
        {
            throw new NotImplementedException();
        }

        public void PleaseMakeAMove()
        {
            throw new NotImplementedException();
        }

        public IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.PermitItAnyway;

    

        public async Task<AgentDecision> RequestMoveAsync(Game game)
        {
            // Take from history.
            while (true)
            {
                if (_storedMoves.ContainsKey(game.NumberOfMovesPlayed + 1))
                {
                    return AgentDecision.MakeMove(this._storedMoves[game.NumberOfMovesPlayed + 1], "The server sent this information.");
                }
                await Task.Delay(1000); // TODO refactor
                // Refactoring will require a task-aware dictionary, perhaps a condition variable?
            }
        }
    }
}
