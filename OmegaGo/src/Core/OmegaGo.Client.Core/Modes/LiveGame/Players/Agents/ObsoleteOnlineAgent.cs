using System.Collections.Generic;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// Represents the agent of a player whose moves are sent to this device via an internet server, either in observation or play mode.
    /// </summary>
    /// <seealso cref="ObsoleteAgentBase" />
    public class ObsoleteOnlineAgent : ObsoleteAgentBase, IObsoleteOnlineAgent
    {
        public override void PleaseMakeAMove()
        {
            // The turn number that we're supposed to make:
            AwaitingTurnNumber = Game.NumberOfMovesPlayed;
            PossiblyAnswerAwaitingTurn();
        }
        

        private readonly Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();
        /// <summary>
        /// The online agent will set this to determine which move is about to be made. This base class will read this to actually make the move when 
        /// we have it.
        /// </summary>
        private int AwaitingTurnNumber { get; set; } = -1;
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
        private void PossiblyAnswerAwaitingTurn()
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

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.PermitItAnyway;

        public void Undo()
        {
            this._storedMoves.Remove(Game.NumberOfMovesPlayed - 1);
        }
    }
}
