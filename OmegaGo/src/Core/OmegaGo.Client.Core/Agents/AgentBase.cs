using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    /// <summary>
    /// This base class contains code that allows an agent to make moves based on a historical record. This is used most often when resuming
    /// a paused game or when entering a game that's already in progress on a server.
    /// </summary>
    public abstract class AgentBase : IAgent
    {
        private Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();


        public abstract Task<AgentDecision> RequestMoveAsync(Game game);

        public abstract IllegalMoveHandling HowToHandleIllegalMove { get; }

        public void ForceHistoricMove(int moveIndex, Move move)
        {
            // Ok.
            if (this._storedMoves.ContainsKey(moveIndex))
                this._storedMoves[moveIndex] = move;
            else
                this._storedMoves.Add(moveIndex, move);
        }

        public virtual void Click(StoneColor color, Position selectedPosition)
        {
            throw new InvalidOperationException("This agent is not a GUI agent.");
        }

        /// <summary>
        /// If this agent has a historical move stored for the current turn number, then this method will return that move; 
        /// if not, then it will return null.
        /// </summary>
        /// <param name="game">The game that this agent is playing.</param>
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
