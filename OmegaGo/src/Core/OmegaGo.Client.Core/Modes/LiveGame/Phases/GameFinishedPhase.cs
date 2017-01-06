using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    class GameFinishedPhase : GamePhaseBase, IGameFinishedPhase
    {
        public override GamePhaseType PhaseType => GamePhaseType.Finished;

        public GameFinishedPhase( IGameController gameController) : base( gameController )
        {

        }
    }
}
