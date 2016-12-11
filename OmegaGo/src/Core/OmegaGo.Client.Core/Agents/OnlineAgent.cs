using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    /// <summary>
    /// Represents the agent of a player whose moves are sent to this device via an internet server, either in observation or play mode.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Agents.AgentBase" />
    public class OnlineAgent : AgentBase, IOnlineAgent
    {
        public override void PleaseMakeAMove()
        {
            // The turn number that we're supposed to make:
            AwaitingTurnNumber = Game.NumberOfMovesPlayed;
            PossiblyAnswerAwaitingTurn();
        }

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.PermitItAnyway;

        private readonly Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();
        /// <summary>
        /// The online agent will set this to determine which move is about to be made. This base class will read this to actually make the move when 
        /// we have it.
        /// </summary>
        protected int AwaitingTurnNumber { private get; set; } = -1;
        public void ForceHistoricMove(int moveIndex, Move move)
        {
            if (_storedMoves.ContainsKey(moveIndex))
            {
                _storedMoves[moveIndex] = move;
            }
            else
            {
                _storedMoves.Add(moveIndex, move);
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

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.PermitItAnyway;

        public void Undo()
        {
            this._storedMoves.Remove(Game.NumberOfMovesPlayed - 1);
        }
        }
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
