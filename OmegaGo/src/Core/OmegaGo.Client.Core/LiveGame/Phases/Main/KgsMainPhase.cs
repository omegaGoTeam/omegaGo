using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;

namespace OmegaGo.Core.LiveGame.Phases.Main
{
    class KgsMainPhase : MainPhaseBase
    {
        public KgsMainPhase(KgsGameController gameController) : base(gameController)
        {
        }

        protected override Task MainRequestUndo()
        {
            throw new NotImplementedException();
        }

        protected override void MainForceUndo()
        {
            throw new NotImplementedException();
        }

        protected override bool HandleLocalClockOut(GamePlayer player)
        {
            return false;
        }
    }
}
