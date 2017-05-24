using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs
{
    public class KgsAgent : AgentBase
    {
        private readonly KgsConnection _connection;
        private Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();

        public KgsAgent(StoneColor color, KgsConnection pandanet) : base(color)
        {
            this._connection = pandanet;
        }

        protected override void WhenAssignedToGame()
        {
        }

      
        public override void PleaseMakeAMove()
        {
            // TODO  (future work) Petr: add request received flag, so we don't make moves before such is demanded
            // TODO  (future work) Petr: (sigh) this will be hard to debug, I guess
            MaybeMakeAMove();
        }

        private void MaybeMakeAMove()
        {
            int moveToMake = this.GameState.NumberOfMoves;
            if (this.GameInfo.HandicapPlacementType == Phases.HandicapPlacement.HandicapPlacementType.Fixed)
            {
                moveToMake--;
            }
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

        public override void GameInitialized()
        {
        }

        public override AgentType Type => AgentType.Remote;
        public override IllegalMoveHandling IllegalMoveHandling => IllegalMoveHandling.PermitItAnyway;
        public override void MoveIllegal(MoveResult moveResult)
        {
            throw new System.Exception(
                "We represent the server and our moves are inherently superior. What does the caller think he is, calling our moves 'illega'. Pche.");
        }

        public void StoreMove(int oneIndexMoveNumber, StoneColor color, Move move)
        {
            if (this.Color == color)
            {
                _storedMoves[oneIndexMoveNumber - 1] = move;
                MaybeMakeAMove();
            }
        }

        public override void MoveUndone()
        {
            var timeline = (GameState.GameTree.PrimaryTimeline.ToList());
            int latestStillExistingMove = timeline.Count - 1;
            foreach (var storedTuple in _storedMoves.ToList())
            {
                if (storedTuple.Key > latestStillExistingMove)
                {
                    _storedMoves.Remove(storedTuple.Key);
                }
            }
        }
    }
}