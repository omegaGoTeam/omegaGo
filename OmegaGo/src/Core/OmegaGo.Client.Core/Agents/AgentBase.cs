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
        protected Game Game { get; private set; }
        protected Player Player { get; private set; }
        private Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();
        protected int AwaitingTurnNumber = -1;
        

        public abstract IllegalMoveHandling HowToHandleIllegalMove { get; }

        public void ForceHistoricMove(int moveIndex, Move move)
        {
            // Ok.
            if (this._storedMoves.ContainsKey(moveIndex))
            {
                this._storedMoves[moveIndex] = move;
            }
            else
            {
                this._storedMoves.Add(moveIndex, move);
            }
            PossiblyAnswerAwaitingTurn();
        }

        protected void PossiblyAnswerAwaitingTurn()
        {
            if (AwaitingTurnNumber != -1)
            {
                if (_storedMoves.ContainsKey(AwaitingTurnNumber))
                {
                    Move storedMove = _storedMoves[AwaitingTurnNumber];
                    AwaitingTurnNumber = -1;
                    Game.GameController.MakeMove(Player, storedMove);
                }
            }

        }

        public virtual void Click(StoneColor color, Position selectedPosition)
        {
            throw new InvalidOperationException("This agent is not a GUI agent.");
        }

        public virtual void ForcePass(StoneColor color)
        {
            throw new InvalidOperationException("This agent is not a GUI agent.");
        }

        public void GameBegins(Player player, Game game)
        {
            this.Player = player;
            this.Game = game;
        }

        public abstract void PleaseMakeAMove();

        /// <summary>
        /// If this agent has a historical move stored for the current turn number, 
        /// then this method will return that move; 
        /// if not, then it will return null.
        /// </summary>
        /// <param name="zeroBasedTurnNumber">The zero-indexed turn number.</param>
        protected Move GetStoredDecision(int zeroBasedTurnNumber)
        {
            if (zeroBasedTurnNumber == -1) return null;
            if (_storedMoves.ContainsKey(zeroBasedTurnNumber + 1))
            {
                return _storedMoves[zeroBasedTurnNumber + 1];
            }
            return null;
        }
    }
}
