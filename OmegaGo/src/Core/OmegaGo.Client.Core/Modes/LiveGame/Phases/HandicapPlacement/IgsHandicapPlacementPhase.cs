using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    internal class IgsHandicapPlacementPhase : GamePhaseBase, IHandicapPlacementPhase
    {
        public IgsHandicapPlacementPhase(GameController gameController) : base(gameController)
        {
        }

        public override GamePhaseType PhaseType  => GamePhaseType.HandicapPlacement;

        public override void StartPhase()
        {
            GoToPhase(GamePhaseType.Main);
        }
    }
}
