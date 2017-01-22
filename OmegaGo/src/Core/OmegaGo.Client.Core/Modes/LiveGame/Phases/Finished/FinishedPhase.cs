using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    class FinishedPhase : GamePhaseBase, IFinishedPhase
    {
        public override GamePhaseType PhaseType => GamePhaseType.Finished;

        public FinishedPhase( IGameController gameController) : base( gameController )
        {

        }

        public override void StartPhase()
        {
            
        }
    }
}
