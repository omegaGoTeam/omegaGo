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
    public class HumanAgent : AgentBase, IHumanAgentActions
    {
        public HumanAgent(StoneColor color) :
            base(color)
        {
        }

        public override AgentType Type => AgentType.Human;

        public override IllegalMoveHandling IllegalMoveHandling => IllegalMoveHandling.InformAgent;

        public override void MoveIllegal(MoveResult move)
        {
            PleaseMakeAMove();
        }

        void IHumanAgentActions.PlaceStone(Position selectedPosition)
        {
            OnPlaceStone(selectedPosition);
        }
        void IHumanAgentActions.Pass()
        {
            OnPass();
        }

        void IHumanAgentActions.Resign()
        {
            OnResign();
        }

        public override void PleaseMakeAMove()
        {
            MoveRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler MoveRequested;
    }
}
