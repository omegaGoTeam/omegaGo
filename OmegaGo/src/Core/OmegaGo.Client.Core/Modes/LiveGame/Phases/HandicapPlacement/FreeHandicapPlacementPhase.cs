using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    class FreeHandicapPlacementPhase : HandicapPlacementPhaseBase
    {
        public FreeHandicapPlacementPhase(GameController gameController) : base(gameController)
        {
        }

        public override void StartPhase()
        {
            //TODO: IMPLEMENT
            GoToPhase( GamePhaseType.Main );
        }
    }
}
