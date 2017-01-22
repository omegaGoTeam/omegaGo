using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    interface IGamePhase
    {
        /// <summary>
        /// Type of the phase
        /// </summary>
        GamePhaseType PhaseType { get; }

        /// <summary>
        /// Starts the phase operation, called by the controller after the phase is set
        /// </summary>
        void StartPhase();
    }
}
