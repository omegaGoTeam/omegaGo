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
        /// <summary>
        /// Gets the game that this agent's player is playing in.
        /// </summary>
        protected GameInfo Game { get; private set; }
        /// <summary>
        /// Gets the player that this agent makes moves for.
        /// </summary>
        protected Player Player { get; private set; }
        protected Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();
        /// <summary>
        /// The online agent will set this to determine which move is about to be made. This base class will read this to actually make the move when 
        /// we have it.
        /// </summary>
        protected int AwaitingTurnNumber { private get; set; } = -1;
        
        public abstract IllegalMoveHandling HowToHandleIllegalMove { get; }

        public void ForceHistoricMove(int moveIndex, Move move)
        {
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

        /// <summary>
        /// If the game controller is waiting for this agent to make a move, and this move is already available in the list of stored moves (received
        /// from the server), then the move will be made.
        /// </summary>
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

        public void GameBegins(Player player, GameInfo game)
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
