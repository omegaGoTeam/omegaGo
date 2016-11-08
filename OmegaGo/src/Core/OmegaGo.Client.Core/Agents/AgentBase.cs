using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    public abstract class AgentBase
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
        protected AgentDecision GetStoredDecision(Game game)
        {
            if (_storedMoves.ContainsKey(game.NumberOfMovesPlayed + 1))
            {
                return AgentDecision.MakeMove(this._storedMoves[game.NumberOfMovesPlayed + 1], "This information was stored.");
            }
            return null;
        }
    }
}
