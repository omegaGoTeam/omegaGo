using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs
{
    /// <summary>
    /// IGS agent
    /// </summary>
    public class IgsAgent : AgentBase
    {
        /// <summary>
        /// Contains the moves already received from the server
        /// </summary>
        private readonly Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();

        /// <summary>
        /// Creates the IGS Agent
        /// </summary>
        /// <param name="color">Color the agent plays</param>
        public IgsAgent(StoneColor color) : base(color)
        {
        }

        /// <summary>
        /// Type of the agent
        /// </summary>
        public override AgentType Type => AgentType.Remote;

        /// <summary>
        /// Handling of illegal moves
        /// </summary>
        public override IllegalMoveHandling IllegalMoveHandling => IllegalMoveHandling.PermitItAnyway;

        /// <summary>
        /// Handles incoming move from server
        /// </summary>
        /// <param name="moveIndex">Index of the move</param>
        /// <param name="move">Move</param>
        public void MoveFromServer(int moveIndex, Move move)
        {
            if (move.WhoMoves != Color) throw new InvalidOperationException("Agent received a move that is not his.");
            _storedMoves[moveIndex] = move;
            MakeMoveIfOnTurn();
        }

        /// <summary>
        /// Resigns the game
        /// </summary>
        public void ResignationFromServer()
        {
            OnResign();
        }

        public override void MoveUndone()
        {
            var timeline = (GameState.GameTree.PrimaryTimeline.ToList());
            int latestStillExistingMove = timeline.Count - 1;
            foreach(var storedTuple in _storedMoves.ToList())
            {
                if (storedTuple.Key > latestStillExistingMove)
                {
                    _storedMoves.Remove(storedTuple.Key);
                }
            }
        }

        /// <summary>
        /// May perform a move if on turn
        /// </summary>
        private void MakeMoveIfOnTurn()
        {
            int moveToMake = GameState.NumberOfMoves;
            if (_storedMoves.ContainsKey(moveToMake))
            {
                Move move = _storedMoves[moveToMake];
                if (move.Kind == MoveKind.PlaceStone)
                {
                    OnPlaceStone(move.Coordinates);
                }
                else if (move.Kind == MoveKind.Pass)
                {
                    OnPass();
                }
            }
        }

        /// <summary>
        /// Asks the player to make a move.
        /// If a the move was already received from server, it will be performed immediately.
        /// </summary>
        public override void PleaseMakeAMove()
        {
            MakeMoveIfOnTurn();
        }

        /// <summary>
        /// In the case of IGS, the move should be made even if evaluated as illegal
        /// </summary>
        /// <param name="moveResult">Move result</param>
        public override void MoveIllegal(MoveResult moveResult)
        {
            throw new InvalidOperationException(
                "We represent the server and our moves are inherently superior. What does the caller think he is, calling our moves 'illegal'. Pche.");
        }
    }
}