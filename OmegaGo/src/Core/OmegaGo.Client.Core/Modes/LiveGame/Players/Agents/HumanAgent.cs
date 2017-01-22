using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    public class HumanAgent : AgentBase
    {
        public HumanAgent(GamePlayer player) : base(player)
        {
        }

        public override void MovePerformed(Move move, GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public override void MoveIllegal(MoveResult move)
        {
            throw new NotImplementedException();
        }

        public override event EventHandler Move;
        public override IllegalMoveHandling IllegalMoveHandling { get; }
        public override void OnTurn()
        {
            throw new NotImplementedException();
        }

        public override void GameInitialized()
        {
            throw new NotImplementedException();
        }

        public override void GamePhaseChanged(GamePhaseType phase)
        {
            throw new NotImplementedException();
        }
    }
}
