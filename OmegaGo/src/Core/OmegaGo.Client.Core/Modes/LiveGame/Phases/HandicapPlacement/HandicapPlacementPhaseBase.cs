using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    /// <summary>
    /// Base for handicap placement phase
    /// </summary>
    internal abstract class HandicapPlacementPhaseBase : GamePhaseBase, IHandicapPlacementPhase
    {
        protected HandicapPlacementPhaseBase(GameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Handicap placement phase
        /// </summary>
        public override GamePhaseType PhaseType => GamePhaseType.HandicapPlacement;
    }
}
