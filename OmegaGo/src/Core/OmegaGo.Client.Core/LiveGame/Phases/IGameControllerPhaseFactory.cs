using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    /// <summary>
    /// Provides factory for game controller's phases
    /// </summary>
    public interface IGameControllerPhaseFactory
    {
        /// <summary>
        /// Creates a given phase
        /// </summary>
        /// <param name="phaseType">Phase to create</param>
        /// <param name="controller">Controller to create the phase for</param>
        /// <returns>Phase</returns>
        IGamePhase CreatePhase(GamePhaseType phaseType, IGameController controller );
    }
}
