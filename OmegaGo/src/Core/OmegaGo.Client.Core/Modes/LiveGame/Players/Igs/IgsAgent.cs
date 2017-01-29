﻿using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Igs
{
    public class IgsAgent : AgentBase
    {
        private readonly IgsConnection _pandanet;

        public IgsAgent(StoneColor color, IgsConnection pandanet) : base(color)
        {
            this._pandanet = pandanet;
        }

        protected override void WhenAssignedToGame()
        {
            _pandanet.IncomingMove += _pandanet_IncomingMove;
        }

        private Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();

        private void _pandanet_IncomingMove(object sender, System.Tuple<Online.OnlineGame, int, Move> e)
        {
            if (e.Item1.Info == this.GameInfo)
            {
                if (e.Item3.WhoMoves == this.Color)
                {
                    _storedMoves[e.Item2] = e.Item3;
                    MaybeMakeMove();
                }
            }
        }

        private void MaybeMakeMove()
        {
            int moveToMake = this.GameState.NumberOfMoves;
            if (_storedMoves.ContainsKey(moveToMake))
            {
                Move move = _storedMoves[moveToMake];
                if (move.Kind == MoveKind.PlaceStone)
                {
                    OnPlaceStone(move.Coordinates);
                }else if (move.Kind == MoveKind.Pass)
                {
                    OnPass();
                }
            }
        }

        public override void PleaseMakeAMove()
        {
            MaybeMakeMove();
        }

        public override void GameInitialized()
        {
        }

        public override AgentType Type => AgentType.Online;
        public override IllegalMoveHandling IllegalMoveHandling => IllegalMoveHandling.PermitItAnyway;
        public override void MoveIllegal(MoveResult move)
        {
            throw new System.Exception(
                "We represent the server and our moves are inherently superior. What does the caller think he is, calling our moves 'illega'. Pche.");
        }
    }
}