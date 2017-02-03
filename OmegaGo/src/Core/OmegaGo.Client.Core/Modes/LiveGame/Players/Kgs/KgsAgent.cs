using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Igs
{
    public class KgsAgent : AgentBase
    {
        private readonly KgsConnection _connection;

        public KgsAgent(StoneColor color, KgsConnection pandanet) : base(color)
        {
            this._connection = pandanet;
        }

        protected override void WhenAssignedToGame()
        {
        }

      
        public override void PleaseMakeAMove()
        {
            // TODO add request received flag, so we don't make moves before such is demanded
            // TODO (sigh) this will be hard to debug, I guess
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